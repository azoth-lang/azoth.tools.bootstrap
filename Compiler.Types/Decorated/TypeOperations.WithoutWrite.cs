using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// The given type but without any write capabilities.
    /// </summary>
    /// <remarks>When inferring the type of a variable declaration, the inferred type should not
    /// have any write capabilities. This method is used to enforce that.</remarks>
    public static IMaybeType WithoutWrite(this IMaybeType self)
        => self switch
        {
            NonVoidType t => t.WithoutWrite(),
            VoidType t => t,
            UnknownType t => t,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    /// <summary>
    /// The given type but without any write capabilities.
    /// </summary>
    /// <remarks>When inferring the type of a variable declaration, the inferred type should not
    /// have any write capabilities. This method is used to enforce that.</remarks>
    public static NonVoidType WithoutWrite(this NonVoidType self)
        => self switch
        {
            CapabilityType t => t.WithoutWrite(),
            // TODO is this correct? Should the viewpoint be constrained?
            CapabilitySetSelfType t => t,
            // TODO is this correct? Should the viewpoint be constrained?
            CapabilitySetRestrictedType t => t,
            // TODO is this correct? Should the viewpoint be constrained?
            CapabilityViewpointType t => t,
            SelfViewpointType t => t,
            GenericParameterType t => t,
            OptionalType t => t,
            FunctionType t => t,
            NeverType t => t,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    /// <summary>
    /// The given type but without any write capabilities.
    /// </summary>
    /// <remarks>When inferring the type of a variable declaration, the inferred type should not
    /// have any write capabilities. This method is used to enforce that.</remarks>
    public static CapabilityType WithoutWrite(this CapabilityType self)
        => self.With(self.Capability.WithoutWrite());
}
