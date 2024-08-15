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
    public IPackageSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public PackageSymbol Symbol { get; }
    public IFixedSet<IPackageReferenceNode> References { get; }
    public IPackageReferenceNode IntrinsicsReference { get; }
    public FixedDictionary<IdentifierName,IPackageDeclarationNode> PackageDeclarations { get; }
    public IPackageFacetNode MainFacet { get; }
    public IPackageFacetNode TestingFacet { get; }
    public DiagnosticCollection Diagnostics { get; }
    public IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; }
    public IFunctionDefinitionNode? EntryPoint { get; }
    public IPackageSymbols PackageSymbols { get; }
    public IdentifierName? AliasOrName { get; }

    public PackageNode(IPackageSyntax syntax, IdentifierName name, PackageSymbol symbol, IFixedSet<IPackageReferenceNode> references, IPackageReferenceNode intrinsicsReference, FixedDictionary<IdentifierName,IPackageDeclarationNode> packageDeclarations, IPackageFacetNode mainFacet, IPackageFacetNode testingFacet, DiagnosticCollection diagnostics, IFixedSet<ITypeDeclarationNode> primitivesDeclarations, IFunctionDefinitionNode? entryPoint, IPackageSymbols packageSymbols, IdentifierName? aliasOrName)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
        References = references;
        IntrinsicsReference = intrinsicsReference;
        PackageDeclarations = packageDeclarations;
        MainFacet = mainFacet;
        TestingFacet = testingFacet;
        Diagnostics = diagnostics;
        PrimitivesDeclarations = primitivesDeclarations;
        EntryPoint = entryPoint;
        PackageSymbols = packageSymbols;
        AliasOrName = aliasOrName;
    }
}

file class PackageReferenceNode // : IPackageReferenceNode
{
    public IPackageReferenceSyntax? Syntax { get; }
    public IPackageSymbolNode SymbolNode { get; }
    public IdentifierName AliasOrName { get; }
    public IPackageSymbols PackageSymbols { get; }
    public bool IsTrusted { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public PackageReferenceNode(IPackageReferenceSyntax? syntax, IPackageSymbolNode symbolNode, IdentifierName aliasOrName, IPackageSymbols packageSymbols, bool isTrusted, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        SymbolNode = symbolNode;
        AliasOrName = aliasOrName;
        PackageSymbols = packageSymbols;
        IsTrusted = isTrusted;
        Parent = parent;
        Package = package;
    }
}

file class PackageFacetNode // : IPackageFacetNode
{
    public IPackageSyntax Syntax { get; }
    public IdentifierName PackageName { get; }
    public PackageSymbol PackageSymbol { get; }
    public PackageNameScope PackageNameScope { get; }
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    public IFixedSet<IPackageMemberDefinitionNode> Definitions { get; }
    public INamespaceDefinitionNode GlobalNamespace { get; }
    public IdentifierName? PackageAliasOrName { get; }
    public PackageSymbol Symbol { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public PackageFacetNode(IPackageSyntax syntax, IdentifierName packageName, PackageSymbol packageSymbol, PackageNameScope packageNameScope, IFixedSet<ICompilationUnitNode> compilationUnits, IFixedSet<IPackageMemberDefinitionNode> definitions, INamespaceDefinitionNode globalNamespace, IdentifierName? packageAliasOrName, PackageSymbol symbol, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        PackageName = packageName;
        PackageSymbol = packageSymbol;
        PackageNameScope = packageNameScope;
        CompilationUnits = compilationUnits;
        Definitions = definitions;
        GlobalNamespace = globalNamespace;
        PackageAliasOrName = packageAliasOrName;
        Symbol = symbol;
        Parent = parent;
        Package = package;
    }
}

file class CompilationUnitNode // : ICompilationUnitNode
{
    public ICompilationUnitSyntax Syntax { get; }
    public IPackageFacetNode ContainingDeclaration { get; }
    public PackageSymbol ContainingSymbol { get; }
    public NamespaceName ImplicitNamespaceName { get; }
    public INamespaceDefinitionNode ImplicitNamespace { get; }
    public NamespaceSymbol ImplicitNamespaceSymbol { get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    public NamespaceScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public DiagnosticCollection Diagnostics { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public CompilationUnitNode(ICompilationUnitSyntax syntax, IPackageFacetNode containingDeclaration, PackageSymbol containingSymbol, NamespaceName implicitNamespaceName, INamespaceDefinitionNode implicitNamespace, NamespaceSymbol implicitNamespaceSymbol, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> definitions, NamespaceScope containingLexicalScope, LexicalScope lexicalScope, DiagnosticCollection diagnostics, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ImplicitNamespaceName = implicitNamespaceName;
        ImplicitNamespace = implicitNamespace;
        ImplicitNamespaceSymbol = implicitNamespaceSymbol;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        Diagnostics = diagnostics;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class UsingDirectiveNode // : IUsingDirectiveNode
{
    public IUsingDirectiveSyntax Syntax { get; }
    public NamespaceName Name { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public UsingDirectiveNode(IUsingDirectiveSyntax syntax, NamespaceName name, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class NamespaceBlockDefinitionNode // : INamespaceBlockDefinitionNode
{
    public INamespaceDefinitionSyntax Syntax { get; }
    public bool IsGlobalQualified { get; }
    public NamespaceName DeclaredNames { get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    public INamespaceDefinitionNode Definition { get; }
    public INamespaceDefinitionNode ContainingDeclaration { get; }
    public INamespaceDefinitionNode ContainingNamespace { get; }
    public NamespaceSymbol ContainingSymbol { get; }
    public NamespaceSymbol Symbol { get; }
    public NamespaceSearchScope ContainingLexicalScope { get; }
    public IPackageFacetNode Facet { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public StandardName? Name { get; }

    public NamespaceBlockDefinitionNode(INamespaceDefinitionSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> members, INamespaceDefinitionNode definition, INamespaceDefinitionNode containingDeclaration, INamespaceDefinitionNode containingNamespace, NamespaceSymbol containingSymbol, NamespaceSymbol symbol, NamespaceSearchScope containingLexicalScope, IPackageFacetNode facet, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, StandardName? name)
    {
        Syntax = syntax;
        IsGlobalQualified = isGlobalQualified;
        DeclaredNames = declaredNames;
        UsingDirectives = usingDirectives;
        Members = members;
        Definition = definition;
        ContainingDeclaration = containingDeclaration;
        ContainingNamespace = containingNamespace;
        ContainingSymbol = containingSymbol;
        Symbol = symbol;
        ContainingLexicalScope = containingLexicalScope;
        Facet = facet;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        Name = name;
    }
}

file class NamespaceDefinitionNode // : INamespaceDefinitionNode
{
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; }
    public IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; }
    public IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    public IdentifierName Name { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }

    public NamespaceDefinitionNode(IFixedList<INamespaceDefinitionNode> memberNamespaces, IFixedList<IPackageMemberDefinitionNode> packageMembers, IFixedList<INamespaceMemberDefinitionNode> members, IdentifierName name, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, NamespaceSymbol symbol, IFixedList<INamespaceMemberDeclarationNode> nestedMembers)
    {
        MemberNamespaces = memberNamespaces;
        PackageMembers = packageMembers;
        Members = members;
        Name = name;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Symbol = symbol;
        NestedMembers = nestedMembers;
    }
}

file class FunctionDefinitionNode // : IFunctionDefinitionNode
{
    public IFunctionDefinitionSyntax Syntax { get; }
    public INamespaceDeclarationNode ContainingDeclaration { get; }
    public NamespaceSymbol ContainingSymbol { get; }
    public IFixedList<IAttributeNode> Attributes { get; }
    public IdentifierName Name { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public IEntryNode Entry { get; }
    public IBodyNode Body { get; }
    public IExitNode Exit { get; }
    public FunctionType Type { get; }
    public FunctionSymbol Symbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ValueIdScope ValueIdScope { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }

    public FunctionDefinitionNode(IFunctionDefinitionSyntax syntax, INamespaceDeclarationNode containingDeclaration, NamespaceSymbol containingSymbol, IFixedList<IAttributeNode> attributes, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit, FunctionType type, FunctionSymbol symbol, AccessModifier accessModifier, IPackageFacetNode facet, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ValueIdScope valueIdScope, FixedDictionary<IVariableBindingNode,int> variableBindingsMap)
    {
        Syntax = syntax;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        Attributes = attributes;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
        Type = type;
        Symbol = symbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        ValueIdScope = valueIdScope;
        VariableBindingsMap = variableBindingsMap;
    }
}

file class ClassDefinitionNode // : IClassDefinitionNode
{
    public IClassDefinitionSyntax Syntax { get; }
    public IFixedList<IAttributeNode> Attributes { get; }
    public bool IsAbstract { get; }
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    public IStandardTypeNameNode? BaseTypeName { get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    public ObjectType DeclaredType { get; }
    public IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    public IFixedSet<IClassMemberDefinitionNode> Members { get; }
    public IDefaultConstructorDefinitionNode? DefaultConstructor { get; }
    public bool IsConst { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public LexicalScope SupertypesLexicalScope { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public Symbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; }

    public ClassDefinitionNode(IClassDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, bool isAbstract, IFixedList<IGenericParameterNode> genericParameters, IStandardTypeNameNode? baseTypeName, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedList<IClassMemberDefinitionNode> sourceMembers, IFixedSet<IClassMemberDefinitionNode> members, IDefaultConstructorDefinitionNode? defaultConstructor, bool isConst, StandardName name, UserTypeSymbol symbol, LexicalScope supertypesLexicalScope, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, Symbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers)
    {
        Syntax = syntax;
        Attributes = attributes;
        IsAbstract = isAbstract;
        GenericParameters = genericParameters;
        BaseTypeName = baseTypeName;
        SupertypeNames = supertypeNames;
        DeclaredType = declaredType;
        SourceMembers = sourceMembers;
        Members = members;
        DefaultConstructor = defaultConstructor;
        IsConst = isConst;
        Name = name;
        Symbol = symbol;
        SupertypesLexicalScope = supertypesLexicalScope;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        InclusiveMembers = inclusiveMembers;
    }
}

file class StructDefinitionNode // : IStructDefinitionNode
{
    public IStructDefinitionSyntax Syntax { get; }
    public IFixedList<IAttributeNode> Attributes { get; }
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    public StructType DeclaredType { get; }
    public IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    public IFixedSet<IStructMemberDefinitionNode> Members { get; }
    public IDefaultInitializerDefinitionNode? DefaultInitializer { get; }
    public bool IsConst { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public LexicalScope SupertypesLexicalScope { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public Symbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; }

    public StructDefinitionNode(IStructDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, StructType declaredType, IFixedList<IStructMemberDefinitionNode> sourceMembers, IFixedSet<IStructMemberDefinitionNode> members, IDefaultInitializerDefinitionNode? defaultInitializer, bool isConst, StandardName name, UserTypeSymbol symbol, LexicalScope supertypesLexicalScope, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, Symbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers)
    {
        Syntax = syntax;
        Attributes = attributes;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        DeclaredType = declaredType;
        SourceMembers = sourceMembers;
        Members = members;
        DefaultInitializer = defaultInitializer;
        IsConst = isConst;
        Name = name;
        Symbol = symbol;
        SupertypesLexicalScope = supertypesLexicalScope;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        InclusiveMembers = inclusiveMembers;
    }
}

file class TraitDefinitionNode // : ITraitDefinitionNode
{
    public ITraitDefinitionSyntax Syntax { get; }
    public IFixedList<IAttributeNode> Attributes { get; }
    public IFixedList<IGenericParameterNode> GenericParameters { get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    public ObjectType DeclaredType { get; }
    public IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    public bool IsConst { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public LexicalScope SupertypesLexicalScope { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public Symbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; }

    public TraitDefinitionNode(ITraitDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedSet<ITraitMemberDefinitionNode> members, bool isConst, StandardName name, UserTypeSymbol symbol, LexicalScope supertypesLexicalScope, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, Symbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers)
    {
        Syntax = syntax;
        Attributes = attributes;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        DeclaredType = declaredType;
        Members = members;
        IsConst = isConst;
        Name = name;
        Symbol = symbol;
        SupertypesLexicalScope = supertypesLexicalScope;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        InclusiveMembers = inclusiveMembers;
    }
}

file class GenericParameterNode // : IGenericParameterNode
{
    public IGenericParameterSyntax Syntax { get; }
    public ICapabilityConstraintNode Constraint { get; }
    public IdentifierName Name { get; }
    public TypeParameterIndependence Independence { get; }
    public TypeParameterVariance Variance { get; }
    public GenericParameter Parameter { get; }
    public IDeclaredUserType ContainingDeclaredType { get; }
    public GenericParameterType DeclaredType { get; }
    public IUserTypeDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public GenericParameterTypeSymbol Symbol { get; }
    public IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    public IPackageFacetDeclarationNode Facet { get; }

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, GenericParameter parameter, IDeclaredUserType containingDeclaredType, GenericParameterType declaredType, IUserTypeDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, GenericParameterTypeSymbol symbol, IFixedSet<ITypeMemberDefinitionNode> members, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IPackageFacetDeclarationNode facet)
    {
        Syntax = syntax;
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
        Parameter = parameter;
        ContainingDeclaredType = containingDeclaredType;
        DeclaredType = declaredType;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        Symbol = symbol;
        Members = members;
        File = file;
        Parent = parent;
        Package = package;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Facet = facet;
    }
}

file class AbstractMethodDefinitionNode // : IAbstractMethodDefinitionNode
{
    public IAbstractMethodDefinitionSyntax Syntax { get; }
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public ObjectType ContainingDeclaredType { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ValueIdScope ValueIdScope { get; }
    public int Arity { get; }
    public FunctionType MethodGroupType { get; }

    public AbstractMethodDefinitionNode(IAbstractMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, ObjectType containingDeclaredType, MethodKind kind, IdentifierName name, MethodSymbol symbol, UserTypeSymbol containingSymbol, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ValueIdScope valueIdScope, int arity, FunctionType methodGroupType)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        ContainingDeclaredType = containingDeclaredType;
        Kind = kind;
        Name = name;
        Symbol = symbol;
        ContainingSymbol = containingSymbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        ValueIdScope = valueIdScope;
        Arity = arity;
        MethodGroupType = methodGroupType;
    }
}

file class StandardMethodDefinitionNode // : IStandardMethodDefinitionNode
{
    public IStandardMethodDefinitionSyntax Syntax { get; }
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public IEntryNode Entry { get; }
    public IBodyNode Body { get; }
    public IExitNode Exit { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ValueIdScope ValueIdScope { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public int Arity { get; }
    public FunctionType MethodGroupType { get; }

    public StandardMethodDefinitionNode(IStandardMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit, MethodKind kind, IdentifierName name, MethodSymbol symbol, UserTypeSymbol containingSymbol, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ValueIdScope valueIdScope, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, int arity, FunctionType methodGroupType)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
        Kind = kind;
        Name = name;
        Symbol = symbol;
        ContainingSymbol = containingSymbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        ValueIdScope = valueIdScope;
        VariableBindingsMap = variableBindingsMap;
        Arity = arity;
        MethodGroupType = methodGroupType;
    }
}

file class GetterMethodDefinitionNode // : IGetterMethodDefinitionNode
{
    public IGetterMethodDefinitionSyntax Syntax { get; }
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode Return { get; }
    public IEntryNode Entry { get; }
    public IBodyNode Body { get; }
    public IExitNode Exit { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ValueIdScope ValueIdScope { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }

    public GetterMethodDefinitionNode(IGetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode @return, IEntryNode entry, IBodyNode body, IExitNode exit, MethodKind kind, IdentifierName name, MethodSymbol symbol, UserTypeSymbol containingSymbol, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ValueIdScope valueIdScope, FixedDictionary<IVariableBindingNode,int> variableBindingsMap)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
        Kind = kind;
        Name = name;
        Symbol = symbol;
        ContainingSymbol = containingSymbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        ValueIdScope = valueIdScope;
        VariableBindingsMap = variableBindingsMap;
    }
}

file class SetterMethodDefinitionNode // : ISetterMethodDefinitionNode
{
    public ISetterMethodDefinitionSyntax Syntax { get; }
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public IEntryNode Entry { get; }
    public IBodyNode Body { get; }
    public IExitNode Exit { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ValueIdScope ValueIdScope { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }

    public SetterMethodDefinitionNode(ISetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit, MethodKind kind, IdentifierName name, MethodSymbol symbol, UserTypeSymbol containingSymbol, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ValueIdScope valueIdScope, FixedDictionary<IVariableBindingNode,int> variableBindingsMap)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
        Kind = kind;
        Name = name;
        Symbol = symbol;
        ContainingSymbol = containingSymbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        ValueIdScope = valueIdScope;
        VariableBindingsMap = variableBindingsMap;
    }
}

file class DefaultConstructorDefinitionNode // : IDefaultConstructorDefinitionNode
{
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IEntryNode Entry { get; }
    public IBodyNode? Body { get; }
    public IExitNode Exit { get; }
    public IConstructorDefinitionSyntax? Syntax { get; }
    public IdentifierName? Name { get; }
    public ConstructorSymbol Symbol { get; }
    public ValueIdScope ValueIdScope { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public AccessModifier AccessModifier { get; }

    public DefaultConstructorDefinitionNode(IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit, IConstructorDefinitionSyntax? syntax, IdentifierName? name, ConstructorSymbol symbol, ValueIdScope valueIdScope, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier)
    {
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
        ValueIdScope = valueIdScope;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
    }
}

file class SourceConstructorDefinitionNode // : ISourceConstructorDefinitionNode
{
    public IConstructorDefinitionSyntax Syntax { get; }
    public IConstructorSelfParameterNode SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IEntryNode Entry { get; }
    public IBlockBodyNode Body { get; }
    public IExitNode Exit { get; }
    public IdentifierName? Name { get; }
    public ConstructorSymbol Symbol { get; }
    public ValueIdScope ValueIdScope { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public AccessModifier AccessModifier { get; }

    public SourceConstructorDefinitionNode(IConstructorDefinitionSyntax syntax, IConstructorSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit, IdentifierName? name, ConstructorSymbol symbol, ValueIdScope valueIdScope, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
        Name = name;
        Symbol = symbol;
        ValueIdScope = valueIdScope;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
    }
}

file class DefaultInitializerDefinitionNode // : IDefaultInitializerDefinitionNode
{
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IEntryNode Entry { get; }
    public IBodyNode? Body { get; }
    public IExitNode Exit { get; }
    public IInitializerDefinitionSyntax? Syntax { get; }
    public IdentifierName? Name { get; }
    public InitializerSymbol Symbol { get; }
    public ValueIdScope ValueIdScope { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public AccessModifier AccessModifier { get; }

    public DefaultInitializerDefinitionNode(IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit, IInitializerDefinitionSyntax? syntax, IdentifierName? name, InitializerSymbol symbol, ValueIdScope valueIdScope, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier)
    {
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
        ValueIdScope = valueIdScope;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
    }
}

file class SourceInitializerDefinitionNode // : ISourceInitializerDefinitionNode
{
    public IInitializerDefinitionSyntax Syntax { get; }
    public IInitializerSelfParameterNode SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    public IEntryNode Entry { get; }
    public IBlockBodyNode Body { get; }
    public IExitNode Exit { get; }
    public IdentifierName? Name { get; }
    public InitializerSymbol Symbol { get; }
    public ValueIdScope ValueIdScope { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public AccessModifier AccessModifier { get; }

    public SourceInitializerDefinitionNode(IInitializerDefinitionSyntax syntax, IInitializerSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit, IdentifierName? name, InitializerSymbol symbol, ValueIdScope valueIdScope, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier)
    {
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
        Name = name;
        Symbol = symbol;
        ValueIdScope = valueIdScope;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
    }
}

file class FieldDefinitionNode // : IFieldDefinitionNode
{
    public IFieldDefinitionSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public IdentifierName Name { get; }
    public ITypeNode TypeNode { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public DataType BindingType { get; }
    public FieldSymbol Symbol { get; }
    public IEntryNode Entry { get; }
    public IAmbiguousExpressionNode? Initializer { get; }
    public IAmbiguousExpressionNode? CurrentInitializer { get; }
    public IExpressionNode? IntermediateInitializer { get; }
    public IExitNode Exit { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public AccessModifier AccessModifier { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public bool IsLentBinding { get; }
    public ValueId BindingValueId { get; }
    public ValueIdScope ValueIdScope { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }

    public FieldDefinitionNode(IFieldDefinitionSyntax syntax, bool isMutableBinding, IdentifierName name, ITypeNode typeNode, IMaybeAntetype bindingAntetype, DataType bindingType, FieldSymbol symbol, IEntryNode entry, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer, IExpressionNode? intermediateInitializer, IExitNode exit, LexicalScope containingLexicalScope, UserTypeSymbol containingSymbol, AccessModifier accessModifier, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, bool isLentBinding, ValueId bindingValueId, ValueIdScope valueIdScope, FixedDictionary<IVariableBindingNode,int> variableBindingsMap)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        TypeNode = typeNode;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        Symbol = symbol;
        Entry = entry;
        Initializer = initializer;
        CurrentInitializer = currentInitializer;
        IntermediateInitializer = intermediateInitializer;
        Exit = exit;
        ContainingLexicalScope = containingLexicalScope;
        ContainingSymbol = containingSymbol;
        AccessModifier = accessModifier;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        IsLentBinding = isLentBinding;
        BindingValueId = bindingValueId;
        ValueIdScope = valueIdScope;
        VariableBindingsMap = variableBindingsMap;
    }
}

file class AssociatedFunctionDefinitionNode // : IAssociatedFunctionDefinitionNode
{
    public IAssociatedFunctionDefinitionSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public ITypeNode? Return { get; }
    public FunctionSymbol Symbol { get; }
    public FunctionType Type { get; }
    public IEntryNode Entry { get; }
    public IBodyNode Body { get; }
    public IExitNode Exit { get; }
    public ValueIdScope ValueIdScope { get; }
    public IPackageFacetNode Facet { get; }
    public ISymbolDeclarationNode ContainingDeclaration { get; }
    public UserTypeSymbol ContainingSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    public AccessModifier AccessModifier { get; }

    public AssociatedFunctionDefinitionNode(IAssociatedFunctionDefinitionSyntax syntax, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, FunctionSymbol symbol, FunctionType type, IEntryNode entry, IBodyNode body, IExitNode exit, ValueIdScope valueIdScope, IPackageFacetNode facet, ISymbolDeclarationNode containingDeclaration, UserTypeSymbol containingSymbol, LexicalScope containingLexicalScope, LexicalScope lexicalScope, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier)
    {
        Syntax = syntax;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Symbol = symbol;
        Type = type;
        Entry = entry;
        Body = body;
        Exit = exit;
        ValueIdScope = valueIdScope;
        Facet = facet;
        ContainingDeclaration = containingDeclaration;
        ContainingSymbol = containingSymbol;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        File = file;
        Parent = parent;
        Package = package;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
    }
}

file class AttributeNode // : IAttributeNode
{
    public IAttributeSyntax Syntax { get; }
    public IStandardTypeNameNode TypeName { get; }
    public ConstructorSymbol? ReferencedSymbol { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public AttributeNode(IAttributeSyntax syntax, IStandardTypeNameNode typeName, ConstructorSymbol? referencedSymbol, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        TypeName = typeName;
        ReferencedSymbol = referencedSymbol;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class CapabilitySetNode // : ICapabilitySetNode
{
    public ICapabilitySetSyntax Syntax { get; }
    public CapabilitySet Constraint { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public CapabilitySetNode(ICapabilitySetSyntax syntax, CapabilitySet constraint, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Constraint = constraint;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class CapabilityNode // : ICapabilityNode
{
    public ICapabilitySyntax Syntax { get; }
    public Capability Capability { get; }
    public ICapabilityConstraint Constraint { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public CapabilityNode(ICapabilitySyntax syntax, Capability capability, ICapabilityConstraint constraint, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Capability = capability;
        Constraint = constraint;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class NamedParameterNode // : INamedParameterNode
{
    public INamedParameterSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public bool IsLentBinding { get; }
    public IdentifierName Name { get; }
    public ITypeNode TypeNode { get; }
    public DataType BindingType { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public ValueId BindingValueId { get; }
    public ParameterType ParameterType { get; }
    public bool Unused { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public LexicalScope ContainingLexicalScope { get; }

    public NamedParameterNode(INamedParameterSyntax syntax, bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeNode typeNode, DataType bindingType, IMaybeAntetype bindingAntetype, ValueId bindingValueId, ParameterType parameterType, bool unused, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, LexicalScope containingLexicalScope)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        IsLentBinding = isLentBinding;
        Name = name;
        TypeNode = typeNode;
        BindingType = bindingType;
        BindingAntetype = bindingAntetype;
        BindingValueId = bindingValueId;
        ParameterType = parameterType;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
        ContainingLexicalScope = containingLexicalScope;
    }
}

file class ConstructorSelfParameterNode // : IConstructorSelfParameterNode
{
    public IConstructorSelfParameterSyntax Syntax { get; }
    public bool IsLentBinding { get; }
    public ICapabilityNode Capability { get; }
    public CapabilityType BindingType { get; }
    public ObjectType ContainingDeclaredType { get; }
    public ITypeDefinitionNode ContainingTypeDefinition { get; }
    public SelfParameterType ParameterType { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public ValueId BindingValueId { get; }
    public IdentifierName? Name { get; }
    public bool Unused { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ConstructorSelfParameterNode(IConstructorSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, ObjectType containingDeclaredType, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, ValueId bindingValueId, IdentifierName? name, bool unused, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
        BindingType = bindingType;
        ContainingDeclaredType = containingDeclaredType;
        ContainingTypeDefinition = containingTypeDefinition;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        BindingValueId = bindingValueId;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class InitializerSelfParameterNode // : IInitializerSelfParameterNode
{
    public IInitializerSelfParameterSyntax Syntax { get; }
    public bool IsLentBinding { get; }
    public ICapabilityNode Capability { get; }
    public CapabilityType BindingType { get; }
    public StructType ContainingDeclaredType { get; }
    public ITypeDefinitionNode ContainingTypeDefinition { get; }
    public SelfParameterType ParameterType { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public ValueId BindingValueId { get; }
    public IdentifierName? Name { get; }
    public bool Unused { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public InitializerSelfParameterNode(IInitializerSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, StructType containingDeclaredType, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, ValueId bindingValueId, IdentifierName? name, bool unused, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
        BindingType = bindingType;
        ContainingDeclaredType = containingDeclaredType;
        ContainingTypeDefinition = containingTypeDefinition;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        BindingValueId = bindingValueId;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class MethodSelfParameterNode // : IMethodSelfParameterNode
{
    public IMethodSelfParameterSyntax Syntax { get; }
    public bool IsLentBinding { get; }
    public ICapabilityConstraintNode Capability { get; }
    public ITypeDefinitionNode ContainingTypeDefinition { get; }
    public IDeclaredUserType ContainingDeclaredType { get; }
    public SelfParameterType ParameterType { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public Pseudotype BindingType { get; }
    public ValueId BindingValueId { get; }
    public IdentifierName? Name { get; }
    public bool Unused { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public MethodSelfParameterNode(IMethodSelfParameterSyntax syntax, bool isLentBinding, ICapabilityConstraintNode capability, ITypeDefinitionNode containingTypeDefinition, IDeclaredUserType containingDeclaredType, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, Pseudotype bindingType, ValueId bindingValueId, IdentifierName? name, bool unused, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
        ContainingTypeDefinition = containingTypeDefinition;
        ContainingDeclaredType = containingDeclaredType;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        BindingValueId = bindingValueId;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class FieldParameterNode // : IFieldParameterNode
{
    public IFieldParameterSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public ITypeDefinitionNode ContainingTypeDefinition { get; }
    public IFieldDefinitionNode? ReferencedField { get; }
    public DataType BindingType { get; }
    public ParameterType ParameterType { get; }
    public bool Unused { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public ValueId BindingValueId { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public FieldParameterNode(IFieldParameterSyntax syntax, IdentifierName name, ITypeDefinitionNode containingTypeDefinition, IFieldDefinitionNode? referencedField, DataType bindingType, ParameterType parameterType, bool unused, IMaybeAntetype bindingAntetype, ValueId bindingValueId, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        ContainingTypeDefinition = containingTypeDefinition;
        ReferencedField = referencedField;
        BindingType = bindingType;
        ParameterType = parameterType;
        Unused = unused;
        BindingAntetype = bindingAntetype;
        BindingValueId = bindingValueId;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class BlockBodyNode // : IBlockBodyNode
{
    public IBlockBodySyntax Syntax { get; }
    public IFixedList<IBodyStatementNode> Statements { get; }
    public IFlowState FlowStateAfter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public BlockBodyNode(IBlockBodySyntax syntax, IFixedList<IBodyStatementNode> statements, IFlowState flowStateAfter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Statements = statements;
        FlowStateAfter = flowStateAfter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class ExpressionBodyNode // : IExpressionBodyNode
{
    public IExpressionBodySyntax Syntax { get; }
    public IResultStatementNode ResultStatement { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public IFixedList<IStatementNode> Statements { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ExpressionBodyNode(IExpressionBodySyntax syntax, IResultStatementNode resultStatement, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IFlowState flowStateAfter, IFixedList<IStatementNode> statements, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        ResultStatement = resultStatement;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Statements = statements;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class IdentifierTypeNameNode // : IIdentifierTypeNameNode
{
    public IIdentifierTypeNameSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public bool IsAttributeType { get; }
    public ITypeDeclarationNode? ReferencedDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public TypeSymbol? ReferencedSymbol { get; }
    public BareType? NamedBareType { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax, IdentifierName name, bool isAttributeType, ITypeDeclarationNode? referencedDeclaration, LexicalScope containingLexicalScope, TypeSymbol? referencedSymbol, BareType? namedBareType, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        IsAttributeType = isAttributeType;
        ReferencedDeclaration = referencedDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        ReferencedSymbol = referencedSymbol;
        NamedBareType = namedBareType;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class SpecialTypeNameNode // : ISpecialTypeNameNode
{
    public ISpecialTypeNameSyntax Syntax { get; }
    public SpecialTypeName Name { get; }
    public TypeSymbol ReferencedSymbol { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public BareType? NamedBareType { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax, SpecialTypeName name, TypeSymbol referencedSymbol, LexicalScope containingLexicalScope, BareType? namedBareType, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        ReferencedSymbol = referencedSymbol;
        ContainingLexicalScope = containingLexicalScope;
        NamedBareType = namedBareType;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class GenericTypeNameNode // : IGenericTypeNameNode
{
    public IGenericTypeNameSyntax Syntax { get; }
    public GenericName Name { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public bool IsAttributeType { get; }
    public ITypeDeclarationNode? ReferencedDeclaration { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public TypeSymbol? ReferencedSymbol { get; }
    public BareType? NamedBareType { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public GenericTypeNameNode(IGenericTypeNameSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments, bool isAttributeType, ITypeDeclarationNode? referencedDeclaration, LexicalScope containingLexicalScope, TypeSymbol? referencedSymbol, BareType? namedBareType, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
        IsAttributeType = isAttributeType;
        ReferencedDeclaration = referencedDeclaration;
        ContainingLexicalScope = containingLexicalScope;
        ReferencedSymbol = referencedSymbol;
        NamedBareType = namedBareType;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class QualifiedTypeNameNode // : IQualifiedTypeNameNode
{
    public IQualifiedTypeNameSyntax Syntax { get; }
    public ITypeNameNode Context { get; }
    public IStandardTypeNameNode QualifiedName { get; }
    public TypeName Name { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public TypeSymbol? ReferencedSymbol { get; }
    public BareType? NamedBareType { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public QualifiedTypeNameNode(IQualifiedTypeNameSyntax syntax, ITypeNameNode context, IStandardTypeNameNode qualifiedName, TypeName name, LexicalScope containingLexicalScope, TypeSymbol? referencedSymbol, BareType? namedBareType, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Context = context;
        QualifiedName = qualifiedName;
        Name = name;
        ContainingLexicalScope = containingLexicalScope;
        ReferencedSymbol = referencedSymbol;
        NamedBareType = namedBareType;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class OptionalTypeNode // : IOptionalTypeNode
{
    public IOptionalTypeSyntax Syntax { get; }
    public ITypeNode Referent { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public OptionalTypeNode(IOptionalTypeSyntax syntax, ITypeNode referent, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Referent = referent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class CapabilityTypeNode // : ICapabilityTypeNode
{
    public ICapabilityTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public CapabilityTypeNode(ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Capability = capability;
        Referent = referent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class FunctionTypeNode // : IFunctionTypeNode
{
    public IFunctionTypeSyntax Syntax { get; }
    public IFixedList<IParameterTypeNode> Parameters { get; }
    public ITypeNode Return { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public FunctionTypeNode(IFunctionTypeSyntax syntax, IFixedList<IParameterTypeNode> parameters, ITypeNode @return, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Parameters = parameters;
        Return = @return;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class ParameterTypeNode // : IParameterTypeNode
{
    public IParameterTypeSyntax Syntax { get; }
    public bool IsLent { get; }
    public ITypeNode Referent { get; }
    public ParameterType Parameter { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ParameterTypeNode(IParameterTypeSyntax syntax, bool isLent, ITypeNode referent, ParameterType parameter, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        IsLent = isLent;
        Referent = referent;
        Parameter = parameter;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class CapabilityViewpointTypeNode // : ICapabilityViewpointTypeNode
{
    public ICapabilityViewpointTypeSyntax Syntax { get; }
    public ICapabilityNode Capability { get; }
    public ITypeNode Referent { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public CapabilityViewpointTypeNode(ICapabilityViewpointTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Capability = capability;
        Referent = referent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class SelfViewpointTypeNode // : ISelfViewpointTypeNode
{
    public ISelfViewpointTypeSyntax Syntax { get; }
    public ITypeNode Referent { get; }
    public Pseudotype? NamedSelfType { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public DataType NamedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public SelfViewpointTypeNode(ISelfViewpointTypeSyntax syntax, ITypeNode referent, Pseudotype? namedSelfType, IMaybeAntetype namedAntetype, DataType namedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Referent = referent;
        NamedSelfType = namedSelfType;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class EntryNode // : IEntryNode
{
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public ICodeSyntax? Syntax { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public EntryNode(IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, ICodeSyntax? syntax, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class ExitNode // : IExitNode
{
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public ICodeSyntax? Syntax { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ExitNode(IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, ICodeSyntax? syntax, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class ResultStatementNode // : IResultStatementNode
{
    public IResultStatementSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IAmbiguousExpressionNode CurrentExpression { get; }
    public IExpressionNode? IntermediateExpression { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public IMaybeAntetype? ResultAntetype { get; }
    public DataType? ResultType { get; }
    public ValueId? ResultValueId { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IMaybeAntetype Antetype { get; }
    public DataType Type { get; }
    public ValueId ValueId { get; }

    public ResultStatementNode(IResultStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IExpressionNode? intermediateExpression, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IFlowState flowStateAfter, IMaybeAntetype? resultAntetype, DataType? resultType, ValueId? resultValueId, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, IMaybeAntetype antetype, DataType type, ValueId valueId)
    {
        Syntax = syntax;
        Expression = expression;
        CurrentExpression = currentExpression;
        IntermediateExpression = intermediateExpression;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        ResultValueId = resultValueId;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
        Antetype = antetype;
        Type = type;
        ValueId = valueId;
    }
}

file class VariableDeclarationStatementNode // : IVariableDeclarationStatementNode
{
    public IVariableDeclarationStatementSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public IdentifierName Name { get; }
    public ICapabilityNode? Capability { get; }
    public ITypeNode? Type { get; }
    public IAmbiguousExpressionNode? Initializer { get; }
    public IAmbiguousExpressionNode? CurrentInitializer { get; }
    public IExpressionNode? IntermediateInitializer { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public IMaybeAntetype? ResultAntetype { get; }
    public DataType? ResultType { get; }
    public ValueId? ResultValueId { get; }
    public IFlowState FlowStateAfter { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public DataType BindingType { get; }
    public bool IsLentBinding { get; }
    public ValueId BindingValueId { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }

    public VariableDeclarationStatementNode(IVariableDeclarationStatementSyntax syntax, bool isMutableBinding, IdentifierName name, ICapabilityNode? capability, ITypeNode? type, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer, IExpressionNode? intermediateInitializer, LexicalScope containingLexicalScope, LexicalScope lexicalScope, IMaybeAntetype? resultAntetype, DataType? resultType, ValueId? resultValueId, IFlowState flowStateAfter, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, DataType bindingType, bool isLentBinding, ValueId bindingValueId, IMaybeAntetype bindingAntetype, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        Capability = capability;
        Type = type;
        Initializer = initializer;
        CurrentInitializer = currentInitializer;
        IntermediateInitializer = intermediateInitializer;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        ResultValueId = resultValueId;
        FlowStateAfter = flowStateAfter;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
        BindingType = bindingType;
        IsLentBinding = isLentBinding;
        BindingValueId = bindingValueId;
        BindingAntetype = bindingAntetype;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

file class ExpressionStatementNode // : IExpressionStatementNode
{
    public IExpressionStatementSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IAmbiguousExpressionNode CurrentExpression { get; }
    public IExpressionNode? IntermediateExpression { get; }
    public IMaybeAntetype? ResultAntetype { get; }
    public DataType? ResultType { get; }
    public ValueId? ResultValueId { get; }
    public IFlowState FlowStateAfter { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ExpressionStatementNode(IExpressionStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IExpressionNode? intermediateExpression, IMaybeAntetype? resultAntetype, DataType? resultType, ValueId? resultValueId, IFlowState flowStateAfter, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Expression = expression;
        CurrentExpression = currentExpression;
        IntermediateExpression = intermediateExpression;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        ResultValueId = resultValueId;
        FlowStateAfter = flowStateAfter;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class BindingContextPatternNode // : IBindingContextPatternNode
{
    public IBindingContextPatternSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public IPatternNode Pattern { get; }
    public ITypeNode? Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public BindingContextPatternNode(IBindingContextPatternSyntax syntax, bool isMutableBinding, IPatternNode pattern, ITypeNode? type, IFlowState flowStateAfter, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class BindingPatternNode // : IBindingPatternNode
{
    public IBindingPatternSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public IdentifierName Name { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public ValueId BindingValueId { get; }
    public IFlowState FlowStateAfter { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public DataType BindingType { get; }
    public bool IsLentBinding { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }

    public BindingPatternNode(IBindingPatternSyntax syntax, bool isMutableBinding, IdentifierName name, LexicalScope containingLexicalScope, ValueId bindingValueId, IFlowState flowStateAfter, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, DataType bindingType, bool isLentBinding, IMaybeAntetype bindingAntetype, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        ContainingLexicalScope = containingLexicalScope;
        BindingValueId = bindingValueId;
        FlowStateAfter = flowStateAfter;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
        BindingType = bindingType;
        IsLentBinding = isLentBinding;
        BindingAntetype = bindingAntetype;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

file class OptionalPatternNode // : IOptionalPatternNode
{
    public IOptionalPatternSyntax Syntax { get; }
    public IOptionalOrBindingPatternNode Pattern { get; }
    public IFlowState FlowStateAfter { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public OptionalPatternNode(IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern, IFlowState flowStateAfter, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Pattern = pattern;
        FlowStateAfter = flowStateAfter;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class BlockExpressionNode // : IBlockExpressionNode
{
    public IBlockExpressionSyntax Syntax { get; }
    public IFixedList<IStatementNode> Statements { get; }
    public IMaybeAntetype Antetype { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public DataType? ExpectedType { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public BlockExpressionNode(IBlockExpressionSyntax syntax, IFixedList<IStatementNode> statements, IMaybeAntetype antetype, DataType type, IFlowState flowStateAfter, ValueId valueId, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Statements = statements;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class NewObjectExpressionNode // : INewObjectExpressionNode
{
    public INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode ConstructingType { get; }
    public IdentifierName? ConstructorName { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IExpressionNode?> IntermediateArguments { get; }
    public IMaybeAntetype ConstructingAntetype { get; }
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { get; }
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { get; }
    public IConstructorDeclarationNode? ReferencedConstructor { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public NewObjectExpressionNode(INewObjectExpressionSyntax syntax, ITypeNameNode constructingType, IdentifierName? constructorName, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IExpressionNode?> intermediateArguments, IMaybeAntetype constructingAntetype, IFixedSet<IConstructorDeclarationNode> referencedConstructors, IFixedSet<IConstructorDeclarationNode> compatibleConstructors, IConstructorDeclarationNode? referencedConstructor, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        ConstructingType = constructingType;
        ConstructorName = constructorName;
        Arguments = arguments;
        IntermediateArguments = intermediateArguments;
        ConstructingAntetype = constructingAntetype;
        ReferencedConstructors = referencedConstructors;
        CompatibleConstructors = compatibleConstructors;
        ReferencedConstructor = referencedConstructor;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnsafeExpressionNode // : IUnsafeExpressionNode
{
    public IUnsafeExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IExpressionNode? IntermediateExpression { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnsafeExpressionNode(IUnsafeExpressionSyntax syntax, IAmbiguousExpressionNode expression, IExpressionNode? intermediateExpression, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Expression = expression;
        IntermediateExpression = intermediateExpression;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class BoolLiteralExpressionNode // : IBoolLiteralExpressionNode
{
    public IBoolLiteralExpressionSyntax Syntax { get; }
    public bool Value { get; }
    public BoolConstValueType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public BoolLiteralExpressionNode(IBoolLiteralExpressionSyntax syntax, bool value, BoolConstValueType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Value = value;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class IntegerLiteralExpressionNode // : IIntegerLiteralExpressionNode
{
    public IIntegerLiteralExpressionSyntax Syntax { get; }
    public BigInteger Value { get; }
    public IntegerConstValueType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public IntegerLiteralExpressionNode(IIntegerLiteralExpressionSyntax syntax, BigInteger value, IntegerConstValueType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Value = value;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class NoneLiteralExpressionNode // : INoneLiteralExpressionNode
{
    public INoneLiteralExpressionSyntax Syntax { get; }
    public OptionalType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public NoneLiteralExpressionNode(INoneLiteralExpressionSyntax syntax, OptionalType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class StringLiteralExpressionNode // : IStringLiteralExpressionNode
{
    public IStringLiteralExpressionSyntax Syntax { get; }
    public string Value { get; }
    public DataType Type { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public StringLiteralExpressionNode(IStringLiteralExpressionSyntax syntax, string value, DataType type, LexicalScope containingLexicalScope, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Value = value;
        Type = type;
        ContainingLexicalScope = containingLexicalScope;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AssignmentExpressionNode // : IAssignmentExpressionNode
{
    public IAssignmentExpressionSyntax Syntax { get; }
    public IAmbiguousAssignableExpressionNode LeftOperand { get; }
    public IAmbiguousAssignableExpressionNode CurrentLeftOperand { get; }
    public IAssignableExpressionNode? IntermediateLeftOperand { get; }
    public AssignmentOperator Operator { get; }
    public IAmbiguousExpressionNode RightOperand { get; }
    public IAmbiguousExpressionNode CurrentRightOperand { get; }
    public IExpressionNode? IntermediateRightOperand { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }

    public AssignmentExpressionNode(IAssignmentExpressionSyntax syntax, IAmbiguousAssignableExpressionNode leftOperand, IAmbiguousAssignableExpressionNode currentLeftOperand, IAssignableExpressionNode? intermediateLeftOperand, AssignmentOperator @operator, IAmbiguousExpressionNode rightOperand, IAmbiguousExpressionNode currentRightOperand, IExpressionNode? intermediateRightOperand, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Syntax = syntax;
        LeftOperand = leftOperand;
        CurrentLeftOperand = currentLeftOperand;
        IntermediateLeftOperand = intermediateLeftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        CurrentRightOperand = currentRightOperand;
        IntermediateRightOperand = intermediateRightOperand;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

file class BinaryOperatorExpressionNode // : IBinaryOperatorExpressionNode
{
    public IBinaryOperatorExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode LeftOperand { get; }
    public IExpressionNode? IntermediateLeftOperand { get; }
    public BinaryOperator Operator { get; }
    public IAmbiguousExpressionNode RightOperand { get; }
    public IExpressionNode? IntermediateRightOperand { get; }
    public IAntetype? NumericOperatorCommonAntetype { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public BinaryOperatorExpressionNode(IBinaryOperatorExpressionSyntax syntax, IAmbiguousExpressionNode leftOperand, IExpressionNode? intermediateLeftOperand, BinaryOperator @operator, IAmbiguousExpressionNode rightOperand, IExpressionNode? intermediateRightOperand, IAntetype? numericOperatorCommonAntetype, LexicalScope containingLexicalScope, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        LeftOperand = leftOperand;
        IntermediateLeftOperand = intermediateLeftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        IntermediateRightOperand = intermediateRightOperand;
        NumericOperatorCommonAntetype = numericOperatorCommonAntetype;
        ContainingLexicalScope = containingLexicalScope;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnaryOperatorExpressionNode // : IUnaryOperatorExpressionNode
{
    public IUnaryOperatorExpressionSyntax Syntax { get; }
    public UnaryOperatorFixity Fixity { get; }
    public UnaryOperator Operator { get; }
    public IAmbiguousExpressionNode Operand { get; }
    public IExpressionNode? IntermediateOperand { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnaryOperatorExpressionNode(IUnaryOperatorExpressionSyntax syntax, UnaryOperatorFixity fixity, UnaryOperator @operator, IAmbiguousExpressionNode operand, IExpressionNode? intermediateOperand, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
        IntermediateOperand = intermediateOperand;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class IdExpressionNode // : IIdExpressionNode
{
    public IIdExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Referent { get; }
    public IExpressionNode? IntermediateReferent { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public IdExpressionNode(IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent, IExpressionNode? intermediateReferent, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Referent = referent;
        IntermediateReferent = intermediateReferent;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class ConversionExpressionNode // : IConversionExpressionNode
{
    public IConversionExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Referent { get; }
    public IExpressionNode? IntermediateReferent { get; }
    public ConversionOperator Operator { get; }
    public ITypeNode ConvertToType { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public ConversionExpressionNode(IConversionExpressionSyntax syntax, IAmbiguousExpressionNode referent, IExpressionNode? intermediateReferent, ConversionOperator @operator, ITypeNode convertToType, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Referent = referent;
        IntermediateReferent = intermediateReferent;
        Operator = @operator;
        ConvertToType = convertToType;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class ImplicitConversionExpressionNode // : IImplicitConversionExpressionNode
{
    public IExpressionNode Referent { get; }
    public IExpressionNode CurrentReferent { get; }
    public SimpleAntetype Antetype { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public ImplicitConversionExpressionNode(IExpressionNode referent, IExpressionNode currentReferent, SimpleAntetype antetype, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        CurrentReferent = currentReferent;
        Antetype = antetype;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class PatternMatchExpressionNode // : IPatternMatchExpressionNode
{
    public IPatternMatchExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Referent { get; }
    public IExpressionNode? IntermediateReferent { get; }
    public IPatternNode Pattern { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public PatternMatchExpressionNode(IPatternMatchExpressionSyntax syntax, IAmbiguousExpressionNode referent, IExpressionNode? intermediateReferent, IPatternNode pattern, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Referent = referent;
        IntermediateReferent = intermediateReferent;
        Pattern = pattern;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class IfExpressionNode // : IIfExpressionNode
{
    public IIfExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Condition { get; }
    public IExpressionNode? IntermediateCondition { get; }
    public IBlockOrResultNode ThenBlock { get; }
    public IElseClauseNode? ElseClause { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public IfExpressionNode(IIfExpressionSyntax syntax, IAmbiguousExpressionNode condition, IExpressionNode? intermediateCondition, IBlockOrResultNode thenBlock, IElseClauseNode? elseClause, IFlowState flowStateAfter, ValueId valueId, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Condition = condition;
        IntermediateCondition = intermediateCondition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class LoopExpressionNode // : ILoopExpressionNode
{
    public ILoopExpressionSyntax Syntax { get; }
    public IBlockExpressionNode Block { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public LoopExpressionNode(ILoopExpressionSyntax syntax, IBlockExpressionNode block, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Block = block;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class WhileExpressionNode // : IWhileExpressionNode
{
    public IWhileExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Condition { get; }
    public IExpressionNode? IntermediateCondition { get; }
    public IBlockExpressionNode Block { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public WhileExpressionNode(IWhileExpressionSyntax syntax, IAmbiguousExpressionNode condition, IExpressionNode? intermediateCondition, IBlockExpressionNode block, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Condition = condition;
        IntermediateCondition = intermediateCondition;
        Block = block;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class ForeachExpressionNode // : IForeachExpressionNode
{
    public IForeachExpressionSyntax Syntax { get; }
    public bool IsMutableBinding { get; }
    public IdentifierName VariableName { get; }
    public IAmbiguousExpressionNode InExpression { get; }
    public IExpressionNode? IntermediateInExpression { get; }
    public ITypeNode? DeclaredType { get; }
    public IBlockExpressionNode Block { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public LexicalScope LexicalScope { get; }
    public ITypeDeclarationNode? ReferencedIterableDeclaration { get; }
    public IStandardMethodDeclarationNode? ReferencedIterateMethod { get; }
    public IMaybeExpressionAntetype IteratorAntetype { get; }
    public DataType IteratorType { get; }
    public ITypeDeclarationNode? ReferencedIteratorDeclaration { get; }
    public IStandardMethodDeclarationNode? ReferencedNextMethod { get; }
    public IMaybeAntetype IteratedAntetype { get; }
    public DataType IteratedType { get; }
    public IFlowState FlowStateBeforeBlock { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }
    public DataType BindingType { get; }
    public bool IsLentBinding { get; }
    public ValueId BindingValueId { get; }
    public IMaybeAntetype BindingAntetype { get; }
    public IdentifierName Name { get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }

    public ForeachExpressionNode(IForeachExpressionSyntax syntax, bool isMutableBinding, IdentifierName variableName, IAmbiguousExpressionNode inExpression, IExpressionNode? intermediateInExpression, ITypeNode? declaredType, IBlockExpressionNode block, LexicalScope containingLexicalScope, LexicalScope lexicalScope, ITypeDeclarationNode? referencedIterableDeclaration, IStandardMethodDeclarationNode? referencedIterateMethod, IMaybeExpressionAntetype iteratorAntetype, DataType iteratorType, ITypeDeclarationNode? referencedIteratorDeclaration, IStandardMethodDeclarationNode? referencedNextMethod, IMaybeAntetype iteratedAntetype, DataType iteratedType, IFlowState flowStateBeforeBlock, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, DataType bindingType, bool isLentBinding, ValueId bindingValueId, IMaybeAntetype bindingAntetype, IdentifierName name, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        VariableName = variableName;
        InExpression = inExpression;
        IntermediateInExpression = intermediateInExpression;
        DeclaredType = declaredType;
        Block = block;
        ContainingLexicalScope = containingLexicalScope;
        LexicalScope = lexicalScope;
        ReferencedIterableDeclaration = referencedIterableDeclaration;
        ReferencedIterateMethod = referencedIterateMethod;
        IteratorAntetype = iteratorAntetype;
        IteratorType = iteratorType;
        ReferencedIteratorDeclaration = referencedIteratorDeclaration;
        ReferencedNextMethod = referencedNextMethod;
        IteratedAntetype = iteratedAntetype;
        IteratedType = iteratedType;
        FlowStateBeforeBlock = flowStateBeforeBlock;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        BindingType = bindingType;
        IsLentBinding = isLentBinding;
        BindingValueId = bindingValueId;
        BindingAntetype = bindingAntetype;
        Name = name;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

file class BreakExpressionNode // : IBreakExpressionNode
{
    public IBreakExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode? Value { get; }
    public IExpressionNode? IntermediateValue { get; }
    public NeverType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public BreakExpressionNode(IBreakExpressionSyntax syntax, IAmbiguousExpressionNode? value, IExpressionNode? intermediateValue, NeverType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Value = value;
        IntermediateValue = intermediateValue;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class NextExpressionNode // : INextExpressionNode
{
    public INextExpressionSyntax Syntax { get; }
    public NeverType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public NextExpressionNode(INextExpressionSyntax syntax, NeverType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class ReturnExpressionNode // : IReturnExpressionNode
{
    public IReturnExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode? Value { get; }
    public IAmbiguousExpressionNode? CurrentValue { get; }
    public IExpressionNode? IntermediateValue { get; }
    public NeverType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public ReturnExpressionNode(IReturnExpressionSyntax syntax, IAmbiguousExpressionNode? value, IAmbiguousExpressionNode? currentValue, IExpressionNode? intermediateValue, NeverType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Value = value;
        CurrentValue = currentValue;
        IntermediateValue = intermediateValue;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnresolvedInvocationExpressionNode // : IUnresolvedInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IAmbiguousExpressionNode CurrentExpression { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public UnresolvedInvocationExpressionNode(IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Expression = expression;
        CurrentExpression = currentExpression;
        Arguments = arguments;
        CurrentArguments = currentArguments;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class FunctionInvocationExpressionNode // : IFunctionInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IFunctionGroupNameNode FunctionGroup { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IExpressionNode?> IntermediateArguments { get; }
    public IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; }
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FunctionInvocationExpressionNode(IInvocationExpressionSyntax syntax, IFunctionGroupNameNode functionGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IExpressionNode?> intermediateArguments, IFixedSet<IFunctionLikeDeclarationNode> compatibleDeclarations, IFunctionLikeDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        FunctionGroup = functionGroup;
        Arguments = arguments;
        IntermediateArguments = intermediateArguments;
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class MethodInvocationExpressionNode // : IMethodInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IMethodGroupNameNode MethodGroup { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    public IFixedList<IExpressionNode?> IntermediateArguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { get; }
    public IStandardMethodDeclarationNode? ReferencedDeclaration { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public MethodInvocationExpressionNode(IInvocationExpressionSyntax syntax, IMethodGroupNameNode methodGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments, IFixedList<IExpressionNode?> intermediateArguments, IFixedSet<IStandardMethodDeclarationNode> compatibleDeclarations, IStandardMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        MethodGroup = methodGroup;
        Arguments = arguments;
        CurrentArguments = currentArguments;
        IntermediateArguments = intermediateArguments;
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class GetterInvocationExpressionNode // : IGetterInvocationExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public GetterInvocationExpressionNode(IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, IGetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class SetterInvocationExpressionNode // : ISetterInvocationExpressionNode
{
    public IAssignmentExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    public IAmbiguousExpressionNode Value { get; }
    public IExpressionNode? IntermediateValue { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public SetterInvocationExpressionNode(IAssignmentExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IAmbiguousExpressionNode value, IExpressionNode? intermediateValue, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, ISetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        Value = value;
        IntermediateValue = intermediateValue;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class FunctionReferenceInvocationExpressionNode // : IFunctionReferenceInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IExpressionNode Expression { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IExpressionNode?> IntermediateArguments { get; }
    public FunctionAntetype FunctionAntetype { get; }
    public FunctionType FunctionType { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FunctionReferenceInvocationExpressionNode(IInvocationExpressionSyntax syntax, IExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IExpressionNode?> intermediateArguments, FunctionAntetype functionAntetype, FunctionType functionType, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Expression = expression;
        Arguments = arguments;
        IntermediateArguments = intermediateArguments;
        FunctionAntetype = functionAntetype;
        FunctionType = functionType;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class InitializerInvocationExpressionNode // : IInitializerInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IInitializerGroupNameNode InitializerGroup { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedList<IExpressionNode?> IntermediateArguments { get; }
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; }
    public IInitializerDeclarationNode? ReferencedDeclaration { get; }
    public ContextualizedOverload? ContextualizedOverload { get; }
    public IEnumerable<IAmbiguousExpressionNode> AllArguments { get; }
    public IEnumerable<IExpressionNode?> AllIntermediateArguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public InitializerInvocationExpressionNode(IInvocationExpressionSyntax syntax, IInitializerGroupNameNode initializerGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IExpressionNode?> intermediateArguments, IFixedSet<IInitializerDeclarationNode> compatibleDeclarations, IInitializerDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload, IEnumerable<IAmbiguousExpressionNode> allArguments, IEnumerable<IExpressionNode?> allIntermediateArguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        InitializerGroup = initializerGroup;
        Arguments = arguments;
        IntermediateArguments = intermediateArguments;
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
        AllArguments = allArguments;
        AllIntermediateArguments = allIntermediateArguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnknownInvocationExpressionNode // : IUnknownInvocationExpressionNode
{
    public IInvocationExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnknownInvocationExpressionNode(IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Expression = expression;
        Arguments = arguments;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class IdentifierNameExpressionNode // : IIdentifierNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public IdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax, IdentifierName name, LexicalScope containingLexicalScope, IFixedList<IDeclarationNode> referencedDeclarations, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        ContainingLexicalScope = containingLexicalScope;
        ReferencedDeclarations = referencedDeclarations;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class GenericNameExpressionNode // : IGenericNameExpressionNode
{
    public IGenericNameExpressionSyntax Syntax { get; }
    public GenericName Name { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public LexicalScope ContainingLexicalScope { get; }
    public IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public GenericNameExpressionNode(IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments, LexicalScope containingLexicalScope, IFixedList<IDeclarationNode> referencedDeclarations, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
        ContainingLexicalScope = containingLexicalScope;
        ReferencedDeclarations = referencedDeclarations;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class MemberAccessExpressionNode // : IMemberAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Context { get; }
    public StandardName MemberName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public MemberAccessExpressionNode(IMemberAccessExpressionSyntax syntax, IAmbiguousExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class PropertyNameNode // : IPropertyNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName PropertyName { get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public PropertyNameNode(IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class UnqualifiedNamespaceNameNode // : IUnqualifiedNamespaceNameNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public UnknownType Type { get; }
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnqualifiedNamespaceNameNode(IIdentifierNameExpressionSyntax syntax, IdentifierName name, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Name = name;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class QualifiedNamespaceNameNode // : IQualifiedNamespaceNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public INamespaceNameNode Context { get; }
    public IdentifierName Name { get; }
    public UnknownType Type { get; }
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public QualifiedNamespaceNameNode(IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IdentifierName name, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        Name = name;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class FunctionGroupNameNode // : IFunctionGroupNameNode
{
    public INameExpressionNode? Context { get; }
    public StandardName FunctionName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }
    public INameExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FunctionGroupNameNode(INameExpressionNode? context, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFixedSet<IFunctionLikeDeclarationNode> referencedDeclarations, INameExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Context = context;
        FunctionName = functionName;
        TypeArguments = typeArguments;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class FunctionNameNode // : IFunctionNameNode
{
    public IFunctionGroupNameNode FunctionGroup { get; }
    public StandardName FunctionName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    public INameExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FunctionNameNode(IFunctionGroupNameNode functionGroup, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFunctionLikeDeclarationNode? referencedDeclaration, INameExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        FunctionGroup = functionGroup;
        FunctionName = functionName;
        TypeArguments = typeArguments;
        ReferencedDeclaration = referencedDeclaration;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class MethodGroupNameNode // : IMethodGroupNameNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public IExpressionNode CurrentContext { get; }
    public StandardName MethodName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public MethodGroupNameNode(IMemberAccessExpressionSyntax syntax, IExpressionNode context, IExpressionNode currentContext, StandardName methodName, IFixedList<ITypeNode> typeArguments, IFixedSet<IStandardMethodDeclarationNode> referencedDeclarations, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        CurrentContext = currentContext;
        MethodName = methodName;
        TypeArguments = typeArguments;
        ReferencedDeclarations = referencedDeclarations;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class FieldAccessExpressionNode // : IFieldAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public IdentifierName FieldName { get; }
    public IFieldDeclarationNode ReferencedDeclaration { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FieldAccessExpressionNode(IMemberAccessExpressionSyntax syntax, IExpressionNode context, IdentifierName fieldName, IFieldDeclarationNode referencedDeclaration, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        FieldName = fieldName;
        ReferencedDeclaration = referencedDeclaration;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class VariableNameExpressionNode // : IVariableNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public ILocalBindingNode ReferencedDefinition { get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public VariableNameExpressionNode(IIdentifierNameExpressionSyntax syntax, IdentifierName name, ILocalBindingNode referencedDefinition, IFixedSet<IDataFlowNode> dataFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Name = name;
        ReferencedDefinition = referencedDefinition;
        DataFlowPrevious = dataFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class StandardTypeNameExpressionNode // : IStandardTypeNameExpressionNode
{
    public IStandardNameExpressionSyntax Syntax { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public StandardName Name { get; }
    public ITypeDeclarationNode ReferencedDeclaration { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public BareType? NamedBareType { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public StandardTypeNameExpressionNode(IStandardNameExpressionSyntax syntax, IFixedList<ITypeNode> typeArguments, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        TypeArguments = typeArguments;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class QualifiedTypeNameExpressionNode // : IQualifiedTypeNameExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public INamespaceNameNode Context { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public StandardName Name { get; }
    public ITypeDeclarationNode ReferencedDeclaration { get; }
    public IMaybeAntetype NamedAntetype { get; }
    public BareType? NamedBareType { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public QualifiedTypeNameExpressionNode(IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IFixedList<ITypeNode> typeArguments, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        TypeArguments = typeArguments;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class InitializerGroupNameNode // : IInitializerGroupNameNode
{
    public INameExpressionSyntax Syntax { get; }
    public ITypeNameExpressionNode Context { get; }
    public StandardName? InitializerName { get; }
    public IMaybeAntetype InitializingAntetype { get; }
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public InitializerGroupNameNode(INameExpressionSyntax syntax, ITypeNameExpressionNode context, StandardName? initializerName, IMaybeAntetype initializingAntetype, IFixedSet<IInitializerDeclarationNode> referencedDeclarations, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        InitializerName = initializerName;
        InitializingAntetype = initializingAntetype;
        ReferencedDeclarations = referencedDeclarations;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class SpecialTypeNameExpressionNode // : ISpecialTypeNameExpressionNode
{
    public ISpecialTypeNameExpressionSyntax Syntax { get; }
    public SpecialTypeName Name { get; }
    public TypeSymbol ReferencedSymbol { get; }
    public UnknownType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public SpecialTypeNameExpressionNode(ISpecialTypeNameExpressionSyntax syntax, SpecialTypeName name, TypeSymbol referencedSymbol, UnknownType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Name = name;
        ReferencedSymbol = referencedSymbol;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class SelfExpressionNode // : ISelfExpressionNode
{
    public ISelfExpressionSyntax Syntax { get; }
    public bool IsImplicit { get; }
    public Pseudotype Pseudotype { get; }
    public IExecutableDefinitionNode ContainingDeclaration { get; }
    public ISelfParameterNode? ReferencedDefinition { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public SelfExpressionNode(ISelfExpressionSyntax syntax, bool isImplicit, Pseudotype pseudotype, IExecutableDefinitionNode containingDeclaration, ISelfParameterNode? referencedDefinition, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        IsImplicit = isImplicit;
        Pseudotype = pseudotype;
        ContainingDeclaration = containingDeclaration;
        ReferencedDefinition = referencedDefinition;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class MissingNameExpressionNode // : IMissingNameExpressionNode
{
    public IMissingNameSyntax Syntax { get; }
    public UnknownType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public MissingNameExpressionNode(IMissingNameSyntax syntax, UnknownType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnknownIdentifierNameExpressionNode // : IUnknownIdentifierNameExpressionNode
{
    public IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name { get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
    public UnknownType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnknownIdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax, IdentifierName name, IFixedSet<IDeclarationNode> referencedDeclarations, UnknownType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Name = name;
        ReferencedDeclarations = referencedDeclarations;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnknownGenericNameExpressionNode // : IUnknownGenericNameExpressionNode
{
    public IGenericNameExpressionSyntax Syntax { get; }
    public GenericName Name { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
    public UnknownType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnknownGenericNameExpressionNode(IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments, IFixedSet<IDeclarationNode> referencedDeclarations, UnknownType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
        ReferencedDeclarations = referencedDeclarations;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class UnknownMemberAccessExpressionNode // : IUnknownMemberAccessExpressionNode
{
    public IMemberAccessExpressionSyntax Syntax { get; }
    public IExpressionNode Context { get; }
    public StandardName MemberName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IDeclarationNode> ReferencedMembers { get; }
    public UnknownType Type { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public UnknownMemberAccessExpressionNode(IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments, IFixedSet<IDeclarationNode> referencedMembers, UnknownType type, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        ReferencedMembers = referencedMembers;
        Type = type;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AmbiguousMoveExpressionNode // : IAmbiguousMoveExpressionNode
{
    public IMoveExpressionSyntax Syntax { get; }
    public ISimpleNameNode Referent { get; }
    public INameExpressionNode? IntermediateReferent { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public AmbiguousMoveExpressionNode(IMoveExpressionSyntax syntax, ISimpleNameNode referent, INameExpressionNode? intermediateReferent, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Referent = referent;
        IntermediateReferent = intermediateReferent;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class MoveVariableExpressionNode // : IMoveVariableExpressionNode
{
    public ILocalBindingNameExpressionNode Referent { get; }
    public bool IsImplicit { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public MoveVariableExpressionNode(ILocalBindingNameExpressionNode referent, bool isImplicit, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        IsImplicit = isImplicit;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class MoveValueExpressionNode // : IMoveValueExpressionNode
{
    public IExpressionNode Referent { get; }
    public bool IsImplicit { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public MoveValueExpressionNode(IExpressionNode referent, bool isImplicit, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        IsImplicit = isImplicit;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class ImplicitTempMoveExpressionNode // : IImplicitTempMoveExpressionNode
{
    public IExpressionNode Referent { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public ImplicitTempMoveExpressionNode(IExpressionNode referent, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AmbiguousFreezeExpressionNode // : IAmbiguousFreezeExpressionNode
{
    public IFreezeExpressionSyntax Syntax { get; }
    public ISimpleNameNode Referent { get; }
    public INameExpressionNode? IntermediateReferent { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public AmbiguousFreezeExpressionNode(IFreezeExpressionSyntax syntax, ISimpleNameNode referent, INameExpressionNode? intermediateReferent, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Syntax = syntax;
        Referent = referent;
        IntermediateReferent = intermediateReferent;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
    }
}

file class FreezeVariableExpressionNode // : IFreezeVariableExpressionNode
{
    public ILocalBindingNameExpressionNode Referent { get; }
    public bool IsTemporary { get; }
    public bool IsImplicit { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FreezeVariableExpressionNode(ILocalBindingNameExpressionNode referent, bool isTemporary, bool isImplicit, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class FreezeValueExpressionNode // : IFreezeValueExpressionNode
{
    public IExpressionNode Referent { get; }
    public bool IsTemporary { get; }
    public bool IsImplicit { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public FreezeValueExpressionNode(IExpressionNode referent, bool isTemporary, bool isImplicit, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Referent = referent;
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class PrepareToReturnExpressionNode // : IPrepareToReturnExpressionNode
{
    public IExpressionNode Value { get; }
    public IExpressionNode CurrentValue { get; }
    public IExpressionSyntax Syntax { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public PrepareToReturnExpressionNode(IExpressionNode value, IExpressionNode currentValue, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Value = value;
        CurrentValue = currentValue;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AsyncBlockExpressionNode // : IAsyncBlockExpressionNode
{
    public IAsyncBlockExpressionSyntax Syntax { get; }
    public IBlockExpressionNode Block { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public AsyncBlockExpressionNode(IAsyncBlockExpressionSyntax syntax, IBlockExpressionNode block, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Block = block;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AsyncStartExpressionNode // : IAsyncStartExpressionNode
{
    public IAsyncStartExpressionSyntax Syntax { get; }
    public bool Scheduled { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IExpressionNode? IntermediateExpression { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public AsyncStartExpressionNode(IAsyncStartExpressionSyntax syntax, bool scheduled, IAmbiguousExpressionNode expression, IExpressionNode? intermediateExpression, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Scheduled = scheduled;
        Expression = expression;
        IntermediateExpression = intermediateExpression;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class AwaitExpressionNode // : IAwaitExpressionNode
{
    public IAwaitExpressionSyntax Syntax { get; }
    public IAmbiguousExpressionNode Expression { get; }
    public IExpressionNode? IntermediateExpression { get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { get; }
    public IMaybeExpressionAntetype Antetype { get; }
    public DataType? ExpectedType { get; }
    public DataType Type { get; }
    public IFlowState FlowStateAfter { get; }
    public ValueId ValueId { get; }
    public CodeFile File { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public ControlFlowSet ControlFlowNext { get; }
    public ControlFlowSet ControlFlowPrevious { get; }

    public AwaitExpressionNode(IAwaitExpressionSyntax syntax, IAmbiguousExpressionNode expression, IExpressionNode? intermediateExpression, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ValueId valueId, CodeFile file, ISemanticNode parent, IPackageDeclarationNode package, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious)
    {
        Syntax = syntax;
        Expression = expression;
        IntermediateExpression = intermediateExpression;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        ValueId = valueId;
        File = file;
        Parent = parent;
        Package = package;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
    }
}

file class PackageSymbolNode // : IPackageSymbolNode
{
    public IPackageFacetDeclarationNode MainFacet { get; }
    public IPackageFacetDeclarationNode TestingFacet { get; }
    public IdentifierName? AliasOrName { get; }
    public IdentifierName Name { get; }
    public PackageSymbol Symbol { get; }
    public ISyntax? Syntax { get; }

    public PackageSymbolNode(IPackageFacetDeclarationNode mainFacet, IPackageFacetDeclarationNode testingFacet, IdentifierName? aliasOrName, IdentifierName name, PackageSymbol symbol, ISyntax? syntax)
    {
        MainFacet = mainFacet;
        TestingFacet = testingFacet;
        AliasOrName = aliasOrName;
        Name = name;
        Symbol = symbol;
        Syntax = syntax;
    }
}

file class PackageFacetSymbolNode // : IPackageFacetSymbolNode
{
    public INamespaceDeclarationNode GlobalNamespace { get; }
    public IdentifierName? PackageAliasOrName { get; }
    public IdentifierName PackageName { get; }
    public PackageSymbol Symbol { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public PackageFacetSymbolNode(INamespaceDeclarationNode globalNamespace, IdentifierName? packageAliasOrName, IdentifierName packageName, PackageSymbol symbol, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        GlobalNamespace = globalNamespace;
        PackageAliasOrName = packageAliasOrName;
        PackageName = packageName;
        Symbol = symbol;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class NamespaceSymbolNode // : INamespaceSymbolNode
{
    public IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    public IdentifierName Name { get; }
    public NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public NamespaceSymbolNode(IFixedList<INamespaceMemberDeclarationNode> members, IdentifierName name, NamespaceSymbol symbol, IFixedList<INamespaceMemberDeclarationNode> nestedMembers, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Members = members;
        Name = name;
        Symbol = symbol;
        NestedMembers = nestedMembers;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class FunctionSymbolNode // : IFunctionSymbolNode
{
    public StandardName Name { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FunctionSymbol Symbol { get; }
    public FunctionType Type { get; }

    public FunctionSymbolNode(StandardName name, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, FunctionSymbol symbol, FunctionType type)
    {
        Name = name;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Symbol = symbol;
        Type = type;
    }
}

file class PrimitiveTypeSymbolNode // : IPrimitiveTypeSymbolNode
{
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    public SpecialTypeName Name { get; }
    public TypeSymbol Symbol { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public PrimitiveTypeSymbolNode(IFixedSet<ITypeMemberDeclarationNode> members, SpecialTypeName name, TypeSymbol symbol, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Members = members;
        Name = name;
        Symbol = symbol;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class UserTypeSymbolNode // : IUserTypeSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }

    public UserTypeSymbolNode(IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITypeMemberDeclarationNode> members, StandardName name, UserTypeSymbol symbol, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<BareReferenceType> supertypes)
    {
        GenericParameters = genericParameters;
        Members = members;
        Name = name;
        Symbol = symbol;
        InclusiveMembers = inclusiveMembers;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Supertypes = supertypes;
    }
}

file class ClassSymbolNode // : IClassSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    public IFixedSet<IClassMemberDeclarationNode> Members { get; }
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }

    public ClassSymbolNode(IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IClassMemberDeclarationNode> members, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers, StandardName name, UserTypeSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<BareReferenceType> supertypes)
    {
        GenericParameters = genericParameters;
        Members = members;
        InclusiveMembers = inclusiveMembers;
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Supertypes = supertypes;
    }
}

file class StructSymbolNode // : IStructSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    public IFixedSet<IStructMemberDeclarationNode> Members { get; }
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }

    public StructSymbolNode(IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IStructMemberDeclarationNode> members, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers, StandardName name, UserTypeSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<BareReferenceType> supertypes)
    {
        GenericParameters = genericParameters;
        Members = members;
        InclusiveMembers = inclusiveMembers;
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Supertypes = supertypes;
    }
}

file class TraitSymbolNode // : ITraitSymbolNode
{
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    public IFixedSet<ITraitMemberDeclarationNode> Members { get; }
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; }
    public StandardName Name { get; }
    public UserTypeSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }

    public TraitSymbolNode(IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITraitMemberDeclarationNode> members, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers, StandardName name, UserTypeSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IFixedSet<BareReferenceType> supertypes)
    {
        GenericParameters = genericParameters;
        Members = members;
        InclusiveMembers = inclusiveMembers;
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Supertypes = supertypes;
    }
}

file class GenericParameterSymbolNode // : IGenericParameterSymbolNode
{
    public IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    public IdentifierName Name { get; }
    public GenericParameterTypeSymbol Symbol { get; }
    public IFixedSet<BareReferenceType> Supertypes { get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IPackageFacetDeclarationNode Facet { get; }

    public GenericParameterSymbolNode(IFixedSet<ITypeMemberDeclarationNode> members, IdentifierName name, GenericParameterTypeSymbol symbol, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IPackageFacetDeclarationNode facet)
    {
        Members = members;
        Name = name;
        Symbol = symbol;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Facet = facet;
    }
}

file class StandardMethodSymbolNode // : IStandardMethodSymbolNode
{
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public int Arity { get; }
    public FunctionType MethodGroupType { get; }

    public StandardMethodSymbolNode(IdentifierName name, MethodSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, int arity, FunctionType methodGroupType)
    {
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Arity = arity;
        MethodGroupType = methodGroupType;
    }
}

file class GetterMethodSymbolNode // : IGetterMethodSymbolNode
{
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public GetterMethodSymbolNode(IdentifierName name, MethodSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class SetterMethodSymbolNode // : ISetterMethodSymbolNode
{
    public IdentifierName Name { get; }
    public MethodSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public SetterMethodSymbolNode(IdentifierName name, MethodSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class ConstructorSymbolNode // : IConstructorSymbolNode
{
    public IdentifierName? Name { get; }
    public ConstructorSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public ConstructorSymbolNode(IdentifierName? name, ConstructorSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class InitializerSymbolNode // : IInitializerSymbolNode
{
    public IdentifierName? Name { get; }
    public InitializerSymbol Symbol { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }

    public InitializerSymbolNode(IdentifierName? name, InitializerSymbol symbol, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package)
    {
        Name = name;
        Symbol = symbol;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
    }
}

file class FieldSymbolNode // : IFieldSymbolNode
{
    public IdentifierName Name { get; }
    public DataType BindingType { get; }
    public FieldSymbol Symbol { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public IPackageFacetDeclarationNode Facet { get; }

    public FieldSymbolNode(IdentifierName name, DataType bindingType, FieldSymbol symbol, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, IPackageFacetDeclarationNode facet)
    {
        Name = name;
        BindingType = bindingType;
        Symbol = symbol;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Facet = facet;
    }
}

file class AssociatedFunctionSymbolNode // : IAssociatedFunctionSymbolNode
{
    public StandardName Name { get; }
    public IPackageFacetDeclarationNode Facet { get; }
    public ISyntax? Syntax { get; }
    public ISemanticNode Parent { get; }
    public IPackageDeclarationNode Package { get; }
    public FunctionSymbol Symbol { get; }
    public FunctionType Type { get; }

    public AssociatedFunctionSymbolNode(StandardName name, IPackageFacetDeclarationNode facet, ISyntax? syntax, ISemanticNode parent, IPackageDeclarationNode package, FunctionSymbol symbol, FunctionType type)
    {
        Name = name;
        Facet = facet;
        Syntax = syntax;
        Parent = parent;
        Package = package;
        Symbol = symbol;
        Type = type;
    }
}

