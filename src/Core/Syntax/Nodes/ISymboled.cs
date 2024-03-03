namespace Pdcl.Core.Syntax;

/// <summary>
/// Several nodes can contain own symbol tables (type declarations and functions)
/// </summary>
public interface ISymboled 
{
    string TableTreePath {get;}
}