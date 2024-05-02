using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class StructDeclaration : TypeDeclaration<IStructMemberDeclaration>, IStructDeclaration
{
    public override IFixedList<IStructMemberDeclaration> Members { get; }
    public InitializerSymbol? DefaultInitializerSymbol { get; }

    public StructDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<ITypeDeclaration> supertypes,
        InitializerSymbol? defaultInitializerSymbol,
        Func<IStructDeclaration, IFixedList<IStructMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan, supertypes)
    {
        DefaultInitializerSymbol = defaultInitializerSymbol;
        Members = buildMembers(this);
    }

    public override string ToString() => $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
