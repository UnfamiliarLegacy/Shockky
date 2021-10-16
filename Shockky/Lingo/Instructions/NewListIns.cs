namespace Shockky.Lingo.Instructions
{
    public class NewListIns : Instruction
    {
        public int ItemCount => Value;
        public bool IsArgumentList { get; }

        public NewListIns(bool argList)
            : base(argList ? OPCode.NewArgList : OPCode.NewList)
        {
            IsArgumentList = argList;
        }
        public NewListIns(LingoFunction function, bool argList)
            : base(argList ? OPCode.NewArgList : OPCode.NewList, function)
        {
            IsArgumentList = argList;
        }
        public NewListIns(LingoFunction function, int itemCount, bool argList)
            : this(function, argList)
        {
            Value = itemCount;
        }

        public override int GetPopCount() => ItemCount;
        public override int GetPushCount() => 1;
    }
}