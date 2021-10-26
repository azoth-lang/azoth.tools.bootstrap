using System;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    [Closed(
        typeof(ObjectType),
        typeof(AnyType))]
    public abstract class ReferenceType : DataType
    {
        public ReferenceCapability Capability { get; }
        public bool IsReadOnly => !Capability.AllowsWrite;
        public bool IsMutable => Capability.AllowsWrite;
        public bool IsMovable => Capability.IsMovable;

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

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Returns self idiom")]
        protected internal Self ToMutable_ReturnsSelf()
        {
            if (!DeclaredCapability.AllowsWrite) throw new InvalidOperationException($"Can't convert type declared {DeclaredCapability} to mutable reference");
            return To_ReturnsSelf(Capability.ToMutable());
        }

        protected internal sealed override Self ToReadable_ReturnsSelf()
        {
            return To_ReturnsSelf(Capability.ToReadOnly());
        }

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Returns self idiom")]
        protected internal abstract Self To_ReturnsSelf(ReferenceCapability referenceCapability);
    }
}
