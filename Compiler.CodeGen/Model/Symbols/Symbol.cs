using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Closed(
    typeof(InternalSymbol),
    typeof(ExternalSymbol))]
public abstract class Symbol : IEquatable<Symbol>
{
    public static ExternalSymbol Void { get; } = new ExternalSymbol("void");

    [return: NotNullIfNotNull(nameof(syntax))]
    public static Symbol? CreateFromSyntax(Grammar? grammar, SymbolNode? syntax)
    {
        if (syntax is null)
            return null;
        return grammar is null || syntax.IsQuoted
            ? CreateExternalFromSyntax(syntax)
            : new InternalSymbol(grammar, syntax.Text);
    }

    [return: NotNullIfNotNull(nameof(syntax))]
    public static InternalSymbol? CreateInternalFromSyntax(Grammar grammar, SymbolNode? syntax)
    {
        if (syntax is null) return null;
        if (syntax.IsQuoted) throw new ArgumentException("Internal symbol cannot be quoted.", nameof(syntax));
        return new InternalSymbol(grammar, syntax.Text);
    }

    [return: NotNullIfNotNull(nameof(syntax))]
    public static ExternalSymbol? CreateExternalFromSyntax(SymbolNode? syntax)
    {
        if (syntax is null) return null;
        if (!syntax.IsQuoted)
            throw new ArgumentException("External symbol must be quoted.", nameof(syntax));
        return new ExternalSymbol(syntax.Text);
    }

    public abstract string FullName { get; }

    private protected Symbol() { }

    #region Equality
    public abstract bool Equals(Symbol? other);

    public override bool Equals(object? obj) => obj is Symbol other && Equals(other);

    public abstract override int GetHashCode();

    public static bool operator ==(Symbol? left, Symbol? right) => Equals(left, right);

    public static bool operator !=(Symbol? left, Symbol? right) => !Equals(left, right);
    #endregion

    public abstract override string ToString();
}
