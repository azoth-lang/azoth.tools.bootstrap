using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

/// <summary>
/// A group of all the inherited attribute equations for the same attribute on the same node.
/// </summary>
public sealed class InheritedAttributeEquationGroupModel
{
    public TreeNodeModel Node { get; }
    public string Name { get; }
    public TypeModel Type => Equations[0].AttributeFamily.Type;
    public IFixedList<InheritedAttributeEquationModel> Equations { get; }
    public bool IsAllDescendants => Equations.Any(a => a.IsAllDescendants);

    public InheritedAttributeEquationGroupModel(
        TreeNodeModel node,
        IEnumerable<InheritedAttributeEquationModel> instances)
    {
        Node = node;
        Equations = instances.ToFixedList();
        if (Equations.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        var representativeAttribute = Equations[0];
        Name = representativeAttribute.Name;
        if (Equations.Any(a => a.Name != Name))
            throw new ArgumentException("All instances must have the same name.", nameof(instances));
    }
}
