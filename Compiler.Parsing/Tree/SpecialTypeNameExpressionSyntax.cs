using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// A name of a variable or namespace
/// </summary>
internal sealed class SpecialTypeNameExpressionSyntax : NameExpressionSyntax, ISpecialTypeNameExpressionSyntax
{
    private SymbolScope? containingLexicalScope;
    public SymbolScope ContainingLexicalScope
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
    public SpecialTypeName Name { get; }
    public override Promise<SpecialTypeNameExpressionSyntaxSemantics> Semantics { [DebuggerStepThrough] get; } = new();
    public override Promise<DataType?> DataType => Promise.Null<DataType>();
    public override Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();

    public SpecialTypeNameExpressionSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    public IEnumerable<IPromise<Symbol>> LookupInContainingScope()
    {
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        return containingLexicalScope.Lookup(Name);
    }


    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    public override string ToString() => Name.ToString();
}
