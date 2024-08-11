using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
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
    typeof(INamedBindingNode),
    typeof(IVariableBindingNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IInvocableDefinitionNode),
    typeof(IExecutableDefinitionNode),
    typeof(IConcreteFunctionInvocableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(INamespaceMemberDefinitionNode),
    typeof(ITypeDefinitionNode),
    typeof(ITypeMemberDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
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
    typeof(IControlFlowNode),
    typeof(IEntryNode),
    typeof(IExitNode),
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode),
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
    typeof(IImplicitConversionExpressionNode),
    typeof(IPatternMatchExpressionNode),
    typeof(IIfExpressionNode),
    typeof(ILoopExpressionNode),
    typeof(IWhileExpressionNode),
    typeof(IFunctionInvocationExpressionNode),
    typeof(IMethodInvocationExpressionNode),
    typeof(IGetterInvocationExpressionNode),
    typeof(ISetterInvocationExpressionNode),
    typeof(IFunctionReferenceInvocationExpressionNode),
    typeof(IInitializerInvocationExpressionNode),
    typeof(IAmbiguousNameNode),
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode),
    typeof(INameExpressionNode),
    typeof(IUnqualifiedNamespaceNameNode),
    typeof(IQualifiedNamespaceNameNode),
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode),
    typeof(ISelfExpressionNode),
    typeof(IUnknownStandardNameExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IImplicitTempMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode),
    typeof(IDeclarationNode),
    typeof(INamedDeclarationNode),
    typeof(IInvocableDeclarationNode),
    typeof(IBindingDeclarationNode),
    typeof(IPackageDeclarationNode),
    typeof(IPackageMemberDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode),
    typeof(INamespaceDeclarationNode),
    typeof(IPrimitiveTypeDeclarationNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IGenericParameterDeclarationNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IInstanceMemberDeclarationNode),
    typeof(IStandardMethodDeclarationNode),
    typeof(IPropertyAccessorDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode),
    typeof(IFunctionSymbolNode),
    typeof(IUserTypeSymbolNode),
    typeof(IMethodSymbolNode),
    typeof(IConstructorSymbolNode),
    typeof(IInitializerSymbolNode),
    typeof(IFieldSymbolNode))]
public partial interface ISemanticNode : ITreeNode
{
    ISyntax? Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(IPackageFacetNode),
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
public partial interface IBodyOrBlockNode : ISemanticNode, ICodeNode
{
    IFixedList<IStatementNode> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultNode),
    typeof(IIfExpressionNode))]
public partial interface IElseClauseNode : IControlFlowNode
{
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
public partial interface INamedBindingNode : ISemanticNode, IBindingNode, INamedBindingDeclarationNode
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
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
public partial interface IVariableBindingNode : ISemanticNode, ILocalBindingNode, IDataFlowNode
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
    Diagnostics Diagnostics { get; }
    IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; }
    IFunctionDefinitionNode? EntryPoint { get; }
    IPackageSymbols PackageSymbols { get; }
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
    Diagnostics Diagnostics { get; }
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
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
public partial interface IConcreteFunctionInvocableDefinitionNode : ISemanticNode, IConcreteInvocableDefinitionNode
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

public partial interface IFunctionDefinitionNode : IPackageMemberDefinitionNode, IFunctionDeclarationNode, IConcreteFunctionInvocableDefinitionNode
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
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    new FunctionType Type { get; }
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
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
    TypeName INamedDeclarationNode.Name => Name;
    StandardName IUserTypeDeclarationNode.Name => Name;
    IDeclaredUserType DeclaredType { get; }
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    UserTypeSymbol IUserTypeDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    LexicalScope SupertypesLexicalScope { get; }
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
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
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterNode> ITypeDefinitionNode.GenericParameters => GenericParameters;
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    IStandardTypeNameNode? BaseTypeName { get; }
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IClassMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
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
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterNode> ITypeDefinitionNode.GenericParameters => GenericParameters;
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IStructMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IDefaultInitializerDefinitionNode? DefaultInitializer { get; }
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
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterNode> ITypeDefinitionNode.GenericParameters => GenericParameters;
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
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
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
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
    TypeName INamedDeclarationNode.Name => Name;
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new MethodSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    MethodSymbol IMethodDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

public partial interface IAbstractMethodDefinitionNode : IMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IAbstractMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
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
    new IBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

public partial interface IStandardMethodDefinitionNode : IConcreteMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IStandardMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
}

public partial interface IGetterMethodDefinitionNode : IConcreteMethodDefinitionNode, IGetterMethodDeclarationNode
{
    new IGetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new ITypeNode Return { get; }
}

public partial interface ISetterMethodDefinitionNode : IConcreteMethodDefinitionNode, ISetterMethodDeclarationNode
{
    new ISetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
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
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{
}

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
public partial interface IInitializerDefinitionNode : ISemanticNode, IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
{
    new IInitializerDefinitionSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IStructMemberDefinitionSyntax? IStructMemberDefinitionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IdentifierName? Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName? IInitializerDeclarationNode.Name => Name;
    new InitializerSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

public partial interface IDefaultInitializerDefinitionNode : IInitializerDefinitionNode
{
}

public partial interface ISourceInitializerDefinitionNode : IInitializerDefinitionNode
{
    new IInitializerDefinitionSyntax Syntax { get; }
    IInitializerDefinitionSyntax? IInitializerDefinitionNode.Syntax => Syntax;
    IInitializerSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, INamedBindingNode, IFieldDeclarationNode, IExecutableDefinitionNode
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
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new DataType BindingType { get; }
    DataType INamedBindingNode.BindingType => BindingType;
    DataType IFieldDeclarationNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FieldSymbol IFieldDeclarationNode.Symbol => Symbol;
    IAmbiguousExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    IExpressionNode? IntermediateInitializer { get; }
}

public partial interface IAssociatedFunctionDefinitionNode : IConcreteFunctionInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IAssociatedMemberDefinitionNode, IAssociatedFunctionDeclarationNode
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
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new FunctionSymbol Symbol { get; }
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    new FunctionType Type { get; }
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
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

public partial interface INamedParameterNode : ISemanticNode, IConstructorOrInitializerParameterNode, ILocalBindingNode
{
    new INamedParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    int? DeclarationNumber { get; }
    ITypeNode TypeNode { get; }
    NamedVariableSymbol Symbol { get; }
}

[Closed(
    typeof(IConstructorSelfParameterNode),
    typeof(IInitializerSelfParameterNode),
    typeof(IMethodSelfParameterNode))]
public partial interface ISelfParameterNode : IParameterNode, IBindingNode
{
    new ISelfParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    SelfParameterType ParameterType { get; }
    SelfParameterSymbol Symbol { get; }
}

public partial interface IConstructorSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype IParameterNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new ObjectType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

public partial interface IInitializerSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IInitializerSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype IParameterNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new StructType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;
}

public partial interface IMethodSelfParameterNode : ISemanticNode, ISelfParameterNode
{
    new IMethodSelfParameterSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    ICapabilityConstraintNode Capability { get; }
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
    IFlowState FlowStateAfter { get; }
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
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
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
    Pseudotype? NamedSelfType { get; }
}

[Closed(
    typeof(IElseClauseNode),
    typeof(IDataFlowNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IExpressionNode))]
public partial interface IControlFlowNode : ISemanticNode, ICodeNode
{
    ControlFlowSet ControlFlowNext { get; }
    ControlFlowSet ControlFlowPrevious { get; }
}

public partial interface IEntryNode : ISemanticNode, IDataFlowNode
{
}

public partial interface IExitNode : ISemanticNode, IDataFlowNode
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
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IMaybeAntetype? ResultAntetype { get; }
    DataType? ResultType { get; }
    ValueId? ResultValueId { get; }
    IFlowState FlowStateAfter { get; }
}

public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IExpressionNode? IntermediateExpression { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
}

public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, IVariableBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IAmbiguousExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    IExpressionNode? IntermediateInitializer { get; }
    LexicalScope LexicalScope { get; }
    NamedVariableSymbol Symbol { get; }
    int? DeclarationNumber { get; }
}

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
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IFlowState FlowStateAfter { get; }
}

public partial interface IBindingContextPatternNode : ISemanticNode, IPatternNode
{
    new IBindingContextPatternSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    IPatternNode Pattern { get; }
    ITypeNode? Type { get; }
}

[Closed(
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode))]
public partial interface IOptionalOrBindingPatternNode : ISemanticNode, IPatternNode
{
    new IOptionalOrBindingPatternSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
}

public partial interface IBindingPatternNode : IOptionalOrBindingPatternNode, IVariableBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
}

public partial interface IOptionalPatternNode : IOptionalOrBindingPatternNode
{
    new IOptionalPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IOptionalOrBindingPatternNode Pattern { get; }
}

[Closed(
    typeof(IAmbiguousAssignableExpressionNode),
    typeof(IExpressionNode),
    typeof(IInvocationExpressionNode),
    typeof(IAmbiguousNameExpressionNode),
    typeof(IAmbiguousMoveExpressionNode),
    typeof(IAmbiguousFreezeExpressionNode))]
public partial interface IAmbiguousExpressionNode : ISemanticNode, ICodeNode
{
    new IExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    typeof(INewObjectExpressionNode),
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
    typeof(IFunctionInvocationExpressionNode),
    typeof(IMethodInvocationExpressionNode),
    typeof(IGetterInvocationExpressionNode),
    typeof(ISetterInvocationExpressionNode),
    typeof(IFunctionReferenceInvocationExpressionNode),
    typeof(IInitializerInvocationExpressionNode),
    typeof(IUnknownInvocationExpressionNode),
    typeof(INameExpressionNode),
    typeof(IMoveExpressionNode),
    typeof(IImplicitTempMoveExpressionNode),
    typeof(IFreezeExpressionNode),
    typeof(IAsyncBlockExpressionNode),
    typeof(IAsyncStartExpressionNode),
    typeof(IAwaitExpressionNode))]
public partial interface IExpressionNode : IAmbiguousExpressionNode, IControlFlowNode
{
    new IExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
public partial interface IAssignableExpressionNode : ISemanticNode, IExpressionNode, IAmbiguousAssignableExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
}

public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
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
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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

public partial interface IUnsafeExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
public partial interface INeverTypedExpressionNode : ISemanticNode, IExpressionNode
{
    new INeverTypedExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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

public partial interface IAssignmentExpressionNode : ISemanticNode, IExpressionNode, IDataFlowNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousAssignableExpressionNode LeftOperand { get; }
    IAmbiguousAssignableExpressionNode CurrentLeftOperand { get; }
    IAssignableExpressionNode? IntermediateLeftOperand { get; }
    AssignmentOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
    IAmbiguousExpressionNode CurrentRightOperand { get; }
    IExpressionNode? IntermediateRightOperand { get; }
}

public partial interface IBinaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode LeftOperand { get; }
    IExpressionNode? IntermediateLeftOperand { get; }
    BinaryOperator Operator { get; }
    IAmbiguousExpressionNode RightOperand { get; }
    IExpressionNode? IntermediateRightOperand { get; }
    IAntetype? NumericOperatorCommonAntetype { get; }
    LexicalScope ContainingLexicalScope { get; }
}

public partial interface IUnaryOperatorExpressionNode : ISemanticNode, IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IAmbiguousExpressionNode Operand { get; }
    IExpressionNode? IntermediateOperand { get; }
}

public partial interface IIdExpressionNode : ISemanticNode, IExpressionNode
{
    new IIdExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
}

public partial interface IConversionExpressionNode : ISemanticNode, IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }
}

public partial interface IImplicitConversionExpressionNode : ISemanticNode, IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    new SimpleAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;
}

public partial interface IPatternMatchExpressionNode : ISemanticNode, IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Referent { get; }
    IExpressionNode? IntermediateReferent { get; }
    IPatternNode Pattern { get; }
}

public partial interface IIfExpressionNode : ISemanticNode, IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Condition { get; }
    IExpressionNode? IntermediateCondition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
}

public partial interface ILoopExpressionNode : ISemanticNode, IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IWhileExpressionNode : ISemanticNode, IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IAmbiguousExpressionNode Condition { get; }
    IExpressionNode? IntermediateCondition { get; }
    IBlockExpressionNode Block { get; }
}

public partial interface IForeachExpressionNode : IExpressionNode, IVariableBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
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

public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    INeverTypedExpressionSyntax INeverTypedExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode? Value { get; }
    IExpressionNode? IntermediateValue { get; }
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
    IAmbiguousExpressionNode? CurrentValue { get; }
    IExpressionNode? IntermediateValue { get; }
}

public partial interface IInvocationExpressionNode : IAmbiguousExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
}

public partial interface IFunctionInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IFunctionGroupNameNode FunctionGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

public partial interface IMethodInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IMethodGroupNameNode MethodGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { get; }
    IStandardMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

public partial interface IGetterInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

public partial interface ISetterInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IAmbiguousExpressionNode Value { get; }
    IExpressionNode? IntermediateValue { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

public partial interface IFunctionReferenceInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Expression { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    FunctionAntetype FunctionAntetype { get; }
    FunctionType FunctionType { get; }
}

public partial interface IInitializerInvocationExpressionNode : ISemanticNode, IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IInitializerGroupNameNode InitializerGroup { get; }
    IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    IFixedList<IExpressionNode?> IntermediateArguments { get; }
    IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; }
    IInitializerDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
}

public partial interface IUnknownInvocationExpressionNode : IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
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

public partial interface IIdentifierNameExpressionNode : ISemanticNode, IStandardNameExpressionNode, ISimpleNameNode, IAmbiguousAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
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

public partial interface IMemberAccessExpressionNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
}

public partial interface IPropertyNameNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
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
public partial interface INameExpressionNode : ISemanticNode, IExpressionNode, IAmbiguousNameExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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

public partial interface IUnqualifiedNamespaceNameNode : ISemanticNode, INamespaceNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IdentifierName Name { get; }
}

public partial interface IQualifiedNamespaceNameNode : ISemanticNode, INamespaceNameNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
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

public partial interface IFunctionNameNode : INameExpressionNode
{
    IFunctionGroupNameNode FunctionGroup { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
}

public partial interface IMethodGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }
}

public partial interface IFieldAccessExpressionNode : INameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IdentifierName FieldName { get; }
    IFieldDeclarationNode ReferencedDeclaration { get; }
}

public partial interface IVariableNameExpressionNode : ILocalBindingNameExpressionNode, IAssignableExpressionNode, ISimpleNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
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
    ITypeNameExpressionNode Context { get; }
    StandardName? InitializerName { get; }
    IMaybeAntetype InitializingAntetype { get; }
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

public partial interface ISelfExpressionNode : ISemanticNode, IInstanceExpressionNode, ILocalBindingNameExpressionNode
{
    new ISelfExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
    IExecutableDefinitionNode ContainingDeclaration { get; }
    new ISelfParameterNode? ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    SelfParameterSymbol? ReferencedSymbol { get; }
}

public partial interface IMissingNameExpressionNode : INameExpressionNode, ISimpleNameNode, IAssignableExpressionNode
{
    new IMissingNameSyntax Syntax { get; }
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax ISimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
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
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
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

public partial interface IUnknownMemberAccessExpressionNode : IUnknownNameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IAssignableExpressionSyntax IAssignableExpressionNode.Syntax => Syntax;
    INameExpressionSyntax INameExpressionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IDeclarationNode> ReferencedMembers { get; }
}

public partial interface IAmbiguousMoveExpressionNode : IAmbiguousExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ISimpleNameNode Referent { get; }
    INameExpressionNode? IntermediateReferent { get; }
}

[Closed(
    typeof(IMoveVariableExpressionNode),
    typeof(IMoveValueExpressionNode))]
public partial interface IMoveExpressionNode : ISemanticNode, IExpressionNode
{
    new IExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Referent { get; }
    bool IsImplicit { get; }
}

public partial interface IMoveVariableExpressionNode : IMoveExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IMoveExpressionNode.Referent => Referent;
}

public partial interface IMoveValueExpressionNode : IMoveExpressionNode
{
}

public partial interface IImplicitTempMoveExpressionNode : ISemanticNode, IExpressionNode
{
    new IExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Referent { get; }
}

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
public partial interface IFreezeExpressionNode : ISemanticNode, IExpressionNode
{
    new IExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IExpressionNode Referent { get; }
    bool IsTemporary { get; }
    bool IsImplicit { get; }
}

public partial interface IFreezeVariableExpressionNode : IFreezeExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IFreezeExpressionNode.Referent => Referent;
}

public partial interface IFreezeValueExpressionNode : IFreezeExpressionNode
{
}

public partial interface IAsyncBlockExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
}

public partial interface IAsyncStartExpressionNode : ISemanticNode, IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
    bool Scheduled { get; }
    IAmbiguousExpressionNode Expression { get; }
    IExpressionNode? IntermediateExpression { get; }
}

public partial interface IAwaitExpressionNode : ISemanticNode, IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionSyntax IExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    IConcreteSyntax? ICodeNode.Syntax => Syntax;
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
public partial interface INamedDeclarationNode : ISemanticNode, IChildDeclarationNode
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
public partial interface IInvocableDeclarationNode : ISemanticNode, ISymbolDeclarationNode, IChildDeclarationNode
{
    new InvocableSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
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
    TypeName INamedDeclarationNode.Name => Name;
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
    TypeName INamedDeclarationNode.Name => Name;
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
public partial interface IPrimitiveTypeDeclarationNode : ISemanticNode, ITypeDeclarationNode
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
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IClassSymbolNode))]
public partial interface IClassDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedSet<IClassMemberDeclarationNode> Members { get; }
    new IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IStructDefinitionNode),
    typeof(IStructSymbolNode))]
public partial interface IStructDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedSet<IStructMemberDeclarationNode> Members { get; }
    new IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(ITraitDefinitionNode),
    typeof(ITraitSymbolNode))]
public partial interface ITraitDeclarationNode : ISemanticNode, IUserTypeDeclarationNode
{
    new IFixedSet<ITraitMemberDeclarationNode> Members { get; }
    new IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IGenericParameterNode),
    typeof(IGenericParameterSymbolNode))]
public partial interface IGenericParameterDeclarationNode : ISemanticNode, ITypeDeclarationNode, IAssociatedMemberDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new GenericParameterTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
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
public partial interface IInstanceMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
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
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IStandardMethodDefinitionNode),
    typeof(IStandardMethodSymbolNode))]
public partial interface IStandardMethodDeclarationNode : ISemanticNode, IMethodDeclarationNode
{
    int Arity { get; }
    FunctionType MethodGroupType { get; }
}

[Closed(
    typeof(IGetterMethodDeclarationNode),
    typeof(ISetterMethodDeclarationNode))]
public partial interface IPropertyAccessorDeclarationNode : ISemanticNode, IMethodDeclarationNode
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
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
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
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
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
public partial interface IAssociatedFunctionDeclarationNode : ISemanticNode, IAssociatedMemberDeclarationNode, IFunctionLikeDeclarationNode
{
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

public partial interface IPrimitiveTypeSymbolNode : IPrimitiveTypeDeclarationNode
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

[Closed(
    typeof(IStandardMethodSymbolNode),
    typeof(IGetterMethodSymbolNode),
    typeof(ISetterMethodSymbolNode))]
public partial interface IMethodSymbolNode : ISemanticNode, IMethodDeclarationNode
{
}

public partial interface IStandardMethodSymbolNode : IMethodSymbolNode, IStandardMethodDeclarationNode
{
}

public partial interface IGetterMethodSymbolNode : IMethodSymbolNode, IGetterMethodDeclarationNode
{
}

public partial interface ISetterMethodSymbolNode : IMethodSymbolNode, ISetterMethodDeclarationNode
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

public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode
{
}

