using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public abstract record TransformMethod : Method
{
    public override bool ParametersDeclared { get; }
    public abstract Parameter? To { get; }
    public abstract IFixedList<Parameter> AllReturnValues { get; }
    public abstract bool AutoGenerate { get; }

    private protected TransformMethod(Pass pass, bool parametersDeclared, NonVoidType fromType)
        : base(pass, fromType)
    {
        ParametersDeclared = parametersDeclared;
    }
}
