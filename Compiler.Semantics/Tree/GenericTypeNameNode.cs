using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericTypeNameNode : TypeNameNode, IGenericTypeNameNode
{
    public override IGenericTypeNameSyntax Syntax { get; }
    private ValueAttribute<bool> attributeType;
    public bool IsAttributeType
        => attributeType.TryGetValue(out var value) ? value
            : attributeType.GetValue(() => Inherited_IsAttributeType(GrammarAttribute.CurrentInheritanceContext()));
    public override GenericName Name => Syntax.Name;
    public override TypeSymbol? ReferencedSymbol => SymbolsAspect.StandardTypeName_ReferencedSymbol(this);
    public IFixedList<ITypeNode> TypeArguments { get; }
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                SymbolNodeAspect.StandardTypeName_ReferencedDeclaration, ReferenceEqualityComparer.Instance);
    private IMaybeAntetype? namedAntetype;
    private bool namedAntetypeCached;
    public override IMaybeAntetype NamedAntetype
        => GrammarAttribute.IsCached(in namedAntetypeCached) ? namedAntetype!
            : this.Synthetic(ref namedAntetypeCached, ref namedAntetype,
                TypeExpressionsAntetypesAspect.GenericTypeName_NamedAntetype);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public override BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType!
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType, BareTypeAspect.GenericTypeName_NamedBareType);

    public GenericTypeNameNode(IGenericTypeNameSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        SymbolNodeAspect.StandardTypeName_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }
}
