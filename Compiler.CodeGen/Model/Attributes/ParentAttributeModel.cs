using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class ParentAttributeModel : AttributeModel
{
    public override AttributeSyntax? Syntax => null;
    public override TreeNodeModel Node { get; }
    public override string Name => "Parent";
    public override SymbolTypeModel Type { get; }
    public override bool IsTemp => false;
    public override bool IsChild => false;
    public override bool IsMethod => false;

    public ParentAttributeModel(TreeNodeModel node, InternalSymbol symbol)
    {
        Node = node;
        Type = SymbolTypeModel.Create(symbol);
    }

    public override string ToString() => $"{Node.Defines}.{Name}: {Type}";
}
