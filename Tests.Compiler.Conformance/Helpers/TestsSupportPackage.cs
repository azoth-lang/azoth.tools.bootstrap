using System.IO;
using Azoth.Tools.Bootstrap.Tests.Unit.Helpers;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers
{
    public static class TestsSupportPackage
    {
        public const string Name = "azoth.language.tests.support";

        public static string GetDirectory()
        {
            return Path.Combine(SolutionDirectory.Get(), Name);
        }
    }
}
