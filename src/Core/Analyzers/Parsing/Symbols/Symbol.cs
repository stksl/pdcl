using System.Diagnostics.CodeAnalysis;
using Pdcl.Core.Syntax;
using System.Runtime.InteropServices;

namespace Pdcl.Core;

internal sealed partial class Parser 
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Symbol : IEquatable<Symbol> 
    {
        public readonly string Name;
        public readonly SymbolType Type;
        public readonly int Scope;
        public readonly SyntaxNode Node;
        public Symbol(string name, SymbolType type, int scope, SyntaxNode node)
        {
            Name = name;
            Type = type;
            Scope = scope;
            Node = node;

        }
        /// <summary>
        /// Only for equality purposes
        /// </summary>
        /// <param name="name"></param>
        public Symbol(string name, SymbolType type) : this(name, type, -1, null!) 
        {
        }
        public bool Equals(Symbol other) 
        {
            return GetHashCode() == other.GetHashCode();
        }
        public override bool Equals([NotNullWhen(true)] object? obj) => 
            obj is Symbol other && Equals(other: other);
        public override int GetHashCode()
        {
            return HashCode.Combine<string, SymbolType>(Name, Type);
        }
        public static bool operator ==(Symbol left, Symbol right) 
        {
            return left.Equals(right);
        } 
        public static bool operator !=(Symbol left, Symbol right) 
        {
            return !left.Equals(right);
        }
    }
    public enum SymbolType : int 
    {
        Undefined = 0,

        LocalVar,
        Function,
        TypeDefinition,
        // todo
    }
}