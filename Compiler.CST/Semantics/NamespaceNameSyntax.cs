using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

/// <summary>
/// Really a group of namespaces names, but for the most part that doesn't matter.
/// </summary>
public sealed class NamespaceNameSyntax : SyntaxSemantics, IIdentifierNameExpressionSyntaxSemantics,
    IMemberAccessSyntaxSemantics
{
    public new IFixedSet<NamespaceSymbol> Symbols { get; }
    IPromise<Symbol?> IMemberAccessSyntaxSemantics.Symbol => Promise.Null<Symbol>();
    IPromise<Symbol?> IIdentifierNameExpressionSyntaxSemantics.Symbol => Promise.Null<Symbol>();

    public NamespaceNameSyntax(IFixedSet<NamespaceSymbol> symbols) : base(symbols)
    {
        Symbols = symbols;
    }
}
