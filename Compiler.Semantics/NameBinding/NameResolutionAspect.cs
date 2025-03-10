using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

/// <summary>
/// Name resolution is the part of name binding that determines what kind of thing a name refers to.
/// </summary>
internal static partial class NameResolutionAspect
{
    #region Unresolved Expressions
    public static partial IUnresolvedMemberAccessExpressionNode? UnresolvedMemberAccessExpression_Deref_Rewrite_UnresolvedMemberAccessExpression(IUnresolvedMemberAccessExpressionNode node)
    {
        // TODO this is a messy rewrite. Should this be changed into an insert on Expression?

        if (node.Context is not { } context || context.PlainType.RefDepth() == 0) return null;

        var deref = IImplicitDerefExpressionNode.Create(context);
        return IUnresolvedMemberAccessExpressionNode.Create(node.Syntax, deref, node.GenericArguments);
    }

    public static partial IExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node)
    {
        if (node.Context is not { } context)
            return null;

        // No need to check if context is INamespaceNameNode or ITypeNameExpressionNode because
        // those rewrites have already run and stop rewriting in those cases.

        // Ignore names that never have members
        if (context is IFunctionNameExpressionNode
            or IMethodAccessExpressionNode
            or IInitializerNameExpressionNode)
            return null;

        var contextTypeDeclaration = node.PackageNameScope().Lookup(context.PlainType);
        // TODO members needs to be filtered to visible accessible members
        var members = contextTypeDeclaration?.InclusiveInstanceMembersNamed(node.MemberName).ToFixedSet() ?? [];
        if (members.Count == 0)
            return null;

        if (members.TryAllOfType<IOrdinaryMethodDeclarationNode>(out var referencedMethods))
            return IMethodAccessExpressionNode.Create(node.Syntax, context, node.GenericArguments, referencedMethods);

        if (members.TryAllOfType<IPropertyAccessorDeclarationNode>(out var referencedProperties)
            && node.GenericArguments.Count == 0)
            // We don't really know that it is a getter, but if it isn't then it will be rewritten to a setter
            return IGetterInvocationExpressionNode.Create(node.Syntax, context, referencedProperties);

        // TODO does this need to change for get vs set?
        if (members.Where(m => m is not IPropertyAccessorDeclarationNode)
                   .TrySingle() is IFieldDeclarationNode fieldDeclaration)
            return IFieldAccessExpressionNode.Create(node.Syntax, context, fieldDeclaration);

        return null;
    }

    public static partial void UnresolvedMemberAccessExpression_Contribute_Diagnostics(IUnresolvedMemberAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Context)
        {
            case IFunctionNameExpressionNode or IMethodAccessExpressionNode or IInitializerNameExpressionNode:
                diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span,
                    "No member accessible from function, method, or initializer."));
                break;
            case INamespaceNameNode:
            case ITypeNameNode:
                diagnostics.Add(NameBindingError.CouldNotBindMember(node.File, node.Syntax.MemberNameSpan));
                break;
            case IUnresolvedExpressionNode:
            case { Type: UnknownType }:
                // These presumably report their own errors and should be ignored here
                break;
            default:
                diagnostics.Add(NameBindingError.NotImplemented(node.File, node.Syntax.Span,
                    $"Could not access `{node.MemberName}` on `{node.Context!.Syntax}` (Unknown member)."));
                break;
        }
    }

    /*public static partial void AmbiguousMemberAccessExpression_Contribute_Diagnostics(
        IAmbiguousMemberAccessExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Context)
        {
            case INamespaceNameNode:
            case ITypeNameExpressionNode:
                // TODO better errors explaining. For example, are they different kinds of declarations?
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.QualifiedName.Span));
                break;
            case IUnresolvedNameExpressionNode:
            case IUnresolvedInvocationExpressionNode:
            case { Type: UnknownType }:
                // These presumably report their own errors and should be ignored here
                break;
            default:
                // TODO aren't normal expression contexts falling into this case right now?
                diagnostics.Add(NameBindingError.NotImplemented(node.File, node.Syntax.Span,
                    $"Could not access `{node.MemberName}` on `{node.Context.Syntax}` (Unknown member)."));
                break;
        }
    }*/
    #endregion

    #region Operator Expressions
    public static partial ISetterInvocationExpressionNode? AssignmentExpression_Rewrite_SetterInvocationExpression(IAssignmentExpressionNode node)
    {
        // TODO refactor to a replacement of the getter node based on an inherited attribute. That
        // should allow optimization of caching compared to a full rewrite.

        if (node.TempLeftOperand is not IGetterInvocationExpressionNode getterInvocation) return null;

        // There must be a setter to call
        if (!getterInvocation.ReferencedDeclarations.OfType<ISetterMethodDeclarationNode>().Any())
            return null;

        // TODO whether this is a set also depends on whether the `ref` and `iref` types say it should be

        return ISetterInvocationExpressionNode.Create(node.Syntax, getterInvocation.Context,
            getterInvocation.PropertyName, node.CurrentRightOperand, getterInvocation.ReferencedDeclarations);
    }
    #endregion

    #region Name Expressions
    public static partial void Validate_FunctionNameExpression(
        INameExpressionSyntax syntax,
        INameNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> genericArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations)
    { }
    // TODO add this validation in somehow
    //=> Requires.That(!ReferencedDeclarations.IsEmpty, nameof(referencedDeclarations),
    //    "Must be at least one referenced declaration");
    #endregion

    #region Unresolved Name Expressions
    public static partial IFixedList<IDeclarationNode> UnresolvedOrdinaryNameExpression_ReferencedDeclarations(IUnresolvedOrdinaryNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();

    public static partial void UnresolvedOrdinaryNameExpression_Contribute_Diagnostics(IUnresolvedOrdinaryNameExpressionNode node, DiagnosticCollectionBuilder diagnostics)
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

    public static partial INameExpressionNode? UnresolvedIdentifierNameExpression_ReplaceWith_NameExpression(IUnresolvedIdentifierNameExpressionNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionNameExpressionNode.Create(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(), referencedFunctions);

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case INamespaceDeclarationNode referencedNamespace:
                    return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespace);
                case ILocalBindingNode referencedVariable:
                    return IVariableNameExpressionNode.Create(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    return IIdentifierTypeNameNode.Create(node.Syntax);
            }

        return null;
    }

    public static partial INameExpressionNode? UnresolvedGenericNameExpression_ReplaceWith_NameExpression(IUnresolvedGenericNameExpressionNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionNameExpressionNode.Create(node.Syntax, null, node.Name, node.GenericArguments, referencedFunctions);

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    return IGenericTypeNameNode.Create(node.Syntax, node.GenericArguments);
            }

        return null;
    }

    public static partial IUnresolvedNamespaceQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedNamespaceQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context) return null;
        var referencedDeclarations = context.ReferencedDeclaration.MembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedNamespaceQualifiedNameExpressionNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial IUnresolvedTypeQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedTypeQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node)
    {
        if (node.Context is not ITypeNameNode { ReferencedDeclaration: { } referencedDeclaration } context) return null;
        var referencedDeclarations = referencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedTypeQualifiedNameExpressionNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial INameExpressionNode? UnresolvedNamespaceQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedNamespaceQualifiedNameExpressionNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.Count == 0)
            // Cannot resolve namespace member access, no need to process other rewrites
            return node;

        if (referencedDeclarations.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return IFunctionNameExpressionNode.Create(node.Syntax, node.Context, node.MemberName, node.GenericArguments, referencedFunctions);

        switch (referencedDeclarations.TrySingle())
        {
            case INamespaceDeclarationNode referencedNamespace:
                return IQualifiedNamespaceNameNode.Create(node.Syntax, node.Context, referencedNamespace);
            case ITypeDeclarationNode referencedType:
                // TODO select correct type declaration based on generic arguments
                // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                return IQualifiedTypeNameNode.Create(node.Syntax, node.Context, node.GenericArguments);
        }

        return null;
    }

    // TODO diagnostics for UnresolvedNamespaceQualifiedNameExpression

    public static partial INameExpressionNode? UnresolvedTypeQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedTypeQualifiedNameExpressionNode node)
    {
        // TODO metatypes would change this into an ordinary expression

        var referencedDeclarations = node.ReferencedDeclarations;
        if (referencedDeclarations.Count == 0)
            // Cannot resolve type associated member access, no need to process other rewrites
            return node;

        if (referencedDeclarations.TryAllOfType<IAssociatedFunctionDeclarationNode>(out var referencedFunctions))
            return IFunctionNameExpressionNode.Create(node.Syntax, node.Context, node.MemberName, node.GenericArguments, referencedFunctions);

        if (referencedDeclarations.TryAllOfType<IInitializerDeclarationNode>(out var referencedInitializers))
            // TODO handle type arguments (which are not allowed for initializers)
            return IInitializerNameExpressionNode.Create(node.Syntax, node.Context, node.MemberName, referencedInitializers);

        // TODO select correct type declaration based on generic arguments
        if (referencedDeclarations.TrySingle() is ITypeDeclarationNode referencedType)
            // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
            return IQualifiedTypeNameNode.Create(node.Syntax, node.Context, node.GenericArguments);

        return null;
    }

    // TODO diagnostics for UnresolvedTypeQualifiedNameExpression
    #endregion

    #region Unresolved Names
    public static partial IFixedList<INamespaceOrTypeDeclarationNode> UnresolvedOrdinaryName_ReferencedDeclarations(IUnresolvedOrdinaryNameNode node)
        => node.ContainingLexicalScope.Lookup<INamespaceOrTypeDeclarationNode>(node.Name).ToFixedList();

    public static partial void UnresolvedOrdinaryName_Contribute_Diagnostics(IUnresolvedOrdinaryNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.ReferencedDeclarations.Count)
        {
            case 0:
                // TODO replace with more specific error about failing to bind a namespace or type name
                diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
                break;
            case 1:
                // If there is only one match, then ReferencedSymbol is not null
                throw new UnreachableException();
            default:
                // TODO replace with more specific error about failing to bind a namespace or type name
                // TODO better errors explaining. For example, are they different kinds of declarations?
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.Span));
                break;
        }
    }

    public static partial INameNode? UnresolvedIdentifierName_ReplaceWith_Name(IUnresolvedIdentifierNameNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case INamespaceDeclarationNode referencedNamespace:
                    return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespace);
                case ITypeDeclarationNode referencedType:
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    return IIdentifierTypeNameNode.Create(node.Syntax);
            }

        return null;
    }

    public static partial INameNode? UnresolvedGenericName_ReplaceWith_Name(IUnresolvedGenericNameNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    return IGenericTypeNameNode.Create(node.Syntax, node.GenericArguments);
            }

        return null;
    }

    public static partial IUnresolvedNamespaceQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedNamespaceQualifiedName(IUnresolvedNameQualifiedNameNode node)
    {
        if (node.Context is not INamespaceNameNode context) return null;
        var referencedDeclarations = context.ReferencedDeclaration.MembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedNamespaceQualifiedNameNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial IUnresolvedTypeQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedTypeQualifiedName(IUnresolvedNameQualifiedNameNode node)
    {
        if (node.Context is not ITypeNameNode { ReferencedDeclaration: { } referencedDeclaration } context) return null;
        var referencedDeclarations = referencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedTypeQualifiedNameNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial INameNode? UnresolvedNamespaceQualifiedName_ReplaceWith_Name(IUnresolvedNamespaceQualifiedNameNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.Count == 0)
            // Cannot resolve namespace member access, no need to process other rewrites
            return node;

        switch (referencedDeclarations.TrySingle())
        {
            case INamespaceDeclarationNode referencedNamespace:
                return IQualifiedNamespaceNameNode.Create(node.Syntax, node.Context, referencedNamespace);

            case ITypeDeclarationNode referencedType:
                // TODO select correct type declaration based on generic arguments
                // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                return IQualifiedTypeNameNode.Create(node.Syntax, node.Context, node.GenericArguments);
        }

        return null;
    }

    // TODO diagnostics for UnresolvedNamespaceQualifiedName

    public static partial INameNode? UnresolvedTypeQualifiedName_ReplaceWith_Name(IUnresolvedTypeQualifiedNameNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;
        if (referencedDeclarations.Count == 0)
            // Cannot resolve type associated member access, no need to process other rewrites
            return node;

        // TODO select correct type declaration based on generic arguments
        if (referencedDeclarations.TrySingle() is ITypeDeclarationNode referencedType)
            throw new NotImplementedException();
        // return IQualifiedTypeNameNode.Create(node.Syntax, node.Context, node.GenericArguments, referencedType);

        return null;
    }

    // TODO diagnostics for UnresolvedTypeQualifiedName
    #endregion

    private static bool TryAllOfType<T>(
        this IReadOnlyCollection<IDeclarationNode> declarations,
        out IFixedList<T> referencedNamespaces)
        where T : IDeclarationNode
    {
        if (declarations.Count == 0)
        {
            referencedNamespaces = [];
            return false;
        }
        referencedNamespaces = declarations.OfType<T>().ToFixedList();
        // All of type T when counts match
        return referencedNamespaces.Count == declarations.Count;
    }
}
