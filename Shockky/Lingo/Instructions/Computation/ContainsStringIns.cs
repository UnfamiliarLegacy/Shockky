namespace Shockky.Lingo.Instructions
{
    public class ContainsStringIns : Computation
    {
        public ContainsStringIns()
            : base(OPCode.ContainsString, BinaryOperatorKind.ContainsString)
        { }
    }
}