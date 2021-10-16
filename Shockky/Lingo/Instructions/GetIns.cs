namespace Shockky.Lingo.Instructions
{
    public class GetIns : Instruction
    {
        public GetIns(LingoFunction function, int value)
            : base(OPCode.Get, function)
        {
            Value = value;
        }
    }
}