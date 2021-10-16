namespace Shockky.Lingo.Instructions
{
    public abstract class VariableReference : Instruction
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

        public bool IsMoviePropertyReference { get; protected set; }
        public bool IsObjectReference { get; protected set; }
        public bool IsMovieReference { get; protected set; }

        protected VariableReference(OPCode op, LingoFunction function)
            : base(op, function)
        { }
        protected VariableReference(OPCode op)
            : base(op)
        { }

        public override int GetPushCount() => 1;
    }
}
