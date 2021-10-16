namespace Shockky.Lingo.Instructions
{
    public class StartTellIns : Instruction
    {
        public StartTellIns()
            : base(OPCode.StartTell)
        { }

        public override int GetPopCount() => 1;
    }
}