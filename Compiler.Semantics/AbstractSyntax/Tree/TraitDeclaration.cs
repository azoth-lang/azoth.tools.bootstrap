using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class TraitDeclaration : TypeDeclaration<ITraitMemberDeclaration>, ITraitDeclaration
{
    public override IFixedList<ITraitMemberDeclaration> Members { get; }

    public TraitDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<ITypeDeclaration> supertypes,
        Func<ITraitDeclaration, IFixedList<ITraitMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan, supertypes)
    {
        Members = buildMembers(this);
    }

    public override string ToString()
        => $"trait {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
