namespace Shockky.Lingo.Instructions
{
    public class PushFloatIns : Primitive
    {
        public PushFloatIns(LingoFunction function)
            : base(OPCode.PushFloat, function)
        { }
        public PushFloatIns(LingoFunction function, int value)
            : this(function)
        {
            Value = BitConverter.Int32BitsToSingle(value);
        }
        public PushFloatIns(LingoFunction function, float value)
            : this(function)
        {
            Value = value;
        }
    }
}
