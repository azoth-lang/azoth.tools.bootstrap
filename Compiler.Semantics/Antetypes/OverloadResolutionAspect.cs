using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class OverloadResolutionAspect
{
    public static IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IFunctionGroupNameNode function) return null;

        return new FunctionInvocationExpressionNode(node.Syntax, function, node.CurrentArguments);
    }

    public static IFixedSet<IFunctionLikeDeclarationNode> FunctionInvocationExpression_CompatibleDeclarations(
        IFunctionInvocationExpressionNode node)
    {
        var arguments = node.IntermediateArguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForFunction(arguments);
        return node.FunctionGroup.ReferencedDeclarations
                   .Select(AntetypeContextualizedOverload.Create)
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static IFunctionLikeDeclarationNode? FunctionInvocationExpression_ReferencedDeclaration(
        IFunctionInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static void FunctionInvocationExpression_ContributeDiagnostics(
        IFunctionInvocationExpressionNode node,
        DiagnosticsBuilder diagnostics)
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

    public static IExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IMethodGroupNameNode method) return null;

        return new MethodInvocationExpressionNode(node.Syntax, method, node.CurrentArguments);
    }

    public static IFixedSet<IStandardMethodDeclarationNode> MethodInvocationExpression_CompatibleDeclarations(
        IMethodInvocationExpressionNode node)
    {
        var contextAntetype = node.MethodGroup.Context.Antetype;
        var arguments = node.IntermediateArguments.Select(AntetypeIfKnown);
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

    public static IStandardMethodDeclarationNode? MethodInvocationExpression_ReferencedDeclaration(
        IMethodInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static void MethodInvocationExpression_ContributeDiagnostics(
        IMethodInvocationExpressionNode node,
        DiagnosticsBuilder diagnostics)
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

    public static IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(
        IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not ITypeNameExpressionNode context) return null;

        // Rewrite to insert an initializer group name node between the type name expression and the
        // invocation expression.
        var referencedDeclarations = context.ReferencedDeclaration.Members.OfType<IInitializerDeclarationNode>()
                                            .Where(c => c.Name is null).ToFixedSet();

        var initializerGroupName = new InitializerGroupNameNode(context.Syntax, context, null, referencedDeclarations);
        return new UnresolvedInvocationExpressionNode(node.Syntax, initializerGroupName, node.CurrentArguments);
    }

    public static IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(
        IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IInitializerGroupNameNode initializer)
            return null;

        return new InitializerInvocationExpressionNode(node.Syntax, initializer, node.CurrentArguments);
    }

    public static IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(
        IInitializerInvocationExpressionNode node)
    {
        var initializingAntetype = node.InitializerGroup.InitializingAntetype;
        var arguments = node.IntermediateArguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForInitializer(arguments);
        return node.InitializerGroup.ReferencedDeclarations
                   .Select(d => AntetypeContextualizedOverload.Create(initializingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(
        IInitializerInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static IExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IExpressionNode { Antetype: FunctionAntetype } expression)
            return null;

        return new FunctionReferenceInvocationExpressionNode(node.Syntax, expression, node.CurrentArguments);
    }

    public static IAmbiguousExpressionNode UnresolvedInvocationExpression_Rewrite_ToUnknown(IUnresolvedInvocationExpressionNode node)
        => new UnknownInvocationExpressionNode(node.Syntax, node.CurrentExpression, node.CurrentArguments);

    public static IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(
        INewObjectExpressionNode node)
    {
        var constructingAntetype = node.ConstructingAntetype;
        var arguments = node.IntermediateArguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForConstructor(arguments);
        return node.ReferencedConstructors
                   .Select(d => AntetypeContextualizedOverload.Create(constructingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node)
        => node.CompatibleConstructors.TrySingle();

    public static void NewObjectExpression_ContributeDiagnostics(INewObjectExpressionNode node, DiagnosticsBuilder diagnostics)
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
