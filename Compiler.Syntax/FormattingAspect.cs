using System.Globalization;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

internal static partial class FormattingAspect
{
    #region Top Level
    public static partial string CompilationUnit_ToString(ICompilationUnitSyntax node)
        => node.File.Reference.ToString();

    public static partial string ImportDirective_ToString(IImportDirectiveSyntax node)
        => $"using {node.Name};";
    #endregion

    #region Packages
    public static partial string Package_ToString(IPackageSyntax node)
        => $"package {node.Name.Text}: {node.CompilationUnits.Count} Compilation Units";

    public static partial string PackageReference_ToString(IPackageReferenceSyntax node)
        => $"reference {node.AliasOrName}: {{ \"package\": \"{node.Package.PackageSymbol}\" \"trusted\": {node.IsTrusted} }}";
    #endregion

    #region Namespace Definitions
    public static partial string NamespaceBlockDefinition_ToString(INamespaceBlockDefinitionSyntax node)
        => node.IsGlobalQualified ? $"namespace ::{node.DeclaredNames} {{ … }}" : $"namespace {node.DeclaredNames} {{ … }}";
    #endregion

    #region Function Definition
    public static partial string FunctionDefinition_ToString(IFunctionDefinitionSyntax node)
    {
        var @return = node.Return?.ToString() ?? "";
        return $"fn {node.Name}({string.Join(", ", node.Parameters)}){@return} {node.Body}";
    }
    #endregion

    #region Type Definitions
    public static partial string ClassDefinition_ToString(IClassDefinitionSyntax node)
    {
        var modifiers = "";
        var accessModifier = node.AccessModifier.ToAccessModifier();
        if (accessModifier != AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (node.AbstractModifier is not null) modifiers += "abstract ";
        if (node.ConstModifier is not null) modifiers += "const ";
        if (node.MoveModifier is not null) modifiers += "move ";
        var generics = !node.GenericParameters.IsEmpty ? $"[{string.Join(", ", node.GenericParameters)}]" : "";
        return $"{modifiers}class {node.Name.ToBareString()}{generics} {{ … }}";
    }

    public static partial string StructDefinition_ToString(IStructDefinitionSyntax node)
    {
        var modifiers = "";
        var accessModifier = node.AccessModifier.ToAccessModifier();
        if (accessModifier != AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (node.ConstModifier is not null) modifiers += "const ";
        if (node.MoveModifier is not null) modifiers += "move ";
        var generics = !node.GenericParameters.IsEmpty ? $"[{string.Join(", ", node.GenericParameters)}]" : "";
        return $"{modifiers}class {node.Name.ToBareString()}{generics} {{ … }}";
    }

    public static partial string TraitDefinition_ToString(ITraitDefinitionSyntax node)
    {
        var modifiers = "";
        var accessModifier = node.AccessModifier.ToAccessModifier();
        if (accessModifier != AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (node.ConstModifier is not null) modifiers += "const ";
        if (node.MoveModifier is not null) modifiers += "move ";
        var generics = !node.GenericParameters.IsEmpty ? $"[{string.Join(", ", node.GenericParameters)}]" : "";
        return $"{modifiers}trait {node.Name.ToBareString()}{generics} {{ … }}";
    }
    #endregion

    #region Type Definition Parts
    public static partial string GenericParameter_ToString(IGenericParameterSyntax node)
        => (node.Independence, node.Variance) switch
        {
            (TypeParameterIndependence.None, TypeParameterVariance.Invariant) => node.Name.ToString(),
            (TypeParameterIndependence.None, _) => $"{node.Name} {node.Variance.ToSourceCodeString()}",
            (_, TypeParameterVariance.Invariant) => $"{node.Name} {node.Independence.ToSourceCodeString()}",
            _ => $"{node.Name} {node.Independence.ToSourceCodeString()} {node.Variance.ToSourceCodeString()}"
        };
    #endregion

    #region Member Definitions
    public static partial string AbstractMethodDefinition_ToString(IAbstractMethodDefinitionSyntax node)
    {
        var @return = node.Return?.ToString() ?? "";
        return $"abstract fn {node.Name}({string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter))}){@return};";
    }

    public static partial string OrdinaryMethodDefinition_ToString(IOrdinaryMethodDefinitionSyntax node)
    {
        var @return = node.Return?.ToString() ?? "";
        return $"fn {node.Name}({string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter))}){@return} {node.Body}";
    }

    public static partial string GetterMethodDefinition_ToString(IGetterMethodDefinitionSyntax node)
        => $"get {node.Name}({node.SelfParameter}){node.Return} {node.Body}";

    public static partial string SetterMethodDefinition_ToString(ISetterMethodDefinitionSyntax node)
        => $"set {node.Name}({string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter))}) {node.Body}";

    public static partial string ConstructorDefinition_ToString(IConstructorDefinitionSyntax node)
    {
        var parameters = string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter));
        return node.Name is null ? $"new({parameters})" : $"new {node.Name}({parameters})";
    }

    public static partial string InitializerDefinition_ToString(IInitializerDefinitionSyntax node)
    {
        var parameters = string.Join(", ", node.Parameters.Prepend<IParameterSyntax>(node.SelfParameter));
        return node.Name is null ? $"init({parameters})" : $"init {node.Name}({parameters})";
    }

    public static partial string FieldDefinition_ToString(IFieldDefinitionSyntax node)
    {
        var result = $"{node.Name}: {node.Type}";
        if (node.Initializer is not null)
            result += node.Initializer.ToString();
        result += ";";
        return result;
    }

    public static partial string AssociatedFunctionDefinition_ToString(IAssociatedFunctionDefinitionSyntax node)
    {
        var @return = node.Return?.ToString() ?? "";
        return $"fn {node.Name}({string.Join(", ", node.Parameters)}){@return} {node.Body}";
    }
    #endregion

    #region Attributes
    public static partial string Attribute_ToString(IAttributeSyntax node) => $"#{node.TypeName}";
    #endregion

    #region Capabilities
    public static partial string CapabilitySet_ToString(ICapabilitySetSyntax node)
        => node.CapabilitySet.ToString();

    public static partial string Capability_ToString(ICapabilitySyntax node)
        => node.Capability.ToSourceCodeString();
    #endregion

    #region Parameters and Return
    public static partial string NamedParameter_ToString(INamedParameterSyntax node)
    {
        var lent = node.IsLentBinding ? "lent " : "";
        var mutable = node.IsMutableBinding ? "var " : "";
        var defaultValue = node.DefaultValue is not null ? " = " + node.DefaultValue : "";
        return $"{lent}{mutable}{node.Name}: {node.Type}{defaultValue}";
    }

    public static partial string SelfParameter_ToString(ISelfParameterSyntax node)
    {
        var lent = node.IsLentBinding ? "lent " : "";
        var constraint = node.Constraint.ToString();
        if (!string.IsNullOrEmpty(constraint)) constraint += " ";
        return $"{lent}{constraint}self";
    }

    public static partial string FieldParameter_ToString(IFieldParameterSyntax node)
    {
        var defaultValue = node.DefaultValue is not null ? " = " + node.DefaultValue : "";
        return $".{node.Name}{defaultValue}";
    }

    public static partial string Return_ToString(IReturnSyntax node) => $"-> {node.Type}";
    #endregion

    #region Function Parts
    public static partial string BlockBody_ToString(IBlockBodySyntax node) => "{ … }";

    public static partial string ExpressionBody_ToString(IExpressionBodySyntax node) => "=> …;";
    #endregion

    #region Types
    public static partial string OptionalType_ToString(IOptionalTypeSyntax node)
        => $"{node.Referent}?";

    public static partial string CapabilityType_ToString(ICapabilityTypeSyntax node)
        => $"{node.Capability} {node.Referent}";

    public static partial string FunctionType_ToString(IFunctionTypeSyntax node)
        => $"({string.Join(", ", node.Parameters.Select(p => p.ToString()))}) -> {node.Return}";

    public static partial string ParameterType_ToString(IParameterTypeSyntax node)
        => $"{(node.IsLent ? "lent " : "")}{node.Referent}";

    public static partial string CapabilityViewpointType_ToString(ICapabilityViewpointTypeSyntax node)
        => $"{node.Capability}|>{node.Referent}";

    public static partial string SelfViewpointType_ToString(ISelfViewpointTypeSyntax node)
        => $"self|>{node.Referent}";
    #endregion

    #region Statements
    public static partial string ResultStatement_ToString(IResultStatementSyntax node)
        => $"=> {node.Expression};";

    public static partial string VariableDeclarationStatement_ToString(IVariableDeclarationStatementSyntax node)
    {
        var binding = node.IsMutableBinding ? "var" : "let";
        var type = node.Type is not null ? ": " + node.Type : "";
        if (node.Capability is not null) type = ": " + node.Capability;
        var initializer = node.Initializer is not null ? " = " + node.Initializer : "";
        return $"{binding} {node.Name}{type}{initializer};";
    }

    public static partial string ExpressionStatement_ToString(IExpressionStatementSyntax node)
        => node.Expression + ";";
    #endregion

    #region Patterns
    public static partial string BindingContextPattern_ToString(IBindingContextPatternSyntax node)
    {
        var binding = node.IsMutableBinding ? "var" : "let";
        var type = node.Type is not null ? ": " + node.Type : "";
        return $"{binding} {node.Pattern}{type}";
    }

    public static partial string BindingPattern_ToString(IBindingPatternSyntax node)
        => node.Name.ToString();

    public static partial string OptionalPattern_ToString(IOptionalPatternSyntax node)
        => $"{node.Pattern}?";
    #endregion

    #region Expressions
    public static partial string BlockExpression_ToString(IBlockExpressionSyntax node)
        => node.Statements.IsEmpty ? "{ }" : $"{{ {node.Statements.Count} Statements }}";

    public static partial string NewObjectExpression_ToString(INewObjectExpressionSyntax node)
    {
        var name = node.ConstructorName is not null ? "." + node.ConstructorName : "";
        return $"new {node.Type}{name}({string.Join(", ", node.Arguments)})";
    }

    public static partial string UnsafeExpression_ToString(IUnsafeExpressionSyntax node)
        => $"unsafe ({node.Expression})";
    #endregion

    #region Literal Expressions
    public static partial string BoolLiteralExpression_ToString(IBoolLiteralExpressionSyntax node)
        => node.Value.ToString(CultureInfo.InvariantCulture);

    public static partial string IntegerLiteralExpression_ToString(IIntegerLiteralExpressionSyntax node)
        => node.Value.ToString(CultureInfo.InvariantCulture);

    public static partial string NoneLiteralExpression_ToString(INoneLiteralExpressionSyntax node)
        => "none";

    public static partial string StringLiteralExpression_ToString(IStringLiteralExpressionSyntax node)
        => $"\"{node.Value.Escape()}\"";
    #endregion

    #region Operator Expressions
    public static partial string AssignmentExpression_ToString(IAssignmentExpressionSyntax node)
        => $"{node.LeftOperand.ToGroupedString(node.ExpressionPrecedence)} {node.Operator.ToSymbolString()} {node.RightOperand.ToGroupedString(node.ExpressionPrecedence)}";

    public static partial string BinaryOperatorExpression_ToString(IBinaryOperatorExpressionSyntax node)
        => $"{node.LeftOperand.ToGroupedString(node.ExpressionPrecedence)} {node.Operator.ToSymbolString()} {node.RightOperand.ToGroupedString(node.ExpressionPrecedence)}";

    public static partial string UnaryOperatorExpression_ToString(IUnaryOperatorExpressionSyntax node)
        => node.Fixity switch
        {
            UnaryOperatorFixity.Prefix => $"{node.Operator.ToSymbolString()}{node.Operand.ToGroupedString(node.ExpressionPrecedence)}",
            UnaryOperatorFixity.Postfix =>
                $"{node.Operand.ToGroupedString(node.ExpressionPrecedence)}{node.Operator.ToSymbolString()}",
            _ => throw ExhaustiveMatch.Failed(node.Fixity)
        };

    public static partial string ConversionExpression_ToString(IConversionExpressionSyntax node)
        => $"{node.Referent.ToGroupedString(node.ExpressionPrecedence)} {node.Operator.ToSymbolString()} {node.ConvertToType}";

    public static partial string PatternMatchExpression_ToString(IPatternMatchExpressionSyntax node)
        => $"{node.Referent.ToGroupedString(node.ExpressionPrecedence)} is {node.Pattern}";
    #endregion

    #region Control Flow Expressions
    public static partial string IfExpression_ToString(IIfExpressionSyntax node)
    {
        if (node.ElseClause is not null)
            return $"if {node.Condition} {node.ThenBlock} else {node.ElseClause}";
        return $"if {node.Condition} {node.ThenBlock}";
    }

    public static partial string LoopExpression_ToString(ILoopExpressionSyntax node)
        => $"loop {node.Block}";

    public static partial string WhileExpression_ToString(IWhileExpressionSyntax node)
        => $"while {node.Condition} {node.Block}";

    public static partial string ForeachExpression_ToString(IForeachExpressionSyntax node)
    {
        var binding = node.IsMutableBinding ? "var " : "";
        var type = node.Type is not null ? $": {node.Type} " : "";
        return $"foreach {binding}{node.VariableName}{type} in {node.InExpression} {node.Block}";
    }

    public static partial string BreakExpression_ToString(IBreakExpressionSyntax node)
    {
        if (node.Value is not null)
            return $"break {node.Value}";
        return "break";
    }

    public static partial string NextExpression_ToString(INextExpressionSyntax node) => "next";

    public static partial string ReturnExpression_ToString(IReturnExpressionSyntax node)
        => node.Value is null ? "return" : $"return {node.Value}";
    #endregion

    #region Invocation Expressions
    public static partial string InvocationExpression_ToString(IInvocationExpressionSyntax node)
        => $"{node.Expression}({string.Join(", ", node.Arguments)})";
    #endregion

    #region Name Expressions
    public static partial string SelfExpression_ToString(ISelfExpressionSyntax node)
        => node.IsImplicit ? "⟦self⟧" : "self";

    public static partial string MemberAccessExpression_ToString(IMemberAccessExpressionSyntax node)
        => $"{node.Context.ToGroupedString(node.ExpressionPrecedence)}.{node.QualifiedName}";

    public static partial string MissingName_ToString(IMissingNameSyntax node) => "⧼unknown⧽";
    #endregion

    #region Names
    public static partial string BuiltInTypeName_ToString(IBuiltInTypeNameSyntax node)
        => node.Name.ToString();

    public static partial string IdentifierName_ToString(IIdentifierNameSyntax node)
        => node.Name.ToString();

    public static partial string GenericName_ToString(IGenericNameSyntax node)
        => $"{node.Name.ToBareString()}[{string.Join(", ", node.GenericArguments)}]";

    public static partial string QualifiedName_ToString(IQualifiedNameSyntax node)
        => $"{node.Context}.{node.QualifiedName}";
    #endregion

    #region Capability Expressions
    public static partial string MoveExpression_ToString(IMoveExpressionSyntax node)
        => $"move {node.Referent}";

    public static partial string FreezeExpression_ToString(IFreezeExpressionSyntax node)
        => $"freeze {node.Referent}";
    #endregion

    #region Async Expressions
    public static partial string AsyncBlockExpression_ToString(IAsyncBlockExpressionSyntax node)
        => $"async {node.Block}";

    public static partial string AsyncStartExpression_ToString(IAsyncStartExpressionSyntax node)
    {
        var op = node.Scheduled ? "go" : "do";
        return $"{op} {node.Expression.ToGroupedString(node.ExpressionPrecedence)}";
    }

    public static partial string AwaitExpression_ToString(IAwaitExpressionSyntax node)
        => $"await {node.Expression.ToGroupedString(node.ExpressionPrecedence)}";
    #endregion
}
