
using Pdcl.Core.Text;

namespace Pdcl.Core;

internal sealed partial class Lexer 
{
    private SourceStream? _swapStream;
    private void switchMode(LexerMode mode) 
    {
        if (Mode == mode) return;
        
        switch(mode) 
        {
            case LexerMode.MacroLexing:
                _swapStream = stream;
                stream = new SourceStream(macroSubstitution!.ToCharArray()) {IgnoreEOF = true};
                stream.line = _swapStream.line;
                break;
            case LexerMode.Default:
                _swapStream!.line = stream.line;
                stream.Dispose();
                stream = _swapStream!;
                macroSubstitution = null;
                break;
        }

        Mode = mode;
    }
}