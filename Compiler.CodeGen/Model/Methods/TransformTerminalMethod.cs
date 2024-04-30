using System;
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

    public override NonVoidType? ToType { get; }
    public override Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public override IFixedList<Parameter> AllReturnValues { get; }
    public override bool AutoGenerate { get; }

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
            autoGenerate: true)
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

        ToType = toType;
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

    public override TransformTerminalMethod ToOptional()
    {
        if (ParametersDeclared)
            throw new NotSupportedException("Cannot make a method with declared parameters optional.");
        return new(Pass, ParametersDeclared,
            new OptionalType(FromType), AdditionalParameters,
            new OptionalType(ToType!), AdditionalReturnValues,
            AutoGenerate);
    }
}
