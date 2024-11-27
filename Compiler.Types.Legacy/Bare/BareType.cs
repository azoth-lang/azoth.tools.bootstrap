using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

[Closed(typeof(BareNonVariableType), typeof(BareTypeVariableType))]
public abstract class BareType : IEquatable<BareType>
{
    public abstract DeclaredType? DeclaredType { get; }
    public abstract TypeName Name { get; }
    public abstract IFixedList<IType> GenericTypeArguments { get; }
    public abstract IEnumerable<GenericParameterArgument> GenericParameterArguments { get; }

    public abstract CapabilityType With(Capability capability);

    /// <summary>
    /// Make a version of this type that is the default read reference capability for the type. That
    /// is either read-only or constant.
    /// </summary>
    public abstract CapabilityType WithRead();

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
