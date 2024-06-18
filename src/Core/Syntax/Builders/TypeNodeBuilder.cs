namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder
{
    public TypeNode TypeNode(SyntaxToken typeName, SyntaxKind starToken) 
    {
        if (typeName.Kind != SyntaxKind.TextToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, typeName.Kind, SyntaxKind.TextToken);

        return SyntaxFacts.IsPrimitiveType(typeName.Metadata.Raw, out PrimitiveTypes? primType) ? 
            new PrimitiveTypeNode(typeName.Metadata.Raw, starToken == SyntaxKind.StarToken) 
            { 
                Type = primType!.Value
            } :
            new TypeNode(typeName.Metadata.Raw, starToken == SyntaxKind.StarToken);
    }
}