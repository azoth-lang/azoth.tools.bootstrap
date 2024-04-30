using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record TransformMethod : Method
{
    public override bool ParametersDeclared { get; }

    private protected TransformMethod(Pass pass, bool parametersDeclared, NonVoidType fromType)
        : base(pass, fromType)
    {
        ParametersDeclared = parametersDeclared;
    }
}
