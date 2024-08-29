using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AttributeNode : CodeNode, IAttributeNode
{
    public override IAttributeSyntax Syntax { get; }
    public IStandardTypeNameNode TypeName { get; }
    private ValueAttribute<ConstructorSymbol?> referencedSymbol;
    public ConstructorSymbol? ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolsAspect.Attribute_ReferencedSymbol);

    public AttributeNode(IAttributeSyntax syntax, IStandardTypeNameNode typeName)
    {
        Syntax = syntax;
        TypeName = Child.Attach(this, typeName);
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == TypeName)
            return true;
        return base.Inherited_IsAttributeType(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        BindingNamesAspect.Attribute_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
