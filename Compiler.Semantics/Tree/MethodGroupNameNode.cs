using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodGroupNameNode : AmbiguousNameExpressionNode, IMethodGroupNameNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private Child<IExpressionNode> context;
    public IExpressionNode Context => context.Value;
    public StandardName MethodName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }

    public MethodGroupNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IStandardMethodDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}
