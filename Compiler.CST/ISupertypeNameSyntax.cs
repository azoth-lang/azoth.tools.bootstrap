using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface ISupertypeNameSyntax
{
    Promise<BareReferenceType?> NamedType { get; }

    IEnumerable<IPromise<UserTypeSymbol>> LookupInContainingScope();
}
