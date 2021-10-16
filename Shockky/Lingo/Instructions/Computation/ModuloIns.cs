namespace Shockky.Lingo.Instructions
{
    public class ModuloIns : Computation
    {
        public ModuloIns()
            : base(OPCode.Modulo, BinaryOperatorKind.Modulo)
        { }
    }
}