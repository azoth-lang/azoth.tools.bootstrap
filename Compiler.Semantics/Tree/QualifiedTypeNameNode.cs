using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class QualifiedTypeNameNode : TypeNameNode, IQualifiedTypeNameNode
{
    public override IQualifiedTypeNameSyntax Syntax { get; }
    public ITypeNameNode Context { get; }
    public IStandardTypeNameNode QualifiedName { get; }

    public override TypeName Name => Syntax.Name;
    public override IMaybeAntetype NamedAntetype => throw new NotImplementedException();
    public override BareType? NamedBareType => throw new NotImplementedException();
    public override TypeSymbol? ReferencedSymbol => throw new NotImplementedException();

    public QualifiedTypeNameNode(
        IQualifiedTypeNameSyntax syntax,
        ITypeNameNode context,
        IStandardTypeNameNode qualifiedName)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        QualifiedName = Child.Attach(this, qualifiedName);
    }
}
