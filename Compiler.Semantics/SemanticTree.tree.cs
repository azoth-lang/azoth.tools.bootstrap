using System.Collections.Generic;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
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
    typeof(INamedBindingNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IInvocableDefinitionNode),
    typeof(IExecutableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(INamespaceMemberDefinitionNode),
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode),
    typeof(ITypeMemberDefinitionNode),
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IAttributeNode),
    typeof(ICapabilityConstraintNode),
    typeof(IParameterNode),
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
    typeof(IOptionalPatternNode),
    typeof(IAmbiguousExpressionNode),
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
    typeof(IFunctionInvocationExpressionNode),
    typeof(IAmbiguousNameNode),
    typeof(IGenericNameExpressionNode),
    typeof(INameExpressionNode),
    typeof(IUnqualifiedNamespaceNameNode),
    typeof(IQualifiedNamespaceNameNode),
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode),
    typeof(ISelfExpressionNode),
    typeof(IUnknownStandardNameExpressionNode),
    typeof(IUnknownMemberAccessExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode),
    typeof(IDeclarationNode),
    typeof(INamedDeclarationNode),
    typeof(IBindingDeclarationNode),
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
    typeof(IInstanceMemberDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode),
    typeof(IFunctionSymbolNode),
    typeof(IUserTypeSymbolNode),
    typeof(IMethodSymbolNode),
    typeof(IConstructorSymbolNode),
    typeof(IFieldSymbolNode))]
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
    IFixedList<IStatementNode> Statements { get; }
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
    typeof(INamedBindingNode),
    typeof(IFieldDefinitionNode),
    typeof(IParameterNode))]
public partial interface IBindingNode : ICodeNode, IBindingDeclarationNode
{
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
public partial interface INamedBindingNode : ISemanticNode, IBindingNode, INamedBindingDeclarationNode
{
    new ILocalBindingSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ValueId ValueId { get; }
    IMaybeAntetype BindingAntetype { get; }
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
    PackageSymbol PackageSymbol { get; }
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
    typeof(ITypeNode),
    typeof(IParameterTypeNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IAmbiguousExpressionNode))]
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
    typeof(IExecutableDefinitionNode),
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
    InvocableSymbol Symbol { get; }
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IFieldDefinitionNode))]
public partial interface IExecutableDefinitionNode : ISemanticNode, IDefinitionNode
{
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IConcreteInvocableDefinitionNode : IInvocableDefinitionNode, IExecutableDefinitionNode
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

public partial interface IFunctionDefinitionNode : ISemanticNode, IPackageMemberDefinitionNode, IFunctionDeclarationNode, IConcreteInvocableDefinitionNode
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
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    IBodyNode Body { get; }
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
public partial interface ITypeDefinitionNode : ISemanticNode, IPackageMemberDefinitionNode, IAssociatedMemberDefinitionNode, IUserTypeDeclarationNode
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
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IFieldDefinitionNode))]
public partial interface IClassMemberDefinitionNode : ITypeMemberDefinitionNode, IClassMemberDeclarationNode
{
    new IClassMemberDefinitionSyntax? Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode))]
public partial interface ITraitMemberDefinitionNode : ITypeMemberDefinitionNode, ITraitMemberDeclarationNode
{
    new ITraitMemberDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode))]
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
    typeof(ITypeDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IAssociatedMemberDefinitionNode : IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, INamedDeclarationNode
{
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
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
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
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
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

public partial interface IInitializerDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
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
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
    IBlockBodyNode Body { get; }
}

public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, IBindingNode, IFieldDeclarationNode, IExecutableDefinitionNode
{
    new IFieldDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IClassMemberDefinitionSyntax? IClassMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    IMaybeAntetype Antetype { get; }
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FieldSymbol IFieldDeclarationNode.Symbol => Symbol;
    IAmbiguousExpressionNode? Initializer { get; }
}

public partial interface IAssociatedFunctionDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IAssociatedMemberDefinitionNode, IAssociatedFunctionDeclarationNode
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
    StandardName INamedDeclarationNode.Name => Name;
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    new FunctionSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
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
public partial interface IParameterNode : ISemanticNode, IBindingNode
{
    new IParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    bool Unused { get; }
    Pseudotype Type { get; }
    ValueId ValueId { get; }
    FlowState FlowStateAfter { get; }
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IFieldParameterNode))]
public partial interface IConstructorOrInitializerParameterNode : IParameterNode
{
    new IConstructorOrInitializerParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IMaybeAntetype BindingAntetype { get; }
    new DataType Type { get; }
    Pseudotype IParameterNode.Type => Type;
    Parameter ParameterType { get; }
}

public partial interface INamedParameterNode : IConstructorOrInitializerParameterNode, INamedBindingNode
{
    new INamedParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    ILocalBindingSyntax INamedBindingNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    bool IsLentBinding { get; }
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    int? DeclarationNumber { get; }
    ITypeNode TypeNode { get; }
    NamedVariableSymbol Symbol { get; }
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
    SelfParameterSymbol Symbol { get; }
}

public partial interface IConstructorSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType Type { get; }
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
    new CapabilityType Type { get; }
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
    FlowState FlowStateAfter { get; }
}

public partial interface IBlockBodyNode : IBodyNode
{
    new IBlockBodySyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IFixedList<IBodyStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
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
    IMaybeAntetype Antetype { get; }
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
    FlowState FlowStateAfter { get; }
}

public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
}

public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, INamedBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    ILocalBindingSyntax INamedBindingNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IAmbiguousExpressionNode? Initializer { get; }
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    NamedVariableSymbol Symbol { get; }
    int? DeclarationNumber { get; }
}

public partial interface IExpressionStatementNode : IBodyStatementNode
{
    new IExpressionStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
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

public partial interface IBindingPatternNode : IOptionalOrBindingPatternNode, INamedBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    ILocalBindingSyntax INamedBindingNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    LexicalScope ContainingLexicalScope { get; }
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
    typeof(IInvocationExpressionNode),
    typeof(IAmbiguousNameExpressionNode))]
public partial interface IAmbiguousExpressionNode : ISemanticNode, ICodeNode
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
    typeof(IFunctionInvocationExpressionNode),
    typeof(INameExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface IExpressionNode : IAmbiguousExpressionNode
{
    ValueId ValueId { get; }
    IMaybeExpressionAntetype Antetype { get; }
    DataType Type { get; }
    FlowState FlowStateAfter { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IMemberAccessExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownIdentifierNameExpressionNode))]
public partial interface IAssignableExpressionNode : ISemanticNode, IExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    new IFixedList<IStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
}

public partial interface INewObjectExpressionNode : ISemanticNode, IExpressionNode
{
    new INewObjectExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ITypeNameNode ConstructingType { get; }
    IdentifierName? ConstructorName { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    ConstructorSymbol? ReferencedSymbol { get; }
}

public partial interface IUnsafeExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode FinalExpression { get; }
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
public partial interface INeverTypedExpressionNode : ISemanticNode, IExpressionNode
{
    new INeverTypedExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    new NeverType Type { get; }
    DataType IExpressionNode.Type => Type;
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
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

public partial interface IBoolLiteralExpressionNode : ILiteralExpressionNode
{
    new IBoolLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    bool Value { get; }
    new BoolConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;
}

public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    new IIntegerLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    BigInteger Value { get; }
    new IntegerConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;
}

public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    new INoneLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    new OptionalType Type { get; }
    DataType IExpressionNode.Type => Type;
}

public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    new IStringLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    string Value { get; }
    LexicalScope ContainingLexicalScope { get; }
}

public partial interface IAssignmentExpressionNode : ISemanticNode, IExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAssignableExpressionNode LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
}

public partial interface IBinaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode LeftOperand { get; }
    BinaryOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
}

public partial interface IUnaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IAmbiguousExpressionNode Operand { get; }
}

public partial interface IIdExpressionNode : ISemanticNode, IExpressionNode
{
    new IIdExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode FinalReferent { get; }
}

public partial interface IConversionExpressionNode : ISemanticNode, IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }
}

public partial interface IPatternMatchExpressionNode : ISemanticNode, IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    IPatternNode Pattern { get; }
}

public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Condition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
}

public partial interface ILoopExpressionNode : ISemanticNode, IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IWhileExpressionNode : ISemanticNode, IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Condition { get; }
    IBlockExpressionNode Block { get; }
}

public partial interface IForeachExpressionNode : IExpressionNode, INamedBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    ILocalBindingSyntax INamedBindingNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    IdentifierName VariableName { get; }
    IAmbiguousExpressionNode InExpression { get; }
    ITypeNode? DeclaredType { get; }
    IBlockExpressionNode Block { get; }
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
}

public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    INeverTypedExpressionSyntax INeverTypedExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode? Value { get; }
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
    IAmbiguousExpressionNode? Value { get; }
}

public partial interface IInvocationExpressionNode : IAmbiguousExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IEnumerable<IAmbiguousExpressionNode> CurrentArguments { get; }
}

public partial interface IFunctionInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IFunctionGroupNameNode FunctionGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
}

[Closed(
    typeof(IAmbiguousNameNode),
    typeof(INameExpressionNode))]
public partial interface IAmbiguousNameExpressionNode : IAmbiguousExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(ISimpleNameNode),
    typeof(IStandardNameExpressionNode),
    typeof(IMemberAccessExpressionNode))]
public partial interface IAmbiguousNameNode : ISemanticNode, IAmbiguousNameExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IInstanceExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownIdentifierNameExpressionNode))]
public partial interface ISimpleNameNode : IAmbiguousNameNode
{
    new ISimpleNameSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode))]
public partial interface IStandardNameExpressionNode : IAmbiguousNameNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    StandardName Name { get; }
    LexicalScope ContainingLexicalScope { get; }
    IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IIdentifierNameExpressionNode : IStandardNameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;
}

public partial interface IGenericNameExpressionNode : ISemanticNode, IStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IMemberAccessExpressionNode : IAmbiguousNameNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
}

[Closed(
    typeof(INamespaceNameNode),
    typeof(IFunctionGroupNameNode),
    typeof(IMethodGroupNameNode),
    typeof(IVariableNameExpressionNode),
    typeof(ITypeNameExpressionNode),
    typeof(IInitializerGroupNameNode),
    typeof(ISpecialTypeNameExpressionNode),
    typeof(IInstanceExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownNameExpressionNode))]
public partial interface INameExpressionNode : ISemanticNode, IExpressionNode, IAmbiguousNameExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IUnqualifiedNamespaceNameNode),
    typeof(IQualifiedNamespaceNameNode))]
public partial interface INamespaceNameNode : INameExpressionNode
{
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
    IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IUnqualifiedNamespaceNameNode : ISemanticNode, INamespaceNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IdentifierName Name { get; }
}

public partial interface IQualifiedNamespaceNameNode : ISemanticNode, INamespaceNameNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
    IdentifierName Name { get; }
}

public partial interface IFunctionGroupNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IMethodGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    INameExpressionNode? Context { get; }
    StandardName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IMethodDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IVariableNameExpressionNode : INameExpressionNode, IAssignableExpressionNode, ISimpleNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IdentifierName Name { get; }
    INamedBindingNode ReferencedDeclaration { get; }
}

[Closed(
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode))]
public partial interface ITypeNameExpressionNode : INameExpressionNode
{
    StandardName Name { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    ITypeDeclarationNode ReferencedDeclaration { get; }
}

public partial interface IStandardTypeNameExpressionNode : ISemanticNode, ITypeNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
}

public partial interface IQualifiedTypeNameExpressionNode : ISemanticNode, ITypeNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
}

public partial interface IInitializerGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ITypeNameExpressionNode? Context { get; }
    StandardName InitializerName { get; }
    IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface ISpecialTypeNameExpressionNode : INameExpressionNode
{
    new ISpecialTypeNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    SpecialTypeName Name { get; }
    TypeSymbol ReferencedSymbol { get; }
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
}

[Closed(
    typeof(ISelfExpressionNode))]
public partial interface IInstanceExpressionNode : INameExpressionNode, ISimpleNameNode
{
}

public partial interface ISelfExpressionNode : ISemanticNode, IInstanceExpressionNode
{
    new ISelfExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
    IExecutableDefinitionNode ContainingDeclaration { get; }
    SelfParameterSymbol? ReferencedSymbol { get; }
}

public partial interface IMissingNameExpressionNode : INameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IMissingNameSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
}

[Closed(
    typeof(IUnknownStandardNameExpressionNode),
    typeof(IUnknownMemberAccessExpressionNode))]
public partial interface IUnknownNameExpressionNode : INameExpressionNode
{
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
}

[Closed(
    typeof(IUnknownIdentifierNameExpressionNode),
    typeof(IUnknownGenericNameExpressionNode))]
public partial interface IUnknownStandardNameExpressionNode : ISemanticNode, IUnknownNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    StandardName Name { get; }
    IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IUnknownIdentifierNameExpressionNode : IUnknownStandardNameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;
}

public partial interface IUnknownGenericNameExpressionNode : IUnknownStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IUnknownMemberAccessExpressionNode : ISemanticNode, IUnknownNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IDeclarationNode> ReferencedMembers { get; }
}

public partial interface IMoveExpressionNode : ISemanticNode, IExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISimpleNameNode Referent { get; }
}

public partial interface IFreezeExpressionNode : ISemanticNode, IExpressionNode
{
    new IFreezeExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISimpleNameNode Referent { get; }
}

public partial interface IAsyncBlockExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IAsyncStartExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    bool Scheduled { get; }
    IAmbiguousExpressionNode Expression { get; }
}

public partial interface IAwaitExpressionNode : ISemanticNode, IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
}

[Closed(
    typeof(IChildDeclarationNode),
    typeof(ISymbolDeclarationNode))]
public partial interface IDeclarationNode : ISemanticNode
{
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IBindingDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode))]
public partial interface IChildDeclarationNode : IDeclarationNode, IChildNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(INamedBindingDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(IFunctionLikeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode),
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
    typeof(IAssociatedMemberDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface ISymbolDeclarationNode : IDeclarationNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(IBindingNode),
    typeof(INamedBindingDeclarationNode))]
public partial interface IBindingDeclarationNode : ISemanticNode, IChildDeclarationNode
{
}

[Closed(
    typeof(INamedBindingNode))]
public partial interface INamedBindingDeclarationNode : IBindingDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
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
    typeof(IFunctionDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IFunctionLikeDeclarationNode : INamedDeclarationNode
{
    FunctionSymbol Symbol { get; }
    FunctionType Type { get; }
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IFunctionSymbolNode))]
public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode, IFunctionLikeDeclarationNode
{
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
    typeof(IStructMemberDeclarationNode),
    typeof(IInstanceMemberDeclarationNode))]
public partial interface ITypeMemberDeclarationNode : IPackageFacetChildDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IFieldDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(ITraitMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IStructMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IAssociatedMemberDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
public partial interface IInstanceMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IMethodSymbolNode))]
public partial interface IMethodDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode, IInstanceMemberDeclarationNode
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
public partial interface IInitializerDeclarationNode : ISemanticNode, IAssociatedMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new InitializerSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IFieldDefinitionNode),
    typeof(IFieldSymbolNode))]
public partial interface IFieldDeclarationNode : INamedDeclarationNode, IClassMemberDeclarationNode, IStructMemberDeclarationNode, IInstanceMemberDeclarationNode
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
public partial interface IAssociatedFunctionDeclarationNode : ISemanticNode, IAssociatedMemberDeclarationNode, IFunctionLikeDeclarationNode
{
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

public partial interface IInitializerSymbolNode : IInitializerDeclarationNode
{
}

public partial interface IFieldSymbolNode : ISemanticNode, IFieldDeclarationNode
{
}

public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode
{
}

