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
        Lexer lexer = new Lexer(stream, false);

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken, // void
            SyntaxKind.TextToken, // main
            SyntaxKind.OpenParentheseToken,
            SyntaxKind.CloseParentheseToken,

            SyntaxKind.OpenBraceToken,

            SyntaxKind.TextToken, // string
            SyntaxKind.TextToken, // str
            SyntaxKind.EqualToken,
            SyntaxKind.StringLiteral,
            SyntaxKind.SemicolonToken,

            SyntaxKind.TextToken, // int32
            SyntaxKind.TextToken, // some_int0
            SyntaxKind.EqualToken,
            SyntaxKind.NumberToken,
            SyntaxKind.SemicolonToken,
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
        for(int i = 0, j = 0; i < kinds.Length; i++) 
        {
            IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();

            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (kinds[i] == SyntaxKind.TextToken) 
            {
                Assert.Equal(textTokens[j++], res.Value!.Value.Metadata.Raw);
            }
        }
    }
    [Fact]
    public void IfdefLexing_Test() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/ifdef.pdcl");
        Lexer lexer = new Lexer(stream, true);

        // expected
        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,
            SyntaxKind.OpenParentheseToken,
            SyntaxKind.CloseParentheseToken,
            SyntaxKind.OpenBraceToken,
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,
            SyntaxKind.EqualToken,
            SyntaxKind.NumberToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.CloseBraceToken
        };
        for(int i = 0; i < kinds.Length; i++) 
        {
            IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();

            Assert.Equal(kinds[i], res.Value!.Value.Kind);
        }

        Assert.Throws<EndOfStreamException>(lexer.Lex);
        stream.Dispose();
    }
    [Fact]
    public void MacroLexingTest_Macro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/macro.pdcl");

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,       
            SyntaxKind.OpenParentheseToken,       
            SyntaxKind.CloseParentheseToken,       
            SyntaxKind.OpenBraceToken,
            SyntaxKind.TextToken,
            SyntaxKind.TextToken,
            SyntaxKind.EqualToken,
            SyntaxKind.NumberToken,
            SyntaxKind.SemicolonToken,
            SyntaxKind.CloseBraceToken,

        };
        Lexer lexer = new Lexer(stream, true);

        for(int i = 0; i < kinds.Length; i++) 
        {
            IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();

            Assert.Equal(kinds[i], res.Value!.Value.Kind);
        }

        stream.Dispose();
    }
    [Fact]
    public void MacroLexingTest_ArgedMacro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/argedMacro.pdcl");

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TextToken, // int32
            SyntaxKind.TextToken, // val
            SyntaxKind.EqualToken, // =
            SyntaxKind.NumberToken,
            SyntaxKind.PlusToken,
            SyntaxKind.NumberToken,
            SyntaxKind.StarToken,
            SyntaxKind.NumberToken,
            SyntaxKind.SemicolonToken,
        };

        Lexer lexer = new Lexer(stream, true);
        for(int i = 0; i < kinds.Length; i++) 
        {
            IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();

            Assert.Equal(kinds[i], res.Value!.Value.Kind);
        }
        stream.Dispose();
    }
}