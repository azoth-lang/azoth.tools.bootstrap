using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class TypeDeclaration<TMember> : Declaration, ITypeDeclaration
    where TMember : IMemberDeclaration
{
    public new ObjectTypeSymbol Symbol { get; }
    public FixedList<ITypeDeclaration> Supertypes { get; }
    public abstract FixedList<TMember> Members { get; }
    FixedList<IMemberDeclaration> ITypeDeclaration.Members => members.Value;
    private readonly Lazy<FixedList<IMemberDeclaration>> members;

    public TypeDeclaration(
        CodeFile file,
        TextSpan span,
        ObjectTypeSymbol symbol,
        TextSpan nameSpan,
        FixedList<ITypeDeclaration> supertypes)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        Supertypes = supertypes;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<IMemberDeclaration>().ToFixedList());
    }
}
