using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
/// <summary>
/// Substitutes preprocessor directives with string tokens.
/// Can be turned off.
/// </summary>
internal sealed class Preprocessor
{
    public const char DirectiveLiteral = '#';
    private readonly SourceReader reader;
    private List<PreprocTrivia> commentTrivias;
    public Preprocessor(SourceReader reader_)
    {
        reader = reader_;
        commentTrivias = new List<PreprocTrivia>();
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
            while (!reader.EOF && reader.Advance() != '*' && reader.Advance() != '/') len++;
        }
        return len;
    }
    private string handleText() 
    {
        if (!char.IsLetter(reader.Peek())) 
        {
            return null!;
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
            if (reader.Advance() == '/')
                commentTrivias.Add(new PreprocTrivia((int)reader.Position, handleComment(isSingleLine: true)));
            else if (reader.Peek() == '*')
                commentTrivias.Add(new PreprocTrivia((int)reader.Position, handleComment(isSingleLine: false)));
            else { reader.Position--; return false; } // slash-non-comment was skipped so we're restoring
        }
        else if (otherTrivia.Contains(reader.Peek())) reader.Advance();
        else return false;

        return true | handleLeadingTrivia();
    }
    /// <summary>
    /// Skips all characters, deleting comments on the way, while directive token is not found or EOF
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public PreprocResult<IDirective?> NextDirective()
    {
        while (!reader.EOF && reader.Peek() != DirectiveLiteral)
        {
            handleLeadingTrivia();
        }
        if (reader.EOF)
            throw new EndOfStreamException("Unable to parse the directive at the end of the file.");

        return ParseDirective();
    }
    private PreprocResult<IDirective?> ParseDirective()
    {
        string dirName = handleText();
        if (handleLeadingTrivia()) 
        {
            switch (dirName)
            {
                case "def":
                    return ParseMacro();
                
            }
        }
        return new PreprocResult<IDirective?>(null, PreprocStatusCode.NonExistingDirective);
    }
    private PreprocResult<IDirective?> ParseMacro()
    {
        string name = handleText();

        if (!handleLeadingTrivia(" ")) 
        {
            return new PreprocResult<IDirective?>(null, PreprocStatusCode.UnknownDeclaration);
        }
        
        string substitution = handleMacroText();
        return new PreprocResult<IDirective?>(new Macro(name, substitution), PreprocStatusCode.Success);
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

        UnknownDeclaration,
        NonExistingDirective,
    }
}