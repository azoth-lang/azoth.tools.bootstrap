using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationModel : EquationModel
{
    public override InheritedAttributeEquationSyntax Syntax { get; }
    public InheritedAttributeSupertypeModel AttributeSupertype => attributeSupertype.Value;
    private readonly Lazy<InheritedAttributeSupertypeModel> attributeSupertype;
    public SelectorModel Selector { get; }
    public bool IsAllDescendants => Selector.IsAllDescendants;
    public bool IsMethod => Syntax.IsMethod;
    public TypeModel Type => AttributeSupertype.Type;

    public InheritedAttributeEquationModel(AspectModel aspect, InheritedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.Expression)
    {
        Syntax = syntax;
        Selector = SelectorModel.Create(syntax.Selector);
        attributeSupertype = new(GetAttributeSupertype);
    }

    private InheritedAttributeSupertypeModel GetAttributeSupertype()
        => Aspect.Tree.AllAttributeSupertypes.Single(a => a.Name == Name);

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Name}{parameters}";
    }
}
