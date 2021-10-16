namespace Shockky.Lingo.Instructions
{
    public class JoinPadStringIns : Computation
    {
        public JoinPadStringIns() 
            : base(OPCode.JoinPadString, BinaryOperatorKind.JoinPadString)
        { }
    }
}