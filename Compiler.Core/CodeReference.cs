using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// Some kind of reference to where the source code came from. For example,
/// this might be a path on disk or a network URL or a git hash or what
/// template file the code was generated from.
public abstract class CodeReference
{
    public FixedList<string> Namespace { get; }

    public bool IsTest { get; }

    protected CodeReference(FixedList<string> @namespace, bool isTest)
    {
        Namespace = @namespace;
        IsTest = isTest;
    }

    public abstract override string ToString();
}
