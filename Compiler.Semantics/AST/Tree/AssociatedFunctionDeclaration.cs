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
    public new IFixedList<INamedParameter> Parameters { get; }
    public IBody Body { get; }

    public AssociatedFunctionDeclaration(
        ITypeDeclaration declaringType,
        CodeFile file,
        TextSpan span,
        FunctionSymbol symbol,
        TextSpan nameSpan,
        IFixedList<INamedParameter> parameters,
        IBody body)
        : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorOrInitializerParameter>())
    {
        Symbol = symbol;
        Parameters = parameters;
        Body = body;
        DeclaringType = declaringType;
    }

    public override string ToString()
    {
        var returnType = Symbol.Return != Return.Void ? " -> " + Symbol.Return.ToILString() : "";
        return $"fn {Symbol.Name}({string.Join(", ", Parameters)}){returnType} {Body}";
    }
}
