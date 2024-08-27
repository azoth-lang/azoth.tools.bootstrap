using System;
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
    /// <summary>
    /// The name of the temporary property.
    /// </summary>
    /// <remarks>For properties that have non-temporary types, this is just the name.</remarks>
    public override string TempName => IsTemp ? $"Temp{Name}" : Name;
    public override bool IsMethod => false;
    public override TypeModel Type { get; }
    public override bool IsChild => ReferencesNode;
    public bool IsTemp => Type.ReferencedNode()?.IsTemp ?? false;
    public TypeModel FinalType => finalType.Value;
    private readonly Lazy<TypeModel> finalType;
    public bool IsCollection => Type is CollectionTypeModel;

    public PropertyModel(TreeNodeModel node, PropertySyntax syntax)
    {
        Node = node;
        Syntax = syntax;
        Name = Syntax.Name;
        Type = TypeModel.CreateFromSyntax(Node.Tree, syntax.Type);
        finalType = new(ComputeFinalType);
    }

    public PropertyModel(TreeNodeModel node, string name, TypeModel type)
    {
        Node = node;
        Name = name;
        Type = type;
        finalType = new(ComputeFinalType);
    }

    private TypeModel ComputeFinalType()
    {
        if (!IsChild || !Type.ReferencedNode()!.IsTemp)
            return Type;

        return Type.WithOptionalSymbol(Type.ReferencedNode()!.FinalNode!.Defines);
    }

    public override string ToString() => $"{Node.Defines}.{Name}:{Type}";
}
