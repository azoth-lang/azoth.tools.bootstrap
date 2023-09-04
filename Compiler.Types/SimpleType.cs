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

    public override TypeSemantics Semantics => TypeSemantics.Copy;

    private protected SimpleType(SpecialTypeName name)
    {
        Name = name;
    }

    public override string ToSourceCodeString()
    {
        return Name.ToString();
    }

    public override string ToILString()
    {
        return ToSourceCodeString();
    }

    public override bool Equals(DataType? other)
    {
        // Most simple types are fixed instances, so a reference comparision suffices
        return ReferenceEquals(this, other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
