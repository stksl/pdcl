using System.Collections;
namespace Pdcl.Core.Preproc;
/// <summary>
/// A defined macros collection
/// </summary>
public sealed class MacroBag<TMacro> : IEnumerable<TMacro> where TMacro : IMacro
{
    private LinkedList<IDictionary<int, TMacro>> macros;
    public MacroBag(LinkedList<IDictionary<int, TMacro>>? list)
    {
        macros = list ?? new LinkedList<IDictionary<int, TMacro>>();

        macros.AddLast(
            new LinkedListNode<IDictionary<int, TMacro>>(
                new Dictionary<int, TMacro>()));
    }
    public TMacro? GetMacroFor(int hashcode) 
    {
        foreach(IDictionary<int, TMacro> macros in macros) 
        {
            if (macros.TryGetValue(hashcode, out TMacro? macro)) 
            {
                return macro;
            }
        }
        return default;
    }
    public bool InsertMacro(TMacro macro) 
    {
        if (GetMacroFor(macro.GetHashCode()) != null) 
        {
            return false;
        }

        macros.Last!.Value[macro.GetHashCode()] = macro;
        return true;
    }
    public IEnumerator<TMacro> GetEnumerator()
    {
        foreach(IDictionary<int, TMacro> node in macros) 
        {
            foreach(KeyValuePair<int, TMacro> pair in node) yield return pair.Value;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}