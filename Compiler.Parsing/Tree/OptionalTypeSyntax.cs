using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    class OptionalTypeSyntax : TypeSyntax, IOptionalTypeSyntax
    {
        public ITypeSyntax Referent { get; }

        public OptionalTypeSyntax(TextSpan span, ITypeSyntax referent)
            : base(span)
        {
            Referent = referent;
        }

        public override string ToString()
        {
            return $"{Referent}?";
        }
    }
}
