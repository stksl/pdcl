namespace Pdcl.Core.Diagnostics;
public interface IDiagnostic 
{
    string Description {get; init;}
    int Identifier {get; init;}
    int Line {get; init;}

    string ToString();
}