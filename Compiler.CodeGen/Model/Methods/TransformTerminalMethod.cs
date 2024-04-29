using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

internal sealed class TransformTerminalMethod : TransformMethod
{
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    public TransformTerminalMethod(Pass pass, Transform transform, NonVoidType fromType, NonVoidType? toType)
        : this(pass, parametersDeclared: true,
            fromType, transform.AdditionalParameters,
            toType, transform.AdditionalReturnValues,
            transform.AutoGenerate)
    {
    }

    public TransformTerminalMethod(Pass pass, NonVoidType fromType, NonVoidType? toType)
        : this(pass, parametersDeclared: false,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: false)
    {
    }

    private TransformTerminalMethod(
        Pass pass,
        bool parametersDeclared,
        NonVoidType fromType,
        IFixedList<Parameter> additionalParameters,
        NonVoidType? toType,
        IFixedList<Parameter> additionalReturnValues,
        bool autoGenerate)
        : base(pass, parametersDeclared, fromType)
    {
        AdditionalParameters = additionalParameters;
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();

        To = Parameter.Create(toType, Parameter.ToName);
        AdditionalReturnValues = additionalReturnValues;
        AllReturnValues = To.YieldValue().Concat(AdditionalReturnValues).ToFixedList();

        AutoGenerate = autoGenerate;
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        if (AutoGenerate)
            yield break;

        yield return Pass.AdvancedCreateMethods.Single(m => m.From.Type == FromCoreType);
    }
}
