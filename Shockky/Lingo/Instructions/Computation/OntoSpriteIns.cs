namespace Shockky.Lingo.Instructions
{
    public class OntoSpriteIns : Computation
    {
        public OntoSpriteIns()
            : base(OPCode.OntoSprite, BinaryOperatorKind.SpriteIntersects)
        { }
    }
}
