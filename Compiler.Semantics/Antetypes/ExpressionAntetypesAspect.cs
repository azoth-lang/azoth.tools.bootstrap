using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class ExpressionAntetypesAspect
{
    public static partial IMaybeExpressionAntetype UnsafeExpression_Antetype(IUnsafeExpressionNode node)
        => node.Expression?.Antetype ?? IAntetype.Unknown;

    public static partial IMaybeExpressionAntetype FunctionInvocationExpression_Antetype(IFunctionInvocationExpressionNode node)
        => node.Function.SelectedCallCandidate?.ReturnAntetype ?? IAntetype.Unknown;

    public static partial IMaybeExpressionAntetype MethodInvocationExpression_Antetype(IMethodInvocationExpressionNode node)
    {
        var unboundAntetype = node.Method.SelectedCallCandidate?.ReturnAntetype ?? IAntetype.Unknown;
        var boundAntetype = node.Method.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybeExpressionAntetype VariableNameExpression_Antetype(IVariableNameExpressionNode node)
        => node.ReferencedDefinition.BindingAntetype;

    public static partial IMaybeExpressionAntetype SelfExpression_Antetype(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingAntetype ?? IAntetype.Unknown;

    public static partial IMaybeExpressionAntetype FieldAccessExpression_Antetype(IFieldAccessExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var unboundAntetype = node.ReferencedDeclaration.BindingType.ToAntetype();
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybeExpressionAntetype NewObjectExpression_Antetype(INewObjectExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var unboundType = node.ReferencedConstructor?.ReturnType.ToAntetype() ?? IAntetype.Unknown;
        var boundType = node.ConstructingAntetype.ReplaceTypeParametersIn(unboundType);
        return boundType;
    }

    public static partial IMaybeExpressionAntetype AssignmentExpression_Antetype(IAssignmentExpressionNode node)
        => node.LeftOperand?.Antetype ?? IAntetype.Unknown;

    public static partial IMaybeAntetype ResultStatement_Antetype(IResultStatementNode node)
        => node.Expression?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static partial IAntetype? BinaryOperatorExpression_NumericOperatorCommonAntetype(IBinaryOperatorExpressionNode node)
    {
        var leftAntetype = node.LeftOperand?.Antetype ?? IAntetype.Unknown;
        var rightAntetype = node.RightOperand?.Antetype ?? IAntetype.Unknown;
        return (leftAntetype, node.Operator, rightAntetype) switch
        {
            (IntegerLiteralTypeConstructor, BinaryOperator.Plus, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.Minus, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.Asterisk, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.Slash, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.EqualsEquals, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.NotEqual, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.LessThan, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.LessThanOrEqual, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.GreaterThan, IntegerLiteralTypeConstructor) => null,
            (IntegerLiteralTypeConstructor, BinaryOperator.GreaterThanOrEqual, IntegerLiteralTypeConstructor) => null,

            (BoolLiteralTypeConstructor, BinaryOperator.EqualsEquals, BoolLiteralTypeConstructor) => null,
            (BoolLiteralTypeConstructor, BinaryOperator.NotEqual, BoolLiteralTypeConstructor) => null,
            (BoolLiteralTypeConstructor, BinaryOperator.And, BoolLiteralTypeConstructor) => null,
            (BoolLiteralTypeConstructor, BinaryOperator.Or, BoolLiteralTypeConstructor) => null,

            (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.ReferenceEquals, INonVoidAntetype { HasReferenceSemantics: true })
                or (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.NotReferenceEqual, INonVoidAntetype { HasReferenceSemantics: true })
                => null,

            (BoolTypeConstructor, BinaryOperator.EqualsEquals, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.NotEqual, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.And, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.Or, BoolTypeConstructor)
                => null,

            (IExpressionAntetype, BinaryOperator.Plus, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Minus, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Asterisk, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Slash, IExpressionAntetype)
                => ((IExpressionAntetype)leftAntetype).NumericOperatorCommonType((IExpressionAntetype)rightAntetype),
            (IExpressionAntetype, BinaryOperator.EqualsEquals, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.NotEqual, IExpressionAntetype)
                or (OptionalAntetype { Referent: IExpressionAntetype }, BinaryOperator.NotEqual, OptionalAntetype { Referent: IExpressionAntetype })
                or (IExpressionAntetype, BinaryOperator.LessThan, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.LessThanOrEqual, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.GreaterThan, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.GreaterThanOrEqual, IExpressionAntetype)
                => ((IExpressionAntetype)leftAntetype).NumericOperatorCommonType((IExpressionAntetype)rightAntetype),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                // TODO for the moment ranges are always integer ranges
                => IAntetype.Int,

            _ => null,
        };
    }

    public static partial IMaybeExpressionAntetype BinaryOperatorExpression_Antetype(IBinaryOperatorExpressionNode node)
    {
        var leftAntetype = node.LeftOperand?.Antetype ?? IAntetype.Unknown;
        var rightAntetype = node.RightOperand?.Antetype ?? IAntetype.Unknown;
        return (leftAntetype, node.Operator, rightAntetype) switch
        {
            (IntegerLiteralTypeConstructor left, BinaryOperator.Plus, IntegerLiteralTypeConstructor right) => left.Add(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.Minus, IntegerLiteralTypeConstructor right) => left.Subtract(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.Asterisk, IntegerLiteralTypeConstructor right) => left.Multiply(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.Slash, IntegerLiteralTypeConstructor right) => left.DivideBy(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.EqualsEquals, IntegerLiteralTypeConstructor right) => left.Equals(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.NotEqual, IntegerLiteralTypeConstructor right) => left.NotEquals(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.LessThan, IntegerLiteralTypeConstructor right) => left.LessThan(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.LessThanOrEqual, IntegerLiteralTypeConstructor right) => left.LessThanOrEqual(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.GreaterThan, IntegerLiteralTypeConstructor right) => left.GreaterThan(right),
            (IntegerLiteralTypeConstructor left, BinaryOperator.GreaterThanOrEqual, IntegerLiteralTypeConstructor right) => left.GreaterThanOrEqual(right),

            (BoolLiteralTypeConstructor left, BinaryOperator.EqualsEquals, BoolLiteralTypeConstructor right) => left.Equals(right),
            (BoolLiteralTypeConstructor left, BinaryOperator.NotEqual, BoolLiteralTypeConstructor right) => left.NotEquals(right),
            (BoolLiteralTypeConstructor left, BinaryOperator.And, BoolLiteralTypeConstructor right) => left.And(right),
            (BoolLiteralTypeConstructor left, BinaryOperator.Or, BoolLiteralTypeConstructor right) => left.Or(right),

            (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.ReferenceEquals, INonVoidAntetype { HasReferenceSemantics: true })
                or (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.NotReferenceEqual, INonVoidAntetype { HasReferenceSemantics: true })
                => IAntetype.Bool,

            (BoolTypeConstructor, BinaryOperator.EqualsEquals, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.NotEqual, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.And, BoolTypeConstructor)
                or (BoolTypeConstructor, BinaryOperator.Or, BoolTypeConstructor)
                => IAntetype.Bool,

            (IExpressionAntetype, BinaryOperator.Plus, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Minus, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Asterisk, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.Slash, IExpressionAntetype)
                => InferNumericOperatorType(node.NumericOperatorCommonAntetype),
            (IExpressionAntetype, BinaryOperator.EqualsEquals, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.NotEqual, IExpressionAntetype)
                or (OptionalAntetype { Referent: IExpressionAntetype }, BinaryOperator.NotEqual, OptionalAntetype { Referent: IExpressionAntetype })
                or (IExpressionAntetype, BinaryOperator.LessThan, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.LessThanOrEqual, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.GreaterThan, IExpressionAntetype)
                or (IExpressionAntetype, BinaryOperator.GreaterThanOrEqual, IExpressionAntetype)
                => InferComparisonOperatorType(node.NumericOperatorCommonAntetype),

            (_, BinaryOperator.DotDot, _)
                or (_, BinaryOperator.LessThanDotDot, _)
                or (_, BinaryOperator.DotDotLessThan, _)
                or (_, BinaryOperator.LessThanDotDotLessThan, _)
                => InferRangeOperatorType(node.ContainingLexicalScope),

            (OptionalAntetype { Referent: var referentType }, BinaryOperator.QuestionQuestion, NeverAntetype)
                => referentType,

            _ => IAntetype.Unknown

            // TODO optional types
        };
    }

    private static IMaybeExpressionAntetype InferNumericOperatorType(IAntetype? commonAntetype)
        => commonAntetype ?? IMaybeAntetype.Unknown;

    private static IMaybeExpressionAntetype InferComparisonOperatorType(IAntetype? commonAntetype)
    {
        if (commonAntetype is null) return IAntetype.Unknown;
        return IAntetype.Bool;
    }

    private static IMaybeExpressionAntetype InferRangeOperatorType(LexicalScope containingLexicalScope)
    {
        // TODO the left and right antetypes need to be compatible with the range type
        var rangeTypeDeclaration = containingLexicalScope.Lookup("azoth")
            .OfType<INamespaceDeclarationNode>().SelectMany(ns => ns.MembersNamed("range"))
            .OfType<ITypeDeclarationNode>().TrySingle();
        var rangeAntetype = (IAntetype?)rangeTypeDeclaration?.Symbol.GetDeclaredType()?.ToAntetype()
                            ?? IMaybeAntetype.Unknown;
        return rangeAntetype;
    }

    public static partial IMaybeExpressionAntetype StringLiteralExpression_Antetype(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName)
                                 .OfType<ITypeDeclarationNode>().TrySingle();
        return (IMaybeExpressionAntetype?)typeSymbolNode?.Symbol.GetDeclaredType()?.ToAntetype() ?? IAntetype.Unknown;
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static partial IMaybeExpressionAntetype IfExpression_Antetype(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.Antetype.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Antetype;
    }

    public static partial IMaybeExpressionAntetype WhileExpression_Antetype(IWhileExpressionNode node)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static partial IMaybeExpressionAntetype LoopExpression_Antetype(ILoopExpressionNode node)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static partial IMaybeExpressionAntetype ForeachExpression_Antetype(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static partial IMaybeAntetype BlockExpression_Antetype(IBlockExpressionNode node)
    {
        foreach (var statement in node.Statements)
            if (statement.ResultAntetype is not null and var resultAntetype)
                return resultAntetype;

        // If there was no result expression, then the block type is void
        return IAntetype.Void;
    }

    public static partial IMaybeExpressionAntetype ConversionExpression_Antetype(IConversionExpressionNode node)
    {
        var convertToAntetype = node.ConvertToType.NamedAntetype;
        if (node.Operator == ConversionOperator.Optional)
            convertToAntetype = convertToAntetype.MakeOptional();
        return convertToAntetype;
    }

    public static partial IMaybeExpressionAntetype NoneLiteralExpression_Antetype(INoneLiteralExpressionNode node)
        => IAntetype.None;

    public static partial IMaybeExpressionAntetype AsyncStartExpression_Antetype(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.Expression?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown);

    public static partial IMaybeExpressionAntetype AwaitExpression_Antetype(IAwaitExpressionNode node)
    {
        if (node.Expression?.Antetype is UserGenericNominalAntetype { DeclaredAntetype: var declaredAntetype } antetype
            && Intrinsic.PromiseAntetype.Equals(declaredAntetype))
            return antetype.TypeArguments[0];

        return IAntetype.Unknown;
    }

    public static partial void AwaitExpression_Contribute_Diagnostics(IAwaitExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO eliminate code duplication with AwaitExpression_Antetype
        if (node.Expression?.Antetype is UserGenericNominalAntetype { DeclaredAntetype: var declaredAntetype }
            && Intrinsic.PromiseAntetype.Equals(declaredAntetype))
            return;

        diagnostics.Add(TypeError.CannotAwaitType(node.File, node.Syntax.Span, node.Expression!.Type));
    }

    public static partial IMaybeExpressionAntetype UnaryOperatorExpression_Antetype(IUnaryOperatorExpressionNode node)
        => node.Operator switch
        {
            UnaryOperator.Not => UnaryOperatorExpression_Antetype_Not(node),
            UnaryOperator.Minus => UnaryOperatorExpression_Antetype_Minus(node),
            UnaryOperator.Plus => UnaryOperatorExpression_Antetype_Plus(node),
            _ => throw ExhaustiveMatch.Failed(node.Operator),
        };

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Not(IUnaryOperatorExpressionNode node)
    {
        if (node.Operand?.Antetype is BoolLiteralTypeConstructor antetype) return antetype.Not();
        return IAntetype.Bool;
    }

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Minus(IUnaryOperatorExpressionNode node)
        => node.Operand?.Antetype switch
        {
            IntegerLiteralTypeConstructor t => t.Negate(),
            FixedSizeIntegerTypeConstructor t => t.WithSign(),
            PointerSizedIntegerTypeConstructor t => t.WithSign(),
            // Even if unsigned before, it is signed now
            BigIntegerTypeConstructor _ => IAntetype.Int,
            _ => IAntetype.Unknown,
        };

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Plus(IUnaryOperatorExpressionNode node)
        => node.Operand?.Antetype switch
        {
            INumericAntetype t => t,
            _ => IAntetype.Unknown,
        };

    public static partial void UnaryOperatorExpression_Contribute_Diagnostics(
        IUnaryOperatorExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var operandAntetype = node.Operand!.Antetype;
        var cannotBeAppliedToOperandType = node.Operator switch
        {
            UnaryOperator.Not => operandAntetype is not (BoolTypeConstructor or BoolLiteralTypeConstructor),
            UnaryOperator.Minus => operandAntetype is not INumericAntetype,
            UnaryOperator.Plus => operandAntetype is not INumericAntetype,
            _ => throw ExhaustiveMatch.Failed(node.Operator),
        };

        if (cannotBeAppliedToOperandType)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(node.File,
                node.Syntax.Span, node.Operator, node.Operand!.Type));
    }

    public static partial IMaybeExpressionAntetype GetterInvocationExpression_Antetype(IGetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ReturnType.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybeExpressionAntetype SetterInvocationExpression_Antetype(ISetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ParameterTypes[0].Type.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybeExpressionAntetype FunctionReferenceInvocationExpression_Antetype(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionAntetype.Return;

    /// <remarks>Can't be an eager attribute because accessing <see cref="IFunctionReferenceInvocationExpressionNode.Expression"/>
    /// requires checking the parent of the <paramref name="node"/>.</remarks>
    public static partial FunctionAntetype FunctionReferenceInvocationExpression_FunctionAntetype(IFunctionReferenceInvocationExpressionNode node)
        => (FunctionAntetype)node.Expression.Antetype;

    public static partial IMaybeExpressionAntetype InitializerInvocationExpression_Antetype(IInitializerInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.ReturnType.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.InitializerGroup.InitializingAntetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static partial IMaybeExpressionAntetype FunctionName_Antetype(IFunctionNameNode node)
        // TODO should probably use Antetype on the declaration
        => node.ReferencedDeclaration?.Type.ToAntetype() ?? IAntetype.Unknown;

    public static partial IMaybeExpressionAntetype MethodName_Antetype(IMethodNameNode node)
        // TODO should probably use Antetype on the declaration
        => node.ReferencedDeclaration?.MethodGroupType.ToAntetype() ?? IAntetype.Unknown;

    // TODO this is strange and maybe a hack
    public static partial IMaybeAntetype? MethodName_Context_ExpectedAntetype(IMethodNameNode node)
        // TODO it would be better if this didn't depend on types, but only on antetypes
        => (node.Parent as IMethodInvocationExpressionNode)
           ?.ContextualizedCall?.SelfParameterType?.Type.ToAntetype().ToNonConstValueType();

    public static partial IExpressionNode? Expression_Rewrite_ImplicitConversion(IExpressionNode node)
    {
        if (node.ShouldNotBeExpression()) return null;

        // To minimize outstanding rewrites, first check whether node.Antetype could possibly
        // support conversion. If node.ExpectedAntetype is checked, that is inherited and if a
        // rewrite is in progress, that can't be cached. Note: this requires thoroughly treating
        // T <: T? as a subtype and not an implicit conversion.
        if (!CanPossiblyImplicitlyConvertFrom(node.Antetype))
            return null;

        if (ImplicitlyConvertToType(node.ExpectedAntetype, node.Antetype) is SimpleTypeConstructor convertToAntetype)
            return IImplicitConversionExpressionNode.Create(node, convertToAntetype);

        return null;
    }

    private static bool CanPossiblyImplicitlyConvertFrom(IMaybeExpressionAntetype fromType)
    {
        return fromType switch
        {
            UnknownAntetype => false,
            BoolLiteralTypeConstructor => true,
            IntegerLiteralTypeConstructor => true,
            // Can't convert from signed because there is not larger type to convert to
            BigIntegerTypeConstructor t => !t.IsSigned,
            PointerSizedIntegerTypeConstructor => true,
            FixedSizeIntegerTypeConstructor => true,
            OptionalAntetype { Referent: var referent } => CanPossiblyImplicitlyConvertFrom(referent),
            _ => false,
        };
    }

    private static SimpleTypeConstructor? ImplicitlyConvertToType(IMaybeExpressionAntetype? toType, IMaybeExpressionAntetype fromType)
    {
        switch (toType, fromType)
        {
            case (null, _):
            case (UnknownAntetype, _):
            case (_, UnknownAntetype):
            case (IExpressionAntetype to, IExpressionAntetype from) when from.Equals(to):
                return null;
            case (FixedSizeIntegerTypeConstructor to, FixedSizeIntegerTypeConstructor from):
                if (to.Bits > from.Bits && (!from.IsSigned || to.IsSigned))
                    return to;
                return null;
            case (FixedSizeIntegerTypeConstructor to, IntegerLiteralTypeConstructor from):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                return to.Bits >= bits && (!requireSigned || to.IsSigned) ? to : null;
            }
            case (PointerSizedIntegerTypeConstructor to, IntegerLiteralTypeConstructor from):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                // Must fit in 32 bits so that it will fit on all platforms
                return bits <= 32 && (!requireSigned || to.IsSigned) ? to : null;
            }
            // Note: Both signed BigIntegerAntetype has already been covered
            case (BigIntegerTypeConstructor { IsSigned: true }, IntegerTypeConstructor):
            case (BigIntegerTypeConstructor { IsSigned: true }, IntegerLiteralTypeConstructor):
                return IAntetype.Int;
            case (BigIntegerTypeConstructor to, IntegerTypeConstructor { IsSigned: false }
                                        or IntegerLiteralTypeConstructor { IsSigned: false }):
                return to;
            case (BoolTypeConstructor, BoolLiteralTypeConstructor):
                return IAntetype.Bool;
            // TODO support lifted implicit conversions
            //case (OptionalAntetype { Referent: var to }, OptionalAntetype { Referent: var from }):
            //    return ImplicitlyConvertToType(to, from)?.MakeOptional();
            case (OptionalAntetype { Referent: var to }, _):
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

    public static partial IMaybeExpressionAntetype IntegerLiteralExpression_Antetype(IIntegerLiteralExpressionNode node)
        => new IntegerLiteralTypeConstructor(node.Value);

    public static partial IMaybeExpressionAntetype BoolLiteralExpression_Antetype(IBoolLiteralExpressionNode node)
        => node.Value ? IExpressionAntetype.True : IExpressionAntetype.False;
}
