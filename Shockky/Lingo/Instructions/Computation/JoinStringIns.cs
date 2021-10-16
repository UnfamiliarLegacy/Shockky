namespace Shockky.Lingo.Instructions
{
    public class JoinStringIns : Computation
    {
        public JoinStringIns()
            : base(OPCode.JoinString, BinaryOperatorKind.JoinString)
        { }
    }
}