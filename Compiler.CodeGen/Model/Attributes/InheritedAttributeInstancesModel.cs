using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// A collection of all the inherited attributes with the same name.
/// </summary>
public sealed class InheritedAttributeInstancesModel
{
    public string Name { get; }
    public TypeModel Type { get; }
    public IFixedSet<InheritedAttributeModel> Instances { get; }

    public InheritedAttributeInstancesModel(IEnumerable<InheritedAttributeModel> instances)
    {
        Instances = instances.ToFixedSet();
        if (Instances.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        var representativeAttribute = Instances.First();
        Name = representativeAttribute.Name;
        Type = representativeAttribute.Type;
    }
}
