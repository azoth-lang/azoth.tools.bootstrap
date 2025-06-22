using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class BuiltInsAspect
{
    #region Facets
    public static partial IFixedSet<ITypeDeclarationNode> PackageFacet_PrimitivesDeclarations(IPackageFacetNode node)
        => Primitive.SymbolTree.GlobalSymbols.Select(SymbolBinder.Symbol).WhereNotNull().Cast<ITypeDeclarationNode>()
                    .ToFixedSet();

    // TODO shouldn't the intrinsics package be more distinct and not something that could be already referenced?
    // If there is already a reference to the intrinsics package, use it. Otherwise, create a new one.
    public static partial IPackageFacetReferenceNode PackageFacet_IntrinsicsReference(IPackageFacetNode node)
        => node.References.SingleOrDefault(r => Intrinsic.Package.Equals(r.Symbols.Package))
           ?? IIntrinsicsPackageFacetReferenceNode.Create();
    #endregion
}
