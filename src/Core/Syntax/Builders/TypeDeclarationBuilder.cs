using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder 
{
    public TypeDeclarationNode TypeDeclaration(SyntaxKind structToken, SyntaxToken identifier, SyntaxKind openBrace, ImmutableArray<FieldDeclaration> fields, ImmutableArray<FunctionMemberDeclaration> functions, SyntaxKind closeBrace) 
    {
        if (structToken != SyntaxKind.StructToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, structToken, SyntaxKind.StructToken);
        if (identifier.Kind != SyntaxKind.TextToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, identifier.Kind, SyntaxKind.TextToken);
        if (openBrace != SyntaxKind.OpenBraceToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, openBrace, SyntaxKind.OpenBraceToken);
        if (closeBrace != SyntaxKind.CloseBraceToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, closeBrace, SyntaxKind.CloseBraceToken);
        
        return new TypeDeclarationNode(identifier.Metadata.Raw, functions, fields);
    }
}