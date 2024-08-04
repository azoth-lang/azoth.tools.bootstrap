using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Walkers;

public abstract class AbstractSyntaxWalker<T>
{
    [DebuggerHidden]
    protected void Walk(IAbstractSyntax? syntax, T arg)
    {
        if (syntax is null) return;
        WalkNonNull(syntax, arg);
    }

    protected abstract void WalkNonNull(IAbstractSyntax syntax, T arg);

    [DebuggerHidden]
    protected void WalkChildren(IAbstractSyntax syntax, T arg)
    {
        foreach (var child in syntax.Children())
            WalkNonNull(child, arg);
    }
}
