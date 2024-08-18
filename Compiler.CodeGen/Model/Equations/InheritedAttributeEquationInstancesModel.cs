using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationInstancesModel
{
    public string Name { get; }
    public TypeModel Type => Instances[0].Type;
    public IFixedList<InheritedAttributeEquationModel> Instances { get; }

    public InheritedAttributeEquationInstancesModel(IEnumerable<InheritedAttributeEquationModel> instances)
    {
        Instances = instances.ToFixedList();
        if (Instances.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        var representativeAttribute = Instances[0];
        Name = representativeAttribute.Name;
        if (Instances.Any(a => a.Name != Name))
            throw new ArgumentException("All instances must have the same name.", nameof(instances));
    }
}
