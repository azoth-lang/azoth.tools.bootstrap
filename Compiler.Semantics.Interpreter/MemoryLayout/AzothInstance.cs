using System;
using InlineMethod;

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
        [Inline(InlineBehavior.Remove)]
        get => Layout is VTable;
    }

    public TypeLayout Layout
    {
        [Inline(InlineBehavior.Remove)]
        get => fields[0].TypeLayoutValue;
    }

    public AzothValue this[IFieldDeclarationNode field]
    {
        [Inline(InlineBehavior.Remove)]
        get => fields[Layout.GetIndex(field)];
        [Inline(InlineBehavior.Remove)]
        set => fields[Layout.GetIndex(field)] = value;
    }

    [Inline(InlineBehavior.Remove)]
    public AzothRef Ref(IFieldDeclarationNode field)
        => new(fields, Layout.GetIndex(field));
}
