
namespace Pdcl.Core.Syntax;

public enum SyntaxKind : int 
{
    BadToken = -1,

    PlusToken, // +
    MinusToken, // -
    StarToken, // *
    SlashToken, // /
    BackslashToken, // \
    EqualToken, // =
    PercentToken, // %
    DotToken, // .
    CommaToken, // ,
    AmpersandToken, // &
    PipeToken, // |
    ExclamationToken, // !
    QuestionToken, // ?
    SemicolonToken, // ;
    LessThenToken, // <
    GreaterThenToken, // >
    HashToken, // #
    CharLiteral, // '
    StringLiteral, // "
    // bounds
    OpenParentheseToken, // (
    CloseParentheseToken, // )
    OpenBraceToken, // {
    CloseBraceToken, // }
    OpenBracketToken, // [
    CloseBracketToken, // ]

    // additional binary operators
    IsEqualToken, // ==
    IsLessEqualToken, // <=
    IsMoreEqualToken, // >=
    NotEqualToken, // !=
    ShortAndToken, // &&
    ShortOrToken, // || 
    // additional bitwise binary operators
    LeftShiftToken, // <<
    RightShiftToken, // >>


    // trivia
    TriviaToken, // could be a comment, a whitespace or a newline

    TextToken, // text123_
    NumberToken, // 123, 1.23, 0xff, 0b00101

    // keywords
    UseToken, // use
    WhileLoopToken, // while
    ForLoopToken, // for
    IfToken, // if
    ElifToken, // elif
    ElseToken, // else
    PInvToken, // pinv
    StructToken, // struct
    ReturnToken, // return
    NamespaceToken, // namespace
    IL_InlineToken, // il_inline
}