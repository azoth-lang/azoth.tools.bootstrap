using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class MemberDefinitionSyntax : DefinitionSyntax, ITypeMemberDefinitionSyntax
{
    public ITypeDefinitionSyntax DefiningType { get; }
    public IAccessModifierToken? AccessModifier { get; }

    protected MemberDefinitionSyntax(
        ITypeDefinitionSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName? name)
        : base(span, file, name, nameSpan)
    {
        DefiningType = declaringType;
        AccessModifier = accessModifier;
    }
}
