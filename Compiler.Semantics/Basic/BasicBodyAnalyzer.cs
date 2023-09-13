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
                                 .Prepend<BindingSymbol>(containingDeclaration.ImplicitSelfParameter.Symbol.Result),
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
        foreach (var parameterSymbol in parameterSymbols)
        {
            capabilities.Declare(parameterSymbol);
            sharing.Declare(parameterSymbol);
        }
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
                InferType(expressionStatement.Expression, flow);
                // At the end of the statement, any result reference is discarded
                flow.Split(SharingVariable.Result);
                break;
            case IResultStatementSyntax resultStatement:
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
        _ = InferType(variableDeclaration.Initializer, flow);
        DataType variableType;
        if (variableDeclaration.Type is not null)
            variableType = typeResolver.Evaluate(variableDeclaration.Type, implicitRead: true);
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
        flow.Union(symbol, SharingVariable.Result);
        // Split out result for end of statement
        flow.Split(SharingVariable.Result);
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
            case IMutateExpressionSyntax when inferCapability is null:
                // If we are explicitly moving or borrowing and no capability is specified, then
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

    public void CheckType(
        IExpressionSyntax? expression,
        DataType expectedType,
        FlowState flow)
    {
        if (expression is null) return;
        _ = InferType(expression, flow, implicitRead: true);
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
            case (ObjectType { IsConstReference: true } to, ObjectType { AllowsFreeze: true } from):
            {
                // Try to recover const
                // TODO support upcasting at the same time
                if (to.DeclaredType == from.DeclaredType && flow.IsIsolated(SharingVariable.Result))
                    return new RecoverConst(priorConversion);

                return null;
            }
            case (ObjectType { IsIsolatedReference: true } to, ObjectType { AllowsRecoverIsolation: true } from):
            {
                // Try to recover isolation
                // TODO support upcasting at the same time
                if (to.DeclaredType == from.DeclaredType && flow.IsIsolated(SharingVariable.Result))
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
    /// <param name="expression"></param>
    /// <param name="flow"></param>
    /// <param name="implicitRead">Whether this expression should implicitly be inferred to be a read.</param>
    [return: NotNullIfNotNull(nameof(expression))]
    private DataType? InferType(
        IExpressionSyntax? expression,
        FlowState flow,
        bool implicitRead = true)
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
                        // Don't need to alias the symbol in flow because it will be moved

                        exp.ReferencedSymbol.Fulfill(bindingSymbol);

                        const ExpressionSemantics semantics = ExpressionSemantics.IsolatedReference;
                        nameExpression.Semantics = semantics;
                        nameExpression.DataType = type;
                        exp.Semantics = semantics;
                        return exp.DataType = type;
                    case IMutateExpressionSyntax:
                        throw new NotImplementedException("Raise error about `move mut` expression");
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
                        // Now that it is frozen, alias it
                        flow.UnionResult(bindingSymbol);
                        flow.Alias(bindingSymbol);

                        exp.ReferencedSymbol.Fulfill(bindingSymbol);

                        const ExpressionSemantics semantics = ExpressionSemantics.ConstReference;
                        nameExpression.Semantics = semantics;
                        nameExpression.DataType = type;
                        exp.Semantics = semantics;
                        return exp.DataType = type;
                    case IMutateExpressionSyntax:
                        throw new NotImplementedException("Raise error about `freeze mut` expression.");
                    case IMoveExpressionSyntax:
                        throw new NotImplementedException("Raise error about `freeze move` expression.");
                    default:
                        throw new NotImplementedException("Tried to freeze expression type that isn't implemented.");
                }
            case IMutateExpressionSyntax exp:
                switch (exp.Referent)
                {
                    case ISimpleNameExpressionSyntax nameExpression:
                    {
                        var symbol = InferNameSymbol(nameExpression);
                        if (symbol is not BindingSymbol bindingSymbol)
                            throw new NotImplementedException("Raise error about `mut` from non-variable");

                        var type = flow.Type(bindingSymbol);
                        switch (type)
                        {
                            case ReferenceType referenceType:
                                if (!referenceType.AllowsWrite)
                                {
                                    diagnostics.Add(TypeError.ExpressionCantBeMutable(file, exp.Referent));
                                    type = DataType.Unknown;
                                }
                                else
                                    type = referenceType.AsMutable();

                                break;
                            default:
                                throw new NotImplementedException("Non-mutable type can't be borrowed mutably");
                        }

                        flow.UnionResult(bindingSymbol);
                        flow.Alias(bindingSymbol);
                        exp.ReferencedSymbol.Fulfill(bindingSymbol);

                        nameExpression.Semantics = ExpressionSemantics.MutableReference;
                        nameExpression.DataType = type;
                        return exp.DataType = type;
                    }
                    case IMutateExpressionSyntax:
                        throw new NotImplementedException("Raise error about `mut mut` expression");
                    case IMoveExpressionSyntax:
                        throw new NotImplementedException("Raise error about `mut move` expression");
                    default:
                        throw new NotImplementedException("Tried to mutate expression type that isn't implemented");
                }
            case IReturnExpressionSyntax exp:
            {
                if (returnType is null)
                    throw new NotImplementedException("Return statement in field initializer.");
                if (exp.Value is not null)
                {
                    var expectedReturnType = returnType;
                    InferType(exp.Value, flow, implicitRead: false);
                    flow.DropAllLocalVariable(); // No longer in scope
                    flow.DropIsolatedParameters(); // No longer external reference
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
                var leftType = InferType(binaryOperatorExpression.LeftOperand, flow);
                var @operator = binaryOperatorExpression.Operator;
                var rightType = InferType(binaryOperatorExpression.RightOperand, flow);

                // If either is unknown, then we can't know whether there is a a problem.
                // Note that the operator could be overloaded
                if (leftType == DataType.Unknown || rightType == DataType.Unknown)
                    return binaryOperatorExpression.DataType = DataType.Unknown;

                bool compatible;
                switch (@operator)
                {
                    case BinaryOperator.Plus:
                    case BinaryOperator.Minus:
                    case BinaryOperator.Asterisk:
                    case BinaryOperator.Slash:
                        compatible = NumericOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand, binaryOperatorExpression.RightOperand, flow);
                        binaryOperatorExpression.DataType = compatible ? leftType : DataType.Unknown;
                        binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                        break;
                    case BinaryOperator.EqualsEquals:
                    case BinaryOperator.NotEqual:
                        compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                                     || NumericOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand,
                                         binaryOperatorExpression.RightOperand, flow)
                                     || IdOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand,
                                         binaryOperatorExpression.RightOperand)
                            /*|| OperatorOverloadDefined(@operator, binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand)*/
                            ;
                        binaryOperatorExpression.DataType = DataType.Bool;
                        binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                        break;
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                        compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                                     || NumericOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand, binaryOperatorExpression.RightOperand, flow)
                            /*|| OperatorOverloadDefined(@operator, binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand)*/;
                        binaryOperatorExpression.DataType = DataType.Bool;
                        binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                        break;
                    case BinaryOperator.And:
                    case BinaryOperator.Or:
                        compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                        binaryOperatorExpression.DataType = DataType.Bool;
                        binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                        break;
                    case BinaryOperator.DotDot:
                    case BinaryOperator.LessThanDotDot:
                    case BinaryOperator.DotDotLessThan:
                    case BinaryOperator.LessThanDotDotLessThan:
                        throw new NotImplementedException("Type analysis of range operators");
                    case BinaryOperator.As:
                    case BinaryOperator.AsExclamation:
                    case BinaryOperator.AsQuestion:
                        throw new NotImplementedException("Type analysis of as operators");
                    default:
                        throw ExhaustiveMatch.Failed(@operator);
                }
                if (!compatible)
                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                        binaryOperatorExpression.Span, @operator, leftType, rightType));

                return binaryOperatorExpression.ConvertedDataType.Assigned();
            }
            case ISimpleNameExpressionSyntax exp:
            {
                var symbol = InferNameSymbol(exp);
                DataType type;
                ExpressionSemantics referenceSemantics;
                if (symbol is VariableSymbol variableSymbol)
                {
                    flow.UnionResult(variableSymbol);
                    flow.Alias(variableSymbol);

                    type = flow.Type(variableSymbol);
                    if (implicitRead) type = type.WithoutWrite();

                    referenceSemantics = implicitRead
                        ? ExpressionSemantics.ReadOnlyReference
                        : ExpressionSemantics.MutableReference;
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
                switch (@operator)
                {
                    default:
                        throw ExhaustiveMatch.Failed(@operator);
                    case UnaryOperator.Not:
                        CheckType(exp.Operand, DataType.Bool, flow);
                        exp.DataType = DataType.Bool;
                        break;
                    case UnaryOperator.Minus:
                    case UnaryOperator.Plus:
                        var operandType = InferType(exp.Operand, flow);
                        switch (operandType)
                        {
                            case IntegerConstantType integerType:
                                exp.DataType = integerType;
                                break;
                            case FixedSizeIntegerType sizedIntegerType:
                                // TODO upgrade the size
                                exp.DataType = sizedIntegerType;
                                break;
                            case BigIntegerType:
                                // Even if unsigned before, it is signed now
                                exp.DataType = DataType.Int;
                                break;
                            case UnknownType _:
                                exp.DataType = DataType.Unknown;
                                break;
                            default:
                                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                    exp.Span, @operator, operandType));
                                exp.DataType = DataType.Unknown;
                                break;
                        }
                        break;
                }

                return exp.ConvertedDataType.Assigned();
            }
            case INewObjectExpressionSyntax exp:
            {
                var argumentTypes = exp.Arguments.Select(arg => InferType(arg, flow)).ToFixedList();
                // TODO handle named constructors here
                var constructingType = typeResolver.EvaluateBareType(exp.Type);
                if (!constructingType.IsFullyKnown)
                {
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
                }

                // TODO handle null typesymbol
                var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new InvalidOperationException();
                DataType constructedType;
                var constructorSymbols = symbolTrees.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedSet();
                constructorSymbols = SelectOverload(constructorSymbols, argumentTypes);
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
                var declaredType = typeResolver.Evaluate(exp.Type, implicitRead: true);
                var expressionType = CheckForeachInType(declaredType, exp.InExpression, flow);
                var variableType = declaredType ?? expressionType.ToNonConstantType();
                var symbol = new VariableSymbol(containingSymbol, exp.VariableName,
                    exp.DeclarationNumber.Result, exp.IsMutableBinding, variableType, false);
                exp.Symbol.Fulfill(symbol);
                symbolTreeBuilder.Add(symbol);

                // TODO check the break types
                InferBlockType(exp.Block, flow);
                // TODO assign correct type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            }
            case IWhileExpressionSyntax exp:
            {
                CheckType(exp.Condition, DataType.Bool, flow);
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
                CheckType(exp.Condition, DataType.Bool, flow);
                InferBlockType(exp.ThenBlock, flow);
                switch (exp.ElseClause)
                {
                    default:
                        throw ExhaustiveMatch.Failed(exp.ElseClause);
                    case null:
                        break;
                    case IIfExpressionSyntax _:
                    case IBlockExpressionSyntax _:
                        var elseExpression = (IExpressionSyntax)exp.ElseClause;
                        InferType(elseExpression, flow);
                        break;
                    case IResultStatementSyntax resultStatement:
                        InferType(resultStatement.Expression, flow);
                        break;
                }
                // TODO assign a type to the expression
                exp.Semantics = ExpressionSemantics.Void;
                return exp.DataType = DataType.Void;
            case IQualifiedNameExpressionSyntax exp:
            {
                // Don't wrap the self expression in a share expression for field access
                var isSelfField = exp.Context is ISelfExpressionSyntax;
                // TODO properly handle mutable self
                var contextType = InferType(exp.Context, flow/*, !isSelfField*/);
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
                // TODO Deal with no context symbol
                var memberSymbols = symbolTrees.Children(contextSymbol)
                                               .Where(s => s.Name == member.Name).ToFixedList();
                var type = InferReferencedSymbol(member, memberSymbols) ?? DataType.Unknown;
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
                var type = flow.Type(InferSelfSymbol(exp));
                var referenceSemantics = implicitRead
                    ? ExpressionSemantics.ReadOnlyReference
                    : ExpressionSemantics.MutableReference;
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
                // TODO shouldn't this be implicit read still (e.g. x as! T should be read only?)
                var convertToType = typeResolver.Evaluate(exp.ConvertToType, false) ?? DataType.Unknown;
                if (!ExplicitConversionTypesAreCompatible(exp.Referent, convertToType))
                    diagnostics.Add(TypeError.CannotExplicitlyConvert(file, exp.Referent, referentType, convertToType));

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
                // TODO handle mutable self
                var contextType = InferType(exp.Context, flow, false);
                DataType type;
                var member = exp.Member;
                switch (contextType)
                {
                    case ReferenceType { AllowsWrite: false } contextReferenceType:
                        diagnostics.Add(TypeError.CannotAssignFieldOfReadOnly(file, expression.Span, contextReferenceType));
                        goto default;
                    case UnknownType:
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
                member.DataType = type;
                var semantics = member.Semantics ??= ExpressionSemantics.CreateReference;
                exp.Semantics = semantics;
                return exp.DataType = type;
            case ISimpleNameExpressionSyntax exp:
                exp.Semantics = ExpressionSemantics.CreateReference;
                var symbol = InferNameSymbol(exp);
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
                var contextType = InferType(exp.Context, flow, implicitRead: false);
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

                var selfParamType = contextType.ReplaceTypeParametersIn(methodSymbol.SelfDataType);
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

                invocation.DataType = contextType.ReplaceTypeParametersIn(methodSymbol.ReturnDataType);
                AssignInvocationSemantics(invocation, invocation.DataType);
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
            || !flow.IsIsolatedExceptResult(symbol))
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
            || context.DataType is not ReferenceType { AllowsFreeze: true } fromType)
            return;

        // TODO allow upcasting
        if (!toType.BareTypeEquals(fromType)) return;

        if (context is not INameExpressionSyntax { ReferencedSymbol.Result: VariableSymbol { IsLocal: true } symbol }
            || !flow.IsIsolatedExceptResult(symbol))
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

                invocation.DataType = functionSymbol.ReturnDataType;
                AssignInvocationSemantics(invocation, functionSymbol.ReturnDataType);
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
            case IBinaryOperatorExpressionSyntax binaryExpression
                when binaryExpression.Operator is BinaryOperator.DotDot
                    or BinaryOperator.LessThanDotDot
                    or BinaryOperator.DotDotLessThan
                    or BinaryOperator.LessThanDotDotLessThan:
                var leftType = InferType(binaryExpression.LeftOperand, flow);
                var rightType = InferType(binaryExpression.RightOperand, flow);
                if (!leftType.IsFullyKnown || !rightType.IsFullyKnown)
                    return inExpression.DataType = DataType.Unknown;

                var assumedType = declaredType;
                if (assumedType is null)
                {
                    if (leftType is IntegerConstantType)
                        assumedType = rightType.ToNonConstantType();
                    else if (rightType is IntegerConstantType)
                        assumedType = leftType.ToNonConstantType();
                }
                var type = assumedType;
                if (assumedType is not null)
                {
                    AddImplicitConversionIfNeeded(binaryExpression.LeftOperand, assumedType, flow);
                    AddImplicitConversionIfNeeded(binaryExpression.RightOperand, assumedType, flow);
                }
                else
                {
                    var compatible = NumericOperatorTypesAreCompatible(binaryExpression.LeftOperand,
                        binaryExpression.RightOperand, flow);
                    type = compatible ? leftType : DataType.Unknown;
                }

                inExpression.Semantics = ExpressionSemantics.CopyValue; // Treat ranges as structs
                return inExpression.DataType = type!;
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

    private static bool NumericOperatorTypesAreCompatible(
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand,
        FlowState flow)
    {
        var leftType = leftOperand.ConvertedDataType;
        switch (leftType)
        {
            default:
                // In theory we could just make the default false, but this
                // way we are forced to note exactly which types this doesn't work on.
                throw ExhaustiveMatch.Failed(leftType);
            case IntegerConstantType _:
                // TODO may need to promote based on size
                throw new NotImplementedException();
            //return !IsIntegerType(rightType);
            case PointerSizedIntegerType integerType:
                // TODO this isn't right we might need to convert either of them
                AddImplicitConversionIfNeeded(rightOperand, integerType, flow);
                return rightOperand.ConvertedDataType is PointerSizedIntegerType;
            case FixedSizeIntegerType integerType:
                // TODO this isn't right we might need to convert either of them
                AddImplicitConversionIfNeeded(rightOperand, integerType, flow);
                return rightOperand.ConvertedDataType is FixedSizeIntegerType;
            case BigIntegerType integerType:
                // TODO this isn't right we might need to convert either of them
                AddImplicitConversionIfNeeded(rightOperand, integerType, flow);
                return rightOperand.ConvertedDataType is IntegerType;
            case OptionalType _:
                throw new NotImplementedException("Trying to do math on optional type");
            case NeverType _:
            case UnknownType _:
                return true;
            case ReferenceType _:
            case GenericParameterType _:
            case BoolType _:
            case VoidType _: // This might need a special error message
                return false;
        }
    }

    /// <summary>
    /// Check if two expressions are `id` types and comparable with `==` and `!=`.
    /// </summary>
    private static bool IdOperatorTypesAreCompatible(
        IExpressionSyntax leftOperand,
        IExpressionSyntax rightOperand)
    {
        return leftOperand.ConvertedDataType is ReferenceType { IsIdentityReference: true } leftType
            && rightOperand.ConvertedDataType is ReferenceType { IsIdentityReference: true } rightType
            && (leftType.IsAssignableFrom(rightType) || rightType.IsAssignableFrom(leftType));
    }

    private static bool ExplicitConversionTypesAreCompatible(IExpressionSyntax expression, DataType convertToType)
    {
        return (expression.ConvertedDataType, convertToType) switch
        {
            // TODO add type for int and uint (currently just using int32)
            (BoolType, IntegerType) => true,
            (IntegerType { IsSigned: false }, BigIntegerType) => true,
            (IntegerType, BigIntegerType { IsSigned: true }) => true,
            _ => false
        };
    }

    //private bool OperatorOverloadDefined(BinaryOperator @operator, ExpressionSyntax leftOperand, ref ExpressionSyntax rightOperand)
    //{
    //    // all other operators are not yet implemented
    //    if (@operator != BinaryOperator.EqualsEquals)
    //        return false;

    //    if (!(leftOperand.Type is UserObjectType userObjectType))
    //        return false;
    //    var equalityOperators = userObjectType.Symbol.Lookup(SpecialName.OperatorEquals);
    //    if (equalityOperators.Count != 1)
    //        return false;
    //    var equalityOperator = equalityOperators.Single();
    //    if (!(equalityOperator.Type is FunctionType functionType) || functionType.Arity != 2)
    //        return false;
    //    InsertImplicitConversionIfNeeded(ref rightOperand, functionType.ParameterTypes[1]);
    //    return IsAssignableFrom(functionType.ParameterTypes[1], rightOperand.Type);

    //}

    //// Re-expose type analyzer to BasicAnalyzer
    //public DataType EvaluateType(ITypeSyntax typeSyntax, bool inferLent)
    //{
    //    return typeResolver.Evaluate(typeSyntax);
    //}

    //private void InferExpressionTypeInInvocation(ExpressionSyntax callee, FixedList<DataType> argumentTypes)
    //{
    //    switch (callee)
    //    {
    //        case NameSyntax identifierName:
    //        {
    //            var symbols = identifierName.LookupInContainingScope();
    //            symbols = ResolveOverload(symbols, null, argumentTypes);
    //            AssignReferencedSymbolAndType(identifierName, symbols);
    //        }
    //        break;
    //        case MemberAccessExpressionSyntax memberAccess:
    //        {
    //            var left = InferExpressionType(ref memberAccess.Expression);
    //            var containingSymbol = GetSymbolForType(left);
    //            var symbols = containingSymbol.Lookup(memberAccess.Member.Name);
    //            symbols = ResolveOverload(symbols, left, argumentTypes);
    //            memberAccess.Type = AssignReferencedSymbolAndType(memberAccess.Member, symbols);
    //        }
    //        break;
    //        default:
    //            throw new NotImplementedException();
    //    }
    //}

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

    private TypeSymbol? LookupSymbolForType(ObjectType objectType)
    {
        var contextSymbols = symbolTrees.Packages.SafeCast<Symbol>();
        foreach (var name in objectType.ContainingNamespace.Segments)
        {
            contextSymbols = contextSymbols.SelectMany(c => symbolTrees.Children(c))
                                           .Where(s => s.Name == name);
        }

        return contextSymbols.SelectMany(c => symbolTrees.Children(c)).OfType<TypeSymbol>()
                             .Single(s => s.Name == objectType.Name);
    }
}
