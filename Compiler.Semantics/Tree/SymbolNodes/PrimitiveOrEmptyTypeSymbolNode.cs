using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class PrimitiveOrEmptyTypeSymbolNode : ChildSymbolNode, IPrimitiveTypeSymbolNode
{
    public abstract SpecialTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    public abstract override TypeSymbol Symbol { get; }
    public IFixedSet<BareReferenceType> Supertypes
        => Symbol.GetDeclaredType()?.Supertypes ?? [];
    private ValueAttribute<IFixedSet<ITypeMemberDeclarationNode>> members;
    public IFixedSet<ITypeMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value : members.GetValue(GetMembers);
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers
        // For now, the symbol tree already includes all inherited members.
        => Members;
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached)
            ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.PrimitiveTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.PrimitiveTypeDeclaration_AssociatedMembersByName);

    private protected PrimitiveOrEmptyTypeSymbolNode()
    {
    }

    private new IFixedSet<ITypeMemberDeclarationNode> GetMembers()
        => ChildSet.Attach(this, GetMembers(Primitive.SymbolTree).OfType<ITypeMemberDeclarationNode>());
}
