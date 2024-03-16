namespace Pdcl.Core.Diagnostics;
public interface IDiagnostic 
{
    string Description {get;}
    int Identifier {get;}
    int Line {get;}

    string ToString();
}