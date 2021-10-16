using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Shockky.Resources;
using Shockky.Resources.Cast;
using Shockky.Resources.Types;

using System.CommandLine;
using System.CommandLine.Invocation;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Shockky.Sandbox
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.Title = "Shockky.Sandbox";

            //TODO: Verbose and Quiet levels, and rest of the resources of course
            var rootCommand = new RootCommand()
            {
                new Argument<IEnumerable<System.IO.FileInfo>>("input")
                {
                    Arity = ArgumentArity.OneOrMore,
                    Description = "Director movie (.dir, .dxt, .dcr) or external cast (.cst, .cxt, .cct) file(s)."
                }.ExistingOnly(),

                new Option<DirectoryInfo>("--output",
                    getDefaultValue: () => new DirectoryInfo("Output/"),
                    description: "Directory for the extracted resources")
                .LegalFilePathsOnly()
            };
            rootCommand.Handler = CommandHandler.Create<IEnumerable<System.IO.FileInfo>, bool, DirectoryInfo>(HandleExtractCommand);

            return rootCommand.Invoke(args);
        }

        private static IReadOnlyDictionary<int, System.Drawing.Color[]> ReadPalettes()
        {
            static System.Drawing.Color[] ReadPalette(string fileName)
            {
                using var fs = File.OpenRead(fileName);
                using var input = new BinaryReader(fs, Encoding.ASCII);

                input.ReadChars(4);
                input.ReadInt32();

                input.ReadChars(4);

                input.ReadChars(4);
                input.ReadInt32();
                input.ReadInt16();

                System.Drawing.Color[] colors = new System.Drawing.Color[input.ReadInt16()];
                for (int i = 0; i < colors.Length; i++)
                {
                    byte r = input.ReadByte();
                    byte g = input.ReadByte();
                    byte b = input.ReadByte();

                    colors[i] = System.Drawing.Color.FromArgb(r, g, b);

                    input.ReadByte();
                }
                return colors;
            }

            return new Dictionary<int, System.Drawing.Color[]>
            {
                { -1, ReadPalette("Palettes/mac.pal") },
                { -2, ReadPalette("Palettes/rainbow.pal") },
                { -3, ReadPalette("Palettes/grey.pal") },
                { -4, ReadPalette("Palettes/pastels.pal") },
                { -5, ReadPalette("Palettes/vivid.pal") },
                { -6, ReadPalette("Palettes/ntsc.pal") },
                { -7, ReadPalette("Palettes/metallic.pal") },
                { -8, ReadPalette("Palettes/web216.pal") },
                { -9, null }, //TODO: "Palettes/VGA.pal"
                { -101, ReadPalette("Palettes/windir4.pal") },
                { -102, ReadPalette("Palettes/win.pal") }
            };
        }

        private static int HandleExtractCommand(IEnumerable<System.IO.FileInfo> input, bool images, DirectoryInfo output)
        {
            output.Create();

            //Load the built-in system palettes
            IReadOnlyDictionary<int, System.Drawing.Color[]> systemPalettes = ReadPalettes();

            foreach (var file in input)
            {
                //TODO: Seperate different resource types.
                DirectoryInfo fileOutputDirectory = output.CreateSubdirectory(file.Name);

                Console.Write($"Disassembling file \"{file.Name}\"..");

                var shockwaveFile = ShockwaveFile.Load(file.FullName);

                if (shockwaveFile.Chunks.Values
                    .FirstOrDefault(c => c.Kind == ResourceKind.KEYPtr) is not AssociationTable associationTable)
                {
                    Console.WriteLine(nameof(AssociationTable) + " was not found!");
                    continue;
                }
                if (shockwaveFile.Chunks.Values
                    .FirstOrDefault(c => c.Kind == ResourceKind.CASPtr) is not CastAssociationTable castAssociationTable)
                {
                    Console.WriteLine(nameof(CastAssociationTable) + " was not found!");
                    continue;
                }
                Console.Write("Extracting bitmaps..");

                //TODO: This is currently just hacks all around. Shockky should have these lookups abstracted away.
                foreach ((ResourceId resourceId, int index) in associationTable.ResourceMap)
                {
                    if (resourceId.Kind != ResourceKind.BITD) continue;

                    var member = shockwaveFile[resourceId.Id] as CastMemberProperties;
                    var bitmapData = shockwaveFile[index] as BitmapData;

                    if (member?.Properties is not BitmapCastProperties bitmapProperties)
                        continue;

                    //TODO: external castlibs
                    if (bitmapProperties.PaletteRef.CastLib > 0)
                        continue;

                    if (bitmapProperties.Rectangle.IsEmpty)
                        continue;

                    string outputFileName = CoerceValidFileName(member?.Metadata?.Name ?? "member-" + resourceId.Id);

                    if (bitmapProperties.PaletteRef.MemberNum > 0 && bitmapProperties.PaletteRef.MemberNum < castAssociationTable.Members.Length)
                    {
                        continue;
                        //TODO:

                        //int paletteOwnerIndex = castAssociationTable.Members[bitmapProperties.PaletteRef.MemberNum];
                        //if (shockwaveFile[paletteOwnerIndex] is not CastMemberProperties paletteMember)
                        //    continue;
                        //
                        //var paletterResId = new ResourceId(ResourceKind.CLUT, paletteOwnerIndex);
                        //
                        //if (associationTable.ResourceMap.TryGetValue(paletterResId, out int paletteChunkIndex))
                        //{
                        //    var palette = shockwaveFile[paletteChunkIndex] as Palette;
                        //    bitmapData.PopulateMedia(bitmapProperties);
                        //    if (!TryExtractBitmapResource(fileOutputDirectory, outputFileName, bitmapData, palette.Colors))
                        //        continue;
                        //}
                    }
                    else if (systemPalettes.TryGetValue(bitmapProperties.PaletteRef.MemberNum - 1, out System.Drawing.Color[] palette))
                    {
                        bitmapData.PopulateMedia(bitmapProperties);
                        if (!TryExtractBitmapResource(fileOutputDirectory, outputFileName, bitmapData, palette))
                            continue;
                    }
                    Console.WriteLine($"({bitmapProperties.PaletteRef.CastLib}, {bitmapProperties.PaletteRef.MemberNum}) {member.Metadata?.Name}:");
                    Console.WriteLine($"    BitDepth: {bitmapProperties.BitDepth}");
                }
                Console.WriteLine();
                Console.WriteLine("Done!");
            }
            return 0;
        }

        //TODO: Can we use ImageSharp's PixelOperations here?
        private static bool TryExtractBitmapResource(DirectoryInfo outputDirectory, string name, BitmapData bitmapData, System.Drawing.Color[] palette)
        {
            //TODO: Properly render flags etc.
            Span<byte> buffer = bitmapData.Data.AsSpan();

            int width = bitmapData.Width < bitmapData.Stride ? bitmapData.Width : bitmapData.Stride;

            using var image = new Image<Rgba32>(bitmapData.Width, bitmapData.Height);
            for (int y = 0; y < bitmapData.Height; y++)
            {
                Span<byte> row = buffer.Slice(y * bitmapData.Stride, bitmapData.Stride);

                if (bitmapData.BitDepth == 32) //TODO: Can't get this right yet, probably wrong PixelFormat
                {
                    return false;

                    //Span<Bgra32> pixels = MemoryMarshal.Cast<byte, Bgra32>(row);
                    //
                    //for (int x = 0; x < bitmap.Width; x++)
                    //{
                    //    image[x, y] = pixels[x];
                    //}
                }
                else if (bitmapData.BitDepth == 16)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pixelColor = MemoryMarshal.Read<short>(row.Slice(x*2));
                        byte r = (byte)((pixelColor & 0b111100000000) >> 8);
                        byte g = (byte)((pixelColor & 0b000011110000) >> 4);
                        byte b = (byte)(pixelColor  & 0b000000001111);
                        image[x, y] = new Rgba32(r, g, b);
                    }
                }
                else if (bitmapData.BitDepth == 8)
                {
                    for (int x = 0; x < width; x++)
                    {
                        System.Drawing.Color pixelColor = palette[row[x]];
                        image[x, y] = new Rgba32(pixelColor.R, pixelColor.G, pixelColor.B);
                    }
                }
                else if (bitmapData.BitDepth == 4)
                {
                    return false;

                    //for (int x = 0; x < width; x++)
                    //{
                    //    System.Drawing.Color pixelColor = palette[row[x] >> 4];
                    //    System.Drawing.Color secondPixelColor = palette[row[x] & 0xF];
                    //
                    //    //image[x, y] = new Bgra32(pixelColor.R, pixelColor.G, pixelColor.B);
                    //}
                }
                else if (bitmapData.BitDepth == 1)
                {
                    for (int x = 0; x < bitmapData.Width; )
                    {
                        for (int c = 0; c < 8 && x < bitmapData.Width; c++, x++)
                        {
                            int p = row[x / 8] & (1 << (7 - c));
                            image[x, y] = new Rgba32(p, p, p);
                        }
                    }
                }
            }

            using var fs = File.Create(Path.Combine(outputDirectory.FullName, name + ".png"));
            image.SaveAsPng(fs);

            return true;
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string CoerceValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }
    }
}