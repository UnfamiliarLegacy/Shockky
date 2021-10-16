namespace Shockky.Lingo.Instructions
{
    public class DupIns : Instruction
    {
        public int Slot => Value;

        public DupIns(int slot)
            : base(OPCode.Dup)
        {
            Value = slot;
        }

        public override int GetPushCount() => 1;
    }
}
