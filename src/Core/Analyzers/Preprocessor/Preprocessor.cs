using System.Collections.Immutable;
using System.Data;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
/// <summary>
/// Substitutes preprocessor directives with string tokens.
/// Can be turned off.
/// </summary>
public sealed partial class Preprocessor
{
    public const char DirectiveLiteral = '#';
    private readonly SourceStream stream;
    private readonly PreprocContext context;
    private Dictionary<int, ISyntaxTrivia> trivias;
    public ImmutableDictionary<int, ISyntaxTrivia> Trivias => trivias.ToImmutableDictionary();
    public Preprocessor(SourceStream stream_, PreprocContext context_)
    {
        stream = stream_;
        context = context_;

        trivias = new Dictionary<int, ISyntaxTrivia>();
    }
    private int handleComment(bool isSingleLine)
    {
        int len = 0;
        if (isSingleLine)
        {
            while (!stream.EOF && stream.Advance() != '\n') len++;
        }
        else
        {
            string prev = stream.Peek().ToString();
            while (!stream.EOF && prev + stream.Peek() != "*/")
            {
                len++;
                prev = stream.Advance().ToString();
            }
            stream.Position++;
        }
        return len;
    }
    private string? handleText()
    {
        if (!char.IsLetter(stream.Peek()))
        {
            return null;
        }
        StringBuilder sb = new StringBuilder();

        while (!stream.EOF && char.IsLetterOrDigit(stream.Peek()) || stream.Peek() == '_')
        {
            sb.Append(stream.Advance());
        }
        return sb.ToString();
    }
    // consumes every symbol till the end of the line
    private string handleMacroText()
    {
        StringBuilder sb = new StringBuilder();
        while (!stream.EOF && stream.Peek() != '\n')
        {
            sb.Append(stream.Advance());
        }
        stream.Position++;

        return sb.ToString();
    }
    /// <summary>
    /// returns whether leading trivia was skipped or not
    /// </summary>
    /// <param name="otherTrivia"></param>
    /// <returns></returns>
    private bool handleLeadingTrivia(bool handleNewline = true)
    {
        int pos = (int)stream.Position;
        string trivia = handleNewline ? "/\n " : "/ ";
        bool res = false;
        int length = 0;
        while (trivia.Contains(stream.Peek()))
        {
            if (stream.Peek() == '/')
            {
                stream.Position++;
                if (stream.Peek() == '/')
                {
                    stream.Position--;
                    length += handleComment(isSingleLine: true);
                }
                else if (stream.Peek() == '*')
                {
                    stream.Position--;
                    length += handleComment(isSingleLine: false);
                }
                else { stream.Position--; break; } // slash-non-comment was skipped so we're restoring
            }
            else if (trivia.Substring(1).Contains(stream.Peek()))
            {
                int temp = (int)stream.Position;
                while (!stream.EOF && trivia.Substring(1).Contains(stream.Peek()))
                {
                    stream.Position++;
                }
                length += (int)stream.Position - temp;
            }
            else break;
            res = true;
        }
        if (res) trivias[pos] = new SyntaxTrivia(new TextPosition(pos, length));

        return res;
    }
    private bool firstTokenSeen = false;
    /// <summary>
    /// Skips all characters marking comments as trivia on the way while directive token is not found or EOF
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public PreprocResult<IDirective?> NextDirective()
    {
        while (stream.Peek() != DirectiveLiteral)
        {
            if (stream.EOF)
                return new PreprocResult<IDirective?>(null, PreprocStatusCode.EOF);
            if (!handleLeadingTrivia())
            {
                firstTokenSeen = true;
                stream.Position++;
            }
        }
        stream.Position++;
        return parseDirective();
    }
    private PreprocResult<IDirective?> parseDirective()
    {
        int pos = (int)stream.Position - 1; // position on '#' token

        string? dirName = handleText();
        if (dirName != null && handleLeadingTrivia())
        {
            switch (dirName)
            {
                case "def":
                    if (firstTokenSeen)
                        return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonFirstToken);
                    return PreprocResult.CreateCovarient<IDirective?, Macro?>(parseMacro(pos));
                case "ifdef":
                    return PreprocResult.CreateCovarient<IDirective?, Ifdef?>(parseIfdef(pos));
                case "else":
                    return PreprocResult.CreateCovarient<IDirective?, Else?>(parseElse(pos));
                case "endif":
                    return PreprocResult.CreateCovarient<IDirective?, EndIf?>(parseEndif(pos));

            }
        }
        return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonExistingDirective);
    }
    private PreprocResult<Ifdef?> parseIfdef(int startPos)
    {
        // #ifdef
        string? macroName = handleText();
        if (macroName == null)
        {
            return new PreprocResult<Ifdef?>(null, PreprocStatusCode.UnknownDeclaration);
        }
        Ifdef ifdef = new Ifdef(macroName,
            new TextPosition(startPos, (int)stream.Position - startPos),
            context.Macros.GetMacroFor(macroName) != null);

        return new PreprocResult<Ifdef?>(ifdef,
            handleLeadingTrivia() ? PreprocStatusCode.Success : PreprocStatusCode.UnknownDeclaration);
    }
    private PreprocResult<Else?> parseElse(int startPos)
    {
        // #else
        return new PreprocResult<Else?>(new Else(new TextPosition(startPos, (int)stream.Position - startPos)), PreprocStatusCode.Success);
    }
    private PreprocResult<EndIf?> parseEndif(int startPos)
    {
        // #endif
        return new PreprocResult<EndIf?>(new EndIf(new TextPosition(startPos, (int)stream.Position - startPos)), PreprocStatusCode.Success);
    }
    private PreprocResult<Macro?> parseMacro(int startPos)
    {
        // #def
        string? name = handleText();

        if (!handleLeadingTrivia(handleNewline: false) || name == null)
            return new PreprocResult<Macro?>(null, PreprocStatusCode.UnknownDeclaration);
        else if (context.Macros.GetMacroFor(name) != null)
            return new PreprocResult<Macro?>(null, PreprocStatusCode.AlreadyDefined);

        if (stream.Peek() == '(')
        {
            stream.Position++;
            List<string> argNames = new List<string>();
            while (stream.Peek() != ')')
            {
                handleLeadingTrivia(handleNewline: false) ;
                string? argName = handleText();
                if (stream.EOF || argName == null)
                    return new PreprocResult<Macro?>(null, PreprocStatusCode.UnknownDeclaration);

                if (stream.Peek() == ',') { argNames.Add(argName); stream.Position++; }
            }
            stream.Position++;
            return new PreprocResult<Macro?>(
                new ArgumentedMacro(name, handleMacroText(), argNames, new TextPosition(startPos, (int)stream.Position - startPos)),
                PreprocStatusCode.Success);
        }
        handleLeadingTrivia();
        return new PreprocResult<Macro?>(
            new Macro(name, handleMacroText(), new TextPosition(startPos, (int)stream.Position - startPos)),
            PreprocStatusCode.Success);
    }
    public enum PreprocStatusCode
    {
        Success = 0,

        EOF,
        UnknownDeclaration,
        NonExistingDirective,

        // macros
        NonFirstToken,
        AlreadyDefined,
        // ifdef
        NonExistingMacro
    }
}