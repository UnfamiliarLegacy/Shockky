namespace Shockky.Lingo.Instructions
{
    public class MultipleIns : Computation
    {
        public MultipleIns()
            : base(OPCode.Multiple, BinaryOperatorKind.Multiply)
        { }
    }
}