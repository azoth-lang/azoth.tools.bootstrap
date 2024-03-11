using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

internal static class CapabilityTypeExtensions
{
    public static CapabilityType ArgumentAt(this CapabilityType type, TypeArgumentIndex index)
        => index.TreeIndex.Aggregate(type, (current1, i) => (CapabilityType)current1.TypeArguments[i]);
}
