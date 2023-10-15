using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class AssociatedFunctionDeclaration : InvocableDeclaration, IAssociatedFunctionDeclaration
{
    public ITypeDeclaration DeclaringType { get; }
    public new FunctionSymbol Symbol { get; }
    public new FixedList<INamedParameter> Parameters { get; }
    public IBody Body { get; }

    public AssociatedFunctionDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        FunctionSymbol symbol,
        TextSpan nameSpan,
        FixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorParameter>())
    {
        Symbol = symbol;
        Parameters = parameters;
        Body = body;
        DeclaringType = declaringType;
    }

    public override string ToString()
    {
        var returnType = Symbol.ReturnType != ReturnType.Void ? " -> " + Symbol.ReturnType.ToILString() : "";
        return $"fn {Symbol.Name}({string.Join(", ", Parameters)}){returnType} {Body}";
    }
}
