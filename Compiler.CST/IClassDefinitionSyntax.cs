using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IClassDefinitionSyntax
{
    IEnumerable<IStandardTypeNameSyntax> ITypeDefinitionSyntax.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}
