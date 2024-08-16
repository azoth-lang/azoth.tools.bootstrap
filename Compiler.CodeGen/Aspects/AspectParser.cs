using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects;

public static class AspectParser
{
    public static AspectSyntax Parse(string aspectDefinition)
    {
        var lines = Parsing.ParseLines(aspectDefinition).ToFixedList();

        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var name = Parsing.GetRequiredConfig(lines, "namespace");

        return new AspectSyntax(ns, name);
    }
}
