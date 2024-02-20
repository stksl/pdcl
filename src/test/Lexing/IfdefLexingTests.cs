using Pdcl.Core;
using Pdcl.Core.Text;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
namespace Pdcl.Test;

public sealed class IfdefLexingTests 
{
    [Fact]
    public void IfdefLexing_Test() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/ifdef.pdcl");
        PreprocContext ctx = new PreprocContext(null);
        Preprocessor preproc = new Preprocessor(stream, ctx);
        while (!preproc.NextDirectiveAsync().Result.IsFailed);
        stream.Dispose();

        stream = new SourceStream(GetRelativePath() + "Lexing/ifdef.pdcl");
        Lexer lexer = new Lexer(stream, ctx);

        // expected
        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken,
            SyntaxKind.OpenParentheseToken,
            SyntaxKind.CloseParentheseToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.OpenBraceToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.CloseBraceToken
        };

        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            res = lexer.Lex();
        }
        Assert.True(res.Status == Lexer.LexerStatusCode.EOF);

        stream.Dispose();
    }
}