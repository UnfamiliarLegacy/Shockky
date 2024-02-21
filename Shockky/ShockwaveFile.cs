using Shockky.IO;
using Shockky.Resources;

namespace Shockky;

#nullable enable
public class ShockwaveFile
{
    public FileMetadata? Metadata { get; set; }
    public DirectorVersion Version { get; set; }

    public IDictionary<int, IResource> Resources { get; set; }

    public ShockwaveFile()
    {
        Version = DirectorVersion.Unknown;
        Resources = new Dictionary<int, IResource>();
    }

    public void Load(ReadOnlySpan<byte> data)
    {
        var input = new ShockwaveReader(data);
        Metadata = new FileMetadata(ref input);
        input.IsBigEndian = Metadata.IsBigEndian;

        if (Metadata.Codec is CodecKind.FGDM or CodecKind.FGDC)
        {
            if (IResource.Read(ref input, default) is FileVersion fileVersion)
            {
                ReaderContext readerContext = new(fileVersion.Version);

                if (IResource.Read(ref input, readerContext) is FileCompressionTypes compressionTypes &&
                    IResource.Read(ref input, readerContext) is AfterburnerMap afterburnerMap)
                {
                    var fgeiHeader = new ResourceHeader(ref input);
                    Resources = FileGzipEmbeddedImage.ReadResources(ref input, readerContext, afterburnerMap);
                }
            }
        }
        else if (Metadata.Codec is CodecKind.MV93)
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
                Resources.Add(i, IResource.Read(ref input, readerContext));
            }
        }
    }

    public void Assemble()
    {
        throw new NotImplementedException();
    }

    public static ShockwaveFile Load(string path)
    {
        ReadOnlySpan<byte> data = File.ReadAllBytes(path);
        var shockwaveFile = new ShockwaveFile();
        shockwaveFile.Load(data);

        return shockwaveFile;
    }
}
