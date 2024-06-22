using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class QualifiedTypeNameExpression : AmbiguousNameExpressionNode, IQualifiedTypeNameExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public INamespaceNameNode Context { get; }
    public StandardName Name => Syntax.MemberName;
    public IFixedList<ITypeNode> TypeArguments { get; }
    public ITypeDeclarationNode ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeAntetype> namedAntetype;
    public IMaybeAntetype NamedAntetype
        => namedAntetype.TryGetValue(out var value) ? value
            : namedAntetype.GetValue(this, TypeExpressionsAntetypesAspect.TypeNameExpression_NamedAntetype);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType!
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType, BareTypeAspect.TypeNameExpression_NamedBareType);

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
