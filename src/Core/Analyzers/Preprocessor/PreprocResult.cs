namespace Pdcl.Core.Preproc;

public sealed partial class Preprocessor
{
    public sealed class PreprocResult<T>
    {
        public bool IsFailed => StatusCode != 0;
        public readonly T Value;
        public readonly PreprocStatusCode StatusCode;
        public PreprocResult(T val, PreprocStatusCode statusCode)
        {
            Value = val;
            StatusCode = statusCode;
        }
    }
    public static class PreprocResult
    {
        public static PreprocResult<TBase> CreateCovarient<TBase, TImpl>(PreprocResult<TImpl> from)
        where TImpl : TBase
        {
            return new PreprocResult<TBase>(from.Value, from.StatusCode);
        }
    }
}