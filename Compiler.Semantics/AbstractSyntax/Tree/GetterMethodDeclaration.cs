using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class GetterMethodDeclaration : ConcreteMethodDeclaration, IGetterMethodDeclaration
{
    public GetterMethodDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IBody body)
        : base(file, span, declaringType, symbol, nameSpan, selfParameter, FixedList.Empty<INamedParameter>(), body) { }

    public override string ToString()
    {
        var returnType = " -> " + Symbol.Return.ToILString();
        return $"get {Symbol.ContainingSymbol}::{Symbol.Name}({SelfParameter}){returnType} {Body}";
    }
}
