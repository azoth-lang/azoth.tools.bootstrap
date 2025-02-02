using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static IMaybeType WithoutWrite(this IMaybeType self)
        => self switch
        {
            NonVoidType t => t.WithoutWrite(),
            VoidType t => t,
            UnknownType t => t,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    public static NonVoidType WithoutWrite(this NonVoidType self)
        => self switch
        {
            CapabilityType t => t.WithoutWrite(),
            CapabilitySetSelfType t => t,
            CapabilityViewpointType t => t,
            SelfViewpointType t => t,
            GenericParameterType t => t,
            OptionalType t => t,
            RefType t => t,
            FunctionType t => t,
            NeverType t => t,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    public static CapabilityType WithoutWrite(this CapabilityType self)
        => self.With(self.Capability.WithoutWrite());
}
