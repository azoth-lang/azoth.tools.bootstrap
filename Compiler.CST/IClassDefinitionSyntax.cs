using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IClassDefinitionSyntax
{
    IEnumerable<IStandardTypeNameSyntax> ITypeDefinitionSyntax.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}
