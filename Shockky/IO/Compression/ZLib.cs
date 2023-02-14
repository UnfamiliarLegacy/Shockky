using System.IO.Compression;

namespace Shockky.IO;

internal static class ZLib
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
        fixed (byte* inputPtr = &input[0])
        {
            using var stream = new UnmanagedMemoryStream(inputPtr, input.Length);
            using var zlibStream = new ZLibStream(stream, CompressionMode.Decompress);

            int totalRead = 0;
            while (totalRead < output.Length)
            {
                int bytesRead = zlibStream.Read(output.Slice(totalRead));
                if (bytesRead == 0) break;
                totalRead += bytesRead;
            }
            return totalRead;
        }
    }
}
