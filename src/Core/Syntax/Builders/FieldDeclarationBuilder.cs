namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder
{
    public FieldDeclaration FieldDeclaration(AccessModifiers? mods, SyntaxKind staticKeyword, TypeNode type, SyntaxToken identifier, SyntaxKind semicolonToken)
    {
        if (identifier.Kind != SyntaxKind.TextToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, identifier.Kind, SyntaxKind.TextToken);
        if (semicolonToken != SyntaxKind.SemicolonToken)
            _diagnosticHandler.ReportTerminatorExpected(_stream.line, semicolonToken, SyntaxKind.SemicolonToken);

        return new FieldDeclaration(identifier.Metadata.Raw, type, mods ?? AccessModifiers.Private)
        {
            IsStatic = staticKeyword == SyntaxKind.StaticToken,
            HasGetter = true,
            HasSetter = true
        };
    }
}