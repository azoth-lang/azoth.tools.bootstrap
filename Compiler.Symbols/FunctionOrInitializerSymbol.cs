using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

/// <summary>
/// A symbol for a function or initializer.
/// </summary>
/// <remarks>Since a call to an initializer is syntactically indistinguishable from from a function
/// call, these two types of symbols must often be dealt with together.</remarks>
[Closed(typeof(FunctionSymbol), typeof(InitializerSymbol))]
public abstract class FunctionOrInitializerSymbol : InvocableSymbol
{
    public override IdentifierName Name { get; }

    protected FunctionOrInitializerSymbol(
        IdentifierName name,
        IFixedList<Parameter> parameters,
        Return @return)
        : base(parameters, @return)
    {
        Name = name;
    }
}
