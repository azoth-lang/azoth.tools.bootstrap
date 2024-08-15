using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AttributeNode : CodeNode, IAttributeNode
{
    public override IAttributeSyntax Syntax { get; }
    public IStandardTypeNameNode TypeName { get; }
    private ValueAttribute<ConstructorSymbol?> referencedSymbol;
    public ConstructorSymbol? ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolAspect.Attribute_ReferencedSymbol);

    public AttributeNode(IAttributeSyntax syntax, IStandardTypeNameNode typeName)
    {
        Syntax = syntax;
        TypeName = Child.Attach(this, typeName);
    }

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
    {
        if (descendant == TypeName)
            return SymbolNodeAspect.Attribute_InheritedIsAttributeType_Child(this);
        return base.InheritedIsAttributeType(child, descendant);
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        SymbolAspect.Attribute_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
