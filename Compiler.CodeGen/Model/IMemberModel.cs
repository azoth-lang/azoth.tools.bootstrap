using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public interface IMemberModel
{
    TreeNodeModel Node { get; }
    string Name { get; }
    TypeModel Type { get; }
    TypeModel FinalType { get; }
    bool IsTemp { get; }
}
