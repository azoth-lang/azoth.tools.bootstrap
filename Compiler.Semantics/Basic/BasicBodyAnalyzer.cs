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
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using DataType = Azoth.Tools.Bootstrap.Compiler.Types.DataType;
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
    private readonly UserTypeSymbol? stringSymbol;
    private readonly UserTypeSymbol? rangeSymbol;
    private readonly Diagnostics diagnostics;
    private readonly Return? returnType;
    private readonly TypeResolver typeResolver;
    private readonly ParameterSharingRelation parameterSharing;

    public BasicBodyAnalyzer(
        IFunctionDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return @return)
        : this(containingDeclaration, null, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, @return)
    { }
    public BasicBodyAnalyzer(
        IAssociatedFunctionDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return @return)
        : this(containingDeclaration, null, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, @return)
    { }

    public BasicBodyAnalyzer(
        IConstructorDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return @return)
        : this(containingDeclaration, containingDeclaration.SelfParameter.DataType.Result,
            containingDeclaration.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, @return)
    { }

    public BasicBodyAnalyzer(
        IInitializerDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return @return)
        : this(containingDeclaration, containingDeclaration.SelfParameter.DataType.Result,
            containingDeclaration.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, @return)
    { }

    public BasicBodyAnalyzer(
        IConcreteMethodDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return @return)
        : this(containingDeclaration, containingDeclaration.SelfParameter.DataType.Result,
            containingDeclaration.Parameters.Select(p => p.Symbol.Result).Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, @return)
    { }

    public BasicBodyAnalyzer(
        IFieldDeclarationSyntax containingDeclaration,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics)
        : this(containingDeclaration, null, Enumerable.Empty<BindingSymbol>(),
            symbolTreeBuilder, symbolTrees, stringSymbol, rangeSymbol, diagnostics, null)
    { }

    private BasicBodyAnalyzer(
        IEntityDeclarationSyntax containingDeclaration,
        Pseudotype? selfType,
        IEnumerable<BindingSymbol> parameterSymbols,
        ISymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        UserTypeSymbol? stringSymbol,
        UserTypeSymbol? rangeSymbol,
        Diagnostics diagnostics,
        Return? returnType)
    {
        file = containingDeclaration.File;
        containingSymbol = (InvocableSymbol)containingDeclaration.Symbol.Result;
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.stringSymbol = stringSymbol;
        this.rangeSymbol = rangeSymbol;
        this.diagnostics = diagnostics;
        this.symbolTrees = symbolTrees;
        this.returnType = returnType;
        typeResolver = new TypeResolver(file, diagnostics, selfType);
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

                    type = referenceType.With(inferCapability.Declared.ToCapability());
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
        Parameter expectedType,
        FlowState flow)
    {
        var syntax = expression.Syntax;
        var conversion = CreateImplicitConversion(expectedType.Type,
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
        var conversion = CreateImplicitConversion(expectedType,
            expression.Type, expression.Variable, syntax.ImplicitConversion, flow, out var newResult);
        if (conversion is not null) syntax.AddConversion(conversion);
        return expression with { Variable = newResult };
    }

    private static ChainedConversion? CreateImplicitConversion(
        DataType toType,
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
                var liftedConversion = CreateImplicitConversion(to, from, fromResult, IdentityConversion.Instance, flow, out newResult);
                return liftedConversion is null ? null : new LiftedConversion(liftedConversion, priorConversion);
            case (OptionalType { Referent: var to }, not OptionalType):
                if (!to.IsAssignableFrom(fromType))
                {
                    var conversion = CreateImplicitConversion(to, fromType, fromResult, priorConversion, flow, out newResult);
                    if (conversion is null) return null; // Not able to convert to the referent type
                    priorConversion = conversion;
                }
                return new OptionalConversion(priorConversion);
            case (ValueType<BoolType> to, BoolConstValueType from):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (ValueType<FixedSizeIntegerType> to, ValueType<FixedSizeIntegerType> from):
                if (to.DeclaredType.Bits > from.DeclaredType.Bits && (!from.DeclaredType.IsSigned || to.DeclaredType.IsSigned))
                    return new SimpleTypeConversion(to.DeclaredType, priorConversion);
                return null;
            case (ValueType<FixedSizeIntegerType> to, IntegerConstValueType from):
            {
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.DeclaredType.IsSigned) * 8;
                if (to.DeclaredType.Bits >= bits && (!requireSigned || to.DeclaredType.IsSigned))
                    return new SimpleTypeConversion(to.DeclaredType, priorConversion);

                return null;
            }
            case (ValueType<BigIntegerType> { DeclaredType.IsSigned: true } to, ValueType { DeclaredType: IntegerType }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (ValueType<BigIntegerType> to, ValueType { DeclaredType: IntegerType { IsSigned: false } }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (ValueType<BigIntegerType> { DeclaredType.IsSigned: true } to, IntegerConstValueType):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (ValueType<BigIntegerType> to, IntegerConstValueType { IsSigned: false }):
                return new SimpleTypeConversion(to.DeclaredType, priorConversion);
            case (ValueType<PointerSizedIntegerType> to, IntegerConstValueType from):
            {
                var requireSigned = from.Value < 0;
                return !requireSigned || to.DeclaredType.IsSigned ? new SimpleTypeConversion(to.DeclaredType, priorConversion) : null;
            }
            case (ReferenceType { IsTemporarilyConstantReference: true } to, ReferenceType { AllowsFreeze: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: false, from.BareType):
            {
                if (enact) newResult = flow.TempFreeze(fromResult!);
                return new FreezeConversion(priorConversion, ConversionKind.Temporary);
            }
            case (ReferenceType { IsConstantReference: true } to, ReferenceType { AllowsFreeze: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: false, from.BareType):
            {
                // Try to recover const. Note a variable name can never be frozen because the result is an alias.
                if (flow.CanFreeze(fromResult))
                    return new FreezeConversion(priorConversion, ConversionKind.Recover);
                return null;
            }
            case (ReferenceType { IsTemporarilyIsolatedReference: true } to, ReferenceType { AllowsRecoverIsolation: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: true, from.BareType):
            {
                if (enact) newResult = flow.TempMove(fromResult!);
                return new MoveConversion(priorConversion, ConversionKind.Temporary);
            }
            case (ReferenceType { IsIsolatedReference: true } to, ReferenceType { AllowsRecoverIsolation: true } from)
                when to.BareType.IsAssignableFrom(targetAllowsWrite: true, from.BareType):
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
        IExpressionSyntax? expression, FlowState flow, Func<Symbol, bool>? symbolFilter = null)
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
                    type = referenceType.With(Capability.Identity);
                else
                {
                    diagnostics.Add(TypeError.CannotIdNonReferenceType(file, exp.Span, result.Type));
                    type = DataType.Unknown;
                }
                // Don't need to alias the symbol or union with result in flow because ids don't matter
                exp.DataType.Fulfill(type);
                flow.Drop(result.Variable); // drop the previous result variable
                return new ExpressionResult(exp);
            }
            case IMoveExpressionSyntax exp:
            {
                BindingSymbol? bindingSymbol = InferBindingSymbol(exp.Referent);
                if (bindingSymbol is not null)
                    return InferMoveExpressionType(exp, bindingSymbol, flow);

                exp.ReferencedSymbol.Fulfill(null);

                exp.Referent.FulfillDataType(DataType.Unknown);
                exp.DataType.Fulfill(DataType.Unknown);
                return new ExpressionResult(exp);
            }
            case IFreezeExpressionSyntax exp:
            {
                BindingSymbol? bindingSymbol = InferBindingSymbol(exp.Referent);
                if (bindingSymbol is not null)
                    return InferFreezeExpressionType(exp, bindingSymbol, flow);

                exp.ReferencedSymbol.Fulfill(null);

                exp.Referent.FulfillDataType(DataType.Unknown);
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
                    result = AddImplicitConversionIfNeeded(result, expectedType, flow);
                    CheckTypeCompatibility(expectedType, exp.Value);
                    // TODO use proper check instead of `is not ValueType`
                    if (flow.IsLent(result.Variable) && expectedReturnType.Type is not ValueType)
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
                exp.DataType.Fulfill(new IntegerConstValueType(exp.Value));
                return new ExpressionResult(exp);
            case IStringLiteralExpressionSyntax exp:
                if (stringSymbol is null)
                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span, "Could not find string type for string literal."));
                exp.DataType.Fulfill(stringSymbol?.DeclaresType.With(Capability.Constant, FixedList.Empty<DataType>())
                                     ?? (DataType)DataType.Unknown);
                return new ExpressionResult(exp);
            case IBoolLiteralExpressionSyntax exp:
                exp.DataType.Fulfill(exp.Value ? DataType.True : DataType.False);
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

                    (ReferenceType, BinaryOperator.EqualsEquals, ReferenceType)
                        or (ReferenceType, BinaryOperator.NotEqual, ReferenceType)
                        => InferReferenceEqualityOperatorType(leftOperand, rightOperand),

                    (ValueType<BoolType>, BinaryOperator.EqualsEquals, ValueType<BoolType>)
                        or (ValueType<BoolType>, BinaryOperator.NotEqual, ValueType<BoolType>)
                        or (ValueType<BoolType>, BinaryOperator.And, ValueType<BoolType>)
                        or (ValueType<BoolType>, BinaryOperator.Or, ValueType<BoolType>)
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

                if (type == DataType.Unknown)
                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                        exp.Span, @operator, leftResult.Type, rightResult.Type));

                exp.DataType.Fulfill(type);
                return new ExpressionResult(exp, resultVariable);
            }
            case IIdentifierNameExpressionSyntax exp:
            {
                // Errors reported by InferSymbol
                var symbol = InferSymbol(exp, symbolFilter);
                DataType type;
                ResultVariable? resultVariable = null;
                switch (symbol)
                {
                    case NamedVariableSymbol variableSymbol:
                        resultVariable = flow.Alias(variableSymbol);
                        type = flow.AliasType(variableSymbol);
                        break;
                    case FunctionSymbol functionSymbol:
                        type = functionSymbol.Type;
                        break;
                    default:
                        // It must be a type or namespace name and as such isn't a proper expression
                        type = DataType.Void;
                        break;
                }
                exp.DataType.Fulfill(type);
                return new ExpressionResult(exp, resultVariable);
            }
            case ISpecialTypeNameExpressionSyntax exp:
            {
                InferSymbol(exp);
                // It is a type name and as such isn't a proper expression
                var type = DataType.Void;
                exp.DataType.Fulfill(type);
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
                            result = AddImplicitConversionIfNeeded(result, expType, flow);
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
                            case ValueType<FixedSizeIntegerType> sizedIntegerType:
                                expType = sizedIntegerType.DeclaredType.WithSign().Type;
                                break;
                            case ValueType<BigIntegerType>:
                                // Even if unsigned before, it is signed now
                                expType = DataType.Int;
                                break;
                            case ValueType<PointerSizedIntegerType> pointerSizedIntegerType:
                                expType = pointerSizedIntegerType.DeclaredType.WithSign().Type;
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
                            case ValueType { DeclaredType: IntegerType }:
                            case IntegerConstValueType:
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

                exp.DataType.Fulfill(expType);
                return new ExpressionResult(exp, resultVariable);
            }
            case INewObjectExpressionSyntax exp:
            {
                var arguments = InferArgumentTypes(exp.Arguments, flow);
                var constructingType = typeResolver.EvaluateConstructableBareType(exp.Type);
                ResultVariable? resultVariable = null;
                if (constructingType is null)
                {
                    exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType.Fulfill(DataType.Unknown);
                    resultVariable = CombineResults<ConstructorSymbol>(null, arguments, flow);
                    return new ExpressionResult(exp, resultVariable);
                }

                if (!constructingType.IsFullyKnown)
                {
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                    exp.ReferencedSymbol.Fulfill(null);
                    exp.DataType.Fulfill(DataType.Unknown);
                    resultVariable = CombineResults<ConstructorSymbol>(null, arguments, flow);
                    return new ExpressionResult(exp, resultVariable);
                }

                if (constructingType is BareReferenceType { DeclaredType.IsAbstract: true })
                {
                    diagnostics.Add(OtherSemanticError.CannotConstructAbstractType(file, exp.Type));
                    exp.ReferencedSymbol.Fulfill(null);
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
                var declaredType = typeResolver.Evaluate(exp.Type);
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
                CheckIndependentExpressionType(exp.Condition, DataType.Bool, flow);
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
                exp.DataType.Fulfill(expType);
                flow.Merge(elseFlow);
                var resultVariable = flow.Combine(thenResult.Variable, elseResult?.Variable, exp);
                return new ExpressionResult(exp, resultVariable);
            }
            case IMemberAccessExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow, NonInvocableSymbols);
                var contextType = exp.Context is ISelfExpressionSyntax self ? self.Pseudotype.Assigned() : contextResult.Type;
                var member = exp.Member;
                var contextSymbol = contextType is VoidType && exp.Context is INameExpressionSyntax context
                    ? context.ReferencedSymbol.Result
                    : LookupSymbolForType(contextType);
                if (contextSymbol is null)
                {
                    member.ReferencedSymbol.Fulfill(null);
                    member.DataType.Fulfill(DataType.Unknown);
                    exp.DataType.Fulfill(DataType.Unknown);
                    return new ExpressionResult(exp, contextResult.Variable);
                }
                var memberSymbols = symbolTrees.Children(contextSymbol)
                                               .Where(s => s.Name == member.Name).ToFixedList();
                var type = InferReferencedSymbol(contextType, member, memberSymbols) ?? DataType.Unknown;
                // Access must be applied first so it can account for independent generic parameters.
                type = type.AccessedVia(contextType);
                // Then type parameters can be replaced now that they have the correct access
                if (contextType is NonEmptyType nonEmptyContext)
                    // resolve generic type fields
                    type = nonEmptyContext.ReplaceTypeParametersIn(type);

                var resultVariable = flow.AccessMember(contextResult.Variable, type);
                member.DataType.Fulfill(type);
                exp.DataType.Fulfill(type);
                return new ExpressionResult(exp, resultVariable);
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
                right = AddImplicitConversionIfNeeded(right, left.Type, flow);
                if (!left.Type.IsAssignableFrom(right.Type))
                    diagnostics.Add(TypeError.CannotImplicitlyConvert(file,
                        exp.RightOperand, right.Type, left.Type));
                var resultVariable = flow.Combine(left.Variable, right.Variable, exp);
                exp.DataType.Fulfill(left.Type);
                return new ExpressionResult(exp, resultVariable);
            }
            case ISelfExpressionSyntax exp:
            {
                // InferSelfSymbol reports diagnostics and returns null if there is a problem
                var selfSymbol = InferSelfParameterSymbol(exp);
                var variableResult = selfSymbol is not null ? flow.Alias(selfSymbol) : null;
                var type = flow.AliasType(selfSymbol);
                exp.DataType.Fulfill(type);
                // Works for `readable` but won't work for any future capability constraints
                exp.Pseudotype.Fulfill(selfSymbol?.Type is CapabilityTypeConstraint ? selfSymbol.Type : type);
                return new ExpressionResult(exp, variableResult);
            }
            case INoneLiteralExpressionSyntax exp:
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
                if (result.Type is not ReferenceType { DeclaredType: { } declaredType } promiseType
                    || !declaredType.Equals(Intrinsic.PromiseType))
                {
                    diagnostics.Add(TypeError.CannotAwaitType(file, exp.Span, result.Type));
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
                type = referenceType.IsTemporarilyIsolatedReference ? type : referenceType.With(Capability.Isolated);
                flow.Move(symbol);
                break;
            case ValueType valueType:
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

        exp.Referent.FulfillDataType(type);
        if (exp.Referent is ISelfExpressionSyntax selfExpression)
            selfExpression.Pseudotype.Fulfill(type);
        exp.DataType.Fulfill(type);
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

                type = referenceType.With(Capability.Constant);
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

        exp.Referent.FulfillDataType(type);
        if (exp.Referent is ISelfExpressionSyntax selfExpression)
            selfExpression.Pseudotype.Fulfill(type);
        exp.DataType.Fulfill(type);
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
                else
                    diagnostics.Add(TypeError.OptionalPatternOnNonOptionalType(file, pat, valueType));

                ResolveTypes(pat.Pattern, valueType, resultVariable, flow, isMutableBinding);
                break;
            }
        }
    }

    private ArgumentResults InferArgumentTypes(IFixedList<IExpressionSyntax> arguments, FlowState flow)
        => InferArgumentTypes(null, arguments, flow);

    private ArgumentResults InferArgumentTypes(
        ExpressionResult? selfArgument,
        IFixedList<IExpressionSyntax> arguments,
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

    private void CheckTypes(ArgumentResults arguments, IEnumerable<Parameter> expectedTypes, FlowState flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.EquiZip(expectedTypes))
        {
            AddImplicitConversionIfNeeded(arg, parameter, flow);
            CheckTypeCompatibility(parameter.Type, arg.Syntax);
            // TODO update the expression result
        }
    }

    private static bool TypesAreCompatible(ArgumentResults arguments, IEnumerable<Parameter> expectedTypes, FlowState flow)
    {
        foreach (var (arg, parameter) in arguments.Arguments.EquiZip(expectedTypes))
            if (!TypesAreCompatible(arg, parameter, flow))
                return false;

        return true;
    }

    private static bool TypesAreCompatible(ExpressionResult arg, Parameter parameter, FlowState flow, bool isSelf = false)
    {
        var argType = arg.Type;
        var priorConversion = arg.Syntax.ImplicitConversion;
        var conversion = CreateImplicitConversion(parameter.Type, arg.Type, arg.Variable,
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
        FlowState flow)
        where TSymbol : InvocableSymbol
        => CombineResults(invocable?.SelfParameterType, invocable?.ParameterTypes, invocable?.ReturnType, results, flow);

    /// <param name="function">The function type being invoked or <see langword="null"/> if not fully known.</param>
    private static ResultVariable? CombineResults(
        FunctionType? function,
        ArgumentResults results,
        FlowState flow)
        => CombineResults(null, function?.Parameters, function?.Return, results, flow);

    private static ResultVariable? CombineResults(
        SelfParameter? selfParameterType,
        IFixedList<Parameter>? parameterTypes,
        Return? returnType,
        ArgumentResults results,
        FlowState flow)
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
        FlowState flow)
        where TParameterType : struct, IParameter
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
        FlowState flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IMemberAccessExpressionSyntax exp:
                var contextResult = InferType(exp.Context, flow, NonInvocableSymbols);
                DataType type;
                var member = exp.Member;
                switch (contextResult.Type)
                {
                    case ReferenceType { AllowsWrite: false, AllowsInit: false } contextReferenceType
                        when contextReferenceType.Capability != Capability.Identity: // Another error will be reported
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
                        var memberSymbols = symbolTrees.Children(contextSymbol)
                                                       .Where(s => s is FieldSymbol or MethodSymbol { Kind: MethodKind.Setter })
                                                       .Where(s => s.Name == member.Name).ToFixedList();
                        type = InferReferencedSymbol(contextResult.Type, member, memberSymbols) ?? DataType.Unknown;
                        break;
                }

                // Check for assigning into fields (self is handled by binding mutability analysis)
                if (exp.Context is not ISelfExpressionSyntax
                    && member.ReferencedSymbol.Result is BindingSymbol { IsMutableBinding: false, Name: IdentifierName name })
                    diagnostics.Add(OtherSemanticError.CannotAssignImmutableField(file, exp.Span, name));

                type = type.AccessedVia(contextResult.Type);
                member.DataType.Fulfill(type);
                exp.DataType.Fulfill(type);
                return new(exp, contextResult.Variable);
            case IIdentifierNameExpressionSyntax exp:
                var symbol = InferBindingSymbol(exp);
                switch (symbol)
                {
                    case null:
                        exp.DataType.Fulfill(DataType.Unknown);
                        return new(exp);
                    case NamedVariableSymbol variableSymbol:
                    {
                        exp.DataType.Fulfill(flow.Type(variableSymbol));
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
        // * Function type invocation
        // * Initializer invocation

        ArgumentResults args;
        FunctionType? functionType = null;
        switch (invocation.Expression)
        {
            case IMemberAccessExpressionSyntax exp:
            {
                var contextResult = InferType(exp.Context, flow, NonInvocableSymbols);
                // Make sure to infer argument type *after* the context type
                args = InferArgumentTypes(contextResult, invocation.Arguments, flow);

                if (contextResult.Type is not VoidType)
                {
                    var typeSymbol = LookupSymbolForType(args.Self.Assigned().Type);
                    var fieldSymbols = LookupSymbols<FieldSymbol>(typeSymbol, exp.Member);
                    if (fieldSymbols?.Any(s => s.Type is FunctionType) ?? false)
                    {
                        var fieldSymbol = InferSymbol(exp.Member, fieldSymbols);
                        invocation.ReferencedSymbol.Fulfill(null);
                        if (fieldSymbol is not null)
                        {
                            functionType = (FunctionType)fieldSymbol.Type;
                            exp.Member.DataType.Fulfill(functionType);
                            exp.DataType.Fulfill(functionType);
                        }
                        else
                        {
                            // Matches field, but not a function type. This is an error. Just mark
                            // the types as unknown.
                            exp.Member.DataType.Fulfill(DataType.Unknown);
                            exp.DataType.Fulfill(DataType.Unknown);
                        }
                        break;
                    }
                    var methodSymbols = LookupSymbols<MethodSymbol>(typeSymbol, exp.Member);

                    var method = InferSymbol(invocation, methodSymbols, args, flow);

                    return InferMethodInvocationType(invocation, method, args, flow);
                }

                if (exp.Context is INameExpressionSyntax { ReferencedSymbol.Result: Symbol contextSymbol })
                {
                    var functionSymbols = LookupSymbols<FunctionOrInitializerSymbol>(contextSymbol, exp.Member);
                    functionType = InferSymbol(invocation, functionSymbols, args, flow);
                }

                // No type for function names
                exp.Member.DataType.Fulfill(DataType.Void);
                break;
            }
            case IIdentifierNameExpressionSyntax exp:
            {
                args = InferArgumentTypes(invocation.Arguments, flow);

                var variableSymbols = LookupSymbols<NamedVariableSymbol>(exp);
                if (variableSymbols.Any(s => s.Type is FunctionType))
                {
                    var variableSymbol = InferSymbol(exp, variableSymbols);
                    invocation.ReferencedSymbol.Fulfill(null);
                    if (variableSymbol is not null)
                    {
                        functionType = (FunctionType)variableSymbol.Type;
                        exp.DataType.Fulfill(functionType);
                    }
                    else
                    {
                        // Matches variable, but not a function type. This is an error. Just mark
                        // the type as unknown.
                        exp.DataType.Fulfill(DataType.Unknown);
                    }
                    break;
                }

                var functionOrInitializerSymbols = LookupSymbols<FunctionOrInitializerSymbol>(exp);
                functionType = InferSymbol(invocation, functionOrInitializerSymbols, args, flow);
                break;
            }
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
        IInvocationExpressionSyntax invocation,
        Contextualized<MethodSymbol>? method,
        ArgumentResults arguments,
        FlowState flow)
    {
        if (method is not null)
        {
            var selfParamType = method.SelfParameterType!.Value;
            var selfParamUpperBound = selfParamType.ToUpperBound();
            var selfResult = arguments.Self.Assigned();
            selfResult = AddImplicitConversionIfNeeded(selfResult, selfParamUpperBound, flow);
            AddImplicitMoveIfNeeded(selfResult, selfParamType, flow);
            AddImplicitFreezeIfNeeded(selfResult, selfParamType, flow);
            CheckTypeCompatibility(selfParamUpperBound.Type, selfResult.Syntax);
            CheckTypes(arguments, method.ParameterTypes, flow);
            arguments = arguments with { Self = selfResult };
            // TODO does flow typing need to be applied?
            invocation.DataType.Fulfill(method.ReturnType.Type.ReplaceSelfWith(selfResult.Type));
        }
        else
        {
            invocation.DataType.Fulfill(DataType.Unknown);
        }

        var resultVariable = CombineResults(method, arguments, flow);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        // There are no types for methods
        if (invocation.Expression is IInvocableNameExpressionSyntax invocableName)
            invocableName.DataType.Fulfill(null);

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is IMemberAccessExpressionSyntax memberAccessExpression)
            memberAccessExpression.Member.DataType.Fulfill(DataType.Void);

        return new(invocation, resultVariable);
    }

    private ExpressionResult InferConstructorInvocationType(
        INewObjectExpressionSyntax invocation,
        Contextualized<ConstructorSymbol>? constructor,
        ArgumentResults arguments,
        FlowState flow)
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
        SelfParameter selfParamType,
        FlowState flow)
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
        Parameter selfParam,
        FlowState flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit moves never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `iso`.
        if (selfParam.IsLent) return null;

        if (selfParam.Type is not ReferenceType { IsIsolatedReference: true } toType
            || selfArgType is not ReferenceType { AllowsRecoverIsolation: true } fromType)
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
        SelfParameter selfParamType,
        FlowState flow)
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
        Parameter selfParam,
        FlowState flow,
        bool enact,
        Conversion priorConversion)
    {
        // Implicit freezes never happen if the parameter is lent. `lent` is an explicit request not
        // to force the caller to have `const`
        if (selfParam.IsLent) return null;

        if (selfParam.Type is not ReferenceType { IsConstantReference: true } toType
            || selfArgType is not ReferenceType { AllowsFreeze: true } fromType)
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
        FlowState flow)
    {
        if (functionType is not null)
        {
            CheckTypes(arguments, functionType.Parameters, flow);
            var returnType = functionType.Return.Type;
            invocation.DataType.Fulfill(returnType);
        }
        else
        {
            invocation.DataType.Fulfill(DataType.Unknown);
        }

        var resultVariable = CombineResults(functionType, arguments, flow);
        flow.Restrict(resultVariable, invocation.DataType.Assigned());

        // There are no types for functions
        if (invocation.Expression is IInvocableNameExpressionSyntax { DataType.IsFulfilled: false } invocableName)
            invocableName.DataType.Fulfill(null);

        return new(invocation, resultVariable);
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
                block.DataType.Fulfill(blockType);
                return new(block, blockResult?.Variable);
            case IResultStatementSyntax result:
                // Can't return an ExpressionResult for the statement, return it for the expression instead
                return InferType(result.Expression, flow);
        }
    }

    private static bool AllSymbols(Symbol symbol) => true;

    private static bool NonInvocableSymbols(Symbol symbol) => symbol is not InvocableSymbol;

    private BindingSymbol? InferBindingSymbol(IVariableNameExpressionSyntax variableNameExpressionSyntax)
    {
        return variableNameExpressionSyntax switch
        {
            IIdentifierNameExpressionSyntax nameExpression => InferBindingSymbol(nameExpression),
            ISelfExpressionSyntax selfExpression => InferSelfParameterSymbol(selfExpression),
            _ => throw ExhaustiveMatch.Failed(variableNameExpressionSyntax)
        };
    }

    private BindingSymbol? InferBindingSymbol(
        IIdentifierNameExpressionSyntax nameExpression,
        Func<Symbol, bool>? symbolFilter = null)
        => (BindingSymbol?)InferSymbol(nameExpression, symbolFilter, true);

    private Symbol? InferSymbol(IIdentifierNameExpressionSyntax nameExpression, Func<Symbol, bool>? symbolFilter, bool bindingOnly = false)
    {
        if (nameExpression.Name is null)
        {
            // Name unknown, no error
            nameExpression.ReferencedSymbol.Fulfill(null);
            return null;
        }

        // First look for local variables
        var variableSymbols = LookupSymbols<NamedVariableSymbol>(nameExpression, symbolFilter);

        if (bindingOnly || variableSymbols.Count != 0)
            return InferSymbol(nameExpression, variableSymbols);

        var symbols = LookupSymbols(nameExpression, symbolFilter);
        return InferSymbol(nameExpression, symbols);
    }

    private void InferSymbol(
        ISpecialTypeNameExpressionSyntax nameExpression)
    {
        var symbol = symbolTrees.PrimitiveSymbolTree.LookupSymbol(nameExpression.Name);
        nameExpression.ReferencedSymbol.Fulfill(symbol);
    }

    private SelfParameterSymbol? InferSelfParameterSymbol(ISelfExpressionSyntax selfExpression)
    {
        switch (containingSymbol)
        {
            default:
                throw ExhaustiveMatch.Failed(containingSymbol);
            case MethodSymbol _:
            case ConstructorSymbol _:
            case InitializerSymbol _:
                var symbols = LookupSymbols<SelfParameterSymbol>(containingSymbol);
                return InferSymbol(selfExpression, symbols);
            case FunctionSymbol _:
                diagnostics.Add(selfExpression.IsImplicit
                    ? OtherSemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                    : OtherSemanticError.SelfOutsideMethod(file, selfExpression.Span));
                selfExpression.ReferencedSymbol.Fulfill(null);
                return null;
        }
    }

    private FunctionType? InferSymbol(
        IInvocationExpressionSyntax invocation,
        FixedSet<FunctionOrInitializerSymbol> functionOrInitializerSymbols,
        ArgumentResults arguments,
        FlowState flow)
    {
        var validOverloads = SelectOverload(null, functionOrInitializerSymbols, arguments, flow);
        FunctionType? functionType;
        switch (validOverloads.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(file, invocation));
                invocation.ReferencedSymbol.Fulfill(null);
                functionType = null;
                break;
            case 1:
                var functionOrInitializer = validOverloads.Single();
                invocation.ReferencedSymbol.Fulfill(functionOrInitializer.Symbol);
                functionType = new FunctionType(functionOrInitializer.ParameterTypes, functionOrInitializer.ReturnType);
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, invocation));
                invocation.ReferencedSymbol.Fulfill(null);
                functionType = null;
                break;
        }

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is IInvocableNameExpressionSyntax nameExpression)
            nameExpression.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);

        return functionType;
    }

    private Contextualized<MethodSymbol>? InferSymbol(
        IInvocationExpressionSyntax invocation,
        FixedSet<MethodSymbol>? methodSymbols,
        ArgumentResults arguments,
        FlowState flow)
    {
        var selfResult = arguments.Self.Assigned();
        var selfArgumentType = selfResult.Type;

        Contextualized<MethodSymbol>? method = null;
        if (methodSymbols is not null)
        {
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
        }
        else
            invocation.ReferencedSymbol.Fulfill(null);

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is IInvocableNameExpressionSyntax nameExpression)
            nameExpression.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);

        return method;
    }

    private Contextualized<ConstructorSymbol>? InferSymbol(
        INewObjectExpressionSyntax invocation,
        FixedSet<ConstructorSymbol>? constructorSymbols,
        ArgumentResults arguments,
        FlowState flow)
    {
        Contextualized<ConstructorSymbol>? constructor = null;
        if (constructorSymbols is not null)
        {
            var validOverloads = SelectOverload(invocation.Type.NamedType.Assigned(), constructorSymbols, arguments, flow);
            switch (validOverloads.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, invocation.Span));
                    invocation.ReferencedSymbol.Fulfill(null);
                    break;
                case 1:
                    constructor = validOverloads.Single();
                    invocation.ReferencedSymbol.Fulfill(constructor.Symbol);
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, invocation.Span));
                    invocation.ReferencedSymbol.Fulfill(null);
                    break;
            }
        }
        else
            invocation.ReferencedSymbol.Fulfill(null);

        return constructor;
    }

    private TSymbol? InferSymbol<TNameSymbol, TSymbol>(
        INameExpressionSyntax<TNameSymbol> exp,
        FixedSet<TSymbol> symbols)
        where TNameSymbol : Symbol
        where TSymbol : TNameSymbol
    {
        switch (symbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(file, exp.Span));
                exp.ReferencedSymbol.Fulfill(null);
                return null;
            case 1:
                var symbol = symbols.Single();
                exp.ReferencedSymbol.Fulfill(symbol);
                return symbol;
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(file, exp.Span));
                exp.ReferencedSymbol.Fulfill(null);
                return null;
        }
    }

    private static FixedSet<Symbol> LookupSymbols(IStandardNameExpressionSyntax exp, Func<Symbol, bool>? symbolFilter = null)
    {
        symbolFilter ??= AllSymbols;
        return exp.LookupInContainingScope().Select(p => p.Result).Where(symbolFilter).ToFixedSet();
    }

    private static FixedSet<TSymbol> LookupSymbols<TSymbol>(
        IStandardNameExpressionSyntax exp,
        Func<TSymbol, bool>? symbolFilter = null)
        where TSymbol : Symbol
    {
        symbolFilter ??= AllSymbols;
        return exp.LookupInContainingScope().Select(p => p.Downcast().As<TSymbol>())
                             .WhereNotNull().Select(p => p.Result).Where(symbolFilter).ToFixedSet();
    }

    /// <returns>The symbols with the given name in the context, or <see langword="null"/> if the
    /// context is unknown.</returns>
    [return: NotNullIfNotNull(nameof(contextSymbol))]
    private FixedSet<TSymbol>? LookupSymbols<TSymbol>(Symbol? contextSymbol, IStandardNameExpressionSyntax exp)
        where TSymbol : Symbol
    {
        if (contextSymbol is null) return null;
        var name = exp.Name;
        return symbolTrees.Children(contextSymbol).OfType<TSymbol>().Where(s => s.Name == name).ToFixedSet();
    }

    private FixedSet<TSymbol> LookupSymbols<TSymbol>(Symbol contextSymbol)
        where TSymbol : Symbol
        => symbolTrees.Children(contextSymbol).OfType<TSymbol>().ToFixedSet();

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
            diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(file, inExpression, iterableType));
            return (result, declaredType ?? DataType.Unknown);
        }

        exp.NextMethod.Fulfill(nextMethod);

        // iteratorType is NonEmptyType because it has a `next()` method
        DataType iteratedType = nextMethod.Return.Type is OptionalType optionalType
            ? ((NonEmptyType)iteratorType).ReplaceTypeParametersIn(optionalType.Referent)
            : throw new UnreachableCodeException();

        if (declaredType is not null)
        {
            var conversion = CreateImplicitConversion(declaredType, iteratedType, null, inExpression.ImplicitConversion, flow,
                out _, enact: false);
            iteratedType = conversion is null ? iteratedType : conversion.Apply(iteratedType);
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
        Pseudotype contextType,
        IStandardNameExpressionSyntax exp,
        IFixedList<Symbol> matchingSymbols)
    {
        // TODO resolve getters overloaded on self type
        switch (matchingSymbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMember(file, exp.Span));
                exp.ReferencedSymbol.Fulfill(null);
                return null;
            case 1:
                var memberSymbol = matchingSymbols.Single();
                DataType type = DataType.Void;
                switch (memberSymbol)
                {
                    case FieldSymbol fieldSymbol:
                    {
                        type = fieldSymbol.Type;

                        if (fieldSymbol.IsMutableBinding
                            && contextType is ReferenceType { Capability: var contextCapability }
                            && contextCapability == Capability.Identity)
                        {
                            diagnostics.Add(TypeError.CannotAccessMutableBindingFieldOfIdentityReference(file, exp, contextType));
                            type = DataType.Unknown;
                        }

                        break;
                    }
                    case MethodSymbol { Kind: MethodKind.Getter } getterSymbol:
                    {
                        type = getterSymbol.Return.Type;
                        var selfType = getterSymbol.SelfParameterType.Type;
                        if (!selfType.IsAssignableFrom(contextType))
                            // TODO error message not correct, should not ToUpperBound()
                            diagnostics.Add(TypeError.CannotImplicitlyConvert(file, exp, contextType.ToUpperBound(), selfType.ToUpperBound()));
                        break;
                    }
                    case MethodSymbol { Kind: MethodKind.Setter } setterSymbol:
                    {
                        // TODO this indicates that setters should be their own symbol?
                        type = setterSymbol.Parameters.Single().Type;
                        var selfType = setterSymbol.SelfParameterType.Type;
                        if (!selfType.IsAssignableFrom(contextType))
                            // TODO error message not correct, should not ToUpperBound()
                            diagnostics.Add(TypeError.CannotImplicitlyConvert(file, exp, contextType.ToUpperBound(),
                                selfType.ToUpperBound()));
                        break;
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
        return rangeSymbol?.DeclaresType.With(Capability.Constant, FixedList.Empty<DataType>())
               ?? (DataType)DataType.Unknown;
    }

    private static bool ExplicitConversionTypesAreCompatible(IExpressionSyntax expression, bool safeOnly, DataType convertToType)
    {
        return (expression.ConvertedDataType.Assigned(), convertToType) switch
        {
            // Safe conversions
            (ValueType<BoolType>, ValueType { DeclaredType: IntegerType }) => true,
            (BoolConstValueType, ValueType { DeclaredType: IntegerType }) => true,
            (ValueType { DeclaredType: IntegerType { IsSigned: false } }, ValueType { DeclaredType: BigIntegerType }) => true,
            (ValueType { DeclaredType: IntegerType }, ValueType { DeclaredType: BigIntegerType { IsSigned: true } }) => true,
            (ValueType<FixedSizeIntegerType> from, ValueType<FixedSizeIntegerType> to)
                when from.DeclaredType.Bits < to.DeclaredType.Bits
                     || (from.DeclaredType.Bits == to.DeclaredType.Bits && from.DeclaredType.IsSigned == to.DeclaredType.IsSigned)
                => true,
            // TODO conversions for constants
            // Unsafe conversions
            (ValueType { DeclaredType: IntegerType }, ValueType { DeclaredType: IntegerType }) => !safeOnly,
            _ => convertToType.IsAssignableFrom(expression.ConvertedDataType),
        };
    }

    private static FixedSet<Contextualized<TSymbol>> SelectOverload<TSymbol>(
        DataType? contextType,
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
        DataType? contextType,
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
            // Is self arg compatible?
            if (s.SelfParameterType is SelfParameter selfParameterType
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

    private static SelfParameter? SelfParameterTypeOrNull(InvocableSymbol symbol)
    {
        if (symbol is MethodSymbol { SelfParameterType: var selfType })
            return selfType;
        return null;
    }

    #region LookupSymbolForType
    // TODO move these methods somewhere else, and possibly make them more efficient
    private TypeSymbol? LookupSymbolForType(Pseudotype pseudotype)
    {
        return pseudotype switch
        {
            DataType t => LookupSymbolForType(t),
            CapabilityTypeConstraint t => LookupSymbolForType(t.BareType.DeclaredType),
            _ => throw ExhaustiveMatch.Failed(pseudotype)
        };
    }

    private TypeSymbol? LookupSymbolForType(DataType dataType)
    {
        return dataType switch
        {
            UnknownType _ => null,
            ConstValueType _ => null,
            OptionalType _ => null,
            EmptyType _ => null,
            FunctionType _ => null,
            ReferenceType t => LookupSymbolForType(t),
            ValueType t => LookupSymbolForType(t),
            GenericParameterType t => LookupSymbolForType(t),
            ViewpointType t => LookupSymbolForType(t.Referent),
            _ => throw ExhaustiveMatch.Failed(dataType),
        };
    }

    private TypeSymbol LookupSymbolForType(ValueType type)
        => LookupSymbolForType(type.DeclaredType);

    private TypeSymbol LookupSymbolForType(ReferenceType type)
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
