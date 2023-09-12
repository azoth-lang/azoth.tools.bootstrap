using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(BoolType),
    typeof(NumericType))]
public abstract class SimpleType : ValueType
{
    public SpecialTypeName Name { get; }

    public override TypeSemantics Semantics => TypeSemantics.CopyValue;

    private protected SimpleType(SpecialTypeName name)
    {
        Name = name;
    }

    public override string ToSourceCodeString() => Name.ToString();

    public override string ToILString() => ToSourceCodeString();

    public override bool Equals(DataType? other)
        // Most simple types are fixed instances, so a reference comparision suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(Name);
}
