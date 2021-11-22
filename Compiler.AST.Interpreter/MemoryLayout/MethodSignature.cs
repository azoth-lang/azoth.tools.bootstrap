using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal class MethodSignature : IEquatable<MethodSignature>
    {
        public Name Name { get; }
        public DataType SelfDataType { get; }
        public FixedList<DataType> ParameterDataTypes { get; }
        public DataType ReturnDataType { get; }
        private readonly int hashCode;

        public MethodSignature(
            Name name,
            DataType selfDataType,
            FixedList<DataType> parameterDataTypes,
            DataType returnDataType)
        {
            Name = name;
            SelfDataType = selfDataType;
            ParameterDataTypes = parameterDataTypes;
            ReturnDataType = returnDataType;
            hashCode = HashCode.Combine(Name, SelfDataType, ParameterDataTypes, ReturnDataType);
        }

        public bool Equals(MethodSignature? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name.Equals(other.Name)
                   && SelfDataType.Equals(other.SelfDataType)
                   && ParameterDataTypes.Equals(other.ParameterDataTypes)
                   && ReturnDataType.Equals(other.ReturnDataType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MethodSignature);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public static bool operator ==(MethodSignature? left, MethodSignature? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MethodSignature? left, MethodSignature? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterDataTypes.Prepend(SelfDataType))}) -> {ReturnDataType}";
        }
    }
}