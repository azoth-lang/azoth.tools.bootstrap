using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class FunctionDeclaration : InvocableDeclaration, IFunctionDeclaration
{
    public IFixedList<IAttribute> Attributes { get; }
    public new FunctionSymbol Symbol { get; }
    public new IFixedList<INamedParameter> Parameters { get; }
    public IBody Body { get; }

    public FunctionDeclaration(
        CodeFile file,
        TextSpan span,
        IFixedList<IAttribute> attributes,
        FunctionSymbol symbol,
        TextSpan nameSpan,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorOrInitializerParameter>())
    {
        Symbol = symbol;
        Parameters = parameters;
        Body = body;
        Attributes = attributes;
    }

    public override string ToString()
    {
        var returnType = Symbol.Return != Return.Void ? " -> " + Symbol.Return.ToILString() : "";
        return $"fn {Symbol.ContainingSymbol}.{Symbol.Name}({string.Join(", ", Parameters)}){returnType} {Body}";
    }
}
