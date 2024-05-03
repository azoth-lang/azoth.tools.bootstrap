using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDeclarationNode : DeclarationNode, ITypeDeclarationNode
{
    public abstract override ITypeDeclarationSyntax Syntax { get; }
    public override StandardName Name => Syntax.Name;

    private ValueAttribute<ITypeSymbolNode> inheritedContainingSymbolNode;
    private ValueAttribute<ITypeSymbolNode> symbolNode;
    public ITypeSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttribute.TypeDeclaration);
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    public IFixedList<ISupertypeNameNode> SupertypeNames { get; }
    public abstract IFixedList<ITypeMemberDeclarationNode> Members { get; }

    protected TypeDeclarationNode(
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<ISupertypeNameNode> supertypeNames)
    {
        GenericParameters = ChildList.CreateFixed(this, genericParameters);
        SupertypeNames = ChildList.CreateFixed(this, supertypeNames);
    }

    internal override ITypeSymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => inheritedContainingSymbolNode.TryGetValue(out var value) ? value
            : inheritedContainingSymbolNode.GetValue(this, SymbolNodeAttribute.TypeDeclarationInherited);
}
