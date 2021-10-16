namespace Shockky.Lingo.Instructions
{
    public class EqualsIns : Computation
    {
        public EqualsIns()
            : base(OPCode.Equals, BinaryOperatorKind.Equality)
        { }
    }
}