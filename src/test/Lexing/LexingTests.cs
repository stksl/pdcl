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
        Lexer lexer = new Lexer(stream);

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
    [Fact]
    public void IfdefLexing_Test() 
    {

        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/ifdef.pdcl");
        Lexer lexer = new Lexer(stream);

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

        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            res = lexer.Lex();
        }

        Assert.True(res.Status == Lexer.LexerStatusCode.EOF);

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
        Lexer lexer = new Lexer(stream);

        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            res = lexer.Lex();
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

        Lexer lexer = new Lexer(stream);
        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            res = lexer.Lex();
        }
        stream.Dispose();
    }
}