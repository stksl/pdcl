
namespace Pdcl.Core.Syntax;

/// <summary>
/// A value holder node (either a local var or a field)
/// </summary>
public abstract class ValueNode : SyntaxNode
{
    protected ValueNode(int tokenInd) : base(tokenInd) 
    {

    }
    public abstract SyntaxKind GetKind();
}