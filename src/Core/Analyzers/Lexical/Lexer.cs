using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;
namespace Pdcl.Core;
internal sealed partial class Lexer
{
    public enum LexerMode 
    {
        Default = 0, // default lexing mode, using a fileStream
        MacroLexing = 1, // lexing macro substitution
    }
    public LexerMode Mode {get; private set;}
    internal SourceStream stream {get; private set;}
    private readonly Preprocessor? preproc;
    private Stack<(int, BranchedDirective)> parentDirectives;
    private IDirective? currDirective;
    private int currChildIndex;
    private string? macroSubstitution; // lexer switches on lexing this field when not null (instead of stream)
    private object _lock = new object();
    public Lexer(SourceStream _stream, bool usePreproc)
    {
        stream = _stream;

        parentDirectives = new Stack<(int, BranchedDirective)>();

        preproc = usePreproc ? new Preprocessor(_stream) : null;
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> success(SyntaxKind kind, int startPos, string raw, ulong metadata = 0)
        => new AnalyzerResult<SyntaxToken?, LexerStatusCode>(
            new SyntaxToken(
                kind,
                new LexemeMetadata(new TextPosition(startPos, stream.Position - startPos), stream.line, raw, metadata)),
            LexerStatusCode.Success);
    public IAnalyzerResult<SyntaxToken?, LexerStatusCode> Lex()
    {
        lock (_lock) 
        {
            return preproc == null ? fastLex() : lex();
        }
    }
    // starts the lexing pipeline
    /*
        lex() => lexOperators() => lexNonOperators() => lexText() => badToken
    */
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lex() // slow lex
    {
        stream.handleLeadingTrivia();

        if (stream.EOF && Mode == LexerMode.MacroLexing) 
        {
            switchMode(LexerMode.Default);
            stream.handleLeadingTrivia();
        }

        int directivePosition = currDirective != null ? currDirective is BranchedDirective branched ? 
                branched.BodyPosition.Position + branched.BodyPosition.Length : 
                currDirective.Position.Position + currDirective.Position.Length : 0; 


        if (directivePosition != 0 && stream.Position >= directivePosition) 
        {
            if (currDirective is BranchedDirective br)
                stream.Position = br.Children[^1].Position.Position + br.Children[^1].Position.Length;

            if (parentDirectives.TryPop(out var parentDirective)) 
            {
                currDirective = parentDirective.Item2;
                currChildIndex = parentDirective.Item1;
            } else { currDirective = null; currChildIndex = 0; }
        }

        var result = fastLex();
        preproc!.firstTokenSeen = true;
        return result;
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> fastLex() // direct lexing
    {
        stream.handleLeadingTrivia();

        if (stream.EOF) throw new EndOfStreamException();
        return lexOperators();        
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexOperators()
    {
        // the beginning of the pipeline, checking for basic binary/unary operators
        switch (stream.Peek())
        {
            case '+':
                stream.Position++;
                if (stream.Peek() == '+')
                {
                    stream.Position++;
                    return success(SyntaxKind.IncrementToken, stream.Position - 2, "++");
                }
                return success(SyntaxKind.PlusToken, stream.Position - 1, "+");
            case '-':
                stream.Position++;
                if (stream.Peek() == '-')
                {
                    stream.Position++;
                    return success(SyntaxKind.DecrementToken, stream.Position - 2, "--");
                }

                return success(SyntaxKind.MinusToken, stream.Position - 1, "-");
            case '*':
                return success(SyntaxKind.StarToken, stream.Position++, "*");
            case '/':
                // checking whether it's a comment or not
                if (stream.handleLeadingTrivia())
                    return lex();

                return success(SyntaxKind.SlashToken, stream.Position++, "/");
            case '%':
                return success(SyntaxKind.PercentToken, stream.Position++, "%");
            case '^':
                return success(SyntaxKind.CaretToken, stream.Position++, "^");
            case ',':
                return success(SyntaxKind.CommaToken, stream.Position++, ",");
            case '.':
                return success(SyntaxKind.DotToken, stream.Position++, ".");
            case '=':
                stream.Position++;
                if (stream.Peek() == '=')
                {
                    stream.Position++;
                    return success(SyntaxKind.IsEqualToken, stream.Position - 2, "==");
                }
                return success(SyntaxKind.EqualToken, stream.Position - 1, "=");
            case '!':
                stream.Position++;
                if (stream.Peek() == '=')
                {
                    stream.Position++;
                    return success(SyntaxKind.NotEqualToken, stream.Position - 2, "!=");
                }
                return success(SyntaxKind.ExclamationToken, stream.Position - 1, "!");

            case '&':
                stream.Position++;
                if (stream.Peek() == '&')
                {
                    stream.Position++;
                    return success(SyntaxKind.ShortAndToken, stream.Position - 2, "&&");
                }
                return success(SyntaxKind.AmpersandToken, stream.Position - 1, "&");
            case '|':
                stream.Position++;
                if (stream.Peek() == '|')
                {
                    stream.Position++;
                    return success(SyntaxKind.ShortOrToken, stream.Position - 2, "||");
                }
                return success(SyntaxKind.PipeToken, stream.Position - 1, "|");
            case '<':
                stream.Position++;
                if (stream.Peek() == '<')
                {
                    stream.Position++;
                    return success(SyntaxKind.LeftShiftToken, stream.Position - 2, "<<");
                }
                return success(SyntaxKind.LessThenToken, stream.Position - 1, "<");
            case '>':
                stream.Position++;
                if (stream.Peek() == '>')
                {
                    stream.Position++;
                    return success(SyntaxKind.RightShiftToken, stream.Position - 2, ">>");
                }
                return success(SyntaxKind.GreaterThenToken, stream.Position - 1, ">");

        }
        // continuing the pipeline 
        return lexNonOperators();
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexNonOperators()
    {
        switch (stream.Peek())
        {
            case '(':
                return success(SyntaxKind.OpenParentheseToken, stream.Position++, "(");
            case ')':
                return success(SyntaxKind.CloseParentheseToken, stream.Position++, ")");
            case '[':
                return success(SyntaxKind.OpenBracketToken, stream.Position++, "[");
            case ']':
                return success(SyntaxKind.CloseBracketToken, stream.Position++, "]");
            case '{':
                return success(SyntaxKind.OpenBraceToken, stream.Position++, "{");
            case '}':
                return success(SyntaxKind.CloseBraceToken, stream.Position++, "}");
            case '#' when Mode == LexerMode.Default && preproc != null: 
                
                if (!handleDirective())
                    return new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.PreprocError);

                return lex();
            case '\\':
                return success(SyntaxKind.BackslashToken, stream.Position++, "\\");
            case ';':
                return success(SyntaxKind.SemicolonToken, stream.Position++, ";");
            case '\"':
                stream.Position++;
                return lexStringLiteral();
            case '\'':
                return lexCharLiteral();
            case >= '0' and <= '9':
                return lexNumber();
        }
        // pipeline
        return lexText();
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexText()
    {
        if (!char.IsLetter(stream.Peek()))
        {
            return lexUnknownToken();
        }

        StringBuilder rawText = new StringBuilder();
        while (char.IsLetterOrDigit(stream.Peek()) || stream.Peek() == '_')
        {
            rawText.Append(stream.Advance());
        }
        string raw = rawText.ToString();
        if (SyntaxKeywords.ContainsKey(raw))
            return lexKeywords(raw);
        else if (preproc != null && preproc.ContainsMacro(raw, out Macro? macro)) 
        {
            macroSubstitution = preproc.TryParseMacro(macro!);

            switchMode(LexerMode.MacroLexing);
            return lex();
        }

        return success(SyntaxKind.TextToken, stream.Position - raw.Length, raw);
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexUnknownToken()
    {
        var badToken = success(SyntaxKind.BadToken, stream.Position, stream.Advance().ToString());

        return badToken;
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexKeywords(string raw)
    {
        if (SyntaxKeywords[raw] == SyntaxKind.IL_InlineToken)
        {
            StringBuilder sb = new StringBuilder();
            stream.Position++;
            IAnalyzerResult<SyntaxToken?, LexerStatusCode> res = lexText();
            while (res.Value!.Value.Kind != SyntaxKind.IL_InlineToken)
            {
                sb.Append(res.Value!.Value.Metadata.Raw);
                stream.handleLeadingTrivia();
                res = lexText();
            }
            sb.Append(res.Value!.Value.Metadata.Raw);
            raw += sb.ToString();
        }
        return success(SyntaxKeywords[raw], stream.Position - raw.Length, raw);
    }

    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexNumber()
    {
        StringBuilder sb = new StringBuilder();
        NumberMetadata numberMetadata = NumberMetadata.Integer;
        while (char.IsDigit(stream.Peek()))
        {
            sb.Append(stream.Advance());

            if (stream.Peek() == '.')
            {
                sb.Append(stream.Advance());

                if (numberMetadata.HasFlag(NumberMetadata.Float64Literal))
                    return new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.OutOfLiteralRange);

                numberMetadata |= NumberMetadata.Float64Literal;
                numberMetadata ^= NumberMetadata.Integer;
            }
        }

        int suffixSize = 1;
        switch(stream.Peek().ToString().ToLower()) 
        {
            case "f":
                numberMetadata |= NumberMetadata.Float32Literal;
                numberMetadata &= ~NumberMetadata.Float64Literal;
                stream.Position++;
                break;
            case "u":
                numberMetadata |= NumberMetadata.Unsigned;
                stream.Position++;
                break;
            case "l":
                numberMetadata |= NumberMetadata.LongLiteral;
                stream.Position++;
                break;
            default:
                suffixSize = 0;
            break;
        }

        string raw = sb.ToString();
        return success(SyntaxKind.NumberToken, stream.Position - raw.Length - suffixSize, raw, (ulong)numberMetadata);
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexStringLiteral()
    {
        StringBuilder rawValue = new StringBuilder();
        while (stream.Peek() != '\"')
        {
            // a possible escape sequence
            if (stream.Peek() == '\\')
            {
                stream.Position++;

                char? escape = parseEscape(isChar: false); // because we're in a string literal.
                if (escape == null)
                    return new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.EscapeExpected);

                stream.Position++;
                rawValue.Append(escape.Value);
            }
            else rawValue.Append(stream.Advance());
        }
        stream.Position++;
        return success(SyntaxKind.StringLiteral, stream.Position - rawValue.Length, rawValue.ToString());
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexCharLiteral()
    {
        char val = stream.Peek();
        stream.Position++;
        int escaped = 0;
        if (stream.Peek() == '\\')
        {
            stream.Position++;
            char? escape = parseEscape(isChar: true);
            if (escape == null)
                return new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.EscapeExpected);
            val = escape.Value;
            escaped = 1;
        }
        stream.Position += 2;
        return success(SyntaxKind.CharLiteral, stream.Position - 3 - escaped, val.ToString());

    }

    private char? parseEscape(bool isChar)
    {
        switch (stream.Peek())
        {
            // a backslash escape
            case '\\':
                return '\\';
            case '\"':
                return '\"';
            case '\'':
                return '\'';
            case 'n':
                return '\n';
            case 't':
                return '\t';
            case '0':
                return '\0';
            default: return null;
        }
    }

    private bool handleDirective()
    {
        if (currDirective == null)
            currDirective = preproc!.TryParseDirective().Result.Value;
        else 
        {
            // meaning that we're in a branched directive that contained child directives
            BranchedDirective branched = (BranchedDirective)currDirective;

            parentDirectives.Push((currChildIndex + 1, branched));

            currDirective = branched.Children[currChildIndex];
            currChildIndex = 0;
        }

        if (currDirective is BranchedDirective branched_ && branched_.Result) 
                stream.Position = branched_.BodyPosition.Position;
        else if (currDirective != null) 
            stream.Position = currDirective.Position.Position + currDirective.Position.Length;

        return currDirective != null;
    }
}