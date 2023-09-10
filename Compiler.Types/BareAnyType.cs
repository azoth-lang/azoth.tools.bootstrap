namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class BareAnyType : BareReferenceType
{
    #region Singleton
    internal static readonly BareAnyType Instance = new();

    private BareAnyType() { }
    #endregion
}
