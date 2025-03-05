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
    #region Function Parts
    public static partial IMaybePlainType? ExpressionBody_ResultStatement_ExpectedPlainType(IExpressionBodyNode node)
    {
        var expectedPlainType = node.ExpectedPlainType; // Avoids repeated access
        // A void return is allowed to have an expression body resulting in any value since the
        // value will just be discarded.
        return expectedPlainType is VoidPlainType ? null : expectedPlainType;
    }
    #endregion

    #region Statements
    public static partial IMaybePlainType ResultStatement_PlainType(IResultStatementNode node)
        => node.Expression?.PlainType ?? PlainType.Unknown;
    #endregion

    #region Patterns
    public static partial void OptionalPattern_Contribute_Diagnostics(IOptionalPatternNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.ContextBindingType() is not OptionalType and var type)
            diagnostics.Add(TypeError.OptionalPatternOnNonOptionalType(node.File, node.Syntax, type));
    }
    #endregion

    #region Expressions
    public static partial IImplicitDerefExpressionNode? OrdinaryTypedExpression_Insert_ImplicitDerefExpression(IOrdinaryTypedExpressionNode node)
    {
        // To minimize outstanding rewrites, first check whether node.PlainType could possibly
        // support dereference. If node.ExpectedPlainType is checked, that is inherited and if a
        // rewrite is in progress, that can't be cached.
        if (node.PlainType is not RefPlainType plainType)
            return null;

        if (node.ExpectedPlainType is PlainType expectedPlainType
            && expectedPlainType.RefDepth() < plainType.RefDepth())
            return IImplicitDerefExpressionNode.Create(node);

        return null;
    }

    public static partial IImplicitConversionExpressionNode? OrdinaryTypedExpression_Insert_ImplicitConversionExpression(IOrdinaryTypedExpressionNode node)
    {
        // TODO can this be dropped? All those things should have an unknown plain type and so be skipped
        if (node.ShouldNotBeExpression()) return null;

        var plainType = node.PlainType; // Avoids repeated access

        // To minimize outstanding rewrites, first check whether node.PlainType could possibly
        // support conversion. If node.ExpectedPlainType is checked, that is inherited and if a
        // rewrite is in progress, that can't be cached. Note that optional type conversion (i.e.
        // `T` to `T?`) is a separate operation so doesn't need to be checked for here.
        if (!CanPossiblyImplicitlyConvertFrom(plainType)) return null;

        // TODO what about self argument context? Shouldn't implicit conversion be disallowed there?

        if (ImplicitlyConvertToType(node.ExpectedPlainType, plainType) is { } convertToTypeConstructor)
            return IImplicitConversionExpressionNode.Create(node, convertToTypeConstructor.PlainType);

        return null;
    }

    /// <summary>
    /// Whether it is possible that an implicit conversion could exist from this type to some other
    /// type.
    /// </summary>
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
            //    return OptionalPlainType.Create(ImplicitlyConvertToType(to, from));
            case (OptionalPlainType { Referent: var to }, _):
                return ImplicitlyConvertToType(to, fromType);
            default:
                return null;
        }
    }

    public static partial IOptionalConversionExpressionNode? OrdinaryTypedExpression_OptionalConversion_Rewrite_OptionalConversionExpression(IOrdinaryTypedExpressionNode node)
    {
        var plainType = node.PlainType; // Avoids repeated access

        // To minimize outstanding rewrites, first check whether node.PlainType could possibly
        // support optional conversion. If node.ExpectedPlainType is checked, that is inherited and
        // if a rewrite is in progress, that can't be cached. Unfortunately, for most types there is
        // an implicit optional conversion from `T` to `T?` so not many cases cane be eliminated.
        if (!CanPossiblyImplicitlyOptionalConvertFrom(plainType)) return null;

        if (ImplicitlyOptionalConvertToType(node.ExpectedPlainType, plainType) is { } depth and > 0)
            return IOptionalConversionExpressionNode.Create(node, depth);

        return null;
    }

    private static uint? ImplicitlyOptionalConvertToType(IMaybePlainType? toType, IMaybePlainType fromType)
    {
        if (toType is not OptionalPlainType toOptionalType)
            return null;

        if (toOptionalType.Referent.Equals(fromType))
            // If it is a reference type, then a subtype relation exists but additional layers above
            // that may need to be implicit conversions.
            return toOptionalType.Referent.Semantics == TypeSemantics.Reference ? 0u : 1;
        return ImplicitlyOptionalConvertToType(toOptionalType.Referent, fromType) + 1;
    }

    /// <remarks>Even though for a reference type <c>R</c>, <c>R &lt;: R?</c>, it is still possible
    /// for an implicit optional conversion to be needed if the expected type is <c>R??</c>. Thus,
    /// the only type that is guaranteed to not have an optional conversion is <see
    /// cref="UnknownPlainType"/>.</remarks>
    private static bool CanPossiblyImplicitlyOptionalConvertFrom(IMaybePlainType fromType)
        => fromType is not UnknownPlainType;

    public static partial IMaybePlainType BlockExpression_PlainType(IBlockExpressionNode node)
    {
        foreach (var statement in node.Statements)
            if (statement.ResultPlainType is not null and var resultPlainType)
                return resultPlainType;

        // If there was no result expression, then the block type is void
        return PlainType.Void;
    }

    public static partial IMaybePlainType UnsafeExpression_PlainType(IUnsafeExpressionNode node)
        => node.Expression?.PlainType ?? PlainType.Unknown;
    #endregion

    #region Instance Member Access Expressions
    public static partial IMaybePlainType FieldAccessExpression_PlainType(IFieldAccessExpressionNode node)
    {
        // TODO should probably use PlainType on the declaration
        var unboundPlainType = node.ReferencedDeclaration.BindingType.PlainType;
        var boundPlainType = node.Context.PlainType.TypeReplacements.ApplyTo(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType MethodAccessExpression_PlainType(IMethodAccessExpressionNode node)
        // TODO should MethodGroupPlainType be renamed to just MethodPlainType? The declaration is one method
        => node.ReferencedDeclaration?.MethodGroupPlainType ?? PlainType.Unknown;

    // TODO this is strange and maybe a hack
    // TODO this ought to be accounting for generic arguments
    public static partial IMaybePlainType? MethodAccessExpression_Context_ExpectedPlainType(IMethodAccessExpressionNode node)
        => (node.Parent as IMethodInvocationExpressionNode)?.SelectedCallCandidate?.SelfParameterPlainType;
    #endregion

    #region Literal Expressions
    public static partial IMaybePlainType BoolLiteralExpression_PlainType(IBoolLiteralExpressionNode node)
        => node.Value ? PlainType.True : PlainType.False;

    public static partial IMaybePlainType IntegerLiteralExpression_PlainType(IIntegerLiteralExpressionNode node)
        => new IntegerLiteralTypeConstructor(node.Value).PlainType;

    public static partial IMaybePlainType NoneLiteralExpression_PlainType(INoneLiteralExpressionNode node)
        => PlainType.None;

    public static partial IMaybePlainType StringLiteralExpression_PlainType(IStringLiteralExpressionNode node)
    {
        var typeDeclarationNode = node.ContainingLexicalScope
                                      .Lookup<ITypeDeclarationNode>(SpecialNames.StringTypeName)
                                      .TrySingle();
        return typeDeclarationNode?.TypeConstructor.TryConstructNullaryPlainType(containingType: null) ?? IMaybePlainType.Unknown;
    }
    #endregion

    #region Operator Expressions
    public static partial IMaybePlainType AssignmentExpression_PlainType(IAssignmentExpressionNode node)
        => node.LeftOperand?.PlainType ?? PlainType.Unknown;

    public static partial IRefAssignmentExpressionNode? AssignmentExpression_ReplaceWith_RefAssignmentExpression(IAssignmentExpressionNode node)
    {
        var leftDepth = node.LeftOperand?.PlainType.RefDepth();
        // The left hand side must be a ref/iref and the depth >= the right hand side. Note that
        // `list.at(i) = list.at(j)` have equal depth.
        // TODO what if the left is a ref var field, then equal depth is a normal assignment?
        if (leftDepth > 0 && leftDepth >= node.RightOperand?.PlainType.RefDepth())
            return IRefAssignmentExpressionNode.Create(node.Syntax, node.CurrentLeftOperand, node.CurrentRightOperand);

        return null;
    }

    public static partial IMaybePlainType RefAssignmentExpression_PlainType(IRefAssignmentExpressionNode node)
        => (node.LeftOperand?.PlainType as RefPlainType)?.Referent ?? IMaybePlainType.Unknown;

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
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor })
                // Don't apply any common type casting in this case.
                => null,

            (BarePlainType { TypeConstructor: BoolLiteralTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolLiteralTypeConstructor })
                // Don't apply any common type casting in this case.
                => null,

            (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                // Don't apply any common type casting in this case.
                => null,

            (BarePlainType { TypeConstructor: BoolTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolTypeConstructor })
                // Don't apply any common type casting in this case.
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
            // TODO this is a hack
            (BarePlainType { TypeConstructor: BoolLiteralTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                BarePlainType { TypeConstructor: BoolLiteralTypeConstructor })
                => PlainType.Bool,

            // TODO this is a hack
            (BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor left },
                BinaryOperator.Plus
                or BinaryOperator.Minus
                or BinaryOperator.Asterisk
                or BinaryOperator.Slash,
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor right })
                => left.IsSigned || right.IsSigned ? PlainType.Int : PlainType.UInt,

            // TODO this is a hack
            (BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.LessThan
                or BinaryOperator.LessThanOrEqual
                or BinaryOperator.GreaterThan
                or BinaryOperator.GreaterThanOrEqual,
                BarePlainType { TypeConstructor: IntegerLiteralTypeConstructor })
                => PlainType.Bool,

            // TODO whether the expression type is known is being used to report errors for whether
            // the operator can be used on the operands. That is wrong. For example, `@==` always
            // produces a `bool` and that should be the type even when it is applied to invalid operands.
            (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (NonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, NonVoidPlainType { Semantics: TypeSemantics.Reference })
                => PlainType.Bool,

            // TODO whether the expression type is known is being used to report errors for whether
            // the operator can be used on the operands. That is wrong.

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

    public static partial void UnaryOperatorExpression_Contribute_Diagnostics(IUnaryOperatorExpressionNode node, DiagnosticCollectionBuilder diagnostics)
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

    public static partial IMaybePlainType ConversionExpression_PlainType(IConversionExpressionNode node)
    {
        var convertToPlainType = node.ConvertToType.NamedPlainType;
        if (node.Operator == ConversionOperator.Optional)
            convertToPlainType = OptionalPlainType.Create(convertToPlainType);
        return convertToPlainType;
    }

    public static partial IMaybePlainType OptionalConversionExpression_PlainType(IOptionalConversionExpressionNode node)
    {
        var plainType = node.Referent.PlainType;
        for (int i = 0; i < node.Depth; i++)
            plainType = OptionalPlainType.Create(plainType);
        return plainType;
    }

    public static partial IMaybePlainType RefExpression_PlainType(IRefExpressionNode node)
        => RefPlainType.Create(node.IsInternal, node.IsMutableBinding, node.Referent?.PlainType) ?? PlainType.Unknown;

    // TODO diagnostics for using `iref` or `var` when it isn not valid

    public static partial IMaybePlainType ImplicitDerefExpression_PlainType(IImplicitDerefExpressionNode node)
        => (node.Referent.PlainType as RefPlainType)?.Referent ?? IMaybePlainType.Unknown;
    #endregion

    #region Control Flow Expressions
    public static partial IMaybePlainType IfExpression_PlainType(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return OptionalPlainType.Create(node.ThenBlock.PlainType);

        // TODO unify with else clause
        return node.ThenBlock.PlainType;
    }

    public static partial IMaybePlainType LoopExpression_PlainType(ILoopExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;

    public static partial IMaybePlainType WhileExpression_PlainType(IWhileExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;

    public static partial IMaybePlainType ForeachExpression_PlainType(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => PlainType.Void;
    #endregion

    #region Invocation Expressions
    public static partial IMaybePlainType FunctionInvocationExpression_PlainType(IFunctionInvocationExpressionNode node)
        => node.Function.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType MethodInvocationExpression_PlainType(IMethodInvocationExpressionNode node)
        => node.Method.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType GetterInvocationExpression_PlainType(IGetterInvocationExpressionNode node)
    {
        var unboundPlainType = node.ReferencedDeclaration?.ReturnPlainType ?? PlainType.Unknown;
        var boundPlainType = node.Context.PlainType.TypeReplacements.ApplyTo(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType SetterInvocationExpression_PlainType(ISetterInvocationExpressionNode node)
    {
        var unboundPlainType = node.ReferencedDeclaration?.ParameterPlainTypes[0] ?? PlainType.Unknown;
        var boundPlainType = node.Context.PlainType.TypeReplacements.ApplyTo(unboundPlainType);
        return boundPlainType;
    }

    public static partial IMaybePlainType FunctionReferenceInvocationExpression_PlainType(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionPlainType.Return;

    /// <remarks>Can't be an eager attribute because accessing <see cref="IFunctionReferenceInvocationExpressionNode.Expression"/>
    /// requires checking the parent of the <paramref name="node"/>.</remarks>
    public static partial FunctionPlainType FunctionReferenceInvocationExpression_FunctionPlainType(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionPlainType)node.Expression.PlainType;

    public static partial IMaybePlainType InitializerInvocationExpression_PlainType(IInitializerInvocationExpressionNode node)
        => node.SelectedCallCandidate?.ReturnPlainType ?? PlainType.Unknown;
    #endregion

    #region Name Expressions
    public static partial IMaybePlainType VariableNameExpression_PlainType(IVariableNameExpressionNode node)
        => node.ReferencedDefinition.BindingPlainType;

    public static partial IMaybePlainType SelfExpression_PlainType(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingPlainType ?? IMaybePlainType.Unknown;

    public static partial IMaybePlainType FunctionNameExpression_PlainType(IFunctionNameExpressionNode node)
        => node.ReferencedDeclaration?.PlainType ?? PlainType.Unknown;

    public static partial IMaybePlainType InitializerNameExpression_PlainType(IInitializerNameExpressionNode node)
        // TODO proper plain type
        // => node.ReferencedDeclaration?.InitializerGroupPlainType ?? PlainType.Unknown;
        => PlainType.Unknown;
    #endregion

    #region Async Expressions
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
    #endregion
}
