using System;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class ClassDeclaration : TypeDeclaration<IClassMemberDeclaration>, IClassDeclaration
{
    public IClassDeclaration? BaseClass { get; }
    public override IFixedList<IClassMemberDeclaration> Members { get; }
    public ConstructorSymbol? DefaultConstructorSymbol { get; }

    public ClassDeclaration(
        CodeFile file,
        TextSpan span,
        UserTypeSymbol symbol,
        TextSpan nameSpan,
        IClassDeclaration? baseClass,
        IFixedList<ITypeDeclaration> supertypes,
        ConstructorSymbol? defaultConstructorSymbol,
        Func<IClassDeclaration, IFixedList<IClassMemberDeclaration>> buildMembers)
        : base(file, span, symbol, nameSpan, supertypes)
    {
        DefaultConstructorSymbol = defaultConstructorSymbol;
        BaseClass = baseClass;
        Members = buildMembers(this);
    }

    public override string ToString()
        => $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
}
