namespace Shockky.Lingo.Instructions
{
    public class PushIntIns : Primitive
    {
        public PushIntIns(LingoFunction function)
            : base(OPCode.PushInt, function)
        { }
        public PushIntIns(LingoFunction function, int value)
            : this(function)
        {
            Value = value;
        }
    }
}