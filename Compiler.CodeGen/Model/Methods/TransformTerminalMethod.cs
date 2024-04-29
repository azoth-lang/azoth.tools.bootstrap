using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

internal sealed record TransformTerminalMethod : TransformMethod
{
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    [SetsRequiredMembers]
    public TransformTerminalMethod(Pass pass, Transform transform, NonVoidType fromType, NonVoidType? toType)
        : this(pass, parametersDeclared: true,
            fromType, transform.AdditionalParameters,
            toType, transform.AdditionalReturnValues,
            transform.AutoGenerate)
    {
    }

    [SetsRequiredMembers]
    public TransformTerminalMethod(Pass pass, NonVoidType fromType, NonVoidType? toType)
        : this(pass, parametersDeclared: false,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: false)
    {
    }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TransformTerminalMethod(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        if (!AutoGenerate || To is null)
            yield break;

        var rule = (To.Type.UnderlyingSymbol as InternalSymbol)?.ReferencedRule;
        if (rule is null) yield break;

        yield return GetCreateMethodCalled(rule);
    }
}
