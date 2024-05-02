using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class TraitDeclaration : TypeDeclaration<AST.ITraitMemberDeclaration>, AST.ITraitDeclaration
{
    public override IFixedList<AST.ITraitMemberDeclaration> Members { get; }

    public TraitDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<AST.ITypeDeclaration> supertypes,
        Func<AST.ITraitDeclaration, IFixedList<AST.ITraitMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan, supertypes)
    {
        Members = buildMembers(this);
    }

    public override string ToString()
        => $"trait {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
