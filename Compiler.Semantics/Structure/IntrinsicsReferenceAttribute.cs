using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class IntrinsicsReferenceAttribute
{
    // If there is already a reference to the intrinsics package, use it. Otherwise, create a new one.
    public static IPackageReferenceNode Package(IPackageNode node)
        => node.References.SingleOrDefault(r => Intrinsic.Package.Equals(r.PackageSymbols.PackageSymbol))
            // TODO this is an NTA. There should be a better pattern for handling attachment.
            ?? Child.Attach(node, new IntrinsicsPackageReferenceNode());
}
