using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

// ReSharper disable once InconsistentNaming
public interface Node
{
    public ISyntax Syntax { get; }
}
