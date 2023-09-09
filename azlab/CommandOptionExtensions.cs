using McMaster.Extensions.CommandLineUtils;

namespace Azoth.Tools.Bootstrap.Lab;

public static class CommandOptionExtensions
{
    public static T? OptionalValue<T>(this CommandOption<T> option)
        where T : struct
        // Trying to use just `default` leads to 0 for int
        => option.HasValue() ? option.ParsedValue : default(T?);

    /// For cases are supported
    /// * "--option" returns true
    /// * "--option=true" returns true
    /// * "--option=false" returns false
    /// * "" returns null
    public static bool? OptionalValue(this CommandOption<bool> option)
        => option.HasValue() ? (option.Value() is null || option.ParsedValue) : default(bool?);
}
