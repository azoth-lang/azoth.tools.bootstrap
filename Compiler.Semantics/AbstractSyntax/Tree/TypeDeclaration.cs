using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal abstract class TypeDeclaration<TMember> : Declaration, ITypeDeclaration
    where TMember : IMemberDeclaration
{
    public new UserTypeSymbol Symbol { get; }
    public IFixedList<ITypeDeclaration> Supertypes { get; }
    public abstract IFixedList<TMember> Members { get; }
    IFixedList<IMemberDeclaration> ITypeDeclaration.Members => members.Value;
    private readonly Lazy<IFixedList<IMemberDeclaration>> members;

    public TypeDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<ITypeDeclaration> supertypes)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        Supertypes = supertypes;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<IMemberDeclaration>().ToFixedList());
    }
}
