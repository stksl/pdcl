using System.Collections;
namespace Pdcl.Core;

internal sealed partial class Parser
{
    public sealed class SymbolTable : IEnumerable<Symbol>
    {
        private HashSet<Symbol> symbolTable;

        public SymbolTable()
        {
            symbolTable = new HashSet<Symbol>();
        }

        public Symbol? GetSymbol(string name)
        {
            return symbolTable.TryGetValue(new Symbol(name), out Symbol val) ? val : null;
        }
        public bool StoreSymbol(Symbol symbol)
        {
            return symbolTable.Add(symbol);
        }
        public bool RemoveSymbol(string name)
        {
            return symbolTable.Remove(new Symbol(name));
        }
        public IEnumerator<Symbol> GetEnumerator() => symbolTable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}