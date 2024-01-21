namespace Pdcl.Core.Preproc;

public class Macro : IDirective 
{
    public string Name => name;
    private readonly string name;
    public readonly string Substitution;
    public Macro(string name_, string substitution)
    {
        name = name_;
        Substitution = substitution;
    }
}
public sealed class ArgumentedMacro : Macro 
{
    public readonly int ArgLength;
    public ArgumentedMacro(string name, string substitution, int argLen) : base(name, substitution)
    {
        ArgLength = argLen;
    }
}