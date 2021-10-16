namespace Shockky.Lingo.Instructions
{
    public class ChunkExpressionIns : Instruction
    {
        public string Body { get; private set; }
        public string Type { get; private set; }

        public ChunkExpressionIns()
            : base(OPCode.ChunkExpression)
        { }

        public override int GetPopCount() => 9;
        public override int GetPushCount() => 1;
    }
}