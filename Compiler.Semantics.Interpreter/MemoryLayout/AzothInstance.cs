using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothInstance
{
    private readonly AzothValue[] fields;

    public AzothInstance(TypeLayout layout)
    {
        fields = layout.CreateInstanceFields();
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
