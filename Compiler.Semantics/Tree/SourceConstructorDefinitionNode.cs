using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SourceConstructorDefinitionNode : ConstructorDefinitionNode, ISourceConstructorDefinitionNode
{
    public override IConstructorDefinitionSyntax Syntax { get; }
    public IConstructorSelfParameterNode SelfParameter { get; }
    public IBlockBodyNode Body { get; }
    private ValueAttribute<ConstructorSymbol> symbol;
    public override ConstructorSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.SourceConstructorDefinition);

    public SourceConstructorDefinitionNode(IConstructorDefinitionSyntax syntax, IConstructorSelfParameterNode selfParameter, IEnumerable<IConstructorOrInitializerParameterNode> parameters, IBlockBodyNode body)
        : base(parameters)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Body = Child.Attach(this, body);
    }
}