namespace Shockky.Lingo.Instructions
{
    public class GreaterThanIns : Computation
    {
        public GreaterThanIns()
            : base(OPCode.GreaterThan, BinaryOperatorKind.GreaterThan)
        { }
    }
}