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
    public override StandardName Name => Syntax.Name;
    public abstract IDeclaredUserType DeclaredType { get; }
    private ValueAttribute<UserTypeSymbol> symbol;
    public override UserTypeSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.TypeDeclaration);
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    private ValueAttribute<LexicalScope> supertypesLexicalScope;
    public LexicalScope SupertypesLexicalScope
        => supertypesLexicalScope.TryGetValue(out var value) ? value
            : supertypesLexicalScope.GetValue(this, LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    private ValueAttribute<CompilerResult<IFixedSet<BareReferenceType>>> supertypes;
    public CompilerResult<IFixedSet<BareReferenceType>> Supertypes
        => supertypes.TryGetValue(out var value) ? value
            : supertypes.GetValue(this, TypeDeclarationsAspect.TypeDeclaration_Supertypes);
    public abstract IFixedList<ITypeMemberDefinitionNode> Members { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.TypeDefinition_LexicalScope);

    protected TypeDefinitionNode(
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames)
        // TODO support attributes on type declarations
        : base(Enumerable.Empty<IAttributeNode>())
    {
        GenericParameters = ChildList.Attach(this, genericParameters);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
    }

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant)
        => SymbolNodeAttributes.TypeDeclaration_InheritedContainingDeclaration(this);

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant)
        => ContainingDeclaredTypeAttribute.TypeDeclaration_InheritedContainingDeclaredType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (((ITypeDefinitionNode)this).AllSupertypeNames.Contains(child))
            return LexicalScopingAspect.TypeDefinition_InheritedLexicalScope_Supertypes(this);
        return LexicalScopingAspect.TypeDefinition_InheritedLexicalScope(this);
    }

    internal override ITypeDefinitionNode InheritedContainingTypeDeclaration(IChildNode child, IChildNode descendant)
        => this;

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => SymbolNodeAttributes.TypeDeclaration_InheritedIsAttributeType(this);

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeDeclarationsAspect.TypeDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
