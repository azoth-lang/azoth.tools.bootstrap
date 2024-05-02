using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Walkers;

public abstract class SyntaxWalker<T>
{
    [DebuggerHidden]
    protected void Walk(IConcreteSyntax? syntax, T arg)
    {
        if (syntax is null) return;
        WalkNonNull(syntax, arg);
    }

    protected abstract void WalkNonNull(IConcreteSyntax syntax, T arg);

    [DebuggerHidden]
    protected void WalkChildren(IConcreteSyntax syntax, T arg)
    {
        foreach (var child in syntax.Children())
            WalkNonNull(child, arg);
    }

    [DebuggerHidden]
    protected void WalkChildrenInReverse(IConcreteSyntax syntax, T arg)
    {
        foreach (var child in syntax.Children().Reverse())
            WalkNonNull(child, arg);
    }
}

public abstract class SyntaxWalker
{
    [DebuggerHidden]
    protected void Walk(IConcreteSyntax? syntax)
    {
        if (syntax is null) return;
        WalkNonNull(syntax);
    }

    protected abstract void WalkNonNull(IConcreteSyntax syntax);

    [DebuggerHidden]
    protected void WalkChildren(IConcreteSyntax syntax)
    {
        foreach (var child in syntax.Children())
            WalkNonNull(child);
    }

    [DebuggerHidden]
    protected void WalkChildrenInReverse(IConcreteSyntax syntax)
    {
        foreach (var child in syntax.Children().Reverse())
            WalkNonNull(child);
    }
}
