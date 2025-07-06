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

    // TODO the intrinsics package is going away and being replaced by uses of `#Intrinsic`
    public static partial IIntrinsicsPackageFacetReferenceNode PackageFacet_IntrinsicsReference(IPackageFacetNode node)
        => IIntrinsicsPackageFacetReferenceNode.Create();
    #endregion
}
