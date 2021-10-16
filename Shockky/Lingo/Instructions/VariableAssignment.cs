namespace Shockky.Lingo.Instructions
{
    public abstract class VariableAssignment : VariableReference
    {
        protected VariableAssignment(OPCode op, LingoFunction function)
            : base(op, function)
        { }
        protected VariableAssignment(OPCode op)
            : base(op)
        { }

        public override int GetPopCount() => 1;
    }
}
