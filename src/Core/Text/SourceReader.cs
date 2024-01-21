namespace Pdcl.Core.Text;
public interface ISourceReader 
{
    char Advance();
    char Peek();
}
/// <summary>
/// A .pdcl file stream reader
/// </summary>
public sealed class SourceReader : ISourceReader, IDisposable
{
    private StreamReader _reader;
    public SourceReader(string path)
    {
        if (!path.EndsWith(".pdcl")) 
            throw new ArgumentException("Not a .pdcl file");
        _reader = new StreamReader(path);
    }

    public long Position 
    {
        get => (int)_reader.BaseStream.Position;
        set => _reader.BaseStream.Position = value;
    }
    public bool EOF => _reader.EndOfStream;
    public char Peek() => (char)_reader.Peek();
    public char Advance() => (char)_reader.Read();
    public string Advance(int len, out int read) 
    {
        char[] buf = new char[len];
        read = _reader.Read(buf, 0, len);
        return new string(buf);
    }
    public async Task<(string, int)> AdvanceAsync(int len) 
    {
        char[] buf = new char[len];
        int read = await _reader.ReadAsync(buf, 0, len);

        return (new string(buf), read);
    }

    public void Dispose() => _reader.Dispose();
} 