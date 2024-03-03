
namespace Pdcl.Core.Syntax;

public sealed class ConstVarNode : ValueHolderNode
{
    public LiteralValue ConstValue {get; internal set;}
    public ConstVarNode(string name, TypeNode type, LiteralValue constVal, int tokenInd) 
        : base(name, type, hasGetter: true, hasSetter: false, tokenInd)
    {
        ConstValue = constVal;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
        yield return ConstValue;
    }
}