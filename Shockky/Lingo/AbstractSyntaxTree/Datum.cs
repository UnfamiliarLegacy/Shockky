using System.Text;
using Shockky.Lingo.AbstractSyntaxTree.Nodes;

namespace Shockky.Lingo.AbstractSyntaxTree;

public class Datum
{
    public Datum()
    {
        Type = DatumType.kDatumVoid;
    }
    
    public Datum(int val)
    {
        Type = DatumType.kDatumInt;
        I = val;
    }
    
    public Datum(double val)
    {
        Type = DatumType.kDatumFloat;
        F = val;
    }
    
    public Datum(DatumType type, string val)
    {
        Type = type;
        S = val;
    }
    
    public Datum(DatumType type, List<Node> val)
    {
        Type = type;
        L = val;
    }
    
    public DatumType Type { get; set; }
    public int? I { get; }
    public double? F { get; }
    public string? S { get; }
    public List<Node>? L { get; }

    public int ToInt()
    {
        switch (Type)
        {
            case DatumType.kDatumInt:
                return I!.Value;
            case DatumType.kDatumFloat:
                return (int)F!.Value;
        }
        
        throw new LingoException($"Cannot convert {Type} to int");
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        
        switch (Type)
        {
            case DatumType.kDatumSymbol:
                builder.Append(S);
                break;
            case DatumType.kDatumString:
                builder.Append(S);
                break;
            case DatumType.kDatumInt:
                builder.Append(I);
                break;
            case DatumType.kDatumFloat:
                builder.Append(F);
                break;
            case DatumType.kDatumList:
            case DatumType.kDatumArgList:
            case DatumType.kDatumArgListNoRet:
                builder.Append(string.Join(", ", L?.Select(x => x.ToString()) ?? []));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Type), $"Unknown datum type {Type}");
        }
        
        return builder.ToString();
    }
}