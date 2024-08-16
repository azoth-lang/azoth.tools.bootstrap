using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects;

public partial class AspectCodeTemplate
{
    private readonly AspectModel aspect;

    public AspectCodeTemplate(AspectModel aspect)
    {
        this.aspect = aspect;
    }
}
