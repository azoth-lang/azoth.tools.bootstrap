using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothObject
{
    private readonly AzothValue[] fields;

    public AzothObject(VTable vTable, BareType bareType)
    {
        fields = vTable.CreateInstanceFields(bareType);
    }

    public VTable VTable => fields[0].VTableValue;
    public BareType BareType => fields[1].BareTypeValue;

    [Inline(InlineBehavior.Remove)]
    public bool ReferenceEquals(AzothObject other) => ReferenceEquals(fields, other.fields);

    [Inline(InlineBehavior.Remove)]
    public int IdentityHash() => RuntimeHelpers.GetHashCode(fields);
}
