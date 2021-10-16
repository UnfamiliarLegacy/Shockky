namespace Shockky.Lingo.Instructions
{
    public class OrIns : Computation
    {
        public OrIns()
            : base(OPCode.Or, BinaryOperatorKind.Or)
        { }
    }
}