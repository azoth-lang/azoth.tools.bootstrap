using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ConstructorDeclaration : InvocableDeclaration, IConstructorDeclaration
{
    public IClassDeclaration DeclaringType { get; }
    ITypeDeclaration IMemberDeclaration.DeclaringType => DeclaringType;
    public new ConstructorSymbol Symbol { get; }
    public ISelfParameter SelfParameter { get; }
    public IBody Body { get; }

    public ConstructorDeclaration(
        CodeFile file,
        TextSpan span,
        IClassDeclaration declaringClass,
        ConstructorSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<IConstructorParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters)
    {
        Symbol = symbol;
        SelfParameter = selfParameter;
        Body = body;
        DeclaringType = declaringClass;
    }

    public override string ToString()
    {
        var name = Symbol.Name is null ? $" {Symbol.Name}" : "";
        var parameters = string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter));
        return $"{Symbol.ContainingSymbol}::new{name}({parameters}) {Body}";
    }
}
