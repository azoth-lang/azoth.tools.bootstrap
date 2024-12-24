using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this should probably be removed. It seems to be used only for improperly handling operators yielding these types
[Closed(
    typeof(SimpleTypeConstructor),
    typeof(LiteralTypeConstructor))]
public abstract class SimpleOrLiteralTypeConstructor : TypeConstructor
{
    public override BuiltInContext Context => BuiltInContext.Instance;

    public override bool IsDeclaredConst => true;

    public override bool CanBeSupertype => false;

    public abstract ConstructedPlainType PlainType { [DebuggerStepThrough] get; }

    public CapabilityType Type
        => Lazy.Initialize(ref type, PlainType,
            static plainType => CapabilityType.Create(Capability.Constant, plainType));

    private CapabilityType? type;
}
