using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FieldParameterNode : ParameterNode, IFieldParameterNode
{
    public override IFieldParameterSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    private ValueAttribute<ITypeDeclarationNode> containingTypeDeclaration;
    public ITypeDeclarationNode ContainingTypeDeclaration
        => containingTypeDeclaration.TryGetValue(out var value) ? value
            : containingTypeDeclaration.GetValue(InheritedContainingTypeDeclaration);
    private ValueAttribute<IFieldSymbolNode?> referencedSymbolNode;
    public IFieldSymbolNode? ReferencedSymbolNode
        => referencedSymbolNode.TryGetValue(out var value) ? value
            : referencedSymbolNode.GetValue(this, SymbolNodeAttributes.FieldParameter_ReferencedSymbolNode);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, InvocableDeclarationsAspect.FieldParameter_Type);
    private ValueAttribute<Parameter> parameterType;
    public Parameter ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, InvocableDeclarationsAspect.FieldParameter_ParameterType);

    public FieldParameterNode(IFieldParameterSyntax syntax)
    {
        Syntax = syntax;
    }
}
