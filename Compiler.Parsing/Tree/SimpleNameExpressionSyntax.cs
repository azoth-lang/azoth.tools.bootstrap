using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// A name of a variable or namespace
/// </summary>
internal class SimpleNameExpressionSyntax : ExpressionSyntax, ISimpleNameExpressionSyntax
{
    private LexicalScope? containingLexicalScope;
    public LexicalScope ContainingLexicalScope
    {
        [DebuggerStepThrough]
        get =>
            containingLexicalScope
            ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
        [DebuggerStepThrough]
        set
        {
            if (containingLexicalScope is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
            containingLexicalScope = value;
        }
    }
    // A null name means this syntax was generated as an assumed missing name and the name is unknown
    public SimpleName? Name { get; }
    public Promise<Symbol?> ReferencedSymbol { get; } = new Promise<Symbol?>();
    IPromise<Symbol?> IVariableNameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;

    public SimpleNameExpressionSyntax(TextSpan span, SimpleName? name)
        : base(span)
    {
        Name = name;
    }

    public IEnumerable<IPromise<Symbol>> LookupInContainingScope()
    {
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        // If name is unknown, no symbols
        if (Name is null) return Enumerable.Empty<IPromise<Symbol>>();

        return containingLexicalScope.Lookup(Name);
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => Name?.ToString() ?? "⧼unknown⧽";
}
