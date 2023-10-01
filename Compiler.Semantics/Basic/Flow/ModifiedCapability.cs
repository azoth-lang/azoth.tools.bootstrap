using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public readonly record struct ModifiedCapability(ReferenceCapability BaseCapability, bool IsWriteRestricted)
{
    public ReferenceCapability CurrentCapability
    {
        get
        {
            if (IsWriteRestricted) return BaseCapability.WithoutWrite();
            return BaseCapability;
        }
    }

    public static implicit operator ModifiedCapability(ReferenceCapability capability)
        => new(capability, false);

    public ModifiedCapability Alias() => this with { BaseCapability = BaseCapability.Alias() };

    public ModifiedCapability Freeze() => this with { BaseCapability = BaseCapability.Freeze() };

    public ModifiedCapability RestrictWrite() => this with { IsWriteRestricted = true };

    public ModifiedCapability RemoveWriteRestriction() => this with { IsWriteRestricted = false };
}
