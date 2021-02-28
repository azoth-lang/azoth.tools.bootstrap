using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    public class ReferenceCapability
    {
        #region Mutable
        /// <summary>
        /// A reference that owns the object and there are *no* references from
        /// the subtree out to other non-constant objects. Isolated references
        /// are always mutable. If the type is declared not mutable, then the
        /// reference capability must not be mutable (except for `self` in a
        /// constructor).
        /// </summary>
        public static readonly ReferenceCapability IsolatedMutable
            = new ReferenceCapability("iso", allowsWrite: true);

        /// <summary>
        /// A reference that can be aliased and has write access to the type
        /// </summary>
        public static readonly ReferenceCapability SharedMutable
            = new ReferenceCapability("shared mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

        public static readonly ReferenceCapability LentMutable
            = new ReferenceCapability("lent mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true, isLent: true);
        #endregion

        #region Readable
        public static readonly ReferenceCapability Isolated
            = new ReferenceCapability("iso", allowsWrite: false);

        /// <summary>
        /// A reference that can be aliased and has read-only access to the type.
        /// It may by mutable through another reference.
        /// </summary>
        public static readonly ReferenceCapability Shared
            = new ReferenceCapability("shared", allowsWriteAliases: true, allowsReadAliases: true);

        public static readonly ReferenceCapability Lent =
            new ReferenceCapability("lent", allowsWriteAliases: true, allowsReadAliases: true, isLent: true);
        #endregion

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
            = new ReferenceCapability("id", allowsWriteAliases: true, allowsReadAliases: true, allowsRead: false);

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
        /// <summary>
        /// Whether this reference is "lent" meaning that it can not be stored on the heap except
        /// in objects that are wholly lent.
        /// </summary>
        public bool IsLent { get; }

        public bool IsMovable => AllowsWrite && !AllowsWriteAliases;


        private ReferenceCapability(string value, bool allowsWrite = false, bool allowsWriteAliases = false, bool allowsRead = true, bool allowsReadAliases = false, bool isLent = false)
        {
            AllowsWrite = allowsWrite;
            AllowsWriteAliases = allowsWriteAliases;
            AllowsRead = allowsRead;
            AllowsReadAliases = allowsReadAliases;
            IsLent = isLent;
            this.value = value;
        }

        public bool IsAssignableFrom(ReferenceCapability from)
        {
            // Can't go from lent to non-lent
            if (!IsLent && from.IsLent) return false;

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
            return IsLent ? LentMutable : SharedMutable;
        }

        public ReferenceCapability ToReadable()
        {
            if (!AllowsRead) throw new InvalidOperationException($"Can't convert '{this}' to readable because it does not allow read.");
            // Already readable. Just return this. That will preserve the correct other attributes
            if (!AllowsWrite) return this;
            return IsLent ? Lent : Shared;
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
