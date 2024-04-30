
using System.Collections.Immutable;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Pdcl.Core.Syntax;

public abstract class SyntaxNode
{
    public abstract IEnumerable<SyntaxNode> GetChildren();
    
}