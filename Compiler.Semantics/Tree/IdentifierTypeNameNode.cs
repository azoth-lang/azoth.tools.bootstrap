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
    public override IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.IdentifierTypeName_Antetype);
    private ValueAttribute<BareType?> bareType;
    public override BareType? BareType
        => bareType.TryGetValue(out var value) ? value
            : bareType.GetValue(this, BareTypeAspect.IdentifierTypeName);

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
