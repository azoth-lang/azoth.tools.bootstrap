using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal sealed partial class DeclarationLexicalScopesBuilder
{
    private To.ClassDeclaration CreateClassDeclaration(
        From.ClassDeclaration from,
        DeclarationScope lexicalScope,
        DeclarationLexicalScope containingScope,
        DeclarationLexicalScope childContainingScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, TransformUnresolvedSupertypeName(from.BaseTypeName, childContainingScope, childContainingScope),
            TransformClassMemberDeclarations(from.Members, childContainingScope), lexicalScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope),
            containingScope, from.ContainingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(
        From.TraitDeclaration from,
        DeclarationScope lexicalScope,
        DeclarationLexicalScope containingScope,
        DeclarationLexicalScope childContainingScope)
        => To.TraitDeclaration.Create(from.Syntax,
            TransformTraitMemberDeclarations(from.Members, childContainingScope), lexicalScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope),
            containingScope, from.ContainingNamespace);

    private To.StructDeclaration CreateStructDeclaration(
        From.StructDeclaration from,
        DeclarationScope lexicalScope,
        DeclarationLexicalScope containingScope,
        DeclarationLexicalScope childContainingScope)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, containingScope), lexicalScope, from.GenericParameters,
            TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope), containingScope, from.ContainingNamespace);
}
