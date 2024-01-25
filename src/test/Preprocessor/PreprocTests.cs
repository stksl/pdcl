using System.ComponentModel;
using System.Reflection;
using Pdcl.Core;
using Pdcl.Core.Preproc;
using Pdcl.Core.Text;

namespace Pdcl.Test;

public partial class PreprocTest 
{
    public static string GetRelativePath() 
    {
        string[] splitPath = Path.GetFullPath(".").Split('/');
        string path = string.Join('/', splitPath.Take(splitPath.Length - 3)) + "/Preprocessor/";

        return path;
    }
    [Fact]
    public MacroBag HandleMacro_Test()
    {
        // Booting up the preprocessor
        PreprocContext ctx = new PreprocContext();
        using SourceStream stream = new SourceStream(GetRelativePath() + "macro.pdcl");
        Preprocessor preproc = new Preprocessor(stream, ctx);
        
        // Expected names
        string[] macrosNames = {"PI", "struct0_", "someArgedMacro"};

        IAnalyzerResult<IDirective, Preprocessor.PreprocStatusCode> macro = preproc.NextDirective();
        for(int i = 0; i < macrosNames.Length; i++) 
        {
            Assert.True(!macro.IsFailed && macrosNames[i] == macro.Value!.Name);
            ctx.Macros.InsertMacro((Macro)macro.Value);
            macro = preproc.NextDirective();
        }

        // Non-first token
        Assert.True(macro.Status == Preprocessor.PreprocStatusCode.NonFirstToken);
        return ctx.Macros;

    }
    [Fact]
    public void HandleIfdef_Test() 
    {
        // HandleMacro_Test is a dependency and has to work fine
        MacroBag bag = HandleMacro_Test(); // handling defined macros in "./macro.pdcl"

        using SourceStream stream = new SourceStream(GetRelativePath() + "ifdef.pdcl");

        PreprocContext ctx = new PreprocContext();

        foreach(Macro macro in bag) 
            Assert.True(ctx.Macros.InsertMacro(macro));

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