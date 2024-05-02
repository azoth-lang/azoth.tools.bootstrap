using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class FieldDeclaration : Declaration, IFieldDeclaration
{
    public IClassOrStructDeclaration DeclaringType { get; }
    AST.ITypeDeclaration IMemberDeclaration.DeclaringType => DeclaringType;
    public new FieldSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;

    public FieldDeclaration(
        CodeFile file,
        TextSpan span,
        IClassOrStructDeclaration declaringType,
        FieldSymbol symbol,
        TextSpan nameSpan)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        DeclaringType = declaringType;
    }

    public override string ToString() => Symbol + ";";
}
