namespace Shockky.Lingo.Instructions
{
    public class SubtractIns : Computation
    {
        public SubtractIns() 
            : base(OPCode.Substract, BinaryOperatorKind.Subtract)
        { }
    }
}