namespace Shockky.Lingo.Instructions
{
    public class NotIns : Unary
    {
        public NotIns() 
            : base(OPCode.Not, UnaryOperatorKind.Not)
        { }
    }
}