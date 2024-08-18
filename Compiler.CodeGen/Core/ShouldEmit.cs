using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    public static bool Class(TreeNodeModel node)
        => !node.IsAbstract;

    public static bool EquationPartialImplementation(EquationModel equation)
        => equation.Expression is null;
}
