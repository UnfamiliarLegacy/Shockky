namespace Shockky.Lingo.Instructions
{
    public class StartsWithIns : Computation
    {
        public StartsWithIns() 
            : base(OPCode.StartsWith, BinaryOperatorKind.StartsWith)
        { }
    }
}