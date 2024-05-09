using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericTypeNameNode : TypeNameNode, IGenericTypeNameNode
{
    public override IGenericTypeNameSyntax Syntax { get; }
    public override GenericName Name => Syntax.Name;
    public override TypeSymbol? ReferencedSymbol => SymbolAttribute.GenericTypeName(this);
    public IFixedList<ITypeNode> TypeArguments { get; }
    private ValueAttribute<ITypeSymbolNode?> referencedSymbolNode;
    public ITypeSymbolNode? ReferencedSymbolNode
        => referencedSymbolNode.TryGetValue(out var value) ? value
            : referencedSymbolNode.GetValue(this, SymbolNodeAttributes.StandardTypeName);
    private ValueAttribute<BareType?> bareType;
    public override BareType? BareType
        => bareType.TryGetValue(out var value) ? value
            : bareType.GetValue(this, BareTypeAttribute.GenericTypeName);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeAttribute.GenericTypeName);

    public GenericTypeNameNode(IGenericTypeNameSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.CreateFixed(this, typeArguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        SymbolNodeAttributes.StandardTypeNameContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}