using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class FieldDeclaration : Declaration, IFieldDeclaration
{
    public IClassDeclaration DeclaringClass { get; }
    public new FieldSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;

    public FieldDeclaration(
        CodeFile file,
        TextSpan span,
        IClassDeclaration declaringClass,
        FieldSymbol symbol,
        TextSpan nameSpan)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        DeclaringClass = declaringClass;
    }

    public override string ToString() => Symbol + ";";
}
