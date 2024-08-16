using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects;

public static class AspectCodeBuilder
{
    public static string GenerateAspect(AspectModel aspect)
    {
        var template = new AspectCodeTemplate(aspect);
        return template.TransformText();
    }
}
