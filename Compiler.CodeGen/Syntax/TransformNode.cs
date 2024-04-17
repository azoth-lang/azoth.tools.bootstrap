using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

internal class TransformNode
{
    public IFixedList<PropertyNode> From { get; }
    public IFixedList<PropertyNode> To { get; }
    public bool AutoGenerate { get; }

    public TransformNode(IFixedList<PropertyNode> from, IFixedList<PropertyNode> to, bool autoGenerate)
    {
        From = from;
        To = to;
        AutoGenerate = autoGenerate;
    }
}
