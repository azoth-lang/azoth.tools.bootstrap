using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// ReSharper disable once InconsistentNaming
public partial class TypeConstructor
{
    // TODO this seems to be a duplicate of ConstructedBareType, merge them?
    [DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
    public sealed class Supertype : IEquatable<Supertype>
    {
        // Note: must use AnyTypeConstructor.PlainType instead of IPlainType.Any to avoid circular dependency when initializing statics
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static readonly Supertype Any = new(AnyTypeConstructor.PlainType, []);
        public static readonly IFixedSet<Supertype> AnySet = Any.Yield().ToFixedSet();

        public ConstructedPlainType PlainType { get; }

        public IFixedList<IType> Arguments { get; }

        public TypeConstructor TypeConstructor => PlainType.TypeConstructor;

        public Supertype(ConstructedPlainType plainType, IFixedList<IType> arguments)
        {
            Requires.That(plainType.Arguments.SequenceEqual(arguments.Select(a => a.PlainType)),
                nameof(arguments), "Type arguments must match plain type.");
            PlainType = plainType;
            Arguments = arguments;
        }

        public static implicit operator ConstructedPlainType(Supertype supertype)
            => supertype.PlainType;

        #region Equality
        public bool Equals(Supertype? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return PlainType.Equals(other.PlainType)
                   && Arguments.Equals(other.Arguments);
        }

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is Supertype other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(PlainType, Arguments);
        #endregion

        public override string ToString() => throw new NotSupportedException();

        public string ToSourceCodeString() => ToString(t => t.ToSourceCodeString());

        public string ToILString() => ToString(t => t.ToILString());

        private string ToString(Func<IType, string> toString)
        {
            var builder = new StringBuilder();
            builder.Append(PlainType.ToBareString());
            if (!Arguments.IsEmpty)
            {
                builder.Append('[');
                builder.AppendJoin(", ", Arguments.Select(toString));
                builder.Append(']');
            }

            return builder.ToString();
        }

        public ConstructedBareType ToBareType()
            => new(PlainType, Arguments);
    }
}
