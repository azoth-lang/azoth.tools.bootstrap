using System.IO;
using Azoth.Tools.Bootstrap.Tests.Unit.Helpers;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;

public static class TestsDirectory
{
    public static string Get()
    {
        return Path.Combine(SolutionDirectory.Get(), "azoth.language.tests");
    }
}
