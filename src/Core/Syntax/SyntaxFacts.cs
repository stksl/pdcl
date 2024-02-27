using System.Diagnostics.Contracts;

namespace Pdcl.Core.Syntax;

internal static class SyntaxFacts 
{
    public static bool IsLiteralKind(SyntaxKind kind) 
    {
        switch (kind) 
        {
            case SyntaxKind.NumberToken:
            case SyntaxKind.StringLiteral:
            case SyntaxKind.CharLiteral:
                return true;
            default: return false;
        }
    }
    public static bool IsOperandKind(SyntaxKind kind) => IsLiteralKind(kind) || kind == SyntaxKind.TextToken;
}