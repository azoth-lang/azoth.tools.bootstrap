using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

// TODO shouldn't this be renamed to TypeMemberDefinitionsAspect?
internal static partial class TypeMemberDeclarationsAspect
{
    // TODO maybe this should be moved to definition types aspect?
    public static partial IMaybeFunctionType ConcreteFunctionInvocableDefinition_Type(IConcreteFunctionInvocableDefinitionNode node)
        => FunctionType.Create(node.ParameterTypes, node.ReturnType);

    public static partial void MethodDefinition_Contribute_Diagnostics(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO do generic methods and functions need to be checked?

        // Only methods declared in generic types need checked
        // TODO what about nesting inside of a generic type?
        if (!node.ContainingDeclaration.GenericParameters.Any()) return;

        var nonwritableSelf = !node.SelfParameter.Capability.Constraint.AnyCapabilityAllowsWrite;

        // The `self` parameter does not get checked for variance safety. It will always operate on
        // the original type so it is safe.
        foreach (var parameter in node.Parameters)
        {
            var type = parameter.BindingType;
            if (!type.IsInputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.ParameterMustBeInputSafe(node.File, parameter.Syntax, (IMaybeType)type));
        }

        var returnType = node.ReturnType;
        if (!returnType.IsOutputSafe(nonwritableSelf))
            diagnostics.Add(TypeError.ReturnTypeMustBeOutputSafe(node.File, node.Return!.Syntax, (IMaybeType)returnType));
    }

    public static partial void FieldDefinition_Contribute_Diagnostics(IFieldDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckFieldIsVarianceSafe(node, diagnostics);

        CheckFieldMaintainsIndependence(node, diagnostics);
    }

    // TODO maybe this should be initial flow state?
    public static partial IFlowState ConcreteInvocableDefinition_FlowStateBefore(IConcreteInvocableDefinitionNode node)
        => IFlowState.Empty;

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
                    diagnostics.Add(TypeError.VarFieldMustBeInputAndOutputSafe(node.File, node.Syntax, (IMaybeType)type));
            }
            else
            {
                // Immutable bindings can only be read, so they must be output safe.
                if (!type.IsOutputSafe(nonwritableSelf: true))
                    diagnostics.Add(TypeError.LetFieldMustBeOutputSafe(node.File, node.Syntax, (IMaybeType)type));
            }
        }
    }

    private static void CheckFieldMaintainsIndependence(IFieldDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = ((IFieldDeclarationNode)node).BindingType;
        // Fields must also maintain the independence of independent type parameters
        if (!type.FieldMaintainsIndependence())
            diagnostics.Add(TypeError.FieldMustMaintainIndependence(node.File, node.Syntax, (IMaybeType)type));
    }
}
