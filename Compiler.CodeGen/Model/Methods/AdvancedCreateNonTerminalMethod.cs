using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed record AdvancedCreateNonTerminalMethod : AdvancedCreateMethod
{
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }
    public IFixedList<Parameter> AllParameters { get; }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AdvancedCreateNonTerminalMethod(Pass pass, Rule rule)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(pass, rule)
    {
        Requires.That(nameof(rule), !rule.IsTerminal, "Must be a non-terminal");
        // Start with an empty list of additional parameters and to bubble up needed parameters
        AdditionalParameters = FixedList.Empty<Parameter>();
        AllParameters = From.Yield().ToFixedList();
    }

    public override IEnumerable<Method> GetMethodsCalled()
        => ToRule.DerivedRules.Select(GetCreateMethodCalled);
}
