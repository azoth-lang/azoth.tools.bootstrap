using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeSupertypes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class AspectModel : IHasUsingNamespaces
{
    public TreeModel Tree { get; }
    public AspectSyntax Syntax { get; }
    public string Namespace => Syntax.Namespace;
    public string Name => Syntax.Name;
    public IFixedSet<string> UsingNamespaces => Syntax.UsingNamespaces;
    public IFixedSet<TypeDeclarationModel> TypeDeclarations { get; }
    public IFixedSet<InheritedAttributeSupertypeModel> DeclaredAttributeSupertypes { get; }
    public IFixedList<AspectAttributeModel> Attributes { get; }
    public IFixedList<EquationModel> DeclaredEquations { get; }
    public IFixedList<SynthesizedAttributeEquationModel> ImplicitlyDeclaredEquations => implicitlyDeclaredEquations.Value;
    private readonly Lazy<IFixedList<SynthesizedAttributeEquationModel>> implicitlyDeclaredEquations;
    public IEnumerable<EquationModel> AllDeclaredEquations => DeclaredEquations.Concat(ImplicitlyDeclaredEquations);

    public AspectModel(TreeModel tree, AspectSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        TypeDeclarations = syntax.TypeDeclarations.Select(t => new TypeDeclarationModel(tree, t)).ToFixedSet();
        DeclaredAttributeSupertypes = syntax.AttributeSupertypes.Select(s => new InheritedAttributeSupertypeModel(Tree, s)).ToFixedSet();
        Attributes = syntax.Attributes.Select(a => AspectAttributeModel.Create(this, a)).ToFixedList();
        DeclaredEquations = syntax.Equations.Select(e => EquationModel.Create(this, e)).ToFixedList();
        implicitlyDeclaredEquations = new(() => ComputeImplicitlyDeclaredEquations().ToFixedList());
    }

    private IEnumerable<SynthesizedAttributeEquationModel> ComputeImplicitlyDeclaredEquations()
        => Tree.Nodes.SelectMany(n => n.ImplicitlyDeclaredEquations.Where(e => e.Aspect == this));
}
