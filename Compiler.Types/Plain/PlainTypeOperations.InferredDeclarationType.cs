namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public static partial class PlainTypeOperations
{
    public static IMaybeNonVoidPlainType InferredDeclarationType(this IMaybePlainType self)
        => self.ToNonVoid() switch
        {
            NonVoidPlainType t => t.InferredDeclarationType(),
            var t => t
        };

    /// <summary>
    /// Given an initializer of this type, what should the inferred declaration type be?
    /// </summary>
    public static NonVoidPlainType InferredDeclarationType(this NonVoidPlainType self)
    {
        // Declarations are never inferred to have `iref` or `ref` types by default. This is
        // recursive (e.g. `ref var ref var T` infers `T`).
        while (self is RefPlainType refType)
            self = refType.Referent;
        return self.ToNonLiteral();
    }
}
