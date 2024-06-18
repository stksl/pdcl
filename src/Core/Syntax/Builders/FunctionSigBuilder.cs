using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder 
{
    public FunctionSignature FunctionSignature(TypeNode retType, SyntaxToken identifier, ImmutableDictionary<string, TypeNode> args) 
    {
        if (identifier.Kind != SyntaxKind.TextToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, identifier.Kind, SyntaxKind.TextToken);
        
        return new FunctionSignature(identifier.Metadata.Raw, retType, args);
    }
} 