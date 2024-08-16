using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> OrderedNamespaces(IHasUsingNamespaces model, params string[] additionalNamespaces)
        => model.UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<PropertyModel> BaseProperties(TreeNodeModel node, PropertyModel property)
        => node.InheritedPropertiesNamedSameAs(property).Where(p => p.IsDeclarationRequired);
}
