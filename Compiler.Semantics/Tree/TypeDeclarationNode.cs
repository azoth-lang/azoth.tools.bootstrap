using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDeclarationNode : PackageMemberDeclarationNode, ITypeDeclarationNode
{
    public abstract override ITypeDeclarationSyntax Syntax { get; }
    public bool IsConst => Syntax.IsConst;
    public StandardName Name => Syntax.Name;
    public abstract override ITypeSymbolNode SymbolNode { get; }
    public UserTypeSymbol Symbol => SymbolNode.Symbol;
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    public abstract IFixedList<ITypeMemberDeclarationNode> Members { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value)
            ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.TypeDeclaration);
    private ValueAttribute<Promise<IDeclaredUserType>> inheritedContainingDeclaredType;

    protected TypeDeclarationNode(
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames)
    {
        GenericParameters = ChildList.CreateFixed(this, genericParameters);
        SupertypeNames = ChildList.CreateFixed(this, supertypeNames);
    }

    internal override ITypeSymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.TypeDeclarationInherited(this);

    internal override Promise<IDeclaredUserType> InheritedContainingDeclaredType(IChildNode caller, IChildNode child)
        => inheritedContainingDeclaredType.TryGetValue(out var value) ? value
            : inheritedContainingDeclaredType.GetValue(this, TypeAttributes.TypeDeclarationInheritedDeclaredType);
}
