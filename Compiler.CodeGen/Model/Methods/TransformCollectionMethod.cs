using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed class TransformCollectionMethod : TransformMethod
{
    public override CollectionType FromCoreType => (CollectionType)FromType;
    public override IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    public TransformCollectionMethod(Pass pass, Transform transform, CollectionType fromType, CollectionType toType)
    : this(pass, parametersDeclared: true,
        fromType, transform.AdditionalParameters,
        toType, transform.AdditionalReturnValues,
        transform.AutoGenerate)
    { }

    public TransformCollectionMethod(Pass pass, CollectionType fromType, CollectionType toType)
        : this(pass, parametersDeclared: false,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: true)
    { }

    private TransformCollectionMethod(Pass pass, bool parametersDeclared,
        CollectionType fromType, IFixedList<Parameter> additionalParameters,
        CollectionType toType, IFixedList<Parameter> additionalReturnValues,
        bool autoGenerate)
        : base(pass, true, fromType)
    {
        AdditionalParameters = additionalParameters;
        AllParameters = AdditionalParameters.Prepend(From).ToFixedList();

        To = Parameter.Create(toType, Parameter.ToName);
        AdditionalReturnValues = additionalReturnValues;
        AllReturnValues = AdditionalReturnValues.Prepend(To).ToFixedList();

        AutoGenerate = autoGenerate;
    }

    public override IEnumerable<Method> GetMethodsCalled()
    {
        if (AutoGenerate)
            yield break;
        yield return Pass.TransformMethods.Single(m => m.FromCoreType == FromCoreType.ElementType);
    }
}
