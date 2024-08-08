using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using DataType = Azoth.Tools.Bootstrap.Compiler.Types.DataType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// Do basic analysis of bodies.
/// </summary>
/// <remarks>
/// This includes type checking with reference capabilities and the sharing relationships
/// between variables.
/// </remarks>
public class BasicBodyAnalyzer
{
    private readonly CodeFile file;
    private readonly InvocableSymbol containingSymbol;
    private readonly ISymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly UserTypeSymbol? rangeSymbol;
    private readonly Diagnostics diagnostics;
    private readonly ReturnType? returnType;
    private readonly ParameterSharingRelation parameterSharing;

    public BasicBodyAnalyzer(
        IFunctionDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDefinition, containingDefinition.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, returnType)
    { }
    public BasicBodyAnalyzer(
        IAssociatedFunctionDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDefinition, containingDefinition.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConstructorDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDefinition,
            containingDefinition.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDefinition.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IInitializerDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDefinition,
            containingDefinition.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDefinition.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConcreteMethodDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDefinition,
            containingDefinition.Parameters.Select(p => p.Symbol.Result).Prepend<BindingSymbol>(containingDefinition.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IFieldDefinitionSyntax containingDefinition,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics)
        : this(containingDefinition, [],
            symbolTreeBuilder, symbolTrees, rangeSymbol, diagnostics, null)
    { }

    private BasicBodyAnalyzer(
        IEntityDefinitionSyntax containingDefinition,
        IEnumerable<BindingSymbol> parameterSymbols,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType? returnType)
    {
        file = containingDefinition.File;
        containingSymbol = (InvocableSymbol)containingDefinition.Symbol.Result;
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.rangeSymbol = rangeSymbol;
        this.diagnostics = diagnostics;
        this.symbolTrees = symbolTrees;
        this.returnType = returnType;
        parameterSharing = new ParameterSharingRelation(parameterSymbols);
    }

    public void ResolveTypes(IBodySyntax body)
    {
        if (returnType is not { } expectedReturnType)
            throw new InvalidOperationException("Field definitions don't have bodies.");
        var flow = new FlowStateMutable(diagnostics, file, parameterSharing);
        switch (body)
        {
            default:
                throw ExhaustiveMatch.Failed(body);
            case IBlockBodySyntax syn:
                ResolveTypes(syn, flow);
                break;
            case IExpressionBodySyntax syn:
                ResolveTypes(syn, flow, expectedReturnType.Type);
                break;
        }
    }

    private void ResolveTypes(IBlockBodySyntax body, FlowStateMutable flow)
    {
        foreach (var statement in body.Statements)
            ResolveTypes(statement, flow);
    }

    private void ResolveTypes(IExpressionBodySyntax body, FlowStateMutable flow, DataType expectedType)
    {
        var result = ResolveTypes(body.ResultStatement, flow)!;
        // local variables are no longer in scope and isolated parameters have no external references
        flow.DropBindingsForReturn();
        result = AddImplicitConversionIfNeeded(result, expectedType, allowMoveOrFreeze: true, flow);
        CheckTypeCompatibility(expectedType, result.Syntax);
    }

    /// <summary>
    /// Resolve the types in a statement. If the statement was a <see cref="IResultStatementSyntax"/>
    /// the <see cref="ExpressionResult"/> is returned. Returns <see langword="null"/> otherwise.
    /// </summary>
    private ExpressionResult? ResolveTypes(
        IStatementSyntax statement,
        FlowStateMutable flow)
    {
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IVariableDeclarationStatementSyntax variableDeclaration:
                ResolveTypes(variableDeclaration, flow);
                break;
            case IExpressionStatementSyntax expressionStatement:
            {
                var result = InferType(expressionStatement.Expression, flow);
                // At the end of the statement, any result reference is discarded
                flow.Drop(result.Variable);
                break;
            }
            case IResultStatementSyntax resultStatement:
            {
                var result = InferType(resultStatement.Expression, flow);

                // Return type for use in determining block type. Keep result shared for use in
                // parent expression.
                return result;
            }
        }
        return null;
    }

    private void ResolveTypes(
        IVariableDeclarationStatementSyntax variableDeclaration,
        FlowStateMutable flow)
    {
        var initializerResult = InferType(variableDeclaration.Initializer, flow);
        DataType variableType;
        if (variableDeclaration.Type is not null)
            variableType = variableDeclaration.Type.NamedType!;
        else if (variableDeclaration.Initializer is not null)
            variableType = InferDeclarationType(variableDeclaration.Initializer, variableDeclaration.Capability);
        else
            variableType = DataType.Unknown;

        if (initializerResult is not null)
        {
            initializerResult = AddImplicitConversionIfNeeded(initializerResult, variableType, allowMoveOrFreeze: true, flow);
            if (!variableType.IsAssignableFrom(initializerResult.Type))
                diagnostics.Add(TypeError.CannotImplicitlyConvert(file, initializerResult.Syntax, initializerResult.Type, variableType));
        }

        var symbol = NamedVariableSymbol.CreateLocal(containingSymbol, variableDeclaration.IsMutableBinding, variableDeclaration.Name, variableDeclaration.DeclarationNumber.Result, variableType);
        variableDeclaration.Symbol.Fulfill(symbol);
        symbolTreeBuilder.Add(symbol);
        // Declare the symbol and combine it with the initializer result
        flow.Declare(symbol, initializerResult?.Variable);
    }

    /// <summary>
    /// Infer the type of a variable declaration from the initializer expression and an optional
    /// reference capability.
    /// </summary>
    private static DataType InferDeclarationType(
        IExpressionSyntax expression,
        ICapabilitySyntax? inferCapability)
    {
        var type = expression.DataType.Assigned();
        if (!type.IsFullyKnown) return DataType.Unknown;
        type = type.ToNonConstValueType();

        switch (expression)
        {
            case IMoveExpressionSyntax when inferCapability is null:
                // If we are explicitly moving and no capability is specified, then
                // take the mutable type.
                return type;
            default:
            {
                // We assume immutability on variables unless explicitly stated
                if (inferCapability is null)
                    type = type.WithoutWrite();
                else
                {
                    if (type is not CapabilityType capabilityType)
                        throw new NotImplementedException(
                            "Compile error: can't infer mutability for non-capability type.");

                    type = capabilityType.With(inferCapability.Declared.ToCapability());
                }

                return type;
            }
        }
    }

    public void CheckFieldInitializerType(IExpressionSyntax expression, DataType expectedType)
    {
        var flow = new FlowStateMutable(diagnostics, file);
        CheckIndependentExpressionType(expression, expectedType, allowMoveOrFreeze: true, flow);
    }

    /// <summary>
    /// Check the type of an expression that is independent. One whose result won't be shared with
    /// anything.
    /// </summary>
    private void CheckIndependentExpressionType(
        IExpressionSyntax expression,
        DataType expectedType,
        bool allowMoveOrFreeze,
        FlowStateMutable flow)
    {
        var result = InferType(expression, flow);
        result = AddImplicitConversionIfNeeded(result, expectedType, allowMoveOrFreeze, flow);
        CheckTypeCompatibility(expectedType, expression);
        flow.Drop(result.Variable);
    }

    /// <summary>
    /// Create an implicit conversion if needed and allowed.
    /// </summary>
    private ExpressionResult AddImplicitConversionIfNeeded(
        ExpressionResult expression,
        ParameterType expectedType,
        bool allowMoveOrFreeze,
        FlowStateMutable flow)
        => AddImplicitConversionIfNeeded(expression, expectedType.Type, allowMoveOrFreeze, flow);

    /// <summary>
    /// Create an implicit conversion if needed and allowed.
    /// </summary>
    private ExpressionResult AddImplicitConversionIfNeeded(
        ExpressionResult expression,
        DataType expectedType,
        bool allowMoveOrFreeze,
        FlowStateMutable flow)
    {
        var syntax = expression.Syntax;
        var conversion = CreateImplicitConversion(expectedType,
            expression.Type, expression.Variable, allowMoveOrFreeze, syntax.ImplicitConversion, flow, out var newResult);
        if (conversion is not null) syntax.AddConversion(conversion);
        return expression with { Variable = newResult };
    }

    private static ChainedConversion? CreateImplicitConversion(
        DataType toType,
        DataType fromType,
        ResultVariable? fromResult,
        bool allowMoveOrFreeze,
        Conversion priorConversion,
        FlowStateMutable flow,
        out ResultVariable? newResult,
        bool enact = true)
    {
        newResult = fromResult;
        switch (toType, fromType)
        {
            case (DataType to, DataType from) when from == to:
                return null;
            case (OptionalType { Referent: var to }, OptionalType { Referent: var from }):
                // Direct subtype
                if (to.IsAssignableFrom(from))
                    return null;
                var liftedConversion = CreateImplicitConversion(to, from, fromResult, allowMoveOrFreeze, IdentityConversion.Instance, flow, out newResult);
                return liftedConversion is null ? null : new LiftedConversion(liftedConversion, priorConversion);
            case (OptionalType { Referent: var to }, not OptionalType):
                if (!to.IsAssignableFrom(fromType))
                {
                    var conversion = CreateImplicitConversion(to, fromType, fromResult, allowMoveOrFreeze, priorConversion, flow, out newResult);
                    if (conversion is null) return null; // Not able to convert to the referent type
                    priorConversion = conversion;
                }
                return new OptionalConversion(priorConversion);
            case (CapabilityType<BoolType> to, BoolConstValueType):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (CapabilityType<FixedSizeIntegerType> to, CapabilityType<FixedSizeIntegerType> from):
                if (to.DeclaredType.Bits > from.DeclaredType.Bits && (!from.DeclaredType.IsSigned || to.DeclaredType.IsSigned))
                    return new SimpleTypeConversion(to.DeclaredType, priorConversion);
                return null;
            case (CapabilityType<FixedSizeIntegerType> to, IntegerConstValueType from):
            {
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.DeclaredType.IsSigned) * 8;
                if (to.DeclaredType.Bits >= bits && (!requireSigned || to.DeclaredType.IsSigned))
                    return new SimpleTypeConversion(to.DeclaredType, priorConversion);

                return null;
            }
            case (CapabilityType<BigIntegerType> { DeclaredType.IsSigned: true } to, CapabilityType { DeclaredType: IntegerType }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (CapabilityType<BigIntegerType> to, CapabilityType { DeclaredType: IntegerType { IsSigned: false } }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (CapabilityType<BigIntegerType> { DeclaredType.IsSigned: true } to, IntegerConstValueType):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (CapabilityType<BigIntegerType> to, IntegerConstValueType { IsSigned: false }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (CapabilityType<PointerSizedIntegerType> to, IntegerConstValueType from):
            {
                var requireSigned = from.Value < 0;
                return !requireSigned || to.DeclaredType.IsSigned ? new SimpleTypeConversion(to.DeclaredType, priorConversion) : null;
            }
            case (CapabilityType { IsTemporarilyConstantReference: true } to, CapabilityType { AllowsFreeze: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: false, from.BareType)
                     && allowMoveOrFreeze:
            {
                if (enact) newResult = flow.TempFreeze(fromResult!);
                return new FreezeConversion(priorConversion, ConversionKind.Temporary);
            }
            case (CapabilityType { IsConstantReference: true } to, CapabilityType { AllowsFreeze: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: false, from.BareType)
                     && allowMoveOrFreeze:
            {
                // Try to recover const. Note a variable name can never be frozen because the result is an alias.
                if (flow.CanFreeze(fromResult))
                    return new FreezeConversion(priorConversion, ConversionKind.Recover);
                return null;
            }
            case (CapabilityType { IsTemporarilyIsolatedReference: true } to, CapabilityType { AllowsRecoverIsolation: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: true, from.BareType)
                     && allowMoveOrFreeze:
            {
                if (enact) newResult = flow.TempMove(fromResult!);
                return new MoveConversion(priorConversion, ConversionKind.Temporary);
            }
            case (CapabilityType { IsIsolatedReference: true } to, CapabilityType { AllowsRecoverIsolation: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: true, from.BareType)
                    && allowMoveOrFreeze:
            {
                // Try to recover isolation. Note a variable name is never isolated because the result is an alias.
                if (flow.IsIsolated(fromResult))
                    return new MoveConversion(priorConversion, ConversionKind.Recover);
                return null;
            }
            default:
                return null;
        }
    }

    /// <summary>
    /// Infer the type of an expression and assign that type to the expression.
    /// </summary>
    [return: NotNullIfNotNull(nameof(expression))]
    private ExpressionResult? InferType(
        IExpressionSyntax? expression, FlowStateMutable flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case null:
                return null;
            case IIdExpressionSyntax exp:
            {
                var result = InferType(exp.Referent, flow);
                DataType type;
                if (result.Type is CapabilityType referenceType)
                    type = referenceType.With(Capability.Identity);
                else
                    type = DataType.Unknown;
                if (exp.DataType.Result != type)
                    throw new InvalidOperationException("Id expression type should be set by semantics");
                // Don't need to alias the symbol or union with result in flow because ids don't matter
                flow.Drop(result.Variable); // drop the previous result variable
                return new ExpressionResult(exp);
            }
            case IMoveExpressionSyntax exp:
            {
                // Semantics already assigned by SemanticsApplier
                var semantics = exp.Referent.Semantics.Result;
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case SelfExpressionSyntax sem:
                        return InferMoveExpressionType(exp, sem, flow);
                    case NamedVariableNameSyntax sem:
                        return InferMoveExpressionType(exp, sem, flow);
                    case FunctionGroupNameSyntax _:
                    case NamespaceNameSyntax _:
                    case TypeNameSyntax _:
                        // TODO add error
                        throw new NotImplementedException();
                    case UnknownNameSyntax _:
                        // Error should already be reported
                        break;
                }

                if (exp.DataType.Result != DataType.Unknown)
                    throw new InvalidOperationException("Move expression type should be set by semantics");
                return new ExpressionResult(exp);
            }
            case IFreezeExpressionSyntax exp:
            {
                // Semantics already assigned by SemanticsApplier
                var semantics = exp.Referent.Semantics.Result;
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case SelfExpressionSyntax sem:
                        return InferFreezeExpressionType(exp, sem, flow);
                    case NamedVariableNameSyntax sem:
                        return InferFreezeExpressionType(exp, sem, flow);
                    case FunctionGroupNameSyntax _:
                    case NamespaceNameSyntax _:
                    case TypeNameSyntax _:
                        // TODO add error
                        throw new NotImplementedException();
                    case UnknownNameSyntax _:
                        // Error should already be reported
                        break;
                }

                exp.DataType.Fulfill(DataType.Unknown);
                return new ExpressionResult(exp);
            }
            case IReturnExpressionSyntax exp:
            {
                if (returnType is not { } expectedReturnType)
                    throw new NotImplementedException("Return statement in field initializer.");

                var expectedType = expectedReturnType.Type;
                if (exp.Value is not null)
                {
                    var result = InferType(exp.Value, flow);
                    // local variables are no longer in scope and isolated parameters have no external references
                    flow.DropBindingsForReturn();
                    result = AddImplicitConversionIfNeeded(result, expectedType, allowMoveOrFreeze: true, flow);
                    CheckTypeCompatibility(expectedType, exp.Value);
                    // TODO aren't there other cases where this shouldn't be an error?
                    if (flow.IsLent(result.Variable))
                        diagnostics.Add(FlowTypingError.CannotReturnLent(file, exp));
                    flow.Drop(result.Variable);
                }

                // Return expressions always have the type Never
                return new ExpressionResult(exp);
            }
            case IIntegerLiteralExpressionSyntax exp:
            {
                // Logic moved to semantic tree
                return new ExpressionResult(exp);
            }
            case IStringLiteralExpressionSyntax exp:
                // Logic moved to semantic tree
                return new ExpressionResult(exp);
            case IBoolLiteralExpressionSyntax exp:
            {
                // Logic moved to semantic tree
                return new ExpressionResult(exp);
            }
            case IBinaryOperatorExpressionSyntax exp:
            {
                var leftOperand = exp.LeftOperand;
                var leftResult = InferType(leftOperand, flow);
                var @operator = exp.Operator;
                var rightOperand = exp.RightOperand;
                var rightResult = InferType(rightOperand, flow);
                var resultVariable = flow.Combine(leftResult.Variable, rightResult.Variable, exp);

                // If either is unknown, then we can't know whether there is a problem.
                // Note that the operator could be overloaded
                if (!leftResult.Type.IsFullyKnown || !rightResult.Type.IsFullyKnown)
                {
                    exp.DataType.Fulfill(DataType.Unknown);
                    return new ExpressionResult(exp, resultVariable);
                }

                DataType type = (leftResult.Type, @operator, rightResult.Type) switch
                {
                    (IntegerConstValueType left, BinaryOperator.Plus, IntegerConstValueType right) => left.Add(right),
                    (IntegerConstValueType left, BinaryOperator.Minus, IntegerConstValueType right) => left.Subtract(right),
                    (IntegerConstValueType left, BinaryOperator.Asterisk, IntegerConstValueType right) => left.Multiply(right),
                    (IntegerConstValueType left, BinaryOperator.Slash, IntegerConstValueType right) => left.DivideBy(right),
                    (IntegerConstValueType left, BinaryOperator.EqualsEquals, IntegerConstValueType right) => left.Equals(right),
                    (IntegerConstValueType left, BinaryOperator.NotEqual, IntegerConstValueType right) => left.NotEquals(right),
                    (IntegerConstValueType left, BinaryOperator.LessThan, IntegerConstValueType right) => left.LessThan(right),
                    (IntegerConstValueType left, BinaryOperator.LessThanOrEqual, IntegerConstValueType right) => left.LessThanOrEqual(right),
                    (IntegerConstValueType left, BinaryOperator.GreaterThan, IntegerConstValueType right) => left.GreaterThan(right),
                    (IntegerConstValueType left, BinaryOperator.GreaterThanOrEqual, IntegerConstValueType right) => left.GreaterThanOrEqual(right),

                    (BoolConstValueType left, BinaryOperator.EqualsEquals, BoolConstValueType right) => left.Equals(right),
                    (BoolConstValueType left, BinaryOperator.NotEqual, BoolConstValueType right) => left.NotEquals(right),
                    (BoolConstValueType left, BinaryOperator.And, BoolConstValueType right) => left.And(right),
                    (BoolConstValueType left, BinaryOperator.Or, BoolConstValueType right) => left.Or(right),

                    (CapabilityType { BareType: BareReferenceType }, BinaryOperator.EqualsEquals, CapabilityType { BareType: BareReferenceType })
                        or (CapabilityType { BareType: BareReferenceType }, BinaryOperator.NotEqual, CapabilityType { BareType: BareReferenceType })
                        => InferReferenceEqualityOperatorType(leftOperand, rightOperand),

                    (CapabilityType<BoolType>, BinaryOperator.EqualsEquals, CapabilityType<BoolType>)
                        or (CapabilityType<BoolType>, BinaryOperator.NotEqual, CapabilityType<BoolType>)
                        or (CapabilityType<BoolType>, BinaryOperator.And, CapabilityType<BoolType>)
                        or (CapabilityType<BoolType>, BinaryOperator.Or, CapabilityType<BoolType>)
                        => DataType.Bool,

                    (NonEmptyType, BinaryOperator.Plus, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.Minus, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.Asterisk, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.Slash, NonEmptyType)
                        => InferNumericOperatorType(leftResult, rightResult, flow),
                    (NonEmptyType, BinaryOperator.EqualsEquals, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.NotEqual, NonEmptyType)
                        or (OptionalType { Referent: NonEmptyType }, BinaryOperator.NotEqual, OptionalType { Referent: NonEmptyType })
                        or (NonEmptyType, BinaryOperator.LessThan, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.LessThanOrEqual, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.GreaterThan, NonEmptyType)
                        or (NonEmptyType, BinaryOperator.GreaterThanOrEqual, NonEmptyType)
                        => InferComparisonOperatorType(leftResult, rightResult, flow),

                    (_, BinaryOperator.DotDot, _)
                        or (_, BinaryOperator.LessThanDotDot, _)
                        or (_, BinaryOperator.DotDotLessThan, _)
                        or (_, BinaryOperator.LessThanDotDotLessThan, _)
                        => InferRangeOperatorType(leftResult, rightResult, flow),

                    (OptionalType { Referent: var referentType }, BinaryOperator.QuestionQuestion, NeverType)
                        => referentType,

                    _ => DataType.Unknown

                    // TODO optional types
                };

                exp.DataType.Fulfill(type);
                return new ExpressionResult(exp, resultVariable);
            }
            case IIdentifierNameExpressionSyntax exp:
            {
                var semantics = InferSemantics(exp);
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case NamedVariableNameSyntax sem:
                        var resultVariable = flow.Alias(sem.Symbol);
                        var type = flow.AliasType(sem.Symbol);
                        sem.Type.Fulfill(type);
                        return new ExpressionResult(exp, resultVariable);
                    case FunctionGroupNameSyntax _:
                        // Symbol already applied
                        return new ExpressionResult(exp);
                    case TypeNameSyntax _:
                    case NamespaceNameSyntax _:
                    case UnknownNameSyntax _:
                        return new ExpressionResult(exp);
                }
            }
            case IMissingNameSyntax exp:
                return new ExpressionResult(exp);
            case ISpecialTypeNameExpressionSyntax exp:
            {
                // Referenced symbol already assigned by SemanticsApplier
                return new ExpressionResult(exp, null);
            }
            case IGenericNameExpressionSyntax exp:
                throw new NotImplementedException("Generic name expressions are not implemented.");
            case IUnaryOperatorExpressionSyntax exp:
            {
                var @operator = exp.Operator;
                var result = InferType(exp.Operand, flow);
                DataType expType;
                var resultVariable = result.Variable;
                switch (@operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed(@operator);
                    case UnaryOperator.Not:
                        if (result.Type is BoolConstValueType boolType)
                            expType = boolType.Not();
                        else
                        {
                            expType = DataType.Bool;
                            result = AddImplicitConversionIfNeeded(result, expType, allowMoveOrFreeze: false, flow);
                            CheckTypeCompatibility(expType, exp.Operand);
                            flow.Drop(result.Variable);
                            resultVariable = null;
                        }
                        break;
                    case UnaryOperator.Minus:
                        switch (result.Type)
                        {
                            case IntegerConstValueType integerType:
                                expType = integerType.Negate();
                                break;
                            case CapabilityType<FixedSizeIntegerType> sizedIntegerType:
                                expType = sizedIntegerType.DeclaredType.WithSign().Type;
                                break;
                            case CapabilityType<BigIntegerType>:
                                // Even if unsigned before, it is signed now
                                expType = DataType.Int;
                                break;
                            case CapabilityType<PointerSizedIntegerType> pointerSizedIntegerType:
                                expType = pointerSizedIntegerType.DeclaredType.WithSign().Type;
                                break;
                            case UnknownType:
                                expType = DataType.Unknown;
                                break;
                            default:
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                    case UnaryOperator.Plus:
                        switch (result.Type)
                        {
                            case CapabilityType { DeclaredType: IntegerType }:
                            case IntegerConstValueType:
                            case UnknownType _:
                                expType = result.Type;
                                break;
                            default:
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                }

                exp.DataType.Fulfill(expType);
                return new ExpressionResult(exp, resultVariable);
            }
            case INewObjectExpressionSyntax exp:
            {
                var arguments = InferArgumentTypes(exp.Arguments, flow);
                // Type already set by semantic tree
                var constructingType = (exp.Type.NamedType as CapabilityType)?.BareType;
                ResultVariable? resultVariable;
                if (constructingType is null)
                {
                    // Symbol already assigned by SemanticsApplier
                    if (exp.ReferencedSymbol.Result is not null)
                        throw new InvalidOperationException("Symbol should match expected");
                    //exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType.Fulfill(DataType.Unknown);
                    resultVariable = CombineResults<ConstructorSymbol>(null, arguments, flow);
                    return new ExpressionResult(exp, resultVariable);
                }

                if (!constructingType.IsFullyKnown)
                {
                    // ERROR could not bind constructor
                    // Symbol already assigned by SemanticsApplier
                    if (exp.ReferencedSymbol.Result is not null)
                        throw new InvalidOperationException("Symbol should match expected");
                    //exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType.Fulfill(DataType.Unknown);
                    resultVariable = CombineResults<ConstructorSymbol>(null, arguments, flow);
                    return new ExpressionResult(exp, resultVariable);
                }

                if (constructingType is BareReferenceType { DeclaredType.IsAbstract: true })
                {
                    // ERROR: Cannot construct abstract type
                    // Symbol already assigned by SemanticsApplier
                    // Note: it may actually pick a constructor on the abstract type, so the symbol may not be null
                    exp.DataType.Fulfill(DataType.Unknown);
                    resultVariable = CombineResults<ConstructorSymbol>(null, arguments, flow);
                    return new ExpressionResult(exp, resultVariable);
                }

                var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new NotImplementedException();
                var constructorSymbols = symbolTrees.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedSet();
                var constructor = InferSymbol(exp, constructorSymbols, arguments, flow);
                return InferConstructorInvocationType(exp, constructor, arguments, flow);
            }
            case IForeachExpressionSyntax exp:
            {
                var declaredType = exp.Type?.NamedType;
                // TODO deal with result variable here
                var (expressionResult, variableType) = CheckForeachInType(declaredType, exp, flow);
                var symbol = NamedVariableSymbol.CreateLocal(containingSymbol, exp.IsMutableBinding, exp.VariableName, exp.DeclarationNumber.Result, variableType);
                exp.Symbol.Fulfill(symbol);
                symbolTreeBuilder.Add(symbol);
                // Declare the variable symbol and combine it with the `in` expression
                flow.Declare(symbol, expressionResult.Variable);

                // TODO check the break types
                var blockResult = InferBlockType(exp.Block, flow);

                flow.Drop(symbol);
                // TODO assign correct type to the expression
                exp.DataType.Fulfill(DataType.Void);
                return new ExpressionResult(exp, blockResult.Variable);
            }
            case IWhileExpressionSyntax exp:
            {
                CheckIndependentExpressionType(exp.Condition, DataType.Bool, allowMoveOrFreeze: false, flow);
                var result = InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.DataType.Fulfill(DataType.Void);
                return new ExpressionResult(exp, result.Variable);
            }
            case ILoopExpressionSyntax exp:
            {
                var result = InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.DataType.Fulfill(DataType.Void);
                return new ExpressionResult(exp, result.Variable);
            }
            case IInvocationExpressionSyntax exp:
                return InferInvocationType(exp, flow);
            case IUnsafeExpressionSyntax exp:
            {
                var result = InferType(exp.Expression, flow);
                exp.DataType.Fulfill(result.Type);
                return new ExpressionResult(exp, result.Variable);
            }
            case IIfExpressionSyntax exp:
            {
                CheckIndependentExpressionType(exp.Condition, DataType.Bool, allowMoveOrFreeze: false, flow);
                var elseClause = exp.ElseClause;
                // Even if there is no else clause, the if could be skipped. Still need to join
                FlowStateMutable elseFlow = flow.Fork();
                var thenResult = InferBlockType(exp.ThenBlock, flow);
                ExpressionResult? elseResult = null;
                switch (elseClause)
                {
                    default:
                        throw ExhaustiveMatch.Failed(elseClause);
                    case null:
                        break;
                    case IIfExpressionSyntax _:
                    case IBlockExpressionSyntax _:
                        var elseExpression = (IExpressionSyntax)elseClause;
                        elseResult = InferType(elseExpression, elseFlow!);
                        break;
                    case IResultStatementSyntax resultStatement:
                        elseResult = InferType(resultStatement.Expression, elseFlow!);
                        break;
                }
                DataType expType;
                if (elseResult is null)
                    expType = thenResult.Type.MakeOptional();
                else
                    // TODO unify the two types
                    expType = thenResult.Type;
                exp.DataType.Fulfill(expType);
                flow.Merge(elseFlow);
                var resultVariable = flow.Combine(thenResult.Variable, elseResult?.Variable, exp);
                return new ExpressionResult(exp, resultVariable);
            }
            case IMemberAccessExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow);
                var semantics = InferSemantics(exp);
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case FieldNameExpressionSyntax sem:
                        FieldSymbol fieldSymbol = sem.Symbol;
                        var contextType = exp.Context is ISelfExpressionSyntax self
                            ? self.Pseudotype.Assigned()
                            : contextResult.Type;
                        // Access must be applied first so it can account for independent generic parameters.
                        var type = fieldSymbol.Type.AccessedVia(contextType);
                        // Then type parameters can be replaced now that they have the correct access
                        if (contextType is NonEmptyType nonEmptyContext)
                            // resolve generic type fields
                            type = nonEmptyContext.ReplaceTypeParametersIn(type);
                        var resultVariable = flow.AccessMember(contextResult.Variable, type);
                        if (fieldSymbol.IsMutableBinding
                            && contextType is CapabilityType { Capability: var contextCapability }
                            && contextCapability == Capability.Identity)
                        {
                            // ERROR cannot access mutable field of identity reference (reported by semantic)
                            type = DataType.Unknown;
                        }
                        sem.Type.Fulfill(type);
                        return new ExpressionResult(exp, resultVariable);
                    case GetterNameSyntax sem:
                        var methodSymbol = Contextualize(contextResult.Type, sem.Symbol.Result.Yield()).Single();
                        var args = new ArgumentResults(contextResult, FixedList.Empty<ExpressionResult>());
                        return InferMethodInvocationType(exp, sem.Type, methodSymbol, args, flow);
                    case NamespaceNameSyntax _:
                        return new ExpressionResult(exp);
                    case MethodGroupNameSyntax _:
                    case TypeNameSyntax _:
                    case FunctionGroupNameSyntax _:
                    case SetterGroupNameSyntax _:
                    case InitializerGroupNameSyntax _:
                        throw new NotImplementedException();
                    case UnknownNameSyntax _:
                        // Error should already be reported
                        return new ExpressionResult(exp);
                }
            }
            case IBreakExpressionSyntax exp:
            {
                var result = InferType(exp.Value, flow);
                // TODO result variable needs to pass out of the loop
                return new ExpressionResult(exp);
            }
            case INextExpressionSyntax exp:
                return new ExpressionResult(exp);
            case IAssignmentExpressionSyntax exp:
            {
                var left = InferAssignmentTargetType(exp.LeftOperand, flow);
                var right = InferType(exp.RightOperand, flow);
                right = AddImplicitConversionIfNeeded(right, left.Type, allowMoveOrFreeze: false, flow);
                if (!left.Type.IsAssignableFrom(right.Type))
                    diagnostics.Add(TypeError.CannotImplicitlyConvert(file,
                        exp.RightOperand, right.Type, left.Type));
                var resultVariable = flow.Combine(left.Variable, right.Variable, exp);
                exp.DataType.Fulfill(left.Type);
                return new ExpressionResult(exp, resultVariable);
            }
            case ISelfExpressionSyntax exp:
            {
                // Semantics already assigned by SemanticsApplier
                var semantics = exp.Semantics.Result;
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case SelfExpressionSyntax sem:
                        var variableResult = flow.Alias(sem.Symbol);
                        var type = flow.AliasType(sem.Symbol);
                        sem.Type.Fulfill(type);
                        var psuedoType = sem.Symbol.Type;
                        if (psuedoType is not CapabilityTypeConstraint)
                            psuedoType = type;
                        sem.Pseudotype.Fulfill(psuedoType);
                        return new ExpressionResult(exp, variableResult);
                    case UnknownNameSyntax _:
                        // Error should already be reported
                        return new ExpressionResult(exp);
                }
            }
            case INoneLiteralExpressionSyntax exp:
                return new ExpressionResult(exp);
            case IBlockExpressionSyntax blockSyntax:
                return InferBlockType(blockSyntax, flow);
            case IConversionExpressionSyntax exp:
            {
                var result = InferType(exp.Referent, flow);
                var convertToType = exp.ConvertToType.NamedType!;
                if (!ExplicitConversionTypesAreCompatible(exp.Referent, exp.Operator == ConversionOperator.Safe, convertToType))
                    diagnostics.Add(TypeError.CannotExplicitlyConvert(file, exp.Referent, result.Type, convertToType));
                if (exp.Operator == ConversionOperator.Optional)
                    convertToType = convertToType.MakeOptional();
                exp.DataType.Fulfill(convertToType);
                flow.Restrict(result.Variable, convertToType);
                return new ExpressionResult(exp, result.Variable);
            }
            case IPatternMatchExpressionSyntax exp:
            {
                var referent = InferType(exp.Referent, flow);
                ResolveTypes(exp.Pattern, referent.Type, referent.Variable, flow);
                flow.Drop(referent.Variable);
                exp.DataType.Fulfill(DataType.Bool);
                return new ExpressionResult(exp);
            }
            case IAsyncBlockExpressionSyntax exp:
            {
                var result = InferBlockType(exp.Block, flow);
                exp.DataType.Fulfill(result.Type);
                return result;
            }
            case IAsyncStartExpressionSyntax exp:
            {
                // TODO these act like function calls holding results longer
                var result = InferType(exp.Expression, flow);
                exp.DataType.Fulfill(Intrinsic.PromiseOf(result.Type));
                return new ExpressionResult(exp, result.Variable);
            }
            case IAwaitExpressionSyntax exp:
            {
                var result = InferType(exp.Expression, flow);
                if (result.Type is not CapabilityType { DeclaredType: { } declaredType } promiseType
                    || !declaredType.Equals(Intrinsic.PromiseType))
                {
                    // ERROR cannot await type
                    exp.DataType.Fulfill(DataType.Unknown);
                    return new ExpressionResult(exp, result.Variable);
                }

                var resultType = promiseType.TypeArguments[0];
                exp.DataType.Fulfill(resultType);
                // TODO what is the effect on the flow typing
                return new ExpressionResult(exp, result.Variable);
            }
        }
    }

    private static ExpressionResult InferMoveExpressionType(
        IMoveExpressionSyntax exp,
        VariableNameSyntax semantics,
        FlowStateMutable flow)
    {
        var symbol = semantics.Symbol;
        var type = flow.Type(symbol);
        switch (type)
        {
            case CapabilityType capabilityType:
                type = capabilityType.IsTemporarilyIsolatedReference ? type : capabilityType.With(Capability.Isolated);
                flow.Move(symbol);
                break;
            case UnknownType:
                type = DataType.Unknown;
                break;
            default:
                throw new NotImplementedException("Non-moveable type can't be moved");
        }
        // Don't need to alias the symbol or union with result in flow because it will be moved

        // Symbol cannot be assigned by semantic tree because it doesn't have symbols for locals
        exp.ReferencedSymbol.Fulfill(symbol);

        if (semantics.Type.Result != type)
            throw new InvalidOperationException("Move expression type should be set by semantics");
        if (semantics is SelfExpressionSyntax selfExpression && !selfExpression.Pseudotype.Result.Equals(type))
            throw new InvalidOperationException("Move expression type should be set by semantics");
        if (exp.DataType.Result != type)
            throw new InvalidOperationException("Move expression type should be set by semantics");

        return new(exp);
    }

    private ExpressionResult InferFreezeExpressionType(
        IFreezeExpressionSyntax exp,
        VariableNameSyntax semantics,
        FlowStateMutable flow)
    {
        var symbol = semantics.Symbol;
        var type = flow.Type(symbol);
        switch (type)
        {
            case CapabilityType capabilityType:
                type = capabilityType.With(Capability.Constant);
                flow.Freeze(symbol);
                break;
            case UnknownType:
                type = DataType.Unknown;
                break;
            default:
                throw new NotImplementedException("Non-freezable type can't be frozen.");
        }
        // Alias not needed because it is already `const`

        // Symbol cannot be assigned by semantic tree because it doesn't have symbols for locals
        exp.ReferencedSymbol.Fulfill(symbol);

        if (semantics.Type.Result != type)
            throw new InvalidOperationException("Freeze expression type should be set by semantics");
        if (semantics is SelfExpressionSyntax selfExpression && !selfExpression.Pseudotype.Result.Equals(type))
            throw new InvalidOperationException("Freeze expression type should be set by semantics");
        if (exp.DataType.Result != type)
            throw new InvalidOperationException("Freeze expression type should be set by semantics");

        return new(exp);
    }

    private void ResolveTypes(
        IPatternSyntax pattern,
        DataType valueType,
        ResultVariable? resultVariable,
        FlowStateMutable flow,
        bool? isMutableBinding = false)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case IBindingContextPatternSyntax pat:
            {
                // Named type already assigned by SemanticsApplier
                valueType = pat.Type?.NamedType ?? valueType;
                ResolveTypes(pat.Pattern, valueType, resultVariable, flow, pat.IsMutableBinding);
                break;
            }
            case IBindingPatternSyntax pat:
            {
                if (isMutableBinding is null) throw new UnreachableException("Binding pattern outside of binding context");
                var symbol = NamedVariableSymbol.CreateLocal(containingSymbol, isMutableBinding.Value, pat.Name,
                    pat.DeclarationNumber.Result, valueType);
                pat.Symbol.Fulfill(symbol);
                symbolTreeBuilder.Add(symbol);
                // Declare the symbol
                flow.Declare(symbol, resultVariable);
                break;
            }
            case IOptionalPatternSyntax pat:
            {
                if (valueType is OptionalType optionalType)
                    valueType = optionalType.Referent;
                // else ERROR optional pattern on non-optional type (reported by semantics)

                ResolveTypes(pat.Pattern, valueType, resultVariable, flow, isMutableBinding);
                break;
            }
        }
    }

    private ArgumentResults InferArgumentTypes(IFixedList<IExpressionSyntax> arguments, FlowStateMutable flow)
        => InferArgumentTypes(null, arguments, flow);

    private ArgumentResults InferArgumentTypes(
        ExpressionResult? selfArgument,
        IFixedList<IExpressionSyntax> arguments,
        FlowStateMutable flow)
    {
        // Give each argument a distinct result
        var inferences = new List<ExpressionResult>();
        foreach (var argument in arguments)
        {
            var result = InferType(argument, flow);
            inferences.Add(result);
        }

        return new ArgumentResults(selfArgument, inferences);
    }

    private void CheckTypes(ArgumentResults arguments, IEnumerable<ParameterType> expectedTypes, FlowStateMutable flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.EquiZip(expectedTypes))
        {
            AddImplicitConversionIfNeeded(arg, parameter, allowMoveOrFreeze: false, flow);
            CheckTypeCompatibility(parameter.Type, arg.Syntax);
            // TODO update the expression result
        }
    }

    private static bool TypesAreCompatible(ArgumentResults arguments, IEnumerable<ParameterType> expectedTypes, FlowStateMutable flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.EquiZip(expectedTypes))
            if (!TypesAreCompatible(arg, parameter, flow))
                return false;

        return true;
    }

    private static bool TypesAreCompatible(ExpressionResult arg, ParameterType parameter, FlowStateMutable flow, bool isSelf = false)
    {
        var argType = arg.Type;
        var priorConversion = arg.Syntax.ImplicitConversion;
        var conversion = CreateImplicitConversion(parameter.Type, arg.Type, arg.Variable, allowMoveOrFreeze: isSelf,
            priorConversion, flow, out _, enact: false);
        if (conversion is not null)
        {
            argType = conversion.Apply(argType);
            priorConversion = conversion;
        }

        if (isSelf)
        {
            conversion = CreateImplicitMoveConversion(argType, arg.Syntax, arg.Variable, parameter,
                flow, enact: false, priorConversion);

            if (conversion is not null)
            {
                argType = conversion.Apply(argType);
                priorConversion = conversion;
            }

            conversion = CreateImplicitFreezeConversion(argType, arg.Syntax, arg.Variable, parameter,
                flow, enact: false, priorConversion);

            if (conversion is not null)
                argType = conversion.Apply(argType);
        }
        return parameter.Type.IsAssignableFrom(argType);
    }

    /// <param name="invocable">The symbol being invoked or <see langword="null"/> if not fully known.</param>
    private static ResultVariable? CombineResults<TSymbol>(
        Contextualized<TSymbol>? invocable,
        ArgumentResults results,
        FlowStateMutable flow)
        where TSymbol : InvocableSymbol
        => CombineResults(invocable?.SelfParameterType, invocable?.ParameterTypes, invocable?.ReturnType, results, flow);

    /// <param name="function">The function type being invoked or <see langword="null"/> if not fully known.</param>
    private static ResultVariable? CombineResults(
        FunctionType? function,
        ArgumentResults results,
        FlowStateMutable flow)
        => CombineResults(null, function?.Parameters, function?.Return, results, flow);

    private static ResultVariable? CombineResults(
        SelfParameterType? selfParameterType,
        IFixedList<ParameterType>? parameterTypes,
        ReturnType? returnType,
        ArgumentResults results,
        FlowStateMutable flow)
    {
        var resultsToDrop = new List<ResultVariable>();
        var resultType = returnType?.Type;
        if (results.Self is not null)
            resultType = resultType?.ReplaceSelfWith(results.Self.Type);
        // TODO if the return type doesn't have sharing tracked, this returns null. It might be necessary
        // to combine results even in that case and then drop the result from the flow.
        var returnResult = flow.CreateReturnResultVariable(resultType);

        if (results.Self is not null)
            CombineParameterIntoReturn(results.Self, selfParameterType, returnResult, resultsToDrop, flow);

        foreach (var (argument, i) in results.Arguments.Enumerate())
            CombineParameterIntoReturn(argument, parameterTypes?[i], returnResult, resultsToDrop, flow);

        foreach (var resultVariable in resultsToDrop)
            flow.Drop(resultVariable);

        return returnResult;
    }

    private static void CombineParameterIntoReturn<TParameterType>(
        ExpressionResult argument,
        TParameterType? parameterType,
        ResultVariable? returnResult,
        List<ResultVariable> resultsToDrop,
        FlowStateMutable flow)
        where TParameterType : struct, IParameterType
    {
        if (argument.Variable is null)
            return;

        var isLent = parameterType?.IsLent ?? false;
        // Lent arguments are not combined.
        if (!isLent)
        {
            if (returnResult is not null)
                flow.SharingUnion(returnResult, argument.Variable, addCannotUnionError: null);
            flow.Drop(argument.Variable);
        }
        else
            resultsToDrop.Add(argument.Variable);
    }

    private ExpressionResult InferAssignmentTargetType(
        IAssignableExpressionSyntax expression,
        FlowStateMutable flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IMemberAccessExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow);
                var semantics = InferSemantics(exp);
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case FieldNameExpressionSyntax sem:
                        var contextType = contextResult.Type;
                        if (contextType is CapabilityType { AllowsWrite: false, AllowsInit: false } capabilityType)
                            diagnostics.Add(TypeError.CannotAssignFieldOfReadOnly(file, expression.Span, capabilityType));
                        // Check for assigning into `let` fields (skip self fields in constructors and initializers)
                        if (exp.Context is not { ConvertedDataType: CapabilityType { AllowsInit: true } }
                            && sem.Symbol is { IsMutableBinding: false, Name: IdentifierName name })
                            diagnostics.Add(OtherSemanticError.CannotAssignImmutableField(file, exp.Span, name));
                        var type = sem.Symbol.Type.AccessedVia(contextType);
                        if (contextType is NonEmptyType nonEmptyContext)
                            type = nonEmptyContext.ReplaceTypeParametersIn(type);
                        var resultVariable = flow.AccessMember(contextResult.Variable, type);
                        sem.Type.Fulfill(type);
                        return new(exp, resultVariable);
                    case SetterGroupNameSyntax sem:
                        // TODO this incorrectly assume there will only be one setter
                        var setterSymbol = sem.Symbols.Single();
                        sem.Symbol.Fulfill(setterSymbol);
                        // TODO this doesn't correctly check argument types
                        var parameterType = setterSymbol.Parameters.Single().Type;
                        sem.Type.Fulfill(parameterType);
                        return new(exp, contextResult.Variable);
                    case UnknownNameSyntax _:
                        return new(exp);
                    case MethodGroupNameSyntax _:
                    case NamespaceNameSyntax _:
                    case TypeNameSyntax _:
                    case FunctionGroupNameSyntax _:
                    case GetterNameSyntax _:
                    case InitializerGroupNameSyntax _:
                        throw new NotImplementedException();
                }
            }
            case IIdentifierNameExpressionSyntax exp:
            {
                var semantics = InferSemantics(exp);
                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case NamedVariableNameSyntax sem:
                        var variableSymbol = sem.Symbol;
                        sem.Type.Fulfill(flow.Type(variableSymbol));
                        // TODO this doesn't seem correct, we don't have to alias the old value
                        var resultVariable = flow.Alias(variableSymbol);
                        return new(exp, resultVariable);
                    case FunctionGroupNameSyntax _:
                    case TypeNameSyntax _:
                    case NamespaceNameSyntax _:
                        throw new NotImplementedException("Raise error about assigning into a non-variable");
                    case UnknownNameSyntax _:
                        return new(exp);
                }
            }
            case IMissingNameSyntax exp:
                return new(exp);
        }
    }

    private ExpressionResult InferInvocationType(
        IInvocationExpressionSyntax invocation,
        FlowStateMutable flow)
    {
        // This could actually be any of the following since the parser can't distinguish them:
        // * Regular function invocation
        // * Associated function invocation
        // * Namespaced function invocation
        // * Method invocation
        // * Function type invocation
        // * Initializer invocation

        ArgumentResults args;
        FunctionType? functionType = null;
        switch (invocation.Expression)
        {
            case IMemberAccessExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow);
                var semantics = InferSemantics(exp);

                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case MethodGroupNameSyntax sem:
                    {
                        // Make sure to infer argument type *after* the context type
                        args = InferArgumentTypes(contextResult, invocation.Arguments, flow);
                        var method = InferSymbol(invocation, sem.Symbols, args, flow);
                        // Symbol already applied by SemanticsApplier
                        //sem.Symbol.Fulfill(method?.Symbol);
                        return InferMethodInvocationType(invocation, invocation.DataType, method, args, flow);
                    }
                    case FunctionGroupNameSyntax sem:
                        // Make sure to infer argument type *after* the context type
                        args = InferArgumentTypes(null, invocation.Arguments, flow);
                        functionType = InferSymbol(invocation, sem.Symbol, sem.Symbols, args, flow);
                        break;
                    case InitializerGroupNameSyntax sem:
                        // Make sure to infer argument type *after the context type
                        args = InferArgumentTypes(null, invocation.Arguments, flow);
                        functionType = InferSymbol(invocation, sem.Symbol, sem.Symbols, args, flow);
                        break;
                    case FieldNameExpressionSyntax sem:
                        // Make sure to infer argument type *after* the context type
                        args = InferArgumentTypes(null, invocation.Arguments, flow);
                        if (sem.Symbol.Type is FunctionType fType)
                        {
                            invocation.ReferencedSymbol.Fulfill(sem.Symbol);
                            sem.Type.Fulfill(fType);
                            functionType = fType;
                        }
                        else
                        {
                            // Matches field, but not a function type. This is an error. Just mark
                            // the types as unknown.
                            invocation.ReferencedSymbol.Fulfill(null);
                            sem.Type.Fulfill(DataType.Unknown);
                        }
                        break;
                    case UnknownNameSyntax _:
                        // Make sure to infer argument type *after* the context type
                        args = InferArgumentTypes(null, invocation.Arguments, flow);
                        invocation.ReferencedSymbol.Fulfill(null);
                        break;
                    case NamespaceNameSyntax _:
                    case TypeNameSyntax _:
                    case GetterNameSyntax _:
                    case SetterGroupNameSyntax _:
                        throw new NotImplementedException(semantics.GetType().Name);
                }
                break;
            }
            case IIdentifierNameExpressionSyntax exp:
            {
                var semantics = InferSemantics(exp);
                args = InferArgumentTypes(invocation.Arguments, flow);

                switch (semantics)
                {
                    default:
                        throw ExhaustiveMatch.Failed(semantics);
                    case NamedVariableNameSyntax sem:
                        var variableSymbol = sem.Symbol;
                        var variableType = flow.Type(variableSymbol);
                        if (variableType is FunctionType variableFunctionType)
                        {
                            functionType = variableFunctionType;
                            invocation.ReferencedSymbol.Fulfill(null);
                            sem.Type.Fulfill(functionType);
                        }
                        else
                        {
                            // Matches variable, but not a function type. This is an error. Just mark
                            // the type as unknown.
                            sem.Type.Fulfill(DataType.Unknown);
                            invocation.ReferencedSymbol.Fulfill(null);
                        }
                        break;
                    case FunctionGroupNameSyntax sem:
                        functionType = InferSymbol(invocation, sem.Symbol, sem.Symbols, args, flow);
                        break;
                    case TypeNameSyntax sem:
                        var initializerSymbols = symbolTrees.Children(sem.Symbol)
                                                            .OfType<InitializerSymbol>()
                                                            .Where(s => s.Name is null)
                                                            .ToFixedSet();
                        var initializerSymbol = new Promise<InitializerSymbol?>();
                        functionType = InferSymbol(invocation, initializerSymbol, initializerSymbols, args, flow);
                        break;
                    case NamespaceNameSyntax _:
                        // No type for function
                        break;
                    case UnknownNameSyntax sem:
                        invocation.ReferencedSymbol.Fulfill(null);
                        break;
                }
                break;
            }
            case IMissingNameSyntax exp:
                args = InferArgumentTypes(invocation.Arguments, flow);
                invocation.ReferencedSymbol.Fulfill(null);
                break;
            default:
            {
                var contextResult = InferType(invocation.Expression, flow);
                // Make sure to infer argument type *after* the context type
                args = InferArgumentTypes(contextResult, invocation.Arguments, flow);

                functionType = contextResult.Type as FunctionType;
                invocation.ReferencedSymbol.Fulfill(null);
                break;
            }
        }

        return InferFunctionInvocationType(invocation, functionType, args, flow);
    }

    private ExpressionResult InferMethodInvocationType(
        IExpressionSyntax invocation,
        Promise<DataType> returnType,
        Contextualized<MethodSymbol>? method,
        ArgumentResults arguments,
        FlowStateMutable flow)
    {
        if (method is not null)
        {
            var selfParamType = method.SelfParameterType!.Value;
            var selfParamUpperBound = selfParamType.ToUpperBound();
            var selfResult = arguments.Self.Assigned();
            selfResult = AddImplicitConversionIfNeeded(selfResult, selfParamUpperBound, allowMoveOrFreeze: true, flow);
            AddImplicitMoveIfNeeded(selfResult, selfParamType, flow);
            AddImplicitFreezeIfNeeded(selfResult, selfParamType, flow);
            CheckTypeCompatibility(selfParamUpperBound.Type, selfResult.Syntax);
            CheckTypes(arguments, method.ParameterTypes, flow);
            arguments = arguments with { Self = selfResult };
            // TODO does flow typing need to be applied?
            returnType.Fulfill(method.ReturnType.Type.ReplaceSelfWith(selfResult.Type));
        }
        else
        {
            returnType.Fulfill(DataType.Unknown);
        }

        var resultVariable = CombineResults(method, arguments, flow);
        flow.Restrict(resultVariable, returnType.Assigned());
        return new(invocation, resultVariable);
    }

    private ExpressionResult InferConstructorInvocationType(
        INewObjectExpressionSyntax invocation,
        Contextualized<ConstructorSymbol>? constructor,
        ArgumentResults arguments,
        FlowStateMutable flow)
    {
        if (constructor is not null)
        {
            CheckTypes(arguments, constructor.ParameterTypes, flow);
            var returnType = constructor.ReturnType.Type;
            invocation.DataType.Fulfill(returnType);
        }
        else
            invocation.DataType.Fulfill(DataType.Unknown);

        var resultVariable = CombineResults(constructor, arguments, flow);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        return new(invocation, resultVariable);
    }

    private static void AddImplicitMoveIfNeeded(
        ExpressionResult selfArg,
        SelfParameterType selfParamType,
        FlowStateMutable flow)
    {
        var conversion = CreateImplicitMoveConversion(selfArg.Type, selfArg.Syntax, selfArg.Variable,
            selfParamType.ToUpperBound(), flow, enact: true, selfArg.Syntax.ImplicitConversion);
        if (conversion is not null)
            selfArg.Syntax.AddConversion(conversion);
    }

    private static MoveConversion? CreateImplicitMoveConversion(
        DataType selfArgType,
        IExpressionSyntax selfArgSyntax,
        ResultVariable? selfArgVariable,
        ParameterType selfParam,
        FlowStateMutable flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit moves never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `iso`.
        if (selfParam.IsLent) return null;

        if (selfParam.Type is not CapabilityType { IsIsolatedReference: true } toType
            || selfArgType is not CapabilityType { AllowsRecoverIsolation: true } fromType)
            return null;

        if (!toType.BareType.IsAssignableFrom(toType.AllowsWrite, fromType.BareType)) return null;

        if (selfArgSyntax is not INameExpressionSyntax { ReferencedSymbol.Result: NamedVariableSymbol { IsLocal: true } symbol }
            || !flow.IsIsolatedExceptFor(symbol, selfArgVariable))
            return null;

        if (enact)
            flow.Move(symbol);

        return new MoveConversion(priorConversion, ConversionKind.Implicit);
    }

    private static void AddImplicitFreezeIfNeeded(
        ExpressionResult selfArg,
        SelfParameterType selfParamType,
        FlowStateMutable flow)
    {
        var conversion = CreateImplicitFreezeConversion(selfArg.Type, selfArg.Syntax, selfArg.Variable,
            selfParamType.ToUpperBound(), flow, enact: true, selfArg.Syntax.ImplicitConversion);
        if (conversion is not null)
            selfArg.Syntax.AddConversion(conversion);
    }

    private static FreezeConversion? CreateImplicitFreezeConversion(
        DataType selfArgType,
        IExpressionSyntax selfArgSyntax,
        ResultVariable? selfArgVariable,
        ParameterType selfParam,
        FlowStateMutable flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit freezes never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `const`
        if (selfParam.IsLent) return null;

        if (selfParam.Type is not CapabilityType { IsConstantReference: true } toType
            || selfArgType is not CapabilityType { AllowsFreeze: true } fromType)
            return null;

        if (!toType.BareType.IsAssignableFrom(toType.AllowsWrite, fromType.BareType)) return null;

        if (selfArgSyntax is not INameExpressionSyntax { ReferencedSymbol.Result: NamedVariableSymbol { IsLocal: true } symbol }
            || !flow.CanFreezeExceptFor(symbol, selfArgVariable))
            return null;

        if (enact)
            flow.Freeze(symbol);

        return new FreezeConversion(priorConversion, ConversionKind.Implicit);
    }

    private ExpressionResult InferFunctionInvocationType(
        IInvocationExpressionSyntax invocation,
        FunctionType? functionType,
        ArgumentResults arguments,
        FlowStateMutable flow)
    {
        if (functionType is not null)
        {
            CheckTypes(arguments, functionType.Parameters, flow);
            var returnType = functionType.Return.Type;
            // TODO doesn't this type need to be modified by flow typing?
            invocation.DataType.Fulfill(returnType);
        }
        else
            invocation.DataType.Fulfill(DataType.Unknown);

        var resultVariable = CombineResults(functionType, arguments, flow);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        return new(invocation, resultVariable);
    }

    private ExpressionResult InferBlockType(
        IBlockOrResultSyntax blockOrResult,
        FlowStateMutable flow)
    {
        switch (blockOrResult)
        {
            default:
                throw ExhaustiveMatch.Failed(blockOrResult);
            case IBlockExpressionSyntax block:
                ExpressionResult? blockResult = null;
                foreach (var statement in block.Statements)
                {
                    var resultType = ResolveTypes(statement, flow);
                    // Always resolve types even if there is already a block type
                    blockResult ??= resultType;
                }

                // Drop any variables in the scope
                foreach (var variableDeclaration in block.Statements.OfType<IVariableDeclarationStatementSyntax>())
                    flow.Drop(variableDeclaration.Symbol.Result);

                // If there was no result expression, then the block type is void
                var blockType = blockResult?.Type ?? DataType.Void;
                block.DataType.Fulfill(blockType);
                return new(block, blockResult?.Variable);
            case IResultStatementSyntax result:
                // Can't return an ExpressionResult for the statement, return it for the expression instead
                return InferType(result.Expression, flow);
        }
    }

    private static IIdentifierNameExpressionSyntaxSemantics InferSemantics(IIdentifierNameExpressionSyntax expression)
        // Semantics already assigned by SemanticsApplier
        => expression.Semantics.Result;

    private static IMemberAccessSyntaxSemantics InferSemantics(IMemberAccessExpressionSyntax expression)
        // Semantics already assigned by SemanticsApplier
        => expression.Semantics.Result;

    private FunctionType? InferSymbol<TSymbol>(
        IInvocationExpressionSyntax invocation,
        Promise<TSymbol?> promise,
        IFixedSet<TSymbol> matchingSymbols,
        ArgumentResults arguments,
        FlowStateMutable flow)
        where TSymbol : FunctionOrInitializerSymbol
    {
        var validOverloads = SelectOverload(null, matchingSymbols, arguments, flow);
        switch (validOverloads.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(file, invocation));
                // Already assigned by SemanticsApplier
                //promise.Fulfill(null);
                //invocation.ReferencedSymbol.Fulfill(null);
                return null;
            case 1:
                var overload = validOverloads.Single();
                // May already be assigned by SemanticsApplier
                if (!promise.IsFulfilled)
                {
                    promise.Fulfill(overload.Symbol);
                    invocation.ReferencedSymbol.Fulfill(overload.Symbol);
                }
                var functionType = new FunctionType(overload.ParameterTypes, overload.ReturnType);
                return functionType;
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, invocation));
                promise.Fulfill(null);
                invocation.ReferencedSymbol.Fulfill(null);
                return null;
        }
    }

    private Contextualized<MethodSymbol>? InferSymbol(
        IInvocationExpressionSyntax invocation,
        IFixedSet<MethodSymbol> methodSymbols,
        ArgumentResults arguments,
        FlowStateMutable flow)
    {
        var selfResult = arguments.Self.Assigned();
        var selfArgumentType = selfResult.Type;

        Contextualized<MethodSymbol>? method = null;
        var validOverloads = SelectOverload(selfArgumentType, methodSymbols, arguments, flow);
        switch (validOverloads.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMethod(file, invocation));
                invocation.ReferencedSymbol.Fulfill(null);
                break;
            case 1:
                method = validOverloads.Single();
                invocation.ReferencedSymbol.Fulfill(method.Symbol);
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousMethodCall(file, invocation));
                invocation.ReferencedSymbol.Fulfill(null);
                break;
        }

        return method;
    }

    private static Contextualized<ConstructorSymbol>? InferSymbol(
        INewObjectExpressionSyntax invocation,
        IFixedSet<ConstructorSymbol>? constructorSymbols,
        ArgumentResults arguments,
        FlowStateMutable flow)
    {
        Contextualized<ConstructorSymbol>? constructor = null;
        if (constructorSymbols is not null)
        {
            var validOverloads = SelectOverload(invocation.Type.NamedType.Assigned(), constructorSymbols, arguments, flow);
            switch (validOverloads.Count)
            {
                case 1:
                    constructor = validOverloads.Single();
                    // Symbol already assigned by SemanticsApplier
                    if (invocation.ReferencedSymbol.Result != constructor.Symbol)
                        throw new InvalidOperationException("Symbol should match expected");
                    break;
                default:
                    // Symbol already assigned by SemanticsApplier
                    if (invocation.ReferencedSymbol.Result is not null)
                        throw new InvalidOperationException("Symbol should match expected");
                    break;
            }
        }
        else
        {
            // Symbol already assigned by SemanticsApplier
            if (invocation.ReferencedSymbol.Result is not null)
                throw new InvalidOperationException("Symbol should match expected");
            invocation.ReferencedSymbol.Fulfill(null);
        }

        return constructor;
    }

    /// <summary>
    /// Eventually, a `foreach` `in` expression will just be a regular expression. However, at the
    /// moment, there isn't enough of the language to implement range expressions. So this
    /// check handles range expressions in the specific case of `foreach` only. It marks them
    /// as having the same type as the range endpoints.
    /// </summary>
    private (ExpressionResult, DataType) CheckForeachInType(
        DataType? declaredType,
        IForeachExpressionSyntax exp,
        FlowStateMutable flow)
    {
        var inExpression = exp.InExpression;
        var result = InferType(inExpression, flow);

        if (result.Type is UnknownType)
            return (result, declaredType ?? DataType.Unknown);

        if (result.Type is not NonEmptyType iterableType)
            return ForeachNoIterateOrNextMethod();

        var iterableSymbol = LookupSymbolForType(iterableType);
        if (iterableSymbol is null) return ForeachNoIterateOrNextMethod();

        var iterateMethod = symbolTrees.Children(iterableSymbol).OfType<MethodSymbol>()
                                       .SingleOrDefault(s => s.Name == "iterate" && s.Arity == 0 && s.Return.Type is NonEmptyType);

        exp.IterateMethod.Fulfill(iterateMethod);

        var iteratorType = iterateMethod is not null ? iterableType.ReplaceTypeParametersIn(iterateMethod.Return.Type) : iterableType;
        var iteratorSymbol = LookupSymbolForType(iteratorType);
        if (iteratorSymbol is null) return ForeachNoIterateOrNextMethod();

        var nextMethod = symbolTrees.Children(iteratorSymbol).OfType<MethodSymbol>()
                                    .SingleOrDefault(s => s.Name == "next" && s.Arity == 0 && s.Return.Type is OptionalType);

        if (nextMethod is null)
        {
            if (iterateMethod is null) return ForeachNoIterateOrNextMethod();
            // ERROR reported by semantic tree
            //diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(file, inExpression, iterableType));
            return (result, declaredType ?? DataType.Unknown);
        }

        exp.NextMethod.Fulfill(nextMethod);

        // iteratorType is NonEmptyType because it has a `next()` method
        DataType iteratedType = nextMethod.Return.Type is OptionalType optionalType
            ? ((NonEmptyType)iteratorType).ReplaceTypeParametersIn(optionalType.Referent)
            : throw new UnreachableException();

        if (declaredType is not null)
        {
            var conversion = CreateImplicitConversion(declaredType, iteratedType, null, allowMoveOrFreeze: false,
                inExpression.ImplicitConversion, flow,
                out _, enact: false);
            iteratedType = conversion is null ? iteratedType : conversion.Apply(iteratedType);
            if (!declaredType.IsAssignableFrom(iteratedType))
                // TODO this error needs to be specific to iterators (e.g. not "Cannot convert expression `0..<count` of type `int` to type `size`")
                diagnostics.Add(TypeError.CannotImplicitlyConvert(file, inExpression, iteratedType, declaredType));
        }

        var variableType = declaredType ?? iteratedType.ToNonConstValueType();
        return (result, variableType);

        (ExpressionResult, DataType) ForeachNoIterateOrNextMethod()
        {
            // ERROR reported by semantic tree
            //diagnostics.Add(OtherSemanticError.ForeachNoIterateOrNextMethod(file, inExpression, result.Type));
            return (result, declaredType ?? DataType.Unknown);
        }
    }

    private void CheckTypeCompatibility(DataType type, IExpressionSyntax arg)
    {
        var fromType = arg.ConvertedDataType.Assigned();
        if (!type.IsAssignableFrom(fromType))
            diagnostics.Add(TypeError.CannotImplicitlyConvert(file, arg, fromType, type));
    }

    private DataType InferNumericOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowStateMutable flow)
    {
        var leftType = leftOperand.Type.Assigned();
        var rightType = rightOperand.Type.Assigned();
        var commonType = leftType.NumericOperatorCommonType(rightType);
        if (commonType is null) return DataType.Unknown;

        AddImplicitConversionIfNeeded(leftOperand, commonType, allowMoveOrFreeze: false, flow);
        AddImplicitConversionIfNeeded(rightOperand, commonType, allowMoveOrFreeze: false, flow);
        return commonType;
    }

    private DataType InferComparisonOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowStateMutable flow)
    {
        var leftType = leftOperand.Type.Assigned();
        var rightType = rightOperand.Type.Assigned();
        var commonType = leftType.NumericOperatorCommonType(rightType);
        if (commonType is null) return DataType.Unknown;

        AddImplicitConversionIfNeeded(leftOperand, commonType, allowMoveOrFreeze: false, flow);
        AddImplicitConversionIfNeeded(rightOperand, commonType, allowMoveOrFreeze: false, flow);
        return DataType.Bool;
    }

    /// <summary>
    /// Check if two expressions are `id` types and comparable with `==` and `!=`.
    /// </summary>
    private static DataType InferReferenceEqualityOperatorType(
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand)
    {
        if (leftOperand.ConvertedDataType is CapabilityType { IsIdentityReference: true } left
           && rightOperand.ConvertedDataType is CapabilityType { IsIdentityReference: true } right)
            return DataType.Bool;
        return DataType.Unknown;
    }

    private DataType InferRangeOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowStateMutable flow)
    {
        AddImplicitConversionIfNeeded(leftOperand, DataType.Int, allowMoveOrFreeze: false, flow);
        AddImplicitConversionIfNeeded(rightOperand, DataType.Int, allowMoveOrFreeze: false, flow);
        return rangeSymbol?.DeclaresType.With(Capability.Constant, FixedList.Empty<DataType>())
               ?? DataType.Unknown;
    }

    private static bool ExplicitConversionTypesAreCompatible(IExpressionSyntax expression, bool safeOnly, DataType convertToType)
    {
        return (expression.ConvertedDataType.Assigned(), convertToType) switch
        {
            // Safe conversions
            (CapabilityType<BoolType>, CapabilityType { DeclaredType: IntegerType }) => true,
            (BoolConstValueType, CapabilityType { DeclaredType: IntegerType }) => true,
            (CapabilityType { DeclaredType: IntegerType { IsSigned: false } }, CapabilityType { DeclaredType: BigIntegerType }) => true,
            (CapabilityType { DeclaredType: IntegerType }, CapabilityType { DeclaredType: BigIntegerType { IsSigned: true } }) => true,
            (CapabilityType<FixedSizeIntegerType> from, CapabilityType<FixedSizeIntegerType> to)
                when from.DeclaredType.Bits < to.DeclaredType.Bits
                     || (from.DeclaredType.Bits == to.DeclaredType.Bits && from.DeclaredType.IsSigned == to.DeclaredType.IsSigned)
                => true,
            // TODO conversions for constants
            // Unsafe conversions
            (CapabilityType { DeclaredType: IntegerType }, CapabilityType { DeclaredType: IntegerType }) => !safeOnly,
            _ => convertToType.IsAssignableFrom(expression.ConvertedDataType),
        };
    }

    private static IFixedSet<Contextualized<TSymbol>> SelectOverload<TSymbol>(
        DataType? contextType,
        IFixedSet<TSymbol> symbols,
        ArgumentResults arguments,
        FlowStateMutable flow)
        where TSymbol : InvocableSymbol
    {
        var contextualizedSymbols = CompatibleOverloads(contextType, symbols, arguments, flow).ToFixedSet();
        // TODO Select most specific match
        return contextualizedSymbols;
    }

    private static IEnumerable<Contextualized<TSymbol>> CompatibleOverloads<TSymbol>(
        DataType? contextType,
        IFixedSet<TSymbol> symbols,
        ArgumentResults arguments,
        FlowStateMutable flow)
        where TSymbol : InvocableSymbol
    {
        var contextualizedSymbols = Contextualize(contextType, symbols);

        // Filter down to symbols that could possibly match
        contextualizedSymbols = contextualizedSymbols.Where(s =>
        {
            // Arity depends on the contextualized symbols because parameters can drop out with `void`
            if (s.Arity != arguments.Arity) return false;
            // Is self arg compatible?
            if (s.SelfParameterType is SelfParameterType selfParameterType
                && (arguments.Self is null || !TypesAreCompatible(arguments.Self, selfParameterType.ToUpperBound(), flow, isSelf: true)))
                return false;
            // Are arguments compatible?
            return TypesAreCompatible(arguments, s.ParameterTypes, flow);
        }).ToFixedSet();

        return contextualizedSymbols;
    }

    private static IEnumerable<Contextualized<TSymbol>> Contextualize<TSymbol>(
        DataType? contextType,
        IEnumerable<TSymbol> symbols)
        where TSymbol : InvocableSymbol
    {
        if (contextType is NonEmptyType context)
            return symbols.Select(s =>
            {
                var effectiveSelfType = context.ReplaceTypeParametersIn(SelfParameterTypeOrNull(s));
                var effectiveParameterTypes = s.Parameters.Select(context.ReplaceTypeParametersIn)
                                               .Where(p => p.Type is NonEmptyType).ToFixedList();
                var effectiveReturnType = context.ReplaceTypeParametersIn(s.Return);
                return new Contextualized<TSymbol>(s, effectiveSelfType, effectiveParameterTypes, effectiveReturnType);
            });

        return symbols.Select(s => new Contextualized<TSymbol>(s, SelfParameterTypeOrNull(s), s.Parameters, s.Return));
    }

    private static SelfParameterType? SelfParameterTypeOrNull(InvocableSymbol symbol)
    {
        if (symbol is MethodSymbol { SelfParameterType: var selfType })
            return selfType;
        return null;
    }

    #region LookupSymbolForType
    // TODO move these methods somewhere else, and possibly make them more efficient
    private TypeSymbol? LookupSymbolForType(DataType dataType)
    {
        return dataType switch
        {
            UnknownType _ => null,
            ConstValueType _ => null,
            OptionalType _ => null,
            EmptyType _ => null,
            FunctionType _ => null,
            CapabilityType t => LookupSymbolForType(t),
            GenericParameterType t => LookupSymbolForType(t),
            ViewpointType t => LookupSymbolForType(t.Referent),
            _ => throw ExhaustiveMatch.Failed(dataType),
        };
    }

    private TypeSymbol LookupSymbolForType(CapabilityType type)
        => LookupSymbolForType(type.DeclaredType);

    private TypeSymbol LookupSymbolForType(DeclaredType type)
    {
        return type switch
        {
            DeclaredReferenceType t => LookupSymbolForType(t),
            DeclaredValueType t => LookupSymbolForType(t),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private TypeSymbol LookupSymbolForType(DeclaredReferenceType type)
    {
        return type switch
        {
            AnyType t => LookupSymbolForType(t),
            ObjectType t => LookupSymbolForType((IDeclaredUserType)t),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private TypeSymbol LookupSymbolForType(DeclaredValueType type)
    {
        return type switch
        {
            SimpleType t => LookupSymbolForType(t),
            StructType t => LookupSymbolForType((IDeclaredUserType)t),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private PrimitiveTypeSymbol LookupSymbolForType(SimpleType type)
        => symbolTrees.PrimitiveSymbolTree.LookupSymbolForType(type);

    private PrimitiveTypeSymbol LookupSymbolForType(AnyType type)
        => symbolTrees.PrimitiveSymbolTree.LookupSymbolForType(type);

    private TypeSymbol LookupSymbolForType(IDeclaredUserType type)
    {
        var contextSymbols = symbolTrees.Packages.SafeCast<Symbol>();
        foreach (var name in type.ContainingNamespace.Segments)
            contextSymbols = contextSymbols.SelectMany(symbolTrees.Children)
                                           .Where(s => s.Name == name);

        return contextSymbols.SelectMany(symbolTrees.Children).OfType<TypeSymbol>()
                             .Single(s => s.Name == type.Name);
    }

    private TypeSymbol LookupSymbolForType(GenericParameterType genericParameterType)
    {
        var declaringTypeSymbol = LookupSymbolForType(genericParameterType.DeclaringType);
        return symbolTrees.Children(declaringTypeSymbol).OfType<TypeSymbol>()
                   .Single(s => s.Name == genericParameterType.Name);
    }
    #endregion
}
