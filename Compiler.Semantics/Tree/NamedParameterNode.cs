using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NamedParameterNode : ParameterNode, INamedParameterNode
{
    public override INamedParameterSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public bool IsLentBinding => Syntax.IsLentBinding;
    public override IdentifierName Name => Syntax.Name;
    public int? DeclarationNumber => throw new System.NotImplementedException();
    public ITypeNode TypeNode { get; }
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, InvocableDeclarationsAspect.NamedParameterNode_Type);
    private ValueAttribute<Parameter> parameterType;
    public Parameter ParameterType
        => parameterType.TryGetValue(out var value) ? value
            : parameterType.GetValue(this, InvocableDeclarationsAspect.NamedParameter_ParameterType);

    public NamedParameterNode(INamedParameterSyntax syntax, ITypeNode type)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, type);
    }
}
