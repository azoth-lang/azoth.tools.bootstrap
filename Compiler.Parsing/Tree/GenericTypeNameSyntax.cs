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
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class GenericTypeNameSyntax : TypeSyntax, IGenericTypeNameSyntax
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
    public GenericName Name { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public BareReferenceType? NamedBareType => (NamedType as ReferenceType)?.BareType;

    public GenericTypeNameSyntax(TextSpan span, string name, IFixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        TypeArguments = typeArguments;
        Name = new GenericName(name, typeArguments.Count);
    }

    public IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope(bool withAttributeSuffix)
    {
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        var name = withAttributeSuffix ? Name.WithAttributeSuffix() : Name;

        return containingLexicalScope.Lookup(name).Select(p => p.Downcast().As<UserTypeSymbol>())
                                     .WhereNotNull();
    }

    public override string ToString() => $"{Name.ToBareString()}[{string.Join(", ", TypeArguments)}]";
}
