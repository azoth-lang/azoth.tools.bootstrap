using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : PackageMemberDeclarationNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    public override INamespaceSymbolNode ContainingSymbolNode => (INamespaceSymbolNode)base.ContainingSymbolNode;
    public override NamespaceSymbol ContainingSymbol => (NamespaceSymbol)base.ContainingSymbol;
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.FunctionDeclaration);

    private ValueAttribute<IFunctionSymbolNode> symbolNode;
    public override IFunctionSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.FunctionDeclaration);
    private ValueAttribute<FunctionSymbol> symbol;
    public override FunctionSymbol Symbol
        => symbol.TryGetValue(out var value) ? value
            : symbol.GetValue(this, SymbolAttribute.FunctionDeclaration);
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    private ValueAttribute<FunctionType> type;
    public FunctionType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, InvocableDeclarationsAspect.FunctionDeclaration_Type);

    public FunctionDeclarationNode(
        IFunctionDeclarationSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        : base(attributes)
    {
        Syntax = syntax;
        Parameters = ChildList.CreateFixed(this, parameters);
        Return = Child.Attach(this, @return);
    }

    internal override bool InheritedIsAttributeType(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.FunctionDeclaration_InheritedIsAttributeType(this);
}
