using System;
using System.Collections.Generic;
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
        PackageFacet(package.MainFacet);
        PackageFacet(package.TestingFacet);
    }

    private static void PackageFacet(IPackageFacetNode node)
        => node.CompilationUnits.ForEach(CompilationUnit);

    private static void CompilationUnit(ICompilationUnitNode node)
        => Declarations(node.Declarations);

    private static void Declarations(IEnumerable<IDeclarationNode> nodes)
        => nodes.ForEach(Declaration);

    private static void Declaration(IDeclarationNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ITypeDeclarationNode n:
                TypeDeclaration(n);
                break;
            case IFunctionDeclarationNode n:
                FunctionDeclaration(n);
                break;
            case INamespaceDeclarationNode n:
                NamespaceDeclaration(n);
                break;
            case IMethodDeclarationNode n:
                MethodDeclaration(n);
                break;
            case IConstructorDeclarationNode n:
                ConstructorDeclaration(n);
                break;
            case IInitializerDeclarationNode n:
                InitializerDeclaration(n);
                break;
            case IFieldDeclarationNode n:
                FieldDeclaration(n);
                break;
            case IAssociatedFunctionDeclarationNode _:
                // TODO
                break;
        }
    }

    private static void TypeDeclaration(ITypeDeclarationNode node)
    {
        var syntax = node.Syntax;
        syntax.Symbol.BeginFulfilling();
        syntax.Symbol.Fulfill(node.Symbol);
        GenericParameters(node.GenericParameters);
        StandardTypeNames(node.AllSupertypeNames);
        Declarations(node.Members);
    }

    private static void GenericParameters(IFixedList<IGenericParameterNode> nodes)
        => nodes.ForEach(GenericParameter);

    private static void GenericParameter(IGenericParameterNode node)
        => node.Syntax.Symbol.Fulfill(node.Symbol);

    private static void FunctionDeclaration(IFunctionDeclarationNode node)
    {
        var syntax = node.Syntax;
        syntax.Symbol.BeginFulfilling();
        syntax.Symbol.Fulfill(node.Symbol);
        NamedParameters(node.Parameters);
        Type(node.Return);
    }

    private static void NamedParameters(IEnumerable<INamedParameterNode> nodes)
        => nodes.ForEach(NamedParameter);

    private static void NamedParameter(INamedParameterNode node) => Type(node.TypeNode);

    private static void ConstructorOrInitializerParameters(IEnumerable<IConstructorOrInitializerParameterNode> nodes)
        => nodes.ForEach(ConstructorOrInitializerParameter);

    private static void ConstructorOrInitializerParameter(IConstructorOrInitializerParameterNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case INamedParameterNode n:
                NamedParameter(n);
                break;
            case IFieldParameterNode n:
                FieldParameter(n);
                break;
        }
    }

    private static void FieldParameter(IFieldParameterNode node)
        => node.Syntax.ReferencedSymbol.Fulfill(node.ReferencedSymbolNode?.Symbol);

    private static void NamespaceDeclaration(INamespaceDeclarationNode node)
    {
        node.Syntax.Symbol.Fulfill(node.Symbol);
        Declarations(node.Declarations);
    }

    private static void MethodDeclaration(IMethodDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        NamedParameters(node.Parameters);
        Type(node.Return);
    }

    private static void ConstructorDeclaration(IConstructorDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        ConstructorOrInitializerParameters(node.Parameters);
    }

    private static void InitializerDeclaration(IInitializerDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        ConstructorOrInitializerParameters(node.Parameters);
    }

    private static void FieldDeclaration(IFieldDeclarationNode node)
    {
        var symbol = node.Syntax.Symbol;
        symbol.BeginFulfilling();
        symbol.Fulfill(node.Symbol);
        Type(node.TypeNode);
    }

    private static void StandardTypeNames(IEnumerable<IStandardTypeNameNode> nodes)
        => nodes.ForEach(StandardTypeName);

    private static void Types(IFixedList<ITypeNode> nodes)
        => nodes.ForEach(Type);

    private static void Type(ITypeNode? node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case null:
                break;
            case ITypeNameNode n:
                TypeName(n);
                break;
            case IOptionalTypeNode n:
                OptionalType(n);
                break;
            case ICapabilityTypeNode n:
                CapabilityType(n);
                break;
            case IFunctionTypeNode n:
                FunctionType(n);
                break;
            case IViewpointTypeNode n:
                ViewpointType(n);
                break;
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

    private static void FunctionType(IFunctionTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        ParameterTypes(node.Parameters);
        Type(node.Return);
    }

    private static void ParameterTypes(IEnumerable<IParameterTypeNode> nodes)
        => nodes.ForEach(ParameterType);

    private static void ParameterType(IParameterTypeNode node)
        => Type(node.Referent);

    private static void ViewpointType(IViewpointTypeNode node)
    {
        node.Syntax.NamedType = node.Type;
        Type(node.Referent);
    }
}
