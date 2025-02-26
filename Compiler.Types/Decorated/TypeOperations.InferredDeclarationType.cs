using Azoth.Tools.Bootstrap.Compiler.Core.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    public static IMaybeNonVoidType InferredDeclarationType(this IMaybeType self, DeclaredCapability? declaredCapability)
        => self.ToNonVoid() switch
        {
            NonVoidType t => t.InferredDeclarationType(declaredCapability),
            var t => t,
        };

    /// <summary>
    /// Given an initializer of this type, what should the inferred declaration type be?
    /// </summary>
    public static IMaybeNonVoidType InferredDeclarationType(this NonVoidType self, DeclaredCapability? declaredCapability)
    {
        // Declarations are never inferred to have `iref` or `ref` types by default. This is
        // recursive (e.g. `ref var ref var T` infers `T`).
        while (self is RefType refType)
            self = refType.Referent;
        self = self.ToNonLiteral();

        if (declaredCapability is not { } capability)
            return self.WithoutWrite();

        if (self is CapabilityType capabilityType)
            return capabilityType.With(capability);

        return Type.Unknown;
    }
}
