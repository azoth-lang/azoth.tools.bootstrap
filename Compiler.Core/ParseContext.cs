namespace Azoth.Tools.Bootstrap.Compiler.Core
{
    public class ParseContext
    {
        public CodeFile File { get; }
        public Diagnostics Diagnostics { get; }

        public ParseContext(CodeFile file, Diagnostics diagnostics)
        {
            File = file;
            Diagnostics = diagnostics;
        }
    }
}
