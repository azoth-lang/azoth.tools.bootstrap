using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class InitializerDeclaration : InvocableDeclaration, IInitializerDeclaration
{
    public IStructDeclaration DeclaringType { get; }
    ITypeDeclaration IMemberDeclaration.DeclaringType => DeclaringType;
    public new InitializerSymbol Symbol { get; }
    public ISelfParameter SelfParameter { get; }
    public IBody Body { get; }

    public InitializerDeclaration(
        CodeFile file,
        TextSpan span,
        IStructDeclaration declaringStruct,
        InitializerSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<IConstructorOrInitializerParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters)
    {
        Symbol = symbol;
        SelfParameter = selfParameter;
        Body = body;
        DeclaringType = declaringStruct;
    }

    public override string ToString()
    {
        var name = Symbol.Name is null ? $" {Symbol.Name}" : "";
        var parameters = string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter));
        return $"{Symbol.ContainingSymbol}::init{name}({parameters}) {Body}";
    }
}
