using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

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
            : symbol.GetValue(this, SymbolAspect.TypeDeclaration);
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    private ValueAttribute<LexicalScope> supertypesLexicalScope;
    public LexicalScope SupertypesLexicalScope
        => supertypesLexicalScope.TryGetValue(out var value) ? value
            : supertypesLexicalScope.GetValue(this, LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    private ValueAttribute<CompilerResult<IFixedSet<BareReferenceType>>> supertypesLegacy;
    public CompilerResult<IFixedSet<BareReferenceType>> SupertypesLegacy
        => supertypesLegacy.TryGetValue(out var value) ? value
            : supertypesLegacy.GetValue(this, TypeDeclarationsAspect.TypeDeclaration_SupertypesLegacy);
    public abstract IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    private MultiMapHashSet<StandardName, IAssociatedMemberDeclarationNode>? associatedMembersByName;
    public abstract IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    private MultiMapHashSet<StandardName, IInstanceMemberDeclarationNode>? inclusiveInstanceMembersByName;
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
        => SymbolNodeAspect.TypeDeclaration_InheritedContainingDeclaration(this);

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => ContainingDeclaredTypeAspect.TypeDeclaration_InheritedContainingDeclaredType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (GenericParameters.Contains(child))
            return ContainingLexicalScope;
        if (((ITypeDefinitionNode)this).AllSupertypeNames.Contains(child))
            return LexicalScopingAspect.TypeDefinition_InheritedLexicalScope_AllSupertypeNames(this);
        return LexicalScopingAspect.TypeDefinition_InheritedLexicalScope_Members(this);
    }

    internal override ITypeDefinitionNode InheritedContainingTypeDefinition(IChildNode child, IChildNode descendant)
        => this;

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => SymbolNodeAspect.TypeDeclaration_InheritedIsAttributeType(this);

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeDeclarationsAspect.TypeDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named)
        => InclusiveMembers.OfType<IInstanceMemberDeclarationNode>().MembersNamed(ref inclusiveInstanceMembersByName, named);

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Members.OfType<IAssociatedMemberDeclarationNode>().MembersNamed(ref associatedMembersByName, named);
}
