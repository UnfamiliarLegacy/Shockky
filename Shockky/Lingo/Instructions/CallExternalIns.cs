namespace Shockky.Lingo.Instructions
{
    public class CallExternalIns : Call
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

        public CallExternalIns(LingoFunction function)
            : base(OPCode.CallExternal, function)
        { }
        public CallExternalIns(LingoFunction function, int externalFunctionNameIndex)
            : this(function)
        {
            FunctionNameIndex = externalFunctionNameIndex;
        }
    }
}