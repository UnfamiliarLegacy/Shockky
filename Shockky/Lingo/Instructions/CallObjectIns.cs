namespace Shockky.Lingo.Instructions
{
    public class CallObjectIns : Call
    {
        private int _functionNameIndex;
        public int FunctionNameIndex
        {
            get => _functionNameIndex;
            set
            {
                _functionNameIndex = value;
            }
        }

        public CallObjectIns(LingoFunction function)
            : base(OPCode.CallObj, function)
        {
            IsObjectCall = true;
        }
        public CallObjectIns(LingoFunction function, int handlerNameIndex)
            : this(function)
        {
            FunctionNameIndex = handlerNameIndex;
        }
    }
}