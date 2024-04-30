namespace Pdcl.Core.Syntax;

internal sealed class TypeDeclarationVisitor : IVisitor<TypeDeclarationNode> 
{
    public TypeDeclarationVisitor Instance => _instance;
    private TypeDeclarationVisitor _instance = new();
    private TypeDeclarationVisitor() {}

    public async Task<TypeDeclarationNode?> VisitAsync(Parser parser) 
    {
        if (parser.IsConsumeMissed(SyntaxKind.StructToken) || parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            return null;
        }

        string identifier = parser.CurrentToken.Metadata.Raw;

        parser.ConsumeToken(SyntaxKind.OpenBraceToken);

        while (parser.CurrentToken.Kind != SyntaxKind.CloseBraceToken) 
        {
            AccessModifiers mods = AccessModifiers.Private;
            bool checkAccess = true;
        _memberCheck:
            switch(parser.CurrentToken.Kind) 
            {
                case > 0 when SyntaxFacts.IsAccessModifier(parser.CurrentToken.Kind, parser.ConsumeToken().Kind, out AccessModifiers? _mods):
                    if (!checkAccess) 
                    { 
                        await parser.diagnostics.ReportUnkownAccessMods(parser.CurrentToken.Metadata.Line);
                    }
                    mods = _mods!.Value;
                    checkAccess = false;
                    goto _memberCheck;

                
            }
        }
        return null!;
    }
}