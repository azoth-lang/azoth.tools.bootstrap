using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSetSnapshot
{
    public bool IsLent { get; }
    public FixedSet<SharingVariable> Variables { get; }
    public FixedSet<SharingVariable> VariablesRestrictingWrite { get; }

    public SharingSetSnapshot(bool isLent, FixedSet<SharingVariable> variables, FixedSet<SharingVariable> variablesRestrictingWrite)
    {
        IsLent = isLent;
        Variables = variables;
        VariablesRestrictingWrite = variablesRestrictingWrite;
    }

    public SharingSet MutableCopy() => new(IsLent, Variables, VariablesRestrictingWrite);
}
