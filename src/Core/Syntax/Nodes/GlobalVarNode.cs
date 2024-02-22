
using System.Reflection.Metadata.Ecma335;

namespace Pdcl.Core.Syntax;

public sealed class ConstVarNode : SyntaxNode 
{
    public readonly string Name;
    public readonly VariableType Type;
    public readonly LiteralValue ConstValue;
    public ConstVarNode(string name, VariableType type, LiteralValue constVal, int tokenInd) : base(tokenInd)
    {
        Name = name;
        Type = type;
        ConstValue = constVal;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ConstValue;
    }
}