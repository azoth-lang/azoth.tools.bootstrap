using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

public partial interface INamespaceSymbolNode
{
    IEnumerable<INamespaceMemberSymbolNode> MembersNamed(IdentifierName named);
}
