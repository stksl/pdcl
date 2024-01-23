using System.ComponentModel;
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

        using SourceReader reader = new SourceReader(GetRelativePath() + "macro.pdcl");
        Preprocessor preproc = new Preprocessor(reader, null!);
        
        // Expected names
        string[] macrosNames = {"PI", "struct0_", "someArgedMacro"};

        MacroBag bag = new MacroBag(null);

        Preprocessor.PreprocResult<IDirective?> macro = preproc.NextDirective();
        for(int i = 0; macro.StatusCode != Preprocessor.PreprocStatusCode.EOF; i++) 
        {
            Assert.True(!macro.IsFailed && macrosNames[i] == macro.Value!.Name);

            Assert.True(bag.InsertMacro((Macro)macro.Value));
            macro = preproc.NextDirective();
        }

        return bag;
    }
    [Fact]
    public void HandleIfdef_Test() 
    {
        // HandleMacro_Test is a dependency and has to work fine
        MacroBag bag = HandleMacro_Test(); // handling defined macros in "./macro.pdcl"

        using SourceReader reader = new SourceReader(GetRelativePath() + "ifdef.pdcl");

        PreprocContext ctx = new PreprocContext();

        foreach(Macro macro in bag) 
            Assert.True(ctx.Macros.InsertMacro(macro));

        Preprocessor preproc = new Preprocessor(reader, ctx);
        /* 
            #ifdef ignored
            #endif 
        */
        Preprocessor.PreprocResult<IDirective?> dir = preproc.NextDirective();
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
        Assert.True(dir.StatusCode == Preprocessor.PreprocStatusCode.UnknownDeclaration);
    }
}