using System.Text;

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

    public void Dispose() => _stream.Dispose();
} 