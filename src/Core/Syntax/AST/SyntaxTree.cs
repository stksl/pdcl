using System.Linq.Expressions;

namespace Pdcl.Core.Syntax;

/// <summary>
/// An abstract syntax tree (AST).
/// </summary>
public sealed class SyntaxTree
{
    public readonly ApplicationContextNode Root;

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
    public void AddGlobalSymbol(Symbol symbol) 
    {
        Root.addChild(symbol.Node);
    }
    public SyntaxNode? GetParent(SyntaxNode node, SyntaxNode parent = null!) 
    {
        throw new NotSupportedException();
        // no point for now
        /* if (parent == null) parent = Root;

        foreach(SyntaxNode child in parent.GetChildren()) 
        {
            if (child == node || (parent = GetParent(node, child)!) != null) 
            {
                return parent; 
            } 
        }
        return null; */
    }
    public sealed class ApplicationContextNode : SyntaxNode
    {
        private List<SyntaxNode> globalNodes; // those could be any of: global var, type declaration, func declaration
        public ApplicationContextNode()
        {
            globalNodes = new List<SyntaxNode>();
        }

        internal void addChild(SyntaxNode node) 
        {
            globalNodes.Add(node);
        }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return globalNodes;
        }
    }
}