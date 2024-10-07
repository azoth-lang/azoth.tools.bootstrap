using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

// TODO there are a lot of places where binding errors need to be reported. It seems like that should be consolidated
internal static partial class OverloadResolutionAspect
{
    private static IMaybeExpressionAntetype AntetypeIfKnown(IExpressionNode? node)
    {
        if (node is not null && !node.ShouldNotBeExpression()) return node.Antetype;
        return IAntetype.Unknown;
    }

    #region Expressions
    public static partial IFixedSet<IConstructorDeclarationNode> NewObjectExpression_CompatibleConstructors(
        INewObjectExpressionNode node)
    {
        var constructingAntetype = node.ConstructingAntetype;
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForConstructor(arguments);
        return node.ReferencedConstructors
                   .Select(d => CallCandidate.Create(constructingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IConstructorDeclarationNode? NewObjectExpression_ReferencedConstructor(INewObjectExpressionNode node)
        => node.CompatibleConstructors.TrySingle();

    public static partial void NewObjectExpression_Contribute_Diagnostics(
        INewObjectExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.ConstructingAntetype)
        {
            case UnknownAntetype:
                // Error should be reported elsewhere
                return;
            case NominalAntetype { DeclaredAntetype.IsAbstract: true }:
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
    public static partial IMaybeAntetype? UnknownInvocationExpression_Expression_ExpectedAntetype(IUnknownInvocationExpressionNode node)
    {
        var expectedReturnAntetype = node.ExpectedAntetype?.ToNonConstValueType() ?? IAntetype.Unknown;
        return new FunctionAntetype(node.Arguments.Select(NonVoidAntetypeIfKnown),
            // TODO this is odd, but the return antetype will be ignored
            NonVoidAntetypeIfKnown(expectedReturnAntetype));
    }

    private static INonVoidAntetype NonVoidAntetypeIfKnown(IExpressionNode? node)
        => NonVoidAntetypeIfKnown(AntetypeIfKnown(node));

    private static INonVoidAntetype NonVoidAntetypeIfKnown(IMaybeExpressionAntetype maybeExpressionAntetype)
    {
        if (maybeExpressionAntetype is INonVoidAntetype antetype) return antetype;
        // This is a little odd, but if the parameter type is not known, then using `never` will
        // cause nothing to match except for `never` itself.
        return IAntetype.Never;
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
        if (node.Expression is not { Antetype: FunctionAntetype } expression)
            return null;

        return IFunctionReferenceInvocationExpressionNode.Create(node.Syntax, expression, node.CurrentArguments);
    }

    public static partial void UnknownInvocationExpression_Contribute_Diagnostics(IUnknownInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node.Expression)
        {
            case IFunctionGroupNameNode functionGroup:
                ContributeFunctionBindingDiagnostics(functionGroup.ReferencedDeclaration,
                    functionGroup.CompatibleDeclarations, node.File, node.Syntax, diagnostics);
                break;
            case IFunctionNameNode function:
                ContributeFunctionBindingDiagnostics(function.ReferencedDeclaration,
                    function.CompatibleDeclarations, node.File, node.Syntax, diagnostics);
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
        IFixedSet<IFunctionInvocableDeclarationNode> compatibleDeclarations,
        CodeFile file,
        IInvocationExpressionSyntax syntax,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (referencedDeclaration is not null) return;

        switch (compatibleDeclarations.Count)
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

        switch (method.CompatibleDeclarations.Count)
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
        var initializingAntetype = node.InitializerGroup.InitializingAntetype;
        var arguments = node.Arguments.Select(AntetypeIfKnown);
        var argumentAntetypes = ArgumentAntetypes.ForInitializer(arguments);
        return node.InitializerGroup.ReferencedDeclarations
                   .Select(d => CallCandidate.Create(initializingAntetype, d))
                   .Where(o => o.CompatibleWith(argumentAntetypes))
                   .Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IInitializerDeclarationNode? InitializerInvocationExpression_ReferencedDeclaration(IInitializerInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();
    #endregion

    #region Name Expressions
    public static partial IFixedSet<IFunctionInvocableDeclarationNode> FunctionGroupName_CompatibleDeclarations(IFunctionGroupNameNode node)
    {
        if (node.ExpectedAntetype is not FunctionAntetype expectedAntetype) return [];

        var argumentAntetypes = ArgumentAntetypes.ForFunction(expectedAntetype.Parameters);
        return node.ReferencedDeclarations.Select(CallCandidate.Create)
                   .Where(o => o.CompatibleWith(argumentAntetypes)).Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IFunctionInvocableDeclarationNode? FunctionGroupName_ReferencedDeclaration(IFunctionGroupNameNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static partial void FunctionGroupName_Contribute_Diagnostics(IFunctionGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
        => ContributeFunctionNameBindingDiagnostics(node.ReferencedDeclaration, node.CompatibleDeclarations, node, diagnostics);

    private static void ContributeFunctionNameBindingDiagnostics(
        IFunctionInvocableDeclarationNode? referencedDeclaration,
        IFixedSet<IFunctionInvocableDeclarationNode> compatibleDeclarations,
        INameExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (referencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnknownInvocationExpressionNode)
            return;

        switch (compatibleDeclarations.Count)
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
        => ContributeFunctionNameBindingDiagnostics(node.ReferencedDeclaration, node.CompatibleDeclarations, node, diagnostics);

    public static partial IFixedSet<IStandardMethodDeclarationNode> MethodGroupName_CompatibleDeclarations(IMethodGroupNameNode node)
    {
        if (node.ExpectedAntetype is not FunctionAntetype expectedAntetype) return [];

        var contextAntetype = node.Context.Antetype;
        var argumentAntetypes = ArgumentAntetypes.ForMethod(contextAntetype, expectedAntetype.Parameters);
        return node.ReferencedDeclarations.Select(m => CallCandidate.Create(contextAntetype, m))
                   .Where(o => o.CompatibleWith(argumentAntetypes)).Select(o => o.Declaration).ToFixedSet();
    }

    public static partial IStandardMethodDeclarationNode? MethodGroupName_ReferencedDeclaration(IMethodGroupNameNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static partial void MethodGroupName_Contribute_Diagnostics(IMethodGroupNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ReferencedDeclaration is not null
            // errors will be reported by the parent in this case
            || node.Parent is IUnknownInvocationExpressionNode)
            return;

        switch (node.CompatibleDeclarations.Count)
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
