
namespace Pdcl.Core.Syntax;

/// <summary>
/// A singly-linked list representing member access expression.
/// </summary>
public sealed class MemberExpression : ExpressionNode 
{
    public readonly RefValueNode Parent;
    public readonly MemberExpressionNode Root;
    public readonly MemberExpressionNode Tail;
    public MemberExpression(RefValueNode parent, MemberExpressionNode root, MemberExpressionNode tail) : base(tail.Type)
    {
        Parent = parent;
        Root = root;
        Tail = tail;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Parent;
        yield return Root;
    }
}
public sealed class MemberExpressionNode : ExpressionNode 
{
    public readonly MemberInvoke Member;
    public readonly MemberExpressionNode? Next;
    public MemberExpressionNode(MemberInvoke member, MemberExpressionNode? next) : base(member.Holder.Type) 
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