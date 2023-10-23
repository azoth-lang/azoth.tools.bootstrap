using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BindingContextPattern : Pattern, IBindingContextPattern
{
    public IPattern Pattern { get; }
    public DataType Type { get; }

    public BindingContextPattern(TextSpan span, IPattern pattern, DataType type)
        : base(span)
    {
        Pattern = pattern;
        Type = type;
    }

    public override string ToString() => $"{Pattern}: {Type.ToILString()}";
}
