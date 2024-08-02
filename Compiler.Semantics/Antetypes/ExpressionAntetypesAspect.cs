using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class ExpressionAntetypesAspect
{
    public static IMaybeExpressionAntetype UnsafeExpression_Antetype(IUnsafeExpressionNode node)
        => node.IntermediateExpression?.Antetype ?? IAntetype.Unknown;

    public static IMaybeExpressionAntetype FunctionInvocationExpression_Antetype(IFunctionInvocationExpressionNode node)
        // TODO should probably use Antetype on the declaration
        => node.ReferencedDeclaration?.Type.Return.Type.ToAntetype() ?? IAntetype.Unknown;

    public static IMaybeExpressionAntetype MethodInvocationExpression_Antetype(IMethodInvocationExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var unboundAntetype = node.ReferencedDeclaration?.MethodGroupType.Return.Type.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.MethodGroup.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static IMaybeExpressionAntetype VariableNameExpression_Antetype(IVariableNameExpressionNode node)
        => node.ReferencedDefinition.BindingAntetype;

    public static IMaybeExpressionAntetype SelfExpression_Antetype(ISelfExpressionNode node)
        => node.ReferencedDefinition?.BindingAntetype ?? IAntetype.Unknown;

    public static IMaybeExpressionAntetype FieldAccessExpression_Antetype(IFieldAccessExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var unboundAntetype = node.ReferencedDeclaration.BindingType.ToAntetype();
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static IMaybeExpressionAntetype NewObjectExpression_Antetype(INewObjectExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var unboundType = node.ReferencedConstructor?.Symbol.ReturnType.ToAntetype() ?? IAntetype.Unknown;
        var boundType = node.ConstructingAntetype.ReplaceTypeParametersIn(unboundType);
        return boundType;
    }

    public static IMaybeExpressionAntetype AssignmentExpression_Antetype(IAssignmentExpressionNode node)
        => node.LeftOperand.Antetype;

    public static IMaybeAntetype ResultStatement_Antetype(IResultStatementNode node)
        => node.IntermediateExpression?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IAntetype? BinaryOperatorExpression_NumericOperatorCommonAntetype(IBinaryOperatorExpressionNode node)
    {
        var leftAntetype = node.IntermediateLeftOperand?.Antetype ?? IAntetype.Unknown;
        var rightAntetype = node.IntermediateRightOperand?.Antetype ?? IAntetype.Unknown;
        return (leftAntetype, node.Operator, rightAntetype) switch
        {
            (IntegerConstValueAntetype, BinaryOperator.Plus, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.Minus, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.Asterisk, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.Slash, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.EqualsEquals, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.NotEqual, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.LessThan, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.LessThanOrEqual, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.GreaterThan, IntegerConstValueAntetype) => null,
            (IntegerConstValueAntetype, BinaryOperator.GreaterThanOrEqual, IntegerConstValueAntetype) => null,

            (BoolConstValueAntetype, BinaryOperator.EqualsEquals, BoolConstValueAntetype) => null,
            (BoolConstValueAntetype, BinaryOperator.NotEqual, BoolConstValueAntetype) => null,
            (BoolConstValueAntetype, BinaryOperator.And, BoolConstValueAntetype) => null,
            (BoolConstValueAntetype, BinaryOperator.Or, BoolConstValueAntetype) => null,

            // TODO this is the legacy reference equality operator that needs to be changed
            (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.EqualsEquals, INonVoidAntetype { HasReferenceSemantics: true })
                or (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.NotEqual, INonVoidAntetype { HasReferenceSemantics: true })
                => null,

            (BoolAntetype, BinaryOperator.EqualsEquals, BoolAntetype)
                or (BoolAntetype, BinaryOperator.NotEqual, BoolAntetype)
                or (BoolAntetype, BinaryOperator.And, BoolAntetype)
                or (BoolAntetype, BinaryOperator.Or, BoolAntetype)
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

    public static IMaybeExpressionAntetype BinaryOperatorExpression_Antetype(IBinaryOperatorExpressionNode node)
    {
        var leftAntetype = node.IntermediateLeftOperand?.Antetype ?? IAntetype.Unknown;
        var rightAntetype = node.IntermediateRightOperand?.Antetype ?? IAntetype.Unknown;
        return (leftAntetype, node.Operator, rightAntetype) switch
        {
            (IntegerConstValueAntetype left, BinaryOperator.Plus, IntegerConstValueAntetype right) => left.Add(right),
            (IntegerConstValueAntetype left, BinaryOperator.Minus, IntegerConstValueAntetype right) => left.Subtract(right),
            (IntegerConstValueAntetype left, BinaryOperator.Asterisk, IntegerConstValueAntetype right) => left.Multiply(right),
            (IntegerConstValueAntetype left, BinaryOperator.Slash, IntegerConstValueAntetype right) => left.DivideBy(right),
            (IntegerConstValueAntetype left, BinaryOperator.EqualsEquals, IntegerConstValueAntetype right) => left.Equals(right),
            (IntegerConstValueAntetype left, BinaryOperator.NotEqual, IntegerConstValueAntetype right) => left.NotEquals(right),
            (IntegerConstValueAntetype left, BinaryOperator.LessThan, IntegerConstValueAntetype right) => left.LessThan(right),
            (IntegerConstValueAntetype left, BinaryOperator.LessThanOrEqual, IntegerConstValueAntetype right) => left.LessThanOrEqual(right),
            (IntegerConstValueAntetype left, BinaryOperator.GreaterThan, IntegerConstValueAntetype right) => left.GreaterThan(right),
            (IntegerConstValueAntetype left, BinaryOperator.GreaterThanOrEqual, IntegerConstValueAntetype right) => left.GreaterThanOrEqual(right),

            (BoolConstValueAntetype left, BinaryOperator.EqualsEquals, BoolConstValueAntetype right) => left.Equals(right),
            (BoolConstValueAntetype left, BinaryOperator.NotEqual, BoolConstValueAntetype right) => left.NotEquals(right),
            (BoolConstValueAntetype left, BinaryOperator.And, BoolConstValueAntetype right) => left.And(right),
            (BoolConstValueAntetype left, BinaryOperator.Or, BoolConstValueAntetype right) => left.Or(right),

            // TODO this is the legacy reference equality operator that needs to be changed
            (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.EqualsEquals, INonVoidAntetype { HasReferenceSemantics: true })
                or (INonVoidAntetype { HasReferenceSemantics: true }, BinaryOperator.NotEqual, INonVoidAntetype { HasReferenceSemantics: true })
                => IAntetype.Bool,

            (BoolAntetype, BinaryOperator.EqualsEquals, BoolAntetype)
                or (BoolAntetype, BinaryOperator.NotEqual, BoolAntetype)
                or (BoolAntetype, BinaryOperator.And, BoolAntetype)
                or (BoolAntetype, BinaryOperator.Or, BoolAntetype)
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
        => commonAntetype ?? IAntetype.UnknownMaybeAntetype;

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
                            ?? IAntetype.UnknownMaybeAntetype;
        return rangeAntetype;
    }

    public static IMaybeExpressionAntetype StringLiteralExpression_Antetype(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName)
                                 .OfType<ITypeDeclarationNode>().TrySingle();
        return (IMaybeExpressionAntetype?)typeSymbolNode?.Symbol.GetDeclaredType()?.ToAntetype() ?? IAntetype.Unknown;
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static IMaybeExpressionAntetype IfExpression_Antetype(IIfExpressionNode node)
    {
        if (node.ElseClause is null) return node.ThenBlock.Antetype.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Antetype;
    }

    public static IMaybeExpressionAntetype WhileExpression_Antetype(IWhileExpressionNode _)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static IMaybeExpressionAntetype LoopExpression_Antetype(ILoopExpressionNode _)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static IMaybeExpressionAntetype ForeachExpression_Antetype(IForeachExpressionNode _)
        // TODO assign correct type to the expression
        => IAntetype.Void;

    public static IMaybeAntetype BlockExpression_Antetype(IBlockExpressionNode node)
    {
        foreach (var statement in node.Statements)
            if (statement.ResultAntetype is not null and var resultAntetype)
                return resultAntetype;

        // If there was no result expression, then the block type is void
        return IAntetype.Void;
    }

    public static IMaybeExpressionAntetype ConversionExpression_Antetype(IConversionExpressionNode node)
    {
        var convertToAntetype = node.ConvertToType.NamedAntetype;
        if (node.Operator == ConversionOperator.Optional)
            convertToAntetype = convertToAntetype.MakeOptional();
        return convertToAntetype;
    }

    public static IMaybeExpressionAntetype NoneLiteralExpression_Antetype(INoneLiteralExpressionNode _)
        => IAntetype.None;

    public static IMaybeExpressionAntetype AsyncStartExpression_Antetype(IAsyncStartExpressionNode node)
        => Intrinsic.PromiseOf(node.IntermediateExpression?.Antetype ?? IAntetype.Unknown);

    public static IMaybeExpressionAntetype AwaitExpression_Antetype(IAwaitExpressionNode node)
    {
        if (node.IntermediateExpression?.Antetype is UserGenericNominalAntetype { DeclaredAntetype: var declaredAntetype } antetype
            && Intrinsic.PromiseAntetype.Equals(declaredAntetype))
            return antetype.TypeArguments[0];

        return IAntetype.Unknown;
    }

    public static void AwaitExpression_ContributeDiagnostics(IAwaitExpressionNode node, Diagnostics diagnostics)
    {
        // TODO eliminate code duplication with AwaitExpression_Antetype
        if (node.IntermediateExpression?.Antetype is UserGenericNominalAntetype { DeclaredAntetype: var declaredAntetype }
            && Intrinsic.PromiseAntetype.Equals(declaredAntetype))
            return;

        diagnostics.Add(TypeError.CannotAwaitType(node.File, node.Syntax.Span, node.IntermediateExpression!.Type));
    }

    public static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype(IUnaryOperatorExpressionNode node)
        => node.Operator switch
        {
            UnaryOperator.Not => UnaryOperatorExpression_Antetype_Not(node),
            UnaryOperator.Minus => UnaryOperatorExpression_Antetype_Minus(node),
            UnaryOperator.Plus => UnaryOperatorExpression_Antetype_Plus(node),
            _ => throw ExhaustiveMatch.Failed(node.Operator),
        };

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Not(IUnaryOperatorExpressionNode node)
    {
        if (node.IntermediateOperand?.Antetype is BoolConstValueAntetype antetype) return antetype.Not();
        return IAntetype.Bool;
    }

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Minus(IUnaryOperatorExpressionNode node)
        => node.IntermediateOperand?.Antetype switch
        {
            IntegerConstValueAntetype t => t.Negate(),
            FixedSizeIntegerAntetype t => t.WithSign(),
            PointerSizedIntegerAntetype t => t.WithSign(),
            // Even if unsigned before, it is signed now
            BigIntegerAntetype _ => IAntetype.Int,
            _ => IAntetype.Unknown,
        };

    private static IMaybeExpressionAntetype UnaryOperatorExpression_Antetype_Plus(IUnaryOperatorExpressionNode node)
        => node.IntermediateOperand?.Antetype switch
        {
            INumericAntetype t => t,
            _ => IAntetype.Unknown,
        };

    public static void UnaryOperatorExpression_ContributeDiagnostics(
        IUnaryOperatorExpressionNode node,
        Diagnostics diagnostics)
    {
        var operandAntetype = node.IntermediateOperand!.Antetype;
        var cannotBeAppliedToOperandType = node.Operator switch
        {
            UnaryOperator.Not => operandAntetype is not (BoolAntetype or BoolConstValueAntetype),
            UnaryOperator.Minus => operandAntetype is not INumericAntetype,
            UnaryOperator.Plus => operandAntetype is not INumericAntetype,
            _ => throw ExhaustiveMatch.Failed(node.Operator),
        };

        if (cannotBeAppliedToOperandType)
            diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(node.File,
                node.Syntax.Span, node.Operator, node.IntermediateOperand!.Type));
    }

    public static IMaybeExpressionAntetype GetterInvocationExpression_Antetype(IGetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.Symbol.Return.Type.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static IMaybeExpressionAntetype SetterInvocationExpression_Antetype(ISetterInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.Symbol.Parameters[0].Type.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.Context.Antetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static IMaybeExpressionAntetype FunctionReferenceInvocation_Antetype(IFunctionReferenceInvocationExpressionNode node)
        => node.FunctionAntetype.Return;

    public static IMaybeExpressionAntetype InitializerInvocationExpression_Antetype(
        IInitializerInvocationExpressionNode node)
    {
        var unboundAntetype = node.ReferencedDeclaration?.Symbol.Return.Type.ToAntetype() ?? IAntetype.Unknown;
        var boundAntetype = node.InitializerGroup.InitializingAntetype.ReplaceTypeParametersIn(unboundAntetype);
        return boundAntetype;
    }

    public static IMaybeExpressionAntetype FunctionName_Antetype(IFunctionNameNode node)
        // TODO should probably use Antetype on the declaration
        => node.ReferencedDeclaration?.Type.ToAntetype() ?? IAntetype.Unknown;

    public static IExpressionNode? Expression_Rewrite_ImplicitConversion(IExpressionNode node)
    {
        if (ImplicitlyConvertToType(node.ExpectedAntetype, node.Antetype) is SimpleAntetype convertToAntetype)
            return new ImplicitConversionExpressionNode(node, convertToAntetype);

        return null;
    }

    private static SimpleAntetype? ImplicitlyConvertToType(IMaybeExpressionAntetype? toType, IMaybeExpressionAntetype fromType)
    {
        switch (toType, fromType)
        {
            case (null, _):
            case (UnknownAntetype, _):
            case (_, UnknownAntetype):
            case (IExpressionAntetype to, IExpressionAntetype from) when from.Equals(to):
                return null;
            case (FixedSizeIntegerAntetype to, FixedSizeIntegerAntetype from):
                if (to.Bits > from.Bits && (!from.IsSigned || to.IsSigned))
                    return to;
                return null;
            case (FixedSizeIntegerAntetype to, IntegerConstValueAntetype from):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                return to.Bits >= bits && (!requireSigned || to.IsSigned) ? to : null;
            }
            case (PointerSizedIntegerAntetype to, IntegerConstValueAntetype from):
            {
                // TODO make a method on antetypes for this check
                var requireSigned = from.Value < 0;
                var bits = from.Value.GetByteCount(!to.IsSigned) * 8;
                // Must fit in 32 bits so that it will fit on all platforms
                return bits <= 32 && (!requireSigned || to.IsSigned) ? to : null;
            }
            case (BigIntegerAntetype { IsSigned: true }, IntegerAntetype):
            case (BigIntegerAntetype { IsSigned: true }, IntegerConstValueAntetype):
                return IAntetype.Int;
            case (BigIntegerAntetype to, IntegerAntetype { IsSigned: false }
                                        or IntegerConstValueAntetype { IsSigned: false }):
                return to;
            case (BoolAntetype, BoolConstValueAntetype):
                return IAntetype.Bool;
            default:
                return null;
        }
    }

    public static void OptionalPattern_ContributeDiagnostics(IOptionalPatternNode node, Diagnostics diagnostics)
    {
        if (node.InheritedBindingAntetype() is not OptionalAntetype)
            diagnostics.Add(TypeError.OptionalPatternOnNonOptionalType(node.File, node.Syntax, node.InheritedBindingType()));
    }
}
