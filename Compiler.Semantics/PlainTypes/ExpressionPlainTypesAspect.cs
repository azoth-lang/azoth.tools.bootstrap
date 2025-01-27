using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class ExpressionPlainTypesAspect
{
    public static partial IMaybePlainType UnsafeExpression_PlainType(IUnsafeExpressionNode node)
        => node.Expression?.PlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType FunctionInvocationExpression_PlainType(IFunctionInvocationExpressionNode node)
        => node.Function.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType MethodInvocationExpression_PlainType(IMethodInvocationExpressionNode node)
    {
        var unboundPlainType = node.Method.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;
        var boundPlainType = node.Method.Context.PlainType.TypeReplacements.Apply(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType VariableNameExpression_PlainType(IVariableNameExpressionNode node)
        => node.ReferencedDefinition.BindingPlainType;

    public static partial IMaybePlainType SelfExpression_PlainType(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingPlainType ?? IMaybePlainType.Unknown;

    public static partial IMaybePlainType FieldAccessExpression_PlainType(IFieldAccessExpressionNode node)
    {
        // TODO should probably use PlainType on the declaration
        var unboundPlainType = node.ReferencedDeclaration.BindingType.PlainType;
        var boundPlainType = node.Context.PlainType.TypeReplacements.Apply(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType AssignmentExpression_PlainType(IAssignmentExpressionNode node)
        => node.LeftOperand?.PlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType ResultStatement_PlainType(IResultStatementNode node)
        => node.Expression?.PlainType ?? PlainType.Unknown;

    public static partial PlainType? BinaryOperatorExpression_NumericOperatorCommonPlainType(IBinaryOperatorExpressionNode node)
    {
        var leftPlainType = node.LeftOperand?.PlainType ?? PlainType.Unknown;
        var rightPlainType = node.RightOperand?.PlainType ?? PlainType.Unknown;
        return (leftPlainType, node.Operator, rightPlainType) switch
        {
            (BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor },
                BinaryOperator.Plus
                or BinaryOperator.Minus
                or BinaryOperator.Asterisk
                or BinaryOperator.Slash
                or BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.LessThan
                or BinaryOperator.LessThanOrEqual
                or BinaryOperator.GreaterThan
                or BinaryOperator.GreaterThanOrEqual,
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor }) => null,

            (BarePlainType { TypeConstructor: BoolLiteralTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolLiteralTypeConstructor }) => null,

            (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                => null,

            (BarePlainType { TypeConstructor: BoolTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolTypeConstructor })
                => null,

            (PlainType, BinaryOperator.Plus, PlainType)
                or (PlainType, BinaryOperator.Minus, PlainType)
                or (PlainType, BinaryOperator.Asterisk, PlainType)
                or (PlainType, BinaryOperator.Slash, PlainType)
                => ((PlainType)leftPlainType).NumericOperatorCommonType((PlainType)rightPlainType),
            (PlainType, BinaryOperator.EqualsEquals, PlainType)
                or (PlainType, BinaryOperator.NotEqual, PlainType)
                or (OptionalPlainType { Referent: PlainType }, BinaryOperator.NotEqual, OptionalPlainType { Referent: PlainType })
                or (PlainType, BinaryOperator.LessThan, PlainType)
                or (PlainType, BinaryOperator.LessThanOrEqual, PlainType)
                or (PlainType, BinaryOperator.GreaterThan, PlainType)
                or (PlainType, BinaryOperator.GreaterThanOrEqual, PlainType)
                => ((PlainType)leftPlainType).NumericOperatorCommonType((PlainType)rightPlainType),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                // TODO for the moment ranges are always integer ranges
                => PlainType.Int,

            _ => null,
        };
    }

    public static partial IMaybePlainType BinaryOperatorExpression_PlainType(IBinaryOperatorExpressionNode node)
    {
        var leftPlainType = node.LeftOperand?.PlainType ?? PlainType.Unknown;
        var rightPlainType = node.RightOperand?.PlainType ?? PlainType.Unknown;
        return (leftPlainType, node.Operator, rightPlainType) switch
        {
            (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                => PlainType.Bool,

            (BarePlainType { TypeConstructor: BoolTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolTypeConstructor })
                => PlainType.Bool,

            (PlainType, BinaryOperator.Plus, PlainType)
                or (PlainType, BinaryOperator.Minus, PlainType)
                or (PlainType, BinaryOperator.Asterisk, PlainType)
                or (PlainType, BinaryOperator.Slash, PlainType)
                => InferNumericOperatorType(node.NumericOperatorCommonPlainType),
            (PlainType, BinaryOperator.EqualsEquals, PlainType)
                or (PlainType, BinaryOperator.NotEqual, PlainType)
                or (OptionalPlainType { Referent: PlainType }, BinaryOperator.NotEqual, OptionalPlainType { Referent: PlainType })
                or (PlainType, BinaryOperator.LessThan, PlainType)
                or (PlainType, BinaryOperator.LessThanOrEqual, PlainType)
                or (PlainType, BinaryOperator.GreaterThan, PlainType)
                or (PlainType, BinaryOperator.GreaterThanOrEqual, PlainType)
                => InferComparisonOperatorType(node.NumericOperatorCommonPlainType),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                => InferRangeOperatorType(node.ContainingLexicalScope),

            (OptionalPlainType { Referent: var referentType }, BinaryOperator.QuestionQuestion, PlainType)
                when referentType.IsSubtypeOf(rightPlainType)
                => rightPlainType,
            (OptionalPlainType { Referent: var referentType }, BinaryOperator.QuestionQuestion, NeverPlainType)
                => referentType,

            _ => PlainType.Unknown

            // TODO optional types
        };
    }

    private static IMaybePlainType InferNumericOperatorType(PlainType? commonPlainType)
        => commonPlainType ?? IMaybePlainType.Unknown;

    private static IMaybePlainType InferComparisonOperatorType(PlainType? commonPlainType)
    {
        if (commonPlainType is null) return PlainType.Unknown;
        return PlainType.Bool;
    }

    private static IMaybePlainType InferRangeOperatorType(LexicalScope containingLexicalScope)
    {
        // TODO the left and right plain types need to be compatible with the range type
        var globalScope = containingLexicalScope.PackageNames.ImportGlobalScope;
        var typeDeclaration = globalScope.Lookup<INamespaceDeclarationNode>("azoth")
            .SelectMany(ns => ns.MembersNamed(SpecialNames.RangeTypeName)).OfType<ITypeDeclarationNode>().TrySingle();
        var typeConstructor = typeDeclaration?.TypeConstructor as BareTypeConstructor;
        var rangePlainType = typeConstructor?.TryConstructNullaryPlainType(containingType: null)
                             ?? IMaybePlainType.Unknown;
        return rangePlainType;
    }

    public static partial IMaybePlainType StringLiteralExpression_PlainType(IStringLiteralExpressionNode node)
    {
        var typeDeclarationNode = node.ContainingLexicalScope
                                      .Lookup<ITypeDeclarationNode>(SpecialNames.StringTypeName)
                                      .TrySingle();
        return typeDeclarationNode?.TypeConstructor.TryConstructNullaryPlainType(containingType: null) ?? IMaybePlainType.Unknown;
    }

    public static partial IMaybePlainType IfExpression_PlainType(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.PlainType.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.PlainType;
    }

    public static partial IMaybePlainType WhileExpression_PlainType(IWhileExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;

    public static partial IMaybePlainType LoopExpression_PlainType(ILoopExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;

    public static partial IMaybePlainType ForeachExpression_PlainType(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;

    public static partial IMaybePlainType BlockExpression_PlainType(IBlockExpressionNode node)
    {
        foreach (var statement in node.Statements)
            if (statement.ResultPlainType is not null and var resultPlainType)
                return resultPlainType;

        // If there was no result expression, then the block type is void
        return PlainType.Void;
    }

    public static partial IMaybePlainType ConversionExpression_PlainType(IConversionExpressionNode node)
    {
        var convertToPlainType = node.ConvertToType.NamedPlainType;
        if (node.Operator == ConversionOperator.Optional)
            convertToPlainType = convertToPlainType.MakeOptional();
        return convertToPlainType;
    }

    public static partial IMaybePlainType NoneLiteralExpression_PlainType(INoneLiteralExpressionNode node)
        => PlainType.None;

    public static partial IMaybePlainType AsyncStartExpression_PlainType(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.Expression?.PlainType ?? PlainType.Unknown);

    public static partial IMaybePlainType AwaitExpression_PlainType(IAwaitExpressionNode node)
    {
        if (node.Expression?.PlainType is BarePlainType { TypeConstructor: var typeConstructor } plainType
            && Intrinsic.PromiseTypeConstructor.Equals(typeConstructor))
            return plainType.Arguments[0];

        return PlainType.Unknown;
    }

    public static partial void AwaitExpression_Contribute_Diagnostics(IAwaitExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO eliminate code duplication with AwaitExpression_PlainType
        if (node.Expression?.PlainType is BarePlainType { TypeConstructor: var typeConstructor }
            && Intrinsic.PromiseTypeConstructor.Equals(typeConstructor))
            return;

        diagnostics.Add(TypeError.CannotAwaitType(node.File, node.Syntax.Span, node.Expression!.Type));
    }

    public static partial IMaybePlainType UnaryOperatorExpression_PlainType(IUnaryOperatorExpressionNode node)
        => node.Operator switch
        {
            UnaryOperator.Not => UnaryOperatorExpression_PlainType_Not(node),
            UnaryOperator.Minus => UnaryOperatorExpression_PlainType_Minus(node),
            UnaryOperator.Plus => UnaryOperatorExpression_PlainType_Plus(node),
            _ => throw ExhaustiveMatch.Failed(node.Operator),
        };

    private static IMaybePlainType UnaryOperatorExpression_PlainType_Not(IUnaryOperatorExpressionNode node)
        => PlainType.Bool;

    private static IMaybePlainType UnaryOperatorExpression_PlainType_Minus(IUnaryOperatorExpressionNode node)
    {
        if (node.Operand?.PlainType is not BarePlainType plainType) return PlainType.Unknown;
        return plainType.TypeConstructor switch
        {
            IntegerLiteralTypeConstructor t => t.Negate().PlainType,
            FixedSizeIntegerTypeConstructor t => t.WithSign().PlainType,
            PointerSizedIntegerTypeConstructor t => t.WithSign().PlainType,
            // Even if unsigned before, it is signed now
            BigIntegerTypeConstructor _ => PlainType.Int,
            _ => PlainType.Unknown,
        };
    }

    private static IMaybePlainType UnaryOperatorExpression_PlainType_Plus(IUnaryOperatorExpressionNode node)
        => node.Operand?.PlainType switch
        {
            BarePlainType { TypeConstructor: IntegerTypeConstructor t } => t.PlainType,
            BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor t } => t.PlainType,
            _ => PlainType.Unknown,
        };

    public static partial void UnaryOperatorExpression_Contribute_Diagnostics(
        IUnaryOperatorExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var operandPlainType = node.Operand!.PlainType;
        var cannotBeAppliedToOperandType
            = operandPlainType is not BarePlainType { TypeConstructor: var typeConstructor }
              || node.Operator switch
              {
                  UnaryOperator.Not => typeConstructor is not (BoolTypeConstructor or BoolLiteralTypeConstructor),
                  UnaryOperator.Minus => typeConstructor is not (IntegerTypeConstructor or IntegerLiteralTypeConstructor),
                  UnaryOperator.Plus => typeConstructor is not (IntegerTypeConstructor or IntegerLiteralTypeConstructor),
                  _ => throw ExhaustiveMatch.Failed(node.Operator),
              };

        if (cannotBeAppliedToOperandType)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(node.File,
                node.Syntax.Span, node.Operator, node.Operand!.Type));
    }

    public static partial IMaybePlainType GetterInvocationExpression_PlainType(IGetterInvocationExpressionNode node)
    {
        var unboundPlainType = node.ReferencedDeclaration?.ReturnPlainType ?? PlainType.Unknown;
        var boundPlainType = node.Context.PlainType.TypeReplacements.Apply(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType SetterInvocationExpression_PlainType(ISetterInvocationExpressionNode node)
    {
        var unboundPlainType = node.ReferencedDeclaration?.ParameterPlainTypes[0] ?? PlainType.Unknown;
        var boundPlainType = node.Context.PlainType.TypeReplacements.Apply(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType FunctionReferenceInvocationExpression_PlainType(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionPlainType.Return;

    /// <remarks>Can't be an eager attribute because accessing <see cref="IFunctionReferenceInvocationExpressionNode.Expression"/>
    /// requires checking the parent of the <paramref name="node"/>.</remarks>
    public static partial FunctionPlainType FunctionReferenceInvocationExpression_FunctionPlainType(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionPlainType)node.Expression.PlainType;

    public static partial IMaybePlainType InitializerInvocationExpression_PlainType(IInitializerInvocationExpressionNode node)
    {
        var unboundPlainType = node.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;
        var boundPlainType = node.Initializer.InitializingPlainType.TypeReplacements.Apply(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType FunctionNameExpression_PlainType(IFunctionNameExpressionNode node)
        => node.ReferencedDeclaration?.PlainType ?? PlainType.Unknown;

    #region Instance Member Access Expressions
    public static partial IMaybePlainType MethodAccessExpression_PlainType(IMethodAccessExpressionNode node)
        // TODO should MethodGroupPlainType be renamed to just MethodPlainType? The declaration is one method
        => node.ReferencedDeclaration?.MethodGroupPlainType ?? PlainType.Unknown;

    // TODO this is strange and maybe a hack
    public static partial IMaybePlainType? MethodAccessExpression_Context_ExpectedPlainType(IMethodAccessExpressionNode node)
        => (node.Parent as IMethodInvocationExpressionNode)
           ?.SelectedCallCandidate?.SelfParameterPlainType;
    #endregion

    public static partial IMaybePlainType InitializerNameExpression_PlainType(IInitializerNameExpressionNode node)
        // TODO proper plain type
        // => node.ReferencedDeclaration?.InitializerGroupPlainType ?? PlainType.Unknown;
        => PlainType.Unknown;

    public static partial IImplicitConversionExpressionNode? OrdinaryTypedExpression_Insert_ImplicitConversionExpression(IOrdinaryTypedExpressionNode node)
    {
        // TODO can this be dropped? All those things should have an unknown plain type and so be skipped
        if (node.ShouldNotBeExpression()) return null;

        // To minimize outstanding rewrites, first check whether node.PlainType could possibly
        // support conversion. If node.ExpectedPlainType is checked, that is inherited and if a
        // rewrite is in progress, that can't be cached. Note: this requires thoroughly treating
        // T <: T? as a subtype and not an implicit conversion.
        if (!CanPossiblyImplicitlyConvertFrom(node.PlainType))
            return null;

        if (ImplicitlyConvertToType(node.ExpectedPlainType, node.PlainType) is SimpleTypeConstructor convertToTypeConstructor)
            return IImplicitConversionExpressionNode.Create(node, convertToTypeConstructor.PlainType);

        return null;
    }

    private static bool CanPossiblyImplicitlyConvertFrom(IMaybePlainType fromType)
        => fromType switch
        {
            UnknownPlainType => false,
            BarePlainType { TypeConstructor: var typeConstructor }
                => CanPossiblyImplicitlyConvertFrom(typeConstructor),
            OptionalPlainType { Referent: var referent } => CanPossiblyImplicitlyConvertFrom(referent),
            _ => false,
        };

    private static bool CanPossiblyImplicitlyConvertFrom(BareTypeConstructor fromTypeConstructor)
        => fromTypeConstructor switch
        {
            BoolLiteralTypeConstructor => true,
            IntegerLiteralTypeConstructor => true,
            // Can't convert from signed because there is not a larger type to convert to
            BigIntegerTypeConstructor t => !t.IsSigned,
            PointerSizedIntegerTypeConstructor => true,
            FixedSizeIntegerTypeConstructor => true,
            _ => false,
        };

    private static SimpleTypeConstructor? ImplicitlyConvertToType(IMaybePlainType? toType, IMaybePlainType fromType)
    {
        switch (toType, fromType)
        {
            case (null, _):
            case (UnknownPlainType, _):
            case (_, UnknownPlainType):
            case (PlainType to, PlainType from) when from.Equals(to):
                return null;
            case (BarePlainType { TypeConstructor: FixedSizeIntegerTypeConstructor to },
                BarePlainType { TypeConstructor: FixedSizeIntegerTypeConstructor from }):
                if (to.Bits > from.Bits && (!from.IsSigned || to.IsSigned))
                    return to;
                return null;
            case (BarePlainType { TypeConstructor: FixedSizeIntegerTypeConstructor to },
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor from }):
            {
                // TODO make a method on plain types for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                return to.Bits >= bits && (!requireSigned || to.IsSigned) ? to : null;
            }
            case (BarePlainType { TypeConstructor: PointerSizedIntegerTypeConstructor to },
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor from }):
            {
                // TODO make a method on plain types for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                // Must fit in 32 bits so that it will fit on all platforms
                return bits <= 32 && (!requireSigned || to.IsSigned) ? to : null;
            }
            // Note: Both signed BigIntegerTypeConstructor has already been covered
            case (BarePlainType { TypeConstructor: BigIntegerTypeConstructor { IsSigned: true } },
                BarePlainType { TypeConstructor: IntegerTypeConstructor }):
            case (BarePlainType { TypeConstructor: BigIntegerTypeConstructor { IsSigned: true } },
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor }):
                return BareTypeConstructor.Int;
            case (BarePlainType { TypeConstructor: BigIntegerTypeConstructor to },
                BarePlainType
                {
                    TypeConstructor: IntegerTypeConstructor { IsSigned: false }
                                        or IntegerLiteralTypeConstructor { IsSigned: false }
                }):
                return to;
            case (BarePlainType { TypeConstructor: BoolTypeConstructor },
                BarePlainType { TypeConstructor: BoolLiteralTypeConstructor }):
                return BareTypeConstructor.Bool;
            // TODO support lifted implicit conversions
            //case (OptionalPlainType { Referent: var to }, OptionalPlainType { Referent: var from }):
            //    return ImplicitlyConvertToType(to, from)?.MakeOptional();
            case (OptionalPlainType { Referent: var to }, _):
                return ImplicitlyConvertToType(to, fromType);
            default:
                return null;
        }
    }

    public static partial void OptionalPattern_Contribute_Diagnostics(IOptionalPatternNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ContextBindingType() is not OptionalType and var type)
            diagnostics.Add(TypeError.OptionalPatternOnNonOptionalType(node.File, node.Syntax, type));
    }

    public static partial IMaybePlainType IntegerLiteralExpression_PlainType(IIntegerLiteralExpressionNode node)
        => new IntegerLiteralTypeConstructor(node.Value).PlainType;

    public static partial IMaybePlainType BoolLiteralExpression_PlainType(IBoolLiteralExpressionNode node)
        => node.Value ? PlainType.True : PlainType.False;
}
