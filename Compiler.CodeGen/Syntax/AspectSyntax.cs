using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class AspectSyntax
{
    public string Namespace { get; }
    public string Name { get; }
    public IFixedList<SnippetSyntax> Snippets { get; }
    public IFixedSet<TypeDeclarationSyntax> TypeDeclarations { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedSet<AttributeFamilySyntax> AttributeFamilies { get; }
    public IFixedList<AspectAttributeSyntax> Attributes { get; }
    public IFixedList<EquationSyntax> Equations { get; }
    public IFixedList<RewriteRuleSyntax> RewriteRules { get; }

    public AspectSyntax(
        string @namespace,
        string name,
        IEnumerable<string> usingNamespaces,
        IEnumerable<TypeDeclarationSyntax> typeDeclarations,
        IEnumerable<SnippetSyntax> snippets,
        IEnumerable<AttributeFamilySyntax> attributeFamilies,
        IEnumerable<AspectAttributeSyntax> attributes,
        IEnumerable<EquationSyntax> equations,
        IEnumerable<RewriteRuleSyntax> rewriteRules)
    {
        Namespace = @namespace;
        Name = name;
        Snippets = snippets.ToFixedList();
        TypeDeclarations = typeDeclarations.ToFixedSet();
        UsingNamespaces = usingNamespaces.ToFixedSet();
        AttributeFamilies = attributeFamilies.ToFixedSet();
        Attributes = attributes.ToFixedList();
        Equations = equations.ToFixedList();
        RewriteRules = rewriteRules.ToFixedList();
    }
}
