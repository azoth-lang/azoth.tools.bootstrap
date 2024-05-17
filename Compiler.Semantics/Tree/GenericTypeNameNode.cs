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
    private ValueAttribute<bool> attributeType;
    public bool IsAttributeType
        => attributeType.TryGetValue(out var value) ? value
            : attributeType.GetValue(InheritedIsAttributeType);
    public override GenericName Name => Syntax.Name;
    public override TypeSymbol? ReferencedSymbol => SymbolAttribute.StandardTypeName(this);
    public IFixedList<ITypeNode> TypeArguments { get; }
    private ValueAttribute<ITypeDeclarationNode?> referencedDeclaration;
    public ITypeDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, SymbolNodeAttributes.StandardTypeName_ReferencedDeclaration);
    private ValueAttribute<BareType?> bareType;
    public override BareType? BareType
        => bareType.TryGetValue(out var value) ? value
            : bareType.GetValue(this, BareTypeAttribute.GenericTypeName);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.TypeName_Type);

    public GenericTypeNameNode(IGenericTypeNameSyntax syntax, IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        SymbolNodeAttributes.StandardTypeName_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
