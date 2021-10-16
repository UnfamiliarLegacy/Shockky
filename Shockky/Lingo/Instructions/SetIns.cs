namespace Shockky.Lingo.Instructions
{
    public class SetIns : Instruction
    {
        public SetIns(LingoFunction function, int value) 
            : base(OPCode.Set, function)
        {
            Value = value;
        }
    }
}