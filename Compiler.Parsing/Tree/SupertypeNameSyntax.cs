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
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SupertypeNameSyntax : Syntax, ISupertypeNameSyntax
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

    public TypeName Name { get; }
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public Promise<UserTypeSymbol?> ReferencedSymbol { get; } = new();
    public Promise<BareReferenceType?> NamedType { get; } = new();

    public SupertypeNameSyntax(TextSpan span, string name, IFixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        Name = StandardName.Create(name, typeArguments.Count);
        TypeArguments = typeArguments;
    }

    public IEnumerable<IPromise<UserTypeSymbol>> LookupInContainingScope()
    {
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        return containingLexicalScope.Lookup(Name).Select(p => p.Downcast().As<UserTypeSymbol>()).WhereNotNull();
    }

    public override string ToString()
    {
        if (TypeArguments.Count == 0)
            return Name.ToBareString();
        return $"{Name.ToBareString()}[{string.Join(", ", TypeArguments)}]";
    }
}
