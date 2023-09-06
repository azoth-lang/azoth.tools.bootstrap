using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// An expression that is a single unqualified name.
/// </summary>
public partial interface ISimpleNameExpressionSyntax
{
    IEnumerable<IPromise<Symbol>> LookupInContainingScope();
}