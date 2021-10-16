namespace Shockky.Lingo.Instructions
{
    public class InsertIns : Instruction
    {
        public InsertIns(LingoFunction function) 
            : base(OPCode.Insert, function)
        { }
        public InsertIns(LingoFunction function, int value)
            : this(function)
        {
            Value = value;
        }

        public override int GetPopCount() => 10;
    }
}
