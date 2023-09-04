using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ConstructorDeclaration : InvocableDeclaration, IConstructorDeclaration
{
    public IClassDeclaration DeclaringClass { get; }
    public new ConstructorSymbol Symbol { get; }
    public ISelfParameter ImplicitSelfParameter { get; }
    public IBody Body { get; }

    public ConstructorDeclaration(
        CodeFile file,
        TextSpan span,
        IClassDeclaration declaringClass,
        ConstructorSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter implicitSelfParameter,
        FixedList<IConstructorParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters)
    {
        Symbol = symbol;
        ImplicitSelfParameter = implicitSelfParameter;
        Body = body;
        DeclaringClass = declaringClass;
    }

    public override string ToString()
    {
        var name = Symbol.Name is null ? $" {Symbol.Name}" : "";
        return $"{Symbol.ContainingSymbol}::new{name}({string.Join(", ", Parameters)})";
    }
}
