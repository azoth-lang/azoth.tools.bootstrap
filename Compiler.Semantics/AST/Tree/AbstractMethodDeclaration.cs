using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class AbstractMethodDeclaration : InvocableDeclaration, IAbstractMethodDeclaration
{
    public ITypeDeclaration DeclaringType { get; }
    public new MethodSymbol Symbol { get; }
    public ISelfParameter SelfParameter { get; }
    public new IFixedList<INamedParameter> Parameters { get; }

    public AbstractMethodDeclaration(
        CodeFile file,
        TextSpan span,
        ITypeDeclaration declaringType,
        MethodSymbol symbol,
        TextSpan nameSpan,
        ISelfParameter selfParameter,
        FixedList<INamedParameter> parameters)
        : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorParameter>())
    {
        Symbol = symbol;
        SelfParameter = selfParameter;
        Parameters = parameters;
        DeclaringType = declaringType;
    }

    public override string ToString()
    {
        var returnType = Symbol.ReturnType != ReturnType.Void ? " -> " + Symbol.ReturnType.ToILString() : "";
        return $"fn {Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}){returnType};";
    }
}
