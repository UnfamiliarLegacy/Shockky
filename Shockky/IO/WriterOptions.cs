using Shockky.Resources;
using Shockky.Resources.Enum;

namespace Shockky.IO;

public readonly ref struct WriterOptions
{
    public DirectorVersion Version { get; }

    public WriterOptions(DirectorVersion version)
    {
        Version = version;
    }
}
