using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

public interface IConversion : IEquatable<IConversion>
{
    CapabilityRestrictions RestrictionsImposed { get; }
}
