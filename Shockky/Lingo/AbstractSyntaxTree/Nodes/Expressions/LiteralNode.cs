using Shockky.Lingo.Enums;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class LiteralNode : ExprNode
{
    public LiteralNode(Datum datum) : base(NodeType.kLiteralNode)
    {
        Value = datum;
    }

    public LiteralNode(LingoLiteral literal) : base(NodeType.kLiteralNode)
    {
        Value = literal.Kind switch
        {
            VariantKind.String => new Datum(DatumType.kDatumString, (string) literal.Value),
            VariantKind.Integer => new Datum((int) literal.Value),
            VariantKind.Float => new Datum((double) literal.Value),
            VariantKind.Null => new Datum(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public Datum Value { get; }

    public override Datum GetValue()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"{nameof(Value)}: {Value}";
    }
}