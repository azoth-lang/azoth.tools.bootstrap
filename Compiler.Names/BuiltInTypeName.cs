using System;

namespace Azoth.Tools.Bootstrap.Compiler.Names;

/// <summary>
/// A built-in type name is a type name that is a keyword in the language. This
/// includes all the simple types (e.g. `int`, `bool`, etc.) and other built-in
/// types like `Any`, `Self`, `never`, etc. Since these built-in names are
/// keywords, they are distinct from an identifier with the same letters. For
/// example, `bool` is a keyword distinct from the identifier `\bool`.
/// </summary>
public sealed class BuiltInTypeName : TypeName
{
    #region Instances
    public static readonly BuiltInTypeName Void = new("void");
    public static readonly BuiltInTypeName Never = new("never");
    public static readonly BuiltInTypeName Bool = new("bool");
    public static readonly BuiltInTypeName Any = new("Any");
    public static readonly BuiltInTypeName Self = new("Self");
    public static readonly BuiltInTypeName Int = new("int");
    public static readonly BuiltInTypeName UInt = new("uint");
    public static readonly BuiltInTypeName Int8 = new("int8");
    public static readonly BuiltInTypeName Byte = new("byte");
    public static readonly BuiltInTypeName Int16 = new("int16");
    public static readonly BuiltInTypeName UInt16 = new("uint16");
    public static readonly BuiltInTypeName Int32 = new("int32");
    public static readonly BuiltInTypeName UInt32 = new("uint32");
    public static readonly BuiltInTypeName Int64 = new("int64");
    public static readonly BuiltInTypeName UInt64 = new("uint64");
    public static readonly BuiltInTypeName Size = new("size");
    public static readonly BuiltInTypeName Offset = new("offset");
    public static readonly BuiltInTypeName NInt = new("nint");
    public static readonly BuiltInTypeName NUInt = new("nuint");
    #endregion

    private BuiltInTypeName(string text)
        : base(text, 0) { }

    #region Equality
    public override bool Equals(TypeName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is BuiltInTypeName otherName && Text == otherName.Text;
    }

    public override int GetHashCode() => HashCode.Combine(typeof(BuiltInTypeName), Text);
    #endregion

    public override OrdinaryName? WithAttributeSuffix() => null;

    public override string ToBareString() => Text;

    public override string ToString() => Text;
}
