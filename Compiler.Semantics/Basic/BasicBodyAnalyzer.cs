using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
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
    private readonly SymbolTreeBuilder symbolTreeBuilder;
    private readonly SymbolForest symbolTrees;
    private readonly ObjectTypeSymbol? stringSymbol;
    private readonly Diagnostics diagnostics;
    private readonly DataType? returnType;
    private readonly TypeResolver typeResolver;
    private readonly ReferenceCapabilitiesSnapshot parameterCapabilities;
    private readonly SharingRelationSnapshot parameterSharing;

    public BasicBodyAnalyzer(
        IFunctionDeclarationSyntax containingDeclaration,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics,
        DataType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IAssociatedFunctionDeclarationSyntax containingDeclaration,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics,
        DataType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConstructorDeclarationSyntax containingDeclaration,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics,
        DataType returnType)
        : this(containingDeclaration,
            containingDeclaration.Parameters.OfType<INamedParameterSyntax>()
                                 .Select(p => p.Symbol.Result)
                                 .Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IConcreteMethodDeclarationSyntax containingDeclaration,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics,
        DataType returnType)
        : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result).Prepend<BindingSymbol>(containingDeclaration.SelfParameter.Symbol.Result),
            symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
    { }

    public BasicBodyAnalyzer(
        IFieldDeclarationSyntax containingDeclaration,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics)
        : this(containingDeclaration, Enumerable.Empty<BindingSymbol>(),
            symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, null)
    { }

    private BasicBodyAnalyzer(
        IEntityDeclarationSyntax containingDeclaration,
        IEnumerable<BindingSymbol> parameterSymbols,
        SymbolTreeBuilder symbolTreeBuilder,
        SymbolForest symbolTrees,
        ObjectTypeSymbol? stringSymbol,
        Diagnostics diagnostics,
        DataType? returnType)
    {
        file = containingDeclaration.File;
        containingSymbol = (InvocableSymbol)containingDeclaration.Symbol.Result;
        this.symbolTreeBuilder = symbolTreeBuilder;
        this.stringSymbol = stringSymbol;
        this.diagnostics = diagnostics;
        this.symbolTrees = symbolTrees;
        this.returnType = returnType;
        typeResolver = new TypeResolver(file, diagnostics);
        var capabilities = new ReferenceCapabilities();
        var sharing = new SharingRelation();
        bool nonLentParametersReferenceDeclared = false;
        var lentParameterNumber = 0;
        foreach (var parameterSymbol in parameterSymbols)
        {
            capabilities.Declare(parameterSymbol);
            sharing.Declare(parameterSymbol);
            if (parameterSymbol.DataType is not ReferenceType { Capability: var capability })
                continue;

            if (capability.IsLent)
                sharing.DeclareLentParameterReference(parameterSymbol, ++lentParameterNumber);
            else if (capability != ReferenceCapability.Isolated
                     && capability != ReferenceCapability.Constant
                     && capability != ReferenceCapability.Identity)
            {
                if (!nonLentParametersReferenceDeclared)
                {
                    sharing.DeclareNonLentParametersReference();
                    nonLentParametersReferenceDeclared = true;
                }
                sharing.Union(ExternalReference.NonParameters, parameterSymbol);
            }
        }
        // TODO assume sharing between parameters unless const, iso, or lent
        parameterCapabilities = capabilities.Snapshot();
        parameterSharing = sharing.Snapshot();
    }

    public void ResolveTypes(IBodySyntax body)
    {
        var flow = new FlowState(parameterCapabilities, parameterSharing);
        foreach (var statement in body.Statements)
            ResolveTypes(statement, StatementContext.BodyLevel, flow);
    }

    /// <summary>
    /// Resolve the types in a statement. If the statement was a <see cref="IResultStatementSyntax"/>
    /// the type expression type is returned. Returns <see langword="null"/> otherwise.
    /// </summary>
    private DataType? ResolveTypes(
        IStatementSyntax statement,
        StatementContext context,
        FlowState flow)
    {
        if (context == StatementContext.AfterResult)
            diagnostics.Add(SemanticError.StatementAfterResult(file, statement.Span));
        switch (statement)
        {
            default:
                throw ExhaustiveMatch.Failed(statement);
            case IVariableDeclarationStatementSyntax variableDeclaration:
                ResolveTypes(variableDeclaration, flow);
                break;
            case IExpressionStatementSyntax expressionStatement:
                flow.NewResult();
                InferType(expressionStatement.Expression, flow);
                // At the end of the statement, any result reference is discarded
                flow.DropCurrentResult();
                break;
            case IResultStatementSyntax resultStatement:
                flow.NewResult();
                var type = InferType(resultStatement.Expression, flow);
                if (context == StatementContext.BodyLevel)
                    diagnostics.Add(SemanticError.ResultStatementInBody(file, resultStatement.Span));

                // Return type for use in determining block type. Keep result shared for use in
                // parent expression.
                return type;
        }
        return null;
    }

    private void ResolveTypes(
        IVariableDeclarationStatementSyntax variableDeclaration,
        FlowState flow)
    {
        flow.NewResult();
        _ = InferType(variableDeclaration.Initializer, flow);
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

        if (variableDeclaration.Initializer is not null)
        {
            var initializerType = AddImplicitConversionIfNeeded(variableDeclaration.Initializer, variableType, flow);
            if (!variableType.IsAssignableFrom(initializerType))
                diagnostics.Add(TypeError.CannotImplicitlyConvert(file, variableDeclaration.Initializer, initializerType, variableType));
        }

        var symbol = new VariableSymbol(containingSymbol, variableDeclaration.Name,
            variableDeclaration.DeclarationNumber.Result, variableDeclaration.IsMutableBinding, variableType, isParameter: false);
        variableDeclaration.Symbol.Fulfill(symbol);
        symbolTreeBuilder.Add(symbol);
        flow.Declare(symbol);
        // Union with the initializer result thereby unioning with any references used in the result
        flow.UnionWithCurrentResult(symbol);
        // Drop out result for end of statement
        flow.DropCurrentResult();
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

    public void CheckType(IExpressionSyntax? expression, DataType expectedType)
    {
        if (expression is null) return;
        var flow = new FlowState();
        flow.NewResult();
        CheckType(expression, expectedType);
        flow.DropCurrentResult();
    }

    private void CheckType(
        IExpressionSyntax expression,
        DataType expectedType,
        FlowState flow)
    {
        _ = InferType(expression, flow);
        _ = AddImplicitConversionIfNeeded(expression, expectedType, flow);
        CheckTypeCompatibility(expectedType, expression);
    }

    /// <summary>
    /// Create an implicit conversion if needed and allowed.
    /// </summary>
    private static DataType AddImplicitConversionIfNeeded(
        IExpressionSyntax expression,
        DataType expectedType,
        FlowState flow)
    {
        var fromType = expression.DataType.Assigned();
        var conversion = CreateImplicitConversion(expectedType, fromType, expression.ImplicitConversion, flow);
        if (conversion is not null) expression.AddConversion(conversion);
        return expression.ConvertedDataType.Assigned();
    }

    private static ChainedConversion? CreateImplicitConversion(
        DataType toType,
        DataType fromType,
        Conversion priorConversion,
        FlowState flow)
    {
        switch (toType, fromType)
        {
            case (DataType to, DataType from) when from == to:
                return null;
            case (OptionalType { Referent: var to }, OptionalType { Referent: var from }):
                // Direct subtype
                if (to.IsAssignableFrom(from))
                    return null;
                var liftedConversion = CreateImplicitConversion(to, from, IdentityConversion.Instance, flow);
                return liftedConversion is null ? null : new LiftedConversion(liftedConversion, priorConversion);
            case (OptionalType { Referent: var to }, not OptionalType):
                if (!to.IsAssignableFrom(fromType))
                {
                    var conversion = CreateImplicitConversion(to, fromType, priorConversion, flow);
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
                when to.DeclaredType.IsAssignableFrom(from.DeclaredType):
            {
                // Try to recover const
                if (flow.CurrentResultIsIsolated()
                    || (to.IsLentReference && flow.LendCurrentResultConst()))
                    return new RecoverConst(priorConversion);

                return null;
            }
            case (ObjectType { IsIsolatedReference: true } to, ObjectType { AllowsRecoverIsolation: true } from):
            {
                // Try to recover isolation
                if (to.DeclaredType.IsAssignableFrom(from.DeclaredType) && flow.CurrentResultIsIsolated())
                    return new RecoverIsolation(priorConversion);

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
    private DataType? InferType(IExpressionSyntax? expression, FlowState flow)
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
                var referentType = InferType(exp.Referent, flow);
                DataType type;
                if (referentType is ReferenceType referenceType)
                    type = referenceType.With(ReferenceCapability.Identity);
                else
                {
                    diagnostics.Add(TypeError.CannotIdNonReferenceType(file, exp.Span, referentType));
                    type = DataType.Unknown;
                }
                // Don't need to alias the symbol or union with result in flow because ids don't matter
                return exp.DataType = type;
            }
            case IMoveExpressionSyntax exp:
                switch (exp.Referent)
                {
                    case ISimpleNameExpressionSyntax nameExpression:
                        var symbol = InferNameSymbol(nameExpression);
                        if (symbol is not BindingSymbol bindingSymbol)
                            throw new NotImplementedException("Raise error about `move` from non-variable");

                        var type = flow.Type(bindingSymbol);
                        switch (type)
                        {
                            case ReferenceType referenceType:
                                if (!referenceType.AllowsMove)
                                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span,
                                        "Reference capability does not allow moving"));
                                if (!flow.IsIsolated(bindingSymbol))
                                    diagnostics.Add(TypeError.CannotMoveValue(file, exp));

                                type = referenceType.With(ReferenceCapability.Isolated);
                                flow.Move(bindingSymbol);
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

                        exp.ReferencedSymbol.Fulfill(bindingSymbol);

                        const ExpressionSemantics semantics = ExpressionSemantics.IsolatedReference;
                        nameExpression.Semantics = semantics;
                        nameExpression.DataType = type;
                        exp.Semantics = semantics;
                        return exp.DataType = type;
                    case IMoveExpressionSyntax:
                        throw new NotImplementedException("Raise error about `move move` expression");
                    default:
                        throw new NotImplementedException("Tried to move out of expression type that isn't implemented");
                }
            case IFreezeExpressionSyntax exp:
                switch (exp.Referent)
                {
                    case ISimpleNameExpressionSyntax nameExpression:
                        var symbol = InferNameSymbol(nameExpression);
                        if (symbol is not BindingSymbol bindingSymbol)
                            throw new NotImplementedException("Raise error about `freeze` of non-variable");

                        var type = flow.Type(bindingSymbol);
                        switch (type)
                        {
                            case ReferenceType referenceType:
                                if (!referenceType.AllowsRecoverIsolation)
                                    diagnostics.Add(TypeError.NotImplemented(file, exp.Span,
                                        "Reference capability does not allow freezing"));
                                if (!flow.IsIsolated(bindingSymbol))
                                    diagnostics.Add(TypeError.CannotFreezeValue(file, exp));

                                type = referenceType.With(ReferenceCapability.Constant);
                                flow.Freeze(bindingSymbol);
                                break;
                            case UnknownType:
                                type = DataType.Unknown;
                                break;
                            default:
                                throw new NotImplementedException("Non-freezable type can't be frozen.");
                        }
                        // Now that it is frozen, union with result to track sharing
                        // Alias not needed because it is already `const`
                        flow.UnionWithCurrentResult(bindingSymbol);

                        exp.ReferencedSymbol.Fulfill(bindingSymbol);

                        const ExpressionSemantics semantics = ExpressionSemantics.ConstReference;
                        nameExpression.Semantics = semantics;
                        nameExpression.DataType = type;
                        exp.Semantics = semantics;
                        return exp.DataType = type;
                    case IMoveExpressionSyntax:
                        throw new NotImplementedException("Raise error about `freeze move` expression.");
                    default:
                        throw new NotImplementedException("Tried to freeze expression type that isn't implemented.");
                }
            case IReturnExpressionSyntax exp:
            {
                if (returnType is null)
                    throw new NotImplementedException("Return statement in field initializer.");
                if (exp.Value is not null)
                {
                    var expectedReturnType = returnType;
                    InferType(exp.Value, flow);
                    // local variables are no longer in scope and isolated parameters have no external references
                    flow.DropBindingsForReturn();
                    AddImplicitConversionIfNeeded(exp.Value, expectedReturnType, flow);
                    CheckTypeCompatibility(expectedReturnType, exp.Value);
                }
                else if (returnType == DataType.Never)
                    diagnostics.Add(TypeError.CannotReturnFromNeverFunction(file, exp.Span));
                else if (returnType != DataType.Void)
                    diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file, exp.Span, returnType ?? DataType.Unknown));

                // Return expressions always have the type Never
                return exp.DataType;
            }
            case IIntegerLiteralExpressionSyntax exp:
                return exp.DataType = new IntegerConstantType(exp.Value);
            case IStringLiteralExpressionSyntax exp:
                return exp.DataType = stringSymbol?.DeclaresType.With(ReferenceCapability.Constant, FixedList<DataType>.Empty) ?? (DataType)DataType.Unknown;
            case IBoolLiteralExpressionSyntax exp:
                return exp.DataType = exp.Value ? DataType.True : DataType.False;
            case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
            {
                var leftOperand = binaryOperatorExpression.LeftOperand;
                var leftType = InferType(leftOperand, flow);
                var leftResult = flow.CurrentResult;
                var @operator = binaryOperatorExpression.Operator;
                flow.NewResult();
                var rightOperand = binaryOperatorExpression.RightOperand;
                var rightType = InferType(rightOperand, flow);
                flow.UnionWithCurrentResultAndDrop(leftResult);

                // If either is unknown, then we can't know whether there is a a problem.
                // Note that the operator could be overloaded
                if (!leftType.IsFullyKnown || !rightType.IsFullyKnown)
                    return binaryOperatorExpression.DataType = DataType.Unknown;

                DataType type = (leftType, @operator, rightType) switch
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
                        => InferNumericOperatorType(leftOperand, rightOperand, flow),
                    (NumericType, BinaryOperator.EqualsEquals, NumericType)
                        or (NumericType, BinaryOperator.NotEqual, NumericType)
                        or (NumericType, BinaryOperator.LessThan, NumericType)
                        or (NumericType, BinaryOperator.LessThanOrEqual, NumericType)
                        or (NumericType, BinaryOperator.GreaterThan, NumericType)
                        or (NumericType, BinaryOperator.GreaterThanOrEqual, NumericType)
                        => InferComparisonOperatorType(leftOperand, rightOperand, flow),

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
                        => throw new NotImplementedException("Type analysis of range operators"),
                    _ => DataType.Unknown
                };

                if (type == DataType.Unknown)
                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                        binaryOperatorExpression.Span, @operator, leftType, rightType));

                binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                return binaryOperatorExpression.DataType = type;
            }
            case ISimpleNameExpressionSyntax exp:
            {
                // Errors reported by InferNameSymbol
                var symbol = InferNameSymbol(exp);
                DataType type;
                ExpressionSemantics referenceSemantics;
                if (symbol is VariableSymbol variableSymbol)
                {
                    flow.UnionWithCurrentResult(variableSymbol);
                    flow.Alias(variableSymbol);

                    type = flow.Type(variableSymbol);
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
                return exp.DataType = type;
            }
            case IUnaryOperatorExpressionSyntax exp:
            {
                var @operator = exp.Operator;
                var operandType = InferType(exp.Operand, flow);
                DataType expType;
                switch (@operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed(@operator);
                    case UnaryOperator.Not:
                        if (operandType is BoolConstantType boolType)
                            expType = boolType.Not();
                        else
                        {
                            expType = DataType.Bool;
                            _ = AddImplicitConversionIfNeeded(exp.Operand, expType, flow);
                            CheckTypeCompatibility(expType, exp.Operand);
                        }
                        break;
                    case UnaryOperator.Minus:
                        switch (operandType)
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
                                    exp.Span, @operator, operandType));
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                    case UnaryOperator.Plus:
                        switch (operandType)
                        {
                            case NumericType:
                            case UnknownType _:
                                expType = operandType;
                                break;
                            default:
                                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                    exp.Span, @operator, operandType));
                                expType = DataType.Unknown;
                                break;
                        }
                        break;
                }
                return exp.DataType = expType;
            }
            case INewObjectExpressionSyntax exp:
            {
                // Give each argument a distinct result, then union them all
                var argumentTypes = new List<DataType>();
                var argumentResults = new List<ResultVariable>();
                foreach (var argument in exp.Arguments)
                {
                    argumentTypes.Add(InferType(argument, flow));
                    argumentResults.Add(flow.CurrentResult);
                    flow.NewResult();
                }
                foreach (var result in argumentResults)
                    flow.UnionWithCurrentResultAndDrop(result);

                var constructingType = typeResolver.EvaluateBareType(exp.Type);
                if (!constructingType.IsFullyKnown)
                {
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
                }

                var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new NotImplementedException();
                DataType constructedType;
                var constructorSymbols = symbolTrees.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedSet();
                constructorSymbols = SelectOverload(constructorSymbols, argumentTypes.ToFixedList());
                switch (constructorSymbols.Count)
                {
                    case 0:
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        constructedType = DataType.Unknown;
                        break;
                    case 1:
                        var constructorSymbol = constructorSymbols.Single();
                        exp.ReferencedSymbol.Fulfill(constructorSymbol);
                        var contextType = (NonEmptyType)constructingType;
                        foreach (var (arg, parameterDataType) in exp.Arguments
                            .Zip(constructorSymbol.ParameterDataTypes.Select(contextType.ReplaceTypeParametersIn)))
                        {
                            AddImplicitConversionIfNeeded(arg, parameterDataType, flow);
                            CheckTypeCompatibility(parameterDataType, arg);
                        }
                        constructedType = contextType.ReplaceTypeParametersIn(constructorSymbol.ReturnDataType);
                        break;
                    default:
                        diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        constructedType = DataType.Unknown;
                        break;
                }
                return exp.DataType = constructedType;
            }
            case IForeachExpressionSyntax exp:
            {
                var declaredType = typeResolver.Evaluate(exp.Type);
                var expressionType = CheckForeachInType(declaredType, exp.InExpression, flow);
                var variableType = declaredType ?? expressionType.ToNonConstantType();
                var symbol = new VariableSymbol(containingSymbol, exp.VariableName,
                    exp.DeclarationNumber.Result, exp.IsMutableBinding, variableType, false);
                exp.Symbol.Fulfill(symbol);
                symbolTreeBuilder.Add(symbol);
                flow.Declare(symbol);
                // Union with the result of the `in` expression
                flow.UnionWithCurrentResult(symbol);
                flow.DropCurrentResult(); // Don't mix the foreach expression into the block

                // TODO check the break types
                InferBlockType(exp.Block, flow);
                flow.DropCurrentResult();

                flow.Drop(symbol);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            }
            case IWhileExpressionSyntax exp:
            {
                CheckType(exp.Condition, DataType.Bool, flow);
                // Condition expression result is complete
                flow.DropCurrentResult();
                flow.NewResult();
                InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            }
            case ILoopExpressionSyntax exp:
                InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            case IInvocationExpressionSyntax exp:
                return InferInvocationType(exp, flow);
            case IUnsafeExpressionSyntax exp:
            {
                exp.DataType = InferType(exp.Expression, flow);
                exp.Semantics = exp.Expression.Semantics.Assigned();
                return exp.ConvertedDataType.Assigned();
            }
            case IIfExpressionSyntax exp:
            {
                CheckType(exp.Condition, DataType.Bool, flow);
                flow.DropCurrentResult();
                flow.NewResult();
                var elseClause = exp.ElseClause;
                FlowState? elseFlow = null;
                if (elseClause is not null) elseFlow = flow.Copy();
                var thenType = InferBlockType(exp.ThenBlock, flow);
                DataType? elseType = null;
                switch (elseClause)
                {
                    default:
                        throw ExhaustiveMatch.Failed(elseClause);
                    case null:
                        break;
                    case IIfExpressionSyntax _:
                    case IBlockExpressionSyntax _:
                        var elseExpression = (IExpressionSyntax)elseClause;
                        elseType = InferType(elseExpression, elseFlow!);
                        break;
                    case IResultStatementSyntax resultStatement:
                        elseType = InferType(resultStatement.Expression, elseFlow!);
                        break;
                }
                DataType expType;
                if (elseType is null)
                    expType = thenType.ToOptional();
                else
                    // TODO unify the two types
                    expType = thenType;
                // TODO correct reference semantics?
                exp.Semantics = expType.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                return exp.DataType = expType;
            }
            case IQualifiedNameExpressionSyntax exp:
            {
                var contextType = InferType(exp.Context, flow);
                var member = exp.Member;
                var contextSymbol = contextType is VoidType && exp.Context is INameExpressionSyntax context
                    ? context.ReferencedSymbol.Result
                    : LookupSymbolForType(contextType);
                if (contextSymbol is null)
                {
                    member.ReferencedSymbol.Fulfill(null);
                    member.DataType = DataType.Unknown;
                    exp.Semantics ??= ExpressionSemantics.CopyValue;
                    return exp.DataType = DataType.Unknown;
                }
                var memberSymbols = symbolTrees.Children(contextSymbol)
                                               .Where(s => s.Name == member.Name).ToFixedList();
                var type = InferReferencedSymbol(member, memberSymbols) ?? DataType.Unknown;
                if (contextType is NonEmptyType nonEmptyContext)
                    // resolve generic type fields
                    type = nonEmptyContext.ReplaceTypeParametersIn(type);
                // If there could be no write aliases, then the current result does not share with anything
                if (!type.AllowsWriteAliases)
                    flow.SplitCurrentResult();
                var semantics = type.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                member.Semantics = semantics;
                member.DataType = type;
                exp.Semantics = semantics;
                return exp.DataType = type;
            }
            case IBreakExpressionSyntax exp:
                InferType(exp.Value, flow);
                return exp.DataType = DataType.Never;
            case INextExpressionSyntax exp:
                return exp.DataType = DataType.Never;
            case IAssignmentExpressionSyntax exp:
            {
                var left = InferAssignmentTargetType(exp.LeftOperand, flow);
                InferType(exp.RightOperand, flow);
                AddImplicitConversionIfNeeded(exp.RightOperand, left, flow);
                var right = exp.RightOperand.ConvertedDataType.Assigned();
                if (!left.IsAssignableFrom(right))
                    diagnostics.Add(TypeError.CannotImplicitlyConvert(file,
                        exp.RightOperand, right, left));
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            }
            case ISelfExpressionSyntax exp:
            {
                // InferSelfSymbol reports diagnostics and returns null if there is a problem
                var selfSymbol = InferSelfSymbol(exp);
                if (selfSymbol is not null)
                {
                    flow.UnionWithCurrentResult(selfSymbol);
                    flow.Alias(selfSymbol);
                }
                var type = flow.Type(selfSymbol);
                // TODO is this correct?
                var referenceSemantics = ExpressionSemantics.MutableReference;
                exp.Semantics = type.Semantics.ToExpressionSemantics(referenceSemantics);
                return exp.DataType = type;
            }
            case INoneLiteralExpressionSyntax exp:
                return exp.DataType = DataType.None;
            case IBlockExpressionSyntax blockSyntax:
                return InferBlockType(blockSyntax, flow);
            case IConversionExpressionSyntax exp:
            {
                var referentType = InferType(exp.Referent, flow);
                var convertToType = typeResolver.Evaluate(exp.ConvertToType);
                if (!ExplicitConversionTypesAreCompatible(exp.Referent, exp.Operator == ConversionOperator.Safe, convertToType))
                    diagnostics.Add(TypeError.CannotExplicitlyConvert(file, exp.Referent, referentType, convertToType));
                if (exp.Operator == ConversionOperator.Optional)
                    convertToType = convertToType.ToOptional();
                exp.Semantics = exp.Referent.ConvertedSemantics!;
                return exp.DataType = convertToType;
            }
        }
    }

    private DataType InferAssignmentTargetType(
        IAssignableExpressionSyntax expression,
        FlowState flow)
    {
        switch (expression)
        {
            default:
                throw ExhaustiveMatch.Failed(expression);
            case IQualifiedNameExpressionSyntax exp:
                var contextType = InferType(exp.Context, flow);
                DataType type;
                var member = exp.Member;
                switch (contextType)
                {
                    case ReferenceType { AllowsWrite: false, IsInitReference: false } contextReferenceType:
                        diagnostics.Add(TypeError.CannotAssignFieldOfReadOnly(file, expression.Span, contextReferenceType));
                        goto default;
                    case UnknownType:
                        member.ReferencedSymbol.Fulfill(null);
                        type = DataType.Unknown;
                        break;
                    default:
                        var contextSymbol = LookupSymbolForType(contextType)
                            ?? throw new NotImplementedException(
                                $"Missing context symbol for type {contextType.ToILString()}.");
                        var memberSymbols = symbolTrees.Children(contextSymbol).OfType<FieldSymbol>()
                                                       .Where(s => s.Name == member.Name).ToFixedList<Symbol>();
                        type = InferReferencedSymbol(member, memberSymbols) ?? DataType.Unknown;
                        break;
                }

                // Check for assigning into fields (self is handled by binding mutability analysis)
                if (exp.Context is not ISelfExpressionSyntax
                    && member.ReferencedSymbol.Result is BindingSymbol { IsMutableBinding: false, Name: Name name })
                    diagnostics.Add(SemanticError.CannotAssignImmutableField(file, exp.Span, name));

                type = type.AccessedVia(contextType);
                member.DataType = type;
                var semantics = member.Semantics ??= ExpressionSemantics.CreateReference;
                exp.Semantics = semantics;
                return exp.DataType = type;
            case ISimpleNameExpressionSyntax exp:
                exp.Semantics = ExpressionSemantics.CreateReference;
                var symbol = InferNameSymbol(exp);
                if (symbol is null) return exp.DataType = DataType.Unknown;
                if (symbol is VariableSymbol variableSymbol)
                    return exp.DataType = flow.Type(variableSymbol);

                throw new NotImplementedException("Raise error about assigning into a non-variable");
        }
    }

    private DataType InferInvocationType(
        IInvocationExpressionSyntax invocation,
        FlowState flow)
    {
        // This could actually be any of the following since the parser can't distinguish them:
        // * Regular function invocation
        // * Associated function invocation
        // * Namespaced function invocation
        // * Method invocation

        var argumentTypes = invocation.Arguments.Select(arg => InferType(arg, flow)).ToFixedList();
        var functionSymbols = FixedSet<FunctionSymbol>.Empty;
        switch (invocation.Expression)
        {
            case IQualifiedNameExpressionSyntax exp:
                var contextType = InferType(exp.Context, flow);
                var name = exp.Member.Name!;
                if (contextType is not VoidType)
                    return InferMethodInvocationType(invocation, exp.Context, name, argumentTypes, flow);
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
                functionSymbols = exp.LookupInContainingScope()
                                    .Select(p => p.As<FunctionSymbol>())
                                    .WhereNotNull()
                                    .Select(p => p.Result).ToFixedSet();
                break;
            default:
                throw new NotImplementedException("Invocation of expression");
        }

        return InferFunctionInvocationType(invocation, functionSymbols, argumentTypes, flow);
    }

    private DataType InferMethodInvocationType(
        IInvocationExpressionSyntax invocation,
        IExpressionSyntax context,
        Name methodName,
        FixedList<DataType> argumentTypes,
        FlowState flow)
    {
        // If it is unknown, we already reported an error
        if (context.DataType == DataType.Unknown)
        {
            invocation.Semantics = ExpressionSemantics.Never;
            return invocation.DataType = DataType.Unknown;
        }

        var contextSymbol = LookupSymbolForType(context.DataType.Known());
        var methodSymbols = symbolTrees.Children(contextSymbol!).OfType<MethodSymbol>()
                                       .Where(s => s.Name == methodName).ToFixedList();
        methodSymbols = SelectMethodOverload(context.DataType.Known(), methodSymbols, argumentTypes);

        switch (methodSymbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindMethod(file, invocation.Span));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                break;
            case 1:
                var methodSymbol = methodSymbols.Single();
                invocation.ReferencedSymbol.Fulfill(methodSymbol);

                // Since a method has been resolved, this must not be an empty type
                var contextType = (NonEmptyType)context.DataType.Known();

                var selfParamType = contextType.ReplaceTypeParametersIn(methodSymbol.SelfParameterType);
                AddImplicitConversionIfNeeded(context, selfParamType, flow);
                AddImplicitMoveIfNeeded(context, selfParamType, flow);
                AddImplicitFreezeIfNeeded(context, selfParamType, flow);
                CheckTypeCompatibility(selfParamType, context);

                foreach (var (arg, type) in invocation.Arguments
                    .Zip(methodSymbol.ParameterDataTypes.Select(contextType.ReplaceTypeParametersIn)))
                {
                    AddImplicitConversionIfNeeded(arg, type, flow);
                    CheckTypeCompatibility(type, arg);
                }

                var returnDataType = methodSymbol.ReturnDataType;
                invocation.DataType = contextType.ReplaceTypeParametersIn(returnDataType);
                AssignInvocationSemantics(invocation, returnDataType);
                // If there could be no write aliases, then the current result does not share with anything
                if (!returnDataType.AllowsWriteAliases)
                    flow.SplitCurrentResult();
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousMethodCall(file, invocation.Span));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                break;
        }

        // There are no types for functions
        invocation.Expression.DataType = DataType.Void;
        invocation.Expression.Semantics = ExpressionSemantics.Void;

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is IQualifiedNameExpressionSyntax nameExpression)
        {
            nameExpression.Member.DataType = DataType.Void;
            nameExpression.Member.Semantics = ExpressionSemantics.Void;
            nameExpression.Member.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);
        }

        return invocation.ConvertedDataType.Assigned();
    }

    private static void AddImplicitMoveIfNeeded(
        IExpressionSyntax context,
        DataType selfParamType,
        FlowState flow)
    {
        if (selfParamType is not ReferenceType { IsIsolatedReference: true } toType
            || context.DataType is not ReferenceType { AllowsRecoverIsolation: true } fromType)
            return;

        // TODO allow upcasting
        if (!toType.BareTypeEquals(fromType))
            return;

        if (context is not INameExpressionSyntax { ReferencedSymbol.Result: VariableSymbol { IsLocal: true } symbol }
            || !flow.IsIsolatedExceptCurrentResult(symbol))
            return;

        context.AddConversion(new ImplicitMove(context.ImplicitConversion));
        flow.Move(symbol);
    }

    private static void AddImplicitFreezeIfNeeded(
        IExpressionSyntax context,
        DataType selfParamType,
        FlowState flow)
    {
        if (selfParamType is not ReferenceType { IsConstReference: true } toType
            || context.ConvertedDataType is not ReferenceType { AllowsFreeze: true } fromType)
            return;

        // TODO allow upcasting
        if (!toType.BareTypeEquals(fromType)) return;

        if (context is not INameExpressionSyntax { ReferencedSymbol.Result: VariableSymbol { IsLocal: true } symbol }
            || !flow.IsIsolatedExceptCurrentResult(symbol))
            return;

        context.AddConversion(new ImplicitFreeze(context.ImplicitConversion));
        flow.Freeze(symbol);
    }

    private DataType InferFunctionInvocationType(
        IInvocationExpressionSyntax invocation,
        FixedSet<FunctionSymbol> functionSymbols,
        FixedList<DataType> argumentTypes,
        FlowState flow)
    {
        functionSymbols = SelectOverload(functionSymbols, argumentTypes);
        switch (functionSymbols.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(file, invocation.Span));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                invocation.Semantics = ExpressionSemantics.Never;
                break;
            case 1:
                var functionSymbol = functionSymbols.Single();
                invocation.ReferencedSymbol.Fulfill(functionSymbol);
                foreach (var (arg, parameterDataType) in invocation.Arguments.Zip(functionSymbol.ParameterDataTypes))
                {
                    AddImplicitConversionIfNeeded(arg, parameterDataType, flow);
                    CheckTypeCompatibility(parameterDataType, arg);
                }

                var returnType = functionSymbol.ReturnDataType;
                invocation.DataType = returnType;
                AssignInvocationSemantics(invocation, returnType);
                // If there could be no write aliases, then the current result does not share with anything
                if (!returnType.AllowsWriteAliases)
                    flow.SplitCurrentResult();
                break;
            default:
                diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, invocation.Span));
                invocation.ReferencedSymbol.Fulfill(null);
                invocation.DataType = DataType.Unknown;
                invocation.Semantics = ExpressionSemantics.Never;
                break;
        }

        // There are no types for functions
        invocation.Expression.DataType = DataType.Void;
        invocation.Expression.Semantics = ExpressionSemantics.Void;

        // Apply the referenced symbol to the underlying name
        if (invocation.Expression is INameExpressionSyntax nameExpression)
            nameExpression.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);

        return invocation.ConvertedDataType.Assigned();
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

    private DataType InferBlockType(
        IBlockOrResultSyntax blockOrResult,
        FlowState flow)
    {
        switch (blockOrResult)
        {
            default:
                throw ExhaustiveMatch.Failed(blockOrResult);
            case IBlockExpressionSyntax block:
                DataType? blockType = null;
                foreach (var statement in block.Statements)
                {
                    var context = blockType is null ? StatementContext.BeforeResult : StatementContext.AfterResult;
                    var resultType = ResolveTypes(statement, context, flow);
                    // Always resolve types even if there is already a block type
                    blockType ??= resultType;
                }

                // Drop any variables in the scope
                foreach (var variableDeclaration in block.Statements.OfType<IVariableDeclarationStatementSyntax>())
                    flow.Drop(variableDeclaration.Symbol.Result);

                // If there was no result expression, then the block type is void
                blockType ??= DataType.Void;
                // TODO what are the correct expression semantics for references?
                block.Semantics = blockType.Semantics.ToExpressionSemantics(ExpressionSemantics.ReadOnlyReference);
                return block.DataType = blockType;
            case IResultStatementSyntax result:
                InferType(result.Expression, flow);
                return result.Expression.ConvertedDataType.Assigned();
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
                                    .Select(p => p.As<VariableSymbol>())
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
                var symbols = symbolTreeBuilder.Children(containingSymbol).OfType<SelfParameterSymbol>().ToList();
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
                    ? SemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                    : SemanticError.SelfOutsideMethod(file, selfExpression.Span));
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
    private DataType CheckForeachInType(
        DataType? declaredType,
        IExpressionSyntax inExpression,
        FlowState flow)
    {
        switch (inExpression)
        {
            case IBinaryOperatorExpressionSyntax
            {
                Operator: BinaryOperator.DotDot
                    or BinaryOperator.LessThanDotDot
                    or BinaryOperator.DotDotLessThan
                    or BinaryOperator.LessThanDotDotLessThan
            } binaryExpression:
                var leftOperand = binaryExpression.LeftOperand;
                var leftType = InferType(leftOperand, flow);
                var leftResult = flow.CurrentResult;
                flow.NewResult();
                var rightOperand = binaryExpression.RightOperand;
                var rightType = InferType(rightOperand, flow);
                flow.UnionWithCurrentResultAndDrop(leftResult);

                if (!leftType.IsFullyKnown || !rightType.IsFullyKnown)
                    return inExpression.DataType = DataType.Unknown;

                DataType expType;
                if (leftType is IntegerConstantType left && rightType is IntegerConstantType right)
                    if (declaredType is not null)
                    {
                        AddImplicitConversionIfNeeded(leftOperand, declaredType, flow);
                        CheckTypeCompatibility(declaredType, leftOperand);
                        AddImplicitConversionIfNeeded(rightOperand, declaredType, flow);
                        CheckTypeCompatibility(declaredType, rightOperand);
                        expType = declaredType;
                    }
                    else
                        expType = left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt;
                else
                    expType = InferNumericOperatorType(leftOperand, rightOperand, flow);

                if (expType == DataType.Unknown)
                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                        binaryExpression.Span, binaryExpression.Operator, leftType, rightType));

                binaryExpression.Semantics = ExpressionSemantics.CopyValue; // Treat ranges as structs
                binaryExpression.DataType = expType;

                var assumedType = declaredType ?? expType.ToNonConstantType();

                AddImplicitConversionIfNeeded(binaryExpression, assumedType, flow);
                CheckTypeCompatibility(assumedType, binaryExpression);

                return binaryExpression.ConvertedDataType.Assigned();
            default:
                return InferType(inExpression, flow);
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
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand,
        FlowState flow)
    {
        var leftType = leftOperand.DataType.Assigned();
        var rightType = rightOperand.DataType.Assigned();
        var commonType = leftType.NumericOperatorCommonType(rightType);
        if (commonType is null) return DataType.Unknown;

        AddImplicitConversionIfNeeded(leftOperand, commonType, flow);
        AddImplicitConversionIfNeeded(rightOperand, commonType, flow);
        return commonType;
    }

    private static DataType InferComparisonOperatorType(
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand,
        FlowState flow)
    {
        var leftType = leftOperand.DataType.Assigned();
        var rightType = rightOperand.DataType.Assigned();
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

    private static bool ExplicitConversionTypesAreCompatible(IExpressionSyntax expression, bool safeOnly, DataType convertToType)
    {
        return (expression.ConvertedDataType, convertToType) switch
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
            _ => false
        };
    }

    private static FixedSet<TSymbol> SelectOverload<TSymbol>(
        FixedSet<TSymbol> symbols,
        FixedList<DataType> argumentTypes)
        where TSymbol : InvocableSymbol
    {
        // Filter down to symbols that could possible match
        symbols = symbols.Where(s =>
        {
            if (s.Arity != argumentTypes.Count) return false;
            // TODO check compatibility over argument types
            return true;
        }).ToFixedSet();
        // TODO Select most specific match
        return symbols;
    }

    private static FixedList<MethodSymbol> SelectMethodOverload(
        DataType selfType,
        FixedList<MethodSymbol> symbols,
        FixedList<DataType> argumentTypes)
    {
        // Filter down to symbols that could possible match
        symbols = symbols.Where(s =>
        {
            if (s.Arity != argumentTypes.Count) return false;
            // TODO check compatibility of self type
            _ = selfType;
            // TODO check compatibility over argument types

            return true;
        }).ToFixedList();

        // TODO Select most specific match
        return symbols;
    }

    private TypeSymbol? LookupSymbolForType(DataType dataType)
    {
        return dataType switch
        {
            UnknownType _ => null,
            ObjectType objectType => LookupSymbolForType(objectType),
            IntegerType integerType => symbolTrees.PrimitiveSymbolTree
                                                  .GlobalSymbols
                                                  .OfType<PrimitiveTypeSymbol>()
                                                  .Single(s => s.DeclaresType == integerType),
            _ => throw new NotImplementedException(
                $"{nameof(LookupSymbolForType)} not implemented for {dataType.GetType().Name}")
        };
    }

    private TypeSymbol LookupSymbolForType(ObjectType objectType)
    {
        var contextSymbols = symbolTrees.Packages.SafeCast<Symbol>();
        foreach (var name in objectType.ContainingNamespace.Segments)
        {
            contextSymbols = contextSymbols.SelectMany(c => symbolTrees.Children(c))
                                           .Where(s => s.Name == name);
        }

        return contextSymbols.SelectMany(symbolTrees.Children).OfType<TypeSymbol>()
                             .Single(s => s.Name == objectType.Name);
    }
}
