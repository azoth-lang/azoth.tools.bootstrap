using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

[Closed(typeof(BareNonVariableType), typeof(BareTypeVariableType))]
public abstract class BareType
{
    public abstract DeclaredType? DeclaredType { get; }
    public abstract IFixedList<IType> GenericTypeArguments { get; }
    public abstract IEnumerable<GenericParameterArgument> GenericParameterArguments { get; }

    public abstract CapabilityType With(Capability capability);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public abstract CapabilityType WithRead();
}
