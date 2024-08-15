using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ITypeDefinitionSyntax
{
    IEnumerable<IStandardTypeNameSyntax> AllSupertypeNames => SupertypeNames;
}
