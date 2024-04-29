using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

/// <summary>
/// An advanced create method is one that will call `Transform` on child nodes.
/// </summary>
[Closed(
    typeof(AdvancedCreateTerminalMethod),
    typeof(AdvancedCreateNonTerminalMethod))]
public abstract record AdvancedCreateMethod : CreateMethod
{
    public abstract IFixedList<Parameter> AllParameters { get; }

    private protected AdvancedCreateMethod(Pass pass, Rule toRule)
        : base(pass, toRule)
    {
    }
}
