using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Test;
public sealed class LexingTests 
{
    [Fact]
    public void LexingTest_MAIN() 
    {
        using SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/general.pdcl");
        Lexer lexer = new Lexer(stream, new PreprocContext(null));

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken, // void
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken, // main
            SyntaxKind.OpenParentheseToken,
            SyntaxKind.CloseParentheseToken,

            SyntaxKind.TriviaToken,
            SyntaxKind.OpenBraceToken,
            SyntaxKind.TriviaToken,

            SyntaxKind.TextToken, // string
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken, // str
            SyntaxKind.TriviaToken,
            SyntaxKind.EqualToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.StringLiteral,
            SyntaxKind.SemicolonToken,
            SyntaxKind.TriviaToken,

            SyntaxKind.TextToken, // int32
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken, // some_int0
            SyntaxKind.TriviaToken,
            SyntaxKind.EqualToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.NumberToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.CloseBraceToken,
        };

        string[] textTokens = 
        {
            "void",
            "main",
            "string",
            "str",
            "int32",
            "some_int0"
        };
        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0, j = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (kinds[i] == SyntaxKind.TextToken) 
            {
                Assert.Equal(textTokens[j++], res.Value!.Value.Metadata.Raw);
            }
            res = lexer.Lex();
        }
        Assert.True(res.Status == Lexer.LexerStatusCode.EOF);
    }
}