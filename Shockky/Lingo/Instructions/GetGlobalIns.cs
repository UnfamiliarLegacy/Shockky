namespace Shockky.Lingo.Instructions
{
    public class GetGlobalIns : VariableReference
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

        public GetGlobalIns(LingoFunction function)
            : base(OPCode.GetGlobal, function)
        { }
        public GetGlobalIns(LingoFunction function, int globalNameIndex)
            : this(function)
        {
            ValueIndex = globalNameIndex;
        }
        public GetGlobalIns(LingoFunction function, string global)
            : this(function)
        {
            Name = global;
        }
    }
}