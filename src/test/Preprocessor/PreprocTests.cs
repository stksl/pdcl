using System.ComponentModel;
using System.Reflection;
using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Text;

namespace Pdcl.Test;

public partial class PreprocTest 
{
    [Fact]
    public MacroBag<Macro> HandleMacro_Test()
    {
        // Booting up the preprocessor
        PreprocContext ctx = new PreprocContext();
        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/macro.pdcl");
        Preprocessor preproc = new Preprocessor(stream, ctx);
        
        // Expected names
        string[] macrosNames = {"PI", "struct0_", "someArgedMacro"};

        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> macro = preproc.NextDirective();
        for(int i = 0; i < macrosNames.Length; i++) 
        {
            Assert.True(!macro.IsFailed && macrosNames[i] == macro.Value!.Name);
            ctx.DefinedMacros.InsertMacro((Macro)macro.Value);
            macro = preproc.NextDirective();
        }

        // Non-first token
        Assert.True(macro.Status == Preprocessor.PreprocStatusCode.NonFirstToken);
        return ctx.DefinedMacros;

    }
    [Fact]
    public void HandleIfdef_Test() 
    {
        // HandleMacro_Test is a dependency and has to work fine
        MacroBag<Macro> bag = HandleMacro_Test(); // handling defined macros in "./macro.pdcl"

        using SourceStream stream = new SourceStream(GetRelativePath() + "Preprocessor/ifdef.pdcl");

        PreprocContext ctx = new PreprocContext();

        foreach(Macro macro in bag) 
            Assert.True(ctx.DefinedMacros.InsertMacro(macro));

        Preprocessor preproc = new Preprocessor(stream, ctx);
        /* 
            #ifdef ignored
            #endif 
        */


        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> dir = preproc.NextDirective();
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result == false);
        dir = preproc.NextDirective();
        Assert.True(!dir.IsFailed && dir.Value is EndIf);
        /*
            #ifdef someArgedMacro
            #endif
        */
        dir = preproc.NextDirective();
        Assert.True(!dir.IsFailed && ((Ifdef)dir.Value!).Result);
        dir = preproc.NextDirective();
        Assert.True(!dir.IsFailed && dir.Value is EndIf);
        /*
            #ifdef
        */
        dir = preproc.NextDirective();
        Assert.True(dir.IsFailed);
    }
}