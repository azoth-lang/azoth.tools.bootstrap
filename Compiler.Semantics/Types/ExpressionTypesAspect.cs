using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public static class ExpressionTypesAspect
{
    public static ValueId Expression_ValueId(IExpressionNode node)
        => node.PreviousValueId().CreateNext();

    public static void NewObjectExpression_ContributeDiagnostics(INewObjectExpressionNode node, Diagnostics diagnostics)
        => CheckConstructingType(node.ConstructingType, diagnostics);

    private static void CheckConstructingType(ITypeNameNode node, Diagnostics diagnostics)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IStandardTypeNameNode n:
                CheckTypeArgumentsAreConstructable(n, diagnostics);
                break;
            case ISpecialTypeNameNode n:
                diagnostics.Add(TypeError.SpecialTypeCannotBeUsedHere(node.File, n.Syntax));
                break;
            case IQualifiedTypeNameNode n:
                diagnostics.Add(TypeError.TypeParameterCannotBeUsedHere(node.File, n.Syntax));
                break;
        }
    }

    public static void CheckTypeArgumentsAreConstructable(
        IStandardTypeNameNode node,
        Diagnostics diagnostics)
    {
        var bareType = node.NamedBareType;
        if (bareType is null) return;

        foreach (GenericParameterArgument arg in bareType.GenericParameterArguments)
            if (!arg.IsConstructable())
                diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(node.File, node.Syntax,
                    arg.Parameter, arg.Argument));
    }

    public static DataType IdExpression_Type(IIdExpressionNode node)
    {
        var referentType = node.FinalReferent.Type;
        if (referentType is CapabilityType capabilityType)
            return capabilityType.With(Capability.Identity);
        return DataType.Unknown;
    }

    public static void IdExpression_ContributeDiagnostics(IIdExpressionNode node, Diagnostics diagnostics)
    {
        //if (node.Type is not UnknownType)
        //    return;

        //var referentType = ((IExpressionNode)node.Referent).Type;
        //if (referentType is not CapabilityType)
        //    diagnostics.Add(TypeError.CannotIdNonReferenceType(node.File, node.Syntax.Span, referentType));
    }

    public static DataType VariableNameExpression_Type(IVariableNameExpressionNode node)
        => node.FlowStateAfter.AliasType(node.ReferencedDeclaration);

    public static FlowState VariableNameExpression_FlowStateAfter(IVariableNameExpressionNode node)
        => node.FlowStateBefore().Alias(node.ReferencedDeclaration, node.ValueId);

    public static FlowState NamedParameter_FlowStateAfter(INamedParameterNode node)
        => node.FlowStateBefore().Declare(node);

    public static ValueId VariableDeclarationStatement_ValueId(IVariableDeclarationStatementNode node)
        => node.PreviousValueId().CreateNext();

    public static ValueId BindingPattern_ValueId(IBindingPatternNode node)
        => node.PreviousValueId().CreateNext();

    public static DataType UnsafeExpression_Type(IUnsafeExpressionNode node)
        => node.FinalExpression.Type;

    public static DataType FunctionInvocationExpression_Type(IFunctionInvocationExpressionNode node)
        => node.ReferencedDeclaration?.Type.Return.Type ?? DataType.Unknown;

    public static BoolConstValueType BoolLiteralExpression_Type(IBoolLiteralExpressionNode node)
        => node.Value ? DataType.True : DataType.False;

    public static IntegerConstValueType IntegerLiteralExpression_Type(IIntegerLiteralExpressionNode node)
        => new IntegerConstValueType(node.Value);

    public static OptionalType NoneLiteralExpression_Type(INoneLiteralExpressionNode _)
        => DataType.None;

    public static DataType StringLiteralExpression_Type(IStringLiteralExpressionNode node)
    {
        var typeSymbolNode = node.ContainingLexicalScope.Lookup(StringTypeName).OfType<ITypeDeclarationNode>().TrySingle();
        return typeSymbolNode?.Symbol.GetDeclaredType()?.With(Capability.Constant, FixedList.Empty<DataType>()) ?? DataType.Unknown;
    }

    public static void StringLiteralExpression_ContributeDiagnostics(
        IStringLiteralExpressionNode node,
        Diagnostics diagnostics)
    {
        if (node.Type is UnknownType)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.Span, "Could not find string type for string literal."));
    }

    private static readonly IdentifierName StringTypeName = "String";

    public static DataType MethodInvocationExpression_Type(IMethodInvocationExpressionNode node)
    {
        var selfType = node.MethodGroup.Context.Type;
        // TODO can flow typing affect this?
        return node.ReferencedDeclaration?.MethodGroupType.Return.Type.ReplaceSelfWith(selfType)
               ?? DataType.Unknown;
    }

    public static ContextualizedOverload<IStandardMethodDeclarationNode>? MethodInvocationExpression_ContextualizedOverload(
        IMethodInvocationExpressionNode node)
        => node.ReferencedDeclaration is not null
            ? ContextualizedOverload.Create(node.MethodGroup.Context.Type, node.ReferencedDeclaration)
            : null;

    public static FlowState MethodInvocationExpression_FlowStateAfter(IMethodInvocationExpressionNode node)
    {
        // The flow state just before the method is called is the state after all arguments have evaluated
        var flowState = node.FinalArguments.LastOrDefault()?.FlowStateAfter ?? node.MethodGroup.Context.FlowStateAfter;
        var argumentValueIds = ArgumentValueIds(node.ContextualizedOverload, node.MethodGroup.Context, node.FinalArguments);
        return flowState.CombineArguments(argumentValueIds, node.ValueId);
    }

    private static IEnumerable<ArgumentValueId> ArgumentValueIds(
        ContextualizedOverload? overload,
        IExpressionNode? selfArgument,
        IEnumerable<IExpressionNode> arguments)
    {
        arguments = selfArgument.YieldValue().Concat(arguments);
        return overload is null
            ? arguments.Select(a => new ArgumentValueId(false, a.ValueId))
            : (overload.SelfParameterType?.ToUpperBound()).YieldValue()
                  .Concat(overload.ParameterTypes)
                  .EquiZip(arguments)
                  .Select((p, a) => new ArgumentValueId(p.IsLent, a.ValueId));
    }
}
