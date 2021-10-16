namespace Shockky.Lingo.Instructions
{
    public class LessThanIns : Computation
    {
        public LessThanIns()
            : base(OPCode.LessThan, BinaryOperatorKind.LessThan)
        { }
    }
}