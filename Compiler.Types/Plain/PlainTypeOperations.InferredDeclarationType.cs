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
        => self.ToNonLiteral();
}
