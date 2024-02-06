using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLend
{
    public static ImplicitLend CreateConstLend(ImplicitLend? currentLend)
        => new ImplicitLend(currentLend, CapabilityRestrictions.Write, ReferenceCapability.Constant);

    public static ImplicitLend CreateIsoLend(ImplicitLend? currentLend)
        => new ImplicitLend(currentLend, CapabilityRestrictions.ReadWrite, ReferenceCapability.Isolated);

    public ulong Number { get; }
    public CapabilityRestrictions RestrictionsImposed { get; }
    public ReferenceCapability LendAs { get; }
    public ImplicitLendFrom From { get; set; }
    public ImplicitLendTo To { get; set; }

    private ImplicitLend(ImplicitLend? currentLend, CapabilityRestrictions restrictionsImposed, ReferenceCapability lendAs)
    {
        Number = currentLend?.Number + 1 ?? 0;
        RestrictionsImposed = restrictionsImposed;
        LendAs = lendAs;
        From = new ImplicitLendFrom(this);
        To = new ImplicitLendTo(this);
    }
}
