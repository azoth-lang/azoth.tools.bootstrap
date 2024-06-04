using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNameNode : TypeNode, ITypeNameNode
{
    public abstract override ITypeNameSyntax Syntax { get; }
    public abstract TypeName Name { get; }
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public virtual LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    public abstract BareType? BareType { get; }
    public abstract TypeSymbol? ReferencedSymbol { get; }
    private ValueAttribute<DataType> type;
    public sealed override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.TypeName_Type);
}
