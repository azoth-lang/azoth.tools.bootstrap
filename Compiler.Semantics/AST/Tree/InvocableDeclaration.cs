using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class InvocableDeclaration : Declaration, IInvocableDeclaration
{
    public new InvocableSymbol Symbol { get; }
    public IFixedList<IConstructorOrInitializerParameter> Parameters { get; }

    protected InvocableDeclaration(
        CodeFile file,
        TextSpan span,
        InvocableSymbol symbol,
        TextSpan nameSpan,
        IFixedList<IConstructorOrInitializerParameter> parameters)
        : base(file, span, symbol, nameSpan)
    {
        Symbol = symbol;
        Parameters = parameters;
    }
}
