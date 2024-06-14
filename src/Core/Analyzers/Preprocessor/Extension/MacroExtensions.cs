using System.Text;

namespace Pdcl.Core.Preproc;

public static class MacroExtension
{
    /// <summary>
    /// Substitutes argument occcurencies to <paramref name="arg"/> value
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string InsertArgument(this ArgumentedMacro macro, string arg, int index)
    {
        string sub = macro.Substitution;
        StringBuilder sb = new StringBuilder(sub);
        int offset = 0;
        for (int i = 0; i < sub.Length; i++)
        {
            if (!char.IsLetter(sub[i])) continue;

            int prev = i;
            while ((i < sub.Length) && (char.IsLetterOrDigit(sub[i]) || sub[i] == '_')) i++;
            string str = sub.Substring(prev, i - prev);
            if (str == macro.ArgNames[index])
            {
                sb.Replace(str, arg, prev + offset, str.Length);

                offset += arg.Length - macro.ArgNames[index].Length;
            }
        }
        return sb.ToString();
    }
}