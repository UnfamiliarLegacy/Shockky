namespace Shockky.Lingo.Instructions
{
    public abstract class Call : Instruction
    {
        private string _targetFunction;
        public string TargetFunction
        {
            get => _targetFunction;
            set
            {
                _targetFunction = value;
            }
        }

        public bool IsObjectCall { get; protected set; }

        protected Call(OPCode op)
            : base(op)
        { }
        protected Call(OPCode op, LingoFunction function)
            : base(op, function)
        { }
    }
}
