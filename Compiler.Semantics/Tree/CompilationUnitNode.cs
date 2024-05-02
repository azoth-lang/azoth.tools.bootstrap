using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CompilationUnitNode : CodeNode, ICompilationUnit
{
    public override ICompilationUnitSyntax Syntax { get; }

    public CodeFile File => Syntax.File;
    public NamespaceName ImplicitNamespaceName => Syntax.ImplicitNamespaceName;

    public IFixedList<IUsingDirective> UsingDirectives { get; }
    public IFixedList<INamespaceMemberDeclaration> Declarations { get; }

    public CompilationUnitNode(
        ICompilationUnitSyntax syntax,
        IEnumerable<IUsingDirective> usingDirectives,
        IEnumerable<INamespaceMemberDeclaration> declarations)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.CreateFixed(usingDirectives);
        Declarations = ChildList.CreateFixed(declarations);
    }
}
