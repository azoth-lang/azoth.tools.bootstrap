using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolAspect
{
    public static PackageSymbol Package_Symbol(IPackageNode node) => new(node.Name);

    public static UserTypeSymbol TypeDefinition_Symbol(ITypeDefinitionNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static GenericParameterTypeSymbol GenericParameter_Symbol(IGenericParameterNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static TypeSymbol? StandardTypeName_ReferencedSymbol(IStandardTypeNameNode node)
        => node.ReferencedDeclaration?.Symbol;

    public static TypeSymbol SpecialTypeName_ReferencedSymbol(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);

    public static FunctionSymbol FunctionDefinition_Symbol(IFunctionDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static MethodSymbol MethodDefinition_Symbol(IMethodDefinitionNode node)
        => new MethodSymbol(node.ContainingSymbol, node.Kind, node.Name,
            node.SelfParameter.ParameterType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList(),
            new(node.Return?.NamedType ?? DataType.Void));

    public static ConstructorSymbol SourceConstructorDefinition_Symbol(ISourceConstructorDefinitionNode node)
        => new ConstructorSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.BindingType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static ConstructorSymbol DefaultConstructorDefinition_Symbol(IDefaultConstructorDefinitionNode node)
        => ConstructorSymbol.CreateDefault(node.ContainingSymbol);

    public static FieldSymbol FieldDefinition_Symbol(IFieldDefinitionNode node)
        => new FieldSymbol(node.ContainingSymbol, node.Name, node.IsMutableBinding, ((IFieldDeclarationNode)node).BindingType);

    public static InitializerSymbol SourceInitializerDefinition_Symbol(ISourceInitializerDefinitionNode node)
        => new InitializerSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.BindingType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static InitializerSymbol DefaultInitializerDefinition_Symbol(IDefaultInitializerDefinitionNode node)
        => InitializerSymbol.CreateDefault(node.ContainingSymbol);

    public static FunctionSymbol AssociatedFunctionDefinition_Symbol(IAssociatedFunctionDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static ConstructorSymbol? Attribute_ReferencedSymbol(IAttributeNode node)
    {
        var referencedTypeSymbolNode = node.TypeName.ReferencedDeclaration;
        if (referencedTypeSymbolNode is not IUserTypeDeclarationNode userTypeSymbolNode)
            return null;

        return userTypeSymbolNode.Members.OfType<IConstructorDeclarationNode>().Select(c => c.Symbol)
                                 .SingleOrDefault(s => s.Arity == 0);
    }

    public static void Attribute_ContributeDiagnostics(IAttributeNode node, Diagnostics diagnostics)
    {
        if (node.ReferencedSymbol is null)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.TypeName.Syntax.Span));
    }

    // TODO remove parameter symbols
    public static SelfParameterSymbol SelfParameter_Symbol(ISelfParameterNode node)
    {
        var parent = (IInvocableDefinitionNode)node.Parent;
        var isConstructor = node.Parent is IConstructorDefinitionNode or IInitializerDefinitionNode;
        return new SelfParameterSymbol(parent.Symbol, node.IsLentBinding && !isConstructor, ((IParameterNode)node).BindingType);
    }

    // TODO remove parameter symbols
    public static NamedVariableSymbol NamedParameter_Symbol(INamedParameterNode node)
    {
        var parent = (IInvocableDefinitionNode)node.Parent;
        var isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return NamedVariableSymbol.CreateParameter(parent.Symbol, node.Name,
            node.DeclarationNumber, node.IsMutableBinding, isLent, node.BindingType);
    }

    public static void NamedParameter_ContributeDiagnostics(INamedParameterNode node, Diagnostics diagnostics)
    {
        var type = node.BindingType;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }

    public static NamedVariableSymbol VariableDeclarationStatement_Symbol(IVariableDeclarationStatementNode node)
        //var symbol = NamedVariableSymbol.CreateLocal(containingSymbol, node.IsMutableBinding, node.Name, node.DeclarationNumber, node.Type);
        => throw new NotImplementedException();

    public static SelfParameterSymbol? SelfExpression_ReferencedSymbol(ISelfExpressionNode node)
        => node.ContainingDeclaration switch
        {
            IConcreteMethodDefinitionNode n => n.SelfParameter.Symbol,
            ISourceConstructorDefinitionNode n => n.SelfParameter.Symbol,
            IDefaultConstructorDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            ISourceInitializerDefinitionNode n => n.SelfParameter.Symbol,
            IDefaultInitializerDefinitionNode _
                => throw new UnreachableException("A `self` expression cannot occur here because it has an empty body."),
            IFieldDefinitionNode _ => null,
            IAssociatedFunctionDefinitionNode _ => null,
            IFunctionDefinitionNode _ => null,
            _ => throw ExhaustiveMatch.Failed(node.ContainingDeclaration),
        };
}
