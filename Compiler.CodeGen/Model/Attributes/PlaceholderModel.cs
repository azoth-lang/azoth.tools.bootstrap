using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class PlaceholderModel : TreeAttributeModel
{
    public override PlaceholderSyntax Syntax { get; }
    public override TreeNodeModel Node { get; }
    public override string Name => Syntax.Name;
    public AttributeModel Attribute => attribute.Value;
    private readonly Lazy<AttributeModel> attribute;
    public override TypeModel Type => Attribute.Type;
    public override bool IsTemp => Attribute.IsTemp;
    public override bool IsPlaceholder => true;
    public override bool IsChild => Attribute.IsChild;
    public override bool IsMethod => Attribute.IsMethod;

    public PlaceholderModel(TreeNodeModel node, PlaceholderSyntax syntax)
    {
        Node = node;
        Syntax = syntax;
        attribute = new(ComputeAttribute);
    }

    private AttributeModel ComputeAttribute()
        => Node.AttributesNamed(Name).Where(a => !a.IsPlaceholder).TrySingle()
           ?? throw new FormatException($"Placeholder '{this}' has no corresponding attribute.");

    public override string ToString() => $"{Node.Defines}./{Name}/";
}
