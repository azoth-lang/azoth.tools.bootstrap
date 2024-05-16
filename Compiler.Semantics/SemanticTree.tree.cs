using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(IChildNode),
    typeof(IBodyOrBlockNode),
    typeof(IElseClauseNode),
    typeof(IBindingNode),
    typeof(IPackageNode),
    typeof(IPackageMemberDefinitionNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDefinitionNode),
    typeof(IConcreteInvocableDefinitionNode),
    typeof(INamespaceDefinitionNode),
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode),
    typeof(IGenericParameterNode),
    typeof(IClassMemberDefinitionNode),
    typeof(ITraitMemberDefinitionNode),
    typeof(IStructMemberDefinitionNode),
    typeof(IAlwaysTypeMemberDefinitionNode),
    typeof(IAbstractMethodDefinitionNode),
    typeof(IStandardMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode),
    typeof(IAttributeNode),
    typeof(ICapabilityConstraintNode),
    typeof(IParameterNode),
    typeof(INamedParameterNode),
    typeof(IConstructorSelfParameterNode),
    typeof(IInitializerSelfParameterNode),
    typeof(IMethodSelfParameterNode),
    typeof(IFieldParameterNode),
    typeof(ITypeNode),
    typeof(IStandardTypeNameNode),
    typeof(ISimpleTypeNameNode),
    typeof(IQualifiedTypeNameNode),
    typeof(IParameterTypeNode),
    typeof(ICapabilityViewpointTypeNode),
    typeof(ISelfViewpointTypeNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode),
    typeof(IUntypedExpressionNode),
    typeof(IAssignableExpressionNode),
    typeof(INewObjectExpressionNode),
    typeof(IUnsafeExpressionNode),
    typeof(INeverTypedExpressionNode),
    typeof(ILiteralExpressionNode),
    typeof(IAssignmentExpressionNode),
    typeof(IBinaryOperatorExpressionNode),
    typeof(IUnaryOperatorExpressionNode),
    typeof(IIdExpressionNode),
    typeof(IConversionExpressionNode),
    typeof(IPatternMatchExpressionNode),
    typeof(ILoopExpressionNode),
    typeof(IWhileExpressionNode),
    typeof(IForeachExpressionNode),
    typeof(IInvocationExpressionNode),
    typeof(IInvocableNameExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IStandardNameExpressionNode),
    typeof(ISimpleNameExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface ISemanticNode
{
    ISyntax? Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(IPackageFacetNode),
    typeof(ICodeNode))]
public partial interface IChildNode : IChild<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
    IPackageNode Package { get; }
}

[Closed(
    typeof(IBodyNode),
    typeof(IBlockExpressionNode))]
public partial interface IBodyOrBlockNode : ISemanticNode, ICodeNode
{
}

[Closed(
    typeof(IBlockOrResultNode),
    typeof(IIfExpressionNode))]
public partial interface IElseClauseNode : ISemanticNode, ICodeNode
{
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBlockExpressionNode))]
public partial interface IBlockOrResultNode : IElseClauseNode
{
}

[Closed(
    typeof(ILocalBindingNode),
    typeof(IFieldDefinitionNode))]
public partial interface IBindingNode : ISemanticNode, ICodeNode
{
    bool IsMutableBinding { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
public partial interface ILocalBindingNode : IBindingNode
{
}

public partial interface IPackageNode : ISemanticNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IdentifierName Name { get; }
    PackageSymbol Symbol { get; }
    IPackageSymbolNode SymbolNode { get; }
    IFixedSet<IPackageReferenceNode> References { get; }
    IPackageReferenceNode IntrinsicsReference { get; }
    FixedDictionary<IdentifierName,IPackageSymbolNode> SymbolNodes { get; }
    IPackageFacetNode MainFacet { get; }
    IPackageFacetNode TestingFacet { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
}

public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPackageSymbolNode SymbolNode { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols PackageSymbols { get; }
    bool IsTrusted { get; }
}

public partial interface IPackageFacetNode : IChildNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IdentifierName PackageName { get; }
    PackageSymbol PackageSymbol { get; }
    IPackageFacetSymbolNode SymbolNode { get; }
    PackageNameScope PackageNameScope { get; }
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    IFixedSet<IPackageMemberDefinitionNode> Definitions { get; }
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode))]
public partial interface IPackageMemberDefinitionNode : ISemanticNode, INamespaceMemberDefinitionNode
{
    IFixedList<IAttributeNode> Attributes { get; }
    AccessModifier AccessModifier { get; }
    new IPackageMemberSymbolNode SymbolNode { get; }
    INamespaceMemberSymbolNode INamespaceMemberDefinitionNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(IBodyOrBlockNode),
    typeof(IElseClauseNode),
    typeof(IBindingNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDefinitionNode),
    typeof(IGenericParameterNode),
    typeof(IAttributeNode),
    typeof(ICapabilityConstraintNode),
    typeof(IParameterNode),
    typeof(ITypeNode),
    typeof(IParameterTypeNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IUntypedExpressionNode))]
public partial interface ICodeNode : IChildNode
{
    new IConcreteSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    CodeFile File { get; }
}

public partial interface ICompilationUnitNode : ISemanticNode, ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IPackageFacetSymbolNode ContainingSymbolNode { get; }
    NamespaceSymbol ContainingSymbol { get; }
    NamespaceName ImplicitNamespaceName { get; }
    INamespaceSymbolNode ImplicitNamespaceSymbolNode { get; }
    NamespaceSymbol ImplicitNamespaceSymbol { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDefinitionNode> Definitions { get; }
    NamespaceScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
}

public partial interface IUsingDirectiveNode : ISemanticNode, ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    NamespaceName Name { get; }
}

[Closed(
    typeof(IInvocableDefinitionNode),
    typeof(INamespaceMemberDefinitionNode),
    typeof(ITypeMemberDefinitionNode))]
public partial interface IDefinitionNode : ISemanticNode, ICodeNode
{
    new IDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ISymbolNode ContainingSymbolNode { get; }
    Symbol ContainingSymbol { get; }
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    IDeclarationSymbolNode SymbolNode { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IMethodDefinitionNode))]
public partial interface IInvocableDefinitionNode : IDefinitionNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
}

[Closed(
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IConcreteInvocableDefinitionNode : ISemanticNode, IInvocableDefinitionNode
{
    IBodyNode Body { get; }
}

public partial interface INamespaceDefinitionNode : ISemanticNode, INamespaceMemberDefinitionNode
{
    new INamespaceDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDefinitionNode> Definitions { get; }
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDefinitionNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new INamespaceSymbolNode SymbolNode { get; }
    INamespaceMemberSymbolNode INamespaceMemberDefinitionNode.SymbolNode => SymbolNode;
    NamespaceSymbol Symbol { get; }
}

[Closed(
    typeof(IPackageMemberDefinitionNode),
    typeof(INamespaceDefinitionNode))]
public partial interface INamespaceMemberDefinitionNode : IDefinitionNode
{
    new INamespaceMemberSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDefinitionNode.SymbolNode => SymbolNode;
}

public partial interface IFunctionDefinitionNode : IPackageMemberDefinitionNode
{
    new IFunctionDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDefinitionNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    IdentifierName Name { get; }
    new IFunctionSymbolNode SymbolNode { get; }
    IPackageMemberSymbolNode IPackageMemberDefinitionNode.SymbolNode => SymbolNode;
    FunctionSymbol Symbol { get; }
    IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    IBodyNode Body { get; }
    FunctionType Type { get; }
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
public partial interface ITypeDefinitionNode : IPackageMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode
{
    new ITypeDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    bool IsConst { get; }
    StandardName Name { get; }
    IDeclaredUserType DeclaredType { get; }
    new IUserTypeSymbolNode SymbolNode { get; }
    IPackageMemberSymbolNode IPackageMemberDefinitionNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDefinitionNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDefinitionNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDefinitionNode.SymbolNode => SymbolNode;
    INamespaceMemberSymbolNode INamespaceMemberDefinitionNode.SymbolNode => SymbolNode;
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
    UserTypeSymbol Symbol { get; }
    IFixedList<IGenericParameterNode> GenericParameters { get; }
    LexicalScope SupertypesLexicalScope { get; }
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    CompilerResult<IFixedSet<BareReferenceType>> Supertypes { get; }
    IFixedList<ITypeMemberDefinitionNode> Members { get; }
}

public partial interface IClassDefinitionNode : ISemanticNode, ITypeDefinitionNode
{
    new IClassDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    bool IsAbstract { get; }
    IStandardTypeNameNode? BaseTypeName { get; }
    new IClassSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDefinitionNode.SymbolNode => SymbolNode;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedList<IClassMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    ConstructorSymbol? DefaultConstructorSymbol { get; }
}

public partial interface IStructDefinitionNode : ISemanticNode, ITypeDefinitionNode
{
    new IStructDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    new IStructSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDefinitionNode.SymbolNode => SymbolNode;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedList<IStructMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
}

public partial interface ITraitDefinitionNode : ISemanticNode, ITypeDefinitionNode
{
    new ITraitDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    new ITraitSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDefinitionNode.SymbolNode => SymbolNode;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedList<ITraitMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
}

public partial interface IGenericParameterNode : ISemanticNode, ICodeNode
{
    new IGenericParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    IdentifierName Name { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
    GenericParameter Parameter { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    GenericParameterType DeclaredType { get; }
    IUserTypeSymbolNode ContainingSymbolNode { get; }
    UserTypeSymbol ContainingSymbol { get; }
    IGenericParameterSymbolNode SymbolNode { get; }
    GenericParameterTypeSymbol Symbol { get; }
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(ITraitMemberDefinitionNode),
    typeof(IStructMemberDefinitionNode),
    typeof(IAlwaysTypeMemberDefinitionNode))]
public partial interface ITypeMemberDefinitionNode : IDefinitionNode
{
    new ITypeMemberDeclarationSyntax Syntax { get; }
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    AccessModifier AccessModifier { get; }
    new ITypeMemberSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDefinitionNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IClassMemberDefinitionNode : ISemanticNode, ITypeMemberDefinitionNode
{
    new IClassMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    new IClassMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface ITraitMemberDefinitionNode : ISemanticNode, ITypeMemberDefinitionNode
{
    new ITraitMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    new ITraitMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IStructMemberDefinitionNode : ISemanticNode, ITypeMemberDefinitionNode
{
    new IStructMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    new IStructMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IAlwaysTypeMemberDefinitionNode : ISemanticNode, ITypeMemberDefinitionNode
{
    new UserTypeSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode))]
public partial interface IMethodDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IInvocableDefinitionNode
{
    new IMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    MethodKind Kind { get; }
    IdentifierName Name { get; }
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IMethodSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDefinitionNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDefinitionNode.SymbolNode => SymbolNode;
    IDeclarationSymbolNode IDefinitionNode.SymbolNode => SymbolNode;
    MethodSymbol Symbol { get; }
}

public partial interface IAbstractMethodDefinitionNode : ISemanticNode, IMethodDefinitionNode
{
    new IAbstractMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    ObjectType ContainingDeclaredType { get; }
}

[Closed(
    typeof(IStandardMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode))]
public partial interface IConcreteMethodDefinitionNode : IMethodDefinitionNode, IStructMemberDefinitionNode, IConcreteInvocableDefinitionNode
{
    new IConcreteMethodDeclarationSyntax Syntax { get; }
    IMethodDeclarationSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<INamedParameterNode> IMethodDefinitionNode.Parameters => Parameters;
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
}

public partial interface IStandardMethodDefinitionNode : ISemanticNode, IConcreteMethodDefinitionNode
{
    new IStandardMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
}

public partial interface IGetterMethodDefinitionNode : ISemanticNode, IConcreteMethodDefinitionNode
{
    new IGetterMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    new ITypeNode Return { get; }
}

public partial interface ISetterMethodDefinitionNode : ISemanticNode, IConcreteMethodDefinitionNode
{
    new ISetterMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
}

public partial interface IConstructorDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode
{
    new IConstructorDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    IConstructorSelfParameterNode SelfParameter { get; }
    ConstructorSymbol Symbol { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode IConcreteInvocableDefinitionNode.Body => Body;
}

public partial interface IInitializerDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode
{
    new IInitializerDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    IInitializerSelfParameterNode SelfParameter { get; }
    InitializerSymbol Symbol { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode IConcreteInvocableDefinitionNode.Body => Body;
}

public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, IBindingNode
{
    new IFieldDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    IdentifierName Name { get; }
    ITypeNode TypeNode { get; }
    DataType Type { get; }
    new IFieldSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDefinitionNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDefinitionNode.SymbolNode => SymbolNode;
    IDeclarationSymbolNode IDefinitionNode.SymbolNode => SymbolNode;
    FieldSymbol Symbol { get; }
    IUntypedExpressionNode? Initializer { get; }
}

public partial interface IAssociatedFunctionDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode
{
    new IAssociatedFunctionDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDefinitionNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName Name { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    new IAssociatedFunctionSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDefinitionNode.SymbolNode => SymbolNode;
    ITypeMemberSymbolNode ITypeMemberDefinitionNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDefinitionNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDefinitionNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDefinitionNode.SymbolNode => SymbolNode;
    FunctionSymbol Symbol { get; }
    FunctionType Type { get; }
}

public partial interface IAttributeNode : ISemanticNode, ICodeNode
{
    new IAttributeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IStandardTypeNameNode TypeName { get; }
    ConstructorSymbol? ReferencedSymbol { get; }
}

[Closed(
    typeof(ICapabilitySetNode),
    typeof(ICapabilityNode))]
public partial interface ICapabilityConstraintNode : ISemanticNode, ICodeNode
{
    new ICapabilityConstraintSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    Compiler.Types.Capabilities.ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    new CapabilitySet Constraint { get; }
    Compiler.Types.Capabilities.ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    Capability Capability { get; }
    new Capability Constraint { get; }
    Compiler.Types.Capabilities.ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

[Closed(
    typeof(IConstructorOrInitializerParameterNode),
    typeof(ISelfParameterNode))]
public partial interface IParameterNode : ISemanticNode, ICodeNode
{
    new IParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    bool Unused { get; }
    Pseudotype Type { get; }
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IFieldParameterNode))]
public partial interface IConstructorOrInitializerParameterNode : IParameterNode
{
    new IConstructorOrInitializerParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    new DataType Type { get; }
    Pseudotype IParameterNode.Type => Type;
    Parameter ParameterType { get; }
}

public partial interface INamedParameterNode : ISemanticNode, IConstructorOrInitializerParameterNode
{
    new INamedParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    bool IsLentBinding { get; }
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    int? DeclarationNumber { get; }
    ITypeNode TypeNode { get; }
}

[Closed(
    typeof(IConstructorSelfParameterNode),
    typeof(IInitializerSelfParameterNode),
    typeof(IMethodSelfParameterNode))]
public partial interface ISelfParameterNode : IParameterNode
{
    new ISelfParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    bool IsLentBinding { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
}

public partial interface IConstructorSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new ReferenceType Type { get; }
    Pseudotype IParameterNode.Type => Type;
    new ObjectType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

public partial interface IInitializerSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IInitializerSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new ValueType Type { get; }
    Pseudotype IParameterNode.Type => Type;
    new StructType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

public partial interface IMethodSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IMethodSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICapabilityConstraintNode Capability { get; }
    SelfParameter ParameterType { get; }
}

public partial interface IFieldParameterNode : ISemanticNode, IConstructorOrInitializerParameterNode
{
    new IFieldParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IFieldSymbolNode? ReferencedSymbolNode { get; }
}

[Closed(
    typeof(IBlockBodyNode),
    typeof(IExpressionBodyNode))]
public partial interface IBodyNode : IBodyOrBlockNode
{
}

public partial interface IBlockBodyNode : IBodyNode
{
    new IBlockBodySyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IFixedList<IBodyStatementNode> Statements { get; }
}

public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IResultStatementNode ResultStatement { get; }
}

[Closed(
    typeof(ITypeNameNode),
    typeof(IOptionalTypeNode),
    typeof(ICapabilityTypeNode),
    typeof(IFunctionTypeNode),
    typeof(IViewpointTypeNode))]
public partial interface ITypeNode : ISemanticNode, ICodeNode
{
    new ITypeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    DataType Type { get; }
}

[Closed(
    typeof(IStandardTypeNameNode),
    typeof(ISimpleTypeNameNode),
    typeof(IQualifiedTypeNameNode))]
public partial interface ITypeNameNode : ITypeNode
{
    new ITypeNameSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    TypeName Name { get; }
    LexicalScope ContainingLexicalScope { get; }
    TypeSymbol? ReferencedSymbol { get; }
    BareType? BareType { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(IGenericTypeNameNode))]
public partial interface IStandardTypeNameNode : ISemanticNode, ITypeNameNode
{
    new IStandardTypeNameSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    bool IsAttributeType { get; }
    new StandardName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    ITypeSymbolNode? ReferencedSymbolNode { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(ISpecialTypeNameNode))]
public partial interface ISimpleTypeNameNode : ISemanticNode, ITypeNameNode
{
    new ISimpleTypeNameSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
}

public partial interface IIdentifierTypeNameNode : IStandardTypeNameNode, ISimpleTypeNameNode
{
    new IIdentifierTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
    TypeName ITypeNameNode.Name => Name;
}

public partial interface ISpecialTypeNameNode : ISimpleTypeNameNode
{
    new ISpecialTypeNameSyntax Syntax { get; }
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    new SpecialTypeName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    new TypeSymbol ReferencedSymbol { get; }
    TypeSymbol? ITypeNameNode.ReferencedSymbol => ReferencedSymbol;
}

public partial interface IGenericTypeNameNode : IStandardTypeNameNode
{
    new IGenericTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IQualifiedTypeNameNode : ISemanticNode, ITypeNameNode
{
    new IQualifiedTypeNameSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ITypeNameNode Context { get; }
    IStandardTypeNameNode QualifiedName { get; }
}

public partial interface IOptionalTypeNode : ITypeNode
{
    new IOptionalTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ITypeNode Referent { get; }
}

public partial interface ICapabilityTypeNode : ITypeNode
{
    new ICapabilityTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    ITypeNode Referent { get; }
}

public partial interface IFunctionTypeNode : ITypeNode
{
    new IFunctionTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    IFixedList<IParameterTypeNode> Parameters { get; }
    ITypeNode Return { get; }
}

public partial interface IParameterTypeNode : ISemanticNode, ICodeNode
{
    new IParameterTypeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    bool IsLent { get; }
    ITypeNode Referent { get; }
    Parameter Parameter { get; }
}

[Closed(
    typeof(ICapabilityViewpointTypeNode),
    typeof(ISelfViewpointTypeNode))]
public partial interface IViewpointTypeNode : ITypeNode
{
    new IViewpointTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ITypeNode Referent { get; }
}

public partial interface ICapabilityViewpointTypeNode : ISemanticNode, IViewpointTypeNode
{
    new ICapabilityViewpointTypeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
}

public partial interface ISelfViewpointTypeNode : ISemanticNode, IViewpointTypeNode
{
    new ISelfViewpointTypeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    Pseudotype? SelfType { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBodyStatementNode))]
public partial interface IStatementNode : ISemanticNode, ICodeNode
{
    new IStatementSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
}

public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IUntypedExpressionNode Expression { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
}

public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, ILocalBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    IdentifierName Name { get; }
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IUntypedExpressionNode? Initializer { get; }
}

public partial interface IExpressionStatementNode : IBodyStatementNode
{
    new IExpressionStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IUntypedExpressionNode Expression { get; }
}

[Closed(
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode))]
public partial interface IPatternNode : ISemanticNode, ICodeNode
{
    new IPatternSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
}

public partial interface IBindingContextPatternNode : IPatternNode
{
    new IBindingContextPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    IPatternNode Pattern { get; }
    ITypeNode? Type { get; }
}

[Closed(
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode))]
public partial interface IOptionalOrBindingPatternNode : IPatternNode
{
    new IOptionalOrBindingPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
}

public partial interface IBindingPatternNode : ISemanticNode, IOptionalOrBindingPatternNode, ILocalBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    IdentifierName Name { get; }
}

public partial interface IOptionalPatternNode : ISemanticNode, IOptionalOrBindingPatternNode
{
    new IOptionalPatternSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    IOptionalOrBindingPatternNode Pattern { get; }
}

[Closed(
    typeof(IExpressionNode),
    typeof(INameExpressionNode))]
public partial interface IUntypedExpressionNode : ISemanticNode, ICodeNode
{
    new IExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
}

[Closed(
    typeof(IAssignableExpressionNode),
    typeof(IBlockExpressionNode),
    typeof(INewObjectExpressionNode),
    typeof(IUnsafeExpressionNode),
    typeof(INeverTypedExpressionNode),
    typeof(ILiteralExpressionNode),
    typeof(IAssignmentExpressionNode),
    typeof(IBinaryOperatorExpressionNode),
    typeof(IUnaryOperatorExpressionNode),
    typeof(IIdExpressionNode),
    typeof(IConversionExpressionNode),
    typeof(IPatternMatchExpressionNode),
    typeof(IIfExpressionNode),
    typeof(ILoopExpressionNode),
    typeof(IWhileExpressionNode),
    typeof(IForeachExpressionNode),
    typeof(IInvocationExpressionNode),
    typeof(ISelfExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface IExpressionNode : IUntypedExpressionNode
{
    new ITypedExpressionSyntax Syntax { get; }
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IMemberAccessExpressionNode),
    typeof(IMissingNameExpressionNode))]
public partial interface IAssignableExpressionNode : ISemanticNode, IExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IFixedList<IStatementNode> Statements { get; }
}

public partial interface INewObjectExpressionNode : ISemanticNode, IExpressionNode
{
    new INewObjectExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    ITypeNameNode Type { get; }
    IdentifierName? ConstructorName { get; }
    IFixedList<IUntypedExpressionNode> Arguments { get; }
    ConstructorSymbol? ReferencedSymbol { get; }
}

public partial interface IUnsafeExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Expression { get; }
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
public partial interface INeverTypedExpressionNode : ISemanticNode, IExpressionNode
{
    new INeverTypedExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    NeverType Type { get; }
}

[Closed(
    typeof(IBoolLiteralExpressionNode),
    typeof(IIntegerLiteralExpressionNode),
    typeof(INoneLiteralExpressionNode),
    typeof(IStringLiteralExpressionNode))]
public partial interface ILiteralExpressionNode : ISemanticNode, IExpressionNode
{
    new ILiteralExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

public partial interface IBoolLiteralExpressionNode : ILiteralExpressionNode
{
    new IBoolLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    bool Value { get; }
    BoolConstValueType Type { get; }
}

public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    new IIntegerLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    BigInteger Value { get; }
    IntegerConstValueType Type { get; }
}

public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    new INoneLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    OptionalType Type { get; }
}

public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    new IStringLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    string Value { get; }
    DataType Type { get; }
    LexicalScope ContainingLexicalScope { get; }
}

public partial interface IAssignmentExpressionNode : ISemanticNode, IExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IAssignableExpressionNode LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IUntypedExpressionNode RightOperand { get; }
}

public partial interface IBinaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode LeftOperand { get; }
    BinaryOperator Operator { get; }
    IUntypedExpressionNode RightOperand { get; }
}

public partial interface IUnaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IUntypedExpressionNode Operand { get; }
}

public partial interface IIdExpressionNode : ISemanticNode, IExpressionNode
{
    new IIdExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Referent { get; }
}

public partial interface IConversionExpressionNode : ISemanticNode, IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Referent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }
}

public partial interface IPatternMatchExpressionNode : ISemanticNode, IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Referent { get; }
    IPatternNode Pattern { get; }
}

public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Condition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
}

public partial interface ILoopExpressionNode : ISemanticNode, IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IWhileExpressionNode : ISemanticNode, IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Condition { get; }
    IBlockExpressionNode Block { get; }
}

public partial interface IForeachExpressionNode : ISemanticNode, IExpressionNode, ILocalBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IdentifierName VariableName { get; }
    IUntypedExpressionNode InExpression { get; }
    ITypeNode? Type { get; }
    IBlockExpressionNode Block { get; }
}

public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    INeverTypedExpressionSyntax INeverTypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode? Value { get; }
}

public partial interface INextExpressionNode : INeverTypedExpressionNode
{
    new INextExpressionSyntax Syntax { get; }
    INeverTypedExpressionSyntax INeverTypedExpressionNode.Syntax => Syntax;
}

public partial interface IReturnExpressionNode : INeverTypedExpressionNode
{
    new IReturnExpressionSyntax Syntax { get; }
    INeverTypedExpressionSyntax INeverTypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode? Value { get; }
}

public partial interface IInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Expression { get; }
    IFixedList<IUntypedExpressionNode> Arguments { get; }
}

[Closed(
    typeof(IInvocableNameExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IStandardNameExpressionNode),
    typeof(ISimpleNameExpressionNode))]
public partial interface INameExpressionNode : IUntypedExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode),
    typeof(IMemberAccessExpressionNode))]
public partial interface IInvocableNameExpressionNode : ISemanticNode, INameExpressionNode
{
    new IInvocableNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(ISelfExpressionNode),
    typeof(IMissingNameExpressionNode))]
public partial interface IVariableNameExpressionNode : ISemanticNode, INameExpressionNode
{
    new IVariableNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode))]
public partial interface IStandardNameExpressionNode : ISemanticNode, INameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    StandardName? Name { get; }
    Symbol? ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(ISpecialTypeNameExpressionNode))]
public partial interface ISimpleNameExpressionNode : ISemanticNode, INameExpressionNode
{
    new ISimpleNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
}

public partial interface IIdentifierNameExpressionNode : IInvocableNameExpressionNode, ISimpleNameExpressionNode, IStandardNameExpressionNode, IVariableNameExpressionNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IInvocableNameExpressionSyntax IInvocableNameExpressionNode.Syntax => Syntax;
    ISimpleNameExpressionSyntax ISimpleNameExpressionNode.Syntax => Syntax;
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    IVariableNameExpressionSyntax IVariableNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    new IdentifierName? Name { get; }
    StandardName? IStandardNameExpressionNode.Name => Name;
}

public partial interface ISpecialTypeNameExpressionNode : ISimpleNameExpressionNode
{
    new ISpecialTypeNameExpressionSyntax Syntax { get; }
    ISimpleNameExpressionSyntax ISimpleNameExpressionNode.Syntax => Syntax;
    SpecialTypeName Name { get; }
    TypeSymbol? ReferencedSymbol { get; }
}

public partial interface IGenericNameExpressionNode : IInvocableNameExpressionNode, IStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IInvocableNameExpressionSyntax IInvocableNameExpressionNode.Syntax => Syntax;
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName? IStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IMemberAccessExpressionNode : IInvocableNameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IInvocableNameExpressionSyntax IInvocableNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Context { get; }
    AccessOperator AccessOperator { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface ISelfExpressionNode : IVariableNameExpressionNode, IExpressionNode
{
    new ISelfExpressionSyntax Syntax { get; }
    IVariableNameExpressionSyntax IVariableNameExpressionNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
}

public partial interface IMissingNameExpressionNode : IVariableNameExpressionNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IVariableNameExpressionSyntax IVariableNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
}

public partial interface IMoveExpressionNode : ISemanticNode, IExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IVariableNameExpressionNode Referent { get; }
}

public partial interface IFreezeExpressionNode : ISemanticNode, IExpressionNode
{
    new IFreezeExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IVariableNameExpressionNode Referent { get; }
}

public partial interface IAsyncBlockExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IAsyncStartExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    bool Scheduled { get; }
    IUntypedExpressionNode Expression { get; }
}

public partial interface IAwaitExpressionNode : ISemanticNode, IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypedExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;
    IUntypedExpressionNode Expression { get; }
}

