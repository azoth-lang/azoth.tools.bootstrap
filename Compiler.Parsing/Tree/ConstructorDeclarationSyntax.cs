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
    public IClassDeclarationSyntax DeclaringClass { get; }
    public ISelfParameterSyntax SelfParameter { get; }
    public new FixedList<IConstructorParameterSyntax> Parameters { get; }
    public virtual IBodySyntax Body { get; }
    public new AcyclicPromise<ConstructorSymbol> Symbol { get; }

    public ConstructorDeclarationSyntax(
        IClassDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        Name? name,
        ISelfParameterSyntax selfParameter,
        FixedList<IConstructorParameterSyntax> parameters,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters,
            new AcyclicPromise<ConstructorSymbol>())
    {
        DeclaringClass = declaringType;
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
