namespace Shockky.Lingo.Instructions
{
    public class PopIns : Instruction
    {
        public PopIns(int popCount)
            : base(OPCode.Pop)
        {
            Value = popCount;
        }

        public override int GetPopCount() => Value;
    }
}
