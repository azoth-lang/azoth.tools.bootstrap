using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class BuiltInsAspect
{
    // If there is already a reference to the intrinsics package, use it. Otherwise, create a new one.
    public static IPackageReferenceNode Package_IntrinsicsReference(IPackageNode node)
        => node.References.SingleOrDefault(r => Intrinsic.Package.Equals(r.PackageSymbols.PackageSymbol))
            // TODO this is an NTA. There should be a better pattern for handling attachment.
            ?? Child.Attach(node, new IntrinsicsPackageReferenceNode());

    public static IFixedSet<ITypeDeclarationNode> Package_PrimitivesDeclarations(IPackageNode node)
        // TODO this is an NTA. There should be a better pattern for handling attachment.
        => ChildSet.Attach(node, Primitive.SymbolTree.GlobalSymbols.Select(SymbolNodeAspect.Symbol)
                                          .WhereNotNull().Cast<ITypeDeclarationNode>());
}