using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class ChildPlaceholderModel : TreeAttributeModel
{
    public override ChildPlaceholderSyntax Syntax { get; }
    public override TreeNodeModel Node { get; }
    public override string Name => Syntax.Name;
    public AspectAttributeModel Attribute => attribute.Value;
    private readonly Lazy<AspectAttributeModel> attribute;
    public override TypeModel Type => Attribute.Type;
    public override bool IsTemp => Attribute.IsTemp;
    public override bool IsPlaceholder => true;
    public override bool IsChild => Attribute.IsChild;
    public override bool IsMethod => Attribute.IsMethod;

    public ChildPlaceholderModel(TreeNodeModel node, ChildPlaceholderSyntax syntax)
    {
        Node = node;
        Syntax = syntax;
        attribute = new(ComputeAttribute);
    }

    private AspectAttributeModel ComputeAttribute()
        => Node.AttributesNamed(Name).OfType<AspectAttributeModel>().TrySingle()
           ?? throw new FormatException($"Child placeholder '{this}' has no corresponding attribute.");

    public override string ToString() => $"{Node.Defines}./{Name}/";
}
