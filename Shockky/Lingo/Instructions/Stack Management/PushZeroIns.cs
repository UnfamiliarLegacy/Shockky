namespace Shockky.Lingo.Instructions
{
    public class PushZeroIns : Primitive
    {
        public PushZeroIns()
            : base(OPCode.PushInt0)
        {
            Value = 0;
        }

        public override int GetPushCount() => 1;
    }
}