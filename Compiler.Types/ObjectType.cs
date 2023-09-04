using System;
using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types
{
    /// <summary>
    /// Object types are the types created with class and trait declarations. An
    /// object type may have generic parameters that may be filled with generic
    /// arguments. An object type with generic parameters but no generic arguments
    /// is an *unbound type*. One with generic arguments supplied for all
    /// parameters is *a constructed type*. One with some but not all arguments
    /// supplied is *partially constructed type*.
    /// </summary>
    /// <remarks>
    /// There will be two special object types `Type` and `Metatype`
    /// </remarks>
    public sealed class ObjectType : ReferenceType
    {
        // TODO this needs a containing package
        public NamespaceName ContainingNamespace { get; }
        public TypeName Name { get; }
        public override bool IsKnown { [DebuggerStepThrough] get => true; }

        /// <summary>
        /// Create a object type for a given class or trait
        /// </summary>
        public static ObjectType Create(
            NamespaceName containingNamespace,
            TypeName name,
            ReferenceCapability declaredCapability)
            // The "root" of the reference capability tree for this type
            => new(containingNamespace, name, declaredCapability, declaredCapability);

        /// <summary>
        /// Create an object type with the given reference capability, enforcing that it be consistent
        /// with the whether the type was declared mutable
        /// </summary>
        /// <returns></returns>
        internal static ObjectType Create(
            NamespaceName containingNamespace,
            TypeName name,
            ReferenceCapability declaredCapability,
            ReferenceCapability referenceCapability)
        {
            if (!declaredCapability.AllowsWrite && referenceCapability.AllowsWrite)
                throw new ArgumentException($"Capability {referenceCapability} not supported for types declared {declaredCapability}");
            return new ObjectType(containingNamespace, name, declaredCapability, referenceCapability);
        }

        private ObjectType(
            NamespaceName containingNamespace,
            TypeName name,
            ReferenceCapability declaredCapability,
            ReferenceCapability capability)
            : base(declaredCapability, capability)
        {
            ContainingNamespace = containingNamespace;
            Name = name;
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        //public ObjectType ToMutable()
        //{
        //Requires.That(nameof(DeclaredMutable), DeclaredMutable, "must be declared as a mutable type to use mutably");
        //return new ObjectType(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability.ToMutable());
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Make a version of this type for use as the constructor parameter. One issue is
        /// that it should be mutable even if the underlying type is declared immutable.
        /// </summary>
        public ObjectType ToConstructorSelf()
            // TODO handle the case where the type is not declared mutable but the constructor arg allows mutate
            => new(ContainingNamespace, Name, DeclaredCapability, ReferenceCapability.Mutable);

        public ObjectType ToConstructorReturn() => To(DeclaredCapability);

        public override string ToSourceCodeString()
        {
            var builder = new StringBuilder();
            if (Capability != ReferenceCapability.ReadOnly)
            {
                builder.Append(Capability.ToSourceString());
                builder.Append(' ');
            }
            builder.Append(ContainingNamespace);
            if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
            builder.Append(Name);
            return builder.ToString();
        }

        public override string ToILString()
        {
            var builder = new StringBuilder();
            builder.Append(Capability.ToILString());
            builder.Append(' ');
            builder.Append(ContainingNamespace);
            if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
            builder.Append(Name);
            return builder.ToString();
        }

        public bool DeclaredTypesEquals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ObjectType otherType
                 && ContainingNamespace == otherType.ContainingNamespace
                 && Name == otherType.Name
                 && DeclaredCapability == otherType.DeclaredCapability;
        }

        #region Equality
        public override bool Equals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ObjectType otherType
                && ContainingNamespace == otherType.ContainingNamespace
                && Name == otherType.Name
                && DeclaredCapability == otherType.DeclaredCapability
                && Capability == otherType.Capability;
        }

        public override int GetHashCode()
            => HashCode.Combine(ContainingNamespace, Name, DeclaredCapability, Capability);

        #endregion

        public override ObjectType To(ReferenceCapability referenceCapability)
            => new(ContainingNamespace, Name, DeclaredCapability, referenceCapability);

        public override ObjectType ToReadOnly() => (ObjectType)base.ToReadOnly();
    }
}
