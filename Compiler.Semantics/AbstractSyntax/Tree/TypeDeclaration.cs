using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal abstract class TypeDeclaration<TMember> : Declaration, AST.ITypeDeclaration
    where TMember : AST.IMemberDeclaration
{
    public new UserTypeSymbol Symbol { get; }
    public IFixedList<AST.ITypeDeclaration> Supertypes { get; }
    public abstract IFixedList<TMember> Members { get; }
    IFixedList<AST.IMemberDeclaration> AST.ITypeDeclaration.Members => members.Value;
    private readonly Lazy<IFixedList<IMemberDeclaration>> members;

    public TypeDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<AST.ITypeDeclaration> supertypes)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        Supertypes = supertypes;
        // TODO not sure why SafeCast doesn't work here
        members = new(() => Members.Cast<IMemberDeclaration>().ToFixedList());
    }
}
