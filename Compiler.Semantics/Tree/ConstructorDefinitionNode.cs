using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ConstructorDefinitionNode : TypeMemberDefinitionNode, IConstructorDefinitionNode
{
    public abstract override IConstructorDefinitionSyntax? Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public override IdentifierName? Name => Syntax?.Name;
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }

    public override LexicalScope LexicalScope => throw new NotImplementedException();
    private ValueAttribute<IConstructorDeclarationNode> symbolNode;
    public override IConstructorDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.ConstructorDeclaration_SymbolNode);
    public abstract override ConstructorSymbol Symbol { get; }

    private protected ConstructorDefinitionNode(
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Parameters = ChildList.Attach(this, parameters);
    }
}
