
namespace Pdcl.Core.Syntax;

public enum SyntaxKind : int 
{
    BadToken = -1,
    MissingToken, // parsing phase

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
    CaretToken, // ^
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
    // unary operators
    IncrementToken, // ++
    DecrementToken, // --
    // additional bitwise binary operators
    LeftShiftToken, // <<
    RightShiftToken, // >>


    // trivia
    TriviaToken, // could be a comment, a whitespace or a newline

    TextToken, // text123_
    NumberToken, // 123, 1.23, 0xff, 0b00101
    MacroSubstitutedToken,


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
    ConstToken, // const
    OperatorToken, // operator
    ImplicitOperatorToken, // implicit
    ExplicitOperatorToken, // explicit    
    TrueToken, // true
    FalseToken, // false

    // access modifiers
    PublicToken,
    PrivateToken,
    AssemblyToken,
    FamilyToken,
    StaticToken
}