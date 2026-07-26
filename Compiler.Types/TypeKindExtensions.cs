using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

internal static class TypeKindExtensions
{
    public static TypeSemantics ToSemantics(this TypeKind kind)
        => kind switch
        {
            TypeKind.Class => TypeSemantics.Reference,
            TypeKind.Trait => TypeSemantics.Reference,
            TypeKind.AffineStruct => TypeSemantics.Hybrid,
            TypeKind.Struct => TypeSemantics.Value,
            _ => throw ExhaustiveMatch.Failed(kind)
        };
}
