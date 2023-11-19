using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSetSnapshot
{
    public bool IsLent { get; }
    public FixedSet<ISharingVariable> Variables { get; }

    public SharingSetSnapshot(bool isLent, FixedSet<ISharingVariable> variables)
    {
        IsLent = isLent;
        Variables = variables;
    }

    public SharingSet MutableCopy() => new(IsLent, Variables);

    public override string ToString()
    {
        var result = $"Count {Variables.Count}";
        if (IsLent) result += ", lent";
        return result;
    }
}
