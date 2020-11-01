using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class UsingDirectiveSyntax : Syntax, IUsingDirectiveSyntax
    {
        // For now, we only support namespace names
        public NamespaceName Name { get; }

        public UsingDirectiveSyntax(TextSpan span, NamespaceName name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"using {Name};";
        }
    }
}
