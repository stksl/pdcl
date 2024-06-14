namespace Pdcl.Core.Syntax;

[Flags]
public enum NumberMetadata : ulong 
{
    Integer = 1,
    LongLiteral = 1 << 1,

    Float32Literal = 1 << 2,
    Float64Literal = 1 << 3,

    Unsigned = 1 << 4
}