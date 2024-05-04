using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax Syntax { get; }

    private static string InheritFailedMessage(string attribute, IChildNode caller, IChildNode child)
        => $"{attribute} not implemented for child node type {child.GetType().GetFriendlyName()} "
           + $"when accessed through caller {caller.GetType().GetFriendlyName()}.";

    internal virtual ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            InheritFailedMessage(nameof(InheritedContainingSymbolNode), caller, child));


    internal virtual IPackageNode InheritedPackage(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            InheritFailedMessage(nameof(InheritedPackage), caller, child));

    internal virtual CodeFile InheritedFile(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            InheritFailedMessage(nameof(InheritedFile), caller, child));

    internal virtual LexicalScope InheritedLexicalScope(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            InheritFailedMessage(nameof(InheritedLexicalScope), caller, child));

    internal virtual LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            InheritFailedMessage(nameof(InheritedContainingLexicalScope), caller, child));
}
