namespace Shockky.Lingo.Instructions
{
    public abstract class Jumper : Instruction
    {
        public int Offset => Value;
        
        protected Jumper(OPCode op)
            : base(op)
        { }
        protected Jumper(OPCode op, LingoFunction function)
            : base(op, function)
        { }
        protected Jumper(OPCode op, LingoFunction function, int offset)
            : base(op, function)
        {
            Value = offset;
        }

        public static bool IsConditional(OPCode op) => (op == OPCode.IfFalse);

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.IfFalse:
                case OPCode.Jump:
                case OPCode.EndRepeat:
                    return true;
                default:
                    return false;
            }
        }
    }
}