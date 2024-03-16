using System.Collections;
namespace Pdcl.Core;

public sealed class SymbolTable : IEnumerable<Symbol>
{
    private HashSet<Symbol> symbolTable;

    public SymbolTable()
    {
        symbolTable = new HashSet<Symbol>();
    }

    public Symbol? GetSymbol(string name, SymbolType type)
    {
        return symbolTable.TryGetValue(new Symbol(name, type), out Symbol actual) ? actual : null;
    }
    public bool StoreSymbol(Symbol symbol)
    {
        return symbolTable.Add(symbol);
    }
    public bool RemoveSymbol(string name, SymbolType type)
    {
        return symbolTable.Remove(new Symbol(name, type));
    }
    public IEnumerator<Symbol> GetEnumerator()
    {
        foreach (Symbol symbol in symbolTable)
        {
            yield return symbol;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}