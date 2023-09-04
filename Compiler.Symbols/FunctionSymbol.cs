using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols
{
    /// <summary>
    /// A symbol for a function
    /// </summary>
    public sealed class FunctionSymbol : FunctionOrMethodSymbol
    {
        public new Name Name { get; }

        public FunctionSymbol(
            Symbol containingSymbol,
            Name name,
            FixedList<DataType> parameterDataTypes,
            DataType returnDataType)
            : base(containingSymbol, name, parameterDataTypes, returnDataType)
        {
            Name = name;
        }

        public FunctionSymbol(
            Symbol containingSymbol,
            Name name,
            FixedList<DataType> parameterDataTypes)
            : this(containingSymbol, name, parameterDataTypes, DataType.Void)
        {
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is FunctionSymbol otherFunction
                   && ContainingSymbol == otherFunction.ContainingSymbol
                   && Name == otherFunction.Name
                   && ParameterDataTypes.SequenceEqual(otherFunction.ParameterDataTypes)
                   && ReturnDataType == otherFunction.ReturnDataType;
        }

        public override int GetHashCode()
            => HashCode.Combine(ContainingSymbol, Name, ParameterDataTypes, ReturnDataType);

        public override string ToILString()
            => $"{ContainingSymbol.ToILString()}.{Name}({string.Join(", ", ParameterDataTypes.Select(d => d.ToILString()))}) -> {ReturnDataType.ToILString()}";
    }
}
