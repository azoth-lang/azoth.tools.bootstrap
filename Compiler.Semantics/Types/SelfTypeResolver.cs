using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
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

    public ValueType EvaluateInitializerSelfParameterType(
        StructType structType,
        ICapabilitySyntax syntax,
        IFixedList<DataType> typeArguments)
    {
        Capability capability;
        switch (syntax.Declared)
        {
            case DeclaredCapability.Read:
                capability = Capability.InitReadOnly;
                break;
            case DeclaredCapability.Mutable:
                capability = Capability.InitMutable;
                break;
            case DeclaredCapability.Isolated:
            case DeclaredCapability.TemporarilyIsolated:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, syntax));
                capability = Capability.InitMutable;
                break;
            case DeclaredCapability.Constant:
            case DeclaredCapability.TemporarilyConstant:
            case DeclaredCapability.Identity:
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, syntax));
                capability = Capability.InitReadOnly;
                break;
            default:
                throw ExhaustiveMatch.Failed(syntax.Declared);
        }

        return structType.With(capability, typeArguments);
    }
}
