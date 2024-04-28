using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed class AdvancedCreateTerminalMethod : AdvancedCreateMethod
{
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public AdvancedCreateTerminalMethod(Pass pass, Rule rule) : base(pass, rule)
    {
        Requires.That(nameof(rule), rule.IsTerminal, "Must be a terminal");
        AdditionalParameters = rule.ModifiedProperties.Select(p => p.Parameter).ToFixedList();
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        yield return Pass.SimpleCreateMethods.Single(m => m.Rule == Rule);
        // TODO add calls to transforms
    }
}
