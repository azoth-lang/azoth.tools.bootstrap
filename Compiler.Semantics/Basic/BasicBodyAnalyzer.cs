using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

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
    private readonly ObjectTypeSymbol? stringSymbol;
    private readonly ObjectTypeSymbol? rangeSymbol;
    private readonly Diagnostics diagnostics;
    private readonly ReturnType? returnType;
    private readonly TypeResolver typeResolver;
    private readonly ParameterSharingRelation parameterSharing;

    public BasicBodyAnalyzer(
        IFunctionDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, returnType)
    { }
    public BasicBodyAnalyzer(
        IAssociatedFunctionDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConstructorDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDeclaration,
            containingDeclaration.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConcreteMethodDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result).Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IFieldDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics)
        : this(containingDeclaration, Enumerable.Empty<BindingSymbol>(),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, null)
    { }

    private BasicBodyAnalyzer(
        IEntityDeclarationSyntax containingDeclaration,
        IEnumerable<BindingSymbol> parameterSymbols,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        ObjectTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        ReturnType? returnType)
    {
        file = containingDeclaration.File;
        containingSymbol = (InvocableSymbol)containingDeclaration.Symbol.Result;
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.stringSymbol = stringSymbol;
        this.rangeSymbol = rangeSymbol;
        this.diagnostics = diagnostics;
        this.symbolTrees = symbolTrees;
        this.returnType = returnType;
        typeResolver = new TypeResolver(file, diagnostics);
        parameterSharing = new ParameterSharingRelation(parameterSymbols);
    }

    public void ResolveTypes(IBodySyntax body)
    {
        var flow = new FlowState(diagnostics, file, parameterSharing);
        switch (body)
        {
            default:
                throw ExhaustiveMatch.Failed(body);
            case IBlockBodySyntax syn:
                ResolveTypes(syn, flow);
                break;
            case IExpressionBodySyntax syn:
                ResolveTypes(syn, flow);
                break;
        }
    }

    private void ResolveTypes(IBlockBodySyntax body, FlowState flow)
    {
        foreach (var statement in body.Statements)
            ResolveTypes(statement, StatementContext.BodyLevel, flow);
    }

    private void ResolveTypes(IExpressionBodySyntax body, FlowState flow)
    {
        if (returnType is not { } expectedReturnType)
            throw new NotImplementedException("Expression body in field initializer.");
        var expectedType = expectedReturnType.Type;
        var result = ResolveTypes(body.ResultStatement, StatementContext.BeforeResult, flow)!;
        // local variables are no longer in scope and isolated parameters have no external references
        flow.DropBindingsForReturn();
        result = AddImplicitConversionIfNeeded(result, expectedType, flow);
        CheckTypeCompatibility(expectedType, result.Syntax);
    }

    /// <summary>
    /// Resolve the types in a statement. If the statement was a <see cref="IResultStatementSyntax"/>
    /// the <see cref="ExpressionResult"/> is returned. Returns <see langword="null"/> otherwise.
    /// </summary>
    private ExpressionResult? ResolveTypes(
        IStatementSyntax statement,
        StatementContext context,
        FlowState flow)
    {
        if (context == StatementContext.AfterResult)
            diagnostics.Add(OtherSemanticError.StatementAfterResult(file, statement.Span));
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
                if (context == StatementContext.BodyLevel)
                    diagnostics.Add(OtherSemanticError.ResultStatementInBody(file, resultStatement.Span));

                // Return type for use in determining block type. Keep result shared for use in
                // parent expression.
                return result;
            }
        }
        return null;
    }

    private void ResolveTypes(
        IVariableDeclarationStatementSyntax variableDeclaration,
        FlowState flow)
    {
        var initializerResult = InferType(variableDeclaration.Initializer, flow);
        DataType variableType;
        if (variableDeclaration.Type is not null)
            variableType = typeResolver.Evaluate(variableDeclaration.Type);
        else if (variableDeclaration.Initializer is not null)
            variableType = InferDeclarationType(variableDeclaration.Initializer, variableDeclaration.Capability);
        else
        {
            diagnostics.Add(TypeError.NotImplemented(file, variableDeclaration.NameSpan,
                "Inference of local variable types not implemented"));
            variableType = DataType.Unknown;
        }

        if (initializerResult is not null)
        {
            initializerResult = AddImplicitConversionIfNeeded(initializerResult, variableType, flow);
            if (!variableType.IsAssignableFrom(initializerResult.Type))
                diagnostics.Add(TypeError.CannotImplicitlyConvert(file, initializerResult.Syntax, initializerResult.Type, variableType));
        }

        var symbol = VariableSymbol.CreateLocal(containingSymbol, variableDeclaration.IsMutableBinding, variableDeclaration.Name, variableDeclaration.DeclarationNumber.Result, variableType);
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
        IReferenceCapabilitySyntax? inferCapability)
    {
        var type = expression.DataType.Assigned();
        if (!type.IsFullyKnown) return DataType.Unknown;
        type = type.ToNonConstantType();

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
                    if (type is not ReferenceType referenceType)
                        throw new NotImplementedException(
                            "Compile error: can't infer mutability for non reference type");

                    type = referenceType.With(inferCapability.Declared.ToReferenceCapability());
                }

                return type;
            }
        }
    }

    public void CheckFieldInitializerType(IExpressionSyntax expression, DataType expectedType)
    {
        var flow = new FlowState(diagnostics, file);
        CheckIndependentExpressionType(expression, expectedType, flow);
    }

    /// <summary>
    /// Check the type of an expression that is independent. One whose result won't be shared with
    /// anything.
    /// </summary>
    private void CheckIndependentExpressionType(
        IExpressionSyntax expression,
        DataType expectedType,
        FlowState flow)
    {
        var result = InferType(expression, flow);
        result = AddImplicitConversionIfNeeded(result, expectedType, flow);
        CheckTypeCompatibility(expectedType, expression);
        flow.Drop(result.Variable);
    }

    /// <summary>
    /// Create an implicit conversion if needed and allowed.
    /// </summary>
    private static ExpressionResult AddImplicitConversionIfNeeded(
        ExpressionResult expression,
        ParameterType expectedType,
        FlowState flow)
    {
        var syntax = expression.Syntax;
        var conversion = CreateImplicitConversion(expectedType.Type, expectedType.IsLentBinding,
            expression.Type, expression.Variable, syntax.ImplicitConversion, flow,
            out var newResult);
        if (conversion is not null) syntax.AddConversion(conversion);
        return expression with { Variable = newResult };
    }

    /// <summary>
    /// Create an implicit conversion if needed and allowed.
    /// </summary>
    private static ExpressionResult AddImplicitConversionIfNeeded(
        ExpressionResult expression,
        DataType expectedType,
        FlowState flow)
    {
        var syntax = expression.Syntax;
        var conversion = CreateImplicitConversion(expectedType, false,
            expression.Type, expression.Variable, syntax.ImplicitConversion, flow, out var newResult);
        if (conversion is not null) syntax.AddConversion(conversion);
        return expression with { Variable = newResult };
    }

    private static ChainedConversion? CreateImplicitConversion(
        DataType toType,
        bool toLentBinding,
        DataType fromType,
        ResultVariable? fromResult,
        Conversion priorConversion,
        FlowState flow,
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
                var liftedConversion = CreateImplicitConversion(to, toLentBinding, from, fromResult, IdentityConversion.Instance, flow, out newResult);
                return liftedConversion is null ? null : new LiftedConversion(liftedConversion, priorConversion);
            case (OptionalType { Referent: var to }, not OptionalType):
                if (!to.IsAssignableFrom(fromType))
                {
                    var conversion = CreateImplicitConversion(to, toLentBinding, fromType, fromResult, priorConversion, flow, out newResult);
                    if (conversion is null) return null; // Not able to convert to the referent type
                    priorConversion = conversion;
                }
                return new OptionalConversion(priorConversion);
            case (FixedSizeIntegerType to, FixedSizeIntegerType from):
                if (to.Bits > from.Bits && (!from.IsSigned || to.IsSigned))
                    return new NumericConversion(to, priorConversion);

                return null;
            case (FixedSizeIntegerType to, IntegerConstantType from):
            {
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                if (to.Bits >= bits && (!requireSigned || to.IsSigned))
                    return new NumericConversion(to, priorConversion);

                return null;
            }
            case (BigIntegerType { IsSigned: true } to, IntegerType):
                return new NumericConversion(to, priorConversion);
            case (BigIntegerType to, IntegerType { IsSigned: false }):
                return new NumericConversion(to, priorConversion);
            case (PointerSizedIntegerType to, IntegerConstantType from):
            {
                var requireSigned = from.Value < 0;
                return !requireSigned || to.IsSigned ? new NumericConversion(to, priorConversion) : null;
            }
            case (ObjectType { IsConstReference: true } to, ObjectType { AllowsFreeze: true } from)
                when to.BareType.IsAssignableFrom(from.BareType):
            {
                // Try to recover const. Note a variable name can never be frozen because the result is an alias.
                if (flow.CanFreeze(fromResult))
                    return new RecoverConst(priorConversion);
                if (toLentBinding)
                {
                    if (enact)
                        newResult = flow.LendConst(fromResult!);
                    return new RecoverConst(priorConversion);
                }

                return null;
            }
            case (ObjectType { IsIsolatedReference: true } to, ObjectType { AllowsRecoverIsolation: true } from)
                when to.BareType.IsAssignableFrom(from.BareType):
            {
                // Try to recover isolation. Note a variable name is never isolated because the result is an alias.
                if (flow.IsIsolated(fromResult))
                    return new RecoverIsolation(priorConversion);
                if (toLentBinding)
                {
                    if (enact)
                        newResult = flow.LendIso(fromResult!);
                    return new RecoverIsolation(priorConversion);
                }
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
    private ExpressionResult? InferType(IExpressionSyntax? expression, FlowState flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case null:
                return null;
            case IIdExpressionSyntax exp:
            {
                // TODO do not allow `id mut T`
                var result = InferType(exp.Referent, flow);
                DataType type;
                if (result.Type is ReferenceType referenceType)
                    type = referenceType.With(ReferenceCapability.Identity);
                else
                {
                    diagnostics.Add(TypeError.CannotIdNonReferenceType(file, exp.Span, result.Type));
                    type = DataType.Unknown;
                }
                // Don't need to alias the symbol or union with result in flow because ids don't matter
                exp.DataType = type;
                flow.Drop(result.Variable); // drop the previous result variable
                return new ExpressionResult(exp);
            }
            case IMoveExpressionSyntax exp:
                switch (exp.Referent)
                {
                    default:
                        throw ExhaustiveMatch.Failed(exp.Referent);
                    case ISimpleNameExpressionSyntax nameExpression:
                    {
                        var symbol = InferNameSymbol(nameExpression);
                        if (symbol is not BindingSymbol bindingSymbol)
                            throw new NotImplementedException("Raise error about `move` from non-variable");

                        return InferMoveExpressionType(exp, bindingSymbol, flow);
                    }
                    case ISelfExpressionSyntax selfExpression:
                    {
                        var symbol = InferSelfSymbol(selfExpression);
                        if (symbol is null) throw new NotImplementedException("Raise error about `move` from self");
                        return InferMoveExpressionType(exp, symbol, flow);
                    }
                }
            case IFreezeExpressionSyntax exp:
                switch (exp.Referent)
                {
                    default:
                        throw ExhaustiveMatch.Failed(exp.Referent);
                    case ISimpleNameExpressionSyntax nameExpression:
                    {
                        var symbol = InferNameSymbol(nameExpression);
                        if (symbol is not BindingSymbol bindingSymbol)
                            throw new NotImplementedException("Raise error about `freeze` of non-variable");

                        return InferFreezeExpressionType(exp, bindingSymbol, flow);
                    }
                    case ISelfExpressionSyntax selfExpression:
                    {
                        var symbol = InferSelfSymbol(selfExpression);
                        if (symbol is null) throw new NotImplementedException("Raise error about `freeze` from self");
                        return InferFreezeExpressionType(exp, symbol, flow);
                    }
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
                    result = AddImplicitConversionIfNeeded(result, expectedType, flow);
                    CheckTypeCompatibility(expectedType, exp.Value);
                    // TODO use proper check instead of `is not ValueType`
                    if (!expectedReturnType.IsLent && flow.IsLent(result.Variable) && expectedReturnType.Type is not ValueType)
                        diagnostics.Add(FlowTypingError.CannotReturnLent(file, exp));
                    flow.Drop(result.Variable);
                }
                else if (expectedType == DataType.Never)
                    diagnostics.Add(TypeError.CannotReturnFromNeverFunction(file, exp.Span));
                else if (expectedType != DataType.Void)
                    diagnostics.Add(TypeError.MustReturnCorrectType(file, exp.Span, expectedType));

                // Return expressions always have the type Never
                return new ExpressionResult(exp);
            }
            case IIntegerLiteralExpressionSyntax exp:
                exp.DataType = new IntegerConstantType(exp.Value);
                return new ExpressionResult(exp);
            case IStringLiteralExpressionSyntax exp:
                if (stringSymbol is null)
                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span, "Could not find string type for string literal."));
                exp.DataType = stringSymbol?.DeclaresType.With(ReferenceCapability.Constant, FixedList<DataType>.Empty)
                               ?? (DataType)DataType.Unknown;
                return new ExpressionResult(exp);
            case IBoolLiteralExpressionSyntax exp:
                exp.DataType = exp.Value ? DataType.True : DataType.False;
                return new ExpressionResult(exp);
            case IBinaryOperatorExpressionSyntax exp:
            {
                var leftOperand = exp.LeftOperand;
                var leftResult = InferType(leftOperand, flow);
                var @operator = exp.Operator;
                var rightOperand = exp.RightOperand;
                var rightResult = InferType(rightOperand, flow);
                var resultVariable = flow.Combine(leftResult.Variable, rightResult.Variable, exp);

                // If either is unknown, then we can't know whether there is a a problem.
                // Note that the operator could be overloaded
                if (!leftResult.Type.IsFullyKnown || !rightResult.Type.IsFullyKnown)
                {
                    exp.Semantics = ExpressionSemantics.Never;
                    exp.DataType = DataType.Unknown;
                    return new ExpressionResult(exp, resultVariable);
                }

                DataType type = (leftResult.Type, @operator, rightResult.Type) switch
                {
                    (IntegerConstantType left, BinaryOperator.Plus, IntegerConstantType right) => left.Add(right),
                    (IntegerConstantType left, BinaryOperator.Minus, IntegerConstantType right) => left.Subtract(right),
                    (IntegerConstantType left, BinaryOperator.Asterisk, IntegerConstantType right) => left.Multiply(right),
                    (IntegerConstantType left, BinaryOperator.Slash, IntegerConstantType right) => left.DivideBy(right),
                    (IntegerConstantType left, BinaryOperator.EqualsEquals, IntegerConstantType right) => left.Equals(right),
                    (IntegerConstantType left, BinaryOperator.NotEqual, IntegerConstantType right) => left.NotEquals(right),
                    (IntegerConstantType left, BinaryOperator.LessThan, IntegerConstantType right) => left.LessThan(right),
                    (IntegerConstantType left, BinaryOperator.LessThanOrEqual, IntegerConstantType right) => left.LessThanOrEqual(right),
                    (IntegerConstantType left, BinaryOperator.GreaterThan, IntegerConstantType right) => left.GreaterThan(right),
                    (IntegerConstantType left, BinaryOperator.GreaterThanOrEqual, IntegerConstantType right) => left.GreaterThanOrEqual(right),

                    (BoolConstantType left, BinaryOperator.EqualsEquals, BoolConstantType right) => left.Equals(right),
                    (BoolConstantType left, BinaryOperator.NotEqual, BoolConstantType right) => left.NotEquals(right),
                    (BoolConstantType left, BinaryOperator.And, BoolConstantType right) => left.And(right),
                    (BoolConstantType left, BinaryOperator.Or, BoolConstantType right) => left.Or(right),

                    (NumericType, BinaryOperator.Plus, NumericType)
                        or (NumericType, BinaryOperator.Minus, NumericType)
                        or (NumericType, BinaryOperator.Asterisk, NumericType)
                        or (NumericType, BinaryOperator.Slash, NumericType)
                        => InferNumericOperatorType(leftResult, rightResult, flow),
                    (NumericType, BinaryOperator.EqualsEquals, NumericType)
                        or (NumericType, BinaryOperator.NotEqual, NumericType)
                        or (OptionalType { Referent: NumericType }, BinaryOperator.NotEqual, OptionalType { Referent: NumericType })
                        or (NumericType, BinaryOperator.LessThan, NumericType)
                        or (NumericType, BinaryOperator.LessThanOrEqual, NumericType)
                        or (NumericType, BinaryOperator.GreaterThan, NumericType)
                        or (NumericType, BinaryOperator.GreaterThanOrEqual, NumericType)
                        => InferComparisonOperatorType(leftResult, rightResult, flow),

                    (BoolType, BinaryOperator.EqualsEquals, BoolType)
                        or (BoolType, BinaryOperator.NotEqual, BoolType)
                        or (BoolType, BinaryOperator.And, BoolType)
                        or (BoolType, BinaryOperator.Or, BoolType)
                        => DataType.Bool,

                    (ReferenceType, BinaryOperator.EqualsEquals, ReferenceType)
                        or (ReferenceType, BinaryOperator.NotEqual, ReferenceType)
                        => InferReferenceEqualityOperatorType(leftOperand, rightOperand),

                    (_, BinaryOperator.DotDot, _)
                        or (_, BinaryOperator.LessThanDotDot, _)
                        or (_, BinaryOperator.DotDotLessThan, _)
                        or (_, BinaryOperator.LessThanDotDotLessThan, _)
                        => InferRangeOperatorType(leftResult, rightResult, flow),
                    _ => DataType.Unknown

                    // TODO optional types
                };

                if (type == DataType.Unknown)
                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                        exp.Span, @operator, leftResult.Type, rightResult.Type));

                exp.Semantics = ExpressionSemantics.CopyValue;
                exp.DataType = type;
                return new ExpressionResult(exp, resultVariable);
            }
            case ISimpleNameExpressionSyntax exp:
            {
                // Errors reported by InferNameSymbol
                var symbol = InferNameSymbol(exp);
                DataType type;
                ExpressionSemantics referenceSemantics;
                ResultVariable? resultVariable = null;
                if (symbol is VariableSymbol variableSymbol)
                {
                    resultVariable = flow.Alias(variableSymbol);
                    type = flow.AliasType(variableSymbol);
                    // TODO is this right?
                    referenceSemantics = ExpressionSemantics.MutableReference;
                }
                else
                {
                    // It must be a type or namespace name and as such isn't a proper expression
                    type = DataType.Void;
                    referenceSemantics = ExpressionSemantics.Void;
                }
                exp.Semantics = type.Semantics.ToExpressionSemantics(referenceSemantics);
                exp.DataType = type;
                return new ExpressionResult(exp, resultVariable);
            }
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
                        if (result.Type is BoolConstantType boolType)
                            expType = boolType.Not();
                        else
                        {
                            expType = DataType.Bool;
                            result = AddImplicitConversionIfNeeded(result, expType, flow);
                            CheckTypeCompatibility(expType, exp.Operand);
                            flow.Drop(result.Variable);
                            resultVariable = null;
                        }
                        break;
                    case UnaryOperator.Minus:
                        switch (result.Type)
                        {
                            case IntegerConstantType integerType:
                                expType = integerType.Negate();
                                break;
                            case FixedSizeIntegerType sizedIntegerType:
                                expType = sizedIntegerType.WithSign();
                                break;
                            case BigIntegerType:
                                // Even if unsigned before, it is signed now
                                expType = DataType.Int;
                                break;
                            case UnknownType:
                                expType = DataType.Unknown;
                                break;
                            default:
                                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                    exp.Span, @operator, result.Type));
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                    case UnaryOperator.Plus:
                        switch (result.Type)
                        {
                            case NumericType:
                            case UnknownType _:
                                expType = result.Type;
                                break;
                            default:
                                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                    exp.Span, @operator, result.Type));
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                }

                exp.DataType = expType;
                return new ExpressionResult(exp, resultVariable);
            }
            case INewObjectExpressionSyntax exp:
            {
                var arguments = InferArgumentTypes(exp.Arguments, flow);
                var constructingType = typeResolver.EvaluateBareType(exp.Type);
                ResultVariable? resultVariable = null;
                if (!constructingType.IsFullyKnown)
                {
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                    exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType = DataType.Unknown;
                    resultVariable = CombineResults(arguments, flow, exp);
                    return new ExpressionResult(exp, resultVariable);
                }

                if (constructingType is ObjectType { DeclaredType.IsAbstract: true })
                {
                    diagnostics.Add(OtherSemanticError.CannotConstructAbstractType(file, exp.Type));
                    exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType = DataType.Unknown;
                    resultVariable = CombineResults(arguments, flow, exp);
                    return new ExpressionResult(exp, resultVariable);
                }

                var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new NotImplementedException();
                DataType constructedType;
                var contextType = (NonEmptyType)constructingType;
                var constructorSymbols = symbolTrees.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedSet();
                var validOverloads = SelectOverload(contextType, constructorSymbols, arguments, flow);
                switch (validOverloads.Count)
                {
                    case 0:
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        constructedType = DataType.Unknown;
                        break;
                    case 1:
                        var constructor = validOverloads.Single();
                        exp.ReferencedSymbol.Fulfill(constructor.Symbol);
                        CheckTypes(arguments, constructor.ParameterTypes, flow);
                        resultVariable = CombineResults(arguments, flow, exp);
                        constructedType = constructor.ReturnType.Type;
                        break;
                    default:
                        diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        constructedType = DataType.Unknown;
                        break;
                }
                exp.DataType = constructedType;
                flow.Restrict(resultVariable, constructedType);
                return new ExpressionResult(exp, resultVariable);
            }
            case IForeachExpressionSyntax exp:
            {
                var declaredType = typeResolver.Evaluate(exp.Type);
                // TODO deal with result variable here
                var (expressionResult, variableType) = CheckForeachInType(declaredType, exp, flow);
                var symbol = VariableSymbol.CreateLocal(containingSymbol, exp.IsMutableBinding, exp.VariableName, exp.DeclarationNumber.Result, variableType);
                exp.Symbol.Fulfill(symbol);
                symbolTreeBuilder.Add(symbol);
                // Declare the variable symbol and combine it with the `in` expression
                flow.Declare(symbol, expressionResult.Variable);

                // TODO check the break types
                var blockResult = InferBlockType(exp.Block, flow);

                flow.Drop(symbol);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                exp.DataType = DataType.Void;
                return new ExpressionResult(exp, blockResult.Variable);
            }
            case IWhileExpressionSyntax exp:
            {
                CheckIndependentExpressionType(exp.Condition, DataType.Bool, flow);
                var result = InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                exp.DataType = DataType.Void;
                return new ExpressionResult(exp, result.Variable);
            }
            case ILoopExpressionSyntax exp:
            {
                var result = InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                exp.DataType = DataType.Void;
                return new ExpressionResult(exp, result.Variable);
            }
            case IInvocationExpressionSyntax exp:
                return InferInvocationType(exp, flow);
            case IUnsafeExpressionSyntax exp:
            {
                var result = InferType(exp.Expression, flow);
                exp.DataType = result.Type;
                exp.Semantics = exp.Expression.Semantics.Assigned();
                return new ExpressionResult(exp, result.Variable);
            }
            case IIfExpressionSyntax exp:
            {
                CheckIndependentExpressionType(exp.Condition, DataType.Bool, flow);
                var elseClause = exp.ElseClause;
                // Even if there is no else clause, the if could be skipped. Still need to join
                FlowState elseFlow = flow.Fork();
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
                    expType = thenResult.Type.ToOptional();
                else
                    // TODO unify the two types
                    expType = thenResult.Type;
                // TODO correct reference semantics?
                exp.Semantics = expType.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                exp.DataType = expType;
                flow.Merge(elseFlow);
                var resultVariable = flow.Combine(thenResult.Variable, elseResult?.Variable, exp);
                return new ExpressionResult(exp, resultVariable);
            }
            case IQualifiedNameExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow);
                var member = exp.Member;
                var contextSymbol = contextResult.Type is VoidType && exp.Context is INameExpressionSyntax context
                    ? context.ReferencedSymbol.Result
                    : LookupSymbolForType(contextResult.Type);
                if (contextSymbol is null)
                {
                    member.ReferencedSymbol.Fulfill(null);
                    member.DataType = DataType.Unknown;
                    exp.Semantics ??= ExpressionSemantics.CopyValue;
                    exp.DataType = DataType.Unknown;
                    return new ExpressionResult(exp, contextResult.Variable);
                }
                var memberSymbols = symbolTrees.Children(contextSymbol)
                                               .Where(s => s.Name == member.Name).ToFixedList();
                var type = InferReferencedSymbol(member, memberSymbols) ?? DataType.Unknown;
                if (contextResult.Type is NonEmptyType nonEmptyContext)
                    // resolve generic type fields
                    type = nonEmptyContext.ReplaceTypeParametersIn(type);
                var resultVariable = contextResult.Variable;
                var semantics = type.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                member.Semantics = semantics;
                member.DataType = type;
                exp.Semantics = semantics;
                exp.DataType = type;
                return new ExpressionResult(exp, resultVariable);
            }
            case IBreakExpressionSyntax exp:
            {
                var result = InferType(exp.Value, flow);
                // TODO result variable needs to pass out of the loop
                exp.DataType = DataType.Never;
                return new ExpressionResult(exp);
            }
            case INextExpressionSyntax exp:
                exp.DataType = DataType.Never;
                return new ExpressionResult(exp);
            case IAssignmentExpressionSyntax exp:
            {
                var left = InferAssignmentTargetType(exp.LeftOperand, flow);
                var right = InferType(exp.RightOperand, flow);
                right = AddImplicitConversionIfNeeded(right, left.Type, flow);
                if (!left.Type.IsAssignableFrom(right.Type))
                    diagnostics.Add(TypeError.CannotImplicitlyConvert(file,
                        exp.RightOperand, right.Type, left.Type));
                var resultVariable = flow.Combine(left.Variable, right.Variable, exp);
                if (left.Type.Semantics == TypeSemantics.MoveValue)
                {
                    exp.Semantics = ExpressionSemantics.Void;
                    exp.DataType = DataType.Void;
                    flow.Drop(resultVariable);
                }
                else
                {
                    exp.Semantics = left.Type.Semantics.ToExpressionSemantics(ExpressionSemantics.MutableReference);
                    exp.DataType = left.Type;
                }
                return new ExpressionResult(exp, resultVariable);
            }
            case ISelfExpressionSyntax exp:
            {
                // InferSelfSymbol reports diagnostics and returns null if there is a problem
                var selfSymbol = InferSelfSymbol(exp);
                var variableResult = selfSymbol is not null ? flow.Alias(selfSymbol) : null;
                var type = flow.AliasType(selfSymbol);
                // TODO is this correct?
                var referenceSemantics = ExpressionSemantics.MutableReference;
                exp.Semantics = type.Semantics.ToExpressionSemantics(referenceSemantics);
                exp.DataType = type;
                return new ExpressionResult(exp, variableResult);
            }
            case INoneLiteralExpressionSyntax exp:
                exp.DataType = DataType.None;
                return new ExpressionResult(exp);
            case IBlockExpressionSyntax blockSyntax:
                return InferBlockType(blockSyntax, flow);
            case IConversionExpressionSyntax exp:
            {
                var result = InferType(exp.Referent, flow);
                var convertToType = typeResolver.Evaluate(exp.ConvertToType);
                if (!ExplicitConversionTypesAreCompatible(exp.Referent, exp.Operator == ConversionOperator.Safe, convertToType))
                    diagnostics.Add(TypeError.CannotExplicitlyConvert(file, exp.Referent, result.Type, convertToType));
                if (exp.Operator == ConversionOperator.Optional)
                    convertToType = convertToType.ToOptional();
                exp.Semantics = exp.Referent.ConvertedSemantics!;
                exp.DataType = convertToType;
                flow.Restrict(result.Variable, convertToType);
                return new ExpressionResult(exp, result.Variable);
            }
            case IPatternMatchExpressionSyntax exp:
            {
                var referent = InferType(exp.Referent, flow);
                ResolveTypes(exp.Pattern, referent.Type, referent.Variable, flow);
                flow.Drop(referent.Variable);
                exp.Semantics = ExpressionSemantics.CopyValue;
                exp.DataType = DataType.Bool;
                return new ExpressionResult(exp);
            }
            case IAsyncBlockExpressionSyntax exp:
            {
                var result = InferBlockType(exp.Block, flow);
                exp.Semantics = exp.Block.Semantics.Assigned();
                exp.DataType = result.Type;
                return result;
            }
            case IAsyncStartExpressionSyntax exp:
            {
                // TODO these act like function calls holding results longer
                var result = InferType(exp.Expression, flow);
                exp.Semantics = ExpressionSemantics.CopyValue;
                exp.DataType = Intrinsic.PromiseOf(result.Type);
                return new ExpressionResult(exp, result.Variable);
            }
            case IAwaitExpressionSyntax exp:
            {
                var result = InferType(exp.Expression, flow);
                if (result.Type is not ObjectType { DeclaredType: { } declaredType } promiseType
                    || declaredType != Intrinsic.PromiseType)
                {
                    diagnostics.Add(TypeError.CannotAwaitType(file, exp.Span, result.Type));
                    exp.Semantics = ExpressionSemantics.CopyValue;
                    exp.DataType = DataType.Unknown;
                    return new ExpressionResult(exp, result.Variable);
                }

                var resultType = promiseType.TypeArguments[0];
                exp.Semantics = ExpressionSemantics.CopyValue;
                exp.DataType = resultType;
                // TODO what is the effect on the flow typing
                return new ExpressionResult(exp, result.Variable);
            }
        }
    }

    private ExpressionResult InferMoveExpressionType(
        IMoveExpressionSyntax exp,
        BindingSymbol symbol,
        FlowState flow)
    {
        var type = flow.Type(symbol);
        switch (type)
        {
            case ReferenceType referenceType:
                if (!referenceType.AllowsMove)
                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span, "Reference capability does not allow moving"));
                else if (!flow.IsIsolated(symbol))
                    diagnostics.Add(FlowTypingError.CannotMoveValue(file, exp));
                type = referenceType.With(ReferenceCapability.Isolated);
                flow.Move(symbol);
                break;
            case ValueType { Semantics: TypeSemantics.MoveValue } valueType:
                type = valueType;
                break;
            case UnknownType:
                type = DataType.Unknown;
                break;
            default:
                throw new NotImplementedException("Non-moveable type can't be moved");
        }
        // Don't need to alias the symbol or union with result in flow because it will be moved

        exp.ReferencedSymbol.Fulfill(symbol);

        const ExpressionSemantics semantics = ExpressionSemantics.IsolatedReference;
        exp.Referent.Semantics = semantics;
        exp.Referent.DataType = type;
        exp.Semantics = semantics;
        exp.DataType = type;
        return new ExpressionResult(exp);
    }

    private ExpressionResult InferFreezeExpressionType(
        IFreezeExpressionSyntax exp,
        BindingSymbol symbol,
        FlowState flow)
    {
        var type = flow.Type(symbol);
        switch (type)
        {
            case ReferenceType referenceType:
                if (!referenceType.AllowsFreeze)
                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span,
                        "Reference capability does not allow freezing"));
                if (!flow.CanFreeze(symbol))
                    diagnostics.Add(FlowTypingError.CannotFreezeValue(file, exp));

                type = referenceType.With(ReferenceCapability.Constant);
                flow.Freeze(symbol);
                break;
            case UnknownType:
                type = DataType.Unknown;
                break;
            default:
                throw new NotImplementedException("Non-freezable type can't be frozen.");
        }
        // Alias not needed because it is already `const`

        exp.ReferencedSymbol.Fulfill(symbol);

        const ExpressionSemantics semantics = ExpressionSemantics.ConstReference;
        exp.Referent.Semantics = semantics;
        exp.Referent.DataType = type;
        exp.Semantics = semantics;
        exp.DataType = type;
        return new ExpressionResult(exp);
    }

    private void ResolveTypes(
        IPatternSyntax pattern,
        DataType valueType,
        ResultVariable? resultVariable,
        FlowState flow,
        bool? isMutableBinding = false)
    {
        switch (pattern)
        {
            default:
                throw ExhaustiveMatch.Failed(pattern);
            case IBindingContextPatternSyntax pat:
            {
                valueType = typeResolver.Evaluate(pat.Type) ?? valueType;
                ResolveTypes(pat.Pattern, valueType, resultVariable, flow, pat.IsMutableBinding);
                break;
            }
            case IBindingPatternSyntax pat:
            {
                if (isMutableBinding is null) throw new UnreachableCodeException("Binding pattern outside of binding context");
                var symbol = VariableSymbol.CreateLocal(containingSymbol, isMutableBinding.Value, pat.Name,
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
                else
                    diagnostics.Add(TypeError.OptionalPatternOnNonOptionalType(file, pat, valueType));

                ResolveTypes(pat.Pattern, valueType, resultVariable, flow, isMutableBinding);
                break;
            }
        }
    }

    private ArgumentResults InferArgumentTypes(FixedList<IExpressionSyntax> arguments, FlowState flow)
        => InferArgumentTypes(null, arguments, flow);

    private ArgumentResults InferArgumentTypes(
        ExpressionResult? selfArgument,
        FixedList<IExpressionSyntax> arguments,
        FlowState flow)
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

    private void CheckTypes(ArgumentResults arguments, IEnumerable<ParameterType> expectedTypes, FlowState flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.Zip(expectedTypes))
        {
            AddImplicitConversionIfNeeded(arg, parameter, flow);
            CheckTypeCompatibility(parameter.Type, arg.Syntax);
            // TODO update the expression result
        }
    }

    private static bool TypesAreCompatible(ArgumentResults arguments, IEnumerable<ParameterType> expectedTypes, FlowState flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.Zip(expectedTypes))
            if (!TypesAreCompatible(arg, parameter, flow))
                return false;

        return true;
    }

    private static bool TypesAreCompatible(ExpressionResult arg, ParameterType parameter, FlowState flow, bool isSelf = false)
    {
        var argType = arg.Type;
        var priorConversion = arg.Syntax.ImplicitConversion;
        var conversion = CreateImplicitConversion(parameter.Type, parameter.IsLentBinding, arg.Type, arg.Variable,
            priorConversion, flow, out _, enact: false);
        if (conversion is not null)
        {
            (argType, _) = conversion.Apply(argType, arg.Syntax.Semantics.Assigned());
            priorConversion = conversion;
        }

        if (isSelf)
        {
            conversion = CreateImplicitMoveConversion(argType, arg.Syntax, arg.Variable, parameter,
                flow, enact: false, priorConversion);

            if (conversion is not null)
            {
                (argType, _) = conversion.Apply(argType, arg.Syntax.Semantics.Assigned());
                priorConversion = conversion;
            }

            conversion = CreateImplicitFreezeConversion(argType, arg.Syntax, arg.Variable, parameter,
                flow, enact: false, priorConversion);

            if (conversion is not null)
                (argType, _) = conversion.Apply(argType, arg.Syntax.Semantics.Assigned());
        }
        return parameter.Type.IsAssignableFrom(argType);
    }

    private static ResultVariable? CombineResults(
        ArgumentResults results,
        FlowState flow,
        IExpressionSyntax exp)
        => results.All.Select(r => r.Variable)
                  .Aggregate(default(ResultVariable?), (v1, v2) => flow.Combine(v1, v2, exp));

    private ExpressionResult InferAssignmentTargetType(
        IAssignableExpressionSyntax expression,
        FlowState flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IQualifiedNameExpressionSyntax exp:
                var contextResult = InferType(exp.Context, flow);
                DataType type;
                var member = exp.Member;
                switch (contextResult.Type)
                {
                    case ReferenceType { AllowsWrite: false, IsInitReference: false } contextReferenceType:
                        diagnostics.Add(TypeError.CannotAssignFieldOfReadOnly(file, expression.Span, contextReferenceType));
                        goto default;
                    case UnknownType:
                        member.ReferencedSymbol.Fulfill(null);
                        type = DataType.Unknown;
                        break;
                    default:
                        var contextSymbol = LookupSymbolForType(contextResult.Type)
                            ?? throw new NotImplementedException(
                                $"Missing context symbol for type {contextResult.Type.ToILString()}.");
                        var memberSymbols = symbolTrees.Children(contextSymbol).OfType<FieldSymbol>()
                                                       .Where(s => s.Name == member.Name).ToFixedList<Symbol>();
                        type = InferReferencedSymbol(member, memberSymbols) ?? DataType.Unknown;
                        break;
                }

                // Check for assigning into fields (self is handled by binding mutability analysis)
                if (exp.Context is not ISelfExpressionSyntax
                    && member.ReferencedSymbol.Result is BindingSymbol { IsMutableBinding: false, Name: SimpleName name })
                    diagnostics.Add(OtherSemanticError.CannotAssignImmutableField(file, exp.Span, name));

                type = type.AccessedVia(contextResult.Type);
                member.DataType = type;
                var semantics = member.Semantics ??= ExpressionSemantics.CreateReference;
                exp.Semantics = semantics;
                exp.DataType = type;
                return new(exp, contextResult.Variable);
            case ISimpleNameExpressionSyntax exp:
                exp.Semantics = ExpressionSemantics.CreateReference;
                var symbol = InferNameSymbol(exp);
                switch (symbol)
                {
                    case null:
                        exp.DataType = DataType.Unknown;
                        return new(exp);
                    case VariableSymbol variableSymbol:
                    {
                        exp.DataType = flow.Type(variableSymbol);
                        var resultVariable = flow.Alias(variableSymbol);
                        return new(exp, resultVariable);
                    }
                    default:
                        throw new NotImplementedException("Raise error about assigning into a non-variable");
                }
        }
    }

    private ExpressionResult InferInvocationType(
        IInvocationExpressionSyntax invocation,
        FlowState flow)
    {
        // This could actually be any of the following since the parser can't distinguish them:
        // * Regular function invocation
        // * Associated function invocation
        // * Namespaced function invocation
        // * Method invocation

        ArgumentResults results;
        var functionSymbols = FixedSet<FunctionSymbol>.Empty;
        switch (invocation.Expression)
        {
            case IQualifiedNameExpressionSyntax exp:
                var contextResult = InferType(exp.Context, flow);
                // Make sure to infer argument type *after* the context type
                results = InferArgumentTypes(contextResult, invocation.Arguments, flow);
                var name = exp.Member.Name!;
                if (contextResult.Type is not VoidType)
                    return InferMethodInvocationType(invocation, name, results, flow);
                if (exp.Context is INameExpressionSyntax { ReferencedSymbol.Result: Symbol contextSymbol })
                {
                    functionSymbols = symbolTrees.Children(contextSymbol).OfType<FunctionSymbol>()
                                                     .Where(s => s.Name == name).ToFixedSet();
                }
                // No type for function names
                exp.Member.DataType = DataType.Void;
                exp.Member.Semantics = ExpressionSemantics.Void;
                break;
            case ISimpleNameExpressionSyntax exp:
                results = InferArgumentTypes(invocation.Arguments, flow);
                functionSymbols = exp.LookupInContainingScope()
                                    .Select(p => p.Downcast().As<FunctionSymbol>())
                                    .WhereNotNull()
                                    .Select(p => p.Result).ToFixedSet();
                break;
            default:
                throw new NotImplementedException("Invocation of expression");
        }

        return InferFunctionInvocationType(invocation, functionSymbols, results, flow);
    }

    private ExpressionResult InferMethodInvocationType(
        IInvocationExpressionSyntax invocation,
        SimpleName methodName,
        ArgumentResults arguments,
        FlowState flow)
    {
        // There are no types for functions
        invocation.Expression.DataType = DataType.Void;
        invocation.Expression.Semantics = ExpressionSemantics.Void;

        var selfResult = arguments.Self.Assigned();
        var selfArgumentType = selfResult.Type;
        // If it is unknown, we already reported an error
        if (!selfArgumentType.IsFullyKnown)
        {
            invocation.Semantics = ExpressionSemantics.Never;
            invocation.DataType = DataType.Unknown;
            return new(invocation, CombineResults(arguments, flow, invocation));
        }

        var validOverloads = SelectMethodOverload(methodName, selfArgumentType, arguments, flow);

        switch (validOverloads.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMethod(file, invocation.Span, methodName));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                break;
            case 1:
                var overload = validOverloads.Single();
                invocation.ReferencedSymbol.Fulfill(overload.Symbol);

                var selfParamType = overload.SelfParameterType!.Value;
                selfResult = AddImplicitConversionIfNeeded(selfResult, selfParamType, flow);
                AddImplicitMoveIfNeeded(selfResult, selfParamType, flow);
                AddImplicitFreezeIfNeeded(selfResult, selfParamType, flow);
                CheckTypeCompatibility(selfParamType.Type, selfResult.Syntax);
                CheckTypes(arguments, overload.ParameterTypes, flow);
                arguments = arguments with { Self = selfResult };
                invocation.DataType = overload.ReturnType.Type;
                AssignInvocationSemantics(invocation, invocation.DataType);
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousMethodCall(file, invocation.Span, methodName));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                break;
        }

        var resultVariable = CombineResults(arguments, flow, invocation);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is IQualifiedNameExpressionSyntax nameExpression)
        {
            nameExpression.Member.DataType = DataType.Void;
            nameExpression.Member.Semantics = ExpressionSemantics.Void;
            nameExpression.Member.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);
        }

        return new(invocation, resultVariable);
    }

    private static void AddImplicitMoveIfNeeded(
        ExpressionResult selfArg,
        ParameterType selfParamType,
        FlowState flow)
    {
        var conversion = CreateImplicitMoveConversion(selfArg.Type, selfArg.Syntax, selfArg.Variable,
            selfParamType, flow, enact: true, selfArg.Syntax.ImplicitConversion);
        if (conversion is not null)
            selfArg.Syntax.AddConversion(conversion);
    }

    private static ImplicitMove? CreateImplicitMoveConversion(
        DataType selfArgType,
        IExpressionSyntax selfArgSyntax,
        ResultVariable? selfArgVariable,
        ParameterType selfParamType,
        FlowState flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit moves never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `iso`.
        if (selfParamType.IsLentBinding) return null;

        if (selfParamType.Type is not ReferenceType { IsIsolatedReference: true } toType
            || selfArgType is not ReferenceType { AllowsRecoverIsolation: true } fromType)
            return null;

        if (!toType.BareType.IsAssignableFrom(fromType.BareType)) return null;

        if (selfArgSyntax is not INameExpressionSyntax { ReferencedSymbol.Result: VariableSymbol { IsLocal: true } symbol }
            || !flow.IsIsolatedExceptFor(symbol, selfArgVariable))
            return null;

        if (enact)
            flow.Move(symbol);

        return new ImplicitMove(priorConversion);
    }

    private static void AddImplicitFreezeIfNeeded(
        ExpressionResult selfArg,
        ParameterType selfParamType,
        FlowState flow)
    {
        var conversion = CreateImplicitFreezeConversion(selfArg.Type, selfArg.Syntax, selfArg.Variable,
            selfParamType, flow, enact: true, selfArg.Syntax.ImplicitConversion);
        if (conversion is not null)
            selfArg.Syntax.AddConversion(conversion);
    }

    private static ImplicitFreeze? CreateImplicitFreezeConversion(
        DataType selfArgType,
        IExpressionSyntax selfArgSyntax,
        ResultVariable? selfArgVariable,
        ParameterType selfParamType,
        FlowState flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit freezes never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `const`
        if (selfParamType.IsLentBinding) return null;

        if (selfParamType.Type is not ReferenceType { IsConstReference: true } toType
            || selfArgType is not ReferenceType { AllowsFreeze: true } fromType)
            return null;

        if (!toType.BareType.IsAssignableFrom(fromType.BareType)) return null;

        if (selfArgSyntax is not INameExpressionSyntax { ReferencedSymbol.Result: VariableSymbol { IsLocal: true } symbol }
            || !flow.CanFreezeExceptFor(symbol, selfArgVariable))
            return null;

        if (enact)
            flow.Freeze(symbol);

        return new ImplicitFreeze(priorConversion);
    }

    private ExpressionResult InferFunctionInvocationType(
        IInvocationExpressionSyntax invocation,
        FixedSet<FunctionSymbol> functionSymbols,
        ArgumentResults arguments,
        FlowState flow)
    {
        var validOverloads = SelectOverload(null, functionSymbols, arguments, flow);
        switch (validOverloads.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(file, invocation));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                invocation.Semantics = ExpressionSemantics.Never;
                break;
            case 1:
                var function = validOverloads.Single();
                invocation.ReferencedSymbol.Fulfill(function.Symbol);
                CheckTypes(arguments, function.ParameterTypes, flow);
                var returnType = function.ReturnType.Type;
                invocation.DataType = returnType;
                AssignInvocationSemantics(invocation, returnType);
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, invocation.Span));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                invocation.Semantics = ExpressionSemantics.Never;
                break;
        }

        var resultVariable = CombineResults(arguments, flow, invocation);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        // There are no types for functions
        invocation.Expression.DataType = DataType.Void;
        invocation.Expression.Semantics = ExpressionSemantics.Void;

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is INameExpressionSyntax nameExpression)
            nameExpression.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);

        return new(invocation, resultVariable);
    }

    private static void AssignInvocationSemantics(
        IInvocationExpressionSyntax invocationExpression,
        DataType returnType)
    {
        switch (returnType.Semantics)
        {
            default:
                throw ExhaustiveMatch.Failed(returnType.Semantics);
            case TypeSemantics.Void:
                invocationExpression.Semantics = ExpressionSemantics.Void;
                break;
            case TypeSemantics.MoveValue:
                invocationExpression.Semantics = ExpressionSemantics.MoveValue;
                break;
            case TypeSemantics.CopyValue:
                invocationExpression.Semantics = ExpressionSemantics.CopyValue;
                break;
            case TypeSemantics.Never:
                invocationExpression.Semantics = ExpressionSemantics.Never;
                break;
            case TypeSemantics.Reference:
                while (returnType is OptionalType optionalType)
                    returnType = optionalType.Referent;
                var referenceType = (ReferenceType)returnType;
                if (referenceType.Capability == ReferenceCapability.Isolated)
                    invocationExpression.Semantics = ExpressionSemantics.IsolatedReference;
                else if (referenceType.AllowsWrite)
                    invocationExpression.Semantics = ExpressionSemantics.MutableReference;
                else
                    invocationExpression.Semantics = ExpressionSemantics.ReadOnlyReference;
                break;
        }
    }

    private ExpressionResult InferBlockType(
        IBlockOrResultSyntax blockOrResult,
        FlowState flow)
    {
        switch (blockOrResult)
        {
            default:
                throw ExhaustiveMatch.Failed(blockOrResult);
            case IBlockExpressionSyntax block:
                ExpressionResult? blockResult = null;
                foreach (var statement in block.Statements)
                {
                    var context = blockResult is null ? StatementContext.BeforeResult : StatementContext.AfterResult;
                    var resultType = ResolveTypes(statement, context, flow);
                    // Always resolve types even if there is already a block type
                    blockResult ??= resultType;
                }

                // Drop any variables in the scope
                foreach (var variableDeclaration in block.Statements.OfType<IVariableDeclarationStatementSyntax>())
                    flow.Drop(variableDeclaration.Symbol.Result);

                // If there was no result expression, then the block type is void
                var blockType = blockResult?.Type ?? DataType.Void;
                // TODO what are the correct expression semantics for references?
                block.Semantics = blockType.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                block.DataType = blockType;
                return new(block, blockResult?.Variable);
            case IResultStatementSyntax result:
                // Can't return an ExpressionResult for the statement, return it for the expression instead
                return InferType(result.Expression, flow);
        }
    }

    private Symbol? InferNameSymbol(ISimpleNameExpressionSyntax nameExpression)
    {
        if (nameExpression.Name is null)
        {
            // Name unknown, no error
            nameExpression.ReferencedSymbol.Fulfill(null);
            return null;
        }

        // First look for local variables
        var variableSymbols = nameExpression.LookupInContainingScope()
                                    .Select(p => p.Downcast().As<VariableSymbol>())
                                    .WhereNotNull()
                                    .ToFixedList();
        var symbolCount = variableSymbols.Count;
        Symbol? symbol = null;
        if (symbolCount == 1)
            symbol = variableSymbols.Single().Result;
        else if (symbolCount == 0)
        {
            // If no local variables, look for other symbols
            var symbols = nameExpression.LookupInContainingScope().ToFixedList();
            symbolCount = symbols.Count;
            if (symbolCount == 1)
                symbol = symbols.Single().Result;
        }

        switch (symbolCount)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, nameExpression.Span));
                nameExpression.ReferencedSymbol.Fulfill(null);
                return null;
            case 1:
                nameExpression.ReferencedSymbol.Fulfill(symbol!);
                return symbol!;
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, nameExpression.Span));
                nameExpression.ReferencedSymbol.Fulfill(null);
                return null;
        }
    }

    private SelfParameterSymbol? InferSelfSymbol(ISelfExpressionSyntax selfExpression)
    {
        switch (containingSymbol)
        {
            default:
                throw ExhaustiveMatch.Failed(containingSymbol);
            case MethodSymbol _:
            case ConstructorSymbol _:
                var symbols = symbolTreeBuilder.GetChildrenOf(containingSymbol).OfType<SelfParameterSymbol>().ToList();
                switch (symbols.Count)
                {
                    case 0:
                        diagnostics.Add(NameBindingError.CouldNotBindName(file, selfExpression.Span));
                        selfExpression.ReferencedSymbol.Fulfill(null);
                        return null;
                    case 1:
                        var symbol = symbols.Single();
                        selfExpression.ReferencedSymbol.Fulfill(symbol);
                        return symbol;
                    default:
                        diagnostics.Add(NameBindingError.AmbiguousName(file, selfExpression.Span));
                        selfExpression.ReferencedSymbol.Fulfill(null);
                        return null;
                }
            case FunctionSymbol _:
                diagnostics.Add(selfExpression.IsImplicit
                    ? OtherSemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                    : OtherSemanticError.SelfOutsideMethod(file, selfExpression.Span));
                selfExpression.ReferencedSymbol.Fulfill(null);
                return null;
        }
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
        FlowState flow)
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
                                       .SingleOrDefault(s => s.Name == "iterate" && s.Arity == 0 && s.ReturnType.Type is NonEmptyType);

        exp.IterateMethod.Fulfill(iterateMethod);

        var iteratorType = iterateMethod is not null ? iterableType.ReplaceTypeParametersIn(iterateMethod.ReturnType.Type) : iterableType;
        var iteratorSymbol = LookupSymbolForType(iteratorType);
        if (iteratorSymbol is null) return ForeachNoIterateOrNextMethod();

        var nextMethod = symbolTrees.Children(iteratorSymbol).OfType<MethodSymbol>()
                                    .SingleOrDefault(s => s.Name == "next" && s.Arity == 0 && s.ReturnType.Type is OptionalType);

        if (nextMethod is null)
        {
            if (iterateMethod is null) return ForeachNoIterateOrNextMethod();
            diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(file, inExpression, iterableType));
            return (result, declaredType ?? DataType.Unknown);
        }

        exp.NextMethod.Fulfill(nextMethod);

        // iteratorType is NonEmptyType because it has a `next()` method
        DataType iteratedType = nextMethod.ReturnType.Type is OptionalType optionalType
            ? ((NonEmptyType)iteratorType).ReplaceTypeParametersIn(optionalType.Referent)
            : throw new UnreachableCodeException();

        if (declaredType is not null)
        {
            var conversion = CreateImplicitConversion(declaredType, false, iteratedType, null, inExpression.ImplicitConversion, flow,
                out _, enact: false);
            iteratedType = conversion is null ? iteratedType : conversion.Apply(iteratedType, ExpressionSemantics.IdReference).Item1;
            if (!declaredType.IsAssignableFrom(iteratedType))
                // TODO this error needs to be specific to iterators (e.g. not "Cannot convert expression `0..<count` of type `int` to type `size`")
                diagnostics.Add(TypeError.CannotImplicitlyConvert(file, inExpression, iteratedType, declaredType));
        }

        var variableType = declaredType ?? iteratedType.ToNonConstantType();
        return (result, variableType);

        (ExpressionResult, DataType) ForeachNoIterateOrNextMethod()
        {
            diagnostics.Add(OtherSemanticError.ForeachNoIterateOrNextMethod(file, inExpression, result.Type));
            return (result, declaredType ?? DataType.Unknown);
        }
    }

    private void CheckTypeCompatibility(DataType type, IExpressionSyntax arg)
    {
        var fromType = arg.ConvertedDataType.Assigned();
        if (!type.IsAssignableFrom(fromType))
            diagnostics.Add(TypeError.CannotImplicitlyConvert(file, arg, fromType, type));
    }

    private DataType? InferReferencedSymbol(
        ISimpleNameExpressionSyntax exp,
        FixedList<Symbol> matchingSymbols)
    {
        switch (matchingSymbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMember(file, exp.Span));
                exp.ReferencedSymbol.Fulfill(null);
                return null;
            case 1:
                var memberSymbol = matchingSymbols.Single();
                DataType type = DataType.Void;
                if (memberSymbol is FieldSymbol fieldSymbol)
                {
                    type = fieldSymbol.DataType;
                    switch (type.Semantics)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(type.Semantics);
                        case TypeSemantics.CopyValue:
                            //exp.Semantics = ExpressionSemantics.CopyValue;
                            break;
                        case TypeSemantics.Never:
                            //exp.Semantics = ExpressionSemantics.Never;
                            break;
                        case TypeSemantics.Reference:
                            // Needs to be assigned based on share/borrow expression
                            break;
                        case TypeSemantics.MoveValue:
                            throw new InvalidOperationException("Can't move out of field");
                        case TypeSemantics.Void:
                            throw new InvalidOperationException("Can't assign semantics to void field");
                    }
                }

                exp.ReferencedSymbol.Fulfill(memberSymbol);
                return type;
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, exp.Span));
                exp.ReferencedSymbol.Fulfill(null);
                return null;
        }
    }

    private static DataType InferNumericOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowState flow)
    {
        var leftType = leftOperand.Type.Assigned();
        var rightType = rightOperand.Type.Assigned();
        var commonType = leftType.NumericOperatorCommonType(rightType);
        if (commonType is null) return DataType.Unknown;

        AddImplicitConversionIfNeeded(leftOperand, commonType, flow);
        AddImplicitConversionIfNeeded(rightOperand, commonType, flow);
        return commonType;
    }

    private static DataType InferComparisonOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowState flow)
    {
        var leftType = leftOperand.Type.Assigned();
        var rightType = rightOperand.Type.Assigned();
        var commonType = leftType.NumericOperatorCommonType(rightType);
        if (commonType is null) return DataType.Unknown;

        AddImplicitConversionIfNeeded(leftOperand, commonType, flow);
        AddImplicitConversionIfNeeded(rightOperand, commonType, flow);
        return DataType.Bool;
    }

    /// <summary>
    /// Check if two expressions are `id` types and comparable with `==` and `!=`.
    /// </summary>
    private static DataType InferReferenceEqualityOperatorType(
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand)
    {
        if (leftOperand.ConvertedDataType is ReferenceType { IsIdentityReference: true } left
           && rightOperand.ConvertedDataType is ReferenceType { IsIdentityReference: true } right
           && (left.IsAssignableFrom(right) || right.IsAssignableFrom(left)))
            return DataType.Bool;
        return DataType.Unknown;
    }

    private DataType InferRangeOperatorType(
        ExpressionResult leftOperand,
        ExpressionResult rightOperand,
        FlowState flow)
    {
        AddImplicitConversionIfNeeded(leftOperand, DataType.Int, flow);
        AddImplicitConversionIfNeeded(rightOperand, DataType.Int, flow);
        return rangeSymbol?.DeclaresType.With(ReferenceCapability.Constant, FixedList<DataType>.Empty)
               ?? (DataType)DataType.Unknown;
    }

    private static bool ExplicitConversionTypesAreCompatible(IExpressionSyntax expression, bool safeOnly, DataType convertToType)
    {
        return (expression.ConvertedDataType.Assigned(), convertToType) switch
        {
            // Safe conversions
            (BoolType, IntegerType) => true,
            (IntegerType { IsSigned: false }, BigIntegerType) => true,
            (IntegerType, BigIntegerType { IsSigned: true }) => true,
            (FixedSizeIntegerType from, FixedSizeIntegerType to)
                when from.Bits < to.Bits || (from.Bits == to.Bits && from.IsSigned == to.IsSigned)
                => true,
            // TODO conversions for constants
            // Unsafe conversions
            (IntegerType, IntegerType) => !safeOnly,
            _ => convertToType.IsAssignableFrom(expression.ConvertedDataType),
        };
    }

    private static FixedSet<Contextualized<TSymbol>> SelectOverload<TSymbol>(
        NonEmptyType? contextType,
        FixedSet<TSymbol> symbols,
        ArgumentResults arguments,
        FlowState flow)
        where TSymbol : InvocableSymbol
    {
        var contextualizedSymbols = CompatibleOverloads(contextType, symbols, arguments, flow).ToFixedSet();
        // TODO Select most specific match
        return contextualizedSymbols;
    }

    private static IEnumerable<Contextualized<TSymbol>> CompatibleOverloads<TSymbol>(
        NonEmptyType? contextType,
        FixedSet<TSymbol> symbols,
        ArgumentResults arguments,
        FlowState flow)
        where TSymbol : InvocableSymbol
    {
        var contextualizedSymbols = Contextualize(contextType, symbols);
        // Filter down to symbols that could possible match
        contextualizedSymbols = contextualizedSymbols.Where(s =>
        {
            // Arity depends on the contextualized symbols because parameters can drop out with `void`
            if (s.Arity != arguments.Arity) return false;
            return TypesAreCompatible(arguments, s.ParameterTypes, flow);
        }).ToFixedSet();
        return contextualizedSymbols;
    }

    private static IEnumerable<Contextualized<TSymbol>> Contextualize<TSymbol>(
        NonEmptyType? contextType,
        IEnumerable<TSymbol> symbols)
        where TSymbol : InvocableSymbol
    {
        if (contextType is null)
            return symbols.Select(s => new Contextualized<TSymbol>(s, SelfParameterTypeOrNull(s), s.ParameterTypes, s.ReturnType));

        return symbols.Select(s =>
        {
            var effectiveSelfType = contextType.ReplaceTypeParametersIn(SelfParameterTypeOrNull(s));
            var effectiveParameterTypes = s.ParameterTypes.Select(contextType.ReplaceTypeParametersIn)
                                           .Where(p => p.Type is NonEmptyType).ToFixedList();
            var effectiveReturnType = contextType.ReplaceTypeParametersIn(s.ReturnType);
            return new Contextualized<TSymbol>(s, effectiveSelfType, effectiveParameterTypes, effectiveReturnType);
        });
    }

    private static ParameterType? SelfParameterTypeOrNull(InvocableSymbol symbol)
    {
        if (symbol is MethodSymbol { SelfParameterType: var selfType })
            return selfType;
        return null;
    }

    private FixedSet<Contextualized<MethodSymbol>> SelectMethodOverload(
        SimpleName methodName,
        DataType selfType,
        ArgumentResults arguments,
        FlowState flow)
    {
        var contextualizedSymbols = CompatibleMethodOverloads(methodName, selfType, arguments, flow).ToFixedSet();
        // TODO Select most specific match
        return contextualizedSymbols;
    }

    private IEnumerable<Contextualized<MethodSymbol>> CompatibleMethodOverloads(
        SimpleName methodName,
        DataType selfType,
        ArgumentResults arguments,
        FlowState flow)
    {
        if (selfType is not NonEmptyType nonEmptySelfType)
            return FixedSet<Contextualized<MethodSymbol>>.Empty;

        if (nonEmptySelfType is OptionalType)
            return FixedSet<Contextualized<MethodSymbol>>.Empty;

        var contextSymbol = LookupSymbolForType(nonEmptySelfType);
        var symbols = symbolTrees.Children(contextSymbol!)
                                 .OfType<MethodSymbol>()
                                 .Where(s => s.Name == methodName);

        var contextualizedSymbols = Contextualize(nonEmptySelfType, symbols);

        // Filter down to symbols that could possible match
        contextualizedSymbols = contextualizedSymbols.Where(s =>
        {
            // Arity depends on the contextualized symbols because parameters can drop out with `void`
            if (s.Arity != arguments.Arity) return false;
            // Is self arg compatible?
            if (!TypesAreCompatible(arguments.Self!, nonEmptySelfType.ReplaceTypeParametersIn(s.SelfParameterType!.Value),
                    flow, isSelf: true))
                return false;
            // Are arguments compatible?
            var parameterTypes = s.ParameterTypes.Select(nonEmptySelfType.ReplaceTypeParametersIn);
            return TypesAreCompatible(arguments, parameterTypes, flow);
        }).ToFixedSet();
        return contextualizedSymbols;
    }

    private TypeSymbol? LookupSymbolForType(DataType dataType)
    {
        return dataType switch
        {
            UnknownType or IntegerConstantType or OptionalType => null,
            ObjectType t => LookupSymbolForType(t),
            GenericParameterType t => LookupSymbolForType(t),
            IntegerType t => symbolTrees.PrimitiveSymbolTree
                                                  .GlobalSymbols
                                                  .OfType<PrimitiveTypeSymbol>()
                                                  .Single(s => s.DeclaresType == t),
            _ => throw new NotImplementedException(
                $"{nameof(LookupSymbolForType)} not implemented for {dataType.GetType().Name}")
        };
    }

    private TypeSymbol LookupSymbolForType(ObjectType type)
        => LookupSymbolForType(type.DeclaredType);

    private TypeSymbol LookupSymbolForType(DeclaredObjectType type)
    {
        var contextSymbols = symbolTrees.Packages.SafeCast<Symbol>();
        foreach (var name in type.ContainingNamespace.Segments)
        {
            contextSymbols = contextSymbols.SelectMany(c => symbolTrees.Children(c))
                                           .Where(s => s.Name == name);
        }

        return contextSymbols.SelectMany(symbolTrees.Children).OfType<TypeSymbol>()
                             .Single(s => s.Name == type.Name);
    }

    private TypeSymbol LookupSymbolForType(GenericParameterType genericParameterType)
    {
        var declaringTypeSymbol = LookupSymbolForType(genericParameterType.DeclaringType);
        return symbolTrees.Children(declaringTypeSymbol).OfType<TypeSymbol>()
                   .Single(s => s.Name == genericParameterType.Name);
    }
}
