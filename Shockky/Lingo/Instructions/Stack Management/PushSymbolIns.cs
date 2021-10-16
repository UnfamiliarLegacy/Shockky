namespace Shockky.Lingo.Instructions
{
    public class PushSymbolIns : Instruction
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        private int _nameIndex;
        public int NameIndex
        {
            get => _nameIndex;
            set
            {
                base.Value = value;
                _nameIndex = value;
            }
        }

        public PushSymbolIns(LingoFunction function)
            : base(OPCode.PushSymbol, function)
        { }
        public PushSymbolIns(LingoFunction function, int nameIndex)
            : this(function)
        {
            NameIndex = nameIndex;
        }
        public PushSymbolIns(LingoFunction function, string name)
            : this(function)
        {
            Name = name;
        }

        public override int GetPushCount() => 1;
    }
}