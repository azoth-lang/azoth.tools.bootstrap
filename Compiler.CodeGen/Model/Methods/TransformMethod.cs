using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

[Closed(
    typeof(TransformCollectionMethod),
    typeof(TransformIdentityMethod),
    typeof(TransformNonTerminalMethod),
    typeof(TransformTerminalMethod))]
public abstract record TransformMethod : Method
{
    public override bool ParametersDeclared { get; }
    public abstract NonVoidType? ToType { get; }
    public abstract Parameter? To { get; }
    public abstract IFixedList<Parameter> AllReturnValues { get; }
    public abstract bool AutoGenerate { get; }

    private protected TransformMethod(Pass pass, bool parametersDeclared, NonVoidType fromType)
        : base(pass, fromType)
    {
        ParametersDeclared = parametersDeclared;
    }

    public abstract TransformMethod ToOptional();
}
