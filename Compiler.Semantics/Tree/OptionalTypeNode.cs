using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalTypeNode : TypeNode, IOptionalTypeNode
{
    public override IOptionalTypeSyntax Syntax { get; }
    public ITypeNode Referent { get; }
    private ValueAttribute<IMaybeAntetype> namedAntetype;
    public override IMaybeAntetype NamedAntetype
        => namedAntetype.TryGetValue(out var value) ? value
            : namedAntetype.GetValue(this, TypeExpressionsAntetypesAspect.OptionalType_NamedAntetype);
    private DataType? namedType;
    private bool namedTypeCached;
    public override DataType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType, TypeExpressionsAspect.OptionalType_NamedType);

    public OptionalTypeNode(IOptionalTypeSyntax syntax, ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
