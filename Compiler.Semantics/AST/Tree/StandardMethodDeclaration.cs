using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal sealed class StandardMethodDeclaration : ConcreteMethodDeclaration, IStandardMethodDeclaration
{
    public StandardMethodDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, declaringType, symbol, nameSpan, selfParameter, parameters, body)
    {
    }
}
