using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq.Extensions;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class ConcreteMethodDeclaration : InvocableDeclaration, IConcreteMethodDeclaration
{
    public ITypeDeclaration DeclaringType { get; }
    public new MethodSymbol Symbol { get; }
    public ISelfParameter SelfParameter { get; }
    public new IFixedList<INamedParameter> Parameters { get; }
    public IBody Body { get; }

    protected ConcreteMethodDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan,
            parameters.ToFixedList<IConstructorOrInitializerParameter>())
    {
        Symbol = symbol;
        Parameters = parameters;
        SelfParameter = selfParameter;
        Body = body;
        DeclaringType = declaringType;
    }

    public override string ToString()
    {
        var returnType = Symbol.Return != Return.Void ? " -> " + Symbol.Return.ToILString() : "";
        return $"fn {Symbol.ContainingSymbol}::{Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}){returnType} {Body}";
    }
}
