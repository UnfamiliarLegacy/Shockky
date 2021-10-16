using System.IO.Compression;

namespace Shockky.IO
{
    public static class ZLib
    {
        //           🙏 Summoning circle 🙏 
        //
        //             🕯       🕯       🕯
        //     🕯                               🕯
        // https://github.com/dotnet/runtime/issues/39327
        // 🕯                                       🕯 
        //            
        //     🕯                               🕯
        //             🕯       🕯       🕯
        internal static unsafe int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
        {
            fixed (byte* pBuffer = &input.Slice(2)[0]) //Skip ZLib header
            {
                using var stream = new UnmanagedMemoryStream(pBuffer, input.Length);
                using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);

                return deflateStream.Read(output);
            }
        }
    }
}
