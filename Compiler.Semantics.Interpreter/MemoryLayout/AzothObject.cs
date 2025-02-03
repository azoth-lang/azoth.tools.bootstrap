using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothObject
{
    private readonly AzothValue[] fields;

    public AzothObject(VTable vTable)
    {
        fields = vTable.CreateInstanceFields();
    }

    public VTable VTable => fields[0].VTableValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReferenceEquals(AzothObject other) => ReferenceEquals(fields, other.fields);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IdentityHash() => RuntimeHelpers.GetHashCode(fields);
}
