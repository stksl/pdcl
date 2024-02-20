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

        public readonly SymbolTable? Children;
        public Symbol(string name, SymbolType type, int scope, SyntaxNode node, SymbolTable? children)
        {
            Name = name;
            Type = type;
            Scope = scope;
            Node = node;

            Children = children;
        }
        /// <summary>
        /// Only for equality purposes
        /// </summary>
        /// <param name="name"></param>
        public Symbol(string name) : this(name, SymbolType.Undefined, -1, null!, null) 
        {
        }
        public bool Equals(Symbol other) 
        {
            return Name == other.Name;
        }
        public override bool Equals([NotNullWhen(true)] object? obj) => 
            obj is Symbol other && Equals(other: other);
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public static bool operator ==(Symbol left, Symbol right) 
        {
            return left.Name == right.Name;
        } 
        public static bool operator !=(Symbol left, Symbol right) 
        {
            return left.Name != right.Name;
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