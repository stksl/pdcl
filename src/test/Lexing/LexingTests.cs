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
        Lexer lexer = new Lexer(stream, new PreprocContext(), new Core.Diagnostics.DiagnosticHandler());

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
        IAnalyzerResult<SyntaxToken?, LexerStatusCode> res = lexer.Lex();
        for(int i = 0, j = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (kinds[i] == SyntaxKind.TextToken) 
            {
                Assert.Equal(textTokens[j++], res.Value!.Value.Metadata.Raw);
            }
            res = lexer.Lex();
        }
        Assert.True(res.Status == LexerStatusCode.EOF);
    }
    [Fact]
    public void LexingTest_ArgedMacro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/argedMacro.pdcl");

        PreprocContext context = new PreprocContext();
        Preprocessor preproc = new Preprocessor(stream, context);

        while (!preproc.NextDirective().IsFailed);

        stream.Dispose();
        stream = new SourceStream(GetRelativePath() + "Lexing/argedMacro.pdcl");

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken, // int32
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken, // val
            SyntaxKind.TriviaToken,
            SyntaxKind.EqualToken, // =
            SyntaxKind.TriviaToken,
            SyntaxKind.MacroSubstitutedToken,
            SyntaxKind.SemicolonToken,
        };


        SyntaxKind[] macroKinds = 
        {
            SyntaxKind.TriviaToken,
            SyntaxKind.NumberToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.PlusToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.NumberToken,
        }; 
        Lexer lexer = new Lexer(stream, context, new Core.Diagnostics.DiagnosticHandler());
        IAnalyzerResult<SyntaxToken?, LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (res.Value.Value.Kind == SyntaxKind.MacroSubstitutedToken) 
            {
                SourceStream stream1 = new SourceStream(res.Value.Value.Metadata.Raw.ToCharArray());
                Lexer macroLexer =new Lexer(stream1, context, new Core.Diagnostics.DiagnosticHandler());
                var res2 = macroLexer.Lex();
                for(int j = 0; j < macroKinds.Length; j++) 
                {
                    Assert.Equal(macroKinds[j], res2.Value!.Value.Kind);
                    res2 = macroLexer.Lex();
                }
            }
            res = lexer.Lex();
        }
        stream.Dispose();
    }
    [Fact]
    public void LexingTest_Macro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/macro.pdcl");

        PreprocContext ctx = new PreprocContext();
        Preprocessor preproc = new Preprocessor(stream, ctx);
        while (!preproc.NextDirective().IsFailed);

        SyntaxKind[] kinds = 
        {
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken,
            SyntaxKind.TriviaToken,
            SyntaxKind.TextToken,       
            SyntaxKind.OpenParentheseToken,       
            SyntaxKind.CloseParentheseToken,       
            SyntaxKind.TriviaToken,
            SyntaxKind.MacroSubstitutedToken,

        };
        stream.Dispose();
        stream = new SourceStream(GetRelativePath() + "Lexing/macro.pdcl");
        Lexer lexer = new Lexer(stream, ctx, new Core.Diagnostics.DiagnosticHandler());

        IAnalyzerResult<SyntaxToken?, LexerStatusCode> res = lexer.Lex();
        string substitution = "";
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (res.Value.Value.Kind == SyntaxKind.MacroSubstitutedToken) 
                substitution = res.Value.Value.Metadata.Raw;
            res = lexer.Lex();
        }
        lexer = new Lexer(new SourceStream(substitution.ToCharArray()), ctx, new Core.Diagnostics.DiagnosticHandler());

        Assert.Equal("struct", lexer.Lex().Value!.Value.Metadata.Raw);

        stream.Dispose();
    }
}