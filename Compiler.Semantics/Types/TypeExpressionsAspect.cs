using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
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
    public static DataType TypeName_NamedType(ITypeNameNode node)
        => (node.NamedBareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType()) ?? DataType.Unknown;

    public static DataType CapabilityType_NamedType(ICapabilityTypeNode node)
        => (node.Referent as ITypeNameNode)?.NamedBareType?.With(node.Capability.Capability) ?? node.Referent.NamedType;

    public static void CapabilityType_ContributeDiagnostics(ICapabilityTypeNode node, Diagnostics diagnostics)
    {
        var capability = node.Capability.Capability;
        if (capability.AllowsWrite && node.NamedType is CapabilityType { IsDeclaredConst: true } capabilityType)
            diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(node.File, node.Syntax, capability,
                capabilityType.DeclaredType));
        if (node.Referent.NamedType is GenericParameterType)
            diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(node.File, node.Syntax));
        if (node.Referent.NamedType is EmptyType)
            diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(node.File, node.Syntax));
        // TODO I think there are more errors that can happen
    }

    public static DataType OptionalType_NamedType(IOptionalTypeNode node)
        => node.Referent.NamedType.MakeOptional();

    public static DataType FunctionType_NamedType(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), new(node.Return.NamedType));

    public static Parameter ParameterType_Parameter(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.NamedType);

    public static DataType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node)
        => CapabilityViewpointType.Create(node.Capability.Capability, node.Referent.NamedType);

    public static void CapabilityViewpointType_ContributeDiagnostics(
        ICapabilityViewpointTypeNode node,
        Diagnostics diagnostics)
    {
        if (node.Referent.NamedType is not GenericParameterType)
            diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }

    public static Pseudotype ConcreteMethodDeclaration_InheritedSelfType(IConcreteMethodDefinitionNode node)
        => node.SelfParameter.Type;

    public static DataType SelfViewpointType_NamedType(ISelfViewpointTypeNode node)
    {
        var selfType = node.NamedSelfType;
        var referentType = node.Referent.NamedType;
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
        if (node.NamedSelfType is not (CapabilityType or CapabilityTypeConstraint))
            diagnostics.Add(TypeError.SelfViewpointNotAvailable(node.File, node.Syntax));

        if (node.Referent.NamedType is not GenericParameterType)
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
