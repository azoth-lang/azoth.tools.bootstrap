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

internal sealed class StandardTypeNameExpressionNode : AmbiguousNameExpressionNode, IStandardTypeNameExpressionNode
{
    public override IStandardNameExpressionSyntax Syntax { get; }
    public StandardName Name => Syntax.Name;
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

    public StandardTypeNameExpressionNode(
        IStandardNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclaration = referencedDeclaration;
    }
}
