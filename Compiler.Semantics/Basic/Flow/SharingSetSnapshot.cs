using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public class SharingSetSnapshot
{
    public bool IsLent { get; }
    public IFixedSet<ISharingVariable> Variables { get; }

    public SharingSetSnapshot(bool isLent, IFixedSet<ISharingVariable> variables)
    {
        IsLent = isLent;
        Variables = variables;
    }

    public SharingSetMutable MutableCopy() => new(IsLent, Variables);

    public override string ToString()
    {
        var result = $"Count {Variables.Count}";
        if (IsLent) result += ", lent";
        return result;
    }
}
