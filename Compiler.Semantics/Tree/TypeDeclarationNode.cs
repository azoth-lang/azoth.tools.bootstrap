using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDeclarationNode : DeclarationNode, ITypeDeclarationNode
{
    public abstract override ITypeDeclarationSyntax Syntax { get; }

    private ValueAttribute<NamespaceSymbol?> inheritedContainingNamespace;
    public override NamespaceSymbol? InheritedContainingNamespace
        => inheritedContainingNamespace.TryGetValue(out var value) ? value
            : inheritedContainingNamespace.GetValue(this, ContainingNamespaceAttribute.TypeDeclarationInherited);

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
}
