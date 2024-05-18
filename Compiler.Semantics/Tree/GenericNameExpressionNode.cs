using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericNameExpressionNode : AmbiguousNameExpressionNode, IGenericNameExpressionNode
{
    public override IGenericNameExpressionSyntax Syntax { get; }
    public GenericName Name => Syntax.Name;
    public IFixedList<ITypeNode> TypeArguments { get; }
    public Symbol? ReferencedSymbol => throw new NotImplementedException();

    public GenericNameExpressionNode(IGenericNameExpressionSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}
