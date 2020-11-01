using System;
using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

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

        // TODO referenceCapability needs to match declared mutable?
        public ObjectType(
            NamespaceName containingNamespace,
            TypeName name,
            bool declaredMutable,
            ReferenceCapability referenceCapability)
            : base(declaredMutable, referenceCapability)
        {
            ContainingNamespace = containingNamespace;
            Name = name;
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType ToMutable()
        {
            //Requires.That(nameof(DeclaredMutable), DeclaredMutable, "must be declared as a mutable type to use mutably");
            //return new ObjectType(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability.ToMutable());
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a lent isolated version of this type  for use as the constructor parameter. One issue it
        /// that it should be mutable even if the underlying type is declared immutable.
        /// </summary>
        public ObjectType ToConstructorSelf()
        {
            // TODO handle the case where the type is not declared mutable but the constructor arg allows mutate
            return new ObjectType(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability.LentIsolated);
        }

        public ObjectType ToConstructorReturn()
        {
            var constructedType = this.To(ReferenceCapability.Isolated);
            if (constructedType.DeclaredMutable) constructedType = constructedType.ToMutable();
            return constructedType;
        }

        public override string ToSourceCodeString()
        {
            var builder = new StringBuilder();
            builder.Append(ReferenceCapability);
            builder.Append(' ');
            builder.Append(ContainingNamespace);
            if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
            builder.Append(Name);
            return builder.ToString();
        }

        public override string ToILString()
        {
            var builder = new StringBuilder();
            builder.Append(ReferenceCapability);
            builder.Append(' ');
            builder.Append(ContainingNamespace);
            if (ContainingNamespace != NamespaceName.Global) builder.Append('.');
            builder.Append(Name);
            return builder.ToString();
        }

        #region Equality
        public override bool Equals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ObjectType otherType
                && ContainingNamespace == otherType.ContainingNamespace
                && Name == otherType.Name
                && DeclaredMutable == otherType.DeclaredMutable
                && ReferenceCapability == otherType.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability);
        }
        #endregion

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new ObjectType(ContainingNamespace, Name, DeclaredMutable, referenceCapability);
        }
    }
}
