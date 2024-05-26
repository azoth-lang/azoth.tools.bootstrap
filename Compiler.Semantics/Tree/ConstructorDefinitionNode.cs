using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ConstructorDefinitionNode : TypeMemberDefinitionNode, IConstructorDefinitionNode
{
    public abstract override IConstructorDefinitionSyntax? Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public override IdentifierName? Name => Syntax?.Name;
    public abstract IConstructorSelfParameterNode? SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }

    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    public abstract override ConstructorSymbol Symbol { get; }

    private protected ConstructorDefinitionNode(
        IEnumerable<IConstructorOrInitializerParameterNode> parameters)
    {
        Parameters = ChildList.Attach(this, parameters);
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == SelfParameter) return null;
        if (descendant is IConstructorOrInitializerParameterNode parameter
            && Parameters.IndexOf(parameter) is int index)
            return index == 0 ? SelfParameter : Parameters[index - 1];

        return base.InheritedPredecessor(child, descendant);
    }
}
