namespace Shockky.Lingo.Instructions
{
    public class IntoSpriteIns : Computation
    {
        public IntoSpriteIns()
            : base(OPCode.IntoSprite, BinaryOperatorKind.SpriteWithin)
        { }
    }
}