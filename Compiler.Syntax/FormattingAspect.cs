using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

internal static partial class FormattingAspect
{
    public static partial string AbstractMethodDefinition_ToString(IAbstractMethodDefinitionSyntax node)
    {
        var @return = node.Return is not null ? node.Return.ToString() : "";
        return $"fn {node.Name}({string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter))}){@return};";
    }

    public static partial string GetterMethodDefinition_ToString(IGetterMethodDefinitionSyntax node)
        => "TESTING!";

    public static partial string SetterMethodDefinition_ToString(ISetterMethodDefinitionSyntax node)
        => "TESTING!";
    public static partial string StandardMethodDefinition_ToString(IStandardMethodDefinitionSyntax node)
        => "TESTING!";
}
