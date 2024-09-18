using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class OverloadResolutionAspect
{
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IFunctionGroupNameNode function) return null;

        return IFunctionInvocationExpressionNode.Create(node.Syntax, function, node.CurrentArguments);
    }

    public static partial IFixedSet<IFunctionInvocableDeclarationNode> FunctionInvocationExpression_CompatibleDeclarations(
        IFunctionInvocationExpressionNode node)
    {
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForFunction(arguments);
        return node.FunctionGroup.ReferencedDeclarations
                   .Select(AntetypeContextualizedOverload.Create)
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IFunctionInvocableDeclarationNode? FunctionInvocationExpression_ReferencedDeclaration(
        IFunctionInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static partial void FunctionInvocationExpression_Contribute_Diagnostics(
        IFunctionInvocationExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null)
            return;

        switch (node.CompatibleDeclarations.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(node.File, node.Syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(node.File, node.Syntax));
                break;
        }
    }

    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IMethodGroupNameNode method) return null;

        return new MethodInvocationExpressionNode(node.Syntax, method, node.CurrentArguments);
    }

    public static partial IFixedSet<IStandardMethodDeclarationNode> MethodInvocationExpression_CompatibleDeclarations(
        IMethodInvocationExpressionNode node)
    {
        var contextAntetype = node.MethodGroup.Context.Antetype;
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForMethod(contextAntetype, arguments);
        return node.MethodGroup.ReferencedDeclarations
                   .Select(d => AntetypeContextualizedOverload.Create(contextAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    private static IMaybeExpressionAntetype AntetypeIfKnown(IExpressionNode? node)
    {
        if (node is not null && !node.ShouldNotBeExpression())
            return node.Antetype;
        return IAntetype.Unknown;
    }

    public static partial IStandardMethodDeclarationNode? MethodInvocationExpression_ReferencedDeclaration(
        IMethodInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static partial void MethodInvocationExpression_Contribute_Diagnostics(
        IMethodInvocationExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null) return;

        switch (node.CompatibleDeclarations.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMethod(node.File, node.Syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousMethodCall(node.File, node.Syntax));
                break;
        }
    }

    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(
        IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not ITypeNameExpressionNode context) return null;

        // Rewrite to insert an initializer group name node between the type name expression and the
        // invocation expression.
        var referencedDeclarations = context.ReferencedDeclaration.Members.OfType<IInitializerDeclarationNode>()
                                            .Where(c => c.Name is null).ToFixedSet();

        var initializerGroupName = IInitializerGroupNameNode.Create(context.Syntax, context, null, referencedDeclarations);
        return IUnresolvedInvocationExpressionNode.Create(node.Syntax, initializerGroupName, node.CurrentArguments);
    }

    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(
        IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IInitializerGroupNameNode initializer)
            return null;

        return IInitializerInvocationExpressionNode.Create(node.Syntax, initializer, node.CurrentArguments);
    }

    public static partial IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(
        IInitializerInvocationExpressionNode node)
    {
        var initializingAntetype = node.InitializerGroup.InitializingAntetype;
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForInitializer(arguments);
        return node.InitializerGroup.ReferencedDeclarations
                   .Select(d => AntetypeContextualizedOverload.Create(initializingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(
        IInitializerInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not { Antetype: FunctionAntetype } expression)
            return null;

        return new FunctionReferenceInvocationExpressionNode(node.Syntax, expression, node.CurrentArguments);
    }

    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_ToUnknown(IUnresolvedInvocationExpressionNode node)
        => new UnknownInvocationExpressionNode(node.Syntax, node.CurrentExpression, node.CurrentArguments);

    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(
        INewObjectExpressionNode node)
    {
        var constructingAntetype = node.ConstructingAntetype;
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForConstructor(arguments);
        return node.ReferencedConstructors
                   .Select(d => AntetypeContextualizedOverload.Create(constructingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node)
        => node.CompatibleConstructors.TrySingle();

    public static partial void NewObjectExpression_Contribute_Diagnostics(INewObjectExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.ConstructingAntetype)
        {
            case UnknownAntetype:
                // Error should be reported elsewhere
                return;
            case NominalAntetype { DeclaredAntetype.IsAbstract: true }:
                diagnostics.Add(OtherSemanticError.CannotConstructAbstractType(node.File, node.ConstructingType.Syntax));
                return;
        }

        if (node.ReferencedConstructor is not null)
            return;

        switch (node.CompatibleConstructors.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindConstructor(node.File, node.Syntax.Span));
                break;
            case 1:
                throw new UnreachableException("ReferencedConstructor would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousConstructorCall(node.File, node.Syntax.Span));
                break;
        }
    }
}
