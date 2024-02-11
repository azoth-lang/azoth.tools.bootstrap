using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

internal static class ReferenceTypeExtensions
{
    public static ReferenceType ArgumentAt(this ReferenceType type, TypeArgumentIndex index)
        => index.TreeIndex.Aggregate(type, (current1, i) => (ReferenceType)current1.TypeArguments[i]);
}
