namespace Pdcl.Core;
public interface IAnalyzerResult<out TVal, TStatus> where TStatus : Enum 
{
    public bool IsFailed => (int)(object)Status != 0;
    TVal? Value {get;}

    /// <summary>
    /// First value has to be Success
    /// </summary>
    TStatus Status {get;}
}