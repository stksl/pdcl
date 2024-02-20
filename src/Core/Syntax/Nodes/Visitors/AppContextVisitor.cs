using System.Numerics;

namespace Pdcl.Core.Syntax;

internal sealed class ApplicationContextVisitor : IVisitor<SyntaxTree.ApplicationContextNode> 
{
    private static ApplicationContextVisitor? _instance;
    public static ApplicationContextVisitor Instance => _instance ??= new ApplicationContextVisitor();
    private ApplicationContextVisitor() {}
    public Task<SyntaxTree.ApplicationContextNode> VisitAsync(Parser parser) 
    {
        var ctx = parser._tree!.Root as SyntaxTree.ApplicationContextNode;
        
        throw new NotImplementedException();
    }
}