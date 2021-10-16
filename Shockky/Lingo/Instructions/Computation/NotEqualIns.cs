namespace Shockky.Lingo.Instructions
{
    public class NotEqualIns : Computation
    {
        public NotEqualIns()
            : base(OPCode.NotEqual, BinaryOperatorKind.InEquality)
        { }
    }
}