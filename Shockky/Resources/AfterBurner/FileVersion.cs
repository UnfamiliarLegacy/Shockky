using System.Text;

using Shockky.IO;

namespace Shockky.Resources
{
    public class FileVersion : Chunk
    {
        public DirectorVersion Version { get; set; }
        public string VersionString { get; set; }

        public FileVersion()
            : base(ResourceKind.Fver)
        { }
        public FileVersion(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            int version = input.ReadVarInt();
            if (version < 0x401) return;
            int unk1 = input.ReadVarInt();
            Version = (DirectorVersion)input.ReadVarInt();
            
            if (version < 0x501) return;
            VersionString = input.ReadString();
        }

        public override int GetBodySize()
        {
            throw new NotImplementedException();
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
