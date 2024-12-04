using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class EmptyType : IType
{
    internal static readonly EmptyType Never = new EmptyType(IPlainType.Never);
    internal static readonly EmptyType Void = new EmptyType(IPlainType.Void);

    public EmptyPlainType PlainType { get; }
    IPlainType IType.PlainType => PlainType;

    private EmptyType(EmptyPlainType plainType)
    {
        PlainType = plainType;
    }

    public override string ToString() => PlainType.ToString();
}
