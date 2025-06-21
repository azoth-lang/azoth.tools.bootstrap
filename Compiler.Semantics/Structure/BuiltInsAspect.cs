using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class BuiltInsAspect
{
    // If there is already a reference to the intrinsics package, use it. Otherwise, create a new one.
    public static partial IPackageFacetReferenceNode PackageFacet_IntrinsicsReference(IPackageFacetNode node)
        => node.References.SingleOrDefault(r => Intrinsic.Package.Equals(r.Symbols.Package))
            ?? IIntrinsicsPackageFacetReferenceNode.Create();

    public static partial IFixedSet<ITypeDeclarationNode> Package_PrimitivesDeclarations(IPackageNode node)
        => Primitive.SymbolTree.GlobalSymbols.Select(SymbolBinder.Symbol)
                    .WhereNotNull().Cast<ITypeDeclarationNode>().ToFixedSet();
}
