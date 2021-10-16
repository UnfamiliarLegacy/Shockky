using System.Drawing;

using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources
{
    //TODO: v5 = memberNum or MemberId
    public record Tile(CastMemberId Id, Rectangle Rect);

    public class Tiles : Chunk
    {
        public Tile[] Items { get; }

        public Tiles()
            : base(ResourceKind.VWTL)
        {
            Items = new Tile[8];
        }
        public Tiles(ref ShockwaveReader input)
            : this()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                input.ReadInt32();
                input.ReadInt32();

                Items[i] = new Tile(
                    Id: new(input.ReadInt16(), input.ReadInt16()),
                    Rect: input.ReadRect());
            }
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
