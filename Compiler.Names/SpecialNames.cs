namespace Azoth.Tools.Bootstrap.Compiler.Names;

public static class SpecialNames
{
    public static string WithAttributeSuffix(string attributeName)
        => attributeName + (char.IsLower(attributeName[0])
            ? SnakeCaseAttributeSuffix
            : PascalCaseAttributeSuffix);
    private const string PascalCaseAttributeSuffix = "_Attribute";
    private const string SnakeCaseAttributeSuffix = "_attribute";

    // TODO eliminate these
    public static readonly string StringTypeName = "string";
    public static readonly string RangeTypeName = "range";
}
