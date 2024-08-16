namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class AspectSyntax
{
    public string Namespace { get; }
    public string Name { get; }

    public AspectSyntax(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }
}
