using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

internal record class PackageFacetReferenceSyntax(IPackageReferenceSyntax PackageReference, FacetKind Facet);
