using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class InitializerDeclarationSyntax : InvocableDeclarationSyntax, IInitializerDeclarationSyntax
{
    public IStructDeclarationSyntax DeclaringType { get; }
    public new SimpleName? Name { get; }
    ITypeDeclarationSyntax IMemberDeclarationSyntax.DeclaringType => DeclaringType;
    public IInitializerSelfParameterSyntax SelfParameter { get; }
    public new IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    public override IFixedList<IParameterSyntax> AllParameters { get; }
    public IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDeclarationSyntax.Body => Body;
    public new AcyclicPromise<InitializerSymbol> Symbol { get; }

    public InitializerDeclarationSyntax(
        IStructDeclarationSyntax declaringStruct,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName? name,
        IInitializerSelfParameterSyntax selfParameter,
        IFixedList<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters, new AcyclicPromise<InitializerSymbol>())
    {
        DeclaringType = declaringStruct;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        AllParameters = parameters.Prepend<IParameterSyntax>(selfParameter).ToFixedList();
        Body = body;
        Symbol = (AcyclicPromise<InitializerSymbol>)base.Symbol;
    }

    public override string ToString()
    {
        var parameters = string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter));
        return Name is null ? $"init({parameters})" : $"init {Name}({parameters})";
    }
}
