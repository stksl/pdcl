
namespace Pdcl.Core.Syntax;

/// <summary>
/// A singly-linked list representing member access expression.
/// </summary>
public sealed class MemberExpression : ExpressionNode 
{
    public readonly RefValueNode Parent;
    public readonly MemberExpressionNode Root;
    public MemberExpression(RefValueNode parent, MemberExpressionNode root, int tokenInd) : base(tokenInd)
    {
        Parent = parent;
        Root = root;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Parent;
        yield return Root;
    }
}
public sealed class MemberExpressionNode : SyntaxNode 
{
    public readonly MemberInvoke Member;
    public readonly MemberExpressionNode? Next;
    public MemberExpressionNode(MemberInvoke member, MemberExpressionNode? next, int tokenInd) : base(tokenInd)
    {
        Member = member;
        Next = next;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        if (Next != null)
            yield return Next;

        yield return Member;
    }
}