using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
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
    public ObjectType EvaluateMethodSelfParameterType(
        DeclaredObjectType objectType,
        IReferenceCapabilitySyntax capability,
        FixedList<DataType> typeArguments) =>
        objectType.With(capability.Declared.ToReferenceCapability(), typeArguments);

    public ObjectType EvaluateConstructorSelfParameterType(
        DeclaredObjectType objectType,
        IReferenceCapabilitySyntax capability,
        FixedList<DataType> typeArguments)
    {
        ReferenceCapability referenceCapability;
        switch (capability.Declared)
        {
            case DeclaredReferenceCapability.ReadOnly:
                referenceCapability = ReferenceCapability.InitReadOnly;
                break;
            case DeclaredReferenceCapability.Mutable:
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Isolated:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capability));
                referenceCapability = ReferenceCapability.InitMutable;
                break;
            case DeclaredReferenceCapability.Constant:
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
