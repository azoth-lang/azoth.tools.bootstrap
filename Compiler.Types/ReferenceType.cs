using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    [Closed(
        typeof(ObjectType),
        typeof(AnyType))]
    public abstract class ReferenceType : DataType
    {
        public ReferenceCapability Capability { get; }
        public bool IsReadOnlyReference => !Capability.AllowsWrite;
        public bool IsMutableReference => Capability.AllowsWrite;
        public bool IsMovableReference => Capability.IsMovable;
        public bool IsConstReference => Capability == ReferenceCapability.Constant;

        public override TypeSemantics Semantics => TypeSemantics.Reference;

        /// <summary>
        /// Whether this type was declared `mut class`, `const class` or just `class`.
        /// </summary>
        public ReferenceCapability DeclaredCapability { get; }

        // TODO clarify this

        private protected ReferenceType(
            ReferenceCapability declaredCapability,
            ReferenceCapability capability)
        {
            DeclaredCapability = declaredCapability;
            Capability = capability;
        }

        public ReferenceType ToMutable()
        {
            if (!DeclaredCapability.AllowsWrite) throw new InvalidOperationException($"Can't convert type declared {DeclaredCapability} to mutable reference");
            return To(Capability.ToMutable());
        }

        public override ReferenceType ToReadOnly()
        {
            return To(Capability.ToReadOnly());
        }

        /// <summary>
        /// Return the same type except with the given reference capability
        /// </summary>
        public abstract ReferenceType To(ReferenceCapability referenceCapability);
    }
}
