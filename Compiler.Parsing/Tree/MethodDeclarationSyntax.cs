using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class MethodDeclarationSyntax : InvocableDeclarationSyntax, IMethodDeclarationSyntax
{
    public IClassDeclarationSyntax DeclaringClass { get; }
    public new Name Name { get; }
    public ISelfParameterSyntax SelfParameter { get; }
    public new FixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public new AcyclicPromise<MethodSymbol> Symbol { get; }

    protected MethodDeclarationSyntax(
        IClassDeclarationSyntax declaringClass,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        Name name,
        ISelfParameterSyntax selfParameter,
        FixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return)
        : base(span, file, accessModifier, nameSpan, name,
            parameters, new AcyclicPromise<MethodSymbol>())
    {
        DeclaringClass = declaringClass;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Symbol = (AcyclicPromise<MethodSymbol>)base.Symbol;
    }
}
