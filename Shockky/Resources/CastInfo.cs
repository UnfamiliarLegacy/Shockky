using Shockky.IO;

namespace Shockky.Resources
{
    public class CastInfo : Chunk
    {
        public CastInfo()
            : base(ResourceKind.Cinf)
        { }
        public CastInfo(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            //Single VList

            byte[] data = input.ReadBytes(header.Length).ToArray();
            File.WriteAllBytes($"Cinf-{header.Length}.bin", data);
            
            
            /*
             * # d3 variant
		     entry = movie.CastInfo()
		     strings, unk2, unk3, entryType = self.parseSubstrings(data)
		     assert len(strings) == 5
		     entry.script = strings[0]
		     entry.name = getString(strings[1])
		     entry.extDirectory = getString(strings[2])
		     entry.extFilename = getString(strings[3])
		     entry.extType = strings[4]

		     print "VWCI: id %d, type %d, name %s, script %s, unk %08x/%08x" % (data.rid, entryType, repr(entry.name), repr(entry.script), unk2, unk3)
		     if entry.extDirectory or entry.extFilename or entry.extType:
		     	print " file %s/%s(%s)" % (repr(entry.extDirectory), repr(entry.extFilename), repr(entry.extType))
		     self.movie.castInfo[data.rid] = entry
            */
        }

        public override int GetBodySize()
        {
            throw new System.NotImplementedException();
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            throw new System.NotImplementedException();
        }
    }
}
