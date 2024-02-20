using System.Linq.Expressions;

namespace Pdcl.Core.Syntax;

/// <summary>
/// An abstract syntax tree (AST).
/// </summary>
internal sealed class SyntaxTree
{
    private readonly Parser.SymbolTable table;
    public readonly SyntaxNode Root;

    public SyntaxTree(Parser.SymbolTable globaltable)
    {
        Root = new ApplicationContextNode();

        table = globaltable;
    }
    /// <summary>
    /// Adds a global node as a Root element's child.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool AddGlobalSymbol(Parser.Symbol symbol) 
    {
        bool res = table.StoreSymbol(symbol);
        if (res) 
            ((ApplicationContextNode)Root).addChild(symbol.Node);
        return res;
    }
    internal sealed class ApplicationContextNode : SyntaxNode
    {
        private List<SyntaxNode> globalNodes; // those could be any of: global var, type declaration, func declaration
        public ApplicationContextNode()
        {
            globalNodes = new List<SyntaxNode>();
        }

        public void addChild(SyntaxNode node) 
        {
            globalNodes.Add(node);
        }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return globalNodes;
        }
    }
}