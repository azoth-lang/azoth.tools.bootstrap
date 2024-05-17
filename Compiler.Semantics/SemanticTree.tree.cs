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
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IInvocableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(INamespaceMemberDefinitionNode),
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode),
    typeof(ITypeMemberDefinitionNode),
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode),
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
    typeof(IVariableDeclarationStatementNode),
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
    typeof(IAwaitExpressionNode),
    typeof(IDeclarationNode),
    typeof(INamedDeclarationNode),
    typeof(ILocalBindingDeclarationNode),
    typeof(IPackageDeclarationNode),
    typeof(IPackageMemberDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode),
    typeof(INamespaceDeclarationNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IGenericParameterDeclarationNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IFunctionSymbolNode),
    typeof(IUserTypeSymbolNode),
    typeof(IMethodSymbolNode),
    typeof(IConstructorSymbolNode),
    typeof(IInitializerSymbolNode),
    typeof(IFieldSymbolNode),
    typeof(IAssociatedFunctionSymbolNode))]
public partial interface ISemanticNode
{
    ISyntax? Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(IPackageFacetNode),
    typeof(ICodeNode),
    typeof(IChildDeclarationNode))]
public partial interface IChildNode : IChild<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
    IPackageDeclarationNode Package { get; }
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
public partial interface IBindingNode : ISemanticNode, ICodeNode, IBindingDeclarationNode
{
    bool IsMutableBinding { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
public partial interface ILocalBindingNode : IBindingNode, ILocalBindingDeclarationNode
{
}

public partial interface IPackageNode : IPackageDeclarationNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedSet<IPackageReferenceNode> References { get; }
    IPackageReferenceNode IntrinsicsReference { get; }
    FixedDictionary<IdentifierName,IPackageDeclarationNode> PackageDeclarations { get; }
    new IPackageFacetNode MainFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.MainFacet => MainFacet;
    new IPackageFacetNode TestingFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.TestingFacet => TestingFacet;
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

public partial interface IPackageFacetNode : IChildNode, IPackageFacetDeclarationNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    PackageNameScope PackageNameScope { get; }
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    IFixedSet<IPackageMemberDefinitionNode> Definitions { get; }
    new INamespaceDefinitionNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode))]
public partial interface IPackageMemberDefinitionNode : INamespaceBlockMemberDefinitionNode, INamespaceMemberDefinitionNode
{
    IFixedList<IAttributeNode> Attributes { get; }
    AccessModifier AccessModifier { get; }
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
    new IConcreteSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    CodeFile File { get; }
}

public partial interface ICompilationUnitNode : ISemanticNode, ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IPackageFacetNode ContainingDeclaration { get; }
    PackageSymbol ContainingSymbol { get; }
    NamespaceName ImplicitNamespaceName { get; }
    INamespaceDefinitionNode ImplicitNamespace { get; }
    NamespaceSymbol ImplicitNamespaceSymbol { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    NamespaceScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
}

public partial interface IUsingDirectiveNode : ISemanticNode, ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    NamespaceName Name { get; }
}

[Closed(
    typeof(IInvocableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(ITypeMemberDefinitionNode))]
public partial interface IDefinitionNode : ICodeNode, IPackageFacetChildDeclarationNode
{
    new IDefinitionSyntax? Syntax { get; }
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IPackageFacetNode Facet { get; }
    IPackageFacetDeclarationNode IPackageFacetChildDeclarationNode.Facet => Facet;
    ISymbolDeclarationNode ContainingDeclaration { get; }
    Symbol ContainingSymbol { get; }
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IMethodDefinitionNode))]
public partial interface IInvocableDefinitionNode : ISemanticNode, IDefinitionNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
}

[Closed(
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IConcreteInvocableDefinitionNode : IInvocableDefinitionNode
{
}

public partial interface INamespaceBlockDefinitionNode : INamespaceBlockMemberDefinitionNode
{
    new INamespaceDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    INamespaceDefinitionNode Definition { get; }
    new INamespaceDefinitionNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    INamespaceDefinitionNode ContainingNamespace { get; }
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    NamespaceSymbol Symbol { get; }
    new NamespaceSearchScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
}

[Closed(
    typeof(IPackageMemberDefinitionNode),
    typeof(INamespaceBlockDefinitionNode))]
public partial interface INamespaceBlockMemberDefinitionNode : ISemanticNode, IDefinitionNode
{
}

public partial interface INamespaceDefinitionNode : INamespaceMemberDefinitionNode, INamespaceDeclarationNode
{
    IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; }
    IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; }
    new IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;
}

[Closed(
    typeof(IPackageMemberDefinitionNode),
    typeof(INamespaceDefinitionNode))]
public partial interface INamespaceMemberDefinitionNode : ISemanticNode, INamespaceMemberDeclarationNode
{
}

public partial interface IFunctionDefinitionNode : ISemanticNode, IPackageMemberDefinitionNode, IFunctionDeclarationNode
{
    new IFunctionDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new INamespaceDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    IBodyNode Body { get; }
    FunctionType Type { get; }
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
public partial interface ITypeDefinitionNode : ISemanticNode, IPackageMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, IUserTypeDeclarationNode
{
    new ITypeDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    bool IsConst { get; }
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    IDeclaredUserType DeclaredType { get; }
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    UserTypeSymbol IUserTypeDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    IFixedList<IGenericParameterNode> GenericParameters { get; }
    LexicalScope SupertypesLexicalScope { get; }
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    CompilerResult<IFixedSet<BareReferenceType>> Supertypes { get; }
    new IFixedList<ITypeMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface IClassDefinitionNode : ITypeDefinitionNode, IClassDeclarationNode
{
    new IClassDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    bool IsAbstract { get; }
    IStandardTypeNameNode? BaseTypeName { get; }
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    new IFixedList<IClassMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedList<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IDefaultConstructorDefinitionNode? DefaultConstructor { get; }
}

public partial interface IStructDefinitionNode : ITypeDefinitionNode, IStructDeclarationNode
{
    new IStructDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedList<IStructMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedList<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface ITraitDefinitionNode : ITypeDefinitionNode, ITraitDeclarationNode
{
    new ITraitDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedList<ITraitMemberDefinitionNode> Members { get; }
    IFixedList<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedList<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface IGenericParameterNode : ICodeNode, IGenericParameterDeclarationNode
{
    new IGenericParameterSyntax Syntax { get; }
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
    GenericParameter Parameter { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    GenericParameterType DeclaredType { get; }
    IUserTypeDeclarationNode ContainingDeclaration { get; }
    UserTypeSymbol ContainingSymbol { get; }
    new GenericParameterTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(ITraitMemberDefinitionNode),
    typeof(IStructMemberDefinitionNode),
    typeof(IAlwaysTypeMemberDefinitionNode))]
public partial interface ITypeMemberDefinitionNode : ISemanticNode, IDefinitionNode, ITypeMemberDeclarationNode
{
    new ITypeMemberDefinitionSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    AccessModifier AccessModifier { get; }
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IClassMemberDefinitionNode : ITypeMemberDefinitionNode, IClassMemberDeclarationNode
{
    new IClassMemberDefinitionSyntax? Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface ITraitMemberDefinitionNode : ITypeMemberDefinitionNode, ITraitMemberDeclarationNode
{
    new ITraitMemberDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IStructMemberDefinitionNode : ITypeMemberDefinitionNode, IStructMemberDeclarationNode
{
    new IStructMemberDefinitionSyntax? Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IAlwaysTypeMemberDefinitionNode : ITypeMemberDefinitionNode
{
    new UserTypeSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode))]
public partial interface IMethodDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IInvocableDefinitionNode, IMethodDeclarationNode
{
    new IMethodDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    MethodKind Kind { get; }
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IMethodDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new MethodSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    MethodSymbol IMethodDeclarationNode.Symbol => Symbol;
}

public partial interface IAbstractMethodDefinitionNode : ISemanticNode, IMethodDefinitionNode
{
    new IAbstractMethodDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ObjectType ContainingDeclaredType { get; }
}

[Closed(
    typeof(IStandardMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode))]
public partial interface IConcreteMethodDefinitionNode : ISemanticNode, IMethodDefinitionNode, IStructMemberDefinitionNode, IConcreteInvocableDefinitionNode
{
    new IConcreteMethodDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<INamedParameterNode> IMethodDefinitionNode.Parameters => Parameters;
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    IBodyNode Body { get; }
}

public partial interface IStandardMethodDefinitionNode : IConcreteMethodDefinitionNode
{
    new IStandardMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
}

public partial interface IGetterMethodDefinitionNode : IConcreteMethodDefinitionNode
{
    new IGetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    new ITypeNode Return { get; }
}

public partial interface ISetterMethodDefinitionNode : IConcreteMethodDefinitionNode
{
    new ISetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(IDefaultConstructorDefinitionNode),
    typeof(ISourceConstructorDefinitionNode))]
public partial interface IConstructorDefinitionNode : ISemanticNode, IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IConstructorDeclarationNode
{
    new IConstructorDefinitionSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName? IConstructorDeclarationNode.Name => Name;
    new ConstructorSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    ConstructorSymbol IConstructorDeclarationNode.Symbol => Symbol;
}

public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{
}

public partial interface ISourceConstructorDefinitionNode : IConstructorDefinitionNode
{
    new IConstructorDefinitionSyntax Syntax { get; }
    IConstructorDefinitionSyntax? IConstructorDefinitionNode.Syntax => Syntax;
    IConstructorSelfParameterNode SelfParameter { get; }
    IBlockBodyNode Body { get; }
}

public partial interface IInitializerDefinitionNode : ISemanticNode, IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
{
    new IInitializerDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName? IInitializerDeclarationNode.Name => Name;
    IInitializerSelfParameterNode SelfParameter { get; }
    new InitializerSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
    IBlockBodyNode Body { get; }
}

public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, IBindingNode, IFieldDeclarationNode
{
    new IFieldDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IBindingDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FieldSymbol IFieldDeclarationNode.Symbol => Symbol;
    IUntypedExpressionNode? Initializer { get; }
}

public partial interface IAssociatedFunctionDefinitionNode : ISemanticNode, IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, IAssociatedFunctionDeclarationNode
{
    new IAssociatedFunctionDefinitionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    ITraitMemberDefinitionSyntax ITraitMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IAssociatedFunctionDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IAssociatedFunctionDeclarationNode.Symbol => Symbol;
    IBodyNode Body { get; }
}

public partial interface IAttributeNode : ISemanticNode, ICodeNode
{
    new IAttributeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    Capability Capability { get; }
}

[Closed(
    typeof(IConstructorOrInitializerParameterNode),
    typeof(ISelfParameterNode))]
public partial interface IParameterNode : ISemanticNode, ICodeNode
{
    new IParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IFieldDefinitionNode? ReferencedField { get; }
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IFixedList<IBodyStatementNode> Statements { get; }
}

public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    ITypeDeclarationNode? ReferencedDeclaration { get; }
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
}

public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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

public partial interface IVariableDeclarationStatementNode : ISemanticNode, IBodyStatementNode, ILocalBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    UnknownType Type { get; }
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

[Closed(
    typeof(IChildDeclarationNode),
    typeof(ISymbolDeclarationNode))]
public partial interface IDeclarationNode : ISemanticNode
{
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode))]
public partial interface IChildDeclarationNode : IDeclarationNode, IChildNode
{
}

[Closed(
    typeof(IBindingDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(IFunctionDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface INamedDeclarationNode : ISemanticNode, IChildDeclarationNode
{
    StandardName Name { get; }
}

[Closed(
    typeof(IPackageDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface ISymbolDeclarationNode : IDeclarationNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(IBindingNode),
    typeof(ILocalBindingDeclarationNode))]
public partial interface IBindingDeclarationNode : INamedDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(ILocalBindingNode))]
public partial interface ILocalBindingDeclarationNode : ISemanticNode, IBindingDeclarationNode
{
}

[Closed(
    typeof(IPackageNode),
    typeof(IPackageSymbolNode))]
public partial interface IPackageDeclarationNode : ISemanticNode, ISymbolDeclarationNode
{
    IdentifierName? AliasOrName { get; }
    IdentifierName Name { get; }
    new PackageSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IPackageFacetDeclarationNode MainFacet { get; }
    IPackageFacetDeclarationNode TestingFacet { get; }
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IUserTypeDeclarationNode))]
public partial interface IPackageMemberDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
}

[Closed(
    typeof(IPackageFacetNode),
    typeof(IPackageFacetSymbolNode))]
public partial interface IPackageFacetDeclarationNode : ISemanticNode, IChildDeclarationNode, ISymbolDeclarationNode
{
    IdentifierName? PackageAliasOrName { get; }
    IdentifierName PackageName { get; }
    new PackageSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    INamespaceDeclarationNode GlobalNamespace { get; }
}

[Closed(
    typeof(IDefinitionNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
public partial interface IPackageFacetChildDeclarationNode : ISemanticNode, IChildDeclarationNode
{
    StandardName? Name { get; }
    IPackageFacetDeclarationNode Facet { get; }
}

[Closed(
    typeof(INamespaceDefinitionNode),
    typeof(INamespaceSymbolNode))]
public partial interface INamespaceDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
}

[Closed(
    typeof(INamespaceMemberDefinitionNode),
    typeof(IPackageMemberDeclarationNode),
    typeof(INamespaceDeclarationNode))]
public partial interface INamespaceMemberDeclarationNode : IPackageFacetChildDeclarationNode, INamedDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IFunctionSymbolNode))]
public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode, INamedDeclarationNode
{
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IUserTypeSymbolNode))]
public partial interface IUserTypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ITypeDeclarationNode
{
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    IFixedList<ITypeMemberDeclarationNode> Members { get; }
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IClassSymbolNode))]
public partial interface IClassDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedList<IClassMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

[Closed(
    typeof(IStructDefinitionNode),
    typeof(IStructSymbolNode))]
public partial interface IStructDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedList<IStructMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

[Closed(
    typeof(ITraitDefinitionNode),
    typeof(ITraitSymbolNode))]
public partial interface ITraitDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedList<ITraitMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

[Closed(
    typeof(IGenericParameterNode),
    typeof(IGenericParameterSymbolNode))]
public partial interface IGenericParameterDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(ITypeMemberDefinitionNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode))]
public partial interface ITypeMemberDeclarationNode : IPackageFacetChildDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(ITraitMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IStructMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IMethodSymbolNode))]
public partial interface IMethodDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    new MethodSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IConstructorDefinitionNode),
    typeof(IConstructorSymbolNode))]
public partial interface IConstructorDeclarationNode : IClassMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new ConstructorSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IInitializerDefinitionNode),
    typeof(IInitializerSymbolNode))]
public partial interface IInitializerDeclarationNode : IStructMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new InitializerSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IFieldDefinitionNode),
    typeof(IFieldSymbolNode))]
public partial interface IFieldDeclarationNode : INamedDeclarationNode, IClassMemberDeclarationNode, IStructMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    DataType Type { get; }
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAssociatedFunctionDefinitionNode),
    typeof(IAssociatedFunctionSymbolNode))]
public partial interface IAssociatedFunctionDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionType Type { get; }
}

[Closed(
    typeof(IUserTypeDeclarationNode),
    typeof(IGenericParameterDeclarationNode))]
public partial interface ITypeDeclarationNode : INamedDeclarationNode, ISymbolDeclarationNode
{
    new TypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

public partial interface IPackageSymbolNode : IPackageDeclarationNode
{
}

public partial interface IPackageFacetSymbolNode : IPackageFacetDeclarationNode
{
}

public partial interface INamespaceSymbolNode : INamespaceDeclarationNode
{
    new IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;
}

public partial interface IFunctionSymbolNode : ISemanticNode, IFunctionDeclarationNode
{
}

public partial interface IUserTypeSymbolNode : ISemanticNode, IUserTypeDeclarationNode
{
}

public partial interface IClassSymbolNode : IClassDeclarationNode
{
}

public partial interface IStructSymbolNode : IStructDeclarationNode
{
}

public partial interface ITraitSymbolNode : ITraitDeclarationNode
{
}

public partial interface IGenericParameterSymbolNode : IGenericParameterDeclarationNode
{
}

public partial interface IMethodSymbolNode : ISemanticNode, IMethodDeclarationNode
{
}

public partial interface IConstructorSymbolNode : ISemanticNode, IConstructorDeclarationNode
{
}

public partial interface IInitializerSymbolNode : ISemanticNode, IInitializerDeclarationNode
{
}

public partial interface IFieldSymbolNode : ISemanticNode, IFieldDeclarationNode
{
}

public partial interface IAssociatedFunctionSymbolNode : ISemanticNode, IAssociatedFunctionDeclarationNode
{
}

