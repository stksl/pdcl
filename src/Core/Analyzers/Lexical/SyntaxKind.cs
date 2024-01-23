using Pdcl.Core.Syntax;

namespace Pdcl.Core;

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

    SingleQuoteToken, // '
    DoubleQuoteToken, // "

    TextToken, // text123_
    NumberToken, // 123, 1.23, 0xff, 0b00101

    DotToken, // .
    CommaToken, // ,
    AmpersandToken, // &
    ExclamationToken, // !
    QuestionToken, // ?
    SemicolonToken, // ;
    LessThenToken, // <
    MoreThenToken, // >
    HashToken, // #

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

    // trivia
    WhitespaceToken,
    NewlineToken,
    SingleCommentToken,
    MultiCommentToken,
    
    // keywords
    UseToken, // use
    AliasToken, // alias
    WhileLoopToken, // while
    ForLoopToken, // for
    IfToken, // if
    ElifToken, // elif
    ElseToken, // else
    PInvToken, // pinv
    StructToken, // struct
    ReturnToken, // ret
    NamespaceToken, // namespace
}