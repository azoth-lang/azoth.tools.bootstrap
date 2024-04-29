using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;
public abstract class TransformMethod : Method
{
    public Pass Pass { get; }
    public NonVoidType FromType { get; }
    public virtual NonOptionalType FromCoreType { get; }
    public Parameter From { get; }
    public bool ParametersDeclared { get; }

    private protected TransformMethod(Pass pass, bool parametersDeclared, NonVoidType fromType)
    {
        Pass = pass;
        FromType = fromType;
        FromCoreType = FromType.ToNonOptional();
        From = Parameter.Create(FromType, Parameter.FromName);
        ParametersDeclared = parametersDeclared;
    }
}
