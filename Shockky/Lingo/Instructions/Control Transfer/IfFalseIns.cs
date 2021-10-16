namespace Shockky.Lingo.Instructions
{
    public class IfFalseIns : Jumper
    {
        public IfFalseIns()
            : base(OPCode.IfFalse)
        { }
        public IfFalseIns(LingoFunction function, int offset)
            : base(OPCode.IfFalse, function, offset)
        { }
    }
}