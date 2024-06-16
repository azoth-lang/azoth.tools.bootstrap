using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierTypeNameNode : TypeNameNode, IIdentifierTypeNameNode
{
    public override IIdentifierTypeNameSyntax Syntax { get; }
    private ValueAttribute<bool> attributeType;
    public bool IsAttributeType
        => attributeType.TryGetValue(out var value) ? value
            : attributeType.GetValue(InheritedIsAttributeType);
    public override IdentifierName Name => Syntax.Name;
    private ValueAttribute<ITypeDeclarationNode?> referencedDeclaration;
    public ITypeDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, SymbolNodeAspect.StandardTypeName_ReferencedDeclaration);
    public override TypeSymbol? ReferencedSymbol
        => SymbolAspect.StandardTypeName(this);
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype NamedAntetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.IdentifierTypeName_NamedAntetype);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public override BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType!
            : GrammarAttribute.Synthetic(ref namedBareTypeCached, this,
                BareTypeAspect.IdentifierTypeName_NamedBareType, ref namedBareType);
    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        SymbolNodeAspect.StandardTypeName_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
