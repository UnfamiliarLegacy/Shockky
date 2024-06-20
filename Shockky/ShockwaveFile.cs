using System.Buffers;
using Shockky.IO;
using Shockky.Resources;
using Shockky.Resources.AfterBurner;
using Shockky.Resources.Enum;

namespace Shockky;

#nullable enable

public class ShockwaveFile
{
    public FileMetadata? Metadata { get; set; }
    public DirectorVersion Version { get; set; }

    public IDictionary<int, IResource> Resources { get; set; }

    public ShockwaveFile()
    {
        Resources = new Dictionary<int, IResource>();
    }

    public static async ValueTask<ShockwaveFile> Read(Stream stream, long streamLen)
    {
        var bufferLength = (int)streamLen;
        var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
        
        try
        {
            // Read stream in chunks of 4096 to buffer.
            const int chunkSize = 8192;
            
            var bufferMemory = buffer.AsMemory(0, bufferLength);
            var bytesRead = 0;
            
            while (bytesRead < bufferLength)
            {
                var read = await stream.ReadAsync(bufferMemory.Slice(bytesRead, Math.Min(chunkSize, bufferLength - bytesRead)));
                if (read == 0)
                    break;
                
                bytesRead += read;
            }

            return Read(buffer.AsSpan(0, bufferLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static ShockwaveFile Read(string path) => Read(File.ReadAllBytes(path));
    public static ShockwaveFile Read(ReadOnlySpan<byte> data)
    {
        var file = new ShockwaveFile();
        var input = new ShockwaveReader(data);

        file.Metadata = new FileMetadata(ref input);
        input.ReverseEndianness = file.Metadata.IsLittleEndian;

        if (file.Metadata.Codec is CodecKind.FGDM or CodecKind.FGDC)
        {
            if (IResource.Read(ref input, default) is not FileVersion fileVersion)
                throw new InvalidDataException();

            ReaderContext readerContext = new(fileVersion.Version);

            if (IResource.Read(ref input, readerContext) is not FileCompressionTypes compressionTypes)
                throw new InvalidDataException();

            if (IResource.Read(ref input, readerContext) is not AfterburnerMap afterburnerMap)
                throw new InvalidDataException();

            var fgeiHeader = new ResourceHeader(ref input);

            if (fgeiHeader.Kind is not OsType.FGEI)
                throw new InvalidDataException();

            file.Version = fileVersion.Version;
            file.Resources = FileGzipEmbeddedImage.ReadResources(ref input, readerContext, afterburnerMap, compressionTypes);
        }
        else if (file.Metadata.Codec is CodecKind.MV93)
        {
            if (IResource.Read(ref input, default) is not IndexMap initialMap)
                throw new InvalidDataException($"Failed to read {nameof(IndexMap)}");

            ReaderContext readerContext = new(initialMap.Version);

            input.Position = initialMap.MemoryMapOffset;

            if (IResource.Read(ref input, readerContext) is not MemoryMap memoryMap)
                throw new InvalidDataException($"Failed to read {nameof(MemoryMap)}.");

            for (int i = 1; i < memoryMap.Entries.Length; i++)
            {
                var entry = memoryMap.Entries[i];

                if (entry.Flags.HasFlag(ChunkEntryFlags.Invalid))
                    continue;

                input.Position = entry.Offset;
                file.Resources.Add(i, IResource.Read(ref input, readerContext));
            }
            
            file.Version = initialMap.Version;
        }

        return file;
    }
}
