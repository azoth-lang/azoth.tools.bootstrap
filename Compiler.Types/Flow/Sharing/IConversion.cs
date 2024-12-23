namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

public interface IConversion : IEquatable<IConversion>
{
    CapabilityRestrictions RestrictionsImposed { get; }
}
