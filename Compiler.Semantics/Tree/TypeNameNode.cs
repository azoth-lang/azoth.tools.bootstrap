using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNameNode : TypeNode, ITypeNameNode
{
    public abstract override ITypeNameSyntax Syntax { get; }
    public abstract TypeName Name { get; }
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public virtual LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);

    public abstract TypeSymbol? ReferencedSymbol { get; }
}
