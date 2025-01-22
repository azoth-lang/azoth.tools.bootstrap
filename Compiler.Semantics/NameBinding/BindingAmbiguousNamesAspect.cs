using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class BindingAmbiguousNamesAspect
{
    public static partial IFixedList<IDeclarationNode> OrdinaryNameExpression_ReferencedDeclarations(IOrdinaryNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();

    public static partial INameExpressionNode? IdentifierNameExpression_ReplaceWith_NameExpression(IIdentifierNameExpressionNode node)
    {
        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (node.ReferencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespaces);

        if (node.ReferencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(), referencedFunctions);

        if (node.ReferencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ILocalBindingNode referencedVariable:
                    return IVariableNameExpressionNode.Create(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    return IStandardTypeNameExpressionNode.Create(node.Syntax, FixedList.Empty<ITypeNode>(), referencedType);
            }

        // TODO theoretically, this has the same problem where uncached ReferencedDeclarations could cause premature rewrite to UnknownNameExpression
        return IUnknownIdentifierNameExpressionNode.Create(node.Syntax, node.ReferencedDeclarations);
    }

    public static partial INameExpressionNode? GenericNameExpression_ReplaceWith_NameExpression(IGenericNameExpressionNode node)
    {
        // TODO rename TypeArguments to GenericArguments
        if (node.ReferencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, null, node.Name, node.TypeArguments, referencedFunctions);

        if (node.ReferencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ITypeDeclarationNode referencedType:
                    return IStandardTypeNameExpressionNode.Create(node.Syntax, node.TypeArguments, referencedType);
            }

        // TODO theoretically, this has the same problem where uncached ReferencedDeclarations could cause premature rewrite to UnknownNameExpression
        return IUnknownGenericNameExpressionNode.Create(node.Syntax, node.TypeArguments, node.ReferencedDeclarations);
    }

    public static partial void UnknownIdentifierNameExpression_Contribute_Diagnostics(
        IUnknownIdentifierNameExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
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

    public static partial INameExpressionNode? UnresolvedMemberAccessExpression_NamespaceNameContext_ReplaceWith_NameExpression(IUnresolvedMemberAccessExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context)
            return null;

        var members = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        if (members.Count == 0)
            // definitely namespace member access, no need to process other rewrites
            return node;

        if (members.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IQualifiedNamespaceNameNode.Create(node.Syntax, context, referencedNamespaces);

        if (members.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        // TODO select correct type declaration based on generic arguments
        if (members.TrySingle() is ITypeDeclarationNode referencedType)
            return IQualifiedTypeNameExpressionNode.Create(node.Syntax, context, node.TypeArguments, referencedType);

        return IAmbiguousMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial INameExpressionNode? UnresolvedMemberAccessExpression_TypeNameExpressionContext_ReplaceWith_NameExpression(IUnresolvedMemberAccessExpressionNode node)
    {
        if (node.Context is not ITypeNameExpressionNode context)
            return null;

        // TODO metatypes would change this into an ordinary expression

        var members = context.ReferencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        if (members.Count == 0)
            // definitely associated member access, no need to process other rewrites
            return node;

        if (members.TryAllOfType<IAssociatedFunctionDeclarationNode>(out var referencedFunctions))
            return IFunctionGroupNameNode.Create(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        if (members.TryAllOfType<IInitializerDeclarationNode>(out var referencedInitializers))
            // TODO handle type arguments (which are not allowed for initializers)
            return IInitializerGroupNameNode.Create(node.Syntax, context, context.Name, referencedInitializers);

        return IAmbiguousMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial INameExpressionNode? UnresolvedMemberAccessExpression_ExpressionContext_ReplaceWith_NameExpression(IUnresolvedMemberAccessExpressionNode node)
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
            return IMethodGroupNameNode.Create(node.Syntax, context, node.MemberName, node.TypeArguments, referencedMethods);

        if (members.TryAllOfType<IPropertyAccessorDeclarationNode>(out var referencedProperties)
            && node.TypeArguments.Count == 0)
            // We don't really know that it is a getter, but if it isn't then it will be rewritten to a setter
            return IGetterInvocationExpressionNode.Create(node.Syntax, context, node.MemberName, referencedProperties);

        // TODO does this need to change for get vs set?
        if (members.Where(m => m is not IPropertyAccessorDeclarationNode)
                   .TrySingle() is IFieldDeclarationNode fieldDeclaration)
            return IFieldAccessExpressionNode.Create(node.Syntax, context, fieldDeclaration.Name, fieldDeclaration);

        return IAmbiguousMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial IExpressionNode? AssignmentExpression_PropertyNameLeftOperand_Rewrite(IAssignmentExpressionNode node)
    {
        // TODO refactor to a replacement of the getter node based on an inherited attribute. That
        // should allow optimization of caching compared to a full rewrite.

        if (node.TempLeftOperand is not IGetterInvocationExpressionNode getterInvocation)
            return null;

        return ISetterInvocationExpressionNode.Create(node.Syntax, getterInvocation.Context,
            getterInvocation.PropertyName, node.CurrentRightOperand,
            getterInvocation.ReferencedDeclarations);
    }

    public static partial void Validate_FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations)
    { }
    // TODO add this validation in somehow
    //=> Requires.That(!ReferencedDeclarations.IsEmpty, nameof(referencedDeclarations),
    //    "Must be at least one referenced declaration");

    public static partial IFunctionNameNode? FunctionGroupName_ReplaceWith_FunctionName(IFunctionGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is only one declaration, then it isn't ambiguous
        return IFunctionNameNode.Create(node.Syntax, node.Context, node.FunctionName, node.TypeArguments,
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

    public static partial IMethodNameNode? MethodGroupName_ReplaceWith_MethodName(IMethodGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is aren't multiple declarations, then it isn't ambiguous (it may fail to reference if there are zero).
        return IMethodNameNode.Create(node.Syntax, node.Context, node.MethodName, node.TypeArguments,
            node.ReferencedDeclarations, node.CallCandidates, node.CompatibleCallCandidates,
            node.SelectedCallCandidate, node.ReferencedDeclaration);
    }

    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO develop a better check that this node is ambiguous
        if (node.Parent is IUnresolvedInvocationExpressionNode) return;

        if (node.CompatibleCallCandidates.Count == 0)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
        else if (node.CompatibleCallCandidates.Count > 1)
            // TODO provide the expected method type that didn't match
            diagnostics.Add(TypeError.AmbiguousMethodGroup(node.File, node.Syntax, Type.Unknown));
    }

    public static partial IInitializerNameNode? InitializerGroupName_ReplaceWith_InitializerName(IInitializerGroupNameNode node)
    {
        if (node.CompatibleCallCandidates.Count > 1)
            // TODO should this be used instead?
            //if (node.ReferencedDeclaration is not null)
            return null;

        // if there is aren't multiple declarations, then it isn't ambiguous (it may fail to reference if there are zero).
        return IInitializerNameNode.Create(node.Syntax, node.Context, node.InitializerName,
            node.ReferencedDeclarations, node.CallCandidates, node.CompatibleCallCandidates, node.SelectedCallCandidate,
            node.ReferencedDeclaration);
    }


    public static partial void UnresolvedMemberAccessExpression_Contribute_Diagnostics(
        IUnresolvedMemberAccessExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Context)
        {
            case IFunctionGroupNameNode or IFunctionNameNode or IMethodGroupNameNode:
                diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "No member accessible from function or method."));
                break;
            case INamespaceNameNode:
            case ITypeNameExpressionNode:
                diagnostics.Add(NameBindingError.CouldNotBindMember(node.File, node.Syntax.MemberNameSpan));
                break;
            case IUnknownNameExpressionNode:
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

    public static partial void AmbiguousMemberAccessExpression_Contribute_Diagnostics(
        IAmbiguousMemberAccessExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Context)
        {
            case INamespaceNameNode:
            case ITypeNameExpressionNode:
                // TODO better errors explaining. For example, are they different kinds of declarations?
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.MemberNameSpan));
                break;
            case IUnknownNameExpressionNode:
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
    }

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
