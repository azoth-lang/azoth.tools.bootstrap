using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class PassNode
{
    public string Namespace { get; }
    public string Name { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public string? FromContext { get; }
    public string? ToContext { get; }
    public SymbolNode? From { get; }
    public SymbolNode? To { get; }
    public IFixedList<TransformNode> Transforms { get; }

    public PassNode(
        string ns,
        string name,
        IEnumerable<string> usingNamespaces,
        string? fromContext,
        string? toContext,
        SymbolNode? from,
        SymbolNode? to,
        IEnumerable<TransformNode> transforms)
    {
        Name = name;
        Namespace = ns;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        FromContext = fromContext;
        ToContext = toContext;
        From = from;
        To = to;
        Transforms = transforms.ToFixedList();
    }
}
