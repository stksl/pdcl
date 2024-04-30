using Pdcl.Core.Diagnostics;
namespace Pdcl.Core.Syntax;

internal static class ErrorRecoverer
{
    public static async Task RecoverAsync(Error error, CompilationContext ctx) 
    {
        await Task.Run(null!);
    } 
}