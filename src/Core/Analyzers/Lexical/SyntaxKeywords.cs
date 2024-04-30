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
            new KeyValuePair<string, SyntaxKind>("const", SyntaxKind.ConstToken),
            new KeyValuePair<string, SyntaxKind>("operator", SyntaxKind.OperatorToken),
            new KeyValuePair<string, SyntaxKind>("implicit", SyntaxKind.ImplicitOperatorToken),
            new KeyValuePair<string, SyntaxKind>("explicit", SyntaxKind.ExplicitOperatorToken),            
            new KeyValuePair<string, SyntaxKind>("false", SyntaxKind.FalseToken),
            new KeyValuePair<string, SyntaxKind>("true", SyntaxKind.TrueToken),

            new KeyValuePair<string, SyntaxKind>("public", SyntaxKind.PublicToken),
            new KeyValuePair<string, SyntaxKind>("private", SyntaxKind.PrivateToken),
            new KeyValuePair<string, SyntaxKind>("family", SyntaxKind.FamilyToken),
            new KeyValuePair<string, SyntaxKind>("assembly", SyntaxKind.AssemblyToken),
            new KeyValuePair<string, SyntaxKind>("static", SyntaxKind.StaticToken),

       });
}