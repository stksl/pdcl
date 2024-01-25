namespace Pdcl.Core.Preproc;

internal sealed partial class Preprocessor
{
    public enum PreprocStatusCode : int
    {
        Success = 0,

        EOF,
        UnknownDeclaration,
        NonExistingDirective,

        // macros
        NonFirstToken,
        AlreadyDefined,
        // ifdef
        NonExistingMacro
    }
}