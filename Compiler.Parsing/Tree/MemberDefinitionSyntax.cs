using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class MemberDefinitionSyntax : DefinitionSyntax, ITypeMemberDefinitionSyntax
{
    public IAccessModifierToken? AccessModifier { get; }

    protected MemberDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName? name)
        : base(span, file, name, nameSpan)
    {
        AccessModifier = accessModifier;
    }
}
