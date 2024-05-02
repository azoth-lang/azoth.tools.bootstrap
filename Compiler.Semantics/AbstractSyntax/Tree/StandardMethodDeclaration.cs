using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq.Extensions;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class StandardMethodDeclaration : ConcreteMethodDeclaration, AST.IStandardMethodDeclaration
{
    public StandardMethodDeclaration(
        CodeFile file,
        TextSpan span,
        AST.ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, declaringType, symbol, nameSpan, selfParameter, parameters, body)
    {
    }

    public override string ToString()
    {
        var returnType = Symbol.Return != Return.Void ? " -> " + Symbol.Return.ToILString() : "";
        return
            $"fn {Symbol.ContainingSymbol}::{Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}){returnType} {Body}";
    }
}
