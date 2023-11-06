namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLendFactory
{
    private ImplicitLend? currentLend;

    public ImplicitLend CreateConstLend()
        => currentLend = new ImplicitLend(currentLend, CapabilityRestrictions.Write);

    public ImplicitLend CreateIsoLend()
        => currentLend = new ImplicitLend(currentLend, CapabilityRestrictions.ReadWrite);
}
