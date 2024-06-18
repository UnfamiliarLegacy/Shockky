﻿using Shockky.IO;
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
        Resources = new Dictionary<int, IResource>();
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
        }

        return file;
    }
}
