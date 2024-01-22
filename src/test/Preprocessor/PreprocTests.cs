using Pdcl.Core.Preproc;
using Pdcl.Core.Text;
namespace Pdcl.Test;

public partial class PreprocTest 
{
    public const string FilePath = "../macro.pdcl";
    [Fact]
    public void HandleMacro_Test()
    {
        // Booting up the preprocessor
        string[] splitPath = Path.GetFullPath(FilePath).Split('/');
        string path = string.Join('/', splitPath.Take(splitPath.Length - 3)) + "/Preprocessor/macro.pdcl";
        using SourceReader reader = new SourceReader(path);
        Preprocessor preproc = new Preprocessor(reader, null!);
        
        // Expected names
        string[] macrosNames = {"PI", "struct0_", "someArgedMacro"};

        Preprocessor.PreprocResult<IDirective?> dir = preproc.NextDirective();
        for(int i = 0; dir.StatusCode != Preprocessor.PreprocStatusCode.EOF; i++) 
        {
            Assert.True(!dir.IsFailed && macrosNames[i] == dir.Value!.Name);

            dir = preproc.NextDirective();
        }
    }
    [Fact]
    public void HandleIfdef_Test() 
    {
        
    }
}