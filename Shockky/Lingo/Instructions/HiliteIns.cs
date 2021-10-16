namespace Shockky.Lingo.Instructions
{
    public class HiliteIns : Instruction
    {
        public HiliteIns()
            : base(OPCode.Hilite)
        { }

        public override int GetPopCount() => 9;
        public override int GetPushCount() => 1;
    }
}
 