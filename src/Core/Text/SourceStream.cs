using System.Text;
using Pdcl.Core.Syntax;

namespace Pdcl.Core.Text;
public interface ISourceStream 
{
    int Position {get;}
    char Advance();
    char Peek();
}
/// <summary>
/// A .pdcl file stream reader
/// </summary>
public sealed class SourceStream : ISourceStream, IDisposable
{
    private Stream _stream;
    public SourceStream(string path)
    {
        if (!path.EndsWith(".pdcl")) 
            throw new ArgumentException("Not a .pdcl file");
        _stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        
    }
    /// <summary>
    /// Saves stream in-memory instead of opening a file
    /// </summary>
    /// <param name="memoryString"></param>
    internal SourceStream(char[] memoryString) 
    {
        byte[] bytes = Encoding.UTF8.GetBytes(memoryString);
        _stream = new MemoryStream(bytes, 0, bytes.Length);
    }
    public int Position 
    {
        get => (int)_stream.Position;
        set => _stream.Position = value;
    }
    public bool EOF => Position >= _stream.Length;
    public char Peek() 
    {
        int pos = Position;
        char c = (char)_stream.ReadByte();
        Position = pos;
        return c;
    }
    public char Advance() 
    {
        return (char)_stream.ReadByte();
    }
    public string Advance(int length, out int bytesRead) 
    {
        byte[] buf = new byte[length];
        int read = _stream.Read(buf, 0, length);
        bytesRead = read; 
        return Encoding.ASCII.GetString(buf);
    }
    private int handleComment(bool isSingleLine)
    {
        int len = 0;
        if (isSingleLine)
        {
            while (Advance() != '\n') len++;
        }
        else
        {
            string prev = Peek().ToString();
            while (prev + Peek() != "*/")
            {
                len++;
                prev = Advance().ToString();
            }
            Position++;
        }
        return len;
    }
    /// <summary>
    /// returns whether leading trivia was skipped or not
    /// </summary>
    /// <param name="otherTrivia"></param>
    /// <returns></returns>
    public bool handleLeadingTrivia(out ISyntaxTrivia? syntaxTrivia, out int linesSkipped, bool handleNewline = true)
    {
        int pos = Position;
        linesSkipped = 0;
        bool res = false;
        while ("/\n ".Contains(Peek()))
        {
            switch(Peek()) 
            {
                case '/':
                    Position++;
                    if (Peek() == '/')
                    {
                        Position--;
                        handleComment(isSingleLine: true);
                    }
                    else if (Peek() == '*')
                    {
                        Position--;
                        handleComment(isSingleLine: false);
                    }
                    else { Position--; goto _ret; } // slash-non-comment was skipped so we're restoring
                    break;
                case ' ':
                    Position++;
                    break;
                case '\n':
                    if (!handleNewline) goto _ret;
                    linesSkipped++;
                    Position++;
                    break;
                default: goto _ret;
            }
            res = true;
        }
        _ret:
        syntaxTrivia = res ? new SyntaxTrivia(new TextPosition(pos, Position - pos)) : null;
        return res;
    }
    public void Dispose() => _stream.Dispose();
} 