
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Pdcl.Core.Syntax;

public class TypeNode : SyntaxNode
{
    public readonly string Name;
    public readonly bool IsPointer;
    public TypeNode(string name, bool isPointer)
    {
        Name = name;

        IsPointer = isPointer;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}
public sealed class PrimitiveTypeNode : TypeNode
{
    public PrimitiveTypes Type { get; init; }
    internal PrimitiveTypeNode(string name, bool isPointer) : base(name, isPointer)
    {

    } // TODO: declare primitive types as a common type system interface and overload implicit/explicit operators 

}
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