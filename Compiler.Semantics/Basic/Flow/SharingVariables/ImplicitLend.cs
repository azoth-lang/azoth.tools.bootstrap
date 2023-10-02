namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class ImplicitLend
{
    public ulong Number { get; }
    public bool RestrictWrite { get; }
    public ISharingVariable From { get; set; }
    public ISharingVariable To { get; set; }

    public ImplicitLend(ImplicitLend? currentLend, bool restrictWrite)
    {
        Number = currentLend?.Number + 1 ?? 0;
        RestrictWrite = restrictWrite;
        From = new ImplicitLendFrom(this);
        To = new ImplicitLendTo(this);
    }
}
