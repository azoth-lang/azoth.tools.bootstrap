using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSetSnapshot
{
    public bool IsLent { get; }
    public FixedSet<ISharingVariable> Variables { get; }
    public FixedSet<ISharingVariable> VariablesRestrictingWrite { get; }

    public SharingSetSnapshot(bool isLent, FixedSet<ISharingVariable> variables, FixedSet<ISharingVariable> variablesRestrictingWrite)
    {
        IsLent = isLent;
        Variables = variables;
        VariablesRestrictingWrite = variablesRestrictingWrite;
    }

    public SharingSet MutableCopy() => new(IsLent, Variables, VariablesRestrictingWrite);
}
