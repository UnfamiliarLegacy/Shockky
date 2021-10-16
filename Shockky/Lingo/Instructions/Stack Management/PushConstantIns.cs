namespace Shockky.Lingo.Instructions
{
    public class PushConstantIns : Primitive
    {
        private int _valueIndex;
        public int ValueIndex
        {
            get => _valueIndex;
            set
            {
                _valueIndex = value;
            }
        }

        public PushConstantIns(LingoFunction function)
            : base(OPCode.PushConstant, function)
        { }
        public PushConstantIns(LingoFunction function, int valueIndex)
            : this(function)
        {
            ValueIndex = valueIndex;
        }
        public PushConstantIns(LingoFunction function, object value)
            : this(function)
        {
            Value = value;
        }
    }
}