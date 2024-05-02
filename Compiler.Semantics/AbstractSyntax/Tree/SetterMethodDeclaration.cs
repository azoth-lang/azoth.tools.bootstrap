using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq.Extensions;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class SetterMethodDeclaration : ConcreteMethodDeclaration, ISetterMethodDeclaration
{
    public SetterMethodDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, declaringType, symbol, nameSpan, selfParameter, parameters, body) { }

    public override string ToString()
        => $"set {Symbol.ContainingSymbol}::{Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}) {Body}";
}
