using System;

namespace Azoth.Tools.Bootstrap.Framework;

public static class EnumExtensions
{
    public static bool IsDefined<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => Enum.IsDefined(typeof(TEnum), value);
}
