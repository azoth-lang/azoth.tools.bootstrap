using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ExpressionTypesAspect
{
    public static partial void Expression_Contribute_Diagnostics(IExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ExpectedType is not DataType expectedType)
            return;

        if (!expectedType.IsAssignableFrom(node.Type))
            diagnostics.Add(TypeError.CannotImplicitlyConvert(node.File, node.Syntax, node.Type, expectedType));
    }

    public static partial DataType IdExpression_Type(IIdExpressionNode node)
    {
        var referentType = node.Referent?.Type ?? DataType.Unknown;
        if (referentType is CapabilityType capabilityType)
            return capabilityType.With(Capability.Identity);
        return DataType.Unknown;
    }

    public static partial IFlowState IdExpression_FlowStateAfter(IIdExpressionNode node)
    {
        var intermediateReferent = node.Referent;
        if (intermediateReferent is null)
            return IFlowState.Empty;
        return intermediateReferent.FlowStateAfter.Transform(intermediateReferent.ValueId, node.ValueId, node.Type);
    }

    public static partial void IdExpression_Contribute_Diagnostics(IIdExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is not UnknownType)
            return;

        var referentType = node.Referent?.Type
                           ?? throw new UnreachableException("Final referent should already be assigned");
        if (referentType is not CapabilityType)
            diagnostics.Add(TypeError.CannotIdNonReferenceType(node.File, node.Syntax.Span, referentType));
    }

    public static partial DataType VariableNameExpression_Type(IVariableNameExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDefinition);

    public static partial IFlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDefinition, node.ValueId);

    public static partial IFlowState NamedParameter_FlowStateAfter(INamedParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static partial IFlowState SelfParameter_FlowStateAfter(ISelfParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static partial DataType UnsafeExpression_Type(IUnsafeExpressionNode node)
        => node.Expression?.Type ?? DataType.Unknown;

    public static partial IFlowState UnsafeExpression_FlowStateAfter(IUnsafeExpressionNode node)
        => node.Expression?.FlowStateAfter.Transform(node.Expression.ValueId, node.ValueId, node.Type)
           ?? IFlowState.Empty;

    public static partial DataType FunctionInvocationExpression_Type(IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration?.Type.Return.Type ?? DataType.UnknownDataType;

    public static partial ContextualizedOverload? FunctionInvocationExpression_ContextualizedOverload(
        IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.ReferencedDeclaration)
            : null;

    public static partial IFlowState FunctionInvocationExpression_FlowStateAfter(IFunctionInvocationExpressionNode node)
    {
        // The flow state just before the function is called is the state after all arguments have evaluated
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.Arguments);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void FunctionInvocationExpression_Contribute_Diagnostics(IFunctionInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.Arguments);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    private static void ContributeCannotUnionDiagnostics(
        IInvocationExpressionNode node,
        IFlowState flowStateBefore,
        IEnumerable<ArgumentValueId> argumentValueIds,
        DiagnosticCollectionBuilder diagnostics)
    {
        var valueIds = flowStateBefore.CombineArgumentsDisallowedDueToLent(argumentValueIds);
        foreach (var valueId in valueIds)
        {
            var arg = node.AllArguments.Single(a => a?.ValueId == valueId)!;
            diagnostics.Add(FlowTypingError.CannotUnion(arg.File, arg.Syntax.Span));
        }
    }

    public static partial FunctionType FunctionReferenceInvocationExpression_FunctionType(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionType)node.Expression.Type;

    public static partial DataType FunctionReferenceInvocationExpression_Type(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionType.Return.Type;

    public static partial IFlowState FunctionReferenceInvocationExpression_FlowStateAfter(IFunctionReferenceInvocationExpressionNode node)
    {
        // The flow state just before the function is called is the state after all arguments have evaluated
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.Expression.FlowStateAfter;
        // TODO handle the fact that the function reference itself must be combined too
        var contextualizedOverload = ContextualizedOverload.Create(node.FunctionType);
        var argumentValueIds = ArgumentValueIds(contextualizedOverload, null, node.Arguments);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void FunctionReferenceInvocationExpression_Contribute_Diagnostics(IFunctionReferenceInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.Expression.FlowStateAfter;
        var contextualizedOverload = ContextualizedOverload.Create(node.FunctionType);
        var argumentValueIds = ArgumentValueIds(contextualizedOverload, null, node.Arguments);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    public static partial BoolConstValueType BoolLiteralExpression_Type(IBoolLiteralExpressionNode node)
        => node.Value ? DataType.True : DataType.False;

    public static partial IntegerConstValueType IntegerLiteralExpression_Type(IIntegerLiteralExpressionNode node)
        => new IntegerConstValueType(node.Value);

    public static partial OptionalType NoneLiteralExpression_Type(INoneLiteralExpressionNode node)
        => DataType.None;

    public static partial DataType StringLiteralExpression_Type(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName).OfType<ITypeDeclarationNode>().TrySingle();
        return typeSymbolNode?.Symbol.GetDeclaredType()?.With(Capability.Constant, []) ?? DataType.UnknownDataType;
    }

    public static partial IFlowState LiteralExpression_FlowStateAfter(ILiteralExpressionNode node)
        => node.FlowStateBefore().Constant(node.ValueId);

    public static partial void StringLiteralExpression_Contribute_Diagnostics(
        IStringLiteralExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is UnknownType)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Could not find string type for string literal."));
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static partial ContextualizedOverload? MethodInvocationExpression_ContextualizedOverload(
        IMethodInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.MethodGroup.Context.Type, node.ReferencedDeclaration)
            : null;

    public static partial IExpressionNode? Expression_Rewrite_ImplicitMove(IExpressionNode node)
    {
        if (!node.ImplicitRecoveryAllowed())
            return null;

        var expectedType = node.ExpectedType;
        if (expectedType is not CapabilityType { Capability: var expectedCapability }
            || (expectedCapability != Capability.Isolated && expectedCapability != Capability.TemporarilyIsolated))
            return null;

        var isTemporary = expectedCapability == Capability.TemporarilyIsolated;

        var type = node.Type.ToNonConstValueType();
        if (type is CapabilityType { Capability: var capability } && capability == expectedCapability)
            return null;

        // TODO what if selfType is not a capability type?

        var syntax = node.Syntax;
        var implicitMove = isTemporary
            ? new ImplicitTempMoveExpressionNode(syntax, node)
            : (IExpressionNode)(node is IVariableNameExpressionNode variableName
                ? IMoveVariableExpressionNode.Create(syntax, variableName, isImplicit: true)
                : IMoveValueExpressionNode.Create(syntax, node, isImplicit: true));
        return implicitMove;
    }

    public static partial IExpressionNode? Expression_Rewrite_ImplicitFreeze(IExpressionNode node)
    {
        if (!node.ImplicitRecoveryAllowed())
            return null;

        var expectedType = node.ExpectedType;
        if (expectedType is not CapabilityType { Capability: var expectedCapability }
            || (expectedCapability != Capability.Constant && expectedCapability != Capability.TemporarilyConstant))
            return null;

        var isTemporary = expectedCapability == Capability.TemporarilyConstant;

        var type = node.Type.ToNonConstValueType();
        if (type is CapabilityType { Capability: var capability } && capability == expectedCapability)
            return null;

        // TODO what if type is not a capability type?

        var syntax = node.Syntax;
        IFreezeExpressionNode implicitFreeze = node is IVariableNameExpressionNode variableName
            ? new FreezeVariableExpressionNode(syntax, variableName, isTemporary, isImplicit: true)
            : new FreezeValueExpressionNode(syntax, node, isTemporary, isImplicit: true);
        return implicitFreeze;
    }

    public static partial IExpressionNode? Expression_Rewrite_PrepareToReturn(IExpressionNode node)
    {
        if (node is IRecoveryExpressionNode { IsImplicit: true } or IPrepareToReturnExpressionNode
            || !node.ShouldPrepareToReturn())
            return null;

        return new PrepareToReturnExpressionNode(node);
    }

    public static partial DataType MethodInvocationExpression_Type(IMethodInvocationExpressionNode node)
    {
        var selfType = node.MethodGroup.Context.Type;
        // TODO does this need to be modified by flow typing?
        var unboundType = node.ContextualizedOverload?.ReturnType.Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static partial IFlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node)
    {
        // The flow state just before the method is called is the state after all arguments have evaluated
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.MethodGroup.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.MethodGroup.Context, node.Arguments);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void MethodInvocationExpression_Contribute_Diagnostics(IMethodInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.MethodGroup.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.MethodGroup.Context, node.Arguments);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    public static partial ContextualizedOverload? GetterInvocationExpression_ContextualizedOverload(IGetterInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.Context.Type, node.ReferencedDeclaration)
            : null;

    public static partial DataType GetterInvocationExpression_Type(IGetterInvocationExpressionNode node)
    {
        var selfType = node.Context.Type;
        var unboundType = node.ContextualizedOverload?.ReturnType.Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static partial IFlowState GetterInvocationExpression_FlowStateAfter(IGetterInvocationExpressionNode node)
    {
        var flowStateBefore = node.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.Context, []);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void GetterInvocationExpression_Contribute_Diagnostics(IGetterInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var flowStateBefore = node.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.Context, []);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    public static partial ContextualizedOverload? SetterInvocationExpression_ContextualizedOverload(ISetterInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.Context.Type, node.ReferencedDeclaration)
            : null;

    public static partial DataType SetterInvocationExpression_Type(ISetterInvocationExpressionNode node)
    {
        var selfType = node.Context.Type;
        var unboundType = node.ContextualizedOverload?.ParameterTypes[0].Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static partial IFlowState SetterInvocationExpression_FlowStateAfter(ISetterInvocationExpressionNode node)
    {
        if (node.Value is not IExpressionNode value)
            return IFlowState.Empty;
        // The flow state just before the setter is called is the state after the argument has been evaluated
        var flowStateBefore = value.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.Context, [value]);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void SetterInvocationExpression_Contribute_Diagnostics(ISetterInvocationExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var value = node.Value!;
        var flowStateBefore = value.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.Context, [value]);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    private static IEnumerable<ArgumentValueId> ArgumentValueIds(
        ContextualizedOverload? overload,
        IExpressionNode? selfArgument,
        IEnumerable<IExpressionNode?> arguments)
    {
        var allArguments = arguments.Prepend(selfArgument).WhereNotNull();
        if (overload is null)
            return allArguments.Select(a => new ArgumentValueId(false, a.ValueId));

        var parameterTypes = overload.ParameterTypes.AsEnumerable();
        if (selfArgument is not null)
        {
            if (overload.SelfParameterType is not SelfParameterType selfParameterType)
                throw new InvalidOperationException("Self argument provided for overload without self parameter");
            parameterTypes = parameterTypes.Prepend(selfParameterType.ToUpperBound());
        }
        return parameterTypes.EquiZip(allArguments)
                             .Select((p, a) => new ArgumentValueId(p.IsLent, a.ValueId));
    }

    public static partial DataType FieldAccessExpression_Type(IFieldAccessExpressionNode node)
    {
        var contextType = node.Context is ISelfExpressionNode selfNode
            ? selfNode.Pseudotype : node.Context.Type;
        var fieldType = node.ReferencedDeclaration.BindingType;
        // Access must be applied first, so it can account for independent generic parameters.
        var type = fieldType.AccessedVia(contextType);
        // Then type parameters can be replaced now that they have the correct access
        if (contextType is NonEmptyType nonEmptyContext)
            // resolve generic type fields
            type = nonEmptyContext.ReplaceTypeParametersIn(type);

        return type;
    }

    public static partial IFlowState FieldAccessExpression_FlowStateAfter(IFieldAccessExpressionNode node)
        => node.Context.FlowStateAfter.AccessField(node);

    public static partial void FieldAccessExpression_Contribute_Diagnostics(IFieldAccessExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Parent is IAssignmentExpressionNode assignmentNode && assignmentNode.TempLeftOperand == node)
            // In this case, a different error will be reported and CannotAccessMutableBindingFieldOfIdentityReference
            // should not be reported.
            return;

        var fieldHasMutableBinding = node.ReferencedDeclaration.Symbol.IsMutableBinding;
        if (fieldHasMutableBinding
            && node.Context.Type is CapabilityType { Capability: var contextCapability }
            && contextCapability == Capability.Identity)
            diagnostics.Add(TypeError.CannotAccessMutableBindingFieldOfIdentityReference(node.File, node.Syntax, node.Context.Type));
    }

    public static partial IFlowState SelfExpression_FlowStateAfter(ISelfExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDefinition, node.ValueId);

    public static partial DataType SelfExpression_Type(ISelfExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDefinition);

    public static partial Pseudotype SelfExpression_Pseudotype(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingType ?? DataType.Unknown;

    public static partial ContextualizedOverload? NewObjectExpression_ContextualizedOverload(
        INewObjectExpressionNode node)
        => node.ReferencedConstructor is not null
            ? ContextualizedOverload.Create(node.ConstructingType.NamedType, node.ReferencedConstructor)
            : null;

    public static partial DataType NewObjectExpression_Type(INewObjectExpressionNode node)
        // TODO does this need to be modified by flow typing?
        => node.ContextualizedOverload?.ReturnType.Type ?? DataType.UnknownDataType;

    public static partial IFlowState NewObjectExpression_FlowStateAfter(INewObjectExpressionNode node)
    {
        // The flow state just before the constructor is called is the state after all arguments have evaluated
        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.Arguments);
        return flowStateBefore.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial void NewObjectExpression_Contribute_Diagnostics(INewObjectExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckConstructingType(node.ConstructingType, diagnostics);

        var flowStateBefore = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.Arguments);
        ContributeCannotUnionDiagnostics(node, flowStateBefore, argumentValueIds, diagnostics);
    }

    private static void CheckConstructingType(ITypeNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IStandardTypeNameNode n:
                CheckTypeArgumentsAreConstructable(n, diagnostics);
                break;
            case ISpecialTypeNameNode n:
                diagnostics.Add(TypeError.SpecialTypeCannotBeUsedHere(node.File, n.Syntax));
                break;
            case IQualifiedTypeNameNode n:
                diagnostics.Add(TypeError.TypeParameterCannotBeUsedHere(node.File, n.Syntax));
                break;
        }
    }

    public static void CheckTypeArgumentsAreConstructable(IStandardTypeNameNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var bareType = node.NamedBareType;
        if (bareType is null) return;

        foreach (GenericParameterArgument arg in bareType.GenericParameterArguments)
            if (!arg.IsConstructable())
                diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(node.File, node.Syntax, arg.Parameter,
                    arg.Argument));
    }

    public static partial ContextualizedOverload? InitializerInvocationExpression_ContextualizedOverload(IInitializerInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
           && node.InitializerGroup.Context.NamedBareType is not null and var initializingType
            ? ContextualizedOverload.Create(initializingType.With(Capability.Mutable), node.ReferencedDeclaration)
            : null;

    public static partial DataType InitializerInvocationExpression_Type(IInitializerInvocationExpressionNode node)
        // TODO does this need to be modified by flow typing?
        => node.ContextualizedOverload?.ReturnType.Type ?? DataType.UnknownDataType;

    public static partial IFlowState InitializerInvocationExpression_FlowStateAfter(IInitializerInvocationExpressionNode node)
    {
        // The flow state just before the initializer is called is the state after all arguments have evaluated
        var flowState = node.Arguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.Arguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static partial DataType AssignmentExpression_Type(IAssignmentExpressionNode node)
        => node.LeftOperand?.Type ?? DataType.Unknown;

    public static partial IFlowState AssignmentExpression_FlowStateAfter(IAssignmentExpressionNode node)
    {
        // TODO this isn't quite right since the original value is replaced by the new value
        if (node.LeftOperand?.ValueId is not ValueId leftValueId)
            return IFlowState.Empty;
        return node.RightOperand?.FlowStateAfter.Combine(leftValueId,
            node.RightOperand.ValueId, node.ValueId) ?? IFlowState.Empty;
    }

    public static partial void AssignmentExpression_Contribute_Diagnostics(IAssignmentExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.LeftOperand is IFieldAccessExpressionNode fieldAccess)
        {
            var contextType = fieldAccess.Context.Type;
            if (contextType is CapabilityType { AllowsWrite: false, AllowsInit: false } capabilityType)
                diagnostics.Add(TypeError.CannotAssignFieldOfReadOnly(node.File, node.Syntax.Span, capabilityType));

            // Check for assigning into `let` fields (skip self fields in constructors and initializers)
            if (contextType is not CapabilityType { AllowsInit: true } && fieldAccess.ReferencedDeclaration.Symbol is
                { IsMutableBinding: false, Name: IdentifierName name })
                diagnostics.Add(OtherSemanticError.CannotAssignImmutableField(node.File, node.Syntax.Span, name));
        }

        if (node is { LeftOperand: { } leftOperand, RightOperand: { } rightOperand })
        {
            var flowStateBefore = rightOperand.FlowStateAfter;
            var leftValueId = leftOperand.ValueId;
            var rightValueId = rightOperand.ValueId;
            var valueIds = flowStateBefore.CombineDisallowedDueToLent(rightValueId, leftValueId);
            foreach (var valueId in valueIds)
            {
                var operand = valueId == leftValueId ? leftOperand : rightOperand;
                diagnostics.Add(FlowTypingError.CannotUnion(operand.File, operand.Syntax.Span));
            }
        }
    }

    public static partial DataType BinaryOperatorExpression_Type(IBinaryOperatorExpressionNode node)
    {
        if (node.Antetype is ISimpleOrConstValueAntetype simpleOrConstValueAntetype)
            return simpleOrConstValueAntetype.ToType();
        if (node.Antetype is UnknownAntetype)
            return DataType.Unknown;

        var leftType = node.LeftOperand?.Type ?? DataType.Unknown;
        var rightType = node.RightOperand?.Type ?? DataType.Unknown;
        return (leftType, node.Operator, rightType) switch
        {
            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                => InferRangeOperatorType(node.ContainingLexicalScope, leftType, rightType),

            (OptionalType { Referent: var referentType }, BinaryOperator.QuestionQuestion, NeverType)
                => referentType,

            _ => DataType.Unknown

            // TODO optional types
        };
    }

    private static DataType InferRangeOperatorType(
        LexicalScope containingLexicalScope,
        DataType leftType,
        DataType rightType)
    {
        // TODO the left and right types need to be compatible with the range type
        var rangeTypeDeclaration = containingLexicalScope.Lookup("azoth")
            .OfType<INamespaceDeclarationNode>().SelectMany(ns => ns.MembersNamed("range"))
            .OfType<ITypeDeclarationNode>().TrySingle();
        var rangeAntetype = rangeTypeDeclaration?.Symbol.GetDeclaredType()?.With(Capability.Constant, [])
                            ?? DataType.UnknownDataType;
        return rangeAntetype;
    }

    public static partial IFlowState BinaryOperatorExpression_FlowStateAfter(IBinaryOperatorExpressionNode node)
        // Left and right are swapped here because right is known to not be null while left may be null and order doesn't matter to combine
        => node.RightOperand?.FlowStateAfter.Combine(
            node.RightOperand.ValueId, node.LeftOperand?.ValueId, node.ValueId)
           ?? IFlowState.Empty;

    public static partial void BinaryOperatorExpression_Contribute_Diagnostics(IBinaryOperatorExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type == DataType.Unknown)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(node.File,
                node.Syntax.Span, node.Operator, node.LeftOperand!.Type, node.RightOperand!.Type));
    }

    public static partial DataType IfExpression_Type(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.Type.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Type;
    }

    public static partial DataType ResultStatement_Type(IResultStatementNode node)
        => node.Expression?.Type.ToNonConstValueType() ?? DataType.Unknown;

    public static partial IFlowState IfExpression_FlowStateAfter(IIfExpressionNode node)
    {
        var thenPath = node.ThenBlock.FlowStateAfter;
        var elsePath = node.ElseClause?.FlowStateAfter ?? node.Condition?.FlowStateAfter ?? IFlowState.Empty;
        var flowStateBefore = thenPath.Merge(elsePath);
        return flowStateBefore.Combine(node.ThenBlock.ValueId, node.ElseClause?.ValueId, node.ValueId);
    }

    public static partial void IfExpression_Contribute_Diagnostics(IIfExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node is { ThenBlock: { } thenBlock, ElseClause: { } elseClause })
        {
            var flowStateBefore = thenBlock.FlowStateAfter.Merge(elseClause.FlowStateAfter);
            var thenValueId = thenBlock.ValueId;
            var elseValueId = elseClause.ValueId;
            var valueIds = flowStateBefore.CombineDisallowedDueToLent(elseValueId, thenValueId);
            foreach (var valueId in valueIds)
            {
                var branch = valueId == thenValueId ? thenBlock : elseClause;
                diagnostics.Add(FlowTypingError.CannotUnion(branch.File, branch.Syntax.Span));
            }
        }
    }

    public static partial DataType BlockExpression_Type(IBlockExpressionNode node)
    {
        // TODO what about blocks that contain a return etc. and never return?
        foreach (var statement in node.Statements)
            if (statement.ResultType is not null and var resultType)
                return resultType;

        // If there was no result expression, then the block type is void
        return DataType.Void;
    }

    public static partial IFlowState BlockExpression_FlowStateAfter(IBlockExpressionNode node)
    {
        var flowState = node.Statements.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        flowState = flowState.DropBindings(node.Statements.OfType<IVariableDeclarationStatementNode>());
        foreach (var statement in node.Statements)
            if (statement.ResultValueId is ValueId resultValueId)
                return flowState.Transform(resultValueId, node.ValueId, node.Type);

        return flowState.Constant(node.ValueId);
    }

    public static partial DataType WhileExpression_Type(IWhileExpressionNode node)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static partial IFlowState WhileExpression_FlowStateAfter(IWhileExpressionNode node)
        // TODO loop flow state
        // Merge condition with block flow state because the body may not be executed
        => (node.Condition?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `while` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);

    public static partial DataType LoopExpression_Type(ILoopExpressionNode node)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static partial IFlowState LoopExpression_FlowStateAfter(ILoopExpressionNode node)
        // Body is always executes at least once
        // TODO loop flow state
        => node.Block.FlowStateAfter
               // TODO when the `loop` has a type other than void, correctly handle the value id
               .Constant(node.ValueId);

    public static partial DataType ConversionExpression_Type(IConversionExpressionNode node)
    {
        var convertToType = node.ConvertToType.NamedType;
        if (node.Operator == ConversionOperator.Optional)
            convertToType = convertToType.MakeOptional();
        return convertToType;
    }

    public static partial IFlowState ConversionExpression_FlowStateAfter(IConversionExpressionNode node)
    {
        var intermediateReferent = node.Referent;
        if (intermediateReferent is null)
            return IFlowState.Empty;
        return intermediateReferent.FlowStateAfter
            .Transform(node.Referent?.ValueId, node.ValueId, node.Type);
    }

    public static partial void ConversionExpression_Contribute_Diagnostics(IConversionExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var convertFromType = node.Referent!.Type;
        var convertToType = node.ConvertToType.NamedType;
        if (!convertFromType.CanBeExplicitlyConvertedTo(convertToType, node.Operator == ConversionOperator.Safe))
            diagnostics.Add(TypeError.CannotExplicitlyConvert(node.File, node.Referent.Syntax, convertFromType, convertToType));
    }

    public static partial Type ImplicitConversionExpression_Type(IImplicitConversionExpressionNode node)
        => node.Antetype.ToType();

    public static partial IFlowState ImplicitConversionExpression_FlowStateAfter(IImplicitConversionExpressionNode node)
        => node.Referent.FlowStateAfter.Transform(node.Referent.ValueId, node.ValueId, node.Type);

    public static partial DataType AsyncStartExpression_Type(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.Expression?.Type ?? DataType.Unknown);

    public static partial IFlowState AsyncStartExpression_FlowStateAfter(IAsyncStartExpressionNode node)
        // TODO this isn't correct, async start can act like a delayed lambda. It is also a transform that wraps
        => node.Expression?.FlowStateAfter.Combine(node.Expression.ValueId, null, node.ValueId) ?? IFlowState.Empty;

    public static partial DataType AwaitExpression_Type(IAwaitExpressionNode node)
    {
        if (node.Expression?.Type is CapabilityType { DeclaredType: var declaredType } type
            && Intrinsic.PromiseType.Equals(declaredType))
            return type.TypeArguments[0];

        return DataType.Unknown;
    }

    public static partial IFlowState AwaitExpression_FlowStateAfter(IAwaitExpressionNode node)
        // TODO actually this is a transform that unwraps
        => node.Expression?.FlowStateAfter.Combine(node.Expression.ValueId, null, node.ValueId) ?? IFlowState.Empty;

    public static partial DataType UnaryOperatorExpression_Type(IUnaryOperatorExpressionNode node)
        => node.Antetype switch
        {
            ISimpleOrConstValueAntetype t => t.ToType(),
            UnknownAntetype => DataType.Unknown,
            _ => throw new InvalidOperationException($"Unexpected antetype {node.Antetype}")
        };

    public static partial IFlowState UnaryOperatorExpression_FlowStateAfter(IUnaryOperatorExpressionNode node)
        => node.Operand?.FlowStateAfter.Transform(node.Operand.ValueId, node.ValueId, node.Type) ?? IFlowState.Empty;

    public static partial DataType FunctionName_Type(IFunctionNameNode node)
        => node.ReferencedDeclaration?.Type ?? DataType.UnknownDataType;

    public static partial IFlowState FunctionName_FlowStateAfter(IFunctionNameNode node)
        => node.FlowStateBefore().Constant(node.ValueId);

    public static partial DataType FreezeExpression_Type(IFreezeExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow freeze, a freeze expression always results in a
        // constant reference. A diagnostic is generated if the capability doesn't allow freeze.

        var capability = node.IsTemporary ? Capability.TemporarilyConstant : Capability.Constant;
        return capabilityType.With(capability);
    }

    public static partial IFlowState FreezeVariableExpression_FlowStateAfter(IFreezeVariableExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        var referentValueId = node.Referent.ValueId;
        return node.IsTemporary
            // TODO this implies that temp freeze is a fundamentally different operation and ought to have its own node type
            ? flowStateBefore.TempFreeze(referentValueId, node.ValueId)
            : flowStateBefore.FreezeVariable(node.Referent.ReferencedDefinition, referentValueId, node.ValueId);
    }

    public static partial IFlowState FreezeValueExpression_FlowStateAfter(IFreezeValueExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        var referentValueId = node.Referent.ValueId;
        return node.IsTemporary
            ? flowStateBefore.TempFreeze(referentValueId, node.ValueId)
            : flowStateBefore.FreezeValue(referentValueId, node.ValueId);
    }

    public static partial void FreezeVariableExpression_Contribute_Diagnostics(IFreezeVariableExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return;

        if (!capabilityType.AllowsFreeze)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow freezing"));
        else if (!node.IsTemporary && !node.Referent.FlowStateAfter.CanFreezeExceptFor(node.Referent.ReferencedDefinition, node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotFreezeValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static partial void FreezeValueExpression_Contribute_Diagnostics(IFreezeValueExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType) return;

        if (!capabilityType.AllowsFreeze)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow freezing"));
        else if (!node.IsTemporary && !node.Referent.FlowStateAfter.CanFreeze(node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotFreezeValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static partial DataType MoveExpression_Type(IMoveExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow move, a move expression always results in an
        // isolated reference. A diagnostic is generated if the capability doesn't allow move.
        // TODO maybe `temp iso` should require `temp move`?
        return capabilityType.IsTemporarilyIsolatedReference ? capabilityType
            : capabilityType.With(Capability.Isolated);
    }

    public static partial IFlowState MoveVariableExpression_FlowStateAfter(IMoveVariableExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.MoveVariable(node.Referent.ReferencedDefinition, node.Referent.ValueId, node.ValueId);
    }

    public static partial void MoveVariableExpression_Contribute_Diagnostics(IMoveVariableExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return;

        if (!capabilityType.AllowsMove)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow moving"));
        else if (!node.Referent.FlowStateAfter.IsIsolatedExceptFor(node.Referent.ReferencedDefinition, node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotMoveValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static partial IFlowState MoveValueExpression_FlowStateAfter(IMoveValueExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.MoveValue(node.Referent.ValueId, node.ValueId);
    }

    public static partial void MoveValueExpression_Contribute_Diagnostics(IMoveValueExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType) return;

        if (!capabilityType.AllowsMove)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow moving"));
        else if (!node.Referent.FlowStateAfter.IsIsolated(node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotMoveValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static partial DataType ImplicitTempMoveExpression_Type(IImplicitTempMoveExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow move, a temp move expression always results in a
        // temp isolated reference. A diagnostic is generated if the capability doesn't allow move.
        return capabilityType.With(Capability.TemporarilyIsolated);
    }

    public static partial IFlowState ImplicitTempMoveExpression_FlowStateAfter(IImplicitTempMoveExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.TempMove(node.Referent.ValueId, node.ValueId);
    }

    public static partial IFlowState ExpressionStatement_FlowStateAfter(IExpressionStatementNode node)
    {
        var intermediateExpression = node.Expression;
        if (intermediateExpression is null)
            return IFlowState.Empty;

        return intermediateExpression.FlowStateAfter.DropValue(intermediateExpression.ValueId);
    }

    public static partial IFlowState ReturnExpression_FlowStateAfter(IReturnExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static partial void ReturnExpression_Contribute_Diagnostics(IReturnExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Value is not { } value)
            return;
        var flowStateBefore = value.FlowStateAfter;
        if (flowStateBefore.IsLent(value.ValueId))
            diagnostics.Add(FlowTypingError.CannotReturnLent(node.File, node.Syntax));
    }

    public static partial IFlowState BreakExpression_FlowStateAfter(IBreakExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static partial IFlowState NextExpression_FlowStateAfter(INextExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static partial IFlowState PatternMatchExpression_FlowStateAfter(IPatternMatchExpressionNode node)
        // Constant for the boolean result of the pattern match
        => node.Pattern.FlowStateAfter.Constant(node.ValueId);

    public static partial IFlowState UnknownMemberAccessExpression_FlowStateAfter(IUnknownMemberAccessExpressionNode node)
        => node.Context.FlowStateAfter.Transform(node.Context.ValueId, node.ValueId, node.Type);

    public static partial IFlowState PrepareToReturnExpression_FlowStateAfter(IPrepareToReturnExpressionNode node)
    {
        var flowStateBefore = node.Value.FlowStateAfter;
        return flowStateBefore.Transform(node.Value.ValueId, node.ValueId, node.Type)
                              .DropBindingsForReturn();
    }
}
