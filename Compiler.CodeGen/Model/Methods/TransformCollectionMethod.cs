using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed class TransformCollectionMethod : TransformMethod
{
    public override CollectionType FromCoreType { get; }
    public Parameter From { get; }
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    public override IFixedSet<Method> CallsMethods => callsMethods.Value;
    private readonly Lazy<IFixedSet<Method>> callsMethods;

    public TransformCollectionMethod(Pass pass, Transform transform, CollectionType fromType, CollectionType toType)
    : base(pass)
    {
        FromCoreType = fromType;
        From = Parameter.Create(fromType, Parameter.FromName);
        AdditionalParameters = transform.AdditionalParameters;
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();
        To = Parameter.Create(toType, Parameter.ToName);
        AdditionalReturnValues = transform.AdditionalReturnValues;
        AllReturnValues = AdditionalReturnValues.Prepend(To).ToFixedList();
        AutoGenerate = transform.AutoGenerate;

        if (AutoGenerate)
            callsMethods = new(() => GetCallsMethods().ToFixedSet());
        else
            callsMethods = new(FixedSet.Empty<Method>);
    }

    private IEnumerable<Method> GetCallsMethods()
    {
        yield return Pass.TransformMethods.Single(m => m.FromCoreType == FromCoreType.ElementType);
    }
}
