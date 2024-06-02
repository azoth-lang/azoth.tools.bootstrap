using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static class BindingAmbiguousNamesAspect
{
    public static IFixedList<IDeclarationNode> StandardNameExpression_ReferencedDeclarations(IStandardNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();

    public static IAmbiguousNameExpressionNode IdentifierName_Rewrite(IIdentifierNameExpressionNode node)
    {
        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (node.ReferencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return new UnqualifiedNamespaceNameNode(node.Syntax, referencedNamespaces);

        if (node.ReferencedDeclarations.TryAllOfType<IFunctionLikeDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(), referencedFunctions);

        if (node.ReferencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case INamedBindingNode referencedVariable:
                    return new VariableNameExpressionNode(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    return new StandardTypeNameExpressionNode(node.Syntax, FixedList.Empty<ITypeNode>(), referencedType);
            }

        return new UnknownIdentifierNameExpressionNode(node.Syntax, node.ReferencedDeclarations);
    }

    public static void UnknownIdentifierNameExpression_ContributeDiagnostics(
        IUnknownIdentifierNameExpressionNode node,
        Diagnostics diagnostics)
    {
        switch (node.ReferencedDeclarations.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
                break;
            case 1:
                // If there is only one match, then ReferencedSymbol is not null
                throw new UnreachableException();
            default:
                // TODO better errors explaining. For example, are they different kinds of declarations?
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.Span));
                break;
        }
    }

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_FunctionOrMethodGroupNameContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not IExpressionNode context)
            return null;

        if (context is not (IFunctionGroupNameNode or IMethodGroupNameNode))
            return null;

        return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments,
            FixedList.Empty<DefinitionNode>());
    }

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_NamespaceNameContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context)
            return null;

        var members = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        if (members.Count == 0)
            return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments,
                FixedList.Empty<DefinitionNode>());

        if (members.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return new QualifiedNamespaceNameNode(node.Syntax, context, referencedNamespaces);

        // TODO do the functions need to be in the same package?
        if (members.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        if (members.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    return new QualifiedTypeNameExpression(node.Syntax, context, node.TypeArguments,
                        referencedType);
            }

        return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments, members);
    }

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_TypeNameExpressionContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not ITypeNameExpressionNode { ReferencedDeclaration: IUserTypeDeclarationNode referencedDeclaration } context)
            return null;

        var members = referencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        if (members.Count == 0)
            return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments, FixedList.Empty<DefinitionNode>());

        if (members.TryAllOfType<IAssociatedFunctionDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        if (members.TryAllOfType<IInitializerDeclarationNode>(out var referencedInitializers))
            // TODO handle type arguments (which are not allowed for initializers)
            return new InitializerGroupNameNode(node.Syntax, context, referencedInitializers);

        return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments, members);
    }

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_ExpressionContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not IExpressionNode context)
            return null;
        // Ignore names that are still treated as expressions right now
        if (context is INamespaceNameNode
            or IFunctionGroupNameNode
            or IMethodGroupNameNode
            or ITypeNameExpressionNode
            or IInitializerGroupNameNode)
            return null;

        // TODO lookup members of the type and rewrite accordingly
        //var contextTypeDeclaration = node.InheritedPackageNameScope().Lookup(context.Type);
        return null;
    }

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_UnknownNameExpressionContext(IMemberAccessExpressionNode node)
    {
        if (node.Context is not IUnknownNameExpressionNode context)
            return null;

        return new UnknownMemberAccessExpressionNode(node.Syntax, context, node.TypeArguments, FixedList.Empty<IDeclarationNode>());
    }

    public static void UnknownMemberAccessExpression_ContributeDiagnostics(
        IUnknownMemberAccessExpressionNode node,
        Diagnostics diagnostics)
    {
        if (node.Context is IFunctionGroupNameNode or IMethodGroupNameNode)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "No member accessible from function or method."));

        if (node.Context is INamespaceNameNode)
        {
            switch (node.ReferencedMembers.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMember(node.File, node.Syntax.MemberNameSpan));
                    break;
                case 1:
                    // If there is only one match, then it would not be an unknown member access expression
                    throw new UnreachableException();
                default:
                    // TODO better errors explaining. For example, are they different kinds of declarations?
                    diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.MemberNameSpan));
                    break;
            }
        }
    }

    private static bool TryAllOfType<T>(
        this IReadOnlyCollection<IDeclarationNode> declarations,
        out IFixedList<T> referencedNamespaces)
        where T : IDeclarationNode
    {
        if (declarations.Count == 0)
        {
            referencedNamespaces = FixedList.Empty<T>();
            return false;
        }
        referencedNamespaces = declarations.OfType<T>().ToFixedList();
        // All of type T when counts match
        return referencedNamespaces.Count == declarations.Count;
    }

    public static void SelfExpression_ContributeDiagnostics(ISelfExpressionNode node, Diagnostics diagnostics)
    {
        if (node.ContainingDeclaration is not (IMethodDefinitionNode or ISourceConstructorDefinitionNode or IInitializerDeclarationNode))
            diagnostics.Add(node.IsImplicit
                ? OtherSemanticError.ImplicitSelfOutsideMethod(node.File, node.Syntax.Span)
                : OtherSemanticError.SelfOutsideMethod(node.File, node.Syntax.Span));
    }
}
