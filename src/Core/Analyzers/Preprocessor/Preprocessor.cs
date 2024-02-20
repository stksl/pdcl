using System.Text;
using Pdcl.Core.Diagnostics;
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
    private Dictionary<string, Macro> definedMacros;
    public Preprocessor(SourceStream stream_, PreprocContext context_)
    {
        stream = stream_;
        context = context_;

        definedMacros = new Dictionary<string, Macro>();
        foreach (var macros in context.Macros)
        {
            foreach (var macro in macros)
            {
                definedMacros[macro.Value.Name] = macro.Value;
            }
        }
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
    public async Task<IAnalyzerResult<IDirective, PS_Code>> NextDirectiveAsync()
    {
        var res = await nextDirectiveAsync();
        if (!res.IsFailed) 
            context.Directives[res.Value!.Position.Position] = res.Value;
        return res;
    }
    private async Task<IAnalyzerResult<IDirective, PS_Code>> nextDirectiveAsync()
    {
        while (stream.Peek() != DirectiveLiteral)
        {
            if (stream.EOF)
                    return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.EOF);
            if (stream.handleLeadingTrivia(out _, out _)) continue;

            firstTokenSeen = true;

            definedMacros.TryGetValue(handleText() ?? "", out Macro? macro);
            if (macro == null)
            {
                stream.Position++;
                continue;
            }

            NonDefinedMacro? parsed = tryParseMacro(macro!);
            if (parsed == null)
            {
                return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.UnknownDeclaration);
                // report an error
            }
            else context.NonDefMacros[parsed.GetHashCode()] = parsed;
        }
        stream.Position++;
        return await parseDirectiveAsync();
    }
    private NonDefinedMacro? tryParseMacro(Macro macro)
    {
        int pos = stream.Position - macro.Name.Length;
        string sub = macro.Substitution;

        if (macro is ArgumentedMacro argedMacro)
        {
            if (stream.Advance() != '(')
                return null;

            StringBuilder passedArg = new StringBuilder();
            for (int i = 0; stream.Peek() != ')'; i++)
            {
                if (stream.EOF || i > argedMacro.ArgNames.Length) return null;

                while (stream.Peek() != ',')
                {
                    if (stream.Peek() == ')' && i == argedMacro.ArgNames.Length - 1)
                    {
                        stream.Position--;
                        break;
                    }
                    stream.handleLeadingTrivia(out _, out _);
                    // reading all symbols and saving as passed arguments, lexer will have to parse them
                    passedArg.Append(stream.Advance());
                }
                sub = argedMacro.InsertArgument(passedArg.ToString(), i);
                argedMacro = new ArgumentedMacro(argedMacro.Name, sub, argedMacro.ArgNames, argedMacro.Position);
                passedArg.Clear();

                stream.Advance();
            }
            stream.Advance();
        }
        return new NonDefinedMacro(new TextPosition(pos, stream.Position - pos), sub);
    }
    private async Task<IAnalyzerResult<IDirective, PS_Code>> parseDirectiveAsync()
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
                    return await parseIfdefAsync(pos);
                case "ifndef":
                    return await parseIfNotdefAsync(pos);
                case "endif":
                    return parseEndif(pos);

            }
        }
        return new AnalyzerResult<IDirective, PS_Code>(null, PS_Code.NonExistingDirective);
    }
    private async Task<IAnalyzerResult<Ifdef, PS_Code>> parseIfdefAsync(int startPos)
    {
        // #ifdef
        string? macroName = handleText();
        if (macroName == null || !stream.handleLeadingTrivia(out _, out _))
        {
            return new AnalyzerResult<Ifdef, PS_Code>(null, PS_Code.UnknownDeclaration);
        }

        List<IDirective> children = new List<IDirective>();

        int bodyPos = stream.Position; 

        IAnalyzerResult<IDirective?, PS_Code> nextDir = await nextDirectiveAsync();
        while (nextDir.Value is not EndIf)
        {
            if (nextDir.IsFailed)
                return new AnalyzerResult<Ifdef, PS_Code>(null, nextDir.Status);

            children.Add(nextDir.Value!);
            nextDir = await nextDirectiveAsync();
        }
        return new AnalyzerResult<Ifdef, PS_Code>(
            new Ifdef(new TextPosition(startPos, stream.Position - startPos), 
                definedMacros.ContainsKey(macroName), 
                children, new TextPosition(bodyPos, stream.Position - nextDir.Value.Position.Length - bodyPos)),
            PS_Code.Success);
    }
    private async Task<IAnalyzerResult<IfNotdef, PS_Code>> parseIfNotdefAsync(int startPos)
    {
        var ifDef = await parseIfdefAsync(startPos);
        if (ifDef.IsFailed) return new AnalyzerResult<IfNotdef, PS_Code>(null, ifDef.Status);

        IfNotdef ifNotdef = new IfNotdef(ifDef.Value!.Position, 
        !ifDef.Value!.Result, ifDef.Value.GetChildren(), ifDef.Value.BodyPosition);

        return new AnalyzerResult<IfNotdef, PS_Code>(ifNotdef, ifDef.Status);
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
        else if (definedMacros.TryGetValue(name, out Macro? alreadyExists))
            return new AnalyzerResult<Macro, PS_Code>(alreadyExists, PS_Code.AlreadyDefined);
        Macro output;
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

            output = new ArgumentedMacro(name, handleMacroText(), argNames, new TextPosition(startPos, stream.Position - startPos));
        }
        else
        {
            stream.handleLeadingTrivia(out _, out _);
            output = new Macro(name, handleMacroText(), new TextPosition(startPos, stream.Position - startPos));
        }
        definedMacros[name] = output;
        context.Macros.Last!.Value[output.GetHashCode()] = output;
        return new AnalyzerResult<Macro, PS_Code>(output, PS_Code.Success);
    }
}