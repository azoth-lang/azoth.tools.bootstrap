using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Parameter
{
    public static Parameter Void { get; } = new Parameter(Model.Type.Void, "_");

    public ParameterNode? Syntax { get; }
    public Type Type { get; }
    public string Name { get; }

    public Parameter(Grammar? grammar, ParameterNode syntax)
    {
        Syntax = syntax;
        Type = new Type(grammar, syntax.Type);
        Name = syntax.Name;
    }

    private Parameter(Type type, string name)
    {
        Type = type;
        Name = name;
    }

    [return: NotNullIfNotNull(nameof(type))]
    public static Parameter? Create(Type? type, string name)
        => type is not null ? new Parameter(type, name) : null;
}
