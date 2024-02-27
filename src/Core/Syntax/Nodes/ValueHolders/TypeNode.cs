
namespace Pdcl.Core.Syntax;

public sealed class TypeNode : SyntaxNode 
{
    public readonly string Name;
    public readonly TypeDeclarationNode TypeDeclaration;
    public TypeNode(string name, TypeDeclarationNode typeDeclar, int tokenInd) : base(tokenInd)
    {
        Name = name;    
        TypeDeclaration = typeDeclar;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return TypeDeclaration;
    }
}