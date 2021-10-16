namespace Shockky.Lingo.Instructions
{
    public class PushObjectIns : Instruction
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

        public PushObjectIns(LingoFunction function)
            : base(OPCode.PushObject, function)
        { }
        public PushObjectIns(LingoFunction function, int nameIndex)
            : this(function)
        {
            NameIndex = nameIndex;
        }
        public PushObjectIns(LingoFunction function, string name)
            : this(function)
        {
            Name = name;
        }

        public override int GetPushCount() => 1;
    }
}
