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
            return new FunctionGroupName(node.Syntax, null, node.Name, FixedList.Empty<ITypeNode>(), referencedFunctions);

        if (node.ReferencedDeclarations.TrySingle() is not null and var referencedDeclaration)
            switch (referencedDeclaration)
            {
                case INamedBindingNode referencedVariable:
                    return new VariableNameExpressionNode(node.Syntax, referencedVariable);
                case ITypeDeclarationNode referencedType:
                    return new StandardTypeNameExpressionNode(node.Syntax, referencedType);
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

    public static IAmbiguousNameExpressionNode? MemberAccessExpression_Rewrite(IMemberAccessExpressionNode node)
    {
        if (node.Context is not INamespaceNameNode context)
            return null;

        var members = context.ReferencedDeclarations.SelectMany(d => d.MembersNamed(node.MemberName)).ToFixedSet();
        if (members.Count == 0)
            return null;

        if (members.TryAllOfType<INamespaceDeclarationNode>(out var referencedNamespaces))
            return new QualifiedNamespaceNameNode(node.Syntax, context, referencedNamespaces);

        if (members.TryAllOfType<IFunctionDeclarationNode>(out var referencedFunctions))
            return new FunctionGroupName(node.Syntax, context, node.MemberName, node.TypeArguments, referencedFunctions);

        return null;
    }

    private static bool TryAllOfType<T>(
        this IReadOnlyCollection<IDeclarationNode> declarations,
        out IFixedList<T> referencedNamespaces)
        where T : IDeclarationNode
    {
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
