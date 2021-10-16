namespace Shockky.Lingo.Instructions
{
    public class GetLocalIns : VariableReference
    {
        private int _localNameIndex;
        public int LocalNameIndex
        {
            get => _localNameIndex;
            set
            {
                _localNameIndex = value;
            }
        }

        public GetLocalIns(LingoFunction function)
            : base(OPCode.GetLocal, function)
        { }

        public GetLocalIns(LingoFunction function, int localIndex)
            : this(function)
        {
            LocalNameIndex = localIndex;
        }
    }
}