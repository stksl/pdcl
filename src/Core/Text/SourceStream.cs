using System.Text;
using Pdcl.Core.Syntax;

namespace Pdcl.Core.Text;
public interface ISourceStream 
{
    long Position {get;}
    char Advance();
    char Peek();
}
/// <summary>
/// A .pdcl file stream reader
/// </summary>
public sealed class SourceStream : ISourceStream, IDisposable
{
    private FileStream _stream;
    public SourceStream(string path)
    {
        if (!path.EndsWith(".pdcl")) 
            throw new ArgumentException("Not a .pdcl file");

        _stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        
    }

    public long Position 
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }
    public bool EOF => Position == _stream.Length;
    public char Peek() 
    {
        int c = _stream.ReadByte(); 
        if (!EOF) Position--;
        return (char)c;
    }
    public char Advance() 
    {
        return (char)_stream.ReadByte();
    }
    public string Advance(int len, out int read) 
    {
        byte[] buf = new byte[len];
        read = _stream.Read(buf, 0, len);
        return Encoding.ASCII.GetString(buf);
    }
    private int handleComment(bool isSingleLine)
    {
        int len = 0;
        if (isSingleLine)
        {
            while (!EOF && Advance() != '\n') len++;
        }
        else
        {
            string prev = Peek().ToString();
            while (!EOF && prev + Peek() != "*/")
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
    public bool handleLeadingTrivia(out ISyntaxTrivia? syntaxTrivia, bool handleNewline = true)
    {
        int pos = (int)Position;
        string trivia = handleNewline ? "/\n " : "/ ";
        bool res = false;
        int length = 0;
        while (trivia.Contains(Peek()))
        {
            if (Peek() == '/')
            {
                Position++;
                if (Peek() == '/')
                {
                    Position--;
                    length += handleComment(isSingleLine: true);
                }
                else if (Peek() == '*')
                {
                    Position--;
                    length += handleComment(isSingleLine: false);
                }
                else { Position--; break; } // slash-non-comment was skipped so we're restoring
            }
            else if (trivia.Substring(1).Contains(Peek()))
            {
                int temp = (int)Position;
                while (!EOF && trivia.Substring(1).Contains(Peek()))
                {
                    Position++;
                }
                length += (int)Position - temp;
            }
            else break;
            res = true;
        }

        syntaxTrivia = res ? new SyntaxTrivia(new TextPosition(pos, length)) : null;
        return res;
    }
    public void Dispose() => _stream.Dispose();
} 