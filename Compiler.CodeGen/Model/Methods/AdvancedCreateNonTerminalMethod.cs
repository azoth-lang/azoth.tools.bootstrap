using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed class AdvancedCreateNonTerminalMethod : AdvancedCreateMethod
{
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }
    public override IFixedSet<Method> CallsMethods => callsMethods.Value;
    private readonly Lazy<IFixedSet<Method>> callsMethods;

    public AdvancedCreateNonTerminalMethod(Pass pass, Rule rule)
        : base(pass, rule)
    {
        Requires.That(nameof(rule), !rule.IsTerminal, "Must be a non-terminal");
        // Start with an empty list of additional parameters and to bubble up needed parameters
        AdditionalParameters = FixedList.Empty<Parameter>();
        AllParameters = From.Yield().ToFixedList();
        callsMethods = new(() => GetCallsMethods().ToFixedSet());
    }

    private IEnumerable<Method> GetCallsMethods()
        => Rule.ChildRules.Select(r => Pass.AdvancedCreateMethods.Single(m => m.Rule == r));
}
