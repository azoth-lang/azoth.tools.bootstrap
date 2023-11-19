namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLendFactory
{
    private ImplicitLend? currentLend;

    public ImplicitLend CreateConstLend()
        => currentLend = ImplicitLend.CreateConstLend(currentLend);

    public ImplicitLend CreateIsoLend()
        => currentLend = ImplicitLend.CreateIsoLend(currentLend);
}
