using System;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A special type name is the name of one of the special types in the system.
/// They are distinguished because for example `bool` is a special name, but
/// `\"bool"` is a regular name with the same text.
/// </summary>
public sealed class SpecialTypeName : TypeName
{
    #region Empty and Simple Types
    public static readonly SpecialTypeName Void = new("void");
    public static readonly SpecialTypeName Never = new("never");
    public static readonly SpecialTypeName Bool = new("bool");
    public static readonly SpecialTypeName Any = new("Any");
    public static readonly SpecialTypeName Int = new("int");
    public static readonly SpecialTypeName UInt = new("uint");
    public static readonly SpecialTypeName Int8 = new("int8");
    public static readonly SpecialTypeName Byte = new("byte");
    public static readonly SpecialTypeName Int16 = new("int16");
    public static readonly SpecialTypeName UInt16 = new("uint16");
    public static readonly SpecialTypeName Int32 = new("int32");
    public static readonly SpecialTypeName UInt32 = new("uint32");
    public static readonly SpecialTypeName Int64 = new("int64");
    public static readonly SpecialTypeName UInt64 = new("uint64");
    public static readonly SpecialTypeName Size = new("size");
    public static readonly SpecialTypeName Offset = new("offset");
    #endregion

    #region Constant Types
    public static readonly SpecialTypeName True = new("True");
    public static readonly SpecialTypeName False = new("False");
    public static readonly SpecialTypeName ConstInt = new("ConstInt");
    #endregion

    private SpecialTypeName(string text)
        : base(text, 0) { }

    #region Equality
    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is SpecialTypeName otherName && Text == otherName.Text;
    }

    public override int GetHashCode() => HashCode.Combine(typeof(SpecialTypeName), Text);
    #endregion

    public override StandardTypeName? WithAttributeSuffix() => null;

    public override string ToBareString() => Text;

    public override string ToString() => Text;
}
