using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ConstructorDeclarationSyntax : InvocableDeclarationSyntax, IConstructorDeclarationSyntax
{
    public IClassDeclarationSyntax DeclaringType { get; }
    public new SimpleName? Name { get; }
    ITypeDeclarationSyntax IMemberDeclarationSyntax.DeclaringType => DeclaringType;
    public IConstructorSelfParameterSyntax SelfParameter { get; }
    public new IFixedList<IConstructorParameterSyntax> Parameters { get; }
    public virtual IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDeclarationSyntax.Body => Body;
    public new AcyclicPromise<ConstructorSymbol> Symbol { get; }

    public ConstructorDeclarationSyntax(
        IClassDeclarationSyntax declaringClass,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName? name,
        IConstructorSelfParameterSyntax selfParameter,
        FixedList<IConstructorParameterSyntax> parameters,
        IBlockBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters,
            new AcyclicPromise<ConstructorSymbol>())
    {
        DeclaringType = declaringClass;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
        Symbol = (AcyclicPromise<ConstructorSymbol>)base.Symbol;
    }

    public override string ToString()
    {
        var parameters = string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter));
        return Name is null ? $"new({parameters})" : $"new {Name}({parameters})";
    }
}
