using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IClassDeclarationSyntax
{
    void CreateDefaultConstructor(ISymbolTreeBuilder symbolTree);

    IEnumerable<IStandardTypeNameSyntax> ITypeDeclarationSyntax.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}
