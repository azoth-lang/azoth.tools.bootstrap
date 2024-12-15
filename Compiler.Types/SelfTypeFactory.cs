using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public class SelfTypeFactory : AssociatedTypeFactory
{
    public override SelfPlainType PlainType { get; }
    public override AssociatedBareType BareType { get; }

    public SelfTypeFactory(SelfPlainType plainType)
    {
        PlainType = plainType;
        BareType = new(plainType);
    }
}
