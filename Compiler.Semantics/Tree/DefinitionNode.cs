using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class DefinitionNode : CodeNode, IDefinitionNode
{
    public abstract override IDefinitionSyntax? Syntax { get; }
    private ValueAttribute<IPackageFacetNode> facet;
    public abstract StandardName? Name { get; }
    public IPackageFacetNode Facet
        => facet.TryGetValue(out var value) ? value : facet.GetValue(() => (IPackageFacetNode)InheritedFacet());
    public virtual ISymbolDeclarationNode ContainingDeclaration
        => InheritedContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public virtual Symbol ContainingSymbol => ContainingDeclaration.Symbol;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public virtual LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    public abstract LexicalScope LexicalScope { get; }
    public abstract Symbol Symbol { get; }
}
