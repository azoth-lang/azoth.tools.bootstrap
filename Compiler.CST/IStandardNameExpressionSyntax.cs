using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// An expression that is an unqualified name (either generic or non-generic).
/// </summary>
public partial interface IStandardNameExpressionSyntax
{
    IEnumerable<IPromise<Symbol>> LookupInContainingScope();
}
