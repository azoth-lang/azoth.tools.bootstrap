using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public class BoolType : SimpleType
{
    #region Singleton
    internal static readonly BoolType Instance = new();

    private BoolType()
        : base(SpecialTypeName.Bool)
    { }
    #endregion

    private protected BoolType(SpecialTypeName name)
        : base(name) { }

    public override bool IsFullyKnown => true;
}
