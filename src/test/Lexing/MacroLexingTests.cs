using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Test;

public sealed class MacroLexingTests 
{
    [Fact]
    public void MacroLexingTest_ArgedMacro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/argedMacro.pdcl");

        PreprocContext context = new PreprocContext(null);
        Preprocessor preproc = new Preprocessor(stream, context);

        while (! preproc.NextDirectiveAsync().Result.IsFailed);

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
        Lexer lexer = new Lexer(stream, context);
        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (res.Value.Value.Kind == SyntaxKind.MacroSubstitutedToken) 
            {
                SourceStream stream1 = new SourceStream(res.Value.Value.Metadata.Raw.ToCharArray());
                Lexer macroLexer =new Lexer(stream1, context);
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
    public void MacroLexingTest_Macro() 
    {
        SourceStream stream = new SourceStream(GetRelativePath() + "Lexing/macro.pdcl");

        PreprocContext ctx = new PreprocContext(null);
        Preprocessor preproc = new Preprocessor(stream, ctx);
        while (!preproc.NextDirectiveAsync().Result.IsFailed);

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
        Lexer lexer = new Lexer(stream, ctx);

        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        string substitution = "";
        for(int i = 0; i < kinds.Length; i++) 
        {
            Assert.Equal(kinds[i], res.Value!.Value.Kind);
            if (res.Value.Value.Kind == SyntaxKind.MacroSubstitutedToken) 
                substitution = res.Value.Value.Metadata.Raw;
            res = lexer.Lex();
        }
        lexer = new Lexer(new SourceStream(substitution.ToCharArray()), ctx);

        Assert.Equal("struct", lexer.Lex().Value!.Value.Metadata.Raw);

        stream.Dispose();
    }
}