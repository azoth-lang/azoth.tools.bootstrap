using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using ValueType = Azoth.Tools.Bootstrap.Compiler.Types.ValueType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class TypeMemberDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDefinitionNode node)
        => FunctionType(node.Parameters, node.Return);

    private static FunctionType FunctionType(IEnumerable<INamedParameterNode> parameters, ITypeNode? @return)
    {
        var parameterTypes = parameters.Select(p => p.ParameterType).ToFixedList();
        var returnType = @return?.Type ?? DataType.Void;
        return new FunctionType(parameterTypes, new Return(returnType));
    }

    public static void MethodDeclaration_ContributeDiagnostics(IMethodDefinitionNode node, Diagnostics diagnostics)
        => CheckParameterAndReturnAreVarianceSafe(node, diagnostics);

    private static void CheckParameterAndReturnAreVarianceSafe(IMethodDefinitionNode node, Diagnostics diagnostics)
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
            var type = parameter.Type;
            if (!type.IsInputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.ParameterMustBeInputSafe(node.File, parameter.Syntax, type));
        }

        var returnType = methodSymbol.Return.Type;
        if (!returnType.IsOutputSafe(nonwritableSelf))
            diagnostics.Add(TypeError.ReturnTypeMustBeOutputSafe(node.File, node.Return!.Syntax, returnType));
    }

    public static ValueId Parameter_ValueId(IParameterNode node)
        => node.PreviousValueId().CreateNext();

    public static Parameter NamedParameter_ParameterType(INamedParameterNode node)
        => new(node.IsLentBinding, node.Type);

    public static DataType NamedParameterNode_Type(INamedParameterNode node)
        => node.TypeNode.Type;

    public static Pseudotype MethodSelfParameter_Type(IMethodSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var genericParameterTypes = declaredType.GenericParameterTypes;
        var capability = node.Capability;
        return capability switch
        {
            ICapabilityNode n => declaredType.With(n.Capability, genericParameterTypes),
            ICapabilitySetNode n => declaredType.With(n.Constraint, genericParameterTypes),
            _ => throw ExhaustiveMatch.Failed(capability)
        };
    }

    public static SelfParameter MethodSelfParameter_ParameterType(IMethodSelfParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.Type.CanBeLent();
        return new SelfParameter(isLent, node.Type);
    }

    public static void MethodSelfParameter_ContributeDiagnostics(IMethodSelfParameterNode node, Diagnostics diagnostics)
    {
        CheckTypeCannotBeLent(node, diagnostics);

        CheckConstClassSelfParameterCannotHaveCapability(node, diagnostics);
    }

    private static void CheckTypeCannotBeLent(IMethodSelfParameterNode node, Diagnostics diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = node.Type;
        if (isLent && !selfType.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }

    private static void CheckConstClassSelfParameterCannotHaveCapability(
        IMethodSelfParameterNode node,
        Diagnostics diagnostics)
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

    public static ReferenceType ConstructorSelfParameter_Type(IConstructorSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void ConstructorSelfParameter_ContributeDiagnostics(
        IConstructorSelfParameterNode node,
        Diagnostics diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    internal static DataType FieldParameter_Type(IFieldParameterNode node)
        => node.ReferencedField?.Type ?? DataType.Unknown;

    public static Parameter FieldParameter_ParameterType(IFieldParameterNode node)
        => new Parameter(false, node.Type);

    public static ValueType InitializerSelfParameter_Type(InitializerSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void InitializerSelfParameter_ContributeDiagnostics(
        IInitializerSelfParameterNode node,
        Diagnostics diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    private static void CheckInvalidConstructorSelfParameterCapability(
        ICapabilitySyntax capabilitySyntax,
        CodeFile file,
        Diagnostics diagnostics)
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

    public static DataType FieldDeclaration_Type(IFieldDefinitionNode node) => node.TypeNode.Type;

    public static void FieldDeclaration_ContributeDiagnostics(IFieldDefinitionNode node, Diagnostics diagnostics)
    {
        CheckFieldIsVarianceSafe(node, diagnostics);

        CheckFieldMaintainsIndependence(node, diagnostics);
    }

    private static void CheckFieldIsVarianceSafe(IFieldDefinitionNode node, Diagnostics diagnostics)
    {
        var type = node.Type;

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

    private static void CheckFieldMaintainsIndependence(IFieldDefinitionNode node, Diagnostics diagnostics)
    {
        var type = node.Type;
        // Fields must also maintain the independence of independent type parameters
        if (!type.FieldMaintainsIndependence())
            diagnostics.Add(TypeError.FieldMustMaintainIndependence(node.File, node.Syntax, type));
    }

    public static FunctionType AssociatedFunctionDeclaration_Type(IAssociatedFunctionDefinitionNode node)
        => FunctionType(node.Parameters, node.Return);

    public static FlowState ConcreteInvocable_FlowStateBefore(IConcreteInvocableDefinitionNode node)
        => FlowState.Empty;

    public static ValueIdScope Invocable_ValueIdScope(IInvocableDefinitionNode _)
        => new ValueIdScope();

    public static IPreviousValueId Invocable_PreviousValueId(IInvocableDefinitionNode node)
        => new BeforeFirstValueId(node.ValueIdScope);

    public static ValueIdScope FieldDefinition_ValueIdScope(IFieldDefinitionNode _)
        => new ValueIdScope();
}
