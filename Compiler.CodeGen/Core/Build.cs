using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> Conditional(bool condition, params string[] namespaces)
        => condition ? namespaces : [];

    public static IEnumerable<string> OrderedNamespaces(IHasUsingNamespaces model, params string[] additionalNamespaces)
        => model.UsingNamespaces.Concat(additionalNamespaces).Distinct()
                .OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<string> OrderedNamespaces(
        IHasUsingNamespaces model,
        params IEnumerable<string>[] additionalNamespaces)
        => model.UsingNamespaces.Concat(additionalNamespaces.SelectMany())
                .Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<AttributeModel> BaseAttributes(TreeNodeModel node, AttributeModel attribute)
        // One would think that this could be only inherited attributes. However, because of the
        // stupid way default interface implementations work where they don't actually override but
        // instead act like an implementation that is copied down to the class, there are rules for
        // always having a most specific implementation. There are cases where there will not be a
        // most specific implementation, even though if it were overriding it would be fine. To
        // avoid these cases, explicitly implement ALL inherited attributes in every interface so
        // that each interface has a most specific implementation for every attribute. This also
        // avoids cases where multiple forwards through interface methods would be necessary.
        // Example:
        //     interface IA { int A { get; } }
        //     interface IB : IA { new int A { get; } int IA.A => A; }
        //     interface IC : IA { new int A { get; } int IA.A => A; }
        //     interface ID : IB, IC { new int A { get; } int IB.A => A; int IC.A => A; }
        // Here on ID, the IA.A property does not have a most specific implementation. The two
        // explicit implementations in IB and IC are at the same level of specificity.
        => node.AllInheritedAttributesNamedSameAs(attribute).Where(p => p.IsDeclarationRequired);
}
