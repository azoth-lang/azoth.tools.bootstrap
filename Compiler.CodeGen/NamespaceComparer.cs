using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public class NamespaceComparer : IComparer<string>
{
    #region Singleton
    public static NamespaceComparer Instance { get; } = new NamespaceComparer();

    private NamespaceComparer() { }
    #endregion

    public int Compare(string? x, string? y)
    {
        (var xAlias, x) = Parsing.OptionalSplitOffStart(x!, "=");
        var xParts = x.Split('.');
        (var yAlias, y) = Parsing.OptionalSplitOffStart(y!, "=");
        var yParts = y.Split('.');

        // Put aliases after regular using
        if (xAlias is not null && yAlias is null) return 1;
        if (xAlias is null && yAlias is not null) return -1;

        if (xParts[0] == "System" && yParts[0] != "System") return -1;
        if (xParts[0] != "System" && yParts[0] == "System") return 1;

        var length = Math.Min(xParts.Length, yParts.Length);
        for (int i = 0; i < length; i++)
        {
            var cmp = string.Compare(xParts[i], yParts[i], StringComparison.InvariantCulture);
            if (cmp != 0) return cmp;
        }

        return xParts.Length.CompareTo(yParts.Length);
    }
}
