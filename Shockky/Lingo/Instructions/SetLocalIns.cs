namespace Shockky.Lingo.Instructions
{
    public class SetLocalIns : VariableAssignment
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

        public SetLocalIns(LingoFunction function)
            : base(OPCode.SetLocal, function)
        { }
        public SetLocalIns(LingoFunction function, int localIndex)
            : this(function)
        {
            LocalNameIndex = localIndex;
        }
        public SetLocalIns(LingoFunction function, string local)
            : this(function)
        {
            Name = local;
        }
    }
}