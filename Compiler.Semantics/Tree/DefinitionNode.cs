using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class DefinitionNode : CodeNode, IDefinitionNode
{
    public abstract override IDefinitionSyntax? Syntax { get; }
    private ValueAttribute<IPackageFacetNode> facet;
    public abstract StandardName? Name { get; }
    public IPackageFacetNode Facet
        => facet.TryGetValue(out var value) ? value : facet.GetValue(() => (IPackageFacetNode)Inherited_Facet());
    public virtual ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public virtual Symbol ContainingSymbol => ContainingDeclaration.Symbol;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public virtual LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope, ReferenceEqualityComparer.Instance);
    public abstract LexicalScope LexicalScope { get; }
    public abstract Symbol Symbol { get; }
}
