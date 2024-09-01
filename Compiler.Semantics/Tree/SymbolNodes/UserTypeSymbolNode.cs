using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class UserTypeSymbolNode : PackageFacetChildSymbolNode, IUserTypeSymbolNode
{
    public override StandardName Name => base.Name!;
    private ValueAttribute<IFixedList<IGenericParameterSymbolNode>> genericParameters;
    public IFixedList<IGenericParameterSymbolNode> GenericParameters
        => genericParameters.TryGetValue(out var value) ? value
            : genericParameters.GetValue(GetGenericParameters);

    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedSet<ITypeMemberSymbolNode> Members { get; }
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

    private protected UserTypeSymbolNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    private IFixedList<IGenericParameterSymbolNode> GetGenericParameters()
    {
        var declarationNodes = SymbolTree().GetChildrenOf(Symbol)
            .OfType<GenericParameterTypeSymbol>().Select(SymbolNodeAspect.Symbol).WhereNotNull()
            .Cast<IGenericParameterSymbolNode>();
        return ChildList.Attach(this, declarationNodes);
    }

    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
}
