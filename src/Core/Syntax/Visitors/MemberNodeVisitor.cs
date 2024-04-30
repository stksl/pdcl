namespace Pdcl.Core.Syntax;

internal sealed class MemberNodeVisitor : IVisitor<MemberNode> 
{
    public static MemberNodeVisitor Instance => _instance;
    private static MemberNodeVisitor _instance = new();
    private MemberNodeVisitor() {}

    public async Task<MemberNode?> VisitAsync(Parser parser) 
    {
        AccessModifiers mods = AccessModifiers.Private;
        bool checkAccess = true;

        SyntaxToken prev = parser.CurrentToken;

    _memberCheck:
        if (SyntaxFacts.IsAccessModifier(prev.Kind, parser.CurrentToken.Kind, out AccessModifiers? _mods))
        {
            if (!checkAccess)
            {
                await parser.diagnostics.ReportUnkownAccessMods(parser.CurrentToken.Metadata.Line);
            }
            else mods = _mods!.Value;
            checkAccess = false;
            parser.ConsumeToken();
            goto _memberCheck;
        }

        TypeNode? memberType = await VisitorFactory.GetVisitorFor<TypeNode>(parser.context)!.VisitAsync(parser);

        string? memberName = parser.CurrentToken.Metadata.Raw;
        parser.ConsumeToken(SyntaxKind.TextToken);
        
        if (parser.CurrentToken.Kind == SyntaxKind.OpenParentheseToken) 
        {
            var visitor = (FunctionDeclarationVisitor)
                VisitorFactory.GetVisitorFor<FunctionDeclaration>(parser.context)!;

            return new FunctionMemberDeclaration(
                (await visitor.VisitAsync(parser, memberType!, memberName))!, mods, parser.currPath + memberName);
        }

        parser.ConsumeToken(SyntaxKind.SemicolonToken);

        return new FieldDeclaration(memberName, memberType!, mods, hasGetter: true, hasSetter: true);
    }
}
