using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
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
            : referencedSymbol.GetValue(this, SymbolAttribute.Attribute_ReferencedSymbol);

    public AttributeNode(IAttributeSyntax syntax, IStandardTypeNameNode typeName)
    {
        Syntax = syntax;
        TypeName = Child.Attach(this, typeName);
    }

    internal override bool InheritedIsAttributeType(IChildNode caller, IChildNode child)
    {
        if (child == TypeName)
            return SymbolNodeAttributes.Attribute_InheritedIsAttributeType_Child(this);
        return base.InheritedIsAttributeType(caller, child);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        SymbolAttribute.Attribute_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
