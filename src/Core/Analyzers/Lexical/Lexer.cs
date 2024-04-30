using System.Collections.Immutable;
using System.Text;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;
namespace Pdcl.Core;
internal sealed partial class Lexer
{
    private readonly SourceStream stream;
    private readonly PreprocContext context;
    private object _lock = new object();

    // endif directive position and length (in case inside a branched directive to skip #endif)
    private TextPosition endifPos;
    public int currLine { get; private set; }
    public Lexer(SourceStream _stream, PreprocContext ctx)
    {
        stream = _stream;
        context = ctx;
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> success(SyntaxKind kind, int startPos, string? raw, ulong metadata = 0)
        => new AnalyzerResult<SyntaxToken?, LexerStatusCode>(
            new SyntaxToken(
                kind,
                new LexemeMetadata(new TextPosition(startPos, stream.Position - startPos), currLine, raw!, metadata)),
            LexerStatusCode.Success);
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> EOF() =>
        new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.EOF);
    public IAnalyzerResult<SyntaxToken?, LexerStatusCode> Lex()
    {
        lock (_lock)
        {
            return lex();
        }
    }
    // starts the lexing pipeline
    /*
        lex() => lexOperators() => lexNonOperators() => lexText() => badToken
    */
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lex()
    {
        if (stream.EOF) return EOF();

        // checking basic trivia (whitespace or newline)
        switch (stream.Peek())
        {
            case ' ':
            case '\n':
                stream.handleLeadingTrivia(out ISyntaxTrivia? trivia, out int linesSkipped);
                currLine += linesSkipped;
                return success(SyntaxKind.TriviaToken, trivia!.Position.Position, null);

        }
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
                if (stream.handleLeadingTrivia(out ISyntaxTrivia? trivia, out int linesSkipped))
                {
                    currLine += linesSkipped;
                    return success(SyntaxKind.TriviaToken, trivia!.Position.Position, null);
                }
                else return success(SyntaxKind.SlashToken, stream.Position++, "/");
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
            case '#':
                if (context.Directives.TryGetValue(stream.Position, out IDirective? directive))
                {
                    handleDirective(directive);
                    return lex();
                }
                else if (endifPos.Position == stream.Position)
                {
                    stream.Position += endifPos.Length;
                    return lex();
                }
                return success(SyntaxKind.HashToken, stream.Position++, "#");
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

        if (context.NonDefMacros.TryGetValue(stream.Position, out NonDefinedMacro? nonDefMacro))
        {
            stream.Position += nonDefMacro.Position.Length;
            return success(SyntaxKind.MacroSubstitutedToken, stream.Position, nonDefMacro.Substitution);
        }

        StringBuilder rawText = new StringBuilder();
        while (char.IsLetterOrDigit(stream.Peek()) || stream.Peek() == '_')
        {
            rawText.Append(stream.Advance());
        }
        string raw = rawText.ToString();
        if (SyntaxKeywords.ContainsKey(raw))
            return lexKeywords(raw);


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
                if (stream.EOF) return EOF();
                sb.Append(res.Value!.Value.Metadata.Raw);
                stream.handleLeadingTrivia(out _, out int linesSkipped);
                currLine += linesSkipped;
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
        /*
            1 => integer
            1 << 1 => long literal (l/L)

            1 << 2 => float32 literal (f/F)
            1 << 3 => float64

            1 << 4 => unsigned
            
        */
        ulong numberMetadata = 0b1;
        while (char.IsDigit(stream.Peek()))
        {
            sb.Append(stream.Advance());

            if (stream.Peek() == '.')
            {
                sb.Append(stream.Advance());

                if (numberMetadata == 1 << 3)
                    return new AnalyzerResult<SyntaxToken?, LexerStatusCode>(null, LexerStatusCode.OutOfLiteralRange);

                numberMetadata |= 1 << 3;
                numberMetadata ^= 0b1;
            }
        }

        int suffixSize = 0;
        if ("fF".Contains(stream.Peek()))
        {
            numberMetadata |= 1 << 2;
            stream.Position++;
            numberMetadata ^= 1 << 3;
            suffixSize++;
        }
        else
        {
            if ("uU".Contains(stream.Peek()))
            {
                numberMetadata |= 1 << 4;
                stream.Position++;
                suffixSize++;
            }
            if ("lL".Contains(stream.Peek()))
            {
                numberMetadata |= 1 << 1;
                stream.Position++;
                suffixSize++;
            }
        }

        string raw = sb.ToString();
        return success(SyntaxKind.NumberToken, stream.Position - raw.Length - suffixSize, raw, numberMetadata);
    }
    private IAnalyzerResult<SyntaxToken?, LexerStatusCode> lexStringLiteral()
    {
        StringBuilder rawValue = new StringBuilder();
        while (stream.Peek() != '\"')
        {
            if (stream.EOF) return EOF();
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
        char val = '\0';
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
        else val = stream.Peek();
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
            case '0' when isChar:
                return '\0';
        }

        return null;
    }

    private void handleDirective(IDirective dir)
    {
        switch (dir)
        {
            case BranchedDirective branched:
                if (!branched.Result)
                {
                    goto default;
                }
                stream.Position = branched.BodyPosition.Position;
                int endifpos_ = stream.Position + branched.BodyPosition.Length;

                endifPos = new TextPosition(endifpos_, dir.Position.Position + dir.Position.Length - endifpos_);
                break;
            default:
                stream.Position += dir.Position.Length;
                break;
        }
    }
}