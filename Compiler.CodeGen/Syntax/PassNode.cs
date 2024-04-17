using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class PassNode
{
    public string Name { get; }
    public string Namespace { get; }
    public SymbolNode? From { get; }
    public SymbolNode? To { get; }
    public SymbolNode? FromContext { get; }
    public SymbolNode? ToContext { get; }
    public IFixedList<TransformNode> Transforms { get; }

    public PassNode(
        string ns,
        string name,
        SymbolNode? from,
        SymbolNode? to,
        SymbolNode? fromContext,
        SymbolNode? toContext,
        IFixedList<TransformNode> transforms)
    {
        Name = name;
        From = from;
        To = to;
        FromContext = fromContext;
        ToContext = toContext;
        Transforms = transforms;
        Namespace = ns;
    }
}
