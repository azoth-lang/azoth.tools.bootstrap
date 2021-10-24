using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    public class ReferenceCapability
    {
        /// <summary>
        /// A reference that has write access and is externally unique. That is,
        /// there are no other references into the object reachable from this
        /// reference and no references out from those objects to non-constant
        /// references.
        /// </summary>
        public static readonly ReferenceCapability Isolated
            = new ReferenceCapability("iso", allowsWrite: true);

        /// <summary>
        /// A reference that has write access and can be stored into fields etc.
        /// </summary>
        public static readonly ReferenceCapability Mutable
            = new ReferenceCapability("mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

        /// <summary>
        /// A reference that has read-only access and can be stored into fields etc.
        /// </summary>
        public static readonly ReferenceCapability ReadOnly
            = new ReferenceCapability("read-only", allowsWriteAliases: true, allowsReadAliases: true);

        /// <summary>
        /// A reference has read-only access and there are no references that
        /// can mutate this object.
        /// </summary>
        public static readonly ReferenceCapability Constant
            = new ReferenceCapability("const", allowsReadAliases: true);

        /// <summary>
        /// A reference that can be used to identify an object but not read or
        /// write to it.
        /// </summary>
        public static readonly ReferenceCapability Identity
            = new ReferenceCapability("id", allowsWriteAliases: true, allowsRead: false, allowsReadAliases: true);

        private readonly string value;
        /// <summary>
        /// Whether this kind of reference allows mutating the referenced object through this reference
        /// </summary>
        public bool AllowsWrite { get; }
        /// <summary>
        /// Whether this kind of reference permits other writable aliases to the object to exist
        /// </summary>
        public bool AllowsWriteAliases { get; }
        /// <summary>
        /// Whether this kind of reference allows reading the value of an object through this reference
        /// </summary>
        public bool AllowsRead { get; }
        /// <summary>
        /// Whether this kind of reference permits other readable aliases to the object to exist
        /// </summary>
        public bool AllowsReadAliases { get; }

        public bool IsMovable => AllowsWrite && !AllowsWriteAliases;


        private ReferenceCapability(
            string value,
            bool allowsWrite = false,
            bool allowsWriteAliases = false,
            bool allowsRead = true,
            bool allowsReadAliases = false)
        {
            AllowsWrite = allowsWrite;
            AllowsWriteAliases = allowsWriteAliases;
            AllowsRead = allowsRead;
            AllowsReadAliases = allowsReadAliases;
            this.value = value;
        }

        public bool IsAssignableFrom(ReferenceCapability from)
        {
            // Can't gain permissions
            if (AllowsWrite && !from.AllowsWrite) return false;
            if (!AllowsWriteAliases && from.AllowsWriteAliases) return false;
            if (AllowsRead && !from.AllowsRead) return false;
            if (!AllowsReadAliases && from.AllowsReadAliases) return false;

            return true;
        }

        public ReferenceCapability ToMutable()
        {
            if (!AllowsWrite) throw new InvalidOperationException($"Can't convert '{this}' to mutable because it does not allow write.");
            return Mutable;
        }

        public ReferenceCapability ToReadOnly()
        {
            if (!AllowsRead) throw new InvalidOperationException($"Can't convert '{this}' to readable because it does not allow read.");
            // Already readable. Just return this. That will preserve the correct other attributes
            if (!AllowsWrite) return this;
            return ReadOnly;
        }

        // TODO this should be can be moved?
        public bool CanBeAcquired()
        {
            return !AllowsWriteAliases;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
