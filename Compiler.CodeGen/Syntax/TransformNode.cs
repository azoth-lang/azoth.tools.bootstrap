using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public class TransformNode
{
    public ParameterNode? From { get; }
    public IFixedList<ParameterNode> AdditionalParameters { get; }
    public ParameterNode? To { get; }
    public IFixedList<ParameterNode> AdditionalReturnValues { get; }
    public bool AutoGenerate { get; }

    public TransformNode(ParameterNode? from, IEnumerable<ParameterNode> additionalParameters,
        ParameterNode? to, IEnumerable<ParameterNode> additionalReturnValues, bool autoGenerate)
    {
        From = from;
        AdditionalParameters = additionalParameters.ToFixedList();
        To = to;
        AdditionalReturnValues = additionalReturnValues.ToFixedList();
        AutoGenerate = autoGenerate;
    }
}
