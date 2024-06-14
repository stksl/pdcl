using System.Collections.Immutable;
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

    public Dictionary<string, Macro> DefinedMacros { get; internal set; } // macros that were defined in the file
    public Dictionary<string, Macro> DependentMacros { get; internal set; } // macros that are loaded after the 'use' instruction  
    public Preprocessor(SourceStream stream_)
    {
        stream = stream_;

        DefinedMacros = new Dictionary<string, Macro>();
        DependentMacros = new Dictionary<string, Macro>();
    }
    private IAnalyzerResult<TDir, PS_Code> success<TDir>(TDir directive) where TDir : IDirective
        => new AnalyzerResult<TDir, PS_Code>(directive, PS_Code.Success);
    private IAnalyzerResult<TDir, PS_Code> failed<TDir>(PS_Code code, TDir? directive = null) where TDir : IDirective
        => new AnalyzerResult<TDir, PS_Code>(directive, code);

    internal bool ContainsMacro(string name, out Macro? result)
    {
        result = DefinedMacros.ContainsKey(name) ? DefinedMacros[name] : DependentMacros.ContainsKey(name) ? DependentMacros[name] : null;
        return result != null;
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
    // consumes every symbol untill matches an enclosing token
    private string handleMacroText(bool enclosed)
    {
        if (enclosed) stream.Position++;
        char enclosingToken = enclosed ? ']' : '\n';
        StringBuilder sb = new StringBuilder();
        while (!stream.EOF && stream.Peek() != enclosingToken)
        {
            if (stream.Peek() != ' ')
                stream.handleLeadingTrivia(handleNewline: enclosed);

            if (stream.Peek() != enclosingToken)
                sb.Append(stream.Advance());
        }
        stream.Position++;
        return sb.ToString();
    }
    internal bool firstTokenSeen = false; // lexer manages the value of the field instead
    public async Task<IAnalyzerResult<IDirective, PS_Code>> TryParseDirective(bool useMetadata = true)
    {
        //metadata indicates whether macros will be saved during parsing etc.
        if (stream.Peek() != DirectiveLiteral)
            return failed<IDirective>(PS_Code.UnmatchingToken, null);
            
        if (stream.EOF) return failed<IDirective>(PS_Code.EOF, null);
        stream.Position++;
        return await parseDirectiveAsync(useMetadata);
    }
    internal string? TryParseMacro(Macro macro)
    {
        if (macro is not ArgumentedMacro argedMacro)
        {
            return macro.Substitution;
        }        
        string sub = macro.Substitution;

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
                stream.handleLeadingTrivia();
                // reading all symbols and saving as passed arguments, lexer will have to parse them
                passedArg.Append(stream.Advance());
            }
            sub = argedMacro.InsertArgument(passedArg.ToString(), i);
            argedMacro = new ArgumentedMacro(argedMacro.Name, sub, argedMacro.ArgNames, argedMacro.Position);
            passedArg.Clear();

            stream.Advance();
        }
        stream.Advance();
        return sub;
    }
    private async Task<IAnalyzerResult<IDirective, PS_Code>> parseDirectiveAsync(bool useMetadata)
    {
        int pos = stream.Position - 1; // position on '#' token

        string? dirName = handleText();
        if (dirName != null && stream.handleLeadingTrivia())
        {
            switch (dirName)
            {
                case "def":
                    var macroResult = parseMacro(pos);
                    if (!macroResult.IsFailed && useMetadata)
                        DefinedMacros[macroResult.Value!.Name] = macroResult.Value;
                    return macroResult;
                case "use":
                    return parseUseDirective(pos);
                case "ifdef":
                    return await parseIfdefAsync(pos);
                case "ifndef":
                    return await parseIfNotdefAsync(pos);
                case "endif":
                    return parseEndif(pos);

            }
        }
        return failed<IDirective>(PS_Code.NonExistingDirective);
    }
    private IAnalyzerResult<UseDirective, PS_Code> parseUseDirective(int startPos)
    {
        if (!stream.handleLeadingTrivia() || stream.Advance() != '\"')
            return failed<UseDirective>(PS_Code.UnknownDeclaration);

        string? asmName = handleText();
        if (asmName == null)
            return failed<UseDirective>(PS_Code.UnknownDeclaration);

        if (stream.Advance() != '\"')
            return failed<UseDirective>(PS_Code.UnknownDeclaration);

        return success(new UseDirective(asmName, new TextPosition(startPos, stream.Position - startPos)));
    }
    private async Task<IAnalyzerResult<Ifdef, PS_Code>> parseIfdefAsync(int startPos)
    {
        // #ifdef
        string? macroName = handleText();
        if (macroName == null || !stream.handleLeadingTrivia())
        {
            return failed<Ifdef>(PS_Code.UnknownDeclaration);
        }

        bool result = ContainsMacro(macroName!, out _);
        int bodyPos = stream.Position;
        List<IDirective> children = new List<IDirective>();
        do 
        {
            if (stream.EOF)
                return failed<Ifdef>(PS_Code.EOF, null);
            while (stream.Peek() != DirectiveLiteral)
                if (!stream.handleLeadingTrivia()) stream.Position++;
            var child = await TryParseDirective(result); // if the result is false then we're not saving anything else parsed 

            if (child.IsFailed)
                return failed<Ifdef>(child.Status);

            children.Add(child.Value!);
        } while (children[^1] is not EndIf);

        var ifdef = new Ifdef(new TextPosition(startPos, stream.Position - startPos),
                result,
                new TextPosition(bodyPos, stream.Position - children[^1].Position.Length - bodyPos)
        ) { Children = children.ToImmutableList() };
        return success(ifdef);
    }
    private async Task<IAnalyzerResult<IfNotdef, PS_Code>> parseIfNotdefAsync(int startPos)
    {
        var ifDef = await parseIfdefAsync(startPos);
        if (ifDef.IsFailed) return failed<IfNotdef>(ifDef.Status);

        IfNotdef ifNotdef = new IfNotdef(ifDef.Value!.Position,
        !ifDef.Value!.Result, ifDef.Value.BodyPosition) { Children = ifDef.Value.Children };

        return new AnalyzerResult<IfNotdef, PS_Code>(ifNotdef, ifDef.Status);
    }
    private IAnalyzerResult<EndIf, PS_Code> parseEndif(int startPos)
    {
        // #endif
        return new AnalyzerResult<EndIf, PS_Code>(new EndIf(new TextPosition(startPos, stream.Position - startPos)), PS_Code.Success);
    }
    private IAnalyzerResult<Macro, PS_Code> parseMacro(int startPos)
    {
        if (firstTokenSeen)
            return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.NonFirstToken);
        // #def
        string? name = handleText();

        if (name == null || !stream.handleLeadingTrivia(handleNewline: false))
            return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.UnknownDeclaration);
        else if (ContainsMacro(name, out _))
            return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.AlreadyDefined);
        Macro output;
        if (stream.Peek() == '(')
        {
            stream.Position++;
            List<string> argNames = new List<string>();
            while (stream.Peek() != ')')
            {
                stream.handleLeadingTrivia(handleNewline: false);
                string? argName = handleText();
                stream.handleLeadingTrivia(handleNewline: false);

                argNames.Add(argName!);
                if (stream.Peek() == ')') break;

                if (stream.EOF || argName == null || stream.Peek() != ',')
                    return new AnalyzerResult<Macro, PS_Code>(null, PS_Code.UnknownDeclaration);
                
                stream.Position++;
            }
            stream.Position++;
            stream.handleLeadingTrivia();
            output = new ArgumentedMacro(name, handleMacroText(stream.Peek() == '['), argNames, new TextPosition(startPos, stream.Position - startPos));

        }
        else
        {
            stream.handleLeadingTrivia();
            output = new Macro(name, handleMacroText(stream.Peek() == '['), new TextPosition(startPos, stream.Position - startPos));
        }
        return success(output);
    }
}