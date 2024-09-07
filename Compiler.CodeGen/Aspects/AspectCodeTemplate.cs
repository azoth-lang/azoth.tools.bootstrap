// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Snippets;
    using ExhaustiveMatching;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class AspectCodeTemplate : AspectCodeTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 11 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  foreach(var usingNamespace in Build.OrderedNamespaces(aspect, "System.CodeDom.Compiler", "System.Runtime.CompilerServices")) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 12 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(usingNamespace));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 13 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 15 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(aspect.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n#nullable enable\r\n// ReSharper disable PartialTypeWithSinglePart\r\n\r\n[Generat" +
                    "edCode(\"AzothCompilerCodeGen\", null)]\r\ninternal static partial class ");
            
            #line 21 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(aspect.Name));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 23 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  foreach (var valid in aspect.Snippets.OfType<ConstructorArgumentValidationModel>()) { 
            
            #line default
            #line hidden
            this.Write("    [MethodImpl(MethodImplOptions.AggressiveInlining)]\r\n    public static partial" +
                    " void Validate_");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ClassName(valid.Node.Defines)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Parameters(Build.PropertiesForClass(valid.Node))));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 26 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 27 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  foreach (var attr in aspect.DeclaredAttributes.OfType<CircularAttributeModel>().Where(ShouldEmit.Initial)) { 
            
            #line default
            #line hidden
            this.Write("    public static partial ");
            
            #line 28 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(attr.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 28 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.InitialMethod(attr)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 28 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(attr.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node);\r\n");
            
            #line 29 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  foreach (var equation in aspect.AllDeclaredEquations.Where(eq => ShouldEmit.EquationPartialImplementation(eq))) { 
            
            #line default
            #line hidden
            this.Write("    [MethodImpl(MethodImplOptions.AggressiveInlining)]\r\n");
            
            #line 32 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
          switch (equation) { 
            
            #line default
            #line hidden
            
            #line 33 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case LocalAttributeEquationModel eq: 
            
            #line default
            #line hidden
            this.Write("    public static partial ");
            
            #line 34 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(eq.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 34 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethod(eq)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 34 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(eq.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node);\r\n");
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  break; 
            
            #line default
            #line hidden
            
            #line 36 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case InheritedAttributeEquationModel eq: 
            
            #line default
            #line hidden
            this.Write("    public static partial ");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(eq.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethod(eq)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(eq.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethodExtraParams(eq)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 38 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  break; 
            
            #line default
            #line hidden
            
            #line 39 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case PreviousAttributeEquationModel eq: 
            
            #line default
            #line hidden
            this.Write("    public static partial ");
            
            #line 40 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(eq.Type)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 40 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethod(eq)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 40 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(eq.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node);\r\n");
            
            #line 41 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  break; 
            
            #line default
            #line hidden
            
            #line 42 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case AggregateAttributeEquationModel eq: 
            
            #line default
            #line hidden
            this.Write("    public static partial void ");
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethod(eq)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(eq.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node");
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethodExtraParams(eq)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 44 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  break; 
            
            #line default
            #line hidden
            
            #line 45 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case CollectionAttributeEquationModel eq: 
            
            #line default
            #line hidden
            this.Write("    public static partial void ");
            
            #line 46 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethod(eq)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 46 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(eq.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node");
            
            #line 46 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EquationMethodExtraParams(eq)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 47 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  break; 
            
            #line default
            #line hidden
            
            #line 48 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              case IntertypeMethodEquationModel _: /* do nothing */ break; 
            
            #line default
            #line hidden
            
            #line 49 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
              default: 
            
            #line default
            #line hidden
            
            #line 50 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
                  throw ExhaustiveMatch.Failed(equation); 
            
            #line default
            #line hidden
            
            #line 51 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
          } 
            
            #line default
            #line hidden
            
            #line 52 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 53 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  foreach (var rule in aspect.RewriteRules) { 
            
            #line default
            #line hidden
            this.Write("    public static partial ");
            
            #line 54 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Type(rule.RewriteToType)));
            
            #line default
            #line hidden
            this.Write("? ");
            
            #line 54 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.RewriteRuleMethod(rule)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 54 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TypeName(rule.Node.Defines)));
            
            #line default
            #line hidden
            this.Write(" node);\r\n");
            
            #line 55 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Aspects\AspectCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("}\r\n");
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
    public class AspectCodeTemplateBase
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
