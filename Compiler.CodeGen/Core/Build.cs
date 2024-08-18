using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> OrderedNamespaces(IHasUsingNamespaces model, params string?[] additionalNamespaces)
        => model.UsingNamespaces.Concat(additionalNamespaces).WhereNotNull().Distinct()
                .OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<AttributeModel> BaseAttributes(TreeNodeModel node, AttributeModel attribute)
        => node.InheritedAttributesNamedSameAs(attribute).Where(p => p.IsDeclarationRequired);
}
