using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class DefaultConstructorDefinitionNode : ConstructorDefinitionNode, IDefaultConstructorDefinitionNode
{
    public override IConstructorDefinitionSyntax? Syntax => null;
    public override IConstructorSelfParameterNode? SelfParameter => null;
    private ValueAttribute<ConstructorSymbol> symbol;
    public override ConstructorSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAspect.DefaultConstructorDefinition);
    public DefaultConstructorDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>())
    { }
}
