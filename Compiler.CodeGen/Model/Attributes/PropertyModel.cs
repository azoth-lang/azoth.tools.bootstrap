using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for a property attribute. That is an attribute that is defined as a property
/// as part of the node definition.
/// </summary>
public sealed class PropertyModel : AttributeModel
{
    public override PropertySyntax? Syntax { get; }

    public override TreeNodeModel Node { get; }
    public override string Name { get; }
    public override TypeModel Type { get; }

    public PropertyModel(TreeNodeModel node, PropertySyntax syntax)
    {
        Node = node;
        Syntax = syntax;
        Name = Syntax.Name;
        Type = TypeModel.CreateFromSyntax(Node.Tree, syntax.Type);
    }

    public PropertyModel(TreeNodeModel node, string name, TypeModel type)
    {
        Node = node;
        Name = name;
        Type = type;
    }

    public override string ToString() => $"{Node.Defines}.{Name}:{Type}";
}
