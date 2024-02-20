using System.ComponentModel;
using System.Reflection;
using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Text;

namespace Pdcl.Test;

public partial class PreprocTest
{
    [Fact]
    public LinkedList<IDictionary<int, Macro>> HandleMacro_Test()
    {
        // Booting up the preprocessor
        PreprocContext ctx = new PreprocContext(null);
        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/macro.pdcl");
        Preprocessor preproc = new Preprocessor(stream, ctx);

        // Expected names
        string[] macrosNames = { "PI", "struct0_", "someArgedMacro" };

        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> macro = preproc.NextDirectiveAsync().Result;
        for (int i = 0; i < macrosNames.Length; i++)
        {
            Assert.True(!macro.IsFailed && macrosNames[i] == macro.Value!.Name);

            ctx.Macros.Last!.Value[macro.Value.GetHashCode()] = (Macro)macro.Value;
            macro = preproc.NextDirectiveAsync().Result;
        }

        // Non-first token
        Assert.True(macro.Status == Preprocessor.PreprocStatusCode.NonFirstToken);
        return ctx.Macros;

    }
    [Fact]
    public void HandleIfdef_Test()
    {
        // HandleMacro_Test is a dependency and has to work fine
        PreprocContext ctx = new PreprocContext(HandleMacro_Test());

        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/ifdef.pdcl");

        Preprocessor preproc = new Preprocessor(stream, ctx);
        /* 
            #ifdef ignored
            #endif 
        */


        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> dir = preproc.NextDirectiveAsync().Result;
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result == false);
        /*
            #ifdef someArgedMacro
            #endif
        */
        dir = preproc.NextDirectiveAsync().Result;
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result);
        /*
            #ifndef someArgedMacro
                #ifdef someArgedMacro
                #endif
        */
        dir = preproc.NextDirectiveAsync().Result;
        Assert.Equal(Preprocessor.PreprocStatusCode.EOF, dir.Status);
    }
}