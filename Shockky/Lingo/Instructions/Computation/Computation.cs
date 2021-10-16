namespace Shockky.Lingo.Instructions
{
    public abstract class Computation : Instruction
    {
        public BinaryOperatorKind Kind { get; }

        protected Computation(OPCode op)
            : this(op, BinaryOperatorKind.Unknown)
        { }
        protected Computation(OPCode op, BinaryOperatorKind kind)
            : base(op)
        {
            Kind = kind;
        }

        public override int GetPopCount() => 2;
        public override int GetPushCount() => 1;

        public static bool IsValid(OPCode op)
        {
            return op switch
            {
                OPCode.Multiple or OPCode.Divide or 
                OPCode.Add or OPCode.Substract or 
                OPCode.Modulo or 
                OPCode.JoinString or OPCode.JoinPadString or
                OPCode.LessThan or OPCode.LessThanEquals or
                OPCode.NotEqual or OPCode.Equals or
                OPCode.GreaterThan or OPCode.GreaterThanEquals or
                OPCode.And or OPCode.Or or
                OPCode.ContainsString or OPCode.StartsWith or
                OPCode.OntoSprite or OPCode.IntoSprite => true,
                _ => false,
            };
        }
    }
}