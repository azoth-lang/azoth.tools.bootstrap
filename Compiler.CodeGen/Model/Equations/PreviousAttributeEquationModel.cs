using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class PreviousAttributeEquationModel : ContributorEquationModel
{
    public override PreviousAttributeEquationSyntax Syntax { get; }
    public PreviousAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<PreviousAttributeFamilyModel> attributeFamily;
    public override PreviousAttributeModel? Attribute => null;
    public override TypeModel Type => AttributeFamily.Type;

    public PreviousAttributeEquationModel(
        AspectModel aspect,
        PreviousAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name,
            syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        attributeFamily = new(ComputeAttributeFamily);
    }

    private PreviousAttributeFamilyModel ComputeAttributeFamily()
        => Aspect.Tree.AllAttributeFamilies.OfType<PreviousAttributeFamilyModel>()
                 .Where(f => f.Name == Name).TrySingle()
                 ?? throw new FormatException($"No attribute family for attribute equation {this}");

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.⮡.{Name}{parameters}";
    }
}
