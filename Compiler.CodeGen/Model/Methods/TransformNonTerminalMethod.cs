using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

internal sealed class TransformNonTerminalMethod : TransformMethod
{
    public Rule FromReferencedRule { get; }
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

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

    public TransformNonTerminalMethod(Pass pass, Rule fromReferencedRule, SymbolType fromType, SymbolType toType)
        : this(pass, parametersDeclared: false, fromReferencedRule,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: true)
    {
    }

    private TransformNonTerminalMethod(
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

        foreach (var childRule in FromReferencedRule.ChildRules)
            yield return Pass.TransformMethods.Single(m => m.FromCoreType == childRule.DefinesType);
    }
}
