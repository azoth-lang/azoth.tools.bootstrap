using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReferenceEquals(AzothObject other) => ReferenceEquals(fields, other.fields);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IdentityHash() => RuntimeHelpers.GetHashCode(fields);
}
