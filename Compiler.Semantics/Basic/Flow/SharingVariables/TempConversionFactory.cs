namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

public class TempConversionFactory
{
    private TempConversion? currentConversion;

    public TempConversion CreateTempFreeze()
        => currentConversion = TempConversion.CreateTempFreeze(currentConversion);

    public TempConversion CreateTempMove()
        => currentConversion = TempConversion.CreateTempMove(currentConversion);
}
