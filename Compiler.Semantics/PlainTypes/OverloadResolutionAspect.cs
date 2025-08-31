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
    private static FunctionPlainType InvocationTargetExpectedPlainType(IFixedList<IExpressionNode?> args)
    {
        // The return type won't be used for overload resolution. However, using `never` logically
        // places no constraints on the expected return type.
        return new(args.Select(ArgumentPlainTypeIfKnown), PlainType.Never);

        static NonVoidPlainType ArgumentPlainTypeIfKnown(IExpressionNode? node)
        {
            if (node?.PlainType is NonVoidPlainType plainType) return plainType;
            // This is a little odd, but if the parameter type is not known, then using `never` will
            // cause nothing to match except for `never` itself.
            return PlainType.Never;
        }
    }

    #region Instance Member Access Expressions
    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodAccessExpression_CallCandidates(IMethodAccessExpressionNode node)
        => node.ReferencedDeclarations.Select(m => CallCandidate.Create(node.Context.PlainType, m)).ToFixedSet();

    public static partial IFixedSet<ICallCandidate<IOrdinaryMethodDeclarationNode>> MethodAccessExpression_CompatibleCallCandidates(IMethodAccessExpressionNode node)
    {
        var expectedPlainType = node.ExpectedPlainType; // Avoids repeated access
        if (expectedPlainType is null or UnknownPlainType) return node.CallCandidates;
        if (expectedPlainType is not FunctionPlainType { Parameters: var expectedParameters }) return [];

        var contextPlainType = node.Context.PlainType;
        var argumentPlainTypes = ArgumentPlainTypes.ForMethod(contextPlainType, expectedParameters);
        return node.CallCandidates.Where(o => o.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial ICallCandidate<IOrdinaryMethodDeclarationNode>? MethodAccessExpression_SelectedCallCandidate(IMethodAccessExpressionNode node)
        => node.CompatibleCallCandidates.TrySingle();

    public static partial void MethodAccessExpression_Contribute_Diagnostics(IMethodAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnresolvedInvocationExpressionNode)
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

    #region Invocation Expressions
    public static partial IMaybePlainType? UnresolvedInvocationExpression_Expression_ExpectedPlainType(IUnresolvedInvocationExpressionNode node)
        => InvocationTargetExpectedPlainType(node.Arguments);

    public static partial IFunctionInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_FunctionInvocationExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IFunctionNameExpressionNode function) return null;

        return IFunctionInvocationExpressionNode.Create(node.Syntax, function, node.CurrentArguments);
    }

    public static partial IMethodInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_MethodInvocationExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IMethodAccessExpressionNode method) return null;

        return IMethodInvocationExpressionNode.Create(node.Syntax, method, node.CurrentArguments);
    }

    public static partial IExpressionNode? UnresolvedInvocationExpression_TypeName_Rewrite(IUnresolvedInvocationExpressionNode node)
    {
        // TODO refactor to replacement of type name expression when used in invocation context
        // based on an inherited attribute. That should allow optimization of caching compared to a
        // full rewrite.

        if (node.Expression is not ITypeNameNode { ReferencedDeclaration: { } referencedDeclaration } context) return null;

        // Rewrite to insert an initializer group name node between the type name expression and the
        // invocation expression.
        var referencedDeclarations = referencedDeclaration.Members.OfType<IInitializerDeclarationNode>()
                                                          .Where(c => c.Name is null).ToFixedSet();

        var initializer = IInitializerNameExpressionNode.Create(context.Syntax, context, null, referencedDeclarations);
        return IInitializerInvocationExpressionNode.Create(node.Syntax, initializer, node.CurrentArguments);
    }

    public static partial IInitializerInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_InitializerInvocationExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not IInitializerNameExpressionNode initializer) return null;

        return IInitializerInvocationExpressionNode.Create(node.Syntax, initializer, node.CurrentArguments);
    }

    public static partial IFunctionReferenceInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_FunctionReferenceInvocationExpression(IUnresolvedInvocationExpressionNode node)
    {
        if (node.Expression is not { PlainType: FunctionPlainType } expression) return null;

        return IFunctionReferenceInvocationExpressionNode.Create(node.Syntax, expression, node.CurrentArguments);
    }

    public static partial INonInvocableInvocationExpressionNode? UnresolvedInvocationExpression_ReplaceWith_NonInvocableInvocationExpression(IUnresolvedInvocationExpressionNode node)
    {
        var expression = node.Expression;

        // The expression isn't available yet.
        if (expression is null) return null;

        var plainType = expression.PlainType;

        // If the type is unknown it isn't known whether it is invocable or not.
        // If it is a function type, it will get rewritten to an invocation by something else.
        if (plainType is UnknownPlainType or FunctionPlainType) return null;

        return INonInvocableInvocationExpressionNode.Create(node.Syntax, node.CurrentExpression, node.CurrentArguments);
    }

    public static partial void UnresolvedInvocationExpression_Contribute_Diagnostics(IUnresolvedInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Expression)
        {
            case IFunctionNameExpressionNode function:
                ContributeFunctionBindingDiagnostics(function.ReferencedDeclaration,
                    function.CompatibleCallCandidates, node.File, node.Syntax, diagnostics);
                break;
            case IUnresolvedExpressionNode:
                // These presumably report their own errors and should be ignored here
                break;
            // TODO other cases?
            default:
                diagnostics.Add(NameBindingError.NotImplemented(node.File, node.Syntax.Span,
                    "Invocation not properly bound. Unknown call."));
                break;
        }
    }

    private static void ContributeFunctionBindingDiagnostics(
        IFunctionInvocableDeclarationNode? referencedDeclaration,
        IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> compatibleCallCandidates,
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

    public static partial IMaybePlainType? FunctionInvocationExpression_Function_ExpectedPlainType(IFunctionInvocationExpressionNode node)
        => InvocationTargetExpectedPlainType(node.Arguments);

    public static partial IMaybePlainType? MethodInvocationExpression_Method_ExpectedPlainType(IMethodInvocationExpressionNode node)
        => InvocationTargetExpectedPlainType(node.Arguments);

    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> GetterInvocationExpression_CallCandidates(IGetterInvocationExpressionNode node)
        => node.ReferencedDeclarations.Select(p => CallCandidate.Create(node.Context.PlainType, p)).ToFixedSet();

    public static partial IFixedSet<ICallCandidate<IGetterMethodDeclarationNode>> GetterInvocationExpression_CompatibleCallCandidates(IGetterInvocationExpressionNode node)
    {
        var argumentPlainTypes = ArgumentPlainTypes.ForMethod(node.Context.PlainType, []);
        return node.CallCandidates.OfType<ICallCandidate<IGetterMethodDeclarationNode>>()
                   .Where(c => c.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial IFixedSet<ICallCandidate<IPropertyAccessorDeclarationNode>> SetterInvocationExpression_CallCandidates(ISetterInvocationExpressionNode node)
        => node.ReferencedDeclarations.Select(p => CallCandidate.Create(node.Context.PlainType, p)).ToFixedSet();

    public static partial IFixedSet<ICallCandidate<ISetterMethodDeclarationNode>> SetterInvocationExpression_CompatibleCallCandidates(ISetterInvocationExpressionNode node)
    {
        // TODO setters should have method names just like others so you can pass them as function references (e.g. `context.setter.set`)
        var expectedPlainType = node.ExpectedPlainType; // Avoids repeated access
        if (expectedPlainType is null or UnknownPlainType)
            return node.CallCandidates.OfType<ICallCandidate<ISetterMethodDeclarationNode>>().ToFixedSet();
        if (expectedPlainType is not FunctionPlainType { Parameters: var expectedParameters }) return [];
        var argumentPlainTypes = ArgumentPlainTypes.ForMethod(node.Context.PlainType, expectedParameters);
        return node.CallCandidates.OfType<ICallCandidate<ISetterMethodDeclarationNode>>()
                   .Where(c => c.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial IMaybePlainType? InitializerInvocationExpression_Initializer_ExpectedPlainType(IInitializerInvocationExpressionNode node)
        => InvocationTargetExpectedPlainType(node.Arguments);
    #endregion

    #region Name Expressions
    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionNameExpression_CallCandidates(IFunctionNameExpressionNode node)
        => node.ReferencedDeclarations.Select(CallCandidate.Create).ToFixedSet();

    public static partial IFixedSet<ICallCandidate<IFunctionInvocableDeclarationNode>> FunctionNameExpression_CompatibleCallCandidates(IFunctionNameExpressionNode node)
    {
        if (node.ExpectedPlainType is null or UnknownPlainType) return node.CallCandidates;
        if (node.ExpectedPlainType is not FunctionPlainType expectedPlainType) return [];

        var argumentPlainTypes = ArgumentPlainTypes.ForFunction(expectedPlainType.Parameters);
        return node.CallCandidates.Where(o => o.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial ICallCandidate<IFunctionInvocableDeclarationNode>? FunctionNameExpression_SelectedCallCandidate(IFunctionNameExpressionNode node)
        => node.CompatibleCallCandidates.TrySingle();

    public static partial void FunctionNameExpression_Contribute_Diagnostics(
        IFunctionNameExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnresolvedInvocationExpressionNode)
            return;

        switch (node.CompatibleCallCandidates.Count)
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

    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerNameExpression_CallCandidates(IInitializerNameExpressionNode node)
    {
        var initializingPlainType = node.InitializingPlainType;
        return node.ReferencedDeclarations.Select(i => CallCandidate.Create(initializingPlainType, i)).ToFixedSet();
    }

    public static partial IFixedSet<ICallCandidate<IInitializerDeclarationNode>> InitializerNameExpression_CompatibleCallCandidates(IInitializerNameExpressionNode node)
    {
        if (node.ExpectedPlainType is null or UnknownPlainType) return node.CallCandidates;
        if (node.ExpectedPlainType is not FunctionPlainType expectedPlainType) return [];
        var argumentPlainTypes = ArgumentPlainTypes.ForInitializer(expectedPlainType.Parameters);
        return node.CallCandidates.Where(o => o.CompatibleWith(argumentPlainTypes)).ToFixedSet();
    }

    public static partial ICallCandidate<IInitializerDeclarationNode>? InitializerNameExpression_SelectedCallCandidate(IInitializerNameExpressionNode node)
        => node.CompatibleCallCandidates.TrySingle();

    public static partial void InitializerNameExpression_Contribute_Diagnostics(IInitializerNameExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Parent is IUnresolvedInvocationExpressionNode)
            // errors will be reported by the parent in this case
            return;

        switch (node.InitializingPlainType)
        {
            case UnknownPlainType:
                // Error should be reported elsewhere
                return;
            case NeverPlainType:
            case GenericParameterPlainType:
            case BarePlainType { TypeConstructor.CanBeInstantiated: false }:
                // TODO type variables, empty types and others also cannot be constructed. Report proper error message in that case
                diagnostics.Add(OtherSemanticError.CannotInitializeAbstractType(node.File, node.Syntax));
                return;
        }

        if (node.ReferencedDeclaration is not null)
            return;

        switch (node.CompatibleCallCandidates.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindInitializer(node.File, node.Syntax.Span));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                diagnostics.Add(NameBindingError.AmbiguousInitializerCall(node.File, node.Syntax.Span));
                break;
        }
    }
    #endregion
}
