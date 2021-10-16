namespace Shockky.Lingo.Instructions
{
    public class AddIns : Computation
    {
        public AddIns()
            : base(OPCode.Add, BinaryOperatorKind.Add)
        { }
    }
}