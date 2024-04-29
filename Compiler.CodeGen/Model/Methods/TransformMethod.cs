using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record TransformMethod : Method
{
    public virtual NonOptionalType FromCoreType { get; }
    public Parameter From { get; }
    public bool ParametersDeclared { get; }

    private protected TransformMethod(Pass pass, bool parametersDeclared, NonVoidType fromType)
        : base(pass, fromType)
    {
        FromCoreType = FromType.ToNonOptional();
        From = Parameter.Create(FromType, Parameter.FromName);
        ParametersDeclared = parametersDeclared;
    }
}
