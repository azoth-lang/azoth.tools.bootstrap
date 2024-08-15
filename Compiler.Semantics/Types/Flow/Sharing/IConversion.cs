using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal interface IConversion : IEquatable<IConversion>
{
    CapabilityRestrictions RestrictionsImposed { get; }
}
