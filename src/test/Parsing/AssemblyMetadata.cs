using Pdcl.Core.Assembly;

namespace Pdcl.Test;

public static class AssemblyTestMetadata 
{
    private static AssemblyInfo _info = new AssemblyInfo(
        new Core.Syntax.SymbolTableTree(), 
        new AssemblyManifest("testasm", (1,0,0,0), null!), null!);
    public static AssemblyInfo GetAssemblyInfo() 
    {
        return _info;
    }
}