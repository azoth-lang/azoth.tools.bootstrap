using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

/// <summary>
/// Provides a common supertype for all instances of an inherited attribute.
/// </summary>
/// <remarks>Also acts as a collection of all instances of the attribute.</remarks>
public sealed class InheritedAttributeFamilyModel : ContextAttributeFamilyModel
{
    public bool IsStable { get; }
    public override string Name { get; }
    public override TypeModel Type => type.Value;
    private readonly Lazy<TypeModel> type;
    public IFixedSet<InheritedAttributeModel> Instances => instances.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeModel>> instances;

    public InheritedAttributeFamilyModel(TreeModel tree, IEnumerable<InheritedAttributeModel> instances)
        : base(tree)
    {
        this.instances = new(instances.ToFixedSet());
        if (Instances.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        IsStable = false;
        var representativeAttribute = Instances.First();
        Name = representativeAttribute.Name;
        if (Instances.Any(a => a.Name != Name))
            throw new ArgumentException("All instances must have the same name.", nameof(instances));
        type = new(ComputeType);
    }

    public InheritedAttributeFamilyModel(TreeModel tree, InheritedAttributeFamilySyntax syntax)
        : base(tree)
    {
        IsStable = syntax.IsStable;
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

        var types = supertypeAttributes.Select(a => a.Type).MostGeneralTypes().ToFixedSet();
        if (types.Count > 1)
            throw new FormatException($"Could not determine base type for inherited attribute {Name}."
                                      + $" Candidates are {string.Join(",", types.Select(t => $"'{t}'"))}");
        return types.Single();
    }

    private IFixedSet<InheritedAttributeModel> ComputeInstances()
        => Tree.Aspects.SelectMany(a => a.DeclaredAttributes).OfType<InheritedAttributeModel>()
               .Where(a => a.Name == Name).ToFixedSet();

    public override string ToString()
    {
        var stable = IsStable ? "stable " : "";
        return $"↓ {stable}*.{Name} <: {Type}";
    }
}
