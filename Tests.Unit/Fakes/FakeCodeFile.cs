using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Fakes
{
    public static class FakeCodeFile
    {

        public static CodeFile For(string text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text));
        }
    }
}
