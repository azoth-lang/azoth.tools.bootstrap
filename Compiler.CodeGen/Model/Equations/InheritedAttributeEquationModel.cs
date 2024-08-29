using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationModel : EquationModel
{
    public override InheritedAttributeEquationSyntax Syntax { get; }
    public InheritedAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<InheritedAttributeFamilyModel> attributeFamily;
    /// <remarks>Inherited attribute equations may not refer to a single attribute because they can
    /// broadcast to multiple attributes.</remarks>
    public override InheritedAttributeModel? Attribute => InheritedToAttributes.TrySingle();
    public SelectorModel Selector { get; }
    public bool IsAllDescendants => Selector.IsAllDescendants;
    public override TypeModel Type => InheritedToTypes.TrySingle() ?? AttributeFamily.Type;
    public IFixedSet<InheritedAttributeModel> InheritedToAttributes => inheritedToAttributes.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeModel>> inheritedToAttributes;
    /// <summary>
    /// The most specific types that this attribute will need to satisfy.
    /// </summary>
    /// <remarks>This is derived from <see cref="InheritedToAttributes"/>. If two attributes have
    /// types <c>T1</c> and <c>T2</c> where <c>T1 &lt;: T2</c> then only <c>T1</c> will be in the
    /// types that must be inherited to.</remarks>
    public IFixedSet<TypeModel> InheritedToTypes => inheritedToTypes.Value;
    private readonly Lazy<IFixedSet<TypeModel>> inheritedToTypes;

    public InheritedAttributeEquationModel(AspectModel aspect, InheritedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        Selector = SelectorModel.Create(syntax.Selector);
        attributeFamily = new(ComputeAttributeFamily);
        inheritedToAttributes = new(ComputeInheritedToAttributes);
        inheritedToTypes = new(ComputeInheritedToTypes);
    }

    private InheritedAttributeFamilyModel ComputeAttributeFamily()
        => Aspect.Tree.AllAttributeFamilies.OfType<InheritedAttributeFamilyModel>()
                 .Single(a => a.Name == Name);

    private IFixedSet<InheritedAttributeModel> ComputeInheritedToAttributes()
    {
        var selectedAttributes = Selector.SelectNodes(Node).SelectMany(n => n.ActualAttributes).ToFixedSet();
        return AttributeFamily.Instances.Where(a => selectedAttributes.Contains(a)).ToFixedSet();
    }

    private IFixedSet<TypeModel> ComputeInheritedToTypes()
        => InheritedToAttributes.Select(a => a.Type).MostSpecificTypes().ToFixedSet();

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Selector}.{Name}{parameters}";
    }
}
