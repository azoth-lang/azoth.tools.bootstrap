using System.Collections.Generic;
using System.IO;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers
{
    public static class CodeFiles
    {
        public static IEnumerable<string> GetIn(string directory)
        {
            return Directory.EnumerateFiles(directory, "*.az", SearchOption.AllDirectories);
        }
    }
}
