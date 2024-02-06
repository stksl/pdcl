using System.Text;
using Pdcl.Core.Text;

using PS_Code = Pdcl.Core.Preproc.Preprocessor.PreprocStatusCode;
namespace Pdcl.Core.Preproc;
/// <summary>
/// Substitutes preprocessor directives with string tokens.
/// Can be turned off.
/// </summary>
internal sealed partial class Preprocessor
{
    public const char DirectiveLiteral = '#';
    private readonly SourceStream stream;
    private readonly PreprocContext context;

    private object _lock = new object();
    public Preprocessor(SourceStream stream_, PreprocContext context_)
    {
        stream = stream_;
        context = context_;

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
    private bool firstTokenSeen = false;
    /// <summary>
    /// Skips all characters marking comments as trivia on the way while directive token is not found or EOF
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public IAnalyzerResult<IDirective, PS_Code> NextDirective()
    {
        lock (_lock)
        {
            while (stream.Peek() != DirectiveLiteral)
            {
                if (stream.EOF)
                    return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.EOF);
                if (!stream.handleLeadingTrivia(out _, out _))
                {
                    firstTokenSeen = true;
                    string? txt = handleText();

                    Macro? macro = txt != null ? context.DefinedMacros.GetMacroFor(txt.GetHashCode()) : null;
                    if (macro == null)
                    {
                        stream.Position++;
                        continue;
                    }

                    NonDefinedMacro? parsed = tryParseMacro(macro!);
                    if (parsed == null)
                    {
                        // report an error
                    } else context.NonDefMacros.InsertMacro(parsed.Value);
                }
            }
            stream.Position++;
            return parseDirective();
        }
    }
    private NonDefinedMacro? tryParseMacro(Macro macro)
    {
        int pos = stream.Position;
        string sub = macro.Substitution;

        if (macro is ArgumentedMacro argedMacro)
        {
            if (stream.Advance() != '(') return null;
            StringBuilder passedArg = new StringBuilder();
            for (int i = 0; stream.Peek() != ')'; i++)
            {
                if (stream.EOF) return null;

                while (stream.Peek() != ',' || stream.Peek() != ')')
                {
                    // reading all symbols and saving as passed arguments, lexer will have to parse them
                    passedArg.Append(stream.Advance());
                }

                sub = argedMacro.InsertArgument(passedArg.ToString(), i);

                passedArg.Clear();

                if (stream.Peek() != ')') stream.Advance();
            }
        }

        return new NonDefinedMacro(new TextPosition(pos, stream.Position - pos), sub);
    }
    private IAnalyzerResult<IDirective, PS_Code> parseDirective()
    {
        int pos = stream.Position - 1; // position on '#' token

        string? dirName = handleText();
        if (dirName != null && stream.handleLeadingTrivia(out _, out _))
        {
            switch (dirName)
            {
                case "def":
                    if (firstTokenSeen)
                        return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.NonFirstToken);
                    return parseMacro(pos);
                case "ifdef":
                    return parseIfdef(pos);
                case "else":
                    return parseElse(pos);
                case "endif":
                    return parseEndif(pos);

            }
        }
        return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.NonExistingDirective);
    }
    private IAnalyzerResult<Ifdef, PS_Code> parseIfdef(int startPos)
    {
        // #ifdef
        string? macroName = handleText();
        if (macroName == null)
        {
            return new AnalyzerResult<Ifdef, PS_Code>(null, PS_Code.UnknownDeclaration);
        }
        Ifdef ifdef = new Ifdef(macroName,
            new TextPosition(startPos, stream.Position - startPos),
            context.DefinedMacros.GetMacroFor(macroName.GetHashCode()) != null);

        return new AnalyzerResult<Ifdef, PS_Code>(ifdef,
            stream.handleLeadingTrivia(out _, out _) ? PS_Code.Success : PS_Code.UnknownDeclaration);
    }
    private IAnalyzerResult<Else, PS_Code> parseElse(int startPos)
    {
        // #else
        return new AnalyzerResult<Else, PS_Code>(new Else(new TextPosition(startPos, stream.Position - startPos)), PS_Code.Success);
    }
    private IAnalyzerResult<EndIf, PS_Code> parseEndif(int startPos)
    {
        // #endif
        return new AnalyzerResult<EndIf, PS_Code>(new EndIf(new TextPosition(startPos, stream.Position - startPos)), PS_Code.Success);
    }
    private IAnalyzerResult<Macro, PS_Code> parseMacro(int startPos)
    {
        // #def
        string? name = handleText();

        if (!stream.handleLeadingTrivia(out _, out _, handleNewline: false) || name == null)
            return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.UnknownDeclaration);
        else if (context.DefinedMacros.GetMacroFor(name.GetHashCode()) != null)
            return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.AlreadyDefined);

        if (stream.Peek() == '(')
        {
            stream.Position++;
            List<string> argNames = new List<string>();
            while (stream.Peek() != ')')
            {
                stream.handleLeadingTrivia(out _, out _, handleNewline: false);
                string? argName = handleText();
                if (stream.EOF || argName == null)
                    return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.UnknownDeclaration);

                if (stream.Peek() == ',')
                {
                    argNames.Add(argName);
                    stream.Position++;
                }
                else if (stream.Peek() == ')') argNames.Add(argName);
            }
            stream.Position++;
            return new AnalyzerResult<Macro, PS_Code>(
                new ArgumentedMacro(name, handleMacroText(), argNames, new TextPosition(startPos, stream.Position - startPos)),
                PS_Code.Success);
        }
        stream.handleLeadingTrivia(out _, out _);
        return new AnalyzerResult<Macro, PS_Code>(
            new Macro(name, handleMacroText(), new TextPosition(startPos, stream.Position - startPos)),
            PS_Code.Success);
    }
}