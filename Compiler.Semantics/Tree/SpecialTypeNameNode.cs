using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SpecialTypeNameNode : TypeNameNode, ISpecialTypeNameNode
{
    public override ISpecialTypeNameSyntax Syntax { get; }
    public override SpecialTypeName Name => Syntax.Name;
    private TypeSymbol? referencedSymbol;
    private bool referencedSymbolCached;
    public override TypeSymbol ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol!
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol, SymbolAspect.SpecialTypeName_ReferencedSymbol);
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype NamedAntetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.SpecialTypeName_NamedAntetype);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public override BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType!
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType, BareTypeAspect.SpecialTypeName_NamedBareType);

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
