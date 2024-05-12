using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SpecialTypeNameNode : TypeNameNode, ISpecialTypeNameNode
{
    public override ISpecialTypeNameSyntax Syntax { get; }
    public override SpecialTypeName Name => Syntax.Name;
    private ValueAttribute<TypeSymbol> referencedSymbol;
    public override TypeSymbol ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolAttribute.SpecialTypeName_ReferencedSymbol);
    private ValueAttribute<BareType?> bareType;
    public override BareType? BareType
    => bareType.TryGetValue(out var value) ? value
        : bareType.GetValue(this, BareTypeAttribute.SpecialTypeName);

    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.SpecialTypeName_Type);

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
