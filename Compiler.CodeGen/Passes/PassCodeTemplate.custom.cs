using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Passes;

public partial class PassCodeTemplate
{
    private readonly Pass pass;

    public PassCodeTemplate(Pass pass)
    {
        this.pass = pass;
    }
}
