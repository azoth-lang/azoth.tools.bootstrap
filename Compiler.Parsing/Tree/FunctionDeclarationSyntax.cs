using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class FunctionDeclarationSyntax : InvocableNonMemberEntityDeclarationSyntax, IFunctionDeclarationSyntax
{
    public new SimpleName Name { get; }
    Name INonMemberEntityDeclarationSyntax.Name => Name;
    public new FixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public new AcyclicPromise<FunctionSymbol> Symbol { get; }

    public FunctionDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName name,
        FixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        : base(containingNamespaceName, span, file, accessModifier, nameSpan, name, parameters, new AcyclicPromise<FunctionSymbol>())
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        Return = @return;
        Symbol = (AcyclicPromise<FunctionSymbol>)base.Symbol;
    }

    public override string ToString()
    {
        var @return = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters)}){@return} {Body}";
    }
}
