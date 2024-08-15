using System.Collections.Generic;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
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
    typeof(IDeclarationNode))]
public partial interface ISemanticNode : ITreeNode
{
    ISyntax? Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(ICodeNode),
    typeof(IChildDeclarationNode))]
public partial interface IChildNode : IChildTreeNode<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
    IPackageDeclarationNode Package { get; }
}

[Closed(
    typeof(IBodyNode),
    typeof(IBlockExpressionNode))]
public partial interface IBodyOrBlockNode : ICodeNode
{
    IFixedList<IStatementNode> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultNode),
    typeof(IIfExpressionNode))]
public partial interface IElseClauseNode : IControlFlowNode
{
    new ICodeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IFlowState FlowStateAfter { get; }
    ValueId ValueId { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBlockExpressionNode))]
public partial interface IBlockOrResultNode : IElseClauseNode
{
    IMaybeAntetype Antetype { get; }
    DataType Type { get; }
}

[Closed(
    typeof(INamedBindingNode),
    typeof(ISelfParameterNode))]
public partial interface IBindingNode : ICodeNode, IBindingDeclarationNode
{
    bool IsLentBinding { get; }
    ValueId BindingValueId { get; }
    IMaybeAntetype BindingAntetype { get; }
    Pseudotype BindingType { get; }
}

[Closed(
    typeof(ILocalBindingNode),
    typeof(IFieldDefinitionNode))]
public partial interface INamedBindingNode : IBindingNode, INamedBindingDeclarationNode
{
    bool IsMutableBinding { get; }
    new DataType BindingType { get; }
    Pseudotype IBindingNode.BindingType => BindingType;
    LexicalScope ContainingLexicalScope { get; }
}

[Closed(
    typeof(IVariableBindingNode),
    typeof(INamedParameterNode))]
public partial interface ILocalBindingNode : INamedBindingNode
{
    new ILocalBindingSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
public partial interface IVariableBindingNode : ILocalBindingNode, IDataFlowNode
{
}

// [Closed(typeof(PackageNode))]
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
    DiagnosticCollection Diagnostics { get; }
    IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; }
    IFunctionDefinitionNode? EntryPoint { get; }
    IPackageSymbols PackageSymbols { get; }
}

// [Closed(typeof(PackageReferenceNode))]
public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPackageSymbolNode SymbolNode { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols PackageSymbols { get; }
    bool IsTrusted { get; }
}

// [Closed(typeof(PackageFacetNode))]
public partial interface IPackageFacetNode : IPackageFacetDeclarationNode
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
    typeof(IControlFlowNode),
    typeof(IAmbiguousExpressionNode))]
public partial interface ICodeNode : IChildNode
{
    new ICodeSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    CodeFile File { get; }
}

// [Closed(typeof(CompilationUnitNode))]
public partial interface ICompilationUnitNode : ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IPackageFacetNode ContainingDeclaration { get; }
    PackageSymbol ContainingSymbol { get; }
    NamespaceName ImplicitNamespaceName { get; }
    INamespaceDefinitionNode ImplicitNamespace { get; }
    NamespaceSymbol ImplicitNamespaceSymbol { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    NamespaceScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    DiagnosticCollection Diagnostics { get; }
}

// [Closed(typeof(UsingDirectiveNode))]
public partial interface IUsingDirectiveNode : ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
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
    ICodeSyntax? ICodeNode.Syntax => Syntax;
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
public partial interface IInvocableDefinitionNode : IDefinitionNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    InvocableSymbol Symbol { get; }
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IFieldDefinitionNode))]
public partial interface IExecutableDefinitionNode : IDefinitionNode
{
    ValueIdScope ValueIdScope { get; }
    IEntryNode Entry { get; }
    IExitNode Exit { get; }
    FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
}

[Closed(
    typeof(IConcreteFunctionInvocableDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode))]
public partial interface IConcreteInvocableDefinitionNode : IInvocableDefinitionNode, IExecutableDefinitionNode
{
    IBodyNode? Body { get; }
    new ValueIdScope ValueIdScope { get; }
    ValueIdScope IInvocableDefinitionNode.ValueIdScope => ValueIdScope;
    ValueIdScope IExecutableDefinitionNode.ValueIdScope => ValueIdScope;
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IConcreteFunctionInvocableDefinitionNode : IConcreteInvocableDefinitionNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new IFixedList<INamedParameterNode> Parameters { get; }
    ITypeNode? Return { get; }
    new IBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
    FunctionType Type { get; }
    new FunctionSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
}

// [Closed(typeof(NamespaceBlockDefinitionNode))]
public partial interface INamespaceBlockDefinitionNode : INamespaceBlockMemberDefinitionNode
{
    new INamespaceDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
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
public partial interface INamespaceBlockMemberDefinitionNode : IDefinitionNode
{
}

// [Closed(typeof(NamespaceDefinitionNode))]
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
public partial interface INamespaceMemberDefinitionNode : INamespaceMemberDeclarationNode
{
}

// [Closed(typeof(FunctionDefinitionNode))]
public partial interface IFunctionDefinitionNode : IPackageMemberDefinitionNode, IFunctionDeclarationNode, IConcreteFunctionInvocableDefinitionNode
{
    new IFunctionDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new INamespaceDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new IdentifierName Name { get; }
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    new FunctionType Type { get; }
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    new FunctionSymbol Symbol { get; }
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
public partial interface ITypeDefinitionNode : IPackageMemberDefinitionNode, IAssociatedMemberDefinitionNode, IUserTypeDeclarationNode
{
    new ITypeDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    bool IsConst { get; }
    new StandardName Name { get; }
    StandardName IAssociatedMemberDefinitionNode.Name => Name;
    IDeclaredUserType DeclaredType { get; }
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    LexicalScope SupertypesLexicalScope { get; }
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    new AccessModifier AccessModifier { get; }
    AccessModifier IPackageMemberDefinitionNode.AccessModifier => AccessModifier;
    AccessModifier ITypeMemberDefinitionNode.AccessModifier => AccessModifier;
}

// [Closed(typeof(ClassDefinitionNode))]
public partial interface IClassDefinitionNode : ITypeDefinitionNode, IClassDeclarationNode
{
    new IClassDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    bool IsAbstract { get; }
    IStandardTypeNameNode? BaseTypeName { get; }
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IClassMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IDefaultConstructorDefinitionNode? DefaultConstructor { get; }
}

// [Closed(typeof(StructDefinitionNode))]
public partial interface IStructDefinitionNode : ITypeDefinitionNode, IStructDeclarationNode
{
    new IStructDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IStructMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IDefaultInitializerDefinitionNode? DefaultInitializer { get; }
}

// [Closed(typeof(TraitDefinitionNode))]
public partial interface ITraitDefinitionNode : ITypeDefinitionNode, ITraitDeclarationNode
{
    new ITraitDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
}

// [Closed(typeof(GenericParameterNode))]
public partial interface IGenericParameterNode : ICodeNode, IGenericParameterDeclarationNode
{
    new IGenericParameterSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    TypeParameterIndependence Independence { get; }
    TypeParameterVariance Variance { get; }
    GenericParameter Parameter { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    GenericParameterType DeclaredType { get; }
    IUserTypeDeclarationNode ContainingDeclaration { get; }
    UserTypeSymbol ContainingSymbol { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(ITraitMemberDefinitionNode),
    typeof(IStructMemberDefinitionNode),
    typeof(IAlwaysTypeMemberDefinitionNode))]
public partial interface ITypeMemberDefinitionNode : IDefinitionNode, ITypeMemberDeclarationNode
{
    new ITypeMemberDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    AccessModifier AccessModifier { get; }
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IFieldDefinitionNode))]
public partial interface IClassMemberDefinitionNode : ITypeMemberDefinitionNode, IClassMemberDeclarationNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode))]
public partial interface ITraitMemberDefinitionNode : ITypeMemberDefinitionNode, ITraitMemberDeclarationNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode))]
public partial interface IStructMemberDefinitionNode : ITypeMemberDefinitionNode, IStructMemberDeclarationNode
{
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
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode))]
public partial interface IMethodDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IInvocableDefinitionNode, IMethodDeclarationNode
{
    new IMethodDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    MethodKind Kind { get; }
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new MethodSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    MethodSymbol IMethodDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(AbstractMethodDefinitionNode))]
public partial interface IAbstractMethodDefinitionNode : IMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IAbstractMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ObjectType ContainingDeclaredType { get; }
}

[Closed(
    typeof(IStandardMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode))]
public partial interface IConcreteMethodDefinitionNode : IMethodDefinitionNode, IStructMemberDefinitionNode, IConcreteInvocableDefinitionNode
{
    new IConcreteMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<INamedParameterNode> IMethodDefinitionNode.Parameters => Parameters;
    new IBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

// [Closed(typeof(StandardMethodDefinitionNode))]
public partial interface IStandardMethodDefinitionNode : IConcreteMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IStandardMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
}

// [Closed(typeof(GetterMethodDefinitionNode))]
public partial interface IGetterMethodDefinitionNode : IConcreteMethodDefinitionNode, IGetterMethodDeclarationNode
{
    new IGetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    new ITypeNode Return { get; }
}

// [Closed(typeof(SetterMethodDefinitionNode))]
public partial interface ISetterMethodDefinitionNode : IConcreteMethodDefinitionNode, ISetterMethodDeclarationNode
{
    new ISetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
}

[Closed(
    typeof(IDefaultConstructorDefinitionNode),
    typeof(ISourceConstructorDefinitionNode))]
public partial interface IConstructorDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IConstructorDeclarationNode
{
    new IConstructorDefinitionSyntax? Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new ConstructorSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    ConstructorSymbol IConstructorDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultConstructorDefinitionNode))]
public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{
}

// [Closed(typeof(SourceConstructorDefinitionNode))]
public partial interface ISourceConstructorDefinitionNode : IConstructorDefinitionNode
{
    new IConstructorDefinitionSyntax Syntax { get; }
    IConstructorDefinitionSyntax? IConstructorDefinitionNode.Syntax => Syntax;
    IConstructorSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

[Closed(
    typeof(IDefaultInitializerDefinitionNode),
    typeof(ISourceInitializerDefinitionNode))]
public partial interface IInitializerDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
{
    new IInitializerDefinitionSyntax? Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new InitializerSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultInitializerDefinitionNode))]
public partial interface IDefaultInitializerDefinitionNode : IInitializerDefinitionNode
{
}

// [Closed(typeof(SourceInitializerDefinitionNode))]
public partial interface ISourceInitializerDefinitionNode : IInitializerDefinitionNode
{
    new IInitializerDefinitionSyntax Syntax { get; }
    IInitializerDefinitionSyntax? IInitializerDefinitionNode.Syntax => Syntax;
    IInitializerSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

// [Closed(typeof(FieldDefinitionNode))]
public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, INamedBindingNode, IFieldDeclarationNode, IExecutableDefinitionNode
{
    new IFieldDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new DataType BindingType { get; }
    DataType INamedBindingNode.BindingType => BindingType;
    DataType IFieldDeclarationNode.BindingType => BindingType;
    IAmbiguousExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    IExpressionNode? IntermediateInitializer { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
}

// [Closed(typeof(AssociatedFunctionDefinitionNode))]
public partial interface IAssociatedFunctionDefinitionNode : IConcreteFunctionInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IAssociatedMemberDefinitionNode, IAssociatedFunctionDeclarationNode
{
    new IAssociatedFunctionDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    StandardName IAssociatedMemberDefinitionNode.Name => Name;
    StandardName IAssociatedFunctionDeclarationNode.Name => Name;
    new FunctionSymbol Symbol { get; }
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    new FunctionType Type { get; }
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
}

// [Closed(typeof(AttributeNode))]
public partial interface IAttributeNode : ICodeNode
{
    new IAttributeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IStandardTypeNameNode TypeName { get; }
    ConstructorSymbol? ReferencedSymbol { get; }
}

[Closed(
    typeof(ICapabilitySetNode),
    typeof(ICapabilityNode))]
public partial interface ICapabilityConstraintNode : ICodeNode
{
    new ICapabilityConstraintSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityConstraint Constraint { get; }
}

// [Closed(typeof(CapabilitySetNode))]
public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

// [Closed(typeof(CapabilityNode))]
public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    Capability Capability { get; }
}

[Closed(
    typeof(IConstructorOrInitializerParameterNode),
    typeof(ISelfParameterNode))]
public partial interface IParameterNode : ICodeNode
{
    new IParameterSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    bool Unused { get; }
    IMaybeAntetype BindingAntetype { get; }
    Pseudotype BindingType { get; }
    ValueId BindingValueId { get; }
    IFlowState FlowStateAfter { get; }
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IFieldParameterNode))]
public partial interface IConstructorOrInitializerParameterNode : IParameterNode
{
    new IConstructorOrInitializerParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    new DataType BindingType { get; }
    Pseudotype IParameterNode.BindingType => BindingType;
    ParameterType ParameterType { get; }
}

// [Closed(typeof(NamedParameterNode))]
public partial interface INamedParameterNode : IConstructorOrInitializerParameterNode, ILocalBindingNode
{
    new INamedParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new DataType BindingType { get; }
    DataType IConstructorOrInitializerParameterNode.BindingType => BindingType;
    DataType INamedBindingNode.BindingType => BindingType;
    new IMaybeAntetype BindingAntetype { get; }
    IMaybeAntetype IParameterNode.BindingAntetype => BindingAntetype;
    IMaybeAntetype IBindingNode.BindingAntetype => BindingAntetype;
    new ValueId BindingValueId { get; }
    ValueId IParameterNode.BindingValueId => BindingValueId;
    ValueId IBindingNode.BindingValueId => BindingValueId;
}

[Closed(
    typeof(IConstructorSelfParameterNode),
    typeof(IInitializerSelfParameterNode),
    typeof(IMethodSelfParameterNode))]
public partial interface ISelfParameterNode : IParameterNode, IBindingNode
{
    new ISelfParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    SelfParameterType ParameterType { get; }
    new IMaybeAntetype BindingAntetype { get; }
    IMaybeAntetype IParameterNode.BindingAntetype => BindingAntetype;
    IMaybeAntetype IBindingNode.BindingAntetype => BindingAntetype;
    new Pseudotype BindingType { get; }
    Pseudotype IParameterNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new ValueId BindingValueId { get; }
    ValueId IParameterNode.BindingValueId => BindingValueId;
    ValueId IBindingNode.BindingValueId => BindingValueId;
}

// [Closed(typeof(ConstructorSelfParameterNode))]
public partial interface IConstructorSelfParameterNode : ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype ISelfParameterNode.BindingType => BindingType;
    new ObjectType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

// [Closed(typeof(InitializerSelfParameterNode))]
public partial interface IInitializerSelfParameterNode : ISelfParameterNode
{
    new IInitializerSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype ISelfParameterNode.BindingType => BindingType;
    new StructType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

// [Closed(typeof(MethodSelfParameterNode))]
public partial interface IMethodSelfParameterNode : ISelfParameterNode
{
    new IMethodSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    ICapabilityConstraintNode Capability { get; }
}

// [Closed(typeof(FieldParameterNode))]
public partial interface IFieldParameterNode : IConstructorOrInitializerParameterNode
{
    new IFieldParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
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
    IFlowState FlowStateAfter { get; }
}

// [Closed(typeof(BlockBodyNode))]
public partial interface IBlockBodyNode : IBodyNode
{
    new IBlockBodySyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    new IFixedList<IBodyStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
}

// [Closed(typeof(ExpressionBodyNode))]
public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IResultStatementNode ResultStatement { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
}

[Closed(
    typeof(ITypeNameNode),
    typeof(IOptionalTypeNode),
    typeof(ICapabilityTypeNode),
    typeof(IFunctionTypeNode),
    typeof(IViewpointTypeNode))]
public partial interface ITypeNode : ICodeNode
{
    new ITypeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IMaybeAntetype NamedAntetype { get; }
    DataType NamedType { get; }
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
    BareType? NamedBareType { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(IGenericTypeNameNode))]
public partial interface IStandardTypeNameNode : ITypeNameNode
{
    new IStandardTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    bool IsAttributeType { get; }
    new StandardName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    ITypeDeclarationNode? ReferencedDeclaration { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(ISpecialTypeNameNode))]
public partial interface ISimpleTypeNameNode : ITypeNameNode
{
    new ISimpleTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
}

// [Closed(typeof(IdentifierTypeNameNode))]
public partial interface IIdentifierTypeNameNode : IStandardTypeNameNode, ISimpleTypeNameNode
{
    new IIdentifierTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
}

// [Closed(typeof(SpecialTypeNameNode))]
public partial interface ISpecialTypeNameNode : ISimpleTypeNameNode
{
    new ISpecialTypeNameSyntax Syntax { get; }
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    new SpecialTypeName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    new TypeSymbol ReferencedSymbol { get; }
    TypeSymbol? ITypeNameNode.ReferencedSymbol => ReferencedSymbol;
}

// [Closed(typeof(GenericTypeNameNode))]
public partial interface IGenericTypeNameNode : IStandardTypeNameNode
{
    new IGenericTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

// [Closed(typeof(QualifiedTypeNameNode))]
public partial interface IQualifiedTypeNameNode : ITypeNameNode
{
    new IQualifiedTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeNameNode Context { get; }
    IStandardTypeNameNode QualifiedName { get; }
}

// [Closed(typeof(OptionalTypeNode))]
public partial interface IOptionalTypeNode : ITypeNode
{
    new IOptionalTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ITypeNode Referent { get; }
}

// [Closed(typeof(CapabilityTypeNode))]
public partial interface ICapabilityTypeNode : ITypeNode
{
    new ICapabilityTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    ITypeNode Referent { get; }
}

// [Closed(typeof(FunctionTypeNode))]
public partial interface IFunctionTypeNode : ITypeNode
{
    new IFunctionTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    IFixedList<IParameterTypeNode> Parameters { get; }
    ITypeNode Return { get; }
}

// [Closed(typeof(ParameterTypeNode))]
public partial interface IParameterTypeNode : ICodeNode
{
    new IParameterTypeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    bool IsLent { get; }
    ITypeNode Referent { get; }
    ParameterType Parameter { get; }
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

// [Closed(typeof(CapabilityViewpointTypeNode))]
public partial interface ICapabilityViewpointTypeNode : IViewpointTypeNode
{
    new ICapabilityViewpointTypeSyntax Syntax { get; }
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
}

// [Closed(typeof(SelfViewpointTypeNode))]
public partial interface ISelfViewpointTypeNode : IViewpointTypeNode
{
    new ISelfViewpointTypeSyntax Syntax { get; }
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    Pseudotype? NamedSelfType { get; }
}

[Closed(
    typeof(IElseClauseNode),
    typeof(IDataFlowNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IExpressionNode))]
public partial interface IControlFlowNode : ICodeNode
{
    ControlFlowSet ControlFlowNext { get; }
    ControlFlowSet ControlFlowPrevious { get; }
}

// [Closed(typeof(EntryNode))]
public partial interface IEntryNode : IDataFlowNode
{
}

// [Closed(typeof(ExitNode))]
public partial interface IExitNode : IDataFlowNode
{
}

[Closed(
    typeof(IVariableBindingNode),
    typeof(IEntryNode),
    typeof(IExitNode),
    typeof(IAssignmentExpressionNode))]
public partial interface IDataFlowNode : IControlFlowNode
{
    IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBodyStatementNode))]
public partial interface IStatementNode : IControlFlowNode
{
    new IStatementSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IMaybeAntetype? ResultAntetype { get; }
    DataType? ResultType { get; }
    ValueId? ResultValueId { get; }
    IFlowState FlowStateAfter { get; }
}

// [Closed(typeof(ResultStatementNode))]
public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IExpressionNode? IntermediateExpression { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
    new IFlowState FlowStateAfter { get; }
    IFlowState IStatementNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
}

// [Closed(typeof(VariableDeclarationStatementNode))]
public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, IVariableBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IAmbiguousExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    IExpressionNode? IntermediateInitializer { get; }
    LexicalScope LexicalScope { get; }
}

// [Closed(typeof(ExpressionStatementNode))]
public partial interface IExpressionStatementNode : IBodyStatementNode
{
    new IExpressionStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

[Closed(
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode))]
public partial interface IPatternNode : IControlFlowNode
{
    new IPatternSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    IFlowState FlowStateAfter { get; }
}

// [Closed(typeof(BindingContextPatternNode))]
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

// [Closed(typeof(BindingPatternNode))]
public partial interface IBindingPatternNode : IOptionalOrBindingPatternNode, IVariableBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
}

// [Closed(typeof(OptionalPatternNode))]
public partial interface IOptionalPatternNode : IOptionalOrBindingPatternNode
{
    new IOptionalPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IOptionalOrBindingPatternNode Pattern { get; }
}

[Closed(
    typeof(IAmbiguousAssignableExpressionNode),
    typeof(IExpressionNode),
    typeof(IUnresolvedInvocationExpressionNode),
    typeof(IAmbiguousNameExpressionNode),
    typeof(IAmbiguousMoveExpressionNode),
    typeof(IAmbiguousFreezeExpressionNode))]
public partial interface IAmbiguousExpressionNode : ICodeNode
{
    new IExpressionSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ValueId ValueId { get; }
}

[Closed(
    typeof(IAssignableExpressionNode),
    typeof(IIdentifierNameExpressionNode),
    typeof(IMemberAccessExpressionNode),
    typeof(IPropertyNameNode))]
public partial interface IAmbiguousAssignableExpressionNode : IAmbiguousExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IAssignableExpressionNode),
    typeof(IBlockExpressionNode),
    typeof(IUnsafeExpressionNode),
    typeof(INeverTypedExpressionNode),
    typeof(ILiteralExpressionNode),
    typeof(IAssignmentExpressionNode),
    typeof(IBinaryOperatorExpressionNode),
    typeof(IUnaryOperatorExpressionNode),
    typeof(IIdExpressionNode),
    typeof(IConversionExpressionNode),
    typeof(IImplicitConversionExpressionNode),
    typeof(IPatternMatchExpressionNode),
    typeof(IIfExpressionNode),
    typeof(ILoopExpressionNode),
    typeof(IWhileExpressionNode),
    typeof(IForeachExpressionNode),
    typeof(IInvocationExpressionNode),
    typeof(IUnknownInvocationExpressionNode),
    typeof(INameExpressionNode),
    typeof(IRecoveryExpressionNode),
    typeof(IImplicitTempMoveExpressionNode),
    typeof(IPrepareToReturnExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface IExpressionNode : IAmbiguousExpressionNode, IControlFlowNode
{
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    IMaybeExpressionAntetype Antetype { get; }
    DataType? ExpectedType { get; }
    DataType Type { get; }
    IFlowState FlowStateAfter { get; }
}

[Closed(
    typeof(IFieldAccessExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownIdentifierNameExpressionNode),
    typeof(IUnknownMemberAccessExpressionNode))]
public partial interface IAssignableExpressionNode : IExpressionNode, IAmbiguousAssignableExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
}

// [Closed(typeof(BlockExpressionNode))]
public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    new IFixedList<IStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
    new IMaybeAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;
    IMaybeAntetype IBlockOrResultNode.Antetype => Antetype;
    new DataType Type { get; }
    DataType IExpressionNode.Type => Type;
    DataType IBlockOrResultNode.Type => Type;
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;
}

// [Closed(typeof(NewObjectExpressionNode))]
public partial interface INewObjectExpressionNode : IInvocationExpressionNode
{
    new INewObjectExpressionSyntax Syntax { get; }
    ITypeNameNode ConstructingType { get; }
    IdentifierName? ConstructorName { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IMaybeAntetype ConstructingAntetype { get; }
    IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { get; }
    IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { get; }
    IConstructorDeclarationNode? ReferencedConstructor { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(UnsafeExpressionNode))]
public partial interface IUnsafeExpressionNode : IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
public partial interface INeverTypedExpressionNode : IExpressionNode
{
    new NeverType Type { get; }
    DataType IExpressionNode.Type => Type;
}

[Closed(
    typeof(IBoolLiteralExpressionNode),
    typeof(IIntegerLiteralExpressionNode),
    typeof(INoneLiteralExpressionNode),
    typeof(IStringLiteralExpressionNode))]
public partial interface ILiteralExpressionNode : IExpressionNode
{
    new ILiteralExpressionSyntax Syntax { get; }
}

// [Closed(typeof(BoolLiteralExpressionNode))]
public partial interface IBoolLiteralExpressionNode : ILiteralExpressionNode
{
    new IBoolLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    bool Value { get; }
    new BoolConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;
}

// [Closed(typeof(IntegerLiteralExpressionNode))]
public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    new IIntegerLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    BigInteger Value { get; }
    new IntegerConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;
}

// [Closed(typeof(NoneLiteralExpressionNode))]
public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    new INoneLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    new OptionalType Type { get; }
    DataType IExpressionNode.Type => Type;
}

// [Closed(typeof(StringLiteralExpressionNode))]
public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    new IStringLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    string Value { get; }
    LexicalScope ContainingLexicalScope { get; }
}

// [Closed(typeof(AssignmentExpressionNode))]
public partial interface IAssignmentExpressionNode : IExpressionNode, IDataFlowNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IAmbiguousAssignableExpressionNode LeftOperand { get; }
    IAmbiguousAssignableExpressionNode CurrentLeftOperand { get; }
    IAssignableExpressionNode? IntermediateLeftOperand { get; }
    AssignmentOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
    IAmbiguousExpressionNode CurrentRightOperand { get; }
    IExpressionNode? IntermediateRightOperand { get; }
}

// [Closed(typeof(BinaryOperatorExpressionNode))]
public partial interface IBinaryOperatorExpressionNode : IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode LeftOperand { get; }
    IExpressionNode? IntermediateLeftOperand { get; }
    BinaryOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
    IExpressionNode? IntermediateRightOperand { get; }
    IAntetype? NumericOperatorCommonAntetype { get; }
    LexicalScope ContainingLexicalScope { get; }
}

// [Closed(typeof(UnaryOperatorExpressionNode))]
public partial interface IUnaryOperatorExpressionNode : IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IAmbiguousExpressionNode Operand { get; }
    IExpressionNode? IntermediateOperand { get; }
}

// [Closed(typeof(IdExpressionNode))]
public partial interface IIdExpressionNode : IExpressionNode
{
    new IIdExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
}

// [Closed(typeof(ConversionExpressionNode))]
public partial interface IConversionExpressionNode : IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }
}

// [Closed(typeof(ImplicitConversionExpressionNode))]
public partial interface IImplicitConversionExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    new SimpleAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;
}

// [Closed(typeof(PatternMatchExpressionNode))]
public partial interface IPatternMatchExpressionNode : IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
    IPatternNode Pattern { get; }
}

// [Closed(typeof(IfExpressionNode))]
public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    IAmbiguousExpressionNode Condition { get; }
    IExpressionNode? IntermediateCondition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;
}

// [Closed(typeof(LoopExpressionNode))]
public partial interface ILoopExpressionNode : IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    IBlockExpressionNode Block { get; }
}

// [Closed(typeof(WhileExpressionNode))]
public partial interface IWhileExpressionNode : IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Condition { get; }
    IExpressionNode? IntermediateCondition { get; }
    IBlockExpressionNode Block { get; }
}

// [Closed(typeof(ForeachExpressionNode))]
public partial interface IForeachExpressionNode : IExpressionNode, IVariableBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IdentifierName VariableName { get; }
    IAmbiguousExpressionNode InExpression { get; }
    IExpressionNode? IntermediateInExpression { get; }
    ITypeNode? DeclaredType { get; }
    IBlockExpressionNode Block { get; }
    LexicalScope LexicalScope { get; }
    ITypeDeclarationNode? ReferencedIterableDeclaration { get; }
    IStandardMethodDeclarationNode? ReferencedIterateMethod { get; }
    IMaybeExpressionAntetype IteratorAntetype { get; }
    DataType IteratorType { get; }
    ITypeDeclarationNode? ReferencedIteratorDeclaration { get; }
    IStandardMethodDeclarationNode? ReferencedNextMethod { get; }
    IMaybeAntetype IteratedAntetype { get; }
    DataType IteratedType { get; }
    IFlowState FlowStateBeforeBlock { get; }
}

// [Closed(typeof(BreakExpressionNode))]
public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode? Value { get; }
    IExpressionNode? IntermediateValue { get; }
}

// [Closed(typeof(NextExpressionNode))]
public partial interface INextExpressionNode : INeverTypedExpressionNode
{
    new INextExpressionSyntax Syntax { get; }
}

// [Closed(typeof(ReturnExpressionNode))]
public partial interface IReturnExpressionNode : INeverTypedExpressionNode
{
    new IReturnExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode? Value { get; }
    IAmbiguousExpressionNode? CurrentValue { get; }
    IExpressionNode? IntermediateValue { get; }
}

// [Closed(typeof(UnresolvedInvocationExpressionNode))]
public partial interface IUnresolvedInvocationExpressionNode : IAmbiguousExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
}

[Closed(
    typeof(INewObjectExpressionNode),
    typeof(IFunctionInvocationExpressionNode),
    typeof(IMethodInvocationExpressionNode),
    typeof(IGetterInvocationExpressionNode),
    typeof(ISetterInvocationExpressionNode),
    typeof(IFunctionReferenceInvocationExpressionNode),
    typeof(IInitializerInvocationExpressionNode))]
public partial interface IInvocationExpressionNode : IExpressionNode
{
    IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
}

// [Closed(typeof(FunctionInvocationExpressionNode))]
public partial interface IFunctionInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IFunctionGroupNameNode FunctionGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(MethodInvocationExpressionNode))]
public partial interface IMethodInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IMethodGroupNameNode MethodGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { get; }
    IStandardMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(GetterInvocationExpressionNode))]
public partial interface IGetterInvocationExpressionNode : IInvocationExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(SetterInvocationExpressionNode))]
public partial interface ISetterInvocationExpressionNode : IInvocationExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IAmbiguousExpressionNode Value { get; }
    IExpressionNode? IntermediateValue { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(FunctionReferenceInvocationExpressionNode))]
public partial interface IFunctionReferenceInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionNode Expression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    FunctionAntetype FunctionAntetype { get; }
    FunctionType FunctionType { get; }
}

// [Closed(typeof(InitializerInvocationExpressionNode))]
public partial interface IInitializerInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IInitializerGroupNameNode InitializerGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; }
    IInitializerDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

// [Closed(typeof(UnknownInvocationExpressionNode))]
public partial interface IUnknownInvocationExpressionNode : IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Expression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
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
    typeof(IMemberAccessExpressionNode),
    typeof(IPropertyNameNode))]
public partial interface IAmbiguousNameNode : IAmbiguousNameExpressionNode
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
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode))]
public partial interface IStandardNameExpressionNode : IAmbiguousNameNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    StandardName Name { get; }
    LexicalScope ContainingLexicalScope { get; }
    IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(IdentifierNameExpressionNode))]
public partial interface IIdentifierNameExpressionNode : IStandardNameExpressionNode, ISimpleNameNode, IAmbiguousAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;
}

// [Closed(typeof(GenericNameExpressionNode))]
public partial interface IGenericNameExpressionNode : IStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

// [Closed(typeof(MemberAccessExpressionNode))]
public partial interface IMemberAccessExpressionNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
}

// [Closed(typeof(PropertyNameNode))]
public partial interface IPropertyNameNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
}

[Closed(
    typeof(ILocalBindingNameExpressionNode),
    typeof(INamespaceNameNode),
    typeof(IFunctionGroupNameNode),
    typeof(IFunctionNameNode),
    typeof(IMethodGroupNameNode),
    typeof(IFieldAccessExpressionNode),
    typeof(ITypeNameExpressionNode),
    typeof(IInitializerGroupNameNode),
    typeof(ISpecialTypeNameExpressionNode),
    typeof(IInstanceExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownNameExpressionNode))]
public partial interface INameExpressionNode : IExpressionNode, IAmbiguousNameExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
}

[Closed(
    typeof(IVariableNameExpressionNode),
    typeof(ISelfExpressionNode))]
public partial interface ILocalBindingNameExpressionNode : INameExpressionNode
{
    IBindingNode? ReferencedDefinition { get; }
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

// [Closed(typeof(UnqualifiedNamespaceNameNode))]
public partial interface IUnqualifiedNamespaceNameNode : INamespaceNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IdentifierName Name { get; }
}

// [Closed(typeof(QualifiedNamespaceNameNode))]
public partial interface IQualifiedNamespaceNameNode : INamespaceNameNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
    IdentifierName Name { get; }
}

// [Closed(typeof(FunctionGroupNameNode))]
public partial interface IFunctionGroupNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(FunctionNameNode))]
public partial interface IFunctionNameNode : INameExpressionNode
{
    IFunctionGroupNameNode FunctionGroup { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
}

// [Closed(typeof(MethodGroupNameNode))]
public partial interface IMethodGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    StandardName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(FieldAccessExpressionNode))]
public partial interface IFieldAccessExpressionNode : INameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IdentifierName FieldName { get; }
    IFieldDeclarationNode ReferencedDeclaration { get; }
}

// [Closed(typeof(VariableNameExpressionNode))]
public partial interface IVariableNameExpressionNode : ILocalBindingNameExpressionNode, IAssignableExpressionNode, ISimpleNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IdentifierName Name { get; }
    new ILocalBindingNode ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
}

[Closed(
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode))]
public partial interface ITypeNameExpressionNode : INameExpressionNode
{
    StandardName Name { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    ITypeDeclarationNode ReferencedDeclaration { get; }
    IMaybeAntetype NamedAntetype { get; }
    BareType? NamedBareType { get; }
}

// [Closed(typeof(StandardTypeNameExpressionNode))]
public partial interface IStandardTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
}

// [Closed(typeof(QualifiedTypeNameExpressionNode))]
public partial interface IQualifiedTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INamespaceNameNode Context { get; }
}

// [Closed(typeof(InitializerGroupNameNode))]
public partial interface IInitializerGroupNameNode : INameExpressionNode
{
    ITypeNameExpressionNode Context { get; }
    StandardName? InitializerName { get; }
    IMaybeAntetype InitializingAntetype { get; }
    IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(SpecialTypeNameExpressionNode))]
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
    new IInstanceExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
}

// [Closed(typeof(SelfExpressionNode))]
public partial interface ISelfExpressionNode : IInstanceExpressionNode, ILocalBindingNameExpressionNode
{
    new ISelfExpressionSyntax Syntax { get; }
    IInstanceExpressionSyntax IInstanceExpressionNode.Syntax => Syntax;
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
    IExecutableDefinitionNode ContainingDeclaration { get; }
    new ISelfParameterNode? ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
}

// [Closed(typeof(MissingNameExpressionNode))]
public partial interface IMissingNameExpressionNode : INameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IMissingNameSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
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
public partial interface IUnknownStandardNameExpressionNode : IUnknownNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    StandardName Name { get; }
    IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(UnknownIdentifierNameExpressionNode))]
public partial interface IUnknownIdentifierNameExpressionNode : IUnknownStandardNameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;
}

// [Closed(typeof(UnknownGenericNameExpressionNode))]
public partial interface IUnknownGenericNameExpressionNode : IUnknownStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }
}

// [Closed(typeof(UnknownMemberAccessExpressionNode))]
public partial interface IUnknownMemberAccessExpressionNode : IUnknownNameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IDeclarationNode> ReferencedMembers { get; }
}

// [Closed(typeof(AmbiguousMoveExpressionNode))]
public partial interface IAmbiguousMoveExpressionNode : IAmbiguousExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISimpleNameNode Referent { get; }
    INameExpressionNode? IntermediateReferent { get; }
}

[Closed(
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode))]
public partial interface IRecoveryExpressionNode : IExpressionNode
{
    bool IsImplicit { get; }
}

[Closed(
    typeof(IMoveVariableExpressionNode),
    typeof(IMoveValueExpressionNode))]
public partial interface IMoveExpressionNode : IRecoveryExpressionNode
{
    IExpressionNode Referent { get; }
}

// [Closed(typeof(MoveVariableExpressionNode))]
public partial interface IMoveVariableExpressionNode : IMoveExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IMoveExpressionNode.Referent => Referent;
}

// [Closed(typeof(MoveValueExpressionNode))]
public partial interface IMoveValueExpressionNode : IMoveExpressionNode
{
}

// [Closed(typeof(ImplicitTempMoveExpressionNode))]
public partial interface IImplicitTempMoveExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
}

// [Closed(typeof(AmbiguousFreezeExpressionNode))]
public partial interface IAmbiguousFreezeExpressionNode : IAmbiguousExpressionNode
{
    new IFreezeExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISimpleNameNode Referent { get; }
    INameExpressionNode? IntermediateReferent { get; }
}

[Closed(
    typeof(IFreezeVariableExpressionNode),
    typeof(IFreezeValueExpressionNode))]
public partial interface IFreezeExpressionNode : IRecoveryExpressionNode
{
    IExpressionNode Referent { get; }
    bool IsTemporary { get; }
}

// [Closed(typeof(FreezeVariableExpressionNode))]
public partial interface IFreezeVariableExpressionNode : IFreezeExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IFreezeExpressionNode.Referent => Referent;
}

// [Closed(typeof(FreezeValueExpressionNode))]
public partial interface IFreezeValueExpressionNode : IFreezeExpressionNode
{
}

// [Closed(typeof(PrepareToReturnExpressionNode))]
public partial interface IPrepareToReturnExpressionNode : IExpressionNode
{
    IExpressionNode Value { get; }
    IExpressionNode CurrentValue { get; }
}

// [Closed(typeof(AsyncBlockExpressionNode))]
public partial interface IAsyncBlockExpressionNode : IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    IBlockExpressionNode Block { get; }
}

// [Closed(typeof(AsyncStartExpressionNode))]
public partial interface IAsyncStartExpressionNode : IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    bool Scheduled { get; }
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

// [Closed(typeof(AwaitExpressionNode))]
public partial interface IAwaitExpressionNode : IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

[Closed(
    typeof(IChildDeclarationNode),
    typeof(ISymbolDeclarationNode))]
public partial interface IDeclarationNode : ISemanticNode
{
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IInvocableDeclarationNode),
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
public partial interface INamedDeclarationNode : IChildDeclarationNode
{
    TypeName Name { get; }
}

[Closed(
    typeof(IInvocableDeclarationNode),
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
    typeof(IFunctionLikeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode))]
public partial interface IInvocableDeclarationNode : ISymbolDeclarationNode, IChildDeclarationNode
{
    new InvocableSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IBindingNode),
    typeof(INamedBindingDeclarationNode))]
public partial interface IBindingDeclarationNode : IChildDeclarationNode
{
}

[Closed(
    typeof(INamedBindingNode))]
public partial interface INamedBindingDeclarationNode : IBindingDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IPackageNode),
    typeof(IPackageSymbolNode))]
public partial interface IPackageDeclarationNode : ISymbolDeclarationNode
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
public partial interface IPackageMemberDeclarationNode : INamespaceMemberDeclarationNode
{
}

[Closed(
    typeof(IPackageFacetNode),
    typeof(IPackageFacetSymbolNode))]
public partial interface IPackageFacetDeclarationNode : IChildDeclarationNode, ISymbolDeclarationNode
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
public partial interface IPackageFacetChildDeclarationNode : IChildDeclarationNode
{
    StandardName? Name { get; }
    IPackageFacetDeclarationNode Facet { get; }
}

[Closed(
    typeof(INamespaceDefinitionNode),
    typeof(INamespaceSymbolNode))]
public partial interface INamespaceDeclarationNode : INamespaceMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamespaceMemberDeclarationNode.Name => Name;
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
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IFunctionLikeDeclarationNode : INamedDeclarationNode, IInvocableDeclarationNode
{
    new FunctionSymbol Symbol { get; }
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    FunctionType Type { get; }
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IFunctionSymbolNode))]
public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode, IFunctionLikeDeclarationNode
{
}

[Closed(
    typeof(IPrimitiveTypeSymbolNode))]
public partial interface IPrimitiveTypeDeclarationNode : ITypeDeclarationNode
{
    new SpecialTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IUserTypeSymbolNode))]
public partial interface IUserTypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ITypeDeclarationNode
{
    IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    new UserTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IClassSymbolNode))]
public partial interface IClassDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<IClassMemberDeclarationNode> Members { get; }
    new IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IStructDefinitionNode),
    typeof(IStructSymbolNode))]
public partial interface IStructDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<IStructMemberDeclarationNode> Members { get; }
    new IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(ITraitDefinitionNode),
    typeof(ITraitSymbolNode))]
public partial interface ITraitDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<ITraitMemberDeclarationNode> Members { get; }
    new IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IGenericParameterNode),
    typeof(IGenericParameterSymbolNode))]
public partial interface IGenericParameterDeclarationNode : ITypeDeclarationNode, IAssociatedMemberDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new GenericParameterTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
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
    typeof(IFieldDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(ITraitMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IStructMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IGenericParameterDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IAssociatedMemberDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
public partial interface IInstanceMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IStandardMethodDeclarationNode),
    typeof(IPropertyAccessorDeclarationNode),
    typeof(IMethodSymbolNode))]
public partial interface IMethodDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode, IInstanceMemberDeclarationNode, IInvocableDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new MethodSymbol Symbol { get; }
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IStandardMethodDefinitionNode),
    typeof(IStandardMethodSymbolNode))]
public partial interface IStandardMethodDeclarationNode : IMethodDeclarationNode
{
    int Arity { get; }
    FunctionType MethodGroupType { get; }
}

[Closed(
    typeof(IGetterMethodDeclarationNode),
    typeof(ISetterMethodDeclarationNode))]
public partial interface IPropertyAccessorDeclarationNode : IMethodDeclarationNode
{
}

[Closed(
    typeof(IGetterMethodDefinitionNode),
    typeof(IGetterMethodSymbolNode))]
public partial interface IGetterMethodDeclarationNode : IPropertyAccessorDeclarationNode
{
}

[Closed(
    typeof(ISetterMethodDefinitionNode),
    typeof(ISetterMethodSymbolNode))]
public partial interface ISetterMethodDeclarationNode : IPropertyAccessorDeclarationNode
{
}

[Closed(
    typeof(IConstructorDefinitionNode),
    typeof(IConstructorSymbolNode))]
public partial interface IConstructorDeclarationNode : IAssociatedMemberDeclarationNode, IInvocableDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new ConstructorSymbol Symbol { get; }
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IInitializerDefinitionNode),
    typeof(IInitializerSymbolNode))]
public partial interface IInitializerDeclarationNode : IAssociatedMemberDeclarationNode, IInvocableDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new InitializerSymbol Symbol { get; }
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IFieldDefinitionNode),
    typeof(IFieldSymbolNode))]
public partial interface IFieldDeclarationNode : INamedDeclarationNode, IClassMemberDeclarationNode, IStructMemberDeclarationNode, IInstanceMemberDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    DataType BindingType { get; }
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAssociatedFunctionDefinitionNode),
    typeof(IAssociatedFunctionSymbolNode))]
public partial interface IAssociatedFunctionDeclarationNode : IAssociatedMemberDeclarationNode, IFunctionLikeDeclarationNode
{
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IPrimitiveTypeDeclarationNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IGenericParameterDeclarationNode))]
public partial interface ITypeDeclarationNode : INamedDeclarationNode, ISymbolDeclarationNode
{
    new TypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IFixedSet<BareReferenceType> Supertypes { get; }
    IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
}

// [Closed(typeof(PackageSymbolNode))]
public partial interface IPackageSymbolNode : IPackageDeclarationNode
{
}

// [Closed(typeof(PackageFacetSymbolNode))]
public partial interface IPackageFacetSymbolNode : IPackageFacetDeclarationNode
{
}

// [Closed(typeof(NamespaceSymbolNode))]
public partial interface INamespaceSymbolNode : INamespaceDeclarationNode
{
    new IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;
}

// [Closed(typeof(FunctionSymbolNode))]
public partial interface IFunctionSymbolNode : IFunctionDeclarationNode
{
}

// [Closed(typeof(PrimitiveTypeSymbolNode))]
public partial interface IPrimitiveTypeSymbolNode : IPrimitiveTypeDeclarationNode
{
}

// [Closed(typeof(UserTypeSymbolNode))]
public partial interface IUserTypeSymbolNode : IUserTypeDeclarationNode
{
}

// [Closed(typeof(ClassSymbolNode))]
public partial interface IClassSymbolNode : IClassDeclarationNode
{
}

// [Closed(typeof(StructSymbolNode))]
public partial interface IStructSymbolNode : IStructDeclarationNode
{
}

// [Closed(typeof(TraitSymbolNode))]
public partial interface ITraitSymbolNode : ITraitDeclarationNode
{
}

// [Closed(typeof(GenericParameterSymbolNode))]
public partial interface IGenericParameterSymbolNode : IGenericParameterDeclarationNode
{
}

[Closed(
    typeof(IStandardMethodSymbolNode),
    typeof(IGetterMethodSymbolNode),
    typeof(ISetterMethodSymbolNode))]
public partial interface IMethodSymbolNode : IMethodDeclarationNode
{
}

// [Closed(typeof(StandardMethodSymbolNode))]
public partial interface IStandardMethodSymbolNode : IMethodSymbolNode, IStandardMethodDeclarationNode
{
}

// [Closed(typeof(GetterMethodSymbolNode))]
public partial interface IGetterMethodSymbolNode : IMethodSymbolNode, IGetterMethodDeclarationNode
{
}

// [Closed(typeof(SetterMethodSymbolNode))]
public partial interface ISetterMethodSymbolNode : IMethodSymbolNode, ISetterMethodDeclarationNode
{
}

// [Closed(typeof(ConstructorSymbolNode))]
public partial interface IConstructorSymbolNode : IConstructorDeclarationNode
{
}

// [Closed(typeof(InitializerSymbolNode))]
public partial interface IInitializerSymbolNode : IInitializerDeclarationNode
{
}

// [Closed(typeof(FieldSymbolNode))]
public partial interface IFieldSymbolNode : IFieldDeclarationNode
{
}

// [Closed(typeof(AssociatedFunctionSymbolNode))]
public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode
{
}

file class PackageNode // : IPackageNode
{
    public IPackageSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public PackageSymbol Symbol { get; } = default!;
    public IFixedSet<IPackageReferenceNode> References { get; } = default!;
    public IPackageReferenceNode IntrinsicsReference { get; } = default!;
    public FixedDictionary<IdentifierName,IPackageDeclarationNode> PackageDeclarations { get; } = default!;
    public IPackageFacetNode MainFacet { get; } = default!;
    public IPackageFacetNode TestingFacet { get; } = default!;
    public DiagnosticCollection Diagnostics { get; } = default!;
    public IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; } = default!;
    public IFunctionDefinitionNode? EntryPoint { get; } = default!;
    public IPackageSymbols PackageSymbols { get; } = default!;
    public IdentifierName? AliasOrName { get; } = default!;
}

file class PackageReferenceNode // : IPackageReferenceNode
{
    public IPackageReferenceSyntax? Syntax { get; } = default!;
    public IPackageSymbolNode SymbolNode { get; } = default!;
    public IdentifierName AliasOrName { get; } = default!;
    public IPackageSymbols PackageSymbols { get; } = default!;
    public bool IsTrusted { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class PackageFacetNode // : IPackageFacetNode
{
    public IPackageSyntax Syntax { get; } = default!;
    public IdentifierName PackageName { get; } = default!;
    public PackageSymbol PackageSymbol { get; } = default!;
    public PackageNameScope PackageNameScope { get; } = default!;
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; } = default!;
    public IFixedSet<IPackageMemberDefinitionNode> Definitions { get; } = default!;
    public INamespaceDefinitionNode GlobalNamespace { get; } = default!;
    public IdentifierName? PackageAliasOrName { get; } = default!;
    public PackageSymbol Symbol { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class CompilationUnitNode // : ICompilationUnitNode
{
    public ICompilationUnitSyntax Syntax { get; } = default!;
    public IPackageFacetNode ContainingDeclaration { get; } = default!;
    public PackageSymbol ContainingSymbol { get; } = default!;
    public NamespaceName ImplicitNamespaceName { get; } = default!;
    public INamespaceDefinitionNode ImplicitNamespace { get; } = default!;
    public NamespaceSymbol ImplicitNamespaceSymbol { get; } = default!;
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; } = default!;
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; } = default!;
    public NamespaceScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public DiagnosticCollection Diagnostics { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class UsingDirectiveNode // : IUsingDirectiveNode
{
    public IUsingDirectiveSyntax Syntax { get; } = default!;
    public NamespaceName Name { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class NamespaceBlockDefinitionNode // : INamespaceBlockDefinitionNode
{
    public INamespaceDefinitionSyntax Syntax { get; } = default!;
    public bool IsGlobalQualified { get; } = default!;
    public NamespaceName DeclaredNames { get; } = default!;
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; } = default!;
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; } = default!;
    public INamespaceDefinitionNode Definition { get; } = default!;
    public INamespaceDefinitionNode ContainingDeclaration { get; } = default!;
    public INamespaceDefinitionNode ContainingNamespace { get; } = default!;
    public NamespaceSymbol ContainingSymbol { get; } = default!;
    public NamespaceSymbol Symbol { get; } = default!;
    public NamespaceSearchScope ContainingLexicalScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public StandardName? Name { get; } = default!;
}

file class NamespaceDefinitionNode // : INamespaceDefinitionNode
{
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; } = default!;
    public IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; } = default!;
    public IFixedList<INamespaceMemberDefinitionNode> Members { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public NamespaceSymbol Symbol { get; } = default!;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; } = default!;
}

file class FunctionDefinitionNode // : IFunctionDefinitionNode
{
    public IFunctionDefinitionSyntax Syntax { get; } = default!;
    public INamespaceDeclarationNode ContainingDeclaration { get; } = default!;
    public NamespaceSymbol ContainingSymbol { get; } = default!;
    public IFixedList<IAttributeNode> Attributes { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode? Return { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public FunctionType Type { get; } = default!;
    public FunctionSymbol Symbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
}

file class ClassDefinitionNode // : IClassDefinitionNode
{
    public IClassDefinitionSyntax Syntax { get; } = default!;
    public IFixedList<IAttributeNode> Attributes { get; } = default!;
    public bool IsAbstract { get; } = default!;
    public IFixedList<IGenericParameterNode> GenericParameters { get; } = default!;
    public IStandardTypeNameNode? BaseTypeName { get; } = default!;
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; } = default!;
    public ObjectType DeclaredType { get; } = default!;
    public IFixedList<IClassMemberDefinitionNode> SourceMembers { get; } = default!;
    public IFixedSet<IClassMemberDefinitionNode> Members { get; } = default!;
    public IDefaultConstructorDefinitionNode? DefaultConstructor { get; } = default!;
    public bool IsConst { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public LexicalScope SupertypesLexicalScope { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public Symbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; } = default!;
}

file class StructDefinitionNode // : IStructDefinitionNode
{
    public IStructDefinitionSyntax Syntax { get; } = default!;
    public IFixedList<IAttributeNode> Attributes { get; } = default!;
    public IFixedList<IGenericParameterNode> GenericParameters { get; } = default!;
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; } = default!;
    public StructType DeclaredType { get; } = default!;
    public IFixedList<IStructMemberDefinitionNode> SourceMembers { get; } = default!;
    public IFixedSet<IStructMemberDefinitionNode> Members { get; } = default!;
    public IDefaultInitializerDefinitionNode? DefaultInitializer { get; } = default!;
    public bool IsConst { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public LexicalScope SupertypesLexicalScope { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public Symbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; } = default!;
}

file class TraitDefinitionNode // : ITraitDefinitionNode
{
    public ITraitDefinitionSyntax Syntax { get; } = default!;
    public IFixedList<IAttributeNode> Attributes { get; } = default!;
    public IFixedList<IGenericParameterNode> GenericParameters { get; } = default!;
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; } = default!;
    public ObjectType DeclaredType { get; } = default!;
    public IFixedSet<ITraitMemberDefinitionNode> Members { get; } = default!;
    public bool IsConst { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public LexicalScope SupertypesLexicalScope { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public Symbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; } = default!;
}

file class GenericParameterNode // : IGenericParameterNode
{
    public IGenericParameterSyntax Syntax { get; } = default!;
    public ICapabilityConstraintNode Constraint { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public TypeParameterIndependence Independence { get; } = default!;
    public TypeParameterVariance Variance { get; } = default!;
    public GenericParameter Parameter { get; } = default!;
    public IDeclaredUserType ContainingDeclaredType { get; } = default!;
    public GenericParameterType DeclaredType { get; } = default!;
    public IUserTypeDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public GenericParameterTypeSymbol Symbol { get; } = default!;
    public IFixedSet<ITypeMemberDefinitionNode> Members { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
}

file class AbstractMethodDefinitionNode // : IAbstractMethodDefinitionNode
{
    public IAbstractMethodDefinitionSyntax Syntax { get; } = default!;
    public IMethodSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode? Return { get; } = default!;
    public ObjectType ContainingDeclaredType { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public int Arity { get; } = default!;
    public FunctionType MethodGroupType { get; } = default!;
}

file class StandardMethodDefinitionNode // : IStandardMethodDefinitionNode
{
    public IStandardMethodDefinitionSyntax Syntax { get; } = default!;
    public IMethodSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode? Return { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public int Arity { get; } = default!;
    public FunctionType MethodGroupType { get; } = default!;
}

file class GetterMethodDefinitionNode // : IGetterMethodDefinitionNode
{
    public IGetterMethodDefinitionSyntax Syntax { get; } = default!;
    public IMethodSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode Return { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
}

file class SetterMethodDefinitionNode // : ISetterMethodDefinitionNode
{
    public ISetterMethodDefinitionSyntax Syntax { get; } = default!;
    public IMethodSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode? Return { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
}

file class DefaultConstructorDefinitionNode // : IDefaultConstructorDefinitionNode
{
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode? Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public IConstructorDefinitionSyntax? Syntax { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public ConstructorSymbol Symbol { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
}

file class SourceConstructorDefinitionNode // : ISourceConstructorDefinitionNode
{
    public IConstructorDefinitionSyntax Syntax { get; } = default!;
    public IConstructorSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBlockBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public ConstructorSymbol Symbol { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
}

file class DefaultInitializerDefinitionNode // : IDefaultInitializerDefinitionNode
{
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode? Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public IInitializerDefinitionSyntax? Syntax { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public InitializerSymbol Symbol { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
}

file class SourceInitializerDefinitionNode // : ISourceInitializerDefinitionNode
{
    public IInitializerDefinitionSyntax Syntax { get; } = default!;
    public IInitializerSelfParameterNode SelfParameter { get; } = default!;
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBlockBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public InitializerSymbol Symbol { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
}

file class FieldDefinitionNode // : IFieldDefinitionNode
{
    public IFieldDefinitionSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ITypeNode TypeNode { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public DataType BindingType { get; } = default!;
    public FieldSymbol Symbol { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IAmbiguousExpressionNode? Initializer { get; } = default!;
    public IAmbiguousExpressionNode? CurrentInitializer { get; } = default!;
    public IExpressionNode? IntermediateInitializer { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
}

file class AssociatedFunctionDefinitionNode // : IAssociatedFunctionDefinitionNode
{
    public IAssociatedFunctionDefinitionSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IFixedList<INamedParameterNode> Parameters { get; } = default!;
    public ITypeNode? Return { get; } = default!;
    public FunctionSymbol Symbol { get; } = default!;
    public FunctionType Type { get; } = default!;
    public IEntryNode Entry { get; } = default!;
    public IBodyNode Body { get; } = default!;
    public IExitNode Exit { get; } = default!;
    public ValueIdScope ValueIdScope { get; } = default!;
    public IPackageFacetNode Facet { get; } = default!;
    public ISymbolDeclarationNode ContainingDeclaration { get; } = default!;
    public UserTypeSymbol ContainingSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; } = default!;
    public AccessModifier AccessModifier { get; } = default!;
}

file class AttributeNode // : IAttributeNode
{
    public IAttributeSyntax Syntax { get; } = default!;
    public IStandardTypeNameNode TypeName { get; } = default!;
    public ConstructorSymbol? ReferencedSymbol { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class CapabilitySetNode // : ICapabilitySetNode
{
    public ICapabilitySetSyntax Syntax { get; } = default!;
    public CapabilitySet Constraint { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class CapabilityNode // : ICapabilityNode
{
    public ICapabilitySyntax Syntax { get; } = default!;
    public Capability Capability { get; } = default!;
    public ICapabilityConstraint Constraint { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class NamedParameterNode // : INamedParameterNode
{
    public INamedParameterSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ITypeNode TypeNode { get; } = default!;
    public DataType BindingType { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public ParameterType ParameterType { get; } = default!;
    public bool Unused { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
}

file class ConstructorSelfParameterNode // : IConstructorSelfParameterNode
{
    public IConstructorSelfParameterSyntax Syntax { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ICapabilityNode Capability { get; } = default!;
    public CapabilityType BindingType { get; } = default!;
    public ObjectType ContainingDeclaredType { get; } = default!;
    public ITypeDefinitionNode ContainingTypeDefinition { get; } = default!;
    public SelfParameterType ParameterType { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public bool Unused { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class InitializerSelfParameterNode // : IInitializerSelfParameterNode
{
    public IInitializerSelfParameterSyntax Syntax { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ICapabilityNode Capability { get; } = default!;
    public CapabilityType BindingType { get; } = default!;
    public StructType ContainingDeclaredType { get; } = default!;
    public ITypeDefinitionNode ContainingTypeDefinition { get; } = default!;
    public SelfParameterType ParameterType { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public bool Unused { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class MethodSelfParameterNode // : IMethodSelfParameterNode
{
    public IMethodSelfParameterSyntax Syntax { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ICapabilityConstraintNode Capability { get; } = default!;
    public ITypeDefinitionNode ContainingTypeDefinition { get; } = default!;
    public IDeclaredUserType ContainingDeclaredType { get; } = default!;
    public SelfParameterType ParameterType { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public Pseudotype BindingType { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public bool Unused { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FieldParameterNode // : IFieldParameterNode
{
    public IFieldParameterSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ITypeDefinitionNode ContainingTypeDefinition { get; } = default!;
    public IFieldDefinitionNode? ReferencedField { get; } = default!;
    public DataType BindingType { get; } = default!;
    public ParameterType ParameterType { get; } = default!;
    public bool Unused { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class BlockBodyNode // : IBlockBodyNode
{
    public IBlockBodySyntax Syntax { get; } = default!;
    public IFixedList<IBodyStatementNode> Statements { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class ExpressionBodyNode // : IExpressionBodyNode
{
    public IExpressionBodySyntax Syntax { get; } = default!;
    public IResultStatementNode ResultStatement { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public IFixedList<IStatementNode> Statements { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class IdentifierTypeNameNode // : IIdentifierTypeNameNode
{
    public IIdentifierTypeNameSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public bool IsAttributeType { get; } = default!;
    public ITypeDeclarationNode? ReferencedDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public TypeSymbol? ReferencedSymbol { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class SpecialTypeNameNode // : ISpecialTypeNameNode
{
    public ISpecialTypeNameSyntax Syntax { get; } = default!;
    public SpecialTypeName Name { get; } = default!;
    public TypeSymbol ReferencedSymbol { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class GenericTypeNameNode // : IGenericTypeNameNode
{
    public IGenericTypeNameSyntax Syntax { get; } = default!;
    public GenericName Name { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public bool IsAttributeType { get; } = default!;
    public ITypeDeclarationNode? ReferencedDeclaration { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public TypeSymbol? ReferencedSymbol { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class QualifiedTypeNameNode // : IQualifiedTypeNameNode
{
    public IQualifiedTypeNameSyntax Syntax { get; } = default!;
    public ITypeNameNode Context { get; } = default!;
    public IStandardTypeNameNode QualifiedName { get; } = default!;
    public TypeName Name { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public TypeSymbol? ReferencedSymbol { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class OptionalTypeNode // : IOptionalTypeNode
{
    public IOptionalTypeSyntax Syntax { get; } = default!;
    public ITypeNode Referent { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class CapabilityTypeNode // : ICapabilityTypeNode
{
    public ICapabilityTypeSyntax Syntax { get; } = default!;
    public ICapabilityNode Capability { get; } = default!;
    public ITypeNode Referent { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FunctionTypeNode // : IFunctionTypeNode
{
    public IFunctionTypeSyntax Syntax { get; } = default!;
    public IFixedList<IParameterTypeNode> Parameters { get; } = default!;
    public ITypeNode Return { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class ParameterTypeNode // : IParameterTypeNode
{
    public IParameterTypeSyntax Syntax { get; } = default!;
    public bool IsLent { get; } = default!;
    public ITypeNode Referent { get; } = default!;
    public ParameterType Parameter { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class CapabilityViewpointTypeNode // : ICapabilityViewpointTypeNode
{
    public ICapabilityViewpointTypeSyntax Syntax { get; } = default!;
    public ICapabilityNode Capability { get; } = default!;
    public ITypeNode Referent { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class SelfViewpointTypeNode // : ISelfViewpointTypeNode
{
    public ISelfViewpointTypeSyntax Syntax { get; } = default!;
    public ITypeNode Referent { get; } = default!;
    public Pseudotype? NamedSelfType { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public DataType NamedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class EntryNode // : IEntryNode
{
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public ICodeSyntax? Syntax { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class ExitNode // : IExitNode
{
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public ICodeSyntax? Syntax { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class ResultStatementNode // : IResultStatementNode
{
    public IResultStatementSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IAmbiguousExpressionNode CurrentExpression { get; } = default!;
    public IExpressionNode? IntermediateExpression { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public IMaybeAntetype? ResultAntetype { get; } = default!;
    public DataType? ResultType { get; } = default!;
    public ValueId? ResultValueId { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IMaybeAntetype Antetype { get; } = default!;
    public DataType Type { get; } = default!;
    public ValueId ValueId { get; } = default!;
}

file class VariableDeclarationStatementNode // : IVariableDeclarationStatementNode
{
    public IVariableDeclarationStatementSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ICapabilityNode? Capability { get; } = default!;
    public ITypeNode? Type { get; } = default!;
    public IAmbiguousExpressionNode? Initializer { get; } = default!;
    public IAmbiguousExpressionNode? CurrentInitializer { get; } = default!;
    public IExpressionNode? IntermediateInitializer { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public IMaybeAntetype? ResultAntetype { get; } = default!;
    public DataType? ResultType { get; } = default!;
    public ValueId? ResultValueId { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public DataType BindingType { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
}

file class ExpressionStatementNode // : IExpressionStatementNode
{
    public IExpressionStatementSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IAmbiguousExpressionNode CurrentExpression { get; } = default!;
    public IExpressionNode? IntermediateExpression { get; } = default!;
    public IMaybeAntetype? ResultAntetype { get; } = default!;
    public DataType? ResultType { get; } = default!;
    public ValueId? ResultValueId { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class BindingContextPatternNode // : IBindingContextPatternNode
{
    public IBindingContextPatternSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public IPatternNode Pattern { get; } = default!;
    public ITypeNode? Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class BindingPatternNode // : IBindingPatternNode
{
    public IBindingPatternSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public DataType BindingType { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
}

file class OptionalPatternNode // : IOptionalPatternNode
{
    public IOptionalPatternSyntax Syntax { get; } = default!;
    public IOptionalOrBindingPatternNode Pattern { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class BlockExpressionNode // : IBlockExpressionNode
{
    public IBlockExpressionSyntax Syntax { get; } = default!;
    public IFixedList<IStatementNode> Statements { get; } = default!;
    public IMaybeAntetype Antetype { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class NewObjectExpressionNode // : INewObjectExpressionNode
{
    public INewObjectExpressionSyntax Syntax { get; } = default!;
    public ITypeNameNode ConstructingType { get; } = default!;
    public IdentifierName? ConstructorName { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IExpressionNode?> IntermediateArguments { get; } = default!;
    public IMaybeAntetype ConstructingAntetype { get; } = default!;
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { get; } = default!;
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { get; } = default!;
    public IConstructorDeclarationNode? ReferencedConstructor { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnsafeExpressionNode // : IUnsafeExpressionNode
{
    public IUnsafeExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IExpressionNode? IntermediateExpression { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class BoolLiteralExpressionNode // : IBoolLiteralExpressionNode
{
    public IBoolLiteralExpressionSyntax Syntax { get; } = default!;
    public bool Value { get; } = default!;
    public BoolConstValueType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class IntegerLiteralExpressionNode // : IIntegerLiteralExpressionNode
{
    public IIntegerLiteralExpressionSyntax Syntax { get; } = default!;
    public BigInteger Value { get; } = default!;
    public IntegerConstValueType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class NoneLiteralExpressionNode // : INoneLiteralExpressionNode
{
    public INoneLiteralExpressionSyntax Syntax { get; } = default!;
    public OptionalType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class StringLiteralExpressionNode // : IStringLiteralExpressionNode
{
    public IStringLiteralExpressionSyntax Syntax { get; } = default!;
    public string Value { get; } = default!;
    public DataType Type { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AssignmentExpressionNode // : IAssignmentExpressionNode
{
    public IAssignmentExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousAssignableExpressionNode LeftOperand { get; } = default!;
    public IAmbiguousAssignableExpressionNode CurrentLeftOperand { get; } = default!;
    public IAssignableExpressionNode? IntermediateLeftOperand { get; } = default!;
    public AssignmentOperator Operator { get; } = default!;
    public IAmbiguousExpressionNode RightOperand { get; } = default!;
    public IAmbiguousExpressionNode CurrentRightOperand { get; } = default!;
    public IExpressionNode? IntermediateRightOperand { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
}

file class BinaryOperatorExpressionNode // : IBinaryOperatorExpressionNode
{
    public IBinaryOperatorExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode LeftOperand { get; } = default!;
    public IExpressionNode? IntermediateLeftOperand { get; } = default!;
    public BinaryOperator Operator { get; } = default!;
    public IAmbiguousExpressionNode RightOperand { get; } = default!;
    public IExpressionNode? IntermediateRightOperand { get; } = default!;
    public IAntetype? NumericOperatorCommonAntetype { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnaryOperatorExpressionNode // : IUnaryOperatorExpressionNode
{
    public IUnaryOperatorExpressionSyntax Syntax { get; } = default!;
    public UnaryOperatorFixity Fixity { get; } = default!;
    public UnaryOperator Operator { get; } = default!;
    public IAmbiguousExpressionNode Operand { get; } = default!;
    public IExpressionNode? IntermediateOperand { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class IdExpressionNode // : IIdExpressionNode
{
    public IIdExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Referent { get; } = default!;
    public IExpressionNode? IntermediateReferent { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class ConversionExpressionNode // : IConversionExpressionNode
{
    public IConversionExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Referent { get; } = default!;
    public IExpressionNode? IntermediateReferent { get; } = default!;
    public ConversionOperator Operator { get; } = default!;
    public ITypeNode ConvertToType { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class ImplicitConversionExpressionNode // : IImplicitConversionExpressionNode
{
    public IExpressionNode Referent { get; } = default!;
    public IExpressionNode CurrentReferent { get; } = default!;
    public SimpleAntetype Antetype { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class PatternMatchExpressionNode // : IPatternMatchExpressionNode
{
    public IPatternMatchExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Referent { get; } = default!;
    public IExpressionNode? IntermediateReferent { get; } = default!;
    public IPatternNode Pattern { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class IfExpressionNode // : IIfExpressionNode
{
    public IIfExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Condition { get; } = default!;
    public IExpressionNode? IntermediateCondition { get; } = default!;
    public IBlockOrResultNode ThenBlock { get; } = default!;
    public IElseClauseNode? ElseClause { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class LoopExpressionNode // : ILoopExpressionNode
{
    public ILoopExpressionSyntax Syntax { get; } = default!;
    public IBlockExpressionNode Block { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class WhileExpressionNode // : IWhileExpressionNode
{
    public IWhileExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Condition { get; } = default!;
    public IExpressionNode? IntermediateCondition { get; } = default!;
    public IBlockExpressionNode Block { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class ForeachExpressionNode // : IForeachExpressionNode
{
    public IForeachExpressionSyntax Syntax { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
    public IdentifierName VariableName { get; } = default!;
    public IAmbiguousExpressionNode InExpression { get; } = default!;
    public IExpressionNode? IntermediateInExpression { get; } = default!;
    public ITypeNode? DeclaredType { get; } = default!;
    public IBlockExpressionNode Block { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public LexicalScope LexicalScope { get; } = default!;
    public ITypeDeclarationNode? ReferencedIterableDeclaration { get; } = default!;
    public IStandardMethodDeclarationNode? ReferencedIterateMethod { get; } = default!;
    public IMaybeExpressionAntetype IteratorAntetype { get; } = default!;
    public DataType IteratorType { get; } = default!;
    public ITypeDeclarationNode? ReferencedIteratorDeclaration { get; } = default!;
    public IStandardMethodDeclarationNode? ReferencedNextMethod { get; } = default!;
    public IMaybeAntetype IteratedAntetype { get; } = default!;
    public DataType IteratedType { get; } = default!;
    public IFlowState FlowStateBeforeBlock { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
    public DataType BindingType { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public ValueId BindingValueId { get; } = default!;
    public IMaybeAntetype BindingAntetype { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; } = default!;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; } = default!;
}

file class BreakExpressionNode // : IBreakExpressionNode
{
    public IBreakExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode? Value { get; } = default!;
    public IExpressionNode? IntermediateValue { get; } = default!;
    public NeverType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class NextExpressionNode // : INextExpressionNode
{
    public INextExpressionSyntax Syntax { get; } = default!;
    public NeverType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class ReturnExpressionNode // : IReturnExpressionNode
{
    public IReturnExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode? Value { get; } = default!;
    public IAmbiguousExpressionNode? CurrentValue { get; } = default!;
    public IExpressionNode? IntermediateValue { get; } = default!;
    public NeverType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnresolvedInvocationExpressionNode // : IUnresolvedInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IAmbiguousExpressionNode CurrentExpression { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FunctionInvocationExpressionNode // : IFunctionInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IFunctionGroupNameNode FunctionGroup { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IExpressionNode?> IntermediateArguments { get; } = default!;
    public IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; } = default!;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class MethodInvocationExpressionNode // : IMethodInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IMethodGroupNameNode MethodGroup { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; } = default!;
    public IFixedList<IExpressionNode?> IntermediateArguments { get; } = default!;
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { get; } = default!;
    public IStandardMethodDeclarationNode? ReferencedDeclaration { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class GetterInvocationExpressionNode // : IGetterInvocationExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public StandardName PropertyName { get; } = default!;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; } = default!;
    public IGetterMethodDeclarationNode? ReferencedDeclaration { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class SetterInvocationExpressionNode // : ISetterInvocationExpressionNode
{
    public IAssignmentExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public StandardName PropertyName { get; } = default!;
    public IAmbiguousExpressionNode Value { get; } = default!;
    public IExpressionNode? IntermediateValue { get; } = default!;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; } = default!;
    public ISetterMethodDeclarationNode? ReferencedDeclaration { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class FunctionReferenceInvocationExpressionNode // : IFunctionReferenceInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Expression { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IExpressionNode?> IntermediateArguments { get; } = default!;
    public FunctionAntetype FunctionAntetype { get; } = default!;
    public FunctionType FunctionType { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class InitializerInvocationExpressionNode // : IInitializerInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IInitializerGroupNameNode InitializerGroup { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IFixedList<IExpressionNode?> IntermediateArguments { get; } = default!;
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; } = default!;
    public IInitializerDeclarationNode? ReferencedDeclaration { get; } = default!;
    public ContextualizedOverload? ContextualizedOverload { get; } = default!;
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; } = default!;
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnknownInvocationExpressionNode // : IUnknownInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class IdentifierNameExpressionNode // : IIdentifierNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public IFixedList<IDeclarationNode> ReferencedDeclarations { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class GenericNameExpressionNode // : IGenericNameExpressionNode
{
    public IGenericNameExpressionSyntax Syntax { get; } = default!;
    public GenericName Name { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public LexicalScope ContainingLexicalScope { get; } = default!;
    public IFixedList<IDeclarationNode> ReferencedDeclarations { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class MemberAccessExpressionNode // : IMemberAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Context { get; } = default!;
    public StandardName MemberName { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class PropertyNameNode // : IPropertyNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public StandardName PropertyName { get; } = default!;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class UnqualifiedNamespaceNameNode // : IUnqualifiedNamespaceNameNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class QualifiedNamespaceNameNode // : IQualifiedNamespaceNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public INamespaceNameNode Context { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class FunctionGroupNameNode // : IFunctionGroupNameNode
{
    public INameExpressionNode? Context { get; } = default!;
    public StandardName FunctionName { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; } = default!;
    public INameExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class FunctionNameNode // : IFunctionNameNode
{
    public IFunctionGroupNameNode FunctionGroup { get; } = default!;
    public StandardName FunctionName { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; } = default!;
    public INameExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class MethodGroupNameNode // : IMethodGroupNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public IExpressionNode CurrentContext { get; } = default!;
    public StandardName MethodName { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class FieldAccessExpressionNode // : IFieldAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public IdentifierName FieldName { get; } = default!;
    public IFieldDeclarationNode ReferencedDeclaration { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class VariableNameExpressionNode // : IVariableNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ILocalBindingNode ReferencedDefinition { get; } = default!;
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class StandardTypeNameExpressionNode // : IStandardTypeNameExpressionNode
{
    public IStandardNameExpressionSyntax Syntax { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public StandardName Name { get; } = default!;
    public ITypeDeclarationNode ReferencedDeclaration { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class QualifiedTypeNameExpressionNode // : IQualifiedTypeNameExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public INamespaceNameNode Context { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public StandardName Name { get; } = default!;
    public ITypeDeclarationNode ReferencedDeclaration { get; } = default!;
    public IMaybeAntetype NamedAntetype { get; } = default!;
    public BareType? NamedBareType { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class InitializerGroupNameNode // : IInitializerGroupNameNode
{
    public INameExpressionSyntax Syntax { get; } = default!;
    public ITypeNameExpressionNode Context { get; } = default!;
    public StandardName? InitializerName { get; } = default!;
    public IMaybeAntetype InitializingAntetype { get; } = default!;
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class SpecialTypeNameExpressionNode // : ISpecialTypeNameExpressionNode
{
    public ISpecialTypeNameExpressionSyntax Syntax { get; } = default!;
    public SpecialTypeName Name { get; } = default!;
    public TypeSymbol ReferencedSymbol { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class SelfExpressionNode // : ISelfExpressionNode
{
    public ISelfExpressionSyntax Syntax { get; } = default!;
    public bool IsImplicit { get; } = default!;
    public Pseudotype Pseudotype { get; } = default!;
    public IExecutableDefinitionNode ContainingDeclaration { get; } = default!;
    public ISelfParameterNode? ReferencedDefinition { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class MissingNameExpressionNode // : IMissingNameExpressionNode
{
    public IMissingNameSyntax Syntax { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnknownIdentifierNameExpressionNode // : IUnknownIdentifierNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnknownGenericNameExpressionNode // : IUnknownGenericNameExpressionNode
{
    public IGenericNameExpressionSyntax Syntax { get; } = default!;
    public GenericName Name { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class UnknownMemberAccessExpressionNode // : IUnknownMemberAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; } = default!;
    public IExpressionNode Context { get; } = default!;
    public StandardName MemberName { get; } = default!;
    public IFixedList<ITypeNode> TypeArguments { get; } = default!;
    public IFixedSet<IDeclarationNode> ReferencedMembers { get; } = default!;
    public UnknownType Type { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AmbiguousMoveExpressionNode // : IAmbiguousMoveExpressionNode
{
    public IMoveExpressionSyntax Syntax { get; } = default!;
    public ISimpleNameNode Referent { get; } = default!;
    public INameExpressionNode? IntermediateReferent { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class MoveVariableExpressionNode // : IMoveVariableExpressionNode
{
    public ILocalBindingNameExpressionNode Referent { get; } = default!;
    public bool IsImplicit { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class MoveValueExpressionNode // : IMoveValueExpressionNode
{
    public IExpressionNode Referent { get; } = default!;
    public bool IsImplicit { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class ImplicitTempMoveExpressionNode // : IImplicitTempMoveExpressionNode
{
    public IExpressionNode Referent { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AmbiguousFreezeExpressionNode // : IAmbiguousFreezeExpressionNode
{
    public IFreezeExpressionSyntax Syntax { get; } = default!;
    public ISimpleNameNode Referent { get; } = default!;
    public INameExpressionNode? IntermediateReferent { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FreezeVariableExpressionNode // : IFreezeVariableExpressionNode
{
    public ILocalBindingNameExpressionNode Referent { get; } = default!;
    public bool IsTemporary { get; } = default!;
    public bool IsImplicit { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class FreezeValueExpressionNode // : IFreezeValueExpressionNode
{
    public IExpressionNode Referent { get; } = default!;
    public bool IsTemporary { get; } = default!;
    public bool IsImplicit { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class PrepareToReturnExpressionNode // : IPrepareToReturnExpressionNode
{
    public IExpressionNode Value { get; } = default!;
    public IExpressionNode CurrentValue { get; } = default!;
    public IExpressionSyntax Syntax { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AsyncBlockExpressionNode // : IAsyncBlockExpressionNode
{
    public IAsyncBlockExpressionSyntax Syntax { get; } = default!;
    public IBlockExpressionNode Block { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AsyncStartExpressionNode // : IAsyncStartExpressionNode
{
    public IAsyncStartExpressionSyntax Syntax { get; } = default!;
    public bool Scheduled { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IExpressionNode? IntermediateExpression { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class AwaitExpressionNode // : IAwaitExpressionNode
{
    public IAwaitExpressionSyntax Syntax { get; } = default!;
    public IAmbiguousExpressionNode Expression { get; } = default!;
    public IExpressionNode? IntermediateExpression { get; } = default!;
    public IMaybeExpressionAntetype? ExpectedAntetype { get; } = default!;
    public IMaybeExpressionAntetype Antetype { get; } = default!;
    public DataType? ExpectedType { get; } = default!;
    public DataType Type { get; } = default!;
    public IFlowState FlowStateAfter { get; } = default!;
    public ValueId ValueId { get; } = default!;
    public CodeFile File { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public ControlFlowSet ControlFlowNext { get; } = default!;
    public ControlFlowSet ControlFlowPrevious { get; } = default!;
}

file class PackageSymbolNode // : IPackageSymbolNode
{
    public IPackageFacetDeclarationNode MainFacet { get; } = default!;
    public IPackageFacetDeclarationNode TestingFacet { get; } = default!;
    public IdentifierName? AliasOrName { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public PackageSymbol Symbol { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
}

file class PackageFacetSymbolNode // : IPackageFacetSymbolNode
{
    public INamespaceDeclarationNode GlobalNamespace { get; } = default!;
    public IdentifierName? PackageAliasOrName { get; } = default!;
    public IdentifierName PackageName { get; } = default!;
    public PackageSymbol Symbol { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class NamespaceSymbolNode // : INamespaceSymbolNode
{
    public IFixedList<INamespaceMemberDeclarationNode> Members { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public NamespaceSymbol Symbol { get; } = default!;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FunctionSymbolNode // : IFunctionSymbolNode
{
    public StandardName Name { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FunctionSymbol Symbol { get; } = default!;
    public FunctionType Type { get; } = default!;
}

file class PrimitiveTypeSymbolNode // : IPrimitiveTypeSymbolNode
{
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; } = default!;
    public SpecialTypeName Name { get; } = default!;
    public TypeSymbol Symbol { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class UserTypeSymbolNode // : IUserTypeSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; } = default!;
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
}

file class ClassSymbolNode // : IClassSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; } = default!;
    public IFixedSet<IClassMemberDeclarationNode> Members { get; } = default!;
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
}

file class StructSymbolNode // : IStructSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; } = default!;
    public IFixedSet<IStructMemberDeclarationNode> Members { get; } = default!;
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
}

file class TraitSymbolNode // : ITraitSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; } = default!;
    public IFixedSet<ITraitMemberDeclarationNode> Members { get; } = default!;
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public StandardName Name { get; } = default!;
    public UserTypeSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
}

file class GenericParameterSymbolNode // : IGenericParameterSymbolNode
{
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public GenericParameterTypeSymbol Symbol { get; } = default!;
    public IFixedSet<BareReferenceType> Supertypes { get; } = default!;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
}

file class StandardMethodSymbolNode // : IStandardMethodSymbolNode
{
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public int Arity { get; } = default!;
    public FunctionType MethodGroupType { get; } = default!;
}

file class GetterMethodSymbolNode // : IGetterMethodSymbolNode
{
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class SetterMethodSymbolNode // : ISetterMethodSymbolNode
{
    public IdentifierName Name { get; } = default!;
    public MethodSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class ConstructorSymbolNode // : IConstructorSymbolNode
{
    public IdentifierName? Name { get; } = default!;
    public ConstructorSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class InitializerSymbolNode // : IInitializerSymbolNode
{
    public IdentifierName? Name { get; } = default!;
    public InitializerSymbol Symbol { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
}

file class FieldSymbolNode // : IFieldSymbolNode
{
    public IdentifierName Name { get; } = default!;
    public DataType BindingType { get; } = default!;
    public FieldSymbol Symbol { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
}

file class AssociatedFunctionSymbolNode // : IAssociatedFunctionSymbolNode
{
    public StandardName Name { get; } = default!;
    public IPackageFacetDeclarationNode Facet { get; } = default!;
    public ISyntax? Syntax { get; } = default!;
    public ISemanticNode Parent { get; } = default!;
    public IPackageDeclarationNode Package { get; } = default!;
    public FunctionSymbol Symbol { get; } = default!;
    public FunctionType Type { get; } = default!;
}

