using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class QualifiedTypeNameExpression : AmbiguousNameExpressionNode, IQualifiedTypeNameExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public INamespaceNameNode Context { get; }
    public StandardName Name => Syntax.MemberName;
    public IFixedList<ITypeNode> TypeArguments { get; }
    public ITypeDeclarationNode ReferencedDeclaration { get; }

    public QualifiedTypeNameExpression(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclaration = referencedDeclaration;
    }
}