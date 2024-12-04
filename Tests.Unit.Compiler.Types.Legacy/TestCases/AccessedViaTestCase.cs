using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.TestCases;

public record class AccessedViaTestCase(Capability Object, Capability Member, Capability Effective)
{
    public override string ToString()
        => $"{Object.ToILString()} ▷ {Member.ToILString()} = {Effective.ToILString()}";
}
