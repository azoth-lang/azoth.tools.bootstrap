using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols
{
    public sealed class ConstructorSymbol : InvocableSymbol
    {
        public new ObjectTypeSymbol ContainingSymbol { get; }
        public new ObjectType ReturnDataType { get; }

        public ConstructorSymbol(
            ObjectTypeSymbol containingSymbol,
            Name? name,
            FixedList<DataType> parameterDataTypes)
            : base(containingSymbol, name, parameterDataTypes,
                containingSymbol.DeclaresDataType.ToConstructorReturn())
        {
            ContainingSymbol = containingSymbol;
            ReturnDataType = containingSymbol.DeclaresDataType.ToConstructorReturn();
        }

        public static ConstructorSymbol CreateDefault(ObjectTypeSymbol containingSymbol)
            => new(containingSymbol, null, FixedList<DataType>.Empty);

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ConstructorSymbol otherConstructor
                   && ContainingSymbol == otherConstructor.ContainingSymbol
                   && Name == otherConstructor.Name
                   && ParameterDataTypes.SequenceEqual(otherConstructor.ParameterDataTypes);
        }

        public override int GetHashCode()
            => HashCode.Combine(ContainingSymbol, Name, ParameterDataTypes);

        public override string ToILString()
        {
            var name = Name is null ? $" {Name}" : "";
            return $"{ContainingSymbol}::new{name}({string.Join(", ", ParameterDataTypes.Select(d => d.ToILString()))})";
        }
    }
}
