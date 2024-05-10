using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

/// <summary>
/// Applies entity symbols created in the semantic tree to the old syntax tree approach.
/// </summary>
/// <remarks>This is an intermediate phase to entirely eliminating the old approach using properties
/// on the concrete syntax tree.</remarks>
internal class EntitySymbolApplier
{
    public static void Apply(IPackageNode package)
    {
        Apply(package.MainFacet);
        Apply(package.TestingFacet);
    }

    private static void Apply(IPackageFacetNode node)
    {
        foreach (var n in node.Declarations.OfType<IClassDeclarationNode>())
        {
            var syntax = n.Syntax;
            syntax.Symbol.BeginFulfilling();
            syntax.Symbol.Fulfill(n.Symbol);
            Apply(n.GenericParameters);
            Apply(n.AllSupertypeNames);
        }
    }

    private static void Apply(IFixedList<IGenericParameterNode> nodes)
        => nodes.ForEach(Apply);

    private static void Apply(IGenericParameterNode node)
        => node.Syntax.Symbol.Fulfill(node.Symbol);

    private static void Apply(IEnumerable<IStandardTypeNameNode> nodes)
        => nodes.ForEach(StandardTypeName);

    private static void Types(IFixedList<ITypeNode> nodes)
        => nodes.ForEach(Type);

    private static void Type(ITypeNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeNameNode n:
                TypeName(n);
                break;
            case IOptionalTypeNode n:
                OptionalType(n);
                break;
            case ICapabilityTypeNode n:
                CapabilityType(n);
                break;
            case IFunctionTypeNode _:
            case IViewpointTypeNode _:
                throw new NotImplementedException();
        }
    }

    private static void TypeName(ITypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IStandardTypeNameNode n:
                StandardTypeName(n);
                break;
            case ISimpleTypeNameNode n:
                SimpleTypeName(n);
                break;
            case IQualifiedTypeNameNode _:
                throw new NotImplementedException();
        }
    }

    private static void StandardTypeName(IStandardTypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierTypeNameNode n:
                IdentifierTypeName(n);
                break;
            case IGenericTypeNameNode n:
                GenericTypeName(n);
                break;
        }
    }

    private static void SimpleTypeName(ISimpleTypeNameNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IIdentifierTypeNameNode n:
                IdentifierTypeName(n);
                break;
            case ISpecialTypeNameNode n:
                SpecialTypeName(n);
                break;
        }
    }

    private static void IdentifierTypeName(IIdentifierTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
    }

    private static void GenericTypeName(IGenericTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
        Types(node.TypeArguments);
    }

    private static void SpecialTypeName(ISpecialTypeNameNode node)
    {
        node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbol);
        node.Syntax.NamedType = node.Type;
    }

    private static void OptionalType(IOptionalTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }

    private static void CapabilityType(ICapabilityTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }
}
