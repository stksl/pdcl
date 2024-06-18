using System.Diagnostics;

namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder
{
    public FunctionMemberDeclaration FunctionMemberDeclaration(AccessModifiers? mods, SyntaxKind staticKeyword, FunctionSignature sig, SyntaxToken openBrace, FunctionBody body, SyntaxToken closeBrace)
    {
        if (openBrace.Kind != SyntaxKind.OpenBraceToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, openBrace.Kind, SyntaxKind.OpenBraceToken);
        if (closeBrace.Kind != SyntaxKind.CloseBraceToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, closeBrace.Kind, SyntaxKind.CloseBraceToken);

        return new FunctionMemberDeclaration(sig, body, mods ?? AccessModifiers.Private)
        {
            HasGetter = true,
            HasSetter = false,
            IsStatic = staticKeyword == SyntaxKind.StaticToken
        };
    }
}