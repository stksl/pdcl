using System.Collections.Immutable;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

/// <summary>
/// Preprocessoring context, contains various tables during preproc phase.
/// </summary>
public sealed class PreprocContext 
{
    /// <summary>
    /// All macros defined throughout the project.
    /// </summary>
    public readonly LinkedList<IDictionary<int, Macro>> Macros;
    /// <summary>
    /// Directives declared in the file.
    /// </summary>
    public readonly IDictionary<int, IDirective> Directives;
    public readonly IDictionary<int, NonDefinedMacro> NonDefMacros;
    public PreprocContext(LinkedList<IDictionary<int, Macro>>? prevMacros)
    {
        Directives = new Dictionary<int, IDirective>();
        NonDefMacros = new Dictionary<int, NonDefinedMacro>();

        prevMacros ??= new LinkedList<IDictionary<int, Macro>>();
        prevMacros.AddLast(new LinkedListNode<IDictionary<int, Macro>>(new Dictionary<int, Macro>()));

        Macros = new LinkedList<IDictionary<int, Macro>>(prevMacros);
    }

    public Macro? FindMacro(int key) 
    {
        foreach(var dict in Macros) 
        {
            if (dict.TryGetValue(key, out Macro? macro))
                return macro;
        }
        return null;
    }
    public IDirective? SkipFromDirective(int key, int offset) 
    {
        int[] keys = Directives.Keys.ToArray();

        int off = key + offset;
        return off < keys.Length ? Directives[keys[off]] : null;
    }
}
