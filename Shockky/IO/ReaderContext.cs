using Shockky.Resources;

namespace Shockky.IO;

public readonly ref struct ReaderContext
{
    public readonly DirectorVersion Version { get; }

    public ReaderContext(DirectorVersion version)
    {
        Version = version;
    }
}
