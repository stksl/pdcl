
namespace Pdcl.Core.Syntax;

public sealed class VariableType : SyntaxNode 
{
    public readonly string Name;
    public readonly TypeDeclarationNode TypeDeclaration;
    public VariableType(string name, TypeDeclarationNode typeDeclar, int tokenInd) : base(tokenInd)
    {
        Name = name;    
        TypeDeclaration = typeDeclar;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}