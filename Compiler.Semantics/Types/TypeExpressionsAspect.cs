using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Attributes for types related to any sort of type expression (e.g. `Foo?`)
/// </summary>
internal static class TypeExpressionsAspect
{
    public static DataType TypeName_Type(ITypeNameNode node)
        => (node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType()) ?? DataType.Unknown;

    public static DataType CapabilityType_Type(ICapabilityTypeNode node)
        => (node.Referent as ITypeNameNode)?.BareType?.With(node.Capability.Capability) ?? node.Referent.Type;

    public static void CapabilityType_ContributeDiagnostics(ICapabilityTypeNode node, Diagnostics diagnostics)
    {
        var capability = node.Capability.Capability;
        if (capability.AllowsWrite && node.Type is CapabilityType { IsDeclaredConst: true } capabilityType)
            diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(node.File, node.Syntax, capability,
                capabilityType.DeclaredType));
        if (node.Referent.Type is GenericParameterType)
            diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(node.File, node.Syntax));
        if (node.Referent.Type is EmptyType)
            diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(node.File, node.Syntax));
        // TODO I think there are more errors that can happen
    }

    public static DataType OptionalType_Type(IOptionalTypeNode node)
        => node.Referent.Type.ToOptional();

    public static DataType FunctionType_Type(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), new(node.Return.Type));

    public static Parameter ParameterType_Parameter(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.Type);

    public static DataType CapabilityViewpointType_Type(ICapabilityViewpointTypeNode node)
        => CapabilityViewpointType.Create(node.Capability.Capability, node.Referent.Type);

    public static void CapabilityViewpointType_ContributeDiagnostics(
        ICapabilityViewpointTypeNode node,
        Diagnostics diagnostics)
    {
        if (node.Referent.Type is not GenericParameterType)
            diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }

    public static Pseudotype ConcreteMethodDeclaration_InheritedSelfType(IConcreteMethodDefinitionNode node)
        => node.SelfParameter.Type;

    public static DataType SelfViewpointType_Type(ISelfViewpointTypeNode node)
    {
        var selfType = node.SelfType;
        var referentType = node.Referent.Type;
        if (selfType is CapabilityType { Capability: var capability }
            && referentType is GenericParameterType genericParameterType)
            return CapabilityViewpointType.Create(capability, genericParameterType);

        if (selfType is CapabilityTypeConstraint { Capability: var capabilityConstraint })
            return SelfViewpointType.Create(capabilityConstraint, referentType);

        // TODO report error if self type is not applicable to referent

        return referentType;
    }

    public static void SelfViewpointType_ContributeDiagnostics(ISelfViewpointTypeNode node, Diagnostics diagnostics)
    {
        if (node.SelfType is not (CapabilityType or CapabilityTypeConstraint))
            diagnostics.Add(TypeError.SelfViewpointNotAvailable(node.File, node.Syntax));

        if (node.Referent.Type is not GenericParameterType)
            diagnostics.Add(TypeError.SelfViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }


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
}
