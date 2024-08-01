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
    public override IBodyNode? Body => null;
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public override ConstructorSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.DefaultConstructorDefinition_Symbol);

    public DefaultConstructorDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>())
    { }
}
