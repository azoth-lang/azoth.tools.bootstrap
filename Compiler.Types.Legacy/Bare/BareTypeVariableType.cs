using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// The type of a type variable that is inherently bare (i.e. a <see cref="SelfType"/> or an
/// <c>AssociatedType</c>).
/// </summary>
[Closed(typeof(SelfType))]
public abstract class BareTypeVariableType : BareType
{
    public sealed override DeclaredType? DeclaredType => null;
    public sealed override IFixedList<IType> TypeArguments => [];
    public sealed override IEnumerable<GenericParameterArgument> GenericParameterArguments => [];
}
