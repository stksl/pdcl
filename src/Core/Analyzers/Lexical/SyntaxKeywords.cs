using Pdcl.Core.Syntax;

namespace Pdcl.Core;

internal sealed partial class Lexer
{
    public static readonly IDictionary<string, SyntaxKind> SyntaxKeywords = new Dictionary<string, SyntaxKind>(
       new KeyValuePair<string, SyntaxKind>[]
       {
            new KeyValuePair<string, SyntaxKind>("use", SyntaxKind.UseToken),
            new KeyValuePair<string, SyntaxKind>("if", SyntaxKind.IfToken),
            new KeyValuePair<string, SyntaxKind>("elif", SyntaxKind.ElifToken),
            new KeyValuePair<string, SyntaxKind>("else", SyntaxKind.ElseToken),
            new KeyValuePair<string, SyntaxKind>("for", SyntaxKind.ForLoopToken),
            new KeyValuePair<string, SyntaxKind>("while", SyntaxKind.WhileLoopToken),
            new KeyValuePair<string, SyntaxKind>("namespace", SyntaxKind.NamespaceToken),
            new KeyValuePair<string, SyntaxKind>("pinv", SyntaxKind.PInvToken),
            new KeyValuePair<string, SyntaxKind>("struct", SyntaxKind.StructToken),
            new KeyValuePair<string, SyntaxKind>("return", SyntaxKind.ReturnToken),
            new KeyValuePair<string, SyntaxKind>("il_inline", SyntaxKind.IL_InlineToken),
       });
}