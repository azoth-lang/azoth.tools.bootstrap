using System.Linq;
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
    public ITypeDeclarationSyntax DeclaringType { get; }
    public abstract MethodKind Kind { get; }
    public new SimpleName Name { get; }
    public IMethodSelfParameterSyntax SelfParameter { get; }
    public new IFixedList<INamedParameterSyntax> Parameters { get; }
    public override IFixedList<IParameterSyntax> AllParameters { get; }
    public abstract IReturnSyntax? Return { get; }
    public new AcyclicPromise<MethodSymbol> Symbol { get; }

    protected MethodDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters)
        : base(span, file, accessModifier, nameSpan, name,
            parameters, new AcyclicPromise<MethodSymbol>())
    {
        DeclaringType = declaringType;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        AllParameters = parameters.Prepend<IParameterSyntax>(selfParameter).ToFixedList();
        Symbol = (AcyclicPromise<MethodSymbol>)base.Symbol;
    }
}
