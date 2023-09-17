using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

public partial class TokenTypes
{
    [GeneratedRegex(@"^(((u?int|float|decimal)\d*)|(u?fixed(\d+\.\d+)?)|(real(\.\d+)?))$",
        RegexOptions.ExplicitCapture)]
    private static partial Regex ReservedTypeNamePattern();

    public static readonly IReadOnlySet<string> ReservedWords = new HashSet<string>()
    {
        // Keywords not yet implemented
        "base",
        "delete",
        "ensures",
        "enum",
        "explicit",
        "external",
        "float32",
        "float64",
        "forever",
        "get",
        "implicit",
        "init",
        "int8",
        "int16",
        "int64",
        "invariant",
        "match",
        "may",
        "Metatype",
        "no",
        "operator",
        "override",
        "params",
        "protected",
        "ref",
        "requires",
        "Self",
        "set",
        "struct",
        "throw",
        "Tuple",
        "Type",
        "uint16",
        "uint64",
        "uninitialized",
        "where",

        // Reserved Words
        "alias",             // Planned Type Alias Feature
        "case",              // Useful for switch like constructs
        "cast",              // Casting
        "checked",           // Checked Operations
        "const_cast",        // Casting
        "continue",          // Useful for control flow
        "default",           // Useful for switch like constructs and default values
        "defer",             // Swift style "`defer`",statements
        "do",                // "`do while`",loop or Swift style "`do`",block
        "dynamic_cast",      // Casting
        "extend",            // Extensions
        "extension",         // Extensions
        "fallthrough",       // Useful for switch like constructs
        "for",               // Common Keyword
        "from",              // Common Keyword
        "guard",             // Swift style guard statements
        "internal",          // Common Keyword
        "null",              // Null value for pointers
        "otherwise",         // Loop Else
        "package",           // Qualify names with the current package (i.e. `package::.name`)
        "partial",           // Partial Classes
        "private",           // Common Keyword
        "reinterpret_cast",  // Casting
        "repeat",            // Swift style "`repeat {} while condition;`",loop
        "replace",           // Partial Classes
        "select",            // C# style query
        "sizeof", "size_of", // Size of Operator
        "switch",            // Useful for switch like constructs
        "symmetric",         // Symmetric operators
        "transmute",         // Reinterpret Cast
        "trn",               // Transition reference capability
        "then",              // Python style loop else
        "type",              // Type aliases and declarations
        "unchecked",         // Unchecked Operations
        "unless",            // Ruby style `if not` statement or `unless break` for Python style loop else
        "when",              // C# style exception filters
        "xor",               // Logical exclusive or operator
        "yield",             // Generators
    };

    /// <summary>
    /// Whether the value is a reserved type name. Note, this returns
    /// true for actual type names like `int32`. So values must be
    /// checked for being a keyword before being checked with this function.
    /// </summary>
    public static bool IsReservedTypeName(string value) => ReservedTypeNamePattern().IsMatch(value);
}
