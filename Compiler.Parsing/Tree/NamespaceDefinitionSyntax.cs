using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NamespaceDefinitionSyntax : NonMemberDefinitionSyntax, INamespaceDefinitionSyntax
{
    /// <summary>
    /// Whether this namespace declaration is in the global namespace, the
    /// implicit file namespace is in the global namespace. As are namespaces
    /// declared using the package qualifier `namespace ::example { }`.
    /// </summary>
    public bool IsGlobalQualified { get; }
    public NamespaceName DeclaredNames { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; }

    public NamespaceDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        bool isGlobalQualified,
        NamespaceName declaredNames,
        TextSpan nameSpan,
        IFixedList<IUsingDirectiveSyntax> usingDirectives,
        IFixedList<INonMemberDefinitionSyntax> declarations)
        : base(span, file, declaredNames.Segments.LastOrDefault(), nameSpan)
    {
        DeclaredNames = declaredNames;
        UsingDirectives = usingDirectives;
        Definitions = declarations;
        IsGlobalQualified = isGlobalQualified;
    }

    public override string ToString()
        => IsGlobalQualified ? $"namespace ::{DeclaredNames} {{ … }}" : $"namespace {DeclaredNames} {{ … }}";
}
