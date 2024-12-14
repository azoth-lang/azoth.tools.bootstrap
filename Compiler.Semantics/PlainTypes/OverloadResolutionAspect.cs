using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

// TODO there are a lot of places where binding errors need to be reported. It seems like that should be consolidated
internal static partial class OverloadResolutionAspect
{
    private static IMaybePlainType PlainTypeIfKnown(IExpressionNode? node)
    {
        if (node is not null && !node.ShouldNotBeExpression()) return node.PlainType;
        return PlainType.Unknown;
    }

    #region Expressions
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(
        INewObjectExpressionNode node)
    {
        var constructingPlainType = node.ConstructingPlainType;
        var arguments = node.Arguments.Select(PlainTypeIfKnown);
        var argumentPlainTypes = ArgumentPlainTypes.ForConstructor(arguments);
        return node.ReferencedConstructors
                   .Select(d => CallCandidate.Create(constructingPlainType, d))
                   .Where(o => o.CompatibleWith(argumentPlainTypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node)
        => node.CompatibleConstructors.TrySingle();

    public static partial void NewObjectExpression_Contribute_Diagnostics(
        INewObjectExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.ConstructingPlainType)
        {
            case UnknownPlainType:
                // Error should be reported elsewhere
                return;
            case NeverPlainType:
            case AssociatedPlainType:
            case GenericParameterPlainType:
            case ConstructedPlainType { TypeConstructor.CanBeInstantiated: false }:
                // TODO type variables, empty types and others also cannot be constructed. Report proper error message in that case
                diagnostics.Add(
                    OtherSemanticError.CannotConstructAbstractType(node.File, node.ConstructingType.Syntax));
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
    #endregion

    #region Invocation Expressions
    public static partial IMaybePlainType? UnknownInvocationExpression_Expression_ExpectedPlainType(IUnknownInvocationExpressionNode node)
    {
        var expectedReturnPlainType = node.ExpectedPlainType ?? PlainType.Unknown;
        return new FunctionPlainType(node.Arguments.Select(NonVoidPlainTypeIfKnown),
            // TODO this is odd, but the return plainType will be ignored
            NonVoidPlainTypeIfKnown(expectedReturnPlainType));
    }

    private static NonVoidPlainType NonVoidPlainTypeIfKnown(IExpressionNode? node)
        => NonVoidPlainTypeIfKnown(PlainTypeIfKnown(node));

    private static NonVoidPlainType NonVoidPlainTypeIfKnown(IMaybePlainType maybeExpressionPlainType)
    {
        if (maybeExpressionPlainType is NonVoidPlainType plainType) return plainType;
        // This is a little odd, but if the parameter type is not known, then using `never` will
        // cause nothing to match except for `never` itself.
        return PlainType.Never;
    }

    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_FunctionNameExpression(IUnknownInvocationExpressionNode node)
    {
        if (node.Expression is not IFunctionNameNode function)
            return null;

        return IFunctionInvocationExpressionNode.Create(node.Syntax, function, node.CurrentArguments);
    }

    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_MethodNameExpression(IUnknownInvocationExpressionNode node)
    {
        if (node.Expression is not IMethodNameNode method) return null;

        // TODO maybe the MethodInvocationExpression should not contain a MethodName. Instead, it
        // could directly contain the context and the method name identifier. That way, weirdness
        // about passing things through the MethodName layer could be avoided.
        return IMethodInvocationExpressionNode.Create(node.Syntax, method, node.CurrentArguments);
    }

    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_TypeNameExpression(IUnknownInvocationExpressionNode node)
    {
        if (node.Expression is not ITypeNameExpressionNode context) return null;

        // Rewrite to insert an initializer group name node between the type name expression and the
        // invocation expression.
        var referencedDeclarations = context.ReferencedDeclaration.Members.OfType<IInitializerDeclarationNode>()
                                            .Where(c => c.Name is null).ToFixedSet();

        var initializerGroupName = IInitializerGroupNameNode.Create(context.Syntax, context, null, referencedDeclarations);
        return IUnknownInvocationExpressionNode.Create(node.Syntax, initializerGroupName, node.CurrentArguments);
    }

    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_InitializerGroupNameExpression(IUnknownInvocationExpressionNode node)
    {
        if (node.Expression is not IInitializerGroupNameNode initializer)
            return null;

        return IInitializerInvocationExpressionNode.Create(node.Syntax, initializer, node.CurrentArguments);
    }

    public static partial IExpressionNode? UnknownInvocationExpression_Rewrite_FunctionReferenceExpression(IUnknownInvocationExpressionNode node)
    {
        if (node.Expression is not { PlainType: FunctionPlainType } expression)
            return null;

        return IFunctionReferenceInvocationExpressionNode.Create(node.Syntax, expression, node.CurrentArguments);
    }

    public static partial void UnknownInvocationExpression_Contribute_Diagnostics(IUnknownInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Expression)
        {
            case IFunctionGroupNameNode functionGroup:
                ContributeFunctionBindingDiagnostics(functionGroup.ReferencedDeclaration,
                    functionGroup.CompatibleCallCandidates, node.File, node.Syntax, diagnostics);
                break;
            case IFunctionNameNode function:
                ContributeFunctionBindingDiagnostics(function.ReferencedDeclaration,
                    function.CompatibleCallCandidates, node.File, node.Syntax, diagnostics);
                break;
            case IUnknownNameExpressionNode:
            case IUnknownInvocationExpressionNode:
                // These presumably report their own errors and should be ignored here
                break;
            // TODO other cases
            default:
                diagnostics.Add(NameBindingError.NotImplemented(node.File, node.Syntax.Span,
                    "Invocation not properly bound. Unknown call."));
                break;
        }
    }

    private static void ContributeFunctionBindingDiagnostics(
        IFunctionInvocableDeclarationNode? referencedDeclaration,
        IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> compatibleCallCandidates,
        CodeFile file,
        IInvocationExpressionSyntax syntax,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (referencedDeclaration is not null) return;

        switch (compatibleCallCandidates.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(file, syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, syntax));
                break;
        }
    }

    public static partial void MethodInvocationExpression_Contribute_Diagnostics(IMethodInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var method = node.Method;
        if (method.ReferencedDeclaration is not null)
            return;

        switch (method.CompatibleCallCandidates.Count)
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

    public static partial IFixedSet<IInitializerDeclarationNode> InitializerInvocationExpression_CompatibleDeclarations(IInitializerInvocationExpressionNode node)
    {
        var initializingPlainType = node.InitializerGroup.InitializingPlainType;
        var arguments = node.Arguments.Select(PlainTypeIfKnown);
        var argumentPlainTypes = ArgumentPlainTypes.ForInitializer(arguments);
        return node.InitializerGroup.ReferencedDeclarations
                   .Select(d => CallCandidate.Create(initializingPlainType, d))
                   .Where(o => o.CompatibleWith(argumentPlainTypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(IInitializerInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();
    #endregion

    #region Name Expressions
    public static partial IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CallCandidates(IFunctionGroupNameNode node)
        => node.ReferencedDeclarations.Select(CallCandidate.Create).ToFixedSet();

    public static partial IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> FunctionGroupName_CompatibleCallCandidates(IFunctionGroupNameNode node)
    {
        if (node.ExpectedPlainType is not FunctionPlainType expectedPlainType) return [];

        var argumentPlainTypes = ArgumentPlainTypes.ForFunction(expectedPlainType.Parameters);
        return node.CallCandidates.Where(o => o.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial CallCandidate<IFunctionInvocableDeclarationNode>? FunctionGroupName_SelectedCallCandidate(IFunctionGroupNameNode node)
        => node.CompatibleCallCandidates.TrySingle();

    public static partial IFunctionInvocableDeclarationNode? FunctionGroupName_ReferencedDeclaration(IFunctionGroupNameNode node)
        => node.SelectedCallCandidate?.Declaration;

    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
        => ContributeFunctionNameBindingDiagnostics(node.ReferencedDeclaration, node.CompatibleCallCandidates, node, diagnostics);

    private static void ContributeFunctionNameBindingDiagnostics(
        IFunctionInvocableDeclarationNode? referencedDeclaration,
        IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> compatibleCallCandidates,
        INameExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (referencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnknownInvocationExpressionNode)
            return;

        switch (compatibleCallCandidates.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunctionName(node.File, node.Syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionName(node.File, node.Syntax));
                break;
        }
    }

    public static partial void FunctionName_Contribute_Diagnostics(IFunctionNameNode node, DiagnosticCollectionBuilder diagnostics)
        => ContributeFunctionNameBindingDiagnostics(node.ReferencedDeclaration, node.CompatibleCallCandidates, node, diagnostics);

    public static partial IFixedSet<CallCandidate<IStandardMethodDeclarationNode>> MethodGroupName_CallCandidates(IMethodGroupNameNode node)
        => node.ReferencedDeclarations.Select(m => CallCandidate.Create(node.Context.PlainType, m)).ToFixedSet();

    public static partial IFixedSet<CallCandidate<IStandardMethodDeclarationNode>> MethodGroupName_CompatibleCallCandidates(IMethodGroupNameNode node)
    {
        if (node.ExpectedPlainType is not FunctionPlainType expectedPlainType) return [];

        var contextPlainType = node.Context.PlainType;
        var argumentPlainTypes = ArgumentPlainTypes.ForMethod(contextPlainType, expectedPlainType.Parameters);
        return node.CallCandidates.Where(o => o.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial CallCandidate<IStandardMethodDeclarationNode>? MethodGroupName_SelectedCallCandidate(IMethodGroupNameNode node)
        => node.CompatibleCallCandidates.TrySingle();

    public static partial IStandardMethodDeclarationNode? MethodGroupName_ReferencedDeclaration(IMethodGroupNameNode node)
        => node.SelectedCallCandidate?.Declaration;

    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnknownInvocationExpressionNode)
            return;

        switch (node.CompatibleCallCandidates.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMethodName(node.File, node.Syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousMethodName(node.File, node.Syntax));
                break;
        }
    }
    #endregion
}
