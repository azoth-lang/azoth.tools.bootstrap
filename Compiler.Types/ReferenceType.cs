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
        public ReferenceCapability ReferenceCapability { get; }
        public bool IsReadOnly => !DeclaredMutable && !ReferenceCapability.AllowsWrite;
        public bool IsMutable => DeclaredMutable && ReferenceCapability.AllowsWrite;
        public bool IsMovable => ReferenceCapability.IsMovable;

        public override TypeSemantics Semantics => TypeSemantics.Reference;

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`. Types
        /// not declared mutably are always immutable.
        /// </summary>
        public bool DeclaredMutable { get; }

        // TODO clarify this

        private protected ReferenceType(bool declaredMutable, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
        }

        protected internal sealed override Self ToReadOnly_ReturnsSelf()
        {
            //return To_ReturnsSelf(ReferenceCapability.ToReadOnly());
            throw new NotImplementedException();
        }

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Returns self idiom")]
        protected internal abstract Self To_ReturnsSelf(ReferenceCapability referenceCapability);
    }
}
