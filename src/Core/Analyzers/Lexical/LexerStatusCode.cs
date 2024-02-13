namespace Pdcl.Core;
internal sealed partial class Lexer
{
    public enum LexerStatusCode : int
    {
        Success = 0,

        OutOfLiteralRange, // when parsing hex or binary numbers
        EscapeExpected, // for string/char literals
        PreprocError,
        EOF,
    }
}