using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionNameNode : AmbiguousNameExpressionNode, IFunctionNameNode
{
    public override INameExpressionSyntax Syntax { get; }
    private Child<IFunctionGroupNameNode> functionGroup;
    public IFunctionGroupNameNode FunctionGroup => functionGroup.Value;
    public StandardName FunctionName => FunctionGroup.FunctionName;
    public IFixedList<ITypeNode> TypeArguments => FunctionGroup.TypeArguments;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.FunctionName_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.FunctionName_Type, ref type);
    public override FlowState FlowStateAfter
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    public FunctionNameNode(
        INameExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IFunctionLikeDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.functionGroup = Child.Create(this, functionGroup);
        ReferencedDeclaration = referencedDeclaration;
    }
}
