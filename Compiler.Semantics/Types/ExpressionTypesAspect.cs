using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
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

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public static class ExpressionTypesAspect
{
    public static ValueId Expression_ValueId(IExpressionNode node)
        => node.PreviousValueId().CreateNext();

    public static ValueId ForeachExpression_BindingValueId(IForeachExpressionNode node)
        // Since value ids are in preorder, to makes some sense that the expression value id is
        // before the binding value id. However, this is also because it would be hard to change the
        // value id of the expression to depend on the binding value id, but it is easy to do this.
        => node.ValueId.CreateNext();

    public static void NewObjectExpression_ContributeDiagnostics(INewObjectExpressionNode node, Diagnostics diagnostics)
        => CheckConstructingType(node.ConstructingType, diagnostics);

    private static void CheckConstructingType(ITypeNameNode node, Diagnostics diagnostics)
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

    public static void CheckTypeArgumentsAreConstructable(
        IStandardTypeNameNode node,
        Diagnostics diagnostics)
    {
        var bareType = node.NamedBareType;
        if (bareType is null) return;

        foreach (GenericParameterArgument arg in bareType.GenericParameterArguments)
            if (!arg.IsConstructable())
                diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(node.File, node.Syntax,
                    arg.Parameter, arg.Argument));
    }

    public static DataType IdExpression_Type(IIdExpressionNode node)
    {
        var referentType = node.IntermediateReferent?.Type ?? DataType.Unknown;
        if (referentType is CapabilityType capabilityType)
            return capabilityType.With(Capability.Identity);
        return DataType.Unknown;
    }

    public static IFlowState IdExpression_FlowStateAfter(IIdExpressionNode node)
    {
        var intermediateReferent = node.IntermediateReferent;
        if (intermediateReferent is null)
            return IFlowState.Empty;
        return intermediateReferent.FlowStateAfter.Transform(intermediateReferent.ValueId, node.ValueId, node.Type);
    }

    public static void IdExpression_ContributeDiagnostics(IIdExpressionNode node, Diagnostics diagnostics)
    {
        if (node.Type is not UnknownType)
            return;

        var referentType = node.IntermediateReferent?.Type
                           ?? throw new UnreachableException("Final referent should already be assigned");
        if (referentType is not CapabilityType)
            diagnostics.Add(TypeError.CannotIdNonReferenceType(node.File, node.Syntax.Span, referentType));
    }

    public static DataType VariableNameExpression_Type(IVariableNameExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDefinition);

    public static IFlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDefinition, node.ValueId);

    public static IFlowState NamedParameter_FlowStateAfter(INamedParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static IFlowState SelfParameter_FlowStateAfter(ISelfParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static ValueId VariableDeclarationStatement_BindingValueId(IVariableDeclarationStatementNode node)
        => node.PreviousValueId().CreateNext();

    public static ValueId BindingPattern_BindingValueId(IBindingPatternNode node)
        => node.PreviousValueId().CreateNext();

    public static DataType UnsafeExpression_Type(IUnsafeExpressionNode node)
        => node.IntermediateExpression?.Type ?? DataType.Unknown;

    public static DataType FunctionInvocationExpression_Type(IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration?.Type.Return.Type ?? DataType.Unknown;

    public static ContextualizedOverload? FunctionInvocationExpression_ContextualizedOverload(
        IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.ReferencedDeclaration)
            : null;

    public static IFlowState FunctionInvocationExpression_FlowStateAfter(IFunctionInvocationExpressionNode node)
    {
        // The flow state just before the function is called is the state after all arguments have evaluated
        var flowState = node.IntermediateArguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.IntermediateArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static FunctionType FunctionReferenceInvocation_FunctionType(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionType)node.Expression.Type;

    public static DataType FunctionReferenceInvocation_Type(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionType.Return.Type;

    public static IFlowState FunctionReferenceInvocation_FlowStateAfter(IFunctionReferenceInvocationExpressionNode node)
    {
        // The flow state just before the function is called is the state after all arguments have evaluated
        var flowState = node.IntermediateArguments.LastOrDefault()?.FlowStateAfter ?? node.Expression.FlowStateAfter;
        // TODO handle the fact that the function reference itself must be combined too
        var contextualizedOverload = ContextualizedOverload.Create(node.FunctionType);
        var argumentValueIds = ArgumentValueIds(contextualizedOverload, null, node.IntermediateArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static BoolConstValueType BoolLiteralExpression_Type(IBoolLiteralExpressionNode node)
        => node.Value ? DataType.True : DataType.False;

    public static IntegerConstValueType IntegerLiteralExpression_Type(IIntegerLiteralExpressionNode node)
        => new IntegerConstValueType(node.Value);

    public static OptionalType NoneLiteralExpression_Type(INoneLiteralExpressionNode _)
        => DataType.None;

    public static DataType StringLiteralExpression_Type(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName).OfType<ITypeDeclarationNode>().TrySingle();
        return typeSymbolNode?.Symbol.GetDeclaredType()?.With(Capability.Constant, FixedList.Empty<DataType>()) ?? DataType.Unknown;
    }

    public static IFlowState LiteralExpression_FlowStateAfter(ILiteralExpressionNode node)
        => node.FlowStateBefore().Constant(node.ValueId);

    public static void StringLiteralExpression_ContributeDiagnostics(
        IStringLiteralExpressionNode node,
        Diagnostics diagnostics)
    {
        if (node.Type is UnknownType)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Could not find string type for string literal."));
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static ContextualizedOverload? MethodInvocationExpression_ContextualizedOverload(
        IMethodInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.MethodGroup.Context.Type, node.ReferencedDeclaration)
            : null;

    public static IAmbiguousExpressionNode? MethodInvocationExpression_Rewrite_ImplicitMove(
        IMethodInvocationExpressionNode node)
    {
        var expectedSelfType = node.ReferencedDeclaration?.Symbol.SelfParameterType.Type ?? DataType.Unknown;
        if (expectedSelfType is not CapabilityType { Capability: var expectedCapability }
            || (expectedCapability != Capability.Isolated && expectedCapability != Capability.TemporarilyIsolated))
            return null;

        var isTemporary = expectedCapability == Capability.TemporarilyIsolated;

        var selfType = node.MethodGroup.Context.Type;
        if (selfType is CapabilityType { Capability: var capability }
            && capability == expectedCapability)
            return null;

        // TODO what if selfType is not a capability type?

        var context = node.MethodGroup.Context;
        var contextSyntax = (ITypedExpressionSyntax)context.Syntax;
        var implicitFreeze = isTemporary
            ? new ImplicitTempMoveExpressionNode(contextSyntax, context)
            : (IExpressionNode)(context is IVariableNameExpressionNode variableName
                ? new MoveVariableExpressionNode(contextSyntax, variableName, isImplicit: true)
                : new MoveValueExpressionNode(contextSyntax, context, isImplicit: true));
        var methodGroup = node.MethodGroup;
        var newMethodGroup = new MethodGroupNameNode(methodGroup.Syntax, implicitFreeze,
            methodGroup.MethodName, methodGroup.TypeArguments, methodGroup.ReferencedDeclarations);
        return new MethodInvocationExpressionNode(node.Syntax, newMethodGroup, node.CurrentArguments);
    }

    public static IAmbiguousExpressionNode? MethodInvocationExpression_Rewrite_ImplicitFreeze(
        IMethodInvocationExpressionNode node)
    {
        var expectedSelfType = node.ReferencedDeclaration?.Symbol.SelfParameterType.Type ?? DataType.Unknown;
        if (expectedSelfType is not CapabilityType { Capability: var expectedCapability }
            || (expectedCapability != Capability.Constant && expectedCapability != Capability.TemporarilyConstant))
            return null;

        var isTemporary = expectedCapability == Capability.TemporarilyConstant;

        var selfType = node.MethodGroup.Context.Type;
        if (selfType is CapabilityType { Capability: var capability }
            && capability == expectedCapability)
            return null;

        // TODO what if selfType is not a capability type?

        var context = node.MethodGroup.Context;
        var contextSyntax = (ITypedExpressionSyntax)context.Syntax;
        IFreezeExpressionNode implicitFreeze = context is IVariableNameExpressionNode variableName
            ? new FreezeVariableExpressionNode(contextSyntax, variableName, isTemporary, isImplicit: true)
            : new FreezeValueExpressionNode(contextSyntax, context, isTemporary, isImplicit: true);
        var methodGroup = node.MethodGroup;
        var newMethodGroup = new MethodGroupNameNode(methodGroup.Syntax, implicitFreeze, methodGroup.MethodName,
            methodGroup.TypeArguments, methodGroup.ReferencedDeclarations);
        return new MethodInvocationExpressionNode(node.Syntax, newMethodGroup, node.CurrentArguments);
    }

    public static DataType MethodInvocationExpression_Type(IMethodInvocationExpressionNode node)
    {
        var selfType = node.MethodGroup.Context.Type;
        // TODO does this need to be modified by flow typing?
        var unboundType = node.ContextualizedOverload?.ReturnType.Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static IFlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node)
    {
        // The flow state just before the method is called is the state after all arguments have evaluated
        var flowState = node.IntermediateArguments.LastOrDefault()?.FlowStateAfter ?? node.MethodGroup.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.MethodGroup.Context, node.IntermediateArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static ContextualizedOverload? GetterInvocationExpression_ContextualizedOverload(
        IGetterInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.Context.Type, node.ReferencedDeclaration)
            : null;
    public static DataType GetterInvocationExpression_Type(IGetterInvocationExpressionNode node)
    {
        var selfType = node.Context.Type;
        var unboundType = node.ContextualizedOverload?.ReturnType.Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static ContextualizedOverload? SetterInvocationExpression_ContextualizedOverload(ISetterInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.Context.Type, node.ReferencedDeclaration)
            : null;

    public static DataType SetterInvocationExpression_Type(ISetterInvocationExpressionNode node)
    {
        var selfType = node.Context.Type;
        var unboundType = node.ContextualizedOverload?.ParameterTypes[0].Type;
        var boundType = unboundType?.ReplaceSelfWith(selfType);
        return boundType ?? DataType.Unknown;
    }

    public static IFlowState SetterInvocationExpression_FlowStateAfter(ISetterInvocationExpressionNode node)
    {
        if (node.IntermediateValue is not IExpressionNode value)
            return IFlowState.Empty;
        // The flow state just before the setter is called is the state after the argument has been evaluated
        var flowState = value.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.Context, [value]);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
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

    public static DataType FieldAccessExpression_Type(IFieldAccessExpressionNode node)
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

    public static IFlowState FieldAccessExpression_FlowStateAfter(IFieldAccessExpressionNode node)
        => node.Context.FlowStateAfter.AccessField(node);

    public static IFlowState SelfExpression_FlowStateAfter(ISelfExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDefinition, node.ValueId);

    public static DataType SelfExpression_Type(ISelfExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDefinition);
    public static Pseudotype SelfExpression_Pseudotype(ISelfExpressionNode node)
        => node.ReferencedSymbol?.Type ?? DataType.Unknown;


    public static ContextualizedOverload? NewObjectExpression_ContextualizedOverload(
        INewObjectExpressionNode node)
        => node.ReferencedConstructor is not null
            ? ContextualizedOverload.Create(node.ConstructingType.NamedType, node.ReferencedConstructor)
            : null;

    public static DataType NewObjectExpression_Type(INewObjectExpressionNode node)
        // TODO does this need to be modified by flow typing?
        => node.ContextualizedOverload?.ReturnType.Type ?? DataType.Unknown;

    public static IFlowState NewObjectExpression_FlowStateAfter(INewObjectExpressionNode node)
    {
        // The flow state just before the constructor is called is the state after all arguments have evaluated
        var flowState = node.IntermediateArguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.IntermediateArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static ContextualizedOverload?
        InitializerInvocationExpression_ContextualizedOverload(IInitializerInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
           && node.InitializerGroup.Context.NamedBareType is not null and var initializingType
            ? ContextualizedOverload.Create(initializingType.With(Capability.Mutable), node.ReferencedDeclaration)
            : null;

    public static DataType InitializerInvocationExpression_Type(IInitializerInvocationExpressionNode node)
        // TODO does this need to be modified by flow typing?
        => node.ContextualizedOverload?.ReturnType.Type ?? DataType.Unknown;

    public static IFlowState InitializerInvocationExpression_FlowStateAfter(IInitializerInvocationExpressionNode node)
    {
        // The flow state just before the initializer is called is the state after all arguments have evaluated
        var flowState = node.IntermediateArguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.IntermediateArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId, node.Type);
    }

    public static DataType AssignmentExpression_Type(IAssignmentExpressionNode node)
        => node.LeftOperand.Type;

    public static IFlowState AssignmentExpression_FlowStateAfter(IAssignmentExpressionNode node)
        => node.IntermediateRightOperand?.FlowStateAfter.Combine(node.LeftOperand.ValueId, node.IntermediateRightOperand.ValueId, node.ValueId) ?? IFlowState.Empty;

    public static DataType BinaryOperatorExpression_Type(IBinaryOperatorExpressionNode node)
    {
        if (node.Antetype is ISimpleOrConstValueAntetype simpleOrConstValueAntetype)
            return simpleOrConstValueAntetype.ToType();
        if (node.Antetype is UnknownAntetype)
            return DataType.Unknown;

        var leftType = node.IntermediateLeftOperand?.Type ?? DataType.Unknown;
        var rightType = node.IntermediateRightOperand?.Type ?? DataType.Unknown;
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
        var rangeAntetype = rangeTypeDeclaration?.Symbol.GetDeclaredType()?.With(Capability.Constant, FixedList.Empty<DataType>())
                            ?? DataType.Unknown;
        return rangeAntetype;
    }

    public static IFlowState BinaryOperatorExpression_FlowStateAfter(IBinaryOperatorExpressionNode node)
        // Left and right are swapped here because right is known to not be null while left may be null and order doesn't matter to combine
        => node.IntermediateRightOperand?.FlowStateAfter.Combine(
            node.IntermediateRightOperand.ValueId, node.IntermediateLeftOperand?.ValueId, node.ValueId)
           ?? IFlowState.Empty;

    public static void BinaryOperatorExpression_CollectDiagnostics(IBinaryOperatorExpressionNode node, Diagnostics diagnostics)
    {
        if (node.Type == DataType.Unknown)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(node.File,
                node.Syntax.Span, node.Operator, node.IntermediateLeftOperand!.Type, node.IntermediateRightOperand!.Type));
    }

    public static DataType IfExpression_Type(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.Type.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Type;
    }

    public static DataType ResultStatement_Type(IResultStatementNode node)
        => node.IntermediateExpression?.Type.ToNonConstValueType() ?? DataType.Unknown;

    public static IFlowState IfExpression_FlowStateAfter(IIfExpressionNode node)
    {
        var thenPath = node.ThenBlock.FlowStateAfter;
        var elsePath = node.ElseClause?.FlowStateAfter ?? node.IntermediateCondition?.FlowStateAfter ?? IFlowState.Empty;
        var flowState = thenPath.Merge(elsePath);
        return flowState.Combine(node.ThenBlock.ValueId, node.ElseClause?.ValueId, node.ValueId);
    }

    public static DataType BlockExpression_Type(IBlockExpressionNode node)
    {
        // TODO what about blocks that contain a return etc. and never return?
        foreach (var statement in node.Statements)
            if (statement.ResultType is not null and var resultType)
                return resultType;

        // If there was no result expression, then the block type is void
        return DataType.Void;
    }

    public static IFlowState BlockExpression_FlowStateAfter(IBlockExpressionNode node)
    {
        var flowState = node.Statements.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        flowState = flowState.DropBindings(node.Statements.OfType<IVariableDeclarationStatementNode>());
        foreach (var statement in node.Statements)
            if (statement.ResultValueId is ValueId resultValueId)
                return flowState.Transform(resultValueId, node.ValueId, node.Type);

        return flowState.Constant(node.ValueId);
    }

    public static DataType WhileExpression_Type(IWhileExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static IFlowState WhileExpression_FlowStateAfter(IWhileExpressionNode node)
        // TODO loop flow state
        // Merge condition with block flow state because the body may not be executed
        => (node.IntermediateCondition?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `while` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);

    public static DataType LoopExpression_Type(ILoopExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static IFlowState LoopExpression_FlowStateAfter(ILoopExpressionNode node)
        // Body is always executes at least once
        // TODO loop flow state
        => node.Block.FlowStateAfter
               // TODO when the `loop` has a type other than void, correctly handle the value id
               .Constant(node.ValueId);

    public static DataType ConversionExpression_Type(IConversionExpressionNode node)
    {
        var convertToType = node.ConvertToType.NamedType;
        if (node.Operator == ConversionOperator.Optional)
            convertToType = convertToType.MakeOptional();
        return convertToType;
    }

    public static IFlowState ConversionExpression_FlowStateAfter(IConversionExpressionNode node)
    {
        var intermediateReferent = node.IntermediateReferent;
        if (intermediateReferent is null)
            return IFlowState.Empty;
        return intermediateReferent.FlowStateAfter
            .Transform(node.IntermediateReferent?.ValueId, node.ValueId, node.Type);
    }

    public static IFlowState ImplicitConversionExpression_FlowStateAfter(IImplicitConversionExpressionNode node)
        => node.Referent.FlowStateAfter.Transform(node.Referent.ValueId, node.ValueId, node.Type);

    public static DataType AsyncStartExpression_Type(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.IntermediateExpression?.Type ?? DataType.Unknown);

    public static IFlowState AsyncStartExpression_FlowStateAfter(IAsyncStartExpressionNode node)
        // TODO this isn't correct, async start can act like a delayed lambda
        => node.IntermediateExpression?.FlowStateAfter.Combine(node.IntermediateExpression.ValueId, null, node.ValueId) ?? IFlowState.Empty;

    public static DataType AwaitExpression_Type(IAwaitExpressionNode node)
    {
        if (node.IntermediateExpression?.Type is CapabilityType { DeclaredType: var declaredType } type
            && Intrinsic.PromiseType.Equals(declaredType))
            return type.TypeArguments[0];

        return DataType.Unknown;
    }

    public static IFlowState AwaitExpression_FlowStateAfter(IAwaitExpressionNode node)
        => node.IntermediateExpression?.FlowStateAfter.Combine(node.IntermediateExpression.ValueId, null, node.ValueId) ?? IFlowState.Empty;

    public static DataType UnaryOperatorExpression_Type(IUnaryOperatorExpressionNode node)
        => node.Antetype switch
        {
            ISimpleOrConstValueAntetype t => t.ToType(),
            UnknownAntetype => DataType.Unknown,
            _ => throw new InvalidOperationException($"Unexpected antetype {node.Antetype}")
        };

    public static IFlowState UnaryOperatorExpression_FlowStateAfter(IUnaryOperatorExpressionNode node)
        => node.IntermediateOperand?.FlowStateAfter.Transform(node.IntermediateOperand.ValueId, node.ValueId, node.Type) ?? IFlowState.Empty;

    public static DataType FunctionName_Type(IFunctionNameNode node)
        => node.ReferencedDeclaration?.Type ?? DataType.Unknown;

    public static IFlowState FunctionName_FlowStateAfter(IFunctionNameNode node)
        => node.FlowStateBefore().Constant(node.ValueId);


    public static DataType FreezeExpression_Type(IFreezeExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow freeze, a freeze expression always results in a
        // constant reference. A diagnostic is generated if the capability doesn't allow freeze.

        var capability = node.IsTemporary ? Capability.TemporarilyConstant : Capability.Constant;
        return capabilityType.With(capability);
    }

    public static IFlowState FreezeVariableExpression_FlowStateAfter(IFreezeVariableExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        var referentValueId = node.Referent.ValueId;
        return node.IsTemporary
            // TODO this implies that temp freeze is a fundamentally different operation and ought to have its own node type
            ? flowStateBefore.TempFreeze(referentValueId, node.ValueId)
            : flowStateBefore.FreezeVariable(node.Referent.ReferencedDefinition, referentValueId, node.ValueId);
    }

    public static IFlowState FreezeValueExpression_FlowStateAfter(IFreezeValueExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        var referentValueId = node.Referent.ValueId;
        return node.IsTemporary
            ? flowStateBefore.TempFreeze(referentValueId, node.ValueId)
            : flowStateBefore.FreezeValue(referentValueId, node.ValueId);
    }

    public static void FreezeVariableExpression_ContributeDiagnostics(IFreezeVariableExpressionNode node, Diagnostics diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return;

        if (!capabilityType.AllowsFreeze)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow freezing"));
        else if (!node.IsTemporary && !node.Referent.FlowStateAfter.CanFreezeExceptFor(node.Referent.ReferencedDefinition, node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotFreezeValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static DataType MoveExpression_Type(IMoveExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow move, a move expression always results in an
        // isolated reference. A diagnostic is generated if the capability doesn't allow move.
        // TODO maybe `temp iso` should require `temp move`?
        return capabilityType.IsTemporarilyIsolatedReference ? capabilityType
            : capabilityType.With(Capability.Isolated);
    }

    public static IFlowState MoveVariableExpression_FlowStateAfter(IMoveVariableExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.MoveVariable(node.Referent.ReferencedDefinition, node.Referent.ValueId, node.ValueId);
    }

    public static void MoveVariableExpression_ContributeDiagnostics(IMoveVariableExpressionNode node, Diagnostics diagnostics)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return;

        if (!capabilityType.AllowsMove)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Reference capability does not allow moving"));
        else if (!node.Referent.FlowStateAfter.IsIsolatedExceptFor(node.Referent.ReferencedDefinition, node.Referent.ValueId))
            diagnostics.Add(FlowTypingError.CannotMoveValue(node.File, node.Syntax, node.Referent.Syntax));
    }

    public static IFlowState MoveValueExpression_FlowStateAfter(IMoveValueExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.MoveValue(node.Referent.ValueId, node.ValueId);
    }

    public static DataType ImplicitTempMoveExpression_Type(IImplicitTempMoveExpressionNode node)
    {
        if (node.Referent.Type is not CapabilityType capabilityType)
            return DataType.Unknown;

        // Even if the capability doesn't allow move, a temp move expression always results in a
        // temp isolated reference. A diagnostic is generated if the capability doesn't allow move.
        return capabilityType.With(Capability.TemporarilyIsolated);
    }

    public static IFlowState ImplicitTempMoveExpression_FlowStateAfter(IImplicitTempMoveExpressionNode node)
    {
        var flowStateBefore = node.Referent.FlowStateAfter;
        return flowStateBefore.TempMove(node.Referent.ValueId, node.ValueId);
    }

    public static IFlowState ExpressionStatement_FlowStateAfter(IExpressionStatementNode node)
    {
        var intermediateExpression = node.IntermediateExpression;
        if (intermediateExpression is null)
            return IFlowState.Empty;

        return intermediateExpression.FlowStateAfter.DropValue(intermediateExpression.ValueId);
    }

    public static IFlowState ReturnExpression_FlowStateAfter(IReturnExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static IFlowState BreakExpression_FlowStateAfter(IBreakExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static IFlowState NextExpression_FlowStateAfter(INextExpressionNode node)
        // Whatever the previous flow state, now nothing exists except the constant for the `never` typed value
        => IFlowState.Empty.Constant(node.ValueId);

    public static IFlowState PatternMatchExpression_FlowStateAfter(IPatternMatchExpressionNode node)
        // Constant for the boolean result of the pattern match
        => node.Pattern.FlowStateAfter.Constant(node.ValueId);
}
