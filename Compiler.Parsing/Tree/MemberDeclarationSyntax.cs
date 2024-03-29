using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class MemberDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
{
    public ITypeDeclarationSyntax DeclaringType { get; }
    public IAccessModifierToken? AccessModifier { get; }

    protected MemberDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName? name,
        IPromise<Symbol> symbol)
        : base(span, file, name, nameSpan, symbol)
    {
        DeclaringType = declaringType;
        AccessModifier = accessModifier;
    }
}
