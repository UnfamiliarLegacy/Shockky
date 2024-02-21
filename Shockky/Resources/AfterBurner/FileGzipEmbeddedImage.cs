using Shockky.IO;
using System.Buffers;

namespace Shockky.Resources;

public static class FileGzipEmbeddedImage
{
    // TODO: Tidy up more.
    public static IDictionary<int, IResource> ReadResources(ref ShockwaveReader input, ReaderContext context, AfterburnerMap afterburnerMap)
    {
        int chunkStart = input.Position;
        var chunks = new Dictionary<int, IResource>(afterburnerMap.Entries.Count);

        ReadInitialLoadSegment(ref input, context, afterburnerMap, chunks);

        foreach ((int index, AfterburnerMapEntry entry) in afterburnerMap.Entries)
        {
            if (entry.Offset < 1) continue; //TODO: ILS always at offset 0?

            input.Position = chunkStart + entry.Offset;
            chunks.Add(index, IResource.Read(ref input, context, entry));
        }
        return chunks;
    }

    private static OperationStatus ReadInitialLoadSegment(ref ShockwaveReader input, ReaderContext context, AfterburnerMap afterburnerMap, Dictionary<int, IResource> chunks)
    {
        // First entry in the AfterburnerMap must be ILS.
        AfterburnerMapEntry ilsEntry = afterburnerMap.Entries.First().Value;
        input.Advance(ilsEntry.Offset);

        // TODO: this shouldn't be here
        ReadOnlySpan<byte> compressedData = input.ReadBytes(ilsEntry.Length);

        Span<byte> decompressedData = ilsEntry.DecompressedLength <= 1024 ?
                stackalloc byte[1024].Slice(0, ilsEntry.DecompressedLength) : new byte[ilsEntry.DecompressedLength];

        ZLib.Decompress(compressedData, decompressedData);

        var ilsReader = new ShockwaveReader(decompressedData, input.IsBigEndian);

        while (ilsReader.IsDataAvailable)
        {
            int index = ilsReader.ReadVarInt();

            if (index < 1 || index > afterburnerMap.LastIndex) 
                return OperationStatus.InvalidData;

            AfterburnerMapEntry entry = afterburnerMap.Entries[index];
            chunks.Add(index, IResource.Read(ref ilsReader, context, entry.Kind, entry.DecompressedLength));
        }
        return OperationStatus.Done;
    }
}
