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

internal class ParameterizedTypeSyntax : TypeSyntax, IParameterizedTypeSyntax
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
    public Name Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();
    public FixedList<ITypeSyntax> TypeArguments { get; }

    public ParameterizedTypeSyntax(TextSpan span, Name name, FixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        TypeArguments = typeArguments;
        Name = name;
    }

    public IEnumerable<TypeSymbol> LookupInContainingScope()
    {
        if (containingLexicalScope is not null)
            return containingLexicalScope.Lookup(Name).Select(p => p.As<ObjectTypeSymbol>())
                .WhereNotNull().Select(p => p.Result)
                .Where(s => s.DeclaresType.GenericParameters.Count == TypeArguments.Count);

        throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");
    }

    public override string ToString() => $"{Name}[{string.Join(", ", TypeArguments)}]";
}