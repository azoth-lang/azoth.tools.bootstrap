using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDefinitionNode : PackageMemberDefinitionNode, ITypeDefinitionNode
{
    public abstract override ITypeDefinitionSyntax Syntax { get; }
    public bool IsConst => Syntax.IsConst;
    public StandardName Name => Syntax.Name;
    public abstract IDeclaredUserType DeclaredType { get; }
    public abstract override IUserTypeDeclarationNode SymbolNode { get; }
    private ValueAttribute<UserTypeSymbol> symbol;
    public override UserTypeSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.TypeDeclaration);
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    private ValueAttribute<LexicalScope> supertypesLexicalScope;
    public LexicalScope SupertypesLexicalScope
        => supertypesLexicalScope.TryGetValue(out var value) ? value
            : supertypesLexicalScope.GetValue(this, LexicalScopeAttributes.TypeDeclaration_SupertypesLexicalScope);
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    private ValueAttribute<CompilerResult<IFixedSet<BareReferenceType>>> supertypes;
    public CompilerResult<IFixedSet<BareReferenceType>> Supertypes
        => supertypes.TryGetValue(out var value) ? value
            : supertypes.GetValue(this, TypeDeclarationsAspect.TypeDeclaration_Supertypes);
    public abstract IFixedList<ITypeMemberDefinitionNode> Members { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.TypeDeclaration_LexicalScope);

    protected TypeDefinitionNode(
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames)
        // TODO support attributes on type declarations
        : base(Enumerable.Empty<IAttributeNode>())
    {
        GenericParameters = ChildList.Attach(this, genericParameters);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
    }

    internal override IUserTypeDeclarationNode InheritedContainingDeclarationNode(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.TypeDeclaration_InheritedContainingSymbolNode(this);

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode caller, IChildNode child)
        => ContainingDeclaredTypeAttribute.TypeDeclaration_InheritedContainingDeclaredType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
    {
        if (((ITypeDefinitionNode)this).AllSupertypeNames.Contains(caller))
            return LexicalScopeAttributes.TypeDeclaration_InheritedLexicalScope_Supertypes(this);
        return LexicalScopeAttributes.TypeDeclaration_InheritedLexicalScope(this);
    }

    internal override ITypeDefinitionNode InheritedContainingTypeDeclaration(IChildNode caller, IChildNode child)
        => this;

    internal override bool InheritedIsAttributeType(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.TypeDeclaration_InheritedIsAttributeType(this);

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeDeclarationsAspect.TypeDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
