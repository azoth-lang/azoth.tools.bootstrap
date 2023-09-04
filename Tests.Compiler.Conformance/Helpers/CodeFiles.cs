using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;

public static class CodeFiles
{
    public static IEnumerable<string> GetIn(string directory)
        => Directory.EnumerateFiles(directory, "*.az", SearchOption.AllDirectories)
                    .Where(p => !p.EndsWith(".skip.az"));
}
