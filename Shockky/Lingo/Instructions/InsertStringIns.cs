namespace Shockky.Lingo.Instructions
{
    public class InsertStringIns : Instruction
    {
        public InsertStringIns(LingoFunction function)
            : base(OPCode.InsertString, function)
        { }
        public InsertStringIns(LingoFunction function, int value)
            : this(function)
        {
            Value = value;
        }

        public override int GetPopCount() => 2;
    }
}
