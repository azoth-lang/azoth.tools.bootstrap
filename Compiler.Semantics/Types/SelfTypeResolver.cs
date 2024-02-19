using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public class SelfTypeResolver
{
    private readonly CodeFile file;
    private readonly Diagnostics diagnostics;
    public SelfTypeResolver(CodeFile file, Diagnostics diagnostics)
    {
        this.file = file;
        this.diagnostics = diagnostics;
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public Pseudotype EvaluateMethodSelfParameterType(
        ObjectType objectType,
        ISelfReferenceCapabilitySyntax capability,
        IFixedList<DataType> typeArguments)
    {
        return capability switch
        {
            IReferenceCapabilitySyntax syn => objectType.With(syn.Declared.ToReferenceCapability(), typeArguments),
            IReferenceCapabilityConstraintSyntax syn => objectType.With(syn.Constraint, typeArguments),
            _ => throw ExhaustiveMatch.Failed(capability)
        };
    }

    public ReferenceType EvaluateConstructorSelfParameterType(
        ObjectType objectType,
        IReferenceCapabilitySyntax capability,
        IFixedList<DataType> typeArguments)
    {
        ReferenceCapability referenceCapability;
        switch (capability.Declared)
        {
            case DeclaredReferenceCapability.Read:
                referenceCapability = ReferenceCapability.InitReadOnly;
                break;
            case DeclaredReferenceCapability.Mutable:
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Isolated:
            case DeclaredReferenceCapability.TemporarilyIsolated:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capability));
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Constant:
            case DeclaredReferenceCapability.TemporarilyConstant:
            case DeclaredReferenceCapability.Identity:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capability));
                referenceCapability = ReferenceCapability.InitReadOnly;
                break;
            default:
                throw ExhaustiveMatch.Failed(capability.Declared);
        }

        return objectType.With(referenceCapability, typeArguments);
    }
}
