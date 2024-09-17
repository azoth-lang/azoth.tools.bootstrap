using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class BindingAmbiguousNamesAspect
{
    public static partial IFixedList<IDeclarationNode> StandardNameExpression_ReferencedDeclarations(IStandardNameExpressionNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).ToFixedList();

    public static partial IAmbiguousNameExpressionNode? IdentifierNameExpression_Rewrite(IIdentifierNameExpressionNode node)
    {
        // If not all referenced declarations are namespaces, then this is not a namespace name.
        if (node.ReferencedDeclarations.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IUnqualifiedNamespaceNameNode.Create(node.Syntax, referencedNamespaces);

        if (node.ReferencedDeclarations.TryAllOfType<IFunctionInvocableDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(), referencedFunctions);

        if (node.ReferencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case ILocalBindingNode referencedVariable:
                    return IVariableNameExpressionNode.Create(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    return new StandardTypeNameExpressionNode(node.Syntax, FixedList.Empty<ITypeNode>(), referencedType);
            }

        return IUnknownIdentifierNameExpressionNode.Create(node.Syntax, node.ReferencedDeclarations);
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

    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_FunctionOrMethodGroupNameContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not IExpressionNode context)
            return null;

        if (context is not (IFunctionGroupNameNode or IMethodGroupNameNode))
            return null;

        return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments,
            FixedList.Empty<IDefinitionNode>());
    }

    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_NamespaceNameContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context)
            return null;

        var members = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        if (members.Count == 0)
            return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments,
                FixedList.Empty<IDefinitionNode>());

        if (members.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return IQualifiedNamespaceNameNode.Create(node.Syntax, context, referencedNamespaces);

        if (members.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        if (members.TrySingle() is ITypeDeclarationNode referencedType)
            return new QualifiedTypeNameExpression(node.Syntax, context, node.TypeArguments, referencedType);

        return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_TypeNameExpressionContext(
        IMemberAccessExpressionNode node)
    {
        if (node.Context is not ITypeNameExpressionNode { ReferencedDeclaration: IUserTypeDeclarationNode referencedDeclaration } context)
            return null;

        var members = referencedDeclaration.AssociatedMembersNamed(node.MemberName).ToFixedSet();
        if (members.Count == 0)
            return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, FixedList.Empty<IDefinitionNode>());

        if (members.TryAllOfType<IAssociatedFunctionDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupNameNode(node.Syntax, context, node.MemberName, node.TypeArguments,
                referencedFunctions);

        if (members.TryAllOfType<IInitializerDeclarationNode>(out var referencedInitializers))
            // TODO handle type arguments (which are not allowed for initializers)
            return IInitializerGroupNameNode.Create(node.Syntax, context, context.Name, referencedInitializers);

        return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_ExpressionContext(
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

        var contextTypeDeclaration = node.PackageNameScope().Lookup(context.Antetype);
        var members = contextTypeDeclaration?.InclusiveInstanceMembersNamed(node.MemberName).ToFixedSet() ?? [];
        if (members.Count == 0)
            return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, FixedList.Empty<IDefinitionNode>());

        if (members.TryAllOfType<IStandardMethodDeclarationNode>(out var referencedMethods))
            return new MethodGroupNameNode(node.Syntax, context, node.MemberName, node.TypeArguments, referencedMethods);

        if (members.TryAllOfType<IPropertyAccessorDeclarationNode>(out var referencedProperties)
            && node.TypeArguments.Count == 0)
            return new PropertyNameNode(node.Syntax, context, node.MemberName, referencedProperties);

        if (members.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case IFieldDeclarationNode fieldDeclaration:
                    return new FieldAccessExpressionNode(node.Syntax, context, fieldDeclaration.Name, fieldDeclaration);
            }

        return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, members);
    }

    public static partial IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite_UnknownNameExpressionContext(IMemberAccessExpressionNode node)
    {
        if (node.Context is not IUnknownNameExpressionNode context)
            return null;

        return IUnknownMemberAccessExpressionNode.Create(node.Syntax, context, node.TypeArguments, FixedList.Empty<IDeclarationNode>());
    }

    public static partial IAmbiguousNameExpressionNode? PropertyName_Rewrite(IPropertyNameNode node)
    {
        if (node.Parent is IAssignmentExpressionNode assignmentExpression
            && assignmentExpression.TempLeftOperand == node)
        {
            // Property on left hand of assignment will be rewritten by the assignment expression
            return null;
        }

        var getter = node.ReferencedPropertyAccessors.OfType<IGetterMethodDeclarationNode>().TrySingle();
        return new GetterInvocationExpressionNode(node.Syntax, node.Context, node.PropertyName,
            node.ReferencedPropertyAccessors, getter);
    }

    public static partial IExpressionNode? AssignmentExpression_Rewrite_PropertyNameLeftOperand(IAssignmentExpressionNode node)
    {
        if (node.TempLeftOperand is not IPropertyNameNode propertyName)
            return null;

        var setter = propertyName.ReferencedPropertyAccessors.OfType<ISetterMethodDeclarationNode>().TrySingle();
        return new SetterInvocationExpressionNode(node.Syntax, propertyName.Context,
            propertyName.PropertyName, node.CurrentRightOperand,
            propertyName.ReferencedPropertyAccessors, setter);
    }

    public static IAmbiguousExpressionNode? FunctionGroupName_Rewrite(IFunctionGroupNameNode node)
    {
        // TODO develop a better check for names that don't need resolved
        if (node.Parent is IFunctionNameNode or IUnresolvedInvocationExpressionNode or IFunctionInvocationExpressionNode
           || node.ReferencedDeclarations.Count > 1)
            return null;

        // if there is only one declaration, then it isn't ambiguous
        return IFunctionNameNode.Create(node.Syntax, node.Context, node.FunctionName, node.TypeArguments,
            node.ReferencedDeclarations, node.ReferencedDeclarations.TrySingle());
    }

    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO develop a better check that this node is ambiguous
        if (node.Parent is IFunctionNameNode or IUnresolvedInvocationExpressionNode or IFunctionInvocationExpressionNode)
            return;

        // TODO this should be based on how many compatible declarations there are
        if (node.ReferencedDeclarations.Count == 0)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
        else if (node.ReferencedDeclarations.Count > 1)
            // TODO provide the expected function type that didn't match
            diagnostics.Add(TypeError.AmbiguousFunctionGroup(node.File, node.Syntax, DataType.Unknown));
    }

    public static partial void UnknownMemberAccessExpression_Contribute_Diagnostics(
        IUnknownMemberAccessExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
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
}
