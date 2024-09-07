using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

public sealed class CollectionAttributeModel : AspectAttributeModel
{
    public override CollectionAttributeSyntax Syntax { get; }
    public override TypeModel Type { get; }
    public Symbol? RootSymbol { get; }
    public TypeModel FromType { get; }
    public string? ConstructExpression => Syntax.ConstructExpression;
    public string DoneMethod => Syntax.DoneMethod;

    public CollectionAttributeModel(AspectModel aspect, CollectionAttributeSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(aspect.Tree, syntax.Type);
        RootSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Root);
        FromType = TypeModel.CreateFromSyntax(aspect.Tree, syntax.FromType);
    }

    public override string ToString()
    {
        var root = RootSymbol is not null ? $" root {RootSymbol}" : "";
        var construct = ConstructExpression is not null ? $" => {ConstructExpression}" : "";
        return $"→*← {NodeSymbol}.{Name}: {Type}{root} from {FromType}{construct} done {DoneMethod}";
    }
}
