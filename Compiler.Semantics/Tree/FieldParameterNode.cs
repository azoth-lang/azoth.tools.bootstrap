using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldParameterNode : ParameterNode, IFieldParameterNode
{
    public override IFieldParameterSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    public override DataType Type => throw new NotImplementedException();

    public FieldParameterNode(IFieldParameterSyntax syntax)
    {
        Syntax = syntax;
    }
}
