namespace Shockky.Lingo.Instructions
{
    public class JumpIns : Jumper
    {
        public JumpIns(LingoFunction function)
            : base(OPCode.Jump, function)
        { }
        public JumpIns(LingoFunction function, int offset)
            : this(function)
        {
            Value = offset;
        }
    }
}