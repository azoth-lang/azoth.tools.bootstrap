using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct ClassReference
{
    private readonly Value[] fields;

    public ClassReference(ClassMetadata classMetadata, BareType bareType)
    {
        fields = classMetadata.CreateInstanceFields(bareType);
    }

    public ClassMetadata ClassMetadata => fields[0].ClassMetadata;
    public BareType BareType => fields[1].BareType;

    [Inline(InlineBehavior.Remove)]
    public bool ReferenceEquals(ClassReference other) => ReferenceEquals(fields, other.fields);

    [Inline(InlineBehavior.Remove)]
    public int IdentityHash() => RuntimeHelpers.GetHashCode(fields);
}
