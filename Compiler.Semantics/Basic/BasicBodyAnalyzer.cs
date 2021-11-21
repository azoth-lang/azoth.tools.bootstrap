using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
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
        private readonly Symbol containingSymbol;
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
            DataType? returnType = null)
            : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
                symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
        { }

        public BasicBodyAnalyzer(
            IAssociatedFunctionDeclarationSyntax containingDeclaration,
            SymbolTreeBuilder symbolTreeBuilder,
            SymbolForest symbolTrees,
            ObjectTypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
            : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
                symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
        { }

        public BasicBodyAnalyzer(
            IConstructorDeclarationSyntax containingDeclaration,
            SymbolTreeBuilder symbolTreeBuilder,
            SymbolForest symbolTrees,
            ObjectTypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
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
            DataType? returnType = null)
            : this(containingDeclaration, containingDeclaration.Parameters.Select(p => p.Symbol.Result),
                symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
        { }

        public BasicBodyAnalyzer(
            IFieldDeclarationSyntax containingDeclaration,
            SymbolTreeBuilder symbolTreeBuilder,
            SymbolForest symbolTrees,
            ObjectTypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
            : this(containingDeclaration, Enumerable.Empty<BindingSymbol>(),
                symbolTreeBuilder, symbolTrees, stringSymbol, diagnostics, returnType)
        { }

        private BasicBodyAnalyzer(
            IEntityDeclarationSyntax containingDeclaration,
            IEnumerable<BindingSymbol> parameterSymbols,
            SymbolTreeBuilder symbolTreeBuilder,
            SymbolForest symbolTrees,
            ObjectTypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
        {
            file = containingDeclaration.File;
            containingSymbol = containingDeclaration.Symbol.Result;
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
            this.parameterCapabilities = capabilities.Snapshot();
            this.parameterSharing = sharing.Snapshot();
        }

        public void ResolveTypes(IBodySyntax body)
        {
            var capabilities = parameterCapabilities.MutableCopy();
            var sharing = parameterSharing.MutableCopy();
            foreach (var statement in body.Statements)
                ResolveTypes(statement, sharing, capabilities);
        }

        private void ResolveTypes(
            IStatementSyntax statement,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    ResolveTypes(variableDeclaration, sharing, capabilities);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    InferType(expressionStatement.Expression, sharing, capabilities);
                    break;
                case IResultStatementSyntax resultStatement:
                    // TODO how does the type of this expression get applied to the containing block?
                    InferType(resultStatement.Expression, sharing, capabilities);
                    break;
            }
        }

        private void ResolveTypes(
            IVariableDeclarationStatementSyntax variableDeclaration,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            DataType type;
            if (variableDeclaration.Type != null)
            {
                type = typeResolver.Evaluate(variableDeclaration.Type);
                CheckType(variableDeclaration.Initializer, type, sharing, capabilities);
            }
            else if (variableDeclaration.Initializer != null)
                type = InferDeclarationType(variableDeclaration.Initializer, variableDeclaration.Capability, sharing, capabilities);
            else
            {
                diagnostics.Add(TypeError.NotImplemented(file, variableDeclaration.NameSpan,
                    "Inference of local variable types not implemented"));
                type = DataType.Unknown;
            }

            if (variableDeclaration.Initializer != null)
            {
                var initializerType = variableDeclaration.Initializer.ConvertedDataType
                                      ?? throw new InvalidOperationException("Initializer type should be determined");

                if (!type.IsAssignableFrom(initializerType))
                    diagnostics.Add(TypeError.CannotConvert(file, variableDeclaration.Initializer, initializerType, type));
            }

            var symbol = new VariableSymbol((InvocableSymbol)containingSymbol, variableDeclaration.Name,
                variableDeclaration.DeclarationNumber.Result, variableDeclaration.IsMutableBinding, type);
            variableDeclaration.Symbol.Fulfill(symbol);
            symbolTreeBuilder.Add(symbol);
            capabilities.Declare(symbol);
            sharing.Declare(symbol);
            if (type is ReferenceType referenceType
                && (referenceType.Capability == ReferenceCapability.Mutable
                    || referenceType.Capability == ReferenceCapability.ReadOnly))
            {
                sharing.Union(symbol, SharingVariable.Result);
            }
        }

        /// <summary>
        /// Infer the type of a variable declaration from an expression
        /// </summary>
        private DataType InferDeclarationType(
            IExpressionSyntax expression,
            IReferenceCapabilitySyntax? inferCapability,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            var type = InferType(expression, sharing, capabilities);
            if (!type.IsKnown) return DataType.Unknown;
            type = type.ToNonConstantType();
            type = AddImplicitConversionIfNeeded(expression, type);

            switch (expression)
            {
                case IMoveExpressionSyntax:
                    // If we are explicitly moving then take the actual type
                    return type;
                case IMutateExpressionSyntax:
                {
                    // If we are explicitly borrowing or moving then take the actual type
                    if (type is not ReferenceType)
                        throw new NotImplementedException("Compile error: can't borrow non reference type");

                    throw new NotImplementedException("Mutate expression declaration type");
                    //return referenceType.To(ReferenceCapability.Borrowed);
                }
                default:
                {
                    // We assume immutability on variables unless explicitly stated
                    if (inferCapability is null) return type.ToReadable();
                    if (type is not ReferenceType referenceType)
                        throw new NotImplementedException("Compile error: can't infer mutability for non reference type");
                    if (!referenceType.IsMutable)
                        throw new NotImplementedException("Compile error: can't infer a mutable type");

                    return type;
                }
            }
        }

        public void CheckType(
            IExpressionSyntax? expression,
            DataType expectedType,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            if (expression is null) return;
            InferType(expression, sharing, capabilities);
            var actualType = AddImplicitConversionIfNeeded(expression, expectedType);
            if (!expectedType.IsAssignableFrom(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, actualType, expectedType));
        }

        /// <summary>
        /// Create an implicit conversion if allowed and needed
        /// </summary>
        private static DataType AddImplicitConversionIfNeeded(
            IExpressionSyntax expression,
            DataType expectedType)
        {
            var fromType = expression.DataType.Assigned();
            var conversion = ImplicitConversion(expectedType, fromType);
            if (conversion != null) expression.ImplicitConversion = conversion;
            return expression.ConvertedDataType.Assigned();
        }

        private static Conversion? ImplicitConversion(DataType to, DataType from)
        {
            switch (to, from)
            {
                case (OptionalType optionalTo, OptionalType optionalFrom):
                    // Direct subtype
                    if (optionalTo.Referent.IsAssignableFrom(optionalFrom.Referent))
                        return null;
                    var liftedConversion = ImplicitConversion(optionalTo.Referent, optionalFrom.Referent);
                    return liftedConversion is null ? null : new LiftedConversion(liftedConversion);
                case (OptionalType targetType, /* non-optional type */ _):
                    // If needed, convert the type to the referent type of the optional type
                    throw new NotImplementedException("Conversion to optional");
                //var type = ApplyImplicitConversionIfNeeded(expression, targetType.Referent);
                //if (targetType.Referent.IsAssignableFrom(type))
                //{
                //    var conversion = ImplicitConversion(targetType.Referent, @from);
                //    expression = new ImplicitOptionalConversionExpression(expression, targetType);
                //}
                //else
                //    return null;
                case (FixedSizeIntegerType targetType, FixedSizeIntegerType expressionType):
                    if (targetType.Bits > expressionType.Bits && (!expressionType.IsSigned || targetType.IsSigned))
                        return new NumericConversion(targetType);
                    else
                        return null;
                case (FixedSizeIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    var bits = expressionType.Value.GetByteCount(!targetType.IsSigned) * 8;
                    if (targetType.Bits >= bits && (!requireSigned || targetType.IsSigned))
                        return new NumericConversion(targetType);
                    else
                        return null;
                }
                case (PointerSizedIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    return !requireSigned || targetType.IsSigned ? new NumericConversion(targetType) : null;
                }
                case (ObjectType { IsReadOnly: true } targetType, ObjectType { IsMutable: true } expressionType):
                    // TODO if source type is explicitly mutable, issue warning about using `mut` in immutable context
                    var capability = targetType.Capability == ReferenceCapability.Constant
                        ? expressionType.To(ReferenceCapability.Constant)
                        : expressionType.ToReadable();
                    return new ImmutabilityConversion(capability);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Infer the type of an expression and assign that type to the expression.
        /// </summary>
        /// <param name="expression">A reference to the repression. This is a reference so that the
        ///     expression can be replaced because the parser can't always correctly determine
        ///     what kind of expression it is without type information.</param>
        /// <param name="sharing"></param>
        /// <param name="capabilities"></param>
        /// <param name="implicitShare">Whether implicit share expressions should be inserted around
        ///     bare variable references.</param>
        private DataType InferType(
            IExpressionSyntax? expression,
            SharingRelation sharing,
            ReferenceCapabilities capabilities,
            bool implicitShare = true)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case null:
                    return DataType.Unknown;
                case IMoveExpressionSyntax exp:
                    switch (exp.Referent)
                    {
                        case INameExpressionSyntax nameExpression:
                            nameExpression.Semantics = ExpressionSemantics.IsolatedReference;
                            var type = InferType(exp.Referent, sharing, capabilities, false);
                            switch (type)
                            {
                                case ReferenceType referenceType:
                                    if (!referenceType.IsMovable)
                                    {
                                        diagnostics.Add(TypeError.CannotMoveValue(file, exp));
                                        type = DataType.Unknown;
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException("Non-moveable type can't be moved");
                            }

                            exp.ReferencedSymbol.Fulfill((NamedBindingSymbol?)nameExpression.ReferencedSymbol.Result);
                            exp.Semantics = ExpressionSemantics.IsolatedReference;
                            return exp.DataType = type;
                        case IMutateExpressionSyntax:
                            throw new NotImplementedException("Raise error about `move mut` expression");
                        case IMoveExpressionSyntax:
                            throw new NotImplementedException("Raise error about `move move` expression");
                        default:
                            throw new NotImplementedException("Tried to move out of expression type that isn't implemented");
                    }
                case IMutateExpressionSyntax exp:
                    switch (exp.Referent)
                    {
                        case INameExpressionSyntax nameExpression:
                        {
                            nameExpression.Semantics = ExpressionSemantics.MutableReference;
                            var type = InferType(exp.Referent, sharing, capabilities, false);
                            switch (type)
                            {
                                case ReferenceType referenceType:
                                    if (!referenceType.IsMutable)
                                    {
                                        diagnostics.Add(TypeError.ExpressionCantBeMutable(file, exp.Referent));
                                        type = DataType.Unknown;
                                    }
                                    else
                                        type = referenceType.ToMutable();
                                    break;
                                default:
                                    throw new NotImplementedException("Non-mutable type can't be borrowed mutably");
                            }

                            exp.ReferencedSymbol.Fulfill((NamedBindingSymbol?)nameExpression.ReferencedSymbol.Result);
                            return exp.DataType = type;
                        }
                        case IMutateExpressionSyntax:
                            throw new NotImplementedException("Raise error about `mut mut` expression");
                        case IMoveExpressionSyntax:
                            throw new NotImplementedException("Raise error about `mut move` expression");
                        default:
                            throw new NotImplementedException("Tried mutate expression type that isn't implemented");
                    }
                case IReturnExpressionSyntax exp:
                {
                    if (exp.Value != null)
                    {
                        var expectedReturnType = returnType ?? throw new InvalidOperationException("Return statement in constructor");
                        InferType(exp.Value, sharing, capabilities, false);
                        // If we return ownership, there can be an implicit move
                        // otherwise there could be an implicit share or borrow
                        InsertImplicitActionIfNeeded(exp.Value, expectedReturnType, implicitMutateAllowed: false);
                        var actualType = AddImplicitConversionIfNeeded(exp.Value, expectedReturnType);
                        if (!expectedReturnType.IsAssignableFrom(actualType))
                            diagnostics.Add(TypeError.CannotConvert(file, exp.Value, actualType, expectedReturnType));
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(TypeError.CantReturnFromNeverFunction(file, exp.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file, exp.Span, returnType ?? DataType.Unknown));

                    return exp.DataType = DataType.Never;
                }
                case IIntegerLiteralExpressionSyntax exp:
                    return exp.DataType = new IntegerConstantType(exp.Value);
                case IStringLiteralExpressionSyntax exp:
                    return exp.DataType = stringSymbol?.DeclaresDataType ?? (DataType)DataType.Unknown;
                case IBoolLiteralExpressionSyntax exp:
                    return exp.DataType = exp.Value ? DataType.True : DataType.False;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                {
                    var leftType = InferType(binaryOperatorExpression.LeftOperand, sharing, capabilities);
                    var @operator = binaryOperatorExpression.Operator;
                    var rightType = InferType(binaryOperatorExpression.RightOperand, sharing, capabilities);

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
                            compatible = NumericOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand, binaryOperatorExpression.RightOperand);
                            binaryOperatorExpression.DataType = compatible ? leftType : DataType.Unknown;
                            binaryOperatorExpression.Semantics = ExpressionSemantics.CopyValue;
                            break;
                        case BinaryOperator.EqualsEquals:
                        case BinaryOperator.NotEqual:
                        case BinaryOperator.LessThan:
                        case BinaryOperator.LessThanOrEqual:
                        case BinaryOperator.GreaterThan:
                        case BinaryOperator.GreaterThanOrEqual:
                            compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                                         || NumericOperatorTypesAreCompatible(binaryOperatorExpression.LeftOperand, binaryOperatorExpression.RightOperand)
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
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                    }
                    if (!compatible)
                        diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                            binaryOperatorExpression.Span, @operator, leftType, rightType));

                    return binaryOperatorExpression.ConvertedDataType.Assigned();
                }
                case INameExpressionSyntax exp:
                {
                    var type = InferVariableNameType(exp);
                    // In many contexts, variable names are implicitly shared
                    if (implicitShare)
                        type = InsertImplicitReadIfNeeded(expression, type);

                    // TODO do a more complete generation of expression semantics
                    if (exp.Semantics is null)
                        switch (type.Semantics)
                        {
                            case TypeSemantics.Copy:
                                exp.Semantics = ExpressionSemantics.CopyValue;
                                break;
                            case TypeSemantics.Move:
                                exp.Semantics = ExpressionSemantics.MoveValue;
                                break;
                        }

                    return type;
                }
                case IUnaryOperatorExpressionSyntax exp:
                {
                    var @operator = exp.Operator;
                    switch (@operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                        case UnaryOperator.Not:
                            CheckType(exp.Operand, DataType.Bool, sharing, capabilities);
                            exp.DataType = DataType.Bool;
                            break;
                        case UnaryOperator.Minus:
                        case UnaryOperator.Plus:
                            var operandType = InferType(exp.Operand, sharing, capabilities);
                            switch (operandType)
                            {
                                case IntegerConstantType integerType:
                                    exp.DataType = integerType;
                                    break;
                                case FixedSizeIntegerType sizedIntegerType:
                                    exp.DataType = sizedIntegerType;
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
                    var argumentTypes = exp.Arguments.Select(arg => InferType(arg, sharing, capabilities)).ToFixedList();
                    // TODO handle named constructors here
                    var constructingType = typeResolver.Evaluate(exp.Type);
                    if (!constructingType.IsKnown)
                    {
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        return exp.DataType  = DataType.Unknown;
                    }

                    // TODO handle null typesymbol
                    var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new InvalidOperationException();
                    var classType = (ObjectType)constructingType;
                    ObjectType constructedType;
                    var constructorSymbols = symbolTreeBuilder.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedSet();
                    constructorSymbols = ResolveOverload(constructorSymbols, argumentTypes);
                    switch (constructorSymbols.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                            exp.ReferencedSymbol.Fulfill(null);
                            constructedType = classType.ToConstructorReturn();
                            break;
                        case 1:
                            var constructorSymbol = constructorSymbols.Single();
                            exp.ReferencedSymbol.Fulfill(constructorSymbol);
                            foreach (var (arg, parameterDataType) in exp.Arguments.Zip(constructorSymbol.ParameterDataTypes))
                            {
                                AddImplicitConversionIfNeeded(arg, parameterDataType);
                                CheckArgumentTypeCompatibility(parameterDataType, arg);
                            }
                            constructedType = constructorSymbol.ReturnDataType;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, exp.Span));
                            exp.ReferencedSymbol.Fulfill(null);
                            constructedType = classType.ToConstructorReturn();
                            break;
                    }

                    return exp.DataType = constructedType;
                }
                case IForeachExpressionSyntax exp:
                {
                    var declaredType = typeResolver.Evaluate(exp.Type);
                    var expressionType = CheckForeachInType(declaredType, exp.InExpression, sharing, capabilities);
                    var variableType = declaredType ?? expressionType;
                    var symbol = new VariableSymbol((InvocableSymbol)containingSymbol, exp.VariableName,
                        exp.DeclarationNumber.Result, exp.IsMutableBinding, variableType);
                    exp.Symbol.Fulfill(symbol);
                    symbolTreeBuilder.Add(symbol);

                    // TODO check the break types
                    InferBlockType(exp.Block, sharing, capabilities);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case IWhileExpressionSyntax exp:
                {
                    CheckType(exp.Condition, DataType.Bool, sharing, capabilities);
                    InferBlockType(exp.Block, sharing, capabilities);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case ILoopExpressionSyntax exp:
                    InferBlockType(exp.Block, sharing, capabilities);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                case IInvocationExpressionSyntax exp:
                    return InferInvocationType(exp, sharing, capabilities);
                case IUnsafeExpressionSyntax exp:
                {
                    exp.DataType = InferType(exp.Expression, sharing, capabilities);
                    exp.Semantics = exp.Expression.Semantics.Assigned();
                    return exp.ConvertedDataType.Assigned();
                }
                case IIfExpressionSyntax exp:
                    CheckType(exp.Condition, DataType.Bool, sharing, capabilities);
                    InferBlockType(exp.ThenBlock, sharing, capabilities);
                    switch (exp.ElseClause)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(exp.ElseClause);
                        case null:
                            break;
                        case IIfExpressionSyntax _:
                        case IBlockExpressionSyntax _:
                            var elseExpression = (IExpressionSyntax)exp.ElseClause;
                            InferType(elseExpression, sharing, capabilities);
                            //ifExpression.ElseClause = elseExpression;
                            break;
                        case IResultStatementSyntax resultStatement:
                            InferType(resultStatement.Expression, sharing, capabilities);
                            break;
                    }
                    // TODO assign a type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                case IQualifiedNameExpressionSyntax exp:
                {
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.Context is ISelfExpressionSyntax;
                    var contextType = InferType(exp.Context, sharing, capabilities, !isSelfField);
                    var member = exp.Field;
                    var contextSymbol = LookupSymbolForType(contextType);
                    if (contextSymbol is null)
                    {
                        member.ReferencedSymbol.Fulfill(null);
                        member.DataType = DataType.Unknown;
                        exp.Semantics ??= ExpressionSemantics.CopyValue;
                        return exp.DataType = DataType.Unknown;
                    }
                    // TODO Deal with no context symbol
                    var memberSymbols = symbolTreeBuilder.Children(contextSymbol!).OfType<FieldSymbol>()
                                                  .Where(s => s.Name == member.Name).ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    // In many contexts, variable names are implicitly shared
                    if (implicitShare) type = InsertImplicitReadIfNeeded(expression, type);

                    if (exp.Semantics is null)
                        switch (type.Semantics)
                        {
                            case TypeSemantics.Copy:
                                exp.Semantics = ExpressionSemantics.CopyValue;
                                break;
                            case TypeSemantics.Move:
                                exp.Semantics = ExpressionSemantics.MoveValue;
                                break;
                        }
                    return exp.DataType = type;
                }
                case IBreakExpressionSyntax exp:
                    InferType(exp.Value, sharing, capabilities);
                    return exp.DataType = DataType.Never;
                case INextExpressionSyntax exp:
                    return exp.DataType = DataType.Never;
                case IAssignmentExpressionSyntax exp:
                {
                    var left = InferAssignmentTargetType(exp.LeftOperand, sharing, capabilities);
                    InferType(exp.RightOperand, sharing, capabilities);
                    AddImplicitConversionIfNeeded(exp.RightOperand, left);
                    var right = exp.RightOperand.ConvertedDataType.Assigned();
                    if (!left.IsAssignableFrom(right))
                        diagnostics.Add(TypeError.CannotConvert(file,
                            exp.RightOperand, right, left));
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case ISelfExpressionSyntax exp:
                {
                    var type = InferSelfType(exp);
                    if (implicitShare)
                        type = InsertImplicitReadIfNeeded(expression, type);

                    if (exp.Semantics is null)
                    {
                        if (type is ReferenceType referenceType)
                            exp.Semantics = referenceType.IsMutable
                                ? ExpressionSemantics.MutableReference : ExpressionSemantics.ReadOnlyReference;
                        else
                            throw new NotImplementedException("Could not assign semantics to `self` expression");
                    }
                    return type;
                }
                case INoneLiteralExpressionSyntax exp:
                    return exp.DataType = DataType.None;
                case IBlockExpressionSyntax blockSyntax:
                    return InferBlockType(blockSyntax, sharing, capabilities);
            }
        }

        // TODO implicit read is no longer a conversion to be inserted, it is just the inferred type (remove method and replace with something else)
        private static DataType InsertImplicitReadIfNeeded(IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (type is not ReferenceType referenceType) return type;

            BindingSymbol? referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.ReadOnlyReference;
                    referencedSymbol = (NamedBindingSymbol?)exp.ReferencedSymbol.Result;
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                case IQualifiedNameExpressionSyntax exp:
                    exp.Field.Semantics = ExpressionSemantics.ReadOnlyReference;
                    exp.Semantics = ExpressionSemantics.ReadOnlyReference;
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                default:
                    // implicit share isn't needed around other expressions
                    return type;
            }

            //type = referenceType.To(ReferenceCapability.Shared);

            //if (referenceType.Capability != ReferenceCapability.Shared)
            //    throw new NotImplementedException("Need implicit read?");
            //expression = new ImplicitReadExpressionSyntax(expression, type, referencedSymbol);

            //return type;
            return referenceType;
        }

        private static void InsertImplicitMutateIfNeeded(IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (type is not ReferenceType referenceType) return;

            BindingSymbol? referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.MutableReference;
                    referencedSymbol = (NamedBindingSymbol?)exp.ReferencedSymbol.Result;
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                default:
                    // implicit mutate isn't needed around other expressions
                    return;
            }

            type = referenceType.ToMutable();

            if (!referenceType.IsMutable)
                throw new NotImplementedException("Need implicit mutate?");
            //expression = new ImplicitMutateExpressionSyntax(expression, type, referencedSymbol);
        }

        private static void InsertImplicitMoveIfNeeded(IExpressionSyntax expression, DataType type)
        {
            // Value types aren't moved
            if (type is not ReferenceType { IsMovable: true })
                return;

            if (expression is not INameExpressionSyntax name)
                // Implicit move not needed
                return;

            var referencedSymbol = name.ReferencedSymbol.Result;
            //expression = new ImplicitMoveSyntax(expression, type, referencedSymbol);
            name.Semantics = ExpressionSemantics.IsolatedReference;
            expression.Semantics = ExpressionSemantics.IsolatedReference;

            throw new NotImplementedException("Need implicit move?");
        }

        private static void InsertImplicitActionIfNeeded(IExpressionSyntax expression, DataType toType, bool implicitMutateAllowed)
        {
            var fromType = expression.DataType.Assigned();
            if (fromType is not ReferenceType from || toType is not ReferenceType to) return;

            if (from.IsMovable && to.IsMovable)
                InsertImplicitMoveIfNeeded(expression, to);
            else if (from.IsReadOnly || to.IsReadOnly)
                InsertImplicitReadIfNeeded(expression, to.ToReadable());
            else if (implicitMutateAllowed)
                InsertImplicitMutateIfNeeded(expression, to);
        }

        private DataType InferAssignmentTargetType(
            IAssignableExpressionSyntax expression,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IQualifiedNameExpressionSyntax exp:
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.Context is ISelfExpressionSyntax;
                    var contextType = InferType(exp.Context, sharing, capabilities, !isSelfField);
                    var member = exp.Field;
                    var contextSymbol = LookupSymbolForType(contextType);
                    // TODO Deal with no context symbol
                    var memberSymbols = symbolTreeBuilder.Children(contextSymbol!).OfType<FieldSymbol>().Where(s => s.Name == member.Name).ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    exp.Field.Semantics ??= ExpressionSemantics.CreateReference;
                    exp.Semantics = exp.Field.Semantics.Assigned();
                    return exp.DataType = type;
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.CreateReference;
                    return InferVariableNameType(exp);
            }
        }

        private DataType InferInvocationType(
            IInvocationExpressionSyntax invocation,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            // This could actually be any of the following since the parser can't distinguish them:
            // * Regular function invocation
            // * Associated function invocation
            // * Namespaced function invocation
            // * Method invocation

            var argumentTypes = invocation.Arguments.Select(arg => InferType(arg, sharing, capabilities)).ToFixedList();
            FixedSet<FunctionSymbol> functionSymbols = FixedSet<FunctionSymbol>.Empty;
            switch (invocation.Expression)
            {
                case IQualifiedNameExpressionSyntax exp:
                    var name = exp.Field.Name!;
                    var contextName = MethodContextAsName(exp.Context);
                    if (contextName != null)
                    {
                        var contextSymbols = invocation.ContainingLexicalScope.Lookup(contextName.Segments[0])
                                                       .Select(p => p.Result);
                        foreach (var namePart in contextName.Segments.Skip(1))
                            contextSymbols =
                                contextSymbols.SelectMany(c => symbolTrees.Children(c).Where(s => s.Name == namePart));

                        functionSymbols = contextSymbols.SelectMany(c =>
                            symbolTrees.Children(c).OfType<FunctionSymbol>()
                                       .Where(s => s.Name == name)).ToFixedSet();
                    }
                    // TODO it isn't always safe to assume that just because we didn't find any functions this is a method
                    if (!functionSymbols.Any())
                    {
                        InferType(exp.Context, sharing, capabilities, false);
                        return InferMethodInvocationType(invocation, exp.Context, name, argumentTypes);
                    }
                    break;
                case INameExpressionSyntax exp:
                    functionSymbols = invocation.ContainingLexicalScope.Lookup(exp.Name!)
                                                .Select(p => p.As<FunctionSymbol>())
                                                .WhereNotNull()
                                                .Select(p => p.Result).ToFixedSet();
                    break;
                default:
                    throw new NotImplementedException("Invocation of expression");
            }


            return InferFunctionInvocationType(invocation, functionSymbols, argumentTypes);
        }

        private DataType InferMethodInvocationType(
            IInvocationExpressionSyntax invocation,
            IExpressionSyntax context,
            Name methodName,
            FixedList<DataType> argumentTypes)
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
            methodSymbols = ResolveMethodOverload(context.DataType.Known(), methodSymbols, argumentTypes);

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

                    var selfParamType = methodSymbol.SelfDataType;
                    InsertImplicitActionIfNeeded(context, selfParamType, implicitMutateAllowed: true);

                    AddImplicitConversionIfNeeded(context, selfParamType);
                    CheckArgumentTypeCompatibility(selfParamType, context);

                    foreach (var (arg, type) in invocation.Arguments
                                                          .Zip(methodSymbol.ParameterDataTypes))
                    {
                        AddImplicitConversionIfNeeded(arg, type);
                        CheckArgumentTypeCompatibility(type, arg);
                    }

                    invocation.DataType = methodSymbol.ReturnDataType;
                    AssignInvocationSemantics(invocation, methodSymbol.ReturnDataType);
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
                nameExpression.Field.DataType = DataType.Void;
                nameExpression.Field.Semantics = ExpressionSemantics.Void;
                nameExpression.Field.ReferencedSymbol.Fulfill(invocation.ReferencedSymbol.Result);
            }

            return invocation.ConvertedDataType.Assigned();
        }

        /// <summary>
        /// Used on the target of an invocation to see if it is could be a name
        /// </summary>
        /// <returns>A name if the expression is a qualified name, otherwise null</returns>
        private static NamespaceName? MethodContextAsName(IExpressionSyntax expression)
        {
            return expression switch
            {
                IQualifiedNameExpressionSyntax memberAccess =>
                // if implicit self
                memberAccess.Context is null // TODO is this correct or is there an expression for implicit self?
                    ? null
                    : MethodContextAsName(memberAccess.Context)?.Qualify(memberAccess.Field.Name!),
                INameExpressionSyntax nameExpression => nameExpression.Name!,
                _ => null
            };
        }

        private DataType InferFunctionInvocationType(
            IInvocationExpressionSyntax invocation,
            FixedSet<FunctionSymbol> functionSymbols,
            FixedList<DataType> argumentTypes)
        {
            functionSymbols = ResolveOverload(functionSymbols, argumentTypes);
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
                        AddImplicitConversionIfNeeded(arg, parameterDataType);
                        CheckArgumentTypeCompatibility(parameterDataType, arg);
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
            DataType type)
        {
            switch (type.Semantics)
            {
                default:
                    throw ExhaustiveMatch.Failed(type.Semantics);
                case TypeSemantics.Void:
                    invocationExpression.Semantics = ExpressionSemantics.Void;
                    break;
                case TypeSemantics.Move:
                    invocationExpression.Semantics = ExpressionSemantics.MoveValue;
                    break;
                case TypeSemantics.Copy:
                    invocationExpression.Semantics = ExpressionSemantics.CopyValue;
                    break;
                case TypeSemantics.Never:
                    invocationExpression.Semantics = ExpressionSemantics.Never;
                    break;
                case TypeSemantics.Reference:
                    var referenceType = (ReferenceType)type;
                    if (referenceType.Capability.CanBeAcquired())
                        invocationExpression.Semantics = ExpressionSemantics.IsolatedReference;
                    else if (referenceType.IsMutable)
                        invocationExpression.Semantics = ExpressionSemantics.MutableReference;
                    else
                        invocationExpression.Semantics = ExpressionSemantics.ReadOnlyReference;
                    break;
            }
        }

        private DataType InferBlockType(
            IBlockOrResultSyntax blockOrResult,
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax block:
                    foreach (var statement in block.Statements)
                        ResolveTypes(statement, sharing, capabilities);

                    block.Semantics = ExpressionSemantics.Void;
                    return block.DataType = DataType.Void; // TODO assign the correct type to the block
                case IResultStatementSyntax result:
                    InferType(result.Expression, sharing, capabilities);
                    return result.Expression.ConvertedDataType.Assigned();
            }
        }

        public DataType InferVariableNameType(INameExpressionSyntax nameExpression)
        {
            if (nameExpression.Name is null)
            {
                // Name unknown, no error
                nameExpression.ReferencedSymbol.Fulfill(null);
                return nameExpression.DataType = DataType.Unknown;
            }

            DataType? type;
            var symbols = nameExpression.LookupInContainingScope()
                                        .Select(p => p.As<NamedBindingSymbol>())
                                        .WhereNotNull()
                                        .ToFixedList();
            switch (symbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol.Fulfill(null);
                    type = DataType.Unknown;
                    break;
                case 1:
                    var symbol = symbols.Single().Result;
                    nameExpression.ReferencedSymbol.Fulfill(symbol);
                    type = symbol.DataType;
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol.Fulfill(null);
                    type = DataType.Unknown;
                    break;
            }

            return nameExpression.DataType = type!;
        }

        private DataType InferSelfType(ISelfExpressionSyntax selfExpression)
        {
            DataType type;
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
                            type = DataType.Unknown;
                            selfExpression.ReferencedSymbol.Fulfill(null);
                            break;
                        case 1:
                            var symbol = symbols.Single();
                            type = symbol.DataType;
                            selfExpression.ReferencedSymbol.Fulfill(symbol);
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, selfExpression.Span));
                            type = DataType.Unknown;
                            selfExpression.ReferencedSymbol.Fulfill(null);
                            break;
                    }
                    break;
                case FunctionSymbol _:
                    diagnostics.Add(selfExpression.IsImplicit
                        ? SemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                        : SemanticError.SelfOutsideMethod(file, selfExpression.Span));
                    type = DataType.Unknown;
                    selfExpression.ReferencedSymbol.Fulfill(null);
                    break;
                case NamespaceOrPackageSymbol _:
                case BindingSymbol _:
                case TypeSymbol _:
                    throw new InvalidOperationException("Invalid containing symbol for body");
            }

            return selfExpression.DataType = type;
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
            SharingRelation sharing,
            ReferenceCapabilities capabilities)
        {
            switch (inExpression)
            {
                case IBinaryOperatorExpressionSyntax binaryExpression
                    when binaryExpression.Operator == BinaryOperator.DotDot
                    || binaryExpression.Operator == BinaryOperator.LessThanDotDot
                    || binaryExpression.Operator == BinaryOperator.DotDotLessThan
                    || binaryExpression.Operator == BinaryOperator.LessThanDotDotLessThan:
                    var leftType = InferType(binaryExpression.LeftOperand, sharing, capabilities);
                    InferType(binaryExpression.RightOperand, sharing, capabilities);
                    if (declaredType != null)
                    {
                        leftType = AddImplicitConversionIfNeeded(binaryExpression.LeftOperand, declaredType);
                        AddImplicitConversionIfNeeded(binaryExpression.RightOperand, declaredType);
                    }

                    inExpression.Semantics = ExpressionSemantics.CopyValue; // Treat ranges as structs
                    return inExpression.DataType = leftType;
                default:
                    return InferType(inExpression, sharing, capabilities);
            }
        }

        private void CheckArgumentTypeCompatibility(DataType type, IExpressionSyntax arg)
        {
            var fromType = arg.ConvertedDataType.Assigned();
            if (!type.IsAssignableFrom(fromType))
                diagnostics.Add(TypeError.CannotConvert(file, arg, fromType, type));
        }

        private DataType AssignReferencedSymbolAndType(
            INameExpressionSyntax exp,
            FixedList<FieldSymbol> matchingSymbols)
        {
            switch (matchingSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMember(file, exp.Span));
                    exp.Semantics = ExpressionSemantics.Never;
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
                case 1:
                    var memberSymbol = matchingSymbols.Single();
                    switch (memberSymbol.DataType.Semantics)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(memberSymbol.DataType.Semantics);
                        case TypeSemantics.Copy:
                            exp.Semantics = ExpressionSemantics.CopyValue;
                            break;
                        case TypeSemantics.Never:
                            exp.Semantics = ExpressionSemantics.Never;
                            break;
                        case TypeSemantics.Reference:
                            // Needs to be assigned based on share/borrow expression
                            break;
                        case TypeSemantics.Move:
                            throw new InvalidOperationException("Can't move out of field");
                        case TypeSemantics.Void:
                            throw new InvalidOperationException("Can't assign semantics to void field");
                    }

                    exp.ReferencedSymbol.Fulfill(memberSymbol);
                    return exp.DataType = memberSymbol.DataType;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, exp.Span));
                    exp.Semantics = ExpressionSemantics.Never;
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
            }
        }

        private static bool NumericOperatorTypesAreCompatible(
            IExpressionSyntax leftOperand,
            IExpressionSyntax rightOperand)
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
                    AddImplicitConversionIfNeeded(rightOperand, integerType);
                    return rightOperand.ConvertedDataType is PointerSizedIntegerType;
                case FixedSizeIntegerType integerType:
                    // TODO this isn't right we might need to convert either of them
                    AddImplicitConversionIfNeeded(rightOperand, integerType);
                    return rightOperand.ConvertedDataType is FixedSizeIntegerType;
                case OptionalType _:
                    throw new NotImplementedException("Trying to do math on optional type");
                case NeverType _:
                case UnknownType _:
                    return true;
                case ReferenceType _:
                case BoolType _:
                case VoidType _: // This might need a special error message
                    return false;
            }
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

        // Re-expose type analyzer to BasicAnalyzer
        public DataType EvaluateType(ITypeSyntax typeSyntax, bool inferLent)
        {
            return typeResolver.Evaluate(typeSyntax);
        }

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

        private static FixedSet<TSymbol> ResolveOverload<TSymbol>(
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

        private static FixedList<MethodSymbol> ResolveMethodOverload(
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
                                                      .OfType<TypeSymbol>()
                                                      .Single(s => s.DeclaresDataType == integerType),
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
}
