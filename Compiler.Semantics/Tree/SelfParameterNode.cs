using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SelfParameterNode : ParameterNode, ISelfParameterNode
{
    public abstract override ISelfParameterSyntax Syntax { get; }
    public bool IsLentBinding => Syntax.IsLentBinding;
    private ValueAttribute<IDeclaredUserType> containingDeclaredType;
    public IDeclaredUserType ContainingDeclaredType
        => containingDeclaredType.TryGetValue(out var value) ? value
            : containingDeclaredType.GetValue(InheritedContainingDeclaredType);
    private ValueAttribute<SelfParameterSymbol> symbol;
    public SelfParameterSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.SelfParameter_Symbol);

    public override FlowState FlowStateAfter => throw new System.NotImplementedException();
}
