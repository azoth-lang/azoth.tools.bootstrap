using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ClassDeclaration : TypeDeclaration<IClassMemberDeclaration>, IClassDeclaration
{
    public override FixedList<IClassMemberDeclaration> Members { get; }
    public ConstructorSymbol? DefaultConstructorSymbol { get; }

    public ClassDeclaration(
        CodeFile file,
        TextSpan span,
        ObjectTypeSymbol symbol,
        TextSpan nameSpan,
        ConstructorSymbol? defaultConstructorSymbol,
        Func<IClassDeclaration, FixedList<IClassMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan)
    {
        DefaultConstructorSymbol = defaultConstructorSymbol;
        Members = buildMembers(this);
    }

    public override string ToString()
        => $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
