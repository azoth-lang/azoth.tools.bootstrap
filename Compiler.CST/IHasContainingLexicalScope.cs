using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.CST
{
    public interface IHasContainingLexicalScope
    {
        LexicalScope ContainingLexicalScope { get; set; }
    }
}
