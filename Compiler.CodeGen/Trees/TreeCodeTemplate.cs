// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class TreeCodeTemplate : TreeCodeTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 10 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  foreach(var usingNamespace in Build.OrderedNamespaces(tree,
    !tree.SimplifiedTree, new [] { "System", "System.Linq", "System.Threading", "Azoth.Tools.Bootstrap.Compiler.Core.Attributes" },
    "ExhaustiveMatching", "System.CodeDom.Compiler", "System.Diagnostics")) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 13 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(usingNamespace));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 14 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 16 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tree.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n// ReSharper disable PartialTypeWithSinglePart\r\n// ReSharper disable Redunda" +
                    "ntTypeDeclarationBody\r\n// ReSharper disable ReturnTypeCanBeNotNullable\r\n// ReSha" +
                    "rper disable ConvertToPrimaryConstructor\r\n\r\n");
            
            #line 23 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  foreach(var node in tree.Nodes) {
            
            #line default
            #line hidden
            
            #line 24 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ClosedAttribute(node)));
            
            #line default
            #line hidden
            this.Write("[GeneratedCode(\"AzothCompilerCodeGen\", null)]\r\npublic partial interface ");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(node.Defines)));
            
            #line default
            #line hidden
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseTypes(node)));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 27 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      foreach (var attribute in node.AttributesRequiringDeclaration) { 
            
            #line default
            #line hidden
            
            #line 28 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          switch (attribute) { 
            
            #line default
            #line hidden
            
            #line 29 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
              case PropertyModel attr: 
            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.IsNew(attr)));
            
            #line default
            #line hidden
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(attr.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attr.Name));
            
            #line default
            #line hidden
            this.Write(" { get; }\r\n");
            
            #line 31 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
                  break;
            
            #line default
            #line hidden
            
            #line 32 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
              case SynthesizedAttributeModel attr: 
            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 33 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(attr.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 33 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attr.Name));
            
            #line default
            #line hidden
            
            #line 33 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attr.Parameters));
            
            #line default
            #line hidden
            
            #line 33 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Body(attr)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 34 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
                  break;
            
            #line default
            #line hidden
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          } 
            
            #line default
            #line hidden
            
            #line 36 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          foreach (var baseAttribute in Build.BaseAttributes(node, attribute)) { 
            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(baseAttribute.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(baseAttribute.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(baseAttribute.Name));
            
            #line default
            #line hidden
            this.Write(" => ");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 38 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          } 
            
            #line default
            #line hidden
            
            #line 39 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 40 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      if (tree.GenerateClasses && !node.IsAbstract) { 
            
            #line default
            #line hidden
            this.Write("\r\n    public static ");
            
            #line 42 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(node.Defines)));
            
            #line default
            #line hidden
            this.Write(" Create(");
            
            #line 42 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", node.ActualProperties.Select(p => $"{Emit.Type(p.Type)} {Emit.ParameterName(p)}"))));
            
            #line default
            #line hidden
            this.Write(")\r\n        => new ");
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ClassName(node.Defines)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", node.ActualProperties.Select(p => Emit.ParameterName(p)))));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 44 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("}\r\n\r\n");
            
            #line 47 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 48 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  if (tree.GenerateClasses) { 
            
            #line default
            #line hidden
            
            #line 49 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  if (!tree.SimplifiedTree) { 
            
            #line default
            #line hidden
            this.Write("// TODO switch back to `file` and not `partial` once fully transitioned\r\ninternal" +
                    " abstract partial class ");
            
            #line 51 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(" : TreeNode, IChildTreeNode<");
            
            #line 51 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(tree.RootSupertype)));
            
            #line default
            #line hidden
            this.Write(">\r\n{\r\n    private ");
            
            #line 53 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write("? parent;\r\n\r\n    protected ");
            
            #line 55 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write("() { }\r\n    protected ");
            
            #line 56 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(@"(bool inFinalTree) : base(inFinalTree) { }

    [DebuggerStepThrough]
    protected sealed override ITreeNode PeekParent()
        // Use volatile read to ensure order of operations as seen by other threads
        => Volatile.Read(in parent) ?? throw Child.ParentMissing(this);

    protected ");
            
            #line 63 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(@" GetParent(IInheritanceContext ctx)
    {
        // Use volatile read to ensure order of operations as seen by other threads
        var node = Volatile.Read(in parent) ?? throw Child.ParentMissing(this);
        ctx.AccessParentOf(this);
        return node;
    }

    void IChildTreeNode<");
            
            #line 71 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(tree.RootSupertype)));
            
            #line default
            #line hidden
            this.Write(">.SetParent(");
            
            #line 71 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(tree.RootSupertype)));
            
            #line default
            #line hidden
            this.Write(" newParent)\r\n    {\r\n        if (newParent is not ");
            
            #line 73 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(" newParentNode)\r\n            throw new ArgumentException($\"Parent must be a {name" +
                    "of(");
            
            #line 74 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(@")}."", nameof(newParent));

        // Use volatile write to ensure order of operations as seen by other threads
        Volatile.Write(ref parent, newParentNode);
    }

    /// <summary>
    /// The previous node to this one in a preorder traversal of the tree.
    /// </summary>
    protected virtual ");
            
            #line 83 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(" Previous(IInheritanceContext ctx)\r\n    {\r\n        ");
            
            #line 85 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write("? previous = null;\r\n        var parent = GetParent(ctx);\r\n        foreach (var ch" +
                    "ild in parent.Children().Cast<");
            
            #line 87 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(@">())
        {
            if (child == this)
                // If this is the first child, return the parent without descending
                return previous?.LastDescendant() ?? parent;
            previous = child;
        }

        throw new UnreachableException(""Node is not a child of its parent."");
    }

    // TODO can this be more efficent?
    internal ");
            
            #line 99 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write(" LastDescendant()\r\n        => ((");
            
            #line 100 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClassName(tree)));
            
            #line default
            #line hidden
            this.Write("?)Children().LastOrDefault())?.LastDescendant() ?? this;\r\n}\r\n\r\n");
            
            #line 103 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 104 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  foreach(var node in tree.Nodes.Where(ShouldEmit.Class)) {
            
            #line default
            #line hidden
            this.Write("[GeneratedCode(\"AzothCompilerCodeGen\", null)]\r\nfile class ");
            
            #line 106 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ClassName(node.Defines)));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 106 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.BaseClass(tree)));
            
            #line default
            #line hidden
            
            #line 106 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(node.Defines)));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 108 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      foreach (var property in node.ActualProperties) { 
            
            #line default
            #line hidden
            this.Write("    public ");
            
            #line 109 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(property.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 109 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" { [DebuggerStepThrough] get; }\r\n");
            
            #line 110 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 111 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      foreach (var equation in node.ActualEquations) { 
            
            #line default
            #line hidden
            
            #line 112 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          switch (equation) { 
            
            #line default
            #line hidden
            
            #line 113 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
              case SynthesizedAttributeEquationModel eq when eq.Strategy == EvaluationStrategy.Eager: 
            
            #line default
            #line hidden
            this.Write("    public ");
            
            #line 114 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(eq.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 114 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eq.Name));
            
            #line default
            #line hidden
            this.Write(" { [DebuggerStepThrough] get; }\r\n");
            
            #line 115 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
                  break;
            
            #line default
            #line hidden
            
            #line 116 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
              case SynthesizedAttributeEquationModel eq when eq.Strategy != EvaluationStrategy.Eager: 
            
            #line default
            #line hidden
            this.Write("    public ");
            
            #line 117 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Override(eq)));
            
            #line default
            #line hidden
            
            #line 117 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(eq.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 117 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eq.Name));
            
            #line default
            #line hidden
            
            #line 117 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(eq.Parameters));
            
            #line default
            #line hidden
            
            #line 117 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Body(eq)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 118 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
                  break;
            
            #line default
            #line hidden
            
            #line 119 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
          } 
            
            #line default
            #line hidden
            
            #line 120 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("\r\n    public ");
            
            #line 122 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ClassName(node.Defines)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 122 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", node.ActualProperties.Select(p => $"{Emit.Type(p.Type)} {Emit.ParameterName(p)}"))));
            
            #line default
            #line hidden
            this.Write(")\r\n    {\r\n");
            
            #line 124 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      foreach (var property in node.ActualProperties) { 
            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 125 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 125 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ParameterName(property)));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 126 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 127 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      foreach (var equation in node.ActualEquations.OfType<SynthesizedAttributeEquationModel>().Where(eq => eq.Strategy == EvaluationStrategy.Eager)) { 
            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 128 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(equation.Name));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 128 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EagerBody(equation)));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 129 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("    }\r\n}\r\n\r\n");
            
            #line 133 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 134 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Trees\TreeCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class TreeCodeTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        public System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
