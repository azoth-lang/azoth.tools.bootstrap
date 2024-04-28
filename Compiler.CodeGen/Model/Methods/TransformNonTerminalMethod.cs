using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

internal sealed class TransformNonTerminalMethod : TransformMethod
{
    public Rule FromReferencedRule { get; }
    public override NonOptionalType FromCoreType { get; }
    public Parameter From { get; }
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
        : base(pass, true)
    {
        FromReferencedRule = fromReferencedRule;
        FromCoreType = fromType.ToNonOptional();
        From = Parameter.Create(fromType, Parameter.FromName);
        AdditionalParameters = transform.AdditionalParameters;
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();

        To = Parameter.Create(toType, Parameter.ToName);
        AdditionalReturnValues = transform.AdditionalReturnValues;
        AllReturnValues = To.YieldValue().Concat(AdditionalReturnValues).ToFixedList();

        AutoGenerate = transform.AutoGenerate;
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        if (AutoGenerate)
            yield break;

        foreach (var childRule in FromReferencedRule.ChildRules)
            yield return Pass.TransformMethods.Single(m => m.FromCoreType == childRule.DefinesType);
    }
}
