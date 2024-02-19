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
    public TypeName Name { get; }
    public Promise<TypeSymbol?> ReferencedSymbol { get; } = new Promise<TypeSymbol?>();
    public IFixedList<ITypeSyntax> TypeArguments { get; }

    public ParameterizedTypeSyntax(TextSpan span, string name, IFixedList<ITypeSyntax> typeArguments)
        : base(span)
    {
        TypeArguments = typeArguments;
        Name = StandardTypeName.Create(name, typeArguments.Count);
    }

    public IEnumerable<IPromise<TypeSymbol>> LookupInContainingScope(bool withAttributeSuffix)
    {
        if (containingLexicalScope is null)
            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

        var name = withAttributeSuffix ? Name.WithAttributeSuffix() : Name;
        if (name is null) return Enumerable.Empty<IPromise<TypeSymbol>>();

        return containingLexicalScope.Lookup(name).Select(p => p.Downcast().As<ObjectTypeSymbol>())
                                     .WhereNotNull();
    }

    public override string ToString() => $"{Name.ToBareString()}[{string.Join(", ", TypeArguments)}]";
}
