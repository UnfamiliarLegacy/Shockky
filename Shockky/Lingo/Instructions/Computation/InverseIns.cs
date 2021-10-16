namespace Shockky.Lingo.Instructions
{
    public class InverseIns : Unary
    {
        public InverseIns()
            : base(OPCode.Inverse, UnaryOperatorKind.Minus)
        { }
    }
}