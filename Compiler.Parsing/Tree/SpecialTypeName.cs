using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

/// <summary>
/// The unqualified name of a type.
/// </summary>
internal sealed class SpecialTypeNameSyntax : TypeSyntax, ISpecialTypeNameSyntax
{
    private SymbolScope? containingLexicalScope;
    public SymbolScope ContainingLexicalScope
    {
        [DebuggerStepThrough]
        get => containingLexicalScope
               ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
        [DebuggerStepThrough]
        set
        {
            if (containingLexicalScope is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
            containingLexicalScope = value;
        }
    }
    public SpecialTypeName Name { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();

    public SpecialTypeNameSyntax(TextSpan span, SpecialTypeName name)
        : base(span)
    {
        Name = name;
    }

    public IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope(bool withAttributeSuffix)
    {
        if (withAttributeSuffix) throw new NotSupportedException("Cannot use special type name as attribute");
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        return containingLexicalScope.Lookup(Name).Select(p => p.Downcast().As<TypeSymbol>()).WhereNotNull();
    }

    public override string ToString() => Name.ToString();
}
