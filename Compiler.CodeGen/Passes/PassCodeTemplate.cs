// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Passes
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class PassCodeTemplate : PassCodeTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 7 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  foreach(var usingNamespace in Build.OrderedNamespaces(pass, "ExhaustiveMatching", "System.Linq", "System.Collections.Generic", "System.CodeDom.Compiler", "Azoth.Tools.Bootstrap.Compiler.IST", "Azoth.Tools.Bootstrap.Framework")) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 8 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(usingNamespace));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 9 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 9 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.UsingAlias("From", pass.FromLanguage)));
            
            #line default
            #line hidden
            
            #line 9 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.UsingAlias("To", pass.ToLanguage)));
            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 10 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pass.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n[GeneratedCode(\"AzothCompilerCodeGen\", null)]\r\ninternal sealed partial class" +
                    " ");
            
            #line 13 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pass.Name));
            
            #line default
            #line hidden
            this.Write(" : ITransformPass<");
            
            #line 13 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TransformInterfaceTypeParameters(pass)));
            
            #line default
            #line hidden
            this.Write(">\r\n{\r\n    public static ");
            
            #line 15 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.RunReturnType(pass)));
            
            #line default
            #line hidden
            this.Write(" Run(");
            
            #line 15 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.RunParameters(pass)));
            
            #line default
            #line hidden
            this.Write(")\r\n    {\r\n        var pass = new ");
            
            #line 17 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pass.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 17 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ContextParameterName(pass)));
            
            #line default
            #line hidden
            this.Write(");\r\n        pass.StartRun();\r\n        ");
            
            #line 19 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EntryResult(pass)));
            
            #line default
            #line hidden
            this.Write("pass.");
            
            #line 19 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.MethodName(pass)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 19 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EntryParameterNames(pass)));
            
            #line default
            #line hidden
            this.Write(");\r\n        ");
            
            #line 20 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EndRunResult(pass)));
            
            #line default
            #line hidden
            this.Write("pass.EndRun(");
            
            #line 20 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EndRunParameterNames(pass)));
            
            #line default
            #line hidden
            this.Write(");\r\n        return ");
            
            #line 21 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.RunReturnNames(pass)));
            
            #line default
            #line hidden
            this.Write(";\r\n    }\r\n\r\n");
            
            #line 24 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  if (ShouldEmit.RunExplicitImplementation(pass)) { 
            
            #line default
            #line hidden
            this.Write("    static ");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.FullRunReturnType(pass)));
            
            #line default
            #line hidden
            this.Write(" ITransformPass<");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TransformInterfaceTypeParameters(pass)));
            
            #line default
            #line hidden
            this.Write(">.Run(");
            
            #line 25 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.FullRunParameters(pass)));
            
            #line default
            #line hidden
            this.Write(")\r\n        => ");
            
            #line 26 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.RunForward(pass)));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n");
            
            #line 28 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n    ");
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.StartRunAccessModifier(pass)));
            
            #line default
            #line hidden
            this.Write("partial ");
            
            #line 30 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.StartRunReturnType(pass)));
            
            #line default
            #line hidden
            this.Write(" StartRun();\r\n\r\n    ");
            
            #line 32 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EndRunAccessModifier(pass)));
            
            #line default
            #line hidden
            this.Write("partial ");
            
            #line 32 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EndRunReturnType(pass)));
            
            #line default
            #line hidden
            this.Write(" EndRun(");
            
            #line 32 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.EndRunParameters(pass)));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 34 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  foreach (var transform in pass.Transforms.Where(t => !t.AutoGenerate)) { 
            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.AccessModifier(transform)));
            
            #line default
            #line hidden
            this.Write("partial ");
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ReturnType(transform)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.MethodName(pass)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 35 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Parameters(transform)));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 37 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 38 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  foreach (var transform in pass.Transforms.Where(t => t.AutoGenerate)) { 
            
            #line default
            #line hidden
            this.Write("    private ");
            
            #line 39 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ReturnType(transform)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 39 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.MethodName(pass)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 39 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.Parameters(transform)));
            
            #line default
            #line hidden
            this.Write(")\r\n        => ");
            
            #line 40 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.TransformMethodBody(transform)));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n");
            
            #line 42 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 43 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
  foreach (var rule in Build.SimpleCreateRules(pass)) { 
            
            #line default
            #line hidden
            this.Write("    private ");
            
            #line 44 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.PassTypeName(pass, rule.Defines)));
            
            #line default
            #line hidden
            this.Write(" Create(");
            
            #line 44 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.ModifiedParameters(pass, rule)));
            
            #line default
            #line hidden
            this.Write(")\r\n        => ");
            
            #line 45 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.PassTypeName(pass, rule.Defines)));
            
            #line default
            #line hidden
            this.Write(".Create(");
            
            #line 45 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Emit.SimpleCreateParameters(rule)));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 47 "C:\dataFast\azoth-lang\azoth.tools.bootstrap\Compiler.CodeGen\Passes\PassCodeTemplate.tt"
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
    public class PassCodeTemplateBase
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
