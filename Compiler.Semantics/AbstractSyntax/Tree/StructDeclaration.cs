using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class StructDeclaration : TypeDeclaration<AST.IStructMemberDeclaration>, AST.IStructDeclaration
{
    public override IFixedList<AST.IStructMemberDeclaration> Members { get; }
    public InitializerSymbol? DefaultInitializerSymbol { get; }

    public StructDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IFixedList<AST.ITypeDeclaration> supertypes,
        InitializerSymbol? defaultInitializerSymbol,
        Func<AST.IStructDeclaration, IFixedList<AST.IStructMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan, supertypes)
    {
        DefaultInitializerSymbol = defaultInitializerSymbol;
        Members = buildMembers(this);
    }

    public override string ToString() => $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
