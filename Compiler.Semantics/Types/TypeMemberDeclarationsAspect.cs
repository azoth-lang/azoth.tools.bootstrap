using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

// TODO shouldn't this be renamed to TypeMemberDefinitionsAspect?
internal static partial class TypeMemberDeclarationsAspect
{
    public static partial FunctionType FunctionDefinition_Type(IFunctionDefinitionNode node)
        => FunctionType(node.Parameters, node.Return);

    private static FunctionType FunctionType(IEnumerable<INamedParameterNode> parameters, ITypeNode? @return)
    {
        var parameterTypes = parameters.Select(p => p.ParameterType).ToFixedList();
        var returnType = @return?.NamedType ?? DataType.Void;
        return new FunctionType(parameterTypes, new ReturnType(returnType));
    }

    public static void MethodDefinition_ContributeDiagnostics(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
        => CheckParameterAndReturnAreVarianceSafe(node, diagnostics);

    private static void CheckParameterAndReturnAreVarianceSafe(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO do generic methods and functions need to be checked?

        var methodSymbol = node.Symbol;
        // Only methods declared in generic types need checked
        if (methodSymbol.ContainingSymbol is not UserTypeSymbol { DeclaresType.IsGeneric: true }) return;

        var nonwritableSelf = !node.SelfParameter.Capability.Constraint.AnyCapabilityAllowsWrite;

        // The `self` parameter does not get checked for variance safety. It will always operate on
        // the original type so it is safe.
        foreach (var parameter in node.Parameters)
        {
            var type = parameter.BindingType;
            if (!type.IsInputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.ParameterMustBeInputSafe(node.File, parameter.Syntax, type));
        }

        var returnType = methodSymbol.Return.Type;
        if (!returnType.IsOutputSafe(nonwritableSelf))
            diagnostics.Add(TypeError.ReturnTypeMustBeOutputSafe(node.File, node.Return!.Syntax, returnType));
    }

    public static DataType NamedParameter_BindingType(INamedParameterNode node)
        => node.TypeNode.NamedType;

    public static ParameterType NamedParameter_ParameterType(INamedParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return new(isLent, node.BindingType);
    }

    public static SelfParameterType SelfParameter_ParameterType(ISelfParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return new(isLent, node.BindingType);
    }

    public static Pseudotype MethodSelfParameter_BindingType(IMethodSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var genericParameterTypes = declaredType.GenericParameterTypes;
        var capability = node.Capability;
        // TODO shouldn't their be an overload of .With() that takes an ICapabilityConstraint (e.g. `capability.Constraint`)
        return capability switch
        {
            ICapabilityNode n => declaredType.With(n.Capability, genericParameterTypes),
            ICapabilitySetNode n => declaredType.With(n.Constraint, genericParameterTypes),
            _ => throw ExhaustiveMatch.Failed(capability)
        };
    }

    public static void MethodSelfParameter_ContributeDiagnostics(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckTypeCannotBeLent(node, diagnostics);

        CheckConstClassSelfParameterCannotHaveCapability(node, diagnostics);
    }

    private static void CheckTypeCannotBeLent(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = ((IParameterNode)node).BindingType;
        if (isLent && !selfType.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }

    private static void CheckConstClassSelfParameterCannotHaveCapability(
        IMethodSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var inConstClass = node.ContainingDeclaredType.IsDeclaredConst;
        var selfParameterType = node.ParameterType;
        var selfType = selfParameterType.Type;
        if (inConstClass
            && ((selfType is CapabilityType { Capability: var selfCapability }
                 && selfCapability != Capability.Constant
                 && selfCapability != Capability.Identity)
                || selfType is CapabilityTypeConstraint))
            diagnostics.Add(TypeError.ConstClassSelfParameterCannotHaveCapability(node.File, node.Syntax));
    }

    public static CapabilityType ConstructorSelfParameter_BindingType(IConstructorSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void ConstructorSelfParameter_ContributeDiagnostics(
        IConstructorSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    internal static DataType FieldParameter_BindingType(IFieldParameterNode node)
        => node.ReferencedField?.BindingType ?? DataType.Unknown;

    public static ParameterType FieldParameter_ParameterType(IFieldParameterNode node)
        => new ParameterType(false, node.BindingType);

    public static CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void InitializerSelfParameter_ContributeDiagnostics(
        IInitializerSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    private static void CheckInvalidConstructorSelfParameterCapability(
        ICapabilitySyntax capabilitySyntax,
        CodeFile file,
        DiagnosticCollectionBuilder diagnostics)
    {
        var declaredCapability = capabilitySyntax.Declared;
        switch (declaredCapability)
        {
            case DeclaredCapability.Read:
            case DeclaredCapability.Mutable:
                break;
            case DeclaredCapability.Isolated:
            case DeclaredCapability.TemporarilyIsolated:
            case DeclaredCapability.Constant:
            case DeclaredCapability.TemporarilyConstant:
            case DeclaredCapability.Identity:

                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capabilitySyntax));
                break;
            default:
                throw ExhaustiveMatch.Failed(declaredCapability);
        }
    }

    public static DataType FieldDefinition_BindingType(IFieldDefinitionNode node) => node.TypeNode.NamedType;

    public static void FieldDefinition_ContributeDiagnostics(IFieldDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckFieldIsVarianceSafe(node, diagnostics);

        CheckFieldMaintainsIndependence(node, diagnostics);
    }

    private static void CheckFieldIsVarianceSafe(IFieldDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = ((IFieldDeclarationNode)node).BindingType;

        // Check variance safety. Only public fields need their safety checked. Effectively, they
        // have getters and setters. Private and protected fields are only accessed from within the
        // class where the exact type parameters are known, so they are always safe.
        if (node.AccessModifier >= AccessModifier.Public)
        {
            if (node.IsMutableBinding)
            {
                // Mutable bindings can be both read and written to, so they must be both input and output
                // safe (i.e. invariant). Self is nonwritable for the output case which is where
                // self writable matters.
                if (!type.IsInputAndOutputSafe(nonwriteableSelf: true))
                    diagnostics.Add(TypeError.VarFieldMustBeInputAndOutputSafe(node.File, node.Syntax, type));
            }
            else
            {
                // Immutable bindings can only be read, so they must be output safe.
                if (!type.IsOutputSafe(nonwritableSelf: true))
                    diagnostics.Add(TypeError.LetFieldMustBeOutputSafe(node.File, node.Syntax, type));
            }
        }
    }

    private static void CheckFieldMaintainsIndependence(IFieldDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = ((IFieldDeclarationNode)node).BindingType;
        // Fields must also maintain the independence of independent type parameters
        if (!type.FieldMaintainsIndependence())
            diagnostics.Add(TypeError.FieldMustMaintainIndependence(node.File, node.Syntax, type));
    }

    public static FunctionType AssociatedFunctionDefinition_Type(IAssociatedFunctionDefinitionNode node)
        => FunctionType(node.Parameters, node.Return);

    // TODO maybe this should be initial flow state?
    public static partial IFlowState ConcreteInvocableDefinition_FlowStateBefore(IConcreteInvocableDefinitionNode node)
        => IFlowState.Empty;

    public static void NamedParameter_ContributeDiagnostics(INamedParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = node.BindingType;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }
}
