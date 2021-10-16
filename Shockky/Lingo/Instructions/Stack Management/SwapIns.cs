namespace Shockky.Lingo.Instructions
{
    public class SwapIns : Instruction
    {
        public SwapIns()
            : base(OPCode.Swap)
        { }

        public override int GetPopCount() => 2;
        public override int GetPushCount() => 2;
    }
}
