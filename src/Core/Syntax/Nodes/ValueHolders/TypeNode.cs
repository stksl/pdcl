
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Pdcl.Core.Syntax;

public class TypeNode : SyntaxNode
{
    public readonly TypeDeclarationNode TypeDeclaration;
    public TypeNode(TypeDeclarationNode typeDeclar)
    {
        TypeDeclaration = typeDeclar;

    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        if (TypeDeclaration != null)
            yield return TypeDeclaration;
    }
}
public sealed class PrimitiveTypeNode : TypeNode 
{
    public readonly bool IsInteger;
    public readonly bool IsUnsigned;
    public readonly bool IsFloat;
    public readonly PrimitiveTypes Type;
    internal PrimitiveTypeNode(PrimitiveTypes type) : base(null!)
    {
        const PrimitiveTypes uMask = PrimitiveTypes.UInt8 | PrimitiveTypes.UInt16 | PrimitiveTypes.UInt32 | PrimitiveTypes.UInt64;
        const PrimitiveTypes iMask = uMask | PrimitiveTypes.Int8 | PrimitiveTypes.Int16 | PrimitiveTypes.Int32 | PrimitiveTypes.Int64;
        const PrimitiveTypes fMask = PrimitiveTypes.Float32 | PrimitiveTypes.Float64;

        IsInteger = iMask.HasFlag(type);
        IsUnsigned = uMask.HasFlag(type);
        IsFloat = fMask.HasFlag(type);
        
        Type = type;
    } // TODO: declare primitive types as a common type system interface and overload implicit/explicit operators 

    [Flags]
    public enum PrimitiveTypes 
    {
        UInt8 = 1,
        UInt16 = 1 << 1,
        UInt32 = 1 << 2,
        UInt64 = 1 << 3,
        Int8 = 1 << 4,
        Int16 = 1 << 5,
        Int32 = 1 << 6,
        Int64 = 1 << 7,
        Float32 = 1 << 8,
        Float64 = 1 << 9,
        Char = 1 << 10,
        Bool = 1 << 11,
        String = 1 << 12,
    }
} 