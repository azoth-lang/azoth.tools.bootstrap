using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

internal sealed record TransformNonTerminalMethod : TransformMethod
{
    public Rule FromReferencedRule { get; }
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }

    public override NonVoidType? ToType { get; }
    public override Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public override IFixedList<Parameter> AllReturnValues { get; }
    public override bool AutoGenerate { get; }

    [SetsRequiredMembers]
    public TransformNonTerminalMethod(
        Pass pass,
        Transform transform,
        Rule fromReferencedRule,
        NonVoidType fromType,
        NonVoidType? toType)
        : this(pass, parametersDeclared: true, fromReferencedRule,
            fromType, transform.AdditionalParameters,
            toType, transform.AdditionalReturnValues,
            transform.AutoGenerate)
    {
    }

    [SetsRequiredMembers]
    public TransformNonTerminalMethod(Pass pass, Rule fromReferencedRule, SymbolType fromType, SymbolType toType)
        : this(pass, parametersDeclared: false, fromReferencedRule,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: true)
    {
    }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TransformNonTerminalMethod(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        Pass pass,
        bool parametersDeclared,
        Rule fromReferencedRule,
        NonVoidType fromType,
        IFixedList<Parameter> additionalParameters,
        NonVoidType? toType,
        IFixedList<Parameter> additionalReturnValues,
        bool autoGenerate)
        : base(pass, parametersDeclared, fromType)
    {
        FromReferencedRule = fromReferencedRule;
        AdditionalParameters = additionalParameters;

        ToType = toType;
        To = Parameter.Create(toType, Parameter.ToName);
        AdditionalReturnValues = additionalReturnValues;
        AllReturnValues = To.YieldValue().Concat(AdditionalReturnValues).ToFixedList();

        AutoGenerate = autoGenerate;
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        if (!AutoGenerate)
            yield break;

        foreach (var derivedRule in FromReferencedRule.DerivedRules)
        {
            var transformMethod = Pass.TransformMethods.SingleOrDefault(m => m.FromCoreType == derivedRule.DefinesType);
            if (transformMethod is not null)
                yield return transformMethod;
        }
    }

    public override TransformNonTerminalMethod ToOptional()
    {
        if (ParametersDeclared)
            throw new NotSupportedException("Cannot make a method with declared parameters optional.");
        return new(Pass, ParametersDeclared, FromReferencedRule,
            new OptionalType(FromType), AdditionalParameters,
            new OptionalType(ToType!), AdditionalReturnValues,
            AutoGenerate);
    }
}
