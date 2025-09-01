using System;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// One of <see cref="ClassReference"/>, <see cref="StructReference"/>, or <see cref="ValueReference"/>.
/// </summary>
internal readonly struct InstanceReference
{
    private readonly Value[] fields;

    [Obsolete($"Must construct an {nameof(ClassReference)} or {nameof(Value)} instead. ", error: true)]
    public InstanceReference(TypeMetadata metadata)
    {
        fields = null!;
        throw new NotSupportedException($"Must construct an {nameof(ClassReference)} or {nameof(Value)} instead.");
    }

    public bool IsClassReference
    {
        [Inline(InlineBehavior.Remove)]
        get => Metadata is ClassMetadata;
    }

    public TypeMetadata Metadata
    {
        [Inline(InlineBehavior.Remove)]
        get => fields[0].TypeMetadata;
    }

    public Value this[IFieldDeclarationNode field]
    {
        [Inline(InlineBehavior.Remove)]
        get => fields[Metadata.GetIndex(field)];
        [Inline(InlineBehavior.Remove)]
        set => fields[Metadata.GetIndex(field)] = value;
    }
}
