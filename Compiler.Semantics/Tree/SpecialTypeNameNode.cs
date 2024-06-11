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
    private ValueAttribute<TypeSymbol> referencedSymbol;
    public override TypeSymbol ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolAspect.SpecialTypeName_ReferencedSymbol);
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype NamedAntetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.SpecialTypeName_NamedAntetype);
    private ValueAttribute<BareType?> bareType;
    public override BareType? NamedBareType
    => bareType.TryGetValue(out var value) ? value
        : bareType.GetValue(this, BareTypeAspect.SpecialTypeName_NamedBareType);

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
