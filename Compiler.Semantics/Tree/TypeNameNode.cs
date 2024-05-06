using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNameNode : TypeNode, ITypeNameNode
{
    public abstract override ITypeNameSyntax Syntax { get; }
    public abstract TypeName Name { get; }
}
