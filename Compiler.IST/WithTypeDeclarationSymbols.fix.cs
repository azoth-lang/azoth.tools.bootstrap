using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

public partial class WithTypeDeclarationPromises
{
    public partial interface FunctionDeclaration
    {
        new CodeFile File { get; }
        new DeclarationLexicalScope ContainingScope { get; }
    }
}
