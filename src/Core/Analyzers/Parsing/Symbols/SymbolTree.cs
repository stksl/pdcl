using System.Collections;
using System.Text;
namespace Pdcl.Core.Syntax;

public sealed class SymbolTableTree
{
    public readonly SymbolTreeNode Root;
    public SymbolTableTree()
    {
        Root = new SymbolTreeNode(null!, new SymbolTable(), "/");
    }

    public bool AddNode(string path, SymbolTreeNode node)
    {
        string[] parts = path.Split('/').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
        SymbolTreeNode runner = Root;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!runner.Children.ContainsKey(parts[i]))
                return false;

            runner = runner.Children[parts[i]];
        }
        runner.Children[parts[^1]] = node;
        return true;
    }
    public SymbolTreeNode? GetNode(string path)
    {
        string[] parts = path.Split('/').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
        SymbolTreeNode runner = Root;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!runner.Children.ContainsKey(parts[i]))
                return null;
            runner = runner.Children[parts[i]];
        }
        return runner;
    }
}
public sealed class SymbolTreeNode : IEnumerable<SymbolTreeNode>
{
    public readonly SymbolTreeNode Parent;
    public readonly SymbolTable? Table;
    public readonly string Name;
    public readonly Dictionary<string, SymbolTreeNode> Children;
    public SymbolTreeNode(SymbolTreeNode parent, SymbolTable? table, string name)
    {
        Parent = parent;
        Name = name;
        Table = table;

        Children = new Dictionary<string, SymbolTreeNode>();
    }
    public string GetFullPath()
    {
        List<string> parts = new List<string>();

        SymbolTreeNode runner = this;
        while (runner != null)
        {
            parts.Add(Name);
            runner = runner.Parent;
        }
        parts.Reverse();
        return string.Join('/', parts).Insert(0, "/");
    }
    /// <summary>
    /// Searches for a symbol (with <paramref name="name"/> and <paramref name="type"/>) up the tree
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Symbol? GetSymbol(string name, SymbolType type)
    {
        SymbolTreeNode runner = this;
        while (runner != null)
        {
            if (runner.Table != null)
            {
                Symbol? symbol = runner.Table.GetSymbol(name, type);
                if (symbol.HasValue) return symbol;
            }
            runner = runner.Parent;
        }
        return null;
    }
    public IEnumerator<SymbolTreeNode> GetEnumerator()
    {
        foreach (KeyValuePair<string, SymbolTreeNode> child in Children)
        {
            yield return child.Value;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}