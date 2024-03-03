
namespace Pdcl.Core.Syntax;

public class TypeNode : SyntaxNode 
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
public sealed class PrimitiveTypeNode : TypeNode 
{
    public readonly PrimitiveTypes Type;
    internal PrimitiveTypeNode(PrimitiveTypes type, string name, TypeDeclarationNode typeDeclar, int tokenInd) 
        : base(name, typeDeclar, tokenInd)
    {
        Type = type;
    }
    public enum PrimitiveTypes 
    {
        UInt8,
        Int8,
        UInt16,
        Int16,
        UInt32,
        Int32,
        UInt64,
        Int64,
        Char,
        Boolean,
        Float32,
        Float64,
        String,
    }
} 