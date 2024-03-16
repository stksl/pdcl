using System.Collections.Immutable;
using Pdcl.Core;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;
using static Pdcl.Test.AssemblyTestMetadata;
namespace Pdcl.Test;

public sealed class ParserMainTest 
{
    [Fact]
    public void MainTest() 
    {
        using SourceStream stream = new SourceStream(GetRelativePath() + "Parsing/main.pdcl");
        Lexer lexer = new Lexer(stream, new Core.Preproc.PreprocContext(null));
        List<SyntaxToken> tokens = new List<SyntaxToken>();
        IAnalyzerResult<SyntaxToken?, Lexer.LexerStatusCode> res = lexer.Lex();
        while (!res.IsFailed) 
        {
            tokens.Add(res.Value!.Value);
            res = lexer.Lex();
        }
        // ParseLiteralExpression is not implemented...
        Parser parser = new Parser(
            tokens.Where(i => i.Kind != SyntaxKind.TriviaToken).ToImmutableList(), 
            new Core.Diagnostics.DiagnosticHandler(), 
            GetAssemblyInfo());
        parser.diagnostics.OnDiagnosticReported += onDiagnostic;
        parser.ParseAsync().Wait();

    }

    private static StreamWriter sw = new StreamWriter(GetRelativePath() + "Parsing/err.txt");
    private Task onDiagnostic(IDiagnostic diagnostic) 
    {
        return sw.WriteLineAsync(diagnostic.ToString());
    }
}