using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Walkers;

public abstract class SyntaxWalker<T>
{
    [DebuggerHidden]
    protected void Walk(ICodeSyntax? syntax, T arg)
    {
        if (syntax is null) return;
        WalkNonNull(syntax, arg);
    }

    protected abstract void WalkNonNull(ICodeSyntax syntax, T arg);

    [DebuggerHidden]
    protected void WalkChildren(ICodeSyntax syntax, T arg)
    {
        foreach (var child in syntax.Children())
            WalkNonNull(child, arg);
    }

    [DebuggerHidden]
    protected void WalkChildrenInReverse(ICodeSyntax syntax, T arg)
    {
        foreach (var child in syntax.Children().Reverse())
            WalkNonNull(child, arg);
    }
}

public abstract class SyntaxWalker
{
    [DebuggerHidden]
    protected void Walk(ICodeSyntax? syntax)
    {
        if (syntax is null) return;
        WalkNonNull(syntax);
    }

    protected abstract void WalkNonNull(ICodeSyntax syntax);

    [DebuggerHidden]
    protected void WalkChildren(ICodeSyntax syntax)
    {
        foreach (var child in syntax.Children())
            WalkNonNull(child);
    }

    [DebuggerHidden]
    protected void WalkChildrenInReverse(ICodeSyntax syntax)
    {
        foreach (var child in syntax.Children().Reverse())
            WalkNonNull(child);
    }
}
