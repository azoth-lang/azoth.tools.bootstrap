using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ITypeNameSyntax : ITypeSyntax
{
    IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope();
}
