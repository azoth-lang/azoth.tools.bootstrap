using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public static class ExpressionTypesAspect
{
    public static ValueId Expression_ValueId(IExpressionNode node)
        => node.PreviousValueId().CreateNext();

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
        var referentType = node.FinalReferent.Type;
        if (referentType is CapabilityType capabilityType)
            return capabilityType.With(Capability.Identity);
        return DataType.Unknown;
    }

    public static void IdExpression_ContributeDiagnostics(IIdExpressionNode node, Diagnostics diagnostics)
    {
        //if (node.Type is not UnknownType)
        //    return;

        //var referentType = ((IExpressionNode)node.Referent).Type;
        //if (referentType is not CapabilityType)
        //    diagnostics.Add(TypeError.CannotIdNonReferenceType(node.File, node.Syntax.Span, referentType));
    }

    public static DataType VariableNameExpression_Type(IVariableNameExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDeclaration);

    public static FlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDeclaration, node.ValueId);

    public static FlowState NamedParameter_FlowStateAfter(INamedParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static FlowState SelfParameter_FlowStateAfter(ISelfParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static ValueId VariableDeclarationStatement_ValueId(IVariableDeclarationStatementNode node)
        => node.PreviousValueId().CreateNext();

    public static ValueId BindingPattern_ValueId(IBindingPatternNode node)
        => node.PreviousValueId().CreateNext();

    public static DataType UnsafeExpression_Type(IUnsafeExpressionNode node)
        => node.FinalExpression.Type;

    public static DataType FunctionInvocationExpression_Type(IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration?.Type.Return.Type ?? DataType.Unknown;

    public static ContextualizedOverload<IFunctionLikeDeclarationNode>? FunctionInvocationExpression_ContextualizedOverload(
        IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.ReferencedDeclaration)
            : null;

    public static FlowState FunctionInvocationExpression_FlowStateAfter(IFunctionInvocationExpressionNode node)
    {
        // The flow state just before the function is called is the state after all arguments have evaluated
        var flowState = node.FinalArguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.FinalArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId);
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

    public static void StringLiteralExpression_ContributeDiagnostics(
        IStringLiteralExpressionNode node,
        Diagnostics diagnostics)
    {
        if (node.Type is UnknownType)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Could not find string type for string literal."));
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static DataType MethodInvocationExpression_Type(IMethodInvocationExpressionNode node)
    {
        var selfType = node.MethodGroup.Context.Type;
        // TODO does this need to be modified by flow typing?
        return node.ContextualizedOverload?.ReturnType.Type.ReplaceSelfWith(selfType)
               ?? DataType.Unknown;
    }

    public static ContextualizedOverload<IStandardMethodDeclarationNode>? MethodInvocationExpression_ContextualizedOverload(
        IMethodInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.MethodGroup.Context.Type, node.ReferencedDeclaration)
            : null;

    public static FlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node)
    {
        // The flow state just before the method is called is the state after all arguments have evaluated
        var flowState = node.FinalArguments.LastOrDefault()?.FlowStateAfter ?? node.MethodGroup.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.MethodGroup.Context, node.FinalArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId);
    }

    private static IEnumerable<ArgumentValueId> ArgumentValueIds(
        ContextualizedOverload? overload,
        IExpressionNode? selfArgument,
        IEnumerable<IExpressionNode> arguments)
    {
        arguments = selfArgument.YieldValue().Concat(arguments);
        if (overload is null)
            return arguments.Select(a => new ArgumentValueId(false, a.ValueId));

        var parameterTypes = overload.ParameterTypes.AsEnumerable();
        if (selfArgument is not null)
        {
            if (overload.SelfParameterType is not SelfParameterType selfParameterType)
                throw new InvalidOperationException("Self argument provided for overload without self parameter");
            parameterTypes = parameterTypes.Prepend(selfParameterType.ToUpperBound());
        }
        return parameterTypes.EquiZip(arguments)
                             .Select((p, a) => new ArgumentValueId(p.IsLent, a.ValueId));
    }

    public static DataType FieldAccessExpression_Type(IFieldAccessExpressionNode node)
    {
        // TODO handle pseudotype of self access
        var contextType = node.Context.Type;
        var fieldType = node.ReferencedDeclaration.BindingType;
        // Access must be applied first, so it can account for independent generic parameters.
        var type = fieldType.AccessedVia(contextType);
        // Then type parameters can be replaced now that they have the correct access
        if (contextType is NonEmptyType nonEmptyContext)
            // resolve generic type fields
            type = nonEmptyContext.ReplaceTypeParametersIn(type);

        return type;
    }

    public static FlowState FieldAccessExpression_FlowStateAfter(IFieldAccessExpressionNode node)
        => node.Context.FlowStateAfter.AccessMember(node.Context.ValueId, node.ValueId, node.Type);

    public static FlowState SelfExpression_FlowStateAfter(ISelfExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedParameter, node.ValueId);

    public static DataType SelfExpression_Type(ISelfExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedParameter);

    public static FlowState NewObjectExpression_FlowStateAfter(INewObjectExpressionNode node)
    {
        // The flow state just before the constructor is called is the state after all arguments have evaluated
        var flowState = node.FinalArguments.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, null, node.FinalArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId);
    }

    public static DataType NewObjectExpression_Type(INewObjectExpressionNode node)
        // TODO does this need to be modified by flow typing?
        => node.ContextualizedOverload?.ReturnType.Type ?? DataType.Unknown;

    public static ContextualizedOverload<IConstructorDeclarationNode>? NewObjectExpression_ContextualizedOverload(
        INewObjectExpressionNode node)
        => node.ReferencedConstructor is not null
            ? ContextualizedOverload.Create(node.ConstructingType.NamedType, node.ReferencedConstructor)
            : null;

    public static DataType AssignmentExpression_Type(IAssignmentExpressionNode node)
        => node.LeftOperand.Type;

    public static FlowState AssignmentExpression_FlowStateAfter(IAssignmentExpressionNode node)
        => node.FinalRightOperand.FlowStateAfter.Combine(node.LeftOperand.ValueId, node.FinalRightOperand.ValueId, node.ValueId);

    public static DataType BinaryOperatorExpression_Type(IBinaryOperatorExpressionNode node)
    {
        if (node.Antetype is ISimpleOrConstValueAntetype simpleOrConstValueAntetype)
            return simpleOrConstValueAntetype.ToType();
        if (node.Antetype is UnknownAntetype)
            return DataType.Unknown;

        var leftType = node.FinalLeftOperand.Type;
        var rightType = node.FinalRightOperand.Type;
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

    public static FlowState BinaryOperatorExpression_FlowStateAfter(IBinaryOperatorExpressionNode node)
        => node.FinalRightOperand.FlowStateAfter;

    public static DataType IfExpression_Type(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.Type.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Type;
    }

    public static DataType ResultStatement_Type(IResultStatementNode node)
        => node.FinalExpression.Type.ToNonConstValueType();

    public static FlowState IfExpression_FlowStateAfter(IIfExpressionNode node)
    {
        var thenPath = node.ThenBlock.FlowStateAfter;
        var elsePath = node.ElseClause?.FlowStateAfter ?? node.FinalCondition.FlowStateAfter;
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

    public static FlowState BlockExpression_FlowStateAfter(IBlockExpressionNode node)
        => node.Statements.LastOrDefault()?.FlowStateAfter ?? node.FlowStateBefore();

    public static DataType WhileExpression_Type(IWhileExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static FlowState WhileExpression_FlowStateAfter(IWhileExpressionNode node)
        // TODO loop flow state
        // Merge condition with block flow state because the body may not be executed
        => node.FinalCondition.FlowStateAfter.Merge(node.Block.FlowStateAfter);

    public static DataType LoopExpression_Type(ILoopExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static DataType ForeachExpression_Type(IForeachExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;
}
