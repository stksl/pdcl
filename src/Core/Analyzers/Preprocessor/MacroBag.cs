using System.Collections;
namespace Pdcl.Core.Preproc;
/// <summary>
/// A defined macros collection
/// </summary>
public sealed class MacroBag : IEnumerable<Macro>
{
    private LinkedList<IDictionary<string, Macro>> definedMacros;
    public MacroBag(LinkedList<IDictionary<string, Macro>>? list)
    {
        definedMacros = list ?? new LinkedList<IDictionary<string, Macro>>();

        definedMacros.AddLast(
            new LinkedListNode<IDictionary<string, Macro>>(
                new Dictionary<string, Macro>()));
    }
    public Macro? GetMacroFor(string? token) 
    {
        if (token == null) return null;
        foreach(IDictionary<string, Macro> macros in definedMacros) 
        {
            if (macros.TryGetValue(token, out Macro? macro)) 
            {
                return macro;
            }
        }
        return null;
    }
    public bool InsertMacro(Macro macro) 
    {
        if (GetMacroFor(macro.Name) != null) 
        {
            return false;
        }

        definedMacros.Last!.Value[macro.Name] = macro;
        return true;
    }
    public IEnumerator<Macro> GetEnumerator()
    {
        foreach(IDictionary<string, Macro> node in definedMacros) 
        {
            foreach(KeyValuePair<string, Macro> pair in node) yield return pair.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}