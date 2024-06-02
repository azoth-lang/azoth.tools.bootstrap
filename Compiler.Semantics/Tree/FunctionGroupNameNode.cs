using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionGroupNameNode : AmbiguousNameExpressionNode, IFunctionGroupNameNode
{
    public override INameExpressionSyntax Syntax { get; }
    public INameExpressionNode? Context { get; }
    public StandardName FunctionName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }

    public FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionLikeDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}
