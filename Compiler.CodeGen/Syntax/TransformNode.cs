using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public class TransformNode
{
    public IFixedList<ParameterNode> From { get; }
    public IFixedList<ParameterNode> To { get; }
    public bool AutoGenerate { get; }

    public TransformNode(IFixedList<ParameterNode> from, IFixedList<ParameterNode> to, bool autoGenerate)
    {
        From = from;
        To = to;
        AutoGenerate = autoGenerate;
    }
}
