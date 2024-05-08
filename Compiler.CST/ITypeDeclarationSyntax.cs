using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ITypeDeclarationSyntax
{
    IEnumerable<IStandardTypeNameSyntax> AllSupertypeNames => SupertypeNames;
}
