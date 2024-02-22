using System.Collections;
namespace Pdcl.Core;

internal sealed partial class Parser
{
    public sealed class SymbolTable : IEnumerable<IList<Symbol>>
    {
        private Dictionary<string, IList<Symbol>> symbolTable;

        public SymbolTable()
        {
            symbolTable = new Dictionary<string, IList<Symbol>>();
        }

        public Symbol? GetSymbol(string name, SymbolType type)
        {
            if (symbolTable.ContainsKey(name))
                foreach(Symbol symbol in symbolTable[name]) 
                    if (symbol.Type == type) return symbol;

            return null;
        }
        public bool StoreSymbol(Symbol symbol)
        {
            if (symbolTable.ContainsKey(symbol.Name)) 
            {
                if (symbolTable[symbol.Name].Contains(symbol))
                    return false;
                symbolTable[symbol.Name].Add(symbol);
            } 
            else symbolTable[symbol.Name] = new List<Symbol>() {symbol};
            return true;
        }
        public bool RemoveSymbol(string name, SymbolType type)
        {
            return symbolTable.ContainsKey(name) && symbolTable[name].Remove(new Symbol(name, type));
        }
        public IEnumerator<IList<Symbol>> GetEnumerator() 
        {
            foreach(KeyValuePair<string, IList<Symbol>> symbols in symbolTable) 
            {
                yield return symbols.Value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}