using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticUserTypeSymbolNode : SemanticDeclarationSymbolNode, IUserTypeDeclarationNode
{
    protected abstract ITypeDefinitionNode Node { get; }
    public override StandardName Name => Node.Name;
    public override UserTypeSymbol Symbol => Node.Symbol;
    public abstract IFixedList<ITypeMemberDeclarationNode> Members { get; }
    private MultiMapHashSet<StandardName, ITypeMemberDeclarationNode>? membersByName;

    public IEnumerable<ITypeMemberDeclarationNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);
}
