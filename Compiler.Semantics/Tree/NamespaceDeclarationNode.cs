using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class NamespaceDeclarationNode : CodeNode, INamespaceDeclaration
{
    public override INamespaceDeclarationSyntax Syntax { get; }
    IDeclarationSyntax IDeclaration.Syntax => Syntax;
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;

    public IFixedList<IUsingDirective> UsingDirectives { get; }
    public IFixedList<INamespaceMemberDeclaration> Declarations { get; }

    public NamespaceDeclarationNode(
        INamespaceDeclarationSyntax syntax,
        IEnumerable<IUsingDirective> usingDirectives,
        IEnumerable<INamespaceMemberDeclaration> declarations)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.CreateFixed(usingDirectives);
        Declarations = ChildList.CreateFixed(declarations);
    }
}
