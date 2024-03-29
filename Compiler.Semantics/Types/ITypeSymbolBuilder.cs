using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Something that can ensure that the symbols for types are built.
/// </summary>
public interface ITypeSymbolBuilder
{
    TSymbol Build<TSymbol>(IPromise<TSymbol> promise)
        where TSymbol : TypeSymbol;
}
