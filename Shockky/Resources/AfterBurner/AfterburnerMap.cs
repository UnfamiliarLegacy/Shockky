using Shockky.IO;

namespace Shockky.Resources
{
    public unsafe class AfterburnerMap : Chunk
    {
        public int LastIndex { get; set; }
        public AfterBurnerMapEntry[] Entries { get; set; }

        public AfterburnerMap()
            : base(ResourceKind.ABMP)
        { }
        public AfterburnerMap(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            byte zero = input.ReadByte();
            Remnants.Enqueue(input.ReadVarInt());

            using DeflateShockwaveReader deflaterInput = CreateDeflateReader(ref input);
            Remnants.Enqueue(deflaterInput.ReadVarInt()); //TODO: 1 - maybe the "initial" chunk index.
            LastIndex = deflaterInput.ReadVarInt();

            Entries = new AfterBurnerMapEntry[deflaterInput.ReadVarInt()];
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new AfterBurnerMapEntry(deflaterInput);
            }
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.Write((byte)0);
            output.WriteVarInt((int)Remnants.Dequeue());
            //TODO: Wrap dat compressor
            output.WriteVarInt((int)Remnants.Dequeue());
            output.WriteVarInt(LastIndex);

            output.WriteVarInt(Entries.Length);
            foreach (var entry in Entries)
            {
                entry.WriteTo(output);
            }
        }

        public override int GetBodySize()
        {
            throw new System.NotImplementedException();
            int size = 0;
            size += sizeof(byte);
            return size;
        }
    }
}
