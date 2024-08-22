using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeSupertypes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationModel : EquationModel
{
    public override InheritedAttributeEquationSyntax Syntax { get; }
    public InheritedAttributeSupertypeModel AttributeSupertype => attributeSupertype.Value;
    private readonly Lazy<InheritedAttributeSupertypeModel> attributeSupertype;
    public SelectorModel Selector { get; }
    public bool IsAllDescendants => Selector.IsAllDescendants;
    public override TypeModel Type => AttributeSupertype.Type;
    public IFixedSet<InheritedAttributeModel> InheritedToAttributes => inheritedToAttributes.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeModel>> inheritedToAttributes;

    public InheritedAttributeEquationModel(AspectModel aspect, InheritedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        Selector = SelectorModel.Create(syntax.Selector);
        attributeSupertype = new(ComputeAttributeSupertype);
        inheritedToAttributes = new(ComputeInheritedToAttributes);
    }

    private InheritedAttributeSupertypeModel ComputeAttributeSupertype()
        => Aspect.Tree.AllAttributeSupertypes.OfType<InheritedAttributeSupertypeModel>()
                 .Single(a => a.Name == Name);

    private IFixedSet<InheritedAttributeModel> ComputeInheritedToAttributes()
    {
        var selectedAttributes = Selector.SelectNodes(Node).SelectMany(n => n.ActualAttributes).ToFixedSet();
        return AttributeSupertype.Instances.Where(a => selectedAttributes.Contains(a)).ToFixedSet();
    }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Selector}.{Name}{parameters}";
    }
}
