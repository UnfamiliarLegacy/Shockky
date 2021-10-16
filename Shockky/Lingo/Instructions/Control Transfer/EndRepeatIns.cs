namespace Shockky.Lingo.Instructions
{
    public class EndRepeatIns : Jumper
    {
        public EndRepeatIns(LingoFunction function, int offset) 
            : base(OPCode.EndRepeat, function, offset)
        { }
    }
}