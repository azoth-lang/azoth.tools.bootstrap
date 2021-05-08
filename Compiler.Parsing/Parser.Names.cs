using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    public partial class Parser
    {
        private INameExpressionSyntax ParseName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = identifier.Value;
            return new NameExpressionSyntax(identifier.Span, name);
        }
    }
}
