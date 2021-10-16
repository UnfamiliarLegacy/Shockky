namespace Shockky.Lingo.Instructions
{
    public class CallLocalIns : Call
    {
        private int _handlerIndex;
        public int HandlerIndex
        {
            get => _handlerIndex;
            set
            {
                _handlerIndex = value;
            }
        }

        public CallLocalIns(LingoFunction function)
            : base(OPCode.CallLocal, function)
        { }
        public CallLocalIns(LingoFunction function, int handlerIndex)
            : this(function)
        {
            HandlerIndex = handlerIndex;
        }
    }
}