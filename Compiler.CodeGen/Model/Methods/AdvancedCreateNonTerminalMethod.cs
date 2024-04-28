using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed class AdvancedCreateNonTerminalMethod : AdvancedCreateMethod
{
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public AdvancedCreateNonTerminalMethod(Pass pass, Rule rule)
        : base(pass, rule)
    {
        Requires.That(nameof(rule), !rule.IsTerminal, "Must be a non-terminal");
        // Start with an empty list of additional parameters and to bubble up needed parameters
        AdditionalParameters = FixedList.Empty<Parameter>();
        AllParameters = From.Yield().ToFixedList();
    }

    public override IEnumerable<Method> GetMethodsCalled()
        => Rule.ChildRules.Select(r => Pass.AdvancedCreateMethods.Single(m => m.Rule == r));
}
