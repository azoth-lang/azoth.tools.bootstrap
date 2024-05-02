using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

// ReSharper disable once InconsistentNaming
public interface Code : Node
{
    public new IConcreteSyntax Syntax { get; }
    ISyntax Node.Syntax => Syntax;
}
