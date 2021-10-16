namespace Shockky.Lingo.Instructions
{
    public class SetGlobalIns : VariableAssignment
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

        public SetGlobalIns(LingoFunction function)
            : base(OPCode.SetGlobal, function)
        { }
        public SetGlobalIns(LingoFunction function, int globalValueIndex)
            : this(function)
        {
            ValueIndex = globalValueIndex;
        }
        public SetGlobalIns(LingoFunction function, string global)
            : this(function)
        {
            Name = global;
        }

        public override int GetPopCount() => 1;
    }
}