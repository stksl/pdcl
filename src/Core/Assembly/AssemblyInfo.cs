using System.Linq.Expressions;
using Pdcl.Core.Syntax;
using Pdcl.Core;
namespace Pdcl.Core.Assembly;

public sealed class AssemblyInfo 
{
    public readonly SymbolTableTree TableTree;
    public readonly AssemblyManifest Manifest;
    public readonly Dictionary<string, AssemblyInfo> ExternalAssemblies;
    internal AssemblyInfo(SymbolTableTree tree, AssemblyManifest manifest) 
    {
        TableTree = tree;
        Manifest = manifest;

        ExternalAssemblies = new Dictionary<string, AssemblyInfo>();
    }
    /// <summary>
    /// Searches for symbol globally. prefix '$' is used to define an assembly
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Symbol? GetSymbol(string path, SymbolType type) 
    {
        if (path.StartsWith('$')) 
        {
            string asmName = path.Substring(1, path.IndexOf('/') - 1);
            return ExternalAssemblies[asmName].GetSymbol(path.Substring(asmName.Length + 2), type);
        }
        return TableTree.GetNode(path)!.GetSymbol(path.Substring(path.LastIndexOf('/')), type);
    }
}