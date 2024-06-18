namespace Pdcl.Core.Preproc;

internal sealed partial class Preprocessor
{
    public enum PreprocStatusCode : int
    {
        Success = 0,

        UnknownDeclaration,
        NonExistingDirective,
        UnmatchingToken,
        // macros
        NonFirstToken,
        AlreadyDefined,
        // ifdef
        NonExistingMacro
    }
}