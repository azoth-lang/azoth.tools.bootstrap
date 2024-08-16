using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;

/// <summary>
/// The semantic model for an attribute.
/// </summary>
[Closed(typeof(AspectAttributeModel), typeof(PropertyModel))]
public abstract class AttributeModel
{
    public abstract AttributeSyntax? Syntax { get; }
    public abstract string Name { get; }
}
