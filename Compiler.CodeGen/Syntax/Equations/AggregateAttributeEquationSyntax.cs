namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class AggregateAttributeEquationSyntax : EquationSyntax
{
    public AggregateAttributeEquationSyntax(SymbolSyntax node, string name)
        : base(node, name, false, null) { }

    public override string ToString() => $"= {Node}.↑.{Name}";
}
