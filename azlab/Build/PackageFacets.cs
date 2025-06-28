using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics;

namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class PackageFacets
{
    public IPackageFacetNode MainFacet { get; }
    public IPackageFacetNode TestsFacet { get; }
    public DiagnosticCollection Diagnostics { get; }

    public PackageFacets(IPackageFacetNode mainFacet, IPackageFacetNode testsFacet)
    {
        MainFacet = mainFacet;
        TestsFacet = testsFacet;
        Diagnostics = new DiagnosticCollectionBuilder() { mainFacet.Diagnostics, testsFacet.Diagnostics }
            .Build();
    }
}
