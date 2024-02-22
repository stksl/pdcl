using System.Linq.Expressions;

namespace Pdcl.Core.Syntax;

/// <summary>
/// An abstract syntax tree (AST).
/// </summary>
internal sealed class SyntaxTree
{
    public readonly SyntaxNode Root;

    public SyntaxTree()
    {
        Root = new ApplicationContextNode();
    }
    /// <summary>
    /// Adds a global node as a Root element's child.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public void AddGlobalSymbol(Parser.Symbol symbol) 
    {
        ((ApplicationContextNode)Root).addChild(symbol.Node);
    }
    public SyntaxNode? GetParent(SyntaxNode node, SyntaxNode parent = null!) 
    {
        if (parent == null) parent = Root;

        foreach(SyntaxNode child in parent.GetChildren()) 
        {
            if (child == node || (parent = GetParent(node, child)!) != null) 
            {
                return parent; 
            } 
        }
        return null;
    }
    internal sealed class ApplicationContextNode : SyntaxNode
    {
        private List<SyntaxNode> globalNodes; // those could be any of: global var, type declaration, func declaration
        public ApplicationContextNode() : base(0)
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