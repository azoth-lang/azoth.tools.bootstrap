using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SupertypeNameNode : CodeNode, ISupertypeNameNode
{
    public override ISupertypeNameSyntax Syntax { get; }
    public TypeName Name => Syntax.Name;
    public IFixedList<ITypeNode> TypeArguments { get; }

    public SupertypeNameNode(ISupertypeNameSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.CreateFixed(this, typeArguments);
    }
}
