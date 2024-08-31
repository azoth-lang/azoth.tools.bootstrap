using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class PreviousAttributeEquationModel : ContributorEquationModel
{
    public override PreviousAttributeEquationSyntax Syntax { get; }
    public PreviousAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<PreviousAttributeFamilyModel> attributeFamily;
    public override TypeModel Type => AttributeFamily.Type;

    public PreviousAttributeEquationModel(
        AspectModel aspect,
        PreviousAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        attributeFamily = new(ComputeAttributeFamily<PreviousAttributeFamilyModel>);
    }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.⮡.{Name}{parameters}";
    }
}
