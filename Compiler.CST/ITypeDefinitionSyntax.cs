using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ITypeDefinitionSyntax
{
    IEnumerable<IStandardTypeNameSyntax> AllSupertypeNames => SupertypeNames;
}
