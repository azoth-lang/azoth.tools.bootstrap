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
    typeof(IPackageMemberDeclarationNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDeclarationNode),
    typeof(IConcreteInvocableDeclarationNode),
    typeof(INamespaceDeclarationNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IGenericParameterNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IAlwaysTypeMemberDeclarationNode),
    typeof(IAbstractMethodDeclarationNode),
    typeof(IStandardMethodDeclarationNode),
    typeof(IGetterMethodDeclarationNode),
    typeof(ISetterMethodDeclarationNode),
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
    typeof(INeverTypedExpressionNode),
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
    typeof(IFieldDeclarationNode))]
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
    IFixedSet<IPackageMemberDeclarationNode> Declarations { get; }
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface IPackageMemberDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    IFixedList<IAttributeNode> Attributes { get; }
    AccessModifier AccessModifier { get; }
    new IPackageMemberSymbolNode SymbolNode { get; }
    INamespaceMemberSymbolNode INamespaceMemberDeclarationNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(IBodyOrBlockNode),
    typeof(IElseClauseNode),
    typeof(IBindingNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDeclarationNode),
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
    IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }
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
    typeof(IInvocableDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
public partial interface IDeclarationNode : ISemanticNode, ICodeNode
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
    typeof(IConcreteInvocableDeclarationNode),
    typeof(IMethodDeclarationNode))]
public partial interface IInvocableDeclarationNode : IDeclarationNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
}

[Closed(
    typeof(IConcreteMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IConcreteInvocableDeclarationNode : ISemanticNode, IInvocableDeclarationNode
{
}

public partial interface INamespaceDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    new INamespaceDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDeclarationNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDeclarationNode.ContainingSymbol => ContainingSymbol;
    new INamespaceSymbolNode SymbolNode { get; }
    INamespaceMemberSymbolNode INamespaceMemberDeclarationNode.SymbolNode => SymbolNode;
    NamespaceSymbol Symbol { get; }
}

[Closed(
    typeof(IPackageMemberDeclarationNode),
    typeof(INamespaceDeclarationNode))]
public partial interface INamespaceMemberDeclarationNode : IDeclarationNode
{
    new INamespaceMemberSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDeclarationNode.SymbolNode => SymbolNode;
}

public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode
{
    new IFunctionDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDeclarationNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDeclarationNode.ContainingSymbol => ContainingSymbol;
    IdentifierName Name { get; }
    new IFunctionSymbolNode SymbolNode { get; }
    IPackageMemberSymbolNode IPackageMemberDeclarationNode.SymbolNode => SymbolNode;
    FunctionSymbol Symbol { get; }
    IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    FunctionType Type { get; }
}

[Closed(
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode))]
public partial interface ITypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode
{
    new ITypeDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    bool IsConst { get; }
    StandardName Name { get; }
    IDeclaredUserType DeclaredType { get; }
    new IUserTypeSymbolNode SymbolNode { get; }
    IPackageMemberSymbolNode IPackageMemberDeclarationNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDeclarationNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDeclarationNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDeclarationNode.SymbolNode => SymbolNode;
    INamespaceMemberSymbolNode INamespaceMemberDeclarationNode.SymbolNode => SymbolNode;
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
    UserTypeSymbol Symbol { get; }
    IFixedList<IGenericParameterNode> GenericParameters { get; }
    LexicalScope SupertypesLexicalScope { get; }
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    CompilerResult<IFixedSet<BareReferenceType>> Supertypes { get; }
    IFixedList<ITypeMemberDeclarationNode> Members { get; }
}

public partial interface IClassDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new IClassDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    bool IsAbstract { get; }
    IStandardTypeNameNode? BaseTypeName { get; }
    new IClassSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDeclarationNode.SymbolNode => SymbolNode;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDeclarationNode.DeclaredType => DeclaredType;
    new IFixedList<IClassMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    ConstructorSymbol? DefaultConstructorSymbol { get; }
}

public partial interface IStructDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new IStructDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    new IStructSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDeclarationNode.SymbolNode => SymbolNode;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDeclarationNode.DeclaredType => DeclaredType;
    new IFixedList<IStructMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
}

public partial interface ITraitDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new ITraitDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    new ITraitSymbolNode SymbolNode { get; }
    IUserTypeSymbolNode ITypeDeclarationNode.SymbolNode => SymbolNode;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDeclarationNode.DeclaredType => DeclaredType;
    new IFixedList<ITraitMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
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
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IAlwaysTypeMemberDeclarationNode))]
public partial interface ITypeMemberDeclarationNode : IDeclarationNode
{
    new ITypeMemberDeclarationSyntax Syntax { get; }
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    AccessModifier AccessModifier { get; }
    new ITypeMemberSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDeclarationNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new IClassMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    new IClassMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new ITraitMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    new ITraitMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(ITypeDeclarationNode),
    typeof(IConcreteMethodDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new IStructMemberDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    new IStructMemberSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
}

[Closed(
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IAlwaysTypeMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new UserTypeSymbol ContainingSymbol { get; }
    Symbol IDeclarationNode.ContainingSymbol => ContainingSymbol;
}

[Closed(
    typeof(IAbstractMethodDeclarationNode),
    typeof(IConcreteMethodDeclarationNode))]
public partial interface IMethodDeclarationNode : IAlwaysTypeMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IInvocableDeclarationNode
{
    new IMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    MethodKind Kind { get; }
    IdentifierName Name { get; }
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDeclarationNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IMethodSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDeclarationNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDeclarationNode.SymbolNode => SymbolNode;
    IDeclarationSymbolNode IDeclarationNode.SymbolNode => SymbolNode;
    MethodSymbol Symbol { get; }
}

public partial interface IAbstractMethodDeclarationNode : ISemanticNode, IMethodDeclarationNode
{
    new IAbstractMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDeclarationNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    ObjectType ContainingDeclaredType { get; }
}

[Closed(
    typeof(IStandardMethodDeclarationNode),
    typeof(IGetterMethodDeclarationNode),
    typeof(ISetterMethodDeclarationNode))]
public partial interface IConcreteMethodDeclarationNode : IMethodDeclarationNode, IStructMemberDeclarationNode, IConcreteInvocableDeclarationNode
{
    new IConcreteMethodDeclarationSyntax Syntax { get; }
    IMethodDeclarationSyntax IMethodDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<INamedParameterNode> IMethodDeclarationNode.Parameters => Parameters;
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDeclarationNode.Parameters => Parameters;
}

public partial interface IStandardMethodDeclarationNode : ISemanticNode, IConcreteMethodDeclarationNode
{
    new IStandardMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDeclarationNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

public partial interface IGetterMethodDeclarationNode : ISemanticNode, IConcreteMethodDeclarationNode
{
    new IGetterMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDeclarationNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    new ITypeNode Return { get; }
}

public partial interface ISetterMethodDeclarationNode : ISemanticNode, IConcreteMethodDeclarationNode
{
    new ISetterMethodDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteMethodDeclarationSyntax IConcreteMethodDeclarationNode.Syntax => Syntax;
    IMethodDeclarationSyntax IMethodDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

public partial interface IConstructorDeclarationNode : IConcreteInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode, IClassMemberDeclarationNode
{
    new IConstructorDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    IConstructorSelfParameterNode SelfParameter { get; }
    ConstructorSymbol Symbol { get; }
}

public partial interface IInitializerDeclarationNode : IConcreteInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode, IStructMemberDeclarationNode
{
    new IInitializerDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    IInitializerSelfParameterNode SelfParameter { get; }
    InitializerSymbol Symbol { get; }
}

public partial interface IFieldDeclarationNode : IAlwaysTypeMemberDeclarationNode, IClassMemberDeclarationNode, IStructMemberDeclarationNode, IBindingNode
{
    new IFieldDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IdentifierName Name { get; }
    ITypeNode TypeNode { get; }
    DataType Type { get; }
    new IFieldSymbolNode SymbolNode { get; }
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDeclarationNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDeclarationNode.SymbolNode => SymbolNode;
    IDeclarationSymbolNode IDeclarationNode.SymbolNode => SymbolNode;
    FieldSymbol Symbol { get; }
    IUntypedExpressionNode? Initializer { get; }
}

public partial interface IAssociatedFunctionDeclarationNode : IConcreteInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode
{
    new IAssociatedFunctionDeclarationSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    IdentifierName Name { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    new IAssociatedFunctionSymbolNode SymbolNode { get; }
    IDeclarationSymbolNode IDeclarationNode.SymbolNode => SymbolNode;
    ITypeMemberSymbolNode ITypeMemberDeclarationNode.SymbolNode => SymbolNode;
    IClassMemberSymbolNode IClassMemberDeclarationNode.SymbolNode => SymbolNode;
    ITraitMemberSymbolNode ITraitMemberDeclarationNode.SymbolNode => SymbolNode;
    IStructMemberSymbolNode IStructMemberDeclarationNode.SymbolNode => SymbolNode;
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
    ITypeDeclarationNode ContainingTypeDeclaration { get; }
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
}

public partial interface IExpressionBodyNode : IBodyNode
{
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
}

public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    IUntypedExpressionNode UntypedExpression { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
public partial interface IBodyStatementNode : IStatementNode
{
}

public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, ILocalBindingNode
{
    IdentifierName Name { get; }
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IUntypedExpressionNode? Initializer { get; }
}

public partial interface IExpressionStatementNode : IBodyStatementNode
{
    IUntypedExpressionNode UntypedExpression { get; }
}

[Closed(
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode))]
public partial interface IPatternNode : ISemanticNode, ICodeNode
{
}

public partial interface IBindingContextPatternNode : IPatternNode
{
    bool IsMutableBinding { get; }
    IPatternNode Pattern { get; }
    ITypeNode? Type { get; }
}

[Closed(
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode))]
public partial interface IOptionalOrBindingPatternNode : IPatternNode
{
}

public partial interface IBindingPatternNode : ISemanticNode, IOptionalOrBindingPatternNode, ILocalBindingNode
{
    IdentifierName Name { get; }
}

public partial interface IOptionalPatternNode : ISemanticNode, IOptionalOrBindingPatternNode
{
    IOptionalOrBindingPatternNode Pattern { get; }
}

[Closed(
    typeof(IExpressionNode),
    typeof(INameExpressionNode))]
public partial interface IUntypedExpressionNode : ISemanticNode, ICodeNode
{
}

[Closed(
    typeof(IAssignableExpressionNode),
    typeof(IBlockExpressionNode),
    typeof(INewObjectExpressionNode),
    typeof(IUnsafeExpressionNode),
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
    typeof(INeverTypedExpressionNode),
    typeof(IInvocationExpressionNode),
    typeof(ISelfExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface IExpressionNode : IUntypedExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IMemberAccessExpressionNode))]
public partial interface IAssignableExpressionNode : ISemanticNode, IExpressionNode
{
}

public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    IFixedList<IStatementNode> Statements { get; }
}

public partial interface INewObjectExpressionNode : ISemanticNode, IExpressionNode
{
    ITypeNameNode Type { get; }
    IdentifierName? ConstructorName { get; }
    IFixedList<IUntypedExpressionNode> Arguments { get; }
    ConstructorSymbol? ReferencedSymbol { get; }
}

public partial interface IUnsafeExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode UntypedExpression { get; }
}

[Closed(
    typeof(IBoolLiteralExpressionNode),
    typeof(IIntegerLiteralExpressionNode),
    typeof(INoneLiteralExpressionNode),
    typeof(IStringLiteralExpressionNode))]
public partial interface ILiteralExpressionNode : ISemanticNode, IExpressionNode
{
}

public partial interface IBoolLiteralExpressionNode : ILiteralExpressionNode
{
    bool Value { get; }
    BoolConstValueType Type { get; }
}

public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    BigInteger Value { get; }
    IntegerConstValueType Type { get; }
}

public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    OptionalType Type { get; }
}

public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    string Value { get; }
    DataType Type { get; }
}

public partial interface IAssignmentExpressionNode : ISemanticNode, IExpressionNode
{
    IAssignableExpressionNode LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IUntypedExpressionNode RightOperand { get; }
}

public partial interface IBinaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode LeftOperand { get; }
    BinaryOperator Operator { get; }
    IUntypedExpressionNode RightOperand { get; }
}

public partial interface IUnaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IUntypedExpressionNode Operand { get; }
}

public partial interface IIdExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode Referent { get; }
}

public partial interface IConversionExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode Referent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }
}

public partial interface IPatternMatchExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode Referent { get; }
    IPatternNode Pattern { get; }
}

public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    IUntypedExpressionNode Condition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
}

public partial interface ILoopExpressionNode : ISemanticNode, IExpressionNode
{
    IBlockExpressionNode Block { get; }
}

public partial interface IWhileExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode Condition { get; }
    IBlockExpressionNode Block { get; }
}

public partial interface IForeachExpressionNode : ISemanticNode, IExpressionNode, ILocalBindingNode
{
    IdentifierName VariableName { get; }
    IUntypedExpressionNode InExpression { get; }
    ITypeNode? Type { get; }
    IBlockExpressionNode Block { get; }
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
public partial interface INeverTypedExpressionNode : ISemanticNode, IExpressionNode
{
    NeverType Type { get; }
}

public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    IUntypedExpressionNode? Value { get; }
}

public partial interface INextExpressionNode : INeverTypedExpressionNode
{
}

public partial interface IReturnExpressionNode : INeverTypedExpressionNode
{
    IUntypedExpressionNode? Value { get; }
}

public partial interface IInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode UntypedExpression { get; }
    IFixedList<IUntypedExpressionNode> Arguments { get; }
}

[Closed(
    typeof(IInvocableNameExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IStandardNameExpressionNode),
    typeof(ISimpleNameExpressionNode))]
public partial interface INameExpressionNode : IUntypedExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode),
    typeof(IMemberAccessExpressionNode))]
public partial interface IInvocableNameExpressionNode : ISemanticNode, INameExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(ISelfExpressionNode))]
public partial interface IVariableNameExpressionNode : ISemanticNode, INameExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode))]
public partial interface IStandardNameExpressionNode : ISemanticNode, INameExpressionNode
{
    StandardName? Name { get; }
    Symbol? ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(ISpecialTypeNameExpressionNode))]
public partial interface ISimpleNameExpressionNode : ISemanticNode, INameExpressionNode
{
}

public partial interface IIdentifierNameExpressionNode : IInvocableNameExpressionNode, ISimpleNameExpressionNode, IStandardNameExpressionNode, IVariableNameExpressionNode, IAssignableExpressionNode
{
    new IdentifierName? Name { get; }
    StandardName? IStandardNameExpressionNode.Name => Name;
}

public partial interface ISpecialTypeNameExpressionNode : ISimpleNameExpressionNode
{
    SpecialTypeName Name { get; }
    TypeSymbol? ReferencedSymbol { get; }
}

public partial interface IGenericNameExpressionNode : IInvocableNameExpressionNode, IStandardNameExpressionNode
{
    new GenericName Name { get; }
    StandardName? IStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface ISelfExpressionNode : IVariableNameExpressionNode, IExpressionNode
{
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
}

public partial interface IMemberAccessExpressionNode : IInvocableNameExpressionNode, IAssignableExpressionNode
{
    IUntypedExpressionNode Context { get; }
    AccessOperator AccessOperator { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IMoveExpressionNode : ISemanticNode, IExpressionNode
{
    IVariableNameExpressionNode Referent { get; }
}

public partial interface IFreezeExpressionNode : ISemanticNode, IExpressionNode
{
    IVariableNameExpressionNode Referent { get; }
}

public partial interface IAsyncBlockExpressionNode : ISemanticNode, IExpressionNode
{
    IBlockExpressionNode Block { get; }
}

public partial interface IAsyncStartExpressionNode : ISemanticNode, IExpressionNode
{
    bool Scheduled { get; }
    IUntypedExpressionNode UntypedExpression { get; }
}

public partial interface IAwaitExpressionNode : ISemanticNode, IExpressionNode
{
    IUntypedExpressionNode UntypedExpression { get; }
}

