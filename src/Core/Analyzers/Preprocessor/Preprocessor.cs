using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
/// <summary>
/// Substitutes preprocessor directives with string tokens.
/// Can be turned off.
/// </summary>
public sealed class Preprocessor
{
    public const char DirectiveLiteral = '#';
    private readonly SourceReader reader;
    private readonly PreprocContext context;

    private List<PreprocTrivia> commentTrivias;
    public Preprocessor(SourceReader reader_, PreprocContext context_)
    {
        reader = reader_;
        commentTrivias = new List<PreprocTrivia>();
        context = context_;
    }
    private int handleComment(bool isSingleLine)
    {
        int len = 0;
        if (isSingleLine)
        {
            while (!reader.EOF && reader.Advance() != '\n') len++;
        }
        else
        {
            string prev = reader.Peek().ToString();
            while (!reader.EOF && prev + reader.Peek() != "*/") 
            {
                len++; 
                prev = reader.Peek().ToString();
                reader.Advance();
            }
            reader.Advance();
        }
        return len;
    }
    private string? handleText()
    {
        if (!char.IsLetter(reader.Peek()))
        {
            return null;
        }
        StringBuilder sb = new StringBuilder();

        while (!reader.EOF && char.IsLetterOrDigit(reader.Peek()) || reader.Peek() == '_')
        {
            sb.Append(reader.Peek());
            reader.Advance();
        }
        return sb.ToString();
    }
    // consumes every symbol till the end of the line
    private string handleMacroText()
    {
        StringBuilder sb = new StringBuilder();
        while (!reader.EOF && reader.Peek() != '\n')
        {
            sb.Append(reader.Peek());
            reader.Advance();
        }
        reader.Advance();

        return sb.ToString();
    }
    /// <summary>
    /// returns whether leading trivia was skipped or not
    /// </summary>
    /// <param name="otherTrivia"></param>
    /// <returns></returns>
    private bool handleLeadingTrivia(string otherTrivia = " \n")
    {
        if (reader.Peek() == '/')
        {
            reader.Advance();
            if (reader.Peek() == '/')
                commentTrivias.Add(new PreprocTrivia((int)reader.Position, handleComment(isSingleLine: true)));
            else if (reader.Peek() == '*') {
                reader.Advance();
                commentTrivias.Add(new PreprocTrivia((int)reader.Position, handleComment(isSingleLine: false)));
            }
            else { reader.Position--; return false; } // slash-non-comment was skipped so we're restoring
        }
        else if (otherTrivia.Contains(reader.Peek())) reader.Advance();
        else return false;

        return true | handleLeadingTrivia();
    }
    private bool firstTokenSeen = false;
    /// <summary>
    /// Skips all characters marking comments as trivia on the way while directive token is not found or EOF
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public PreprocResult<IDirective?> NextDirective()
    {
        while (reader.Peek() != DirectiveLiteral)
        {
            if (reader.EOF)
                return new PreprocResult<IDirective?>(null, PreprocStatusCode.EOF);
            if (!handleLeadingTrivia()) 
            {
                firstTokenSeen = true;
                reader.Advance();
            }
        }
        reader.Advance();
        return parseDirective();
    }
    private PreprocResult<IDirective?> parseDirective()
    {
        string? dirName = handleText();
        if (dirName != null && handleLeadingTrivia())
        {
            switch (dirName)
            {
                case "def":
                    if (firstTokenSeen) 
                        return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonFirstToken);
                    return parseMacro();
                case "ifdef":
                    return parseIfdef();

            }
        }
        return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonExistingDirective);
    }
    private PreprocResult<IDirective?> parseIfdef() 
    {
        string? macroName = handleText();
        if (macroName == null || context.Macros.GetMacroFor(macroName) == null) 
        {
            return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonExistingMacro);
        }
        Ifdef ifdef = new Ifdef(macroName);
        return new PreprocResult<IDirective?>(ifdef, 
            handleLeadingTrivia(" ") ? PreprocStatusCode.Success : PreprocStatusCode.UnknownDeclaration);
    }
    private PreprocResult<IDirective?> parseMacro()
    {
        string? name = handleText();
        if (!handleLeadingTrivia(" ") || name == null)
        {
            return new PreprocResult<IDirective?>(null, PreprocStatusCode.UnknownDeclaration);
        }
        if (reader.Peek() == '(')
        {
            reader.Advance();
            List<string> argNames = new List<string>();
            while (reader.Peek() != ')')
            {
                handleLeadingTrivia(" ");
                string? argName = handleText();
                if (reader.EOF || argName == null)
                    return new PreprocResult<IDirective?>(null, PreprocStatusCode.UnknownDeclaration);

                if (reader.Peek() == ',') {argNames.Add(argName); reader.Advance();}
            }
            reader.Advance();
            return new PreprocResult<IDirective?>(new ArgumentedMacro(name, handleMacroText(), argNames), 
            PreprocStatusCode.Success);
        }
        handleLeadingTrivia();
        return new PreprocResult<IDirective?>(new Macro(name, handleMacroText()), PreprocStatusCode.Success);
    }
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
    public enum PreprocStatusCode
    {
        Success = 0,

        EOF,
        UnknownDeclaration,
        NonExistingDirective,

        // macros
        NonFirstToken,

        // ifdef
        NonExistingMacro
    }
}