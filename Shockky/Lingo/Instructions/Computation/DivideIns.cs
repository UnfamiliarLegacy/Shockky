namespace Shockky.Lingo.Instructions
{
    public class DivideIns : Computation
    {
        public DivideIns()
            : base(OPCode.Divide, BinaryOperatorKind.Divide)
        { }
    }
}