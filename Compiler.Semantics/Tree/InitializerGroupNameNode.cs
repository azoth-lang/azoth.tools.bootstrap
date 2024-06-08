using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerGroupNameNode : AmbiguousNameExpressionNode, IInitializerGroupNameNode
{
    public override INameExpressionSyntax Syntax { get; }
    public ITypeNameExpressionNode Context { get; }
    public StandardName? InitializerName { get; }
    public IMaybeAntetype InitializingAntetype => Context.NamedAntetype;
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }

    public InitializerGroupNameNode(
        INameExpressionSyntax syntax,
        ITypeNameExpressionNode context,
        StandardName? initializerName,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        InitializerName = initializerName;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}
