using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class OverloadResolutionAspect
{
    public static IExpressionNode? InvocationExpression_Rewrite_FunctionGroupNameExpression(IInvocationExpressionNode node)
    {
        if (node.IntermediateExpression is not IFunctionGroupNameNode function) return null;

        return new FunctionInvocationExpressionNode(node.Syntax, function, node.CurrentArguments);
    }

    public static IFixedSet<IFunctionLikeDeclarationNode> FunctionInvocationExpression_CompatibleDeclarations(
        IFunctionInvocationExpressionNode node)
    {
        // For now, overload resolution is based on number of arguments only, not their types. This
        // avoids the issues with flow typing interacting with overload resolution. In the future,
        // we may want to revisit this and implement a more sophisticated overload resolution algorithm.
        var arity = node.Arguments.Count;
        return node.FunctionGroup.ReferencedDeclarations.Where(d => d.Type.Arity == arity).ToFixedSet();
    }

    public static IFunctionLikeDeclarationNode? FunctionInvocationExpression_ReferencedDeclaration(
        IFunctionInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static void FunctionInvocationExpression_ContributeDiagnostics(
        IFunctionInvocationExpressionNode node,
        Diagnostics diagnostics)
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
                // TODO add back when overloading is fixed
                //diagnostics.Add(NameBindingError.AmbiguousFunctionCall(node.File, node.Syntax));
                break;
        }
    }

    public static IExpressionNode? InvocationExpression_Rewrite_MethodGroupNameExpression(IInvocationExpressionNode node)
    {
        if (node.IntermediateExpression is not IMethodGroupNameNode method) return null;

        return new MethodInvocationExpressionNode(node.Syntax, method, node.CurrentArguments);
    }

    public static IFixedSet<IStandardMethodDeclarationNode> MethodInvocationExpression_CompatibleDeclarations(
        IMethodInvocationExpressionNode node)
    {
        var arity = node.Arguments.Count;
        return node.MethodGroup.ReferencedDeclarations.Where(d => d.Arity == arity).ToFixedSet();
    }

    public static IStandardMethodDeclarationNode? MethodInvocationExpression_ReferencedDeclaration(
        IMethodInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static IAmbiguousExpressionNode? InvocationExpression_Rewrite_TypeNameExpression(
        IInvocationExpressionNode node)
    {
        if (node.IntermediateExpression is not ITypeNameExpressionNode context) return null;

        // Rewrite to insert an initializer group name node between the type name expression and the
        // invocation expression.
        var referencedDeclarations = context.ReferencedDeclaration.Members.OfType<IInitializerDeclarationNode>()
                                            .Where(c => c.Name is null).ToFixedSet();

        var initializerGroupName = new InitializerGroupNameNode(context.Syntax, context, null, referencedDeclarations);
        return new InvocationExpressionNode(node.Syntax, initializerGroupName, node.CurrentArguments);
    }

    public static IAmbiguousExpressionNode? InvocationExpression_Rewrite_InitializerGroupNameExpression(
        IInvocationExpressionNode node)
    {
        if (node.IntermediateExpression is not IInitializerGroupNameNode initializer)
            return null;

        return new InitializerInvocationExpressionNode(node.Syntax, initializer, node.CurrentArguments);
    }


    public static IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(
        IInitializerInvocationExpressionNode node)
    {
        var initializingAntetype = node.InitializerGroup.InitializingAntetype;
        var arity = node.Arguments.Count;
        return node.InitializerGroup.ReferencedDeclarations
                   .Select(i => ContextualizedOverload.Create(initializingAntetype, i))
                   .Where(d => d.Arity == arity)
                   .Select(c => c.Declaration).ToFixedSet();
    }

    public static IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(
        IInitializerInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static IExpressionNode? InvocationExpression_Rewrite_FunctionReferenceExpression(IInvocationExpressionNode node)
    {
        // TODO remove condition once all cases are handled
        if (!SemanticTreeTypeValidator.Validating)
            return null;

        if (node.IntermediateExpression is not IExpressionNode { Antetype: FunctionAntetype } expression)
            return null;

        return new FunctionReferenceInvocationNode(node.Syntax, expression, node.CurrentArguments);
    }

    public static IAmbiguousExpressionNode InvocationExpression_Rewrite_ToUnknown(IInvocationExpressionNode node)
        => new UnknownInvocationExpressionNode(node.Syntax, node.CurrentExpression, node.CurrentArguments);

    public static IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(
        INewObjectExpressionNode node)
    {
        var constructingAntetype = node.ConstructingAntetype;
        var arity = node.Arguments.Count;
        return node.ReferencedConstructors
                   .Select(c => ContextualizedOverload.Create(constructingAntetype, c))
                   .Where(c => c.Arity == arity)
                   .Select(c => c.Declaration).ToFixedSet();
    }

    public static IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node)
        => node.CompatibleConstructors.TrySingle();
}
