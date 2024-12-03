using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class ExpressionPlainTypesAspect
{
    public static partial IMaybePlainType UnsafeExpression_PlainType(IUnsafeExpressionNode node)
        => node.Expression?.PlainType ?? IPlainType.Unknown;

    public static partial IMaybePlainType FunctionInvocationExpression_PlainType(IFunctionInvocationExpressionNode node)
        => node.Function.SelectedCallCandidate?.ReturnPlainType ?? IPlainType.Unknown;

    public static partial IMaybePlainType MethodInvocationExpression_PlainType(IMethodInvocationExpressionNode node)
    {
        var unboundAntetype = node.Method.SelectedCallCandidate?.ReturnPlainType ?? IPlainType.Unknown;
        var boundAntetype = node.Method.Context.PlainType.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybePlainType VariableNameExpression_PlainType(IVariableNameExpressionNode node)
        => node.ReferencedDefinition.BindingPlainType;

    public static partial IMaybePlainType SelfExpression_PlainType(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingPlainType ?? IPlainType.Unknown;

    public static partial IMaybePlainType FieldAccessExpression_PlainType(IFieldAccessExpressionNode node)
    {
        // TODO should probably use PlainType on the declaration
        var unboundAntetype = node.ReferencedDeclaration.BindingType.ToPlainType();
        var boundAntetype = node.Context.PlainType.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybePlainType NewObjectExpression_PlainType(INewObjectExpressionNode node)
    {
        // TODO should probably use PlainType on the declaration
        var unboundType = node.ReferencedConstructor?.ReturnType.ToPlainType() ?? IPlainType.Unknown;
        var boundType = node.ConstructingPlainType.ReplaceTypeParametersIn(unboundType);
        return boundType;
    }

    public static partial IMaybePlainType AssignmentExpression_PlainType(IAssignmentExpressionNode node)
        => node.LeftOperand?.PlainType ?? IPlainType.Unknown;

    public static partial IMaybePlainType ResultStatement_PlainType(IResultStatementNode node)
        => node.Expression?.PlainType.ToNonLiteral() ?? IPlainType.Unknown;

    public static partial IPlainType? BinaryOperatorExpression_NumericOperatorCommonPlainType(IBinaryOperatorExpressionNode node)
    {
        var leftPlainType = node.LeftOperand?.PlainType ?? IPlainType.Unknown;
        var rightPlainType = node.RightOperand?.PlainType ?? IPlainType.Unknown;
        return (leftAntetype: leftPlainType, node.Operator, rightAntetype: rightPlainType) switch
        {
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor },
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
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor }) => null,

            (OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor }) => null,

            (INonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, INonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (INonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, INonVoidPlainType { Semantics: TypeSemantics.Reference })
                => null,

            (OrdinaryNamedPlainType { TypeConstructor: BoolTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                OrdinaryNamedPlainType { TypeConstructor: BoolTypeConstructor })
                => null,

            (IPlainType, BinaryOperator.Plus, IPlainType)
                or (IPlainType, BinaryOperator.Minus, IPlainType)
                or (IPlainType, BinaryOperator.Asterisk, IPlainType)
                or (IPlainType, BinaryOperator.Slash, IPlainType)
                => ((IPlainType)leftPlainType).NumericOperatorCommonType((IPlainType)rightPlainType),
            (IPlainType, BinaryOperator.EqualsEquals, IPlainType)
                or (IPlainType, BinaryOperator.NotEqual, IPlainType)
                or (OptionalPlainType { Referent: IPlainType }, BinaryOperator.NotEqual, OptionalPlainType { Referent: IPlainType })
                or (IPlainType, BinaryOperator.LessThan, IPlainType)
                or (IPlainType, BinaryOperator.LessThanOrEqual, IPlainType)
                or (IPlainType, BinaryOperator.GreaterThan, IPlainType)
                or (IPlainType, BinaryOperator.GreaterThanOrEqual, IPlainType)
                => ((IPlainType)leftPlainType).NumericOperatorCommonType((IPlainType)rightPlainType),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                // TODO for the moment ranges are always integer ranges
                => IPlainType.Int,

            _ => null,
        };
    }

    public static partial IMaybePlainType BinaryOperatorExpression_PlainType(IBinaryOperatorExpressionNode node)
    {
        var leftAntetype = node.LeftOperand?.PlainType ?? IPlainType.Unknown;
        var rightAntetype = node.RightOperand?.PlainType ?? IPlainType.Unknown;
        return (leftAntetype, node.Operator, rightAntetype) switch
        {
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.Plus,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.Add(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.Minus,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.Subtract(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.Asterisk,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.Multiply(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.Slash,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.DivideBy(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.EqualsEquals,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.Equals(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.NotEqual,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.NotEquals(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.LessThan,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.LessThan(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.LessThanOrEqual,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.LessThanOrEqual(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.GreaterThan,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.GreaterThan(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor left }, BinaryOperator.GreaterThanOrEqual,
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor right }) => left.GreaterThanOrEqual(right).PlainType,

            (OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor left }, BinaryOperator.EqualsEquals,
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor right }) => left.Equals(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor left }, BinaryOperator.NotEqual,
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor right }) => left.NotEquals(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor left }, BinaryOperator.And,
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor right }) => left.And(right).PlainType,
            (OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor left }, BinaryOperator.Or,
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor right }) => left.Or(right).PlainType,

            (INonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.ReferenceEquals, INonVoidPlainType { Semantics: TypeSemantics.Reference })
                or (INonVoidPlainType { Semantics: TypeSemantics.Reference }, BinaryOperator.NotReferenceEqual, INonVoidPlainType { Semantics: TypeSemantics.Reference })
                => IPlainType.Bool,

            (OrdinaryNamedPlainType { TypeConstructor: BoolTypeConstructor },
                BinaryOperator.EqualsEquals
                or BinaryOperator.NotEqual
                or BinaryOperator.And
                or BinaryOperator.Or,
                OrdinaryNamedPlainType { TypeConstructor: BoolTypeConstructor })
                => IPlainType.Bool,

            (IPlainType, BinaryOperator.Plus, IPlainType)
                or (IPlainType, BinaryOperator.Minus, IPlainType)
                or (IPlainType, BinaryOperator.Asterisk, IPlainType)
                or (IPlainType, BinaryOperator.Slash, IPlainType)
                => InferNumericOperatorType(node.NumericOperatorCommonPlainType),
            (IPlainType, BinaryOperator.EqualsEquals, IPlainType)
                or (IPlainType, BinaryOperator.NotEqual, IPlainType)
                or (OptionalPlainType { Referent: IPlainType }, BinaryOperator.NotEqual, OptionalPlainType { Referent: IPlainType })
                or (IPlainType, BinaryOperator.LessThan, IPlainType)
                or (IPlainType, BinaryOperator.LessThanOrEqual, IPlainType)
                or (IPlainType, BinaryOperator.GreaterThan, IPlainType)
                or (IPlainType, BinaryOperator.GreaterThanOrEqual, IPlainType)
                => InferComparisonOperatorType(node.NumericOperatorCommonPlainType),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                => InferRangeOperatorType(node.ContainingLexicalScope),

            (OptionalPlainType { Referent: var referentType }, BinaryOperator.QuestionQuestion, NeverPlainType)
                => referentType,

            _ => IPlainType.Unknown

            // TODO optional types
        };
    }

    private static IMaybePlainType InferNumericOperatorType(IPlainType? commonAntetype)
        => commonAntetype ?? IMaybePlainType.Unknown;

    private static IMaybePlainType InferComparisonOperatorType(IPlainType? commonAntetype)
    {
        if (commonAntetype is null) return IPlainType.Unknown;
        return IPlainType.Bool;
    }

    private static IMaybePlainType InferRangeOperatorType(LexicalScope containingLexicalScope)
    {
        // TODO the left and right antetypes need to be compatible with the range type
        var rangeTypeDeclaration = containingLexicalScope.Lookup("azoth")
            .OfType<INamespaceDeclarationNode>().SelectMany(ns => ns.MembersNamed("range"))
            .OfType<ITypeDeclarationNode>().TrySingle();
        var rangeAntetype = rangeTypeDeclaration?.Symbol.TryGetDeclaredType()?.TryToPlainType()
                            ?? IMaybePlainType.Unknown;
        return rangeAntetype;
    }

    public static partial IMaybePlainType StringLiteralExpression_PlainType(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName)
                                 .OfType<ITypeDeclarationNode>().TrySingle();
        return (IMaybePlainType?)typeSymbolNode?.Symbol.TryGetDeclaredType()?.TryToPlainType() ?? IPlainType.Unknown;
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static partial IMaybePlainType IfExpression_PlainType(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.PlainType.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.PlainType;
    }

    public static partial IMaybePlainType WhileExpression_PlainType(IWhileExpressionNode node)
        // TODO assign correct type to the expression
        => IPlainType.Void;

    public static partial IMaybePlainType LoopExpression_PlainType(ILoopExpressionNode node)
        // TODO assign correct type to the expression
        => IPlainType.Void;

    public static partial IMaybePlainType ForeachExpression_PlainType(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => IPlainType.Void;

    public static partial IMaybePlainType BlockExpression_PlainType(IBlockExpressionNode node)
    {
        foreach (var statement in node.Statements)
            if (statement.ResultPlainType is not null and var resultAntetype)
                return resultAntetype;

        // If there was no result expression, then the block type is void
        return IPlainType.Void;
    }

    public static partial IMaybePlainType ConversionExpression_PlainType(IConversionExpressionNode node)
    {
        var convertToAntetype = node.ConvertToType.NamedPlainType;
        if (node.Operator == ConversionOperator.Optional)
            convertToAntetype = convertToAntetype.MakeOptional();
        return convertToAntetype;
    }

    public static partial IMaybePlainType NoneLiteralExpression_PlainType(INoneLiteralExpressionNode node)
        => IPlainType.None;

    public static partial IMaybePlainType AsyncStartExpression_PlainType(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.Expression?.PlainType.ToNonLiteral() ?? IPlainType.Unknown);

    public static partial IMaybePlainType AwaitExpression_PlainType(IAwaitExpressionNode node)
    {
        if (node.Expression?.PlainType is OrdinaryNamedPlainType { TypeConstructor: var typeConstructor } antetype
            && Intrinsic.PromiseTypeConstructor.Equals(typeConstructor))
            return antetype.TypeArguments[0];

        return IPlainType.Unknown;
    }

    public static partial void AwaitExpression_Contribute_Diagnostics(IAwaitExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO eliminate code duplication with AwaitExpression_Antetype
        if (node.Expression?.PlainType is OrdinaryNamedPlainType { TypeConstructor: var typeConstructor }
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
    {
        if (node.Operand?.PlainType is OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor antetype })
            return antetype.Not().PlainType;
        return IPlainType.Bool;
    }

    private static IMaybePlainType UnaryOperatorExpression_PlainType_Minus(IUnaryOperatorExpressionNode node)
    {
        if (node.Operand?.PlainType is not OrdinaryNamedPlainType plainType) return IPlainType.Unknown;
        return plainType.TypeConstructor switch
        {
            IntegerLiteralTypeConstructor t => t.Negate().PlainType,
            FixedSizeIntegerTypeConstructor t => t.WithSign().PlainType,
            PointerSizedIntegerTypeConstructor t => t.WithSign().PlainType,
            // Even if unsigned before, it is signed now
            BigIntegerTypeConstructor _ => IPlainType.Int,
            _ => IPlainType.Unknown,
        };
    }

    private static IMaybePlainType UnaryOperatorExpression_PlainType_Plus(IUnaryOperatorExpressionNode node)
        => node.Operand?.PlainType switch
        {
            OrdinaryNamedPlainType { TypeConstructor: INumericTypeConstructor t } => t.PlainType,
            _ => IPlainType.Unknown,
        };

    public static partial void UnaryOperatorExpression_Contribute_Diagnostics(
        IUnaryOperatorExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var operandPlainType = node.Operand!.PlainType;
        var cannotBeAppliedToOperandType
            = operandPlainType is not OrdinaryNamedPlainType { TypeConstructor: var typeConstructor }
              || node.Operator switch
              {
                  UnaryOperator.Not => typeConstructor is not (BoolTypeConstructor or BoolLiteralTypeConstructor),
                  UnaryOperator.Minus => typeConstructor is not INumericTypeConstructor,
                  UnaryOperator.Plus => typeConstructor is not INumericTypeConstructor,
                  _ => throw ExhaustiveMatch.Failed(node.Operator),
              };

        if (cannotBeAppliedToOperandType)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(node.File,
                node.Syntax.Span, node.Operator, node.Operand!.Type));
    }

    public static partial IMaybePlainType GetterInvocationExpression_PlainType(IGetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ReturnType.ToPlainType() ?? IPlainType.Unknown;
        var boundAntetype = node.Context.PlainType.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybePlainType SetterInvocationExpression_PlainType(ISetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ParameterTypes[0].Type.ToPlainType() ?? IPlainType.Unknown;
        var boundAntetype = node.Context.PlainType.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybePlainType FunctionReferenceInvocationExpression_PlainType(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionPlainType.Return;

    /// <remarks>Can't be an eager attribute because accessing <see cref="IFunctionReferenceInvocationExpressionNode.Expression"/>
    /// requires checking the parent of the <paramref name="node"/>.</remarks>
    public static partial FunctionPlainType FunctionReferenceInvocationExpression_FunctionPlainType(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionPlainType)node.Expression.PlainType;

    public static partial IMaybePlainType InitializerInvocationExpression_PlainType(IInitializerInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ReturnType.ToPlainType() ?? IPlainType.Unknown;
        var boundAntetype = node.InitializerGroup.InitializingPlainType.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybePlainType FunctionName_PlainType(IFunctionNameNode node)
        // TODO should probably use PlainType on the declaration
        => node.ReferencedDeclaration?.Type.ToPlainType() ?? IPlainType.Unknown;

    public static partial IMaybePlainType MethodName_PlainType(IMethodNameNode node)
        // TODO should probably use PlainType on the declaration
        => node.ReferencedDeclaration?.MethodGroupType.ToPlainType() ?? IPlainType.Unknown;

    // TODO this is strange and maybe a hack
    public static partial IMaybePlainType? MethodName_Context_ExpectedPlainType(IMethodNameNode node)
        // TODO it would be better if this didn't depend on types, but only on antetypes
        => (node.Parent as IMethodInvocationExpressionNode)
           ?.ContextualizedCall?.SelfParameterType?.Type.ToPlainType();

    public static partial IExpressionNode? Expression_Rewrite_ImplicitConversion(IExpressionNode node)
    {
        if (node.ShouldNotBeExpression()) return null;

        // To minimize outstanding rewrites, first check whether node.PlainType could possibly
        // support conversion. If node.ExpectedAntetype is checked, that is inherited and if a
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
            OrdinaryNamedPlainType { TypeConstructor: var typeConstructor }
                => CanPossiblyImplicitlyConvertFrom(typeConstructor),
            OptionalPlainType { Referent: var referent } => CanPossiblyImplicitlyConvertFrom(referent),
            _ => false,
        };

    private static bool CanPossiblyImplicitlyConvertFrom(TypeConstructor fromTypeConstructor)
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
            case (IPlainType to, IPlainType from) when from.Equals(to):
                return null;
            case (OrdinaryNamedPlainType { TypeConstructor: FixedSizeIntegerTypeConstructor to },
                OrdinaryNamedPlainType { TypeConstructor: FixedSizeIntegerTypeConstructor from }):
                if (to.Bits > from.Bits && (!from.IsSigned || to.IsSigned))
                    return to;
                return null;
            case (OrdinaryNamedPlainType { TypeConstructor: FixedSizeIntegerTypeConstructor to },
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor from }):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                return to.Bits >= bits && (!requireSigned || to.IsSigned) ? to : null;
            }
            case (OrdinaryNamedPlainType { TypeConstructor: PointerSizedIntegerTypeConstructor to },
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor from }):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                // Must fit in 32 bits so that it will fit on all platforms
                return bits <= 32 && (!requireSigned || to.IsSigned) ? to : null;
            }
            // Note: Both signed BigIntegerAntetype has already been covered
            case (OrdinaryNamedPlainType { TypeConstructor: BigIntegerTypeConstructor { IsSigned: true } },
                OrdinaryNamedPlainType { TypeConstructor: IntegerTypeConstructor }):
            case (OrdinaryNamedPlainType { TypeConstructor: BigIntegerTypeConstructor { IsSigned: true } },
                OrdinaryNamedPlainType { TypeConstructor: IntegerLiteralTypeConstructor }):
                return TypeConstructor.Int;
            case (OrdinaryNamedPlainType { TypeConstructor: BigIntegerTypeConstructor to },
                OrdinaryNamedPlainType
                {
                    TypeConstructor: IntegerTypeConstructor { IsSigned: false }
                                        or IntegerLiteralTypeConstructor { IsSigned: false }
                }):
                return to;
            case (OrdinaryNamedPlainType { TypeConstructor: BoolTypeConstructor },
                OrdinaryNamedPlainType { TypeConstructor: BoolLiteralTypeConstructor }):
                return TypeConstructor.Bool;
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
        => node.Value ? IPlainType.True : IPlainType.False;
}
