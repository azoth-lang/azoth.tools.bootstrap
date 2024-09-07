using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

public sealed class CollectionAttributeFamilyModel : AttributeFamilyModel
{
    public IFixedSet<CollectionAttributeModel> Instances { get; }
    public override string Name { get; }
    public override TypeModel Type { get; }
    public TypeModel FromType { get; }
    public bool HasRoot { get; }
    public string ConstructExpression { get; }
    public string DoneMethod { get; }

    public CollectionAttributeFamilyModel(TreeModel tree, IEnumerable<CollectionAttributeModel> instances)
        : base(tree)
    {
        Instances = instances.ToFixedSet();
        if (Instances.IsEmpty)
            throw new ArgumentException("At least one instance is required.", nameof(instances));
        var representativeAttribute = Instances.First();
        Name = representativeAttribute.Name;
        if (Instances.Any(a => a.Name != Name))
            throw new ArgumentException("All instances must have the same name.", nameof(instances));
        Type = representativeAttribute.Type;
        if (Instances.Any(a => a.Type != Type))
            throw new ArgumentException("All instances must have the same type.", nameof(instances));
        FromType = representativeAttribute.FromType;
        if (Instances.Any(a => a.FromType != FromType))
            throw new ArgumentException("All instances must have the same from type.", nameof(instances));
        HasRoot = Instances.Any(a => a.RootSymbol is not null);
        ConstructExpression = representativeAttribute.ConstructExpression;
        if (Instances.Any(a => a.ConstructExpression != ConstructExpression))
            throw new ArgumentException("All instances must have the same construct expression.", nameof(instances));
        DoneMethod = representativeAttribute.DoneMethod;
        if (Instances.Any(a => a.DoneMethod != DoneMethod))
            throw new ArgumentException("All instances must have the same done method.", nameof(instances));
    }

    public override string ToString() => $"→*← *.{Name}: {Type} from {FromType} => {ConstructExpression} done {DoneMethod}";
}
