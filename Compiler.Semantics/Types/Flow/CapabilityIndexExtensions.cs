using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal static class CapabilityIndexExtensions
{
    // TODO this does not properly handle optional types and should probably be replaced with TypeAt
    public static CapabilityType ArgumentAt(this CapabilityType type, CapabilityIndex index)
        => index.TreeIndex.Aggregate(type, (currentType, i) => (CapabilityType)currentType.TypeArguments[i]);

    /// <summary>
    /// Get the type at this index in the given type.
    /// </summary>
    /// <remarks>The given type must be either the type that the index was created on or the basis
    /// for that type. For example, the index could be from a type constructed by substituting type
    /// parameters into the given type.</remarks>
    public static IMaybeType TypeAt(this IMaybeType type, CapabilityIndex index)
        => type.TypeAt(index, 0);

    private static IMaybeType TypeAt(this IMaybeType type, CapabilityIndex index, int depth)
        => type switch
        {
            OptionalType t => t.Referent.TypeAt(index, depth),
            _ when depth == index.TreeIndex.Count => type,
            CapabilityType t => t.TypeArguments[index.TreeIndex[depth]].TypeAt(index, depth + 1),
            _ => throw new InvalidOperationException("The index is invalid for the given type.")
        };
}
