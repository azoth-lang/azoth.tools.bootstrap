using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    /// <summary>
    /// The universal type all reference types can be converted to. A top type
    /// for reference and function types.
    /// </summary>
    /// <remarks>
    /// `Any` is "declared" mutable so that it can hold mutable references to
    /// mutable types.
    /// </remarks>
    public sealed class AnyType : ReferenceType
    {
        public AnyType(ReferenceCapability capability)
            : base(ReferenceCapability.Mutable, capability)
        {
        }

        public override bool IsKnown => true;



        public override bool Equals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is AnyType otherType
                   && Capability == otherType.Capability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecialTypeName.Any, Capability);
        }

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new AnyType(referenceCapability);
        }

        public override string ToILString()
        {
            return $"{Capability} Any";
        }

        public override string ToSourceCodeString()
        {
            return $"{Capability} Any";
        }
    }
}
