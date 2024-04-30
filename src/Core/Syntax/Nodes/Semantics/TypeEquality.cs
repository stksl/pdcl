
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Pdcl.Core.Syntax.Semantics;
/// <summary>
/// A type semantic extension class
/// </summary>
public static class TypeEquality
{
    /// <summary>
    /// Performs equality checks on implicit/explicit operators overload
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="op"></param>
    /// <param name="overload"></param>
    /// <returns></returns>
    public static TypeNode? UnaryTypeCheck(this TypeNode type, UnaryOperator.UnaryOperators op)
    {
        if (type is PrimitiveTypeNode prim)
            return PrimitiveTypeEquality.UnaryTypeCheck(prim, op) ? prim : null;

        return type.TypeDeclaration.GetOperatorOverloads()
        .FirstOrDefault(i => i.Operator is UnaryOperator unary && unary.OperatorType == op)?.Signature.ReturnType;
    }
    public static bool ImplicitTypeCheck(this TypeNode from, TypeNode into)
    {
        if (from is PrimitiveTypeNode prim && into is PrimitiveTypeNode primInto)
            return PrimitiveTypeEquality.ImplicitTypeCheck(prim, primInto);

        return from.TypeDeclaration == into.TypeDeclaration || from.TypeDeclaration.GetOperatorOverloads()
        .FirstOrDefault(i => i.Operator is UnaryOperator unary
            && unary.OperatorType == UnaryOperator.UnaryOperators.Implicit
            && i.Signature.ReturnType.TypeDeclaration == into.TypeDeclaration) != null;
    }
    public static bool ExplicitTypeCheck(this TypeNode from, TypeNode into)
    {
        if (from is PrimitiveTypeNode prim && into is PrimitiveTypeNode primInto)
            return PrimitiveTypeEquality.ExplicitTypeCheck(prim, primInto);
            
        return from.TypeDeclaration == into.TypeDeclaration || from.TypeDeclaration.GetOperatorOverloads()
        .FirstOrDefault(i => i.Operator is UnaryOperator unary
            && unary.OperatorType == UnaryOperator.UnaryOperators.Cast
            && i.Signature.ReturnType.TypeDeclaration == into.TypeDeclaration) != null;
    }
    /// <summary>
    /// <paramref name="other"/> is either left or right
    /// </summary>
    /// <param name="type"></param>
    /// <param name="other"></param>
    /// <param name="op"></param>
    /// <returns></returns>
    public static TypeNode? BinaryExpTypeCheck(this TypeNode type, TypeNode other, BinaryOperator.BinaryOperators op)
    {
        if (type is PrimitiveTypeNode prim && other is PrimitiveTypeNode primOther)
            return PrimitiveTypeEquality.BinaryExpTypeCheck(prim, primOther, op);

        foreach (var overload in type.TypeDeclaration.GetOperatorOverloads()
        .Where(i => i.Operator is BinaryOperator binOp && binOp.OperatorType == op))
        {
            ImmutableDictionary<string, TypeNode> args = overload.Signature.Arguments;
            if ((args.First().Value.ImplicitTypeCheck(type) && args.Last().Value.ImplicitTypeCheck(other))
            || (args.First().Value.ImplicitTypeCheck(other) && args.Last().Value.ImplicitTypeCheck(type)))
            {
                return overload.Signature.ReturnType;
            }
        }

        return null;
    }

    private static class PrimitiveTypeEquality
    {
        public static bool UnaryTypeCheck(PrimitiveTypeNode type, UnaryOperator.UnaryOperators op)
        {
            if (type.Type == PrimitiveTypeNode.PrimitiveTypes.String)
                return false;
            switch (type.Type)
            {
                case PrimitiveTypeNode.PrimitiveTypes.Bool when op == UnaryOperator.UnaryOperators.Not:
                    return true;
                case >= PrimitiveTypeNode.PrimitiveTypes.UInt8 and <= PrimitiveTypeNode.PrimitiveTypes.Char
                when op == UnaryOperator.UnaryOperators.Increment || op == UnaryOperator.UnaryOperators.Decrement || op == UnaryOperator.UnaryOperators.Minus:
                    return true;

                default:
                    return false;
            }
        }
        public static bool ImplicitTypeCheck(PrimitiveTypeNode from, PrimitiveTypeNode into)
        {
            if (from.Type == into.Type || into.Type == PrimitiveTypeNode.PrimitiveTypes.String) return true;

            if (from.Type == PrimitiveTypeNode.PrimitiveTypes.String) 
            {
                return false;
            }

            switch(from.Type) 
            {
                case PrimitiveTypeNode.PrimitiveTypes.Float32:
                    return into.Type == PrimitiveTypeNode.PrimitiveTypes.Float64;
                case PrimitiveTypeNode.PrimitiveTypes.Char:
                    return into.Type == PrimitiveTypeNode.PrimitiveTypes.Int32; // for simplicity
            }
            return from.IsInteger && (from.IsUnsigned ? into.IsUnsigned : (into.IsInteger || into.IsFloat)) && into.Type > from.Type;
        }
        public static bool ExplicitTypeCheck(PrimitiveTypeNode from, PrimitiveTypeNode into) 
        {
            if(from.Type == into.Type) return true;

            const PrimitiveTypeNode.PrimitiveTypes mask = 
                PrimitiveTypeNode.PrimitiveTypes.String | PrimitiveTypeNode.PrimitiveTypes.Bool;

            return !mask.HasFlag(from.Type) && into.Type != PrimitiveTypeNode.PrimitiveTypes.Bool;
        }
        /// <summary>
        /// <paramref name="other"/> is either left or right
        /// </summary>
        /// <param name="type"></param>
        /// <param name="other"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static PrimitiveTypeNode? BinaryExpTypeCheck(PrimitiveTypeNode type, PrimitiveTypeNode other, BinaryOperator.BinaryOperators op)
        {
            PrimitiveTypeNode? res = 
                ImplicitTypeCheck(type, other) ? other : null;

            const PrimitiveTypeNode.PrimitiveTypes mask = PrimitiveTypeNode.PrimitiveTypes.String | 
                PrimitiveTypeNode.PrimitiveTypes.Float64 | 
                PrimitiveTypeNode.PrimitiveTypes.Float32;

            switch (op)
            {
                case BinaryOperator.BinaryOperators.BitwiseShiftLeft:
                case BinaryOperator.BinaryOperators.BitwiseShiftRight:
                    
                    bool isLong = type.Type == PrimitiveTypeNode.PrimitiveTypes.Int64;
                    bool isULong = type.Type == PrimitiveTypeNode.PrimitiveTypes.UInt64;

                    if (mask.HasFlag(type.Type) || !ImplicitTypeCheck(other, new PrimitiveTypeNode(PrimitiveTypeNode.PrimitiveTypes.Int32)))
                    {
                        return null;
                    }
                    return isULong ? new PrimitiveTypeNode(PrimitiveTypeNode.PrimitiveTypes.UInt64) : 
                        (isLong ? new PrimitiveTypeNode(PrimitiveTypeNode.PrimitiveTypes.Int64) 
                        : new PrimitiveTypeNode(PrimitiveTypeNode.PrimitiveTypes.Int32));
                case BinaryOperator.BinaryOperators.BitwiseOr:
                case BinaryOperator.BinaryOperators.BitwiseAnd:
                case BinaryOperator.BinaryOperators.BitwiseXor:
                    if (mask.HasFlag(type.Type) || mask.HasFlag(other.Type)) 
                    {
                        return null;
                    }
                    goto default;
                case BinaryOperator.BinaryOperators.IsEqual:
                case BinaryOperator.BinaryOperators.IsNotEqual:
                case BinaryOperator.BinaryOperators.ShortOr:
                case BinaryOperator.BinaryOperators.ShortAnd:
                    return res == null ? null : new PrimitiveTypeNode(PrimitiveTypeNode.PrimitiveTypes.Bool);

                default:
                    return res;
            }
        }
    }
}