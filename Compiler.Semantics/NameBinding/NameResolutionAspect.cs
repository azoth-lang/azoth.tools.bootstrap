using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

/// <summary>
/// Name resolution is the part of name binding that determines what kind of thing a name refers to.
/// </summary>
internal static partial class NameResolutionAspect
{
    #region Unresolved Expressions
    public static partial IExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_Expression(IUnresolvedMemberAccessExpressionNode node)
    {
        if (node.Context is not { } context)
            return null;

        // No need to check if context is INamespaceNameNode or ITypeNameExpressionNode because
        // those rewrites have already run and stop rewriting in those cases.

        // Ignore names that never have members
        if (context is IFunctionGroupNameNode
            or IMethodGroupNameNode
            or IInitializerGroupNameNode)
            return null;

        var contextTypeDeclaration = node.PackageNameScope().Lookup(context.PlainType);
        // TODO members needs to be filtered to visible accessible members
        var members = contextTypeDeclaration?.InclusiveInstanceMembersNamed(node.MemberName).ToFixedSet() ?? [];
        if (members.Count == 0)
            return null;

        if (members.TryAllOfType<IOrdinaryMethodDeclarationNode>(out var referencedMethods))
            return IMethodGroupNameNode.Create(node.Syntax, context, node.GenericArguments, referencedMethods);

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
            case IFunctionGroupNameNode or IFunctionNameExpressionNode or IMethodGroupNameNode:
                diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span,
                    "No member accessible from function or method."));
                break;
            case INamespaceNameNode:
            case ITypeNameExpressionNode:
                diagnostics.Add(NameBindingError.CouldNotBindMember(node.File, node.Syntax.MemberNameSpan));
                break;
            case IUnresolvedNameExpressionNode:
            case IUnresolvedInvocationExpressionNode:
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

    #region Instance Member Access Expressions
    public static partial IMethodAccessExpressionNode? MethodGroupName_ReplaceWith_MethodAccessExpression(IMethodGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is aren't multiple declarations, then it isn't ambiguous (it may fail to reference if there are zero).
        return IMethodAccessExpressionNode.Create(node.Syntax, node.Context, node.GenericArguments,
            node.ReferencedDeclarations, node.CallCandidates, node.CompatibleCallCandidates, node.SelectedCallCandidate,
            node.ReferencedDeclaration);
    }

    public static partial void MethodGroupName_Contribute_Diagnostics(
        IMethodGroupNameNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        // TODO develop a better check that this node is ambiguous
        if (node.Parent is IUnresolvedInvocationExpressionNode) return;

        if (node.CompatibleCallCandidates.Count == 0)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
        else if (node.CompatibleCallCandidates.Count > 1)
            // TODO provide the expected method type that didn't match
            diagnostics.Add(TypeError.AmbiguousMethodGroup(node.File, node.Syntax, Type.Unknown));
    }
    #endregion

    #region Operator Expressions
    public static partial IExpressionNode? AssignmentExpression_PropertyNameLeftOperand_Rewrite(IAssignmentExpressionNode node)
    {
        // TODO refactor to a replacement of the getter node based on an inherited attribute. That
        // should allow optimization of caching compared to a full rewrite.

        if (node.TempLeftOperand is not IGetterInvocationExpressionNode getterInvocation) return null;

        return ISetterInvocationExpressionNode.Create(node.Syntax, getterInvocation.Context,
            getterInvocation.PropertyName, node.CurrentRightOperand, getterInvocation.ReferencedDeclarations);
    }
    #endregion

    #region Name Expressions
    public static partial void Validate_FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        IResolvedNameNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> genericArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations)
    { }
    // TODO add this validation in somehow
    //=> Requires.That(!ReferencedDeclarations.IsEmpty, nameof(referencedDeclarations),
    //    "Must be at least one referenced declaration");

    public static partial IFunctionNameExpressionNode? FunctionGroupName_ReplaceWith_FunctionNameExpression(IFunctionGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is only one declaration, then it isn't ambiguous
        return IFunctionNameExpressionNode.Create(node.Syntax, node.Context, node.FunctionName, node.GenericArguments,
            node.ReferencedDeclarations, node.CallCandidates, node.CompatibleCallCandidates,
            node.SelectedCallCandidate, node.ReferencedDeclaration);
    }

    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO develop a better check that this node is ambiguous
        if (node.Parent is IUnresolvedInvocationExpressionNode)
            return;

        if (node.CompatibleCallCandidates.Count == 0)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
        else if (node.CompatibleCallCandidates.Count > 1)
            // TODO provide the expected function type that didn't match
            diagnostics.Add(TypeError.AmbiguousFunctionGroup(node.File, node.Syntax, Type.Unknown));
    }

    public static partial IInitializerNameExpressionNode? InitializerGroupName_ReplaceWith_InitializerNameExpression(IInitializerGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is aren't multiple declarations, then it isn't ambiguous (it may fail to reference if there are zero).
        return IInitializerNameExpressionNode.Create(node.Syntax, node.Context, node.InitializerName,
            node.ReferencedDeclarations, node.CallCandidates, node.CompatibleCallCandidates, node.SelectedCallCandidate,
            node.ReferencedDeclaration);
    }

    // TODO diagnostics for errors binding InitializerGroupName?
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

        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (referencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespaces);

        if (referencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(),
                referencedFunctions);

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ILocalBindingNode referencedVariable:
                    return IVariableNameExpressionNode.Create(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    return IOrdinaryTypeNameExpressionNode.Create(node.Syntax, FixedList.Empty<ITypeNode>(),
                        referencedType);
            }

        return null;
    }

    public static partial INameExpressionNode? UnresolvedGenericNameExpression_ReplaceWith_NameExpression(IUnresolvedGenericNameExpressionNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, null, node.Name, node.GenericArguments, referencedFunctions);

        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    return IOrdinaryTypeNameExpressionNode.Create(node.Syntax, node.GenericArguments, referencedType);
            }

        return null;
    }

    public static partial IUnresolvedNamespaceQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedNamespaceQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context) return null;
        var referencedDeclarations = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        return IUnresolvedNamespaceQualifiedNameExpressionNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial IUnresolvedTypeQualifiedNameExpressionNode? UnresolvedNameExpressionQualifiedNameExpression_ReplaceWith_UnresolvedTypeQualifiedNameExpression(IUnresolvedNameExpressionQualifiedNameExpressionNode node)
    {
        if (node.Context is not ITypeNameExpressionNode context) return null;
        var referencedDeclarations = context.ReferencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedTypeQualifiedNameExpressionNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial INameExpressionNode? UnresolvedNamespaceQualifiedNameExpression_ReplaceWith_NameExpression(IUnresolvedNamespaceQualifiedNameExpressionNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.Count == 0)
            // Cannot resolve namespace member access, no need to process other rewrites
            return node;

        if (referencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IQualifiedNamespaceNameNode.Create(node.Syntax, node.Context, referencedNamespaces);

        if (referencedDeclarations.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, node.Context, node.MemberName, node.GenericArguments,
                referencedFunctions);

        // TODO select correct type declaration based on generic arguments
        if (referencedDeclarations.TrySingle() is ITypeDeclarationNode referencedType)
            return IQualifiedTypeNameExpressionNode.Create(node.Syntax, node.Context, node.GenericArguments,
                referencedType);

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
            return IFunctionGroupNameNode.Create(node.Syntax, node.Context, node.MemberName, node.GenericArguments,
                referencedFunctions);

        if (referencedDeclarations.TryAllOfType<IInitializerDeclarationNode>(out var referencedInitializers))
            // TODO handle type arguments (which are not allowed for initializers)
            return IInitializerGroupNameNode.Create(node.Syntax, node.Context, node.Context.Name, referencedInitializers);

        return null;
    }

    // TODO diagnostics for UnresolvedTypeQualifiedNameExpression
    #endregion

    #region Unresolved Names
    public static partial IFixedList<INamespaceOrOrdinaryTypeDeclarationNode> UnresolvedOrdinaryName_ReferencedDeclarations(IUnresolvedOrdinaryNameNode node)
        => node.ContainingLexicalScope.Lookup<INamespaceOrOrdinaryTypeDeclarationNode>(node.Name).ToFixedList();

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

        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (referencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespaces);


        if (referencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    throw new NotImplementedException();
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    //return IIdentifierTypeNameNode.Create(node.Syntax);
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
                    throw new NotImplementedException();
                    // TODO a way to pass along referenced declarations rather than requiring they be figured out again?
                    //return IGenericTypeNameNode.Create(node.Syntax, node.GenericArguments);
            }

        return null;
    }

    public static partial IUnresolvedNamespaceQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedNamespaceQualifiedName(IUnresolvedNameQualifiedNameNode node)
    {
        if (node.Context is not INamespaceNameNode context) return null;
        var referencedDeclarations = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        return IUnresolvedNamespaceQualifiedNameNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial IUnresolvedTypeQualifiedNameNode? UnresolvedNameQualifiedName_ReplaceWith_UnresolvedTypeQualifiedName(IUnresolvedNameQualifiedNameNode node)
    {
        if (node.Context is not ITypeNameExpressionNode context) return null;
        var referencedDeclarations = context.ReferencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        return IUnresolvedTypeQualifiedNameNode.Create(node.Syntax, context, node.GenericArguments, referencedDeclarations);
    }

    public static partial INameNode? UnresolvedNamespaceQualifiedName_ReplaceWith_Name(IUnresolvedNamespaceQualifiedNameNode node)
    {
        var referencedDeclarations = node.ReferencedDeclarations;

        if (referencedDeclarations.Count == 0)
            // Cannot resolve namespace member access, no need to process other rewrites
            return node;

        if (referencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IQualifiedNamespaceNameNode.Create(node.Syntax, node.Context, referencedNamespaces);

        // TODO select correct type declaration based on generic arguments
        if (referencedDeclarations.TrySingle() is ITypeDeclarationNode referencedType)
            throw new NotImplementedException();
        // return IQualifiedTypeNameNode.Create(node.Syntax, node.Context, node.GenericArguments, referencedType);

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
