using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class InvocableDeclarationSyntax : DeclarationSyntax, IInvocableDeclarationSyntax
{
    public IAccessModifierToken? AccessModifier { get; }
    public IFixedList<IConstructorParameterSyntax> Parameters { get; }
    public new IPromise<InvocableSymbol> Symbol { get; }

    protected InvocableDeclarationSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName? name,
        IEnumerable<IConstructorParameterSyntax> parameters,
        IPromise<InvocableSymbol> symbol)
        : base(span, file, name, nameSpan, symbol)
    {
        AccessModifier = accessModifier;
        Parameters = parameters.ToFixedList();
        Symbol = symbol;
    }
}
