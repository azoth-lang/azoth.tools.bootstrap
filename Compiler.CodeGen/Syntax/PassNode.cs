namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

internal class PassNode
{
    public string Name { get; }
    public SymbolNode? From { get; }
    public SymbolNode? To { get; }
    public SymbolNode? FromContext { get; }
    public SymbolNode? ToContext { get; }

    public PassNode(string name, SymbolNode? from, SymbolNode? to, SymbolNode? fromContext, SymbolNode? toContext)
    {
        Name = name;
        From = from;
        To = to;
        FromContext = fromContext;
        ToContext = toContext;
    }
}
