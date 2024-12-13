using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Create a type with the upper bound of the capability constraint.
    /// </summary>
    public static IMaybeType ToUpperBound(this IMaybeType self)
        => self switch
        {
            IType t => t.ToUpperBound(),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    public static IMaybeNonVoidType ToUpperBound(this IMaybeNonVoidType self)
        => self switch
        {
            NonVoidType t => t.ToUpperBound(),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    public static IType ToUpperBound(this IType self)
        => self switch
        {
            NonVoidType t => t.ToUpperBound(),
            VoidType _ => IType.Void,
            _ => throw ExhaustiveMatch.Failed(self),
        };

    public static NonVoidType ToUpperBound(this NonVoidType self)
        => self switch
        {
            CapabilitySetSelfType t => CapabilityType.Create(t.Capability.UpperBound, t.PlainType),
            // TODO shouldn't these be recursive on referents and arguments?
            OptionalType t => t,
            GenericParameterType t => t,
            CapabilityViewpointType t => t,
            SelfViewpointType t => t,
            FunctionType t => t,
            CapabilityType t => t,

            NeverType _ => IType.Never,
            _ => throw ExhaustiveMatch.Failed(self),
        };
}
