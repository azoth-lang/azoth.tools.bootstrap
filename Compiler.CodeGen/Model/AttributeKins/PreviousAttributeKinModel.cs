using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

public sealed class PreviousAttributeKinModel : ContextAttributeKinModel
{
    public override string Name { get; }
    public override TypeModel Type => type.Value;
    private readonly Lazy<TypeModel> type;
    public IFixedSet<PreviousAttributeModel> Instances => instances.Value;
    private readonly Lazy<IFixedSet<PreviousAttributeModel>> instances;

    public PreviousAttributeKinModel(TreeModel tree, IEnumerable<PreviousAttributeModel> instances)
        : base(tree)
    {
        this.instances = new(instances.ToFixedSet());
        if (Instances.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        var representativeAttribute = Instances.First();
        Name = representativeAttribute.Name;
        if (Instances.Any(a => a.Name != Name))
            throw new ArgumentException("All instances must have the same name.", nameof(instances));
        type = new(ComputeType);
    }

    private TypeModel ComputeType()
    {
        var supertypeAttributes = Instances.ToHashSet();
        foreach (var attribute in Instances)
            if (attribute.Node.InheritedAttributes.Any(a => a.Name == Name))
                supertypeAttributes.Remove(attribute);

        var types = supertypeAttributes.Select(a => a.Type).ToFixedSet();
        if (types.Count > 1)
            throw new FormatException($"Could not determine base type for inherited attribute {Name}."
                                      + $" Candidates are {string.Join(",", types.Select(t => $"'{t}'"))}");
        return types.Single();
    }

    public override string ToString() => $"⮡ *.{Name} <: {Type}";
}
