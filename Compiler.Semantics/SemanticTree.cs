using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public partial interface ITypeDeclarationNode
{
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames => SupertypeNames;
}

public partial interface IClassDeclarationNode
{
    IEnumerable<IStandardTypeNameNode> ITypeDeclarationNode.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}
