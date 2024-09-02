using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
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
    public IFixedSet<AttributeFamilyModel> DeclaredAttributeKins { get; }
    public IFixedList<AspectAttributeModel> DeclaredAttributes { get; }
    public IFixedList<EquationModel> DeclaredEquations { get; }
    public IFixedList<LocalAttributeEquationModel> ImplicitlyDeclaredEquations => implicitlyDeclaredEquations.Value;
    private readonly Lazy<IFixedList<LocalAttributeEquationModel>> implicitlyDeclaredEquations;
    public IEnumerable<EquationModel> AllDeclaredEquations => DeclaredEquations.Concat(ImplicitlyDeclaredEquations);
    public IFixedList<RewriteRuleModel> RewriteRules { get; }

    public AspectModel(TreeModel tree, AspectSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        TypeDeclarations = syntax.TypeDeclarations.Select(t => new TypeDeclarationModel(tree, t)).ToFixedSet();
        DeclaredAttributeKins = syntax.AttributeFamilies.Select(s => AttributeFamilyModel.Create(Tree, s)).ToFixedSet();
        DeclaredAttributes = syntax.Attributes.Select(a => AspectAttributeModel.Create(this, a)).ToFixedList();
        DeclaredEquations = syntax.Equations.Select(e => EquationModel.Create(this, e)).ToFixedList();
        implicitlyDeclaredEquations = new(() => ComputeImplicitlyDeclaredEquations().ToFixedList());
        RewriteRules = syntax.RewriteRules.Select(r => new RewriteRuleModel(this, r)).ToFixedList();
    }

    private IEnumerable<LocalAttributeEquationModel> ComputeImplicitlyDeclaredEquations()
        => Tree.Nodes.SelectMany(n => n.ImplicitlyDeclaredEquations.Where(e => e.Aspect == this));
}
