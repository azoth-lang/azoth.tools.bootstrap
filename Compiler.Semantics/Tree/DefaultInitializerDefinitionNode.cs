using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class DefaultInitializerDefinitionNode : InitializerDefinitionNode, IDefaultInitializerDefinitionNode
{
    public override IInitializerDefinitionSyntax? Syntax => null;
    public override IInitializerSelfParameterNode? SelfParameter => null;
    public override IBlockBodyNode? Body => null;
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public override InitializerSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.DefaultInitializerDefinition_Symbol);

    public DefaultInitializerDefinitionNode()
        : base(FixedList.Empty<IConstructorOrInitializerParameterNode>()) { }
}
