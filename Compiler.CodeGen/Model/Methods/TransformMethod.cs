using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;
public abstract class TransformMethod : Method
{
    public Pass Pass { get; }
    public abstract NonOptionalType FromCoreType { get; }

    private protected TransformMethod(Pass pass)
    {
        Pass = pass;
    }
}
