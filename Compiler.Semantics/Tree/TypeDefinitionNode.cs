using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeDefinitionNode : PackageMemberDefinitionNode, ITypeDefinitionNode
{
    public abstract override ITypeDefinitionSyntax Syntax { get; }
    public bool IsConst => Syntax.ConstModifier is not null;
    public override StandardName Name => Syntax.Name;
    public abstract IDeclaredUserType DeclaredType { get; }
    private UserTypeSymbol? symbol;
    private bool symbolCached;
    public override UserTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolAspect.TypeDefinition_Symbol);
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    private ValueAttribute<LexicalScope> supertypesLexicalScope;
    public LexicalScope SupertypesLexicalScope
        => supertypesLexicalScope.TryGetValue(out var value) ? value
            : supertypesLexicalScope.GetValue(this, LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    private Circular<IFixedSet<BareReferenceType>> supertypes = new([]);
    private bool supertypesCached;
    public IFixedSet<BareReferenceType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDeclarationsAspect.TypeDefinition_Supertypes, FixedSet.EqualityComparer<BareType>());
    public abstract IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    //private MultiMapHashSet<StandardName, IAssociatedMemberDeclarationNode>? associatedMembersByName;
    public abstract IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope, ReferenceEqualityComparer.Instance);

    protected TypeDefinitionNode(
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames)
        // TODO support attributes on type declarations
        : base([])
    {
        GenericParameters = ChildList.Attach(this, genericParameters);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
    }

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => SymbolNodeAspect.TypeDeclaration_InheritedContainingDeclaration(this);

    internal override IDeclaredUserType InheritedContainingDeclaredType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => ContainingDeclaredTypeAspect.TypeDeclaration_InheritedContainingDeclaredType(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (GenericParameters.Contains(child))
            return ContainingLexicalScope;
        if (((ITypeDefinitionNode)this).AllSupertypeNames.Contains(child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
    }

    internal override ITypeDefinitionNode InheritedContainingTypeDefinition(IChildNode child, IChildNode descendant)
        => this;

    internal override bool InheritedIsAttributeType(IChildNode child, IChildNode descendant)
        => SymbolNodeAspect.TypeDeclaration_InheritedIsAttributeType(this);

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeDeclarationsAspect.TypeDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
