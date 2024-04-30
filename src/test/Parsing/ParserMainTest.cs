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
        // ParseLiteralExpression is not implemented...
        Parser parser = new Parser(lexer,
            new DiagnosticHandler(), 
            GetAssemblyInfo());
        parser.diagnostics.OnDiagnosticReported += onDiagnostic;

        sw.AutoFlush = true;
        parser.ParseAsync().Wait();
        parser.diagnostics.OnDiagnosticReported -= onDiagnostic;

        sw.Dispose();
    }

    private static StreamWriter sw = new StreamWriter(GetRelativePath() + "Parsing/err.txt");
    private Task onDiagnostic(IDiagnostic diagnostic) 
    {
        return sw.WriteLineAsync(diagnostic.ToString());
    }
}