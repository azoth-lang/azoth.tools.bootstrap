using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolAttribute
{
    public static PackageSymbol Package(IPackageNode node) => new(node.Name);

    public static UserTypeSymbol TypeDeclaration(ITypeDefinitionNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static GenericParameterTypeSymbol GenericParameter(IGenericParameterNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static TypeSymbol? StandardTypeName(IStandardTypeNameNode node)
        => node.ReferencedDeclaration?.Symbol;

    public static TypeSymbol SpecialTypeName_ReferencedSymbol(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);

    public static FunctionSymbol FunctionDeclaration(IFunctionDefinitionNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static MethodSymbol MethodDeclaration(IMethodDefinitionNode node)
        => new MethodSymbol(node.ContainingSymbol, node.Kind, node.Name,
            node.SelfParameter.ParameterType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList(),
            new(node.Return?.Type ?? DataType.Void));

    public static ConstructorSymbol SourceConstructorDefinition(ISourceConstructorDefinitionNode node)
        => new ConstructorSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.Type,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static ConstructorSymbol DefaultConstructorDefinition(IDefaultConstructorDefinitionNode node)
        => ConstructorSymbol.CreateDefault(node.ContainingSymbol);

    public static FieldSymbol FieldDeclaration(IFieldDefinitionNode node)
        => new FieldSymbol(node.ContainingSymbol, node.Name, node.IsMutableBinding, node.Type);

    public static InitializerSymbol InitializerDeclaration(IInitializerDefinitionNode node)
        => new InitializerSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.Type,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static FunctionSymbol AssociatedFunctionDeclaration(IAssociatedFunctionDefinitionNode node)
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
        return new SelfParameterSymbol(parent.Symbol, node.IsLentBinding && !isConstructor, node.Type);
    }

    // TODO remove parameter symbols
    public static NamedVariableSymbol NamedParameter_Symbol(INamedParameterNode node)
    {
        var parent = (IInvocableDefinitionNode)node.Parent;
        var isLent = node.IsLentBinding && node.Type.CanBeLent();
        return NamedVariableSymbol.CreateParameter(parent.Symbol, node.Name,
            node.DeclarationNumber, node.IsMutableBinding, isLent, node.Type);
    }

    public static void NamedParameter_ContributeDiagnostics(INamedParameterNode node, Diagnostics diagnostics)
    {
        var type = node.Type;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }

    public static NamedVariableSymbol VariableDeclarationStatement_Symbol(IVariableDeclarationStatementNode node)
        //var symbol = NamedVariableSymbol.CreateLocal(containingSymbol, node.IsMutableBinding, node.Name, node.DeclarationNumber, node.Type);
        => throw new NotImplementedException();
}
