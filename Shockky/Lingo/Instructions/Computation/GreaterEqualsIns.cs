namespace Shockky.Lingo.Instructions
{
    public class GreaterEqualsIns : Computation
    {
        public GreaterEqualsIns()
            : base(OPCode.GreaterThanEquals, BinaryOperatorKind.GreaterThanOrEqual)
        { }
    }
}