using System;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// Acts as a "base class" for <see cref="AzothObject"/> and <see cref="AzothValue"/>.
/// </summary>
internal readonly struct AzothInstance
{
    private readonly AzothValue[] fields;

    [Obsolete($"Must construct an {nameof(AzothObject)} or {nameof(AzothValue)} instead. ", error: true)]
    public AzothInstance(TypeLayout layout)
    {
        fields = null!;
        throw new NotSupportedException($"Must construct an {nameof(AzothObject)} or {nameof(AzothValue)} instead.");
    }

    public bool IsObject
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Layout is VTable;
    }

    public TypeLayout Layout
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => fields[0].TypeLayoutValue;
    }

    public AzothValue this[IFieldDeclarationNode field]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => fields[Layout.GetIndex(field)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => fields[Layout.GetIndex(field)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AzothRef Ref(IFieldDeclarationNode field)
        => new(fields, Layout.GetIndex(field));
}
