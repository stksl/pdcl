using System.Text;

namespace Pdcl.Core.Preproc;

public static class MacroExtension 
{
    /// <summary>
    /// Substitutes argument occcurencies to <paramref name="arg"/>'s value
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string InsertArgument(this ArgumentedMacro macro, string arg, int index) 
    {
        string sub = macro.Substitution;
        StringBuilder sb = new StringBuilder(sub);
        for(int i = 0; i < sub.Length; i++) 
        {
            if (i < sub.Length - 1 && char.IsLetter(sub[i])) 
            {
                i++;
                // skipping text
                while (char.IsLetterOrDigit(sub[i]) || sub[i] == '_') i++;
            }

            sb.Replace(macro.ArgNames[index], arg, i, macro.ArgNames[index].Length);

        }
        return sb.ToString();
    }
}