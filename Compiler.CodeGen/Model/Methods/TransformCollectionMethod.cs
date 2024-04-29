using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Methods;

public sealed record TransformCollectionMethod : TransformMethod
{
    public override CollectionType FromCoreType => (CollectionType)FromType;
    public override required IFixedList<Parameter> AdditionalParameters { get; init; }
    public IFixedList<Parameter> AllParameters { get; }

    public Parameter To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    [SetsRequiredMembers]
    public TransformCollectionMethod(Pass pass, Transform transform, CollectionType fromType, CollectionType toType)
    : this(pass, parametersDeclared: true,
        fromType, transform.AdditionalParameters,
        toType, transform.AdditionalReturnValues,
        transform.AutoGenerate)
    { }

    [SetsRequiredMembers]
    public TransformCollectionMethod(Pass pass, CollectionType fromType, CollectionType toType)
        : this(pass, parametersDeclared: false,
            fromType, FixedList.Empty<Parameter>(),
            toType, FixedList.Empty<Parameter>(),
            autoGenerate: true)
    { }

    [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TransformCollectionMethod(Pass pass, bool parametersDeclared,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        CollectionType fromType, IFixedList<Parameter> additionalParameters,
        CollectionType toType, IFixedList<Parameter> additionalReturnValues,
        bool autoGenerate)
        : base(pass, parametersDeclared, fromType)
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
        if (!AutoGenerate)
            yield break;
        yield return Pass.TransformMethods.Single(m => m.FromCoreType == FromCoreType.ElementType);
    }
}
