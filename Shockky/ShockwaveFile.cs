using Shockky.IO;
using Shockky.Resources;

namespace Shockky
{
    //TODO: RIFF Container abstraction coming soon
    public class ShockwaveFile
    {
        public IDictionary<int, Chunk> Chunks { get; set; }

        public DirectorVersion Version { get; set; }
        public FileMetadata Metadata { get; set; }

#nullable enable
        public Chunk? this[int id]
        {
            get
            {
                Chunks.TryGetValue(id, out var chunk);
                return chunk;
            } 
        }

        public ShockwaveFile()
        {
            Chunks = new Dictionary<int, Chunk>();
        }

        public void Load(ReadOnlySpan<byte> data)
        {
            var input = new ShockwaveReader(data);
            Metadata = new FileMetadata(ref input);
            input.IsBigEndian = Metadata.IsBigEndian;

            if (Metadata.Codec is CodecKind.FGDM or CodecKind.FGDC)
            {
                if (Chunk.Read(ref input) is FileVersion version &&
                    Chunk.Read(ref input) is FileCompressionTypes compressionTypes &&
                    Chunk.Read(ref input) is AfterburnerMap afterburnerMap &&
                    Chunk.Read(ref input) is FileGzipEmbeddedImage fgei)
                {
                    Chunks = fgei.ReadChunks(ref input, afterburnerMap.Entries);
                }
            }
            else if (Metadata.Codec == CodecKind.MV93)
            {
                if (Chunk.Read(ref input) is not InitialMap initialMap)
                    throw new InvalidDataException($"Failed to read {nameof(InitialMap)}");

                Version = initialMap.Version;

                foreach (int offset in initialMap.MemoryMapOffsets)
                {
                    input.Position = offset;

                    if (Chunk.Read(ref input) is not MemoryMap memoryMap)
                        throw new InvalidDataException($"Failed to read {nameof(MemoryMap)} at offset {offset}.");

                    for (int i = 1; i < memoryMap.Entries.Length; i++)
                    {
                        var entry = memoryMap.Entries[i];

                        if (entry.Flags.HasFlag(ChunkEntryFlags.Invalid))
                            continue;

                        input.Position = entry.Offset;
                        Chunks.Add(i, Chunk.Read(ref input));
                    }
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
}
