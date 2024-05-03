using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IHasContainingLexicalScope
{
    SymbolScope ContainingLexicalScope { get; set; }
}
