namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLendFactory
{
    private ImplicitLend? currentLend;

    public ImplicitLend Create(bool restrictWrite)
        => currentLend = new ImplicitLend(currentLend, restrictWrite);
}
