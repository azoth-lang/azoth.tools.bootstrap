using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AssociatedFunctionDeclarationSyntax : InvocableDeclarationSyntax, IAssociatedFunctionDeclarationSyntax
{
    public ITypeDeclarationSyntax DeclaringType { get; }
    public new Name Name { get; }
    public new FixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }
    public new AcyclicPromise<FunctionSymbol> Symbol { get; }

    public AssociatedFunctionDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        Name name,
        FixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters, new AcyclicPromise<FunctionSymbol>())
    {
        DeclaringType = declaringType;
        Name = name;
        Parameters = parameters;
        Body = body;
        Return = @return;
        Symbol = (AcyclicPromise<FunctionSymbol>)base.Symbol;
    }

    public override string ToString()
    {
        var returnType = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters)}){returnType} {Body}";
    }
}
