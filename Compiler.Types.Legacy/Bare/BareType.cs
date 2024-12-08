using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

[Closed(typeof(BareNonVariableType), typeof(BareAssociatedType))]
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public abstract class BareType : IEquatable<BareType>
{
    public abstract TypeConstructor? TypeConstructor { get; }
    public abstract bool IsDeclaredConst { get; }
    public abstract TypeName Name { get; }
    public abstract bool AllowsVariance { get; }
    public abstract bool HasIndependentTypeArguments { get; }
    public abstract IFixedList<IType> TypeArguments { get; }
    public abstract IEnumerable<GenericParameterArgument> GenericParameterArguments { get; }
    public abstract IFixedSet<BareNonVariableType> Supertypes { get; }

    public abstract IType ReplaceTypeParametersIn(IType type);

    public abstract IMaybeType ReplaceTypeParametersIn(IMaybeType type);

    public abstract IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype);

    public abstract IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype);

    public abstract BareType AccessedVia(Capability capability);

    public abstract BareType With(IFixedList<IType> typeArguments);

    public abstract CapabilityType With(Capability capability);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public abstract CapabilityType WithRead();

    public abstract ConstructedOrVariablePlainType ToPlainType();

    #region Equality
    public sealed override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is BareType other && Equals(other);
    }

    public abstract bool Equals(BareType? other);

    public abstract override int GetHashCode();

    public static bool operator ==(BareType? left, BareType? right) => Equals(left, right);

    public static bool operator !=(BareType? left, BareType? right) => !Equals(left, right);
    #endregion

    public sealed override string ToString() => throw new NotSupportedException();

    public abstract string ToSourceCodeString();
    public abstract string ToILString();
}
