using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;

/// <summary>
/// The supertype for an inherited attribute. Also acts as a collection of all instances of the attribute.
/// </summary>
public sealed class InheritedAttributeKinModel : ContextAttributeKinModel
{
    public override string Name { get; }
    public override TypeModel Type => type.Value;
    private readonly Lazy<TypeModel> type;
    public IFixedSet<InheritedAttributeModel> Instances => instances.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeModel>> instances;

    public InheritedAttributeKinModel(TreeModel tree, IEnumerable<InheritedAttributeModel> instances)
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

    public InheritedAttributeKinModel(TreeModel tree, InheritedAttributeSupertypeSyntax syntax)
        : base(tree)
    {
        Name = syntax.Name;
        type = new(TypeModel.CreateFromSyntax(tree, syntax.Type));
        instances = new(ComputeInstances);
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

    private IFixedSet<InheritedAttributeModel> ComputeInstances()
        => Tree.Aspects.SelectMany(a => a.Attributes).OfType<InheritedAttributeModel>()
               .Where(a => a.Name == Name).ToFixedSet();

    public override string ToString() => $"↓ *.{Name} <: {Type}";
}
