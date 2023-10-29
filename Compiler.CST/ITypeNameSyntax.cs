using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ITypeNameSyntax
{
    IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope(bool withAttributeSuffix);
}
