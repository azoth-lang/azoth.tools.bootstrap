using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// <summary>
/// Some kind of reference to where the source code came from. For example,
/// this might be a path on disk or a network URL or a git hash or what
/// template file the code was generated from.
/// </summary>
public abstract class CodeReference : IComparable<CodeReference>
{
    public IFixedList<string> Namespace { get; }

    public bool IsTesting { get; }

    protected CodeReference(IFixedList<string> @namespace, bool isTesting)
    {
        Namespace = @namespace;
        IsTesting = isTesting;
    }

    public abstract int CompareTo(CodeReference? other);

    public abstract override string ToString();
}
