namespace Pdcl.Core;
internal sealed class AnalyzerResult<TVal, TStatus> : IAnalyzerResult<TVal, TStatus> where TStatus : Enum
{
    public TVal? Value {get; private set;}
    public TStatus Status {get; private set;}
    public AnalyzerResult(TVal? val, TStatus status)
    {
        Value = val;
        Status = status;
    }
}