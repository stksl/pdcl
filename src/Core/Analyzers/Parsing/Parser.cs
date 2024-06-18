using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;
using Pdcl.Core.Assembly;
namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser : IDisposable
{
    public readonly DiagnosticHandler diagnostics;
    public readonly CompilationContext context;
    public readonly AssemblyInfo AssemblyInfo;
    private readonly Lexer lexer;
    private readonly SyntaxNodeBuilder _syntaxBuilder;
    public readonly SyntaxTree tree;
    public volatile string currPath = "/";

    public SyntaxToken CurrentToken { get; private set; }

    public Parser(Lexer _lexer, DiagnosticHandler _handler, AssemblyInfo assembly)
    {
        lexer = _lexer;

        diagnostics = _handler;
        diagnostics.OnDiagnosticReported += onDiagnostic;

        context = new CompilationContext();

        AssemblyInfo = assembly;

        tree = new SyntaxTree();
        _syntaxBuilder = new SyntaxNodeBuilder(_handler, _lexer.stream);
    }

    /// <summary>
    /// Consumes one token and returns it skipping trivia
    /// </summary>
    /// <param name="throw_"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public SyntaxToken ConsumeToken()
    {
        var result = lexer.Lex();

        if (result.IsFailed)
        {
            diagnostics.ReportBadToken(lexer.stream.line, result.Value.GetValueOrDefault());
        }

        CurrentToken = result.Value.GetValueOrDefault();
        return CurrentToken;
    }
    /// <summary>
    /// Consumes when <paramref name="kind"/> is matched
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public SyntaxToken ConsumeToken(SyntaxKind kind)
    {
        return CurrentToken.Kind == kind ? ConsumeToken() : SyntaxToken.Missing;
    }
    public bool IsConsumeMissed(SyntaxKind kind)
    {
        return ConsumeToken(kind).Kind == SyntaxKind.MissingToken;
    }
    public IEnumerable<SyntaxToken> ConsumeTokens(params SyntaxKind[] kinds)
    {
        foreach (SyntaxKind kind in kinds)
        {
            yield return ConsumeToken(kind);
        }
    }
    private void onDiagnostic(IDiagnostic diagnostic)
    {
        if (diagnostic is Error err)
            synchronize(err);
    }
    public SymbolTreeNode GetCurrentTableNode()
    {
        return AssemblyInfo.TableTree.GetNode(currPath)!;
    }
    public void ParseCompilationUnit()
    {
        // checks for all possible top-level nodes, visiting them 

        while (!IsConsumeMissed(SyntaxKind.UseToken))
        {
            UseNode? use = parseUseNode();
            if (use != null) tree.Root.addChild(use);
        }

        string nsName = this.ParseNamespaceName();
        if (nsName != string.Empty) ConsumeToken(SyntaxKind.OpenBraceToken);
        while (!lexer.stream.EOF) 
        {
            try 
            {
                SyntaxNode? topLevelNode = parseCompilationUnitCore();
                if (topLevelNode == null) 
                    diagnostics.ReportUnsuitableSyntaxToken(lexer.stream.line, CurrentToken.Kind, SyntaxKind.TextToken);
                tree.Root.addChild(topLevelNode!);
            } 
            catch (EndOfStreamException) 
            {
                break;
            }
        }
        if (nsName != string.Empty) ConsumeToken(SyntaxKind.CloseBraceToken);
    }
    private SyntaxNode? parseCompilationUnitCore()
    {
        SyntaxNode? node = null;
        switch (CurrentToken.Kind)
        {
            // global constant variable declaration
            case SyntaxKind.ConstToken:
                /* ConstVarNode? constVar = await VisitorFactory.GetVisitorFor<ConstVarNode>(parser.context)!.VisitAsync(parser);
                if (constVar != null)
                {
                    AssemblyInfo.TableTree.Root.Table!.StoreSymbol(
                        new Symbol(constVar.Name, SymbolType.Variable, 0, constVar));
                }
                node = constVar; */
                break;
            // supposably a function declaration
            case SyntaxKind.TextToken:
                break;
            case SyntaxKind.StructToken:
                break;
        }
        return node;
    }
    private UseNode parseUseNode()
    {
        SyntaxToken useToken = ConsumeToken(SyntaxKind.UseToken);

        string nsName = this.ParseNamespaceName();

        SyntaxToken semicolonToken = ConsumeToken(SyntaxKind.SemicolonToken);
        return _syntaxBuilder.UseNode(useToken.Kind, nsName, semicolonToken.Kind);
    }
    private TypeDeclarationNode? parseTypeDeclaration() 
    {
        SyntaxToken structToken = ConsumeToken(SyntaxKind.StructToken);
        SyntaxToken identifier = ConsumeToken(SyntaxKind.TextToken);

        SyntaxToken openBrace = ConsumeToken(SyntaxKind.OpenBraceToken);

        List<FieldDeclaration> fields = new List<FieldDeclaration>();
        List<FunctionMemberDeclaration> functions = new List<FunctionMemberDeclaration>(); 
        while (CurrentToken.Kind != SyntaxKind.CloseBraceToken) 
        {
            MemberNode member = parseMemberNode();
            switch(member.GetType().Name) 
            {
                case nameof(FunctionMemberDeclaration):
                    functions.Add((FunctionMemberDeclaration)member);
                    break;
                case nameof(FieldDeclaration):
                    fields.Add((FieldDeclaration)member);
                    break;
            }
        }
        SyntaxToken closeBrace = ConsumeToken(SyntaxKind.CloseBraceToken);
        return _syntaxBuilder.TypeDeclaration(structToken.Kind, identifier, openBrace.Kind, fields.ToImmutableArray(), functions.ToImmutableArray(), closeBrace.Kind);
    }
    private MemberNode parseMemberNode() 
    {
        AccessModifiers? mods = tryParseAccessMods();
        SyntaxToken staticKeyword = ConsumeToken(SyntaxKind.StaticToken);
        TypeNode type = parseTypeNode();
        SyntaxToken identifier = ConsumeToken(SyntaxKind.TextToken);

        return IsConsumeMissed(SyntaxKind.OpenParentheseToken) ? 
            parseFieldDeclaration(mods, staticKeyword, type, identifier) : 
            parseFunctionMemberDeclaration(mods, staticKeyword, type, identifier);
    }
    private FieldDeclaration parseFieldDeclaration(AccessModifiers? mods, SyntaxToken staticKeyword, TypeNode type, SyntaxToken identifier) 
    {
        SyntaxToken semicolonToken = ConsumeToken(SyntaxKind.SemicolonToken);

        return _syntaxBuilder.FieldDeclaration(mods, staticKeyword.Kind, type, identifier, semicolonToken.Kind);
    }
    private FunctionMemberDeclaration parseFunctionMemberDeclaration(AccessModifiers? mods, SyntaxToken staticKeyword, TypeNode retType, SyntaxToken identifier) 
    {        
        FunctionSignature signature = parseFunctionSignature(retType, identifier);
        SyntaxToken openBrace = ConsumeToken(SyntaxKind.OpenBraceToken);

        // todo: parse body
        FunctionBody body = parseFunctionBody(signature);

        SyntaxToken closeBrace = ConsumeToken(SyntaxKind.OpenBraceToken);

        return _syntaxBuilder.FunctionMemberDeclaration(mods, staticKeyword.Kind, signature, openBrace, body, closeBrace);
    }
    private FunctionSignature parseFunctionSignature(TypeNode retType, SyntaxToken identifier) 
    {
        Dictionary<string, TypeNode> args = new Dictionary<string, TypeNode>();
        for(;;)
        {
            TypeNode argType = parseTypeNode();
            SyntaxToken argIdentifier = ConsumeToken(SyntaxKind.TextToken);

            if (argIdentifier.Kind != SyntaxKind.TextToken)
                diagnostics.ReportUnsuitableSyntaxToken(lexer.stream.line, argIdentifier.Kind, SyntaxKind.TextToken);
            if (args.ContainsKey(argIdentifier.Metadata.Raw))
                diagnostics.ReportAlreadyDefined(CurrentToken.Metadata.Line, argIdentifier.Metadata.Raw);
            else args[argIdentifier.Metadata.Raw] = argType;

            if (!IsConsumeMissed(SyntaxKind.CloseParentheseToken)) break;

            if (IsConsumeMissed(SyntaxKind.CommaToken)) 
                diagnostics.ReportTerminatorExpected(lexer.stream.line, CurrentToken.Kind, SyntaxKind.CommaToken);

        }

        return _syntaxBuilder.FunctionSignature(retType, identifier, args.ToImmutableDictionary());
    }
    private FunctionBody parseFunctionBody(FunctionSignature sig) 
    {
        throw new NotImplementedException();
    }
    private AccessModifiers? tryParseAccessMods() 
    {
        AccessModifiers? mods = SyntaxFacts.IsAccessModifier(CurrentToken.Kind);
        if (mods != null)
            mods |= SyntaxFacts.IsAccessModifier(ConsumeToken().Kind);
        return mods;
    }
    private TypeNode parseTypeNode() 
    {
        SyntaxToken typeName = ConsumeToken(SyntaxKind.TextToken);
        SyntaxToken starToken = ConsumeToken(SyntaxKind.StarToken);

        return _syntaxBuilder.TypeNode(typeName, starToken.Kind);
    }
    public void Dispose()
    {
        diagnostics.OnDiagnosticReported -= onDiagnostic;
    }
}