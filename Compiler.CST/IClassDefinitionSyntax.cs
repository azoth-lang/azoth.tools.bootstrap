using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IClassDefinitionSyntax
{
    void CreateDefaultConstructor(ISymbolTreeBuilder symbolTree);

    IEnumerable<IStandardTypeNameSyntax> ITypeDefinitionSyntax.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}
