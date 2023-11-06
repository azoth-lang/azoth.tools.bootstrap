namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLend
{
    public ulong Number { get; }
    public CapabilityRestrictions RestrictionsImposed { get; }
    public ImplicitLendFrom From { get; set; }
    public ImplicitLendTo To { get; set; }

    public ImplicitLend(ImplicitLend? currentLend, CapabilityRestrictions restrictionsImposed)
    {
        Number = currentLend?.Number + 1 ?? 0;
        RestrictionsImposed = restrictionsImposed;
        From = new ImplicitLendFrom(this);
        To = new ImplicitLendTo(this);
    }
}
