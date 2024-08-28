using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
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
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable ConvertToPrimaryConstructor

[Closed(
    typeof(IChildNode),
    typeof(IDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISemanticNode : ITreeNode
{
    ISyntax? Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(ICodeNode),
    typeof(IChildDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IChildNode : IChildTreeNode<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
    IPackageDeclarationNode Package { get; }
}

[Closed(
    typeof(IBodyNode),
    typeof(IBlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyOrBlockNode : ICodeNode
{
    LexicalScope ContainingLexicalScope();
    IFixedList<IStatementNode> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultNode),
    typeof(IIfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IElseClauseNode : IControlFlowNode
{
    new ICodeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFlowState FlowStateAfter { get; }
    ValueId ValueId { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockOrResultNode : IElseClauseNode
{
    IMaybeAntetype Antetype { get; }
    DataType Type { get; }
}

[Closed(
    typeof(INamedBindingNode),
    typeof(ISelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingNode : ICodeNode, IBindingDeclarationNode
{
    bool IsLentBinding { get; }
    IMaybeAntetype BindingAntetype { get; }
    Pseudotype BindingType { get; }
    ValueId BindingValueId { get; }
}

[Closed(
    typeof(ILocalBindingNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILocalBindingNode : INamedBindingNode
{
    new ILocalBindingSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IBindingPatternNode),
    typeof(IForeachExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableBindingNode : ILocalBindingNode, IDataFlowNode
{
}

// [Closed(typeof(PackageNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageNode : IPackageDeclarationNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedSet<IPackageReferenceNode> References { get; }
    new IPackageFacetNode MainFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.MainFacet => MainFacet;
    new IPackageFacetNode TestingFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.TestingFacet => TestingFacet;
    DiagnosticCollection Diagnostics { get; }
    FixedDictionary<IdentifierName, IPackageDeclarationNode> PackageDeclarations { get; }
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IPackageDeclarationNode.Name => Name;
    IFunctionDefinitionNode? EntryPoint { get; }
    IPackageSymbols PackageSymbols { get; }
    IPackageReferenceNode IntrinsicsReference { get; }
    IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; }
    IdentifierName? IPackageDeclarationNode.AliasOrName
        => null;

    public static IPackageNode Create(IPackageSyntax syntax, IFixedSet<IPackageReferenceNode> references, IPackageFacetNode mainFacet, IPackageFacetNode testingFacet, DiagnosticCollection diagnostics)
        => new PackageNode(syntax, references, mainFacet, testingFacet, diagnostics);
}

// [Closed(typeof(PackageReferenceNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IdentifierName AliasOrName { get; }
    IPackageSymbols PackageSymbols { get; }
    bool IsTrusted { get; }
    IPackageSymbolNode SymbolNode { get; }

    public static IPackageReferenceNode Create(ISemanticNode parent, IPackageReferenceSyntax? syntax, IdentifierName aliasOrName, IPackageSymbols packageSymbols, bool isTrusted)
        => new PackageReferenceNode(parent, syntax, aliasOrName, packageSymbols, isTrusted);
}

// [Closed(typeof(PackageFacetNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetNode : IPackageFacetDeclarationNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    PackageSymbol PackageSymbol { get; }
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    PackageNameScope PackageNameScope { get; }
    new INamespaceDefinitionNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;
    IFixedSet<IPackageMemberDefinitionNode> Definitions { get; }

    public static IPackageFacetNode Create(ISemanticNode parent, IdentifierName? packageAliasOrName, PackageSymbol symbol, IPackageSyntax syntax, IdentifierName packageName, PackageSymbol packageSymbol, IFixedSet<ICompilationUnitNode> compilationUnits)
        => new PackageFacetNode(parent, packageAliasOrName, symbol, syntax, packageName, packageSymbol, compilationUnits);
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICodeNode : IChildNode
{
    new ICodeSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    CodeFile File { get; }
}

// [Closed(typeof(CompilationUnitNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICompilationUnitNode : ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    PackageSymbol ContainingSymbol { get; }
    NamespaceName ImplicitNamespaceName { get; }
    NamespaceSymbol ImplicitNamespaceSymbol { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    DiagnosticCollection Diagnostics { get; }
    NamespaceScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    IPackageFacetNode ContainingDeclaration { get; }
    INamespaceDefinitionNode ImplicitNamespace { get; }

    public static ICompilationUnitNode Create(ISemanticNode parent, ICompilationUnitSyntax syntax, PackageSymbol containingSymbol, NamespaceName implicitNamespaceName, NamespaceSymbol implicitNamespaceSymbol, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> definitions, DiagnosticCollection diagnostics)
        => new CompilationUnitNode(parent, syntax, containingSymbol, implicitNamespaceName, implicitNamespaceSymbol, usingDirectives, definitions, diagnostics);
}

// [Closed(typeof(UsingDirectiveNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUsingDirectiveNode : ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    NamespaceName Name { get; }

    public static IUsingDirectiveNode Create(ISemanticNode parent, IUsingDirectiveSyntax syntax, NamespaceName name)
        => new UsingDirectiveNode(parent, syntax, name);
}

[Closed(
    typeof(IInvocableDefinitionNode),
    typeof(IExecutableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(ITypeMemberDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefinitionNode : ICodeNode, IPackageFacetChildDeclarationNode
{
    new IDefinitionSyntax? Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    Symbol ContainingSymbol { get; }
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    new IPackageFacetNode Facet { get; }
    IPackageFacetDeclarationNode IPackageFacetChildDeclarationNode.Facet => Facet;
    ISymbolDeclarationNode ContainingDeclaration { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDefinitionNode : IDefinitionNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    InvocableSymbol Symbol { get; }
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExecutableDefinitionNode : IDefinitionNode
{
    IEntryNode Entry { get; }
    IExitNode Exit { get; }
    FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { get; }
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IConcreteFunctionInvocableDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConcreteFunctionInvocableDefinitionNode : IConcreteInvocableDefinitionNode
{
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
    FunctionType Type { get; }
    new FunctionSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
}

// [Closed(typeof(NamespaceBlockDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockDefinitionNode : INamespaceBlockMemberDefinitionNode
{
    new INamespaceDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    NamespaceSymbol Symbol { get; }
    new NamespaceSearchScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    new INamespaceDefinitionNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    INamespaceDefinitionNode ContainingNamespace { get; }
    INamespaceDefinitionNode Definition { get; }

    public static INamespaceBlockDefinitionNode Create(ISemanticNode parent, StandardName? name, INamespaceDefinitionSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> members, NamespaceSymbol containingSymbol, NamespaceSymbol symbol)
        => new NamespaceBlockDefinitionNode(parent, name, syntax, isGlobalQualified, declaredNames, usingDirectives, members, containingSymbol, symbol);
}

[Closed(
    typeof(IPackageMemberDefinitionNode),
    typeof(INamespaceBlockDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockMemberDefinitionNode : IDefinitionNode
{
}

// [Closed(typeof(NamespaceDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceDefinitionNode : INamespaceMemberDefinitionNode, INamespaceDeclarationNode
{
    IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; }
    IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; }
    new IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;

    public static INamespaceDefinitionNode Create(ISyntax? syntax, ISemanticNode parent, NamespaceSymbol symbol, IdentifierName name, IFixedList<INamespaceMemberDeclarationNode> nestedMembers, IFixedList<INamespaceDefinitionNode> memberNamespaces, IFixedList<IPackageMemberDefinitionNode> packageMembers, IFixedList<INamespaceMemberDefinitionNode> members)
        => new NamespaceDefinitionNode(syntax, parent, symbol, name, nestedMembers, memberNamespaces, packageMembers, members);
}

[Closed(
    typeof(IPackageMemberDefinitionNode),
    typeof(INamespaceDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberDefinitionNode : INamespaceMemberDeclarationNode
{
}

// [Closed(typeof(FunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDefinitionNode : IPackageMemberDefinitionNode, IFunctionDeclarationNode, IConcreteFunctionInvocableDefinitionNode
{
    new IFunctionDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    new FunctionType Type { get; }
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    new INamespaceDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;

    public static IFunctionDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IFunctionDefinitionSyntax syntax, NamespaceSymbol containingSymbol, IFixedList<IAttributeNode> attributes, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit, FunctionType type)
        => new FunctionDefinitionNode(parent, accessModifier, variableBindingsMap, syntax, containingSymbol, attributes, name, parameters, @return, entry, body, exit, type);
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDefinitionNode : IPackageMemberDefinitionNode, IAssociatedMemberDefinitionNode, IUserTypeDeclarationNode
{
    new ITypeDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    bool IsConst { get; }
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    StandardName IAssociatedMemberDefinitionNode.Name => Name;
    IDeclaredUserType DeclaredType { get; }
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    LexicalScope SupertypesLexicalScope { get; }
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames
        => SupertypeNames;
    new AccessModifier AccessModifier { get; }
    AccessModifier IPackageMemberDefinitionNode.AccessModifier => AccessModifier;
    AccessModifier ITypeMemberDefinitionNode.AccessModifier => AccessModifier;
}

// [Closed(typeof(ClassDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassDefinitionNode : ITypeDefinitionNode, IClassDeclarationNode
{
    new IClassDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    bool IsAbstract { get; }
    IStandardTypeNameNode? BaseTypeName { get; }
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IClassMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IDefaultConstructorDefinitionNode? DefaultConstructor { get; }
    IEnumerable<IStandardTypeNameNode> ITypeDefinitionNode.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);

    public static IClassDefinitionNode Create(ISemanticNode parent, Symbol containingSymbol, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IClassDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, bool isAbstract, IFixedList<IGenericParameterNode> genericParameters, IStandardTypeNameNode? baseTypeName, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedList<IClassMemberDefinitionNode> sourceMembers, IFixedSet<IClassMemberDefinitionNode> members, IDefaultConstructorDefinitionNode? defaultConstructor)
        => new ClassDefinitionNode(parent, containingSymbol, inclusiveMembers, isConst, name, supertypes, accessModifier, syntax, attributes, isAbstract, genericParameters, baseTypeName, supertypeNames, declaredType, sourceMembers, members, defaultConstructor);
}

// [Closed(typeof(StructDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructDefinitionNode : ITypeDefinitionNode, IStructDeclarationNode
{
    new IStructDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    new IFixedSet<IStructMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IDefaultInitializerDefinitionNode? DefaultInitializer { get; }

    public static IStructDefinitionNode Create(ISemanticNode parent, Symbol containingSymbol, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IStructDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, StructType declaredType, IFixedList<IStructMemberDefinitionNode> sourceMembers, IFixedSet<IStructMemberDefinitionNode> members, IDefaultInitializerDefinitionNode? defaultInitializer)
        => new StructDefinitionNode(parent, containingSymbol, inclusiveMembers, isConst, name, supertypes, accessModifier, syntax, attributes, genericParameters, supertypeNames, declaredType, sourceMembers, members, defaultInitializer);
}

// [Closed(typeof(TraitDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitDefinitionNode : ITypeDefinitionNode, ITraitDeclarationNode
{
    new ITraitDefinitionSyntax Syntax { get; }
    ITypeDefinitionSyntax ITypeDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;

    public static ITraitDefinitionNode Create(ISemanticNode parent, Symbol containingSymbol, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, ITraitDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedSet<ITraitMemberDefinitionNode> members)
        => new TraitDefinitionNode(parent, containingSymbol, inclusiveMembers, isConst, name, supertypes, accessModifier, syntax, attributes, genericParameters, supertypeNames, declaredType, members);
}

// [Closed(typeof(GenericParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterNode : ICodeNode, IGenericParameterDeclarationNode
{
    new IGenericParameterSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    TypeParameterIndependence Independence { get; }
    TypeParameterVariance Variance { get; }
    GenericParameter Parameter { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
    GenericParameterType DeclaredType { get; }
    UserTypeSymbol ContainingSymbol { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IUserTypeDeclarationNode ContainingDeclaration { get; }

    public static IGenericParameterNode Create(ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, GenericParameter parameter, IDeclaredUserType containingDeclaredType, GenericParameterType declaredType, UserTypeSymbol containingSymbol, IFixedSet<ITypeMemberDefinitionNode> members)
        => new GenericParameterNode(parent, supertypes, inclusiveMembers, syntax, constraint, name, independence, variance, parameter, containingDeclaredType, declaredType, containingSymbol, members);
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(ITraitMemberDefinitionNode),
    typeof(IStructMemberDefinitionNode),
    typeof(IAlwaysTypeMemberDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberDefinitionNode : IDefinitionNode, ITypeMemberDeclarationNode
{
    new ITypeMemberDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    AccessModifier AccessModifier { get; }
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassMemberDefinitionNode : ITypeMemberDefinitionNode, IClassMemberDeclarationNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitMemberDefinitionNode : ITypeMemberDefinitionNode, ITraitMemberDeclarationNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(IConcreteMethodDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberDefinitionNode : ITypeMemberDefinitionNode, IStructMemberDeclarationNode
{
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAlwaysTypeMemberDefinitionNode : ITypeMemberDefinitionNode
{
    new UserTypeSymbol ContainingSymbol { get; }
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedMemberDefinitionNode : IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, INamedDeclarationNode
{
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IConcreteMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IInvocableDefinitionNode, IMethodDeclarationNode
{
    new IMethodDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    MethodKind Kind { get; }
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

// [Closed(typeof(AbstractMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAbstractMethodDefinitionNode : IMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IAbstractMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ObjectType ContainingDeclaredType { get; }

    public static IAbstractMethodDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, int arity, FunctionType methodGroupType, IAbstractMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, ObjectType containingDeclaredType)
        => new AbstractMethodDefinitionNode(parent, accessModifier, containingSymbol, kind, name, arity, methodGroupType, syntax, selfParameter, parameters, @return, containingDeclaredType);
}

[Closed(
    typeof(IStandardMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConcreteMethodDefinitionNode : IMethodDefinitionNode, IStructMemberDefinitionNode, IConcreteInvocableDefinitionNode
{
    new IConcreteMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;
}

// [Closed(typeof(StandardMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardMethodDefinitionNode : IConcreteMethodDefinitionNode, IStandardMethodDeclarationNode
{
    new IStandardMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;

    public static IStandardMethodDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, int arity, FunctionType methodGroupType, IStandardMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit)
        => new StandardMethodDefinitionNode(parent, accessModifier, containingSymbol, kind, name, variableBindingsMap, arity, methodGroupType, syntax, selfParameter, parameters, @return, entry, body, exit);
}

// [Closed(typeof(GetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDefinitionNode : IConcreteMethodDefinitionNode, IGetterMethodDeclarationNode
{
    new IGetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new ITypeNode Return { get; }
    ITypeNode? IMethodDefinitionNode.Return => Return;

    public static IGetterMethodDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IGetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode @return, IEntryNode entry, IBodyNode body, IExitNode exit)
        => new GetterMethodDefinitionNode(parent, accessModifier, containingSymbol, kind, name, variableBindingsMap, syntax, selfParameter, parameters, @return, entry, body, exit);
}

// [Closed(typeof(SetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDefinitionNode : IConcreteMethodDefinitionNode, ISetterMethodDeclarationNode
{
    new ISetterMethodDefinitionSyntax Syntax { get; }
    IConcreteMethodDefinitionSyntax IConcreteMethodDefinitionNode.Syntax => Syntax;
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;

    public static ISetterMethodDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, ISetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit)
        => new SetterMethodDefinitionNode(parent, accessModifier, containingSymbol, kind, name, variableBindingsMap, syntax, selfParameter, parameters, @return, entry, body, exit);
}

[Closed(
    typeof(IDefaultConstructorDefinitionNode),
    typeof(ISourceConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IConstructorDeclarationNode
{
    new IConstructorDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new ConstructorSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    ConstructorSymbol IConstructorDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{

    public static IDefaultConstructorDefinitionNode Create(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IConstructorDefinitionSyntax? syntax, IdentifierName? name, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit)
        => new DefaultConstructorDefinitionNode(parent, containingSymbol, variableBindingsMap, accessModifier, syntax, name, parameters, entry, body, exit);
}

// [Closed(typeof(SourceConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISourceConstructorDefinitionNode : IConstructorDefinitionNode
{
    new IConstructorDefinitionSyntax Syntax { get; }
    IConstructorDefinitionSyntax? IConstructorDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IConstructorSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;

    public static ISourceConstructorDefinitionNode Create(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IdentifierName? name, IConstructorDefinitionSyntax syntax, IConstructorSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit)
        => new SourceConstructorDefinitionNode(parent, containingSymbol, variableBindingsMap, accessModifier, name, syntax, selfParameter, parameters, entry, body, exit);
}

[Closed(
    typeof(IDefaultInitializerDefinitionNode),
    typeof(ISourceInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerDefinitionNode : IConcreteInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
{
    new IInitializerDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new InitializerSymbol Symbol { get; }
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultInitializerDefinitionNode : IInitializerDefinitionNode
{

    public static IDefaultInitializerDefinitionNode Create(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IInitializerDefinitionSyntax? syntax, IdentifierName? name, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit)
        => new DefaultInitializerDefinitionNode(parent, containingSymbol, variableBindingsMap, accessModifier, syntax, name, parameters, entry, body, exit);
}

// [Closed(typeof(SourceInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISourceInitializerDefinitionNode : IInitializerDefinitionNode
{
    new IInitializerDefinitionSyntax Syntax { get; }
    IInitializerDefinitionSyntax? IInitializerDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IInitializerSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IConcreteInvocableDefinitionNode.Body => Body;

    public static ISourceInitializerDefinitionNode Create(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IdentifierName? name, IInitializerDefinitionSyntax syntax, IInitializerSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit)
        => new SourceInitializerDefinitionNode(parent, containingSymbol, variableBindingsMap, accessModifier, name, syntax, selfParameter, parameters, entry, body, exit);
}

// [Closed(typeof(FieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldDefinitionNode : IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, INamedBindingNode, IFieldDeclarationNode, IExecutableDefinitionNode
{
    new IFieldDefinitionSyntax Syntax { get; }
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new DataType BindingType { get; }
    DataType INamedBindingNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    DataType IFieldDeclarationNode.BindingType => BindingType;
    IAmbiguousExpressionNode? TempInitializer { get; }
    IExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
    new FieldSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FieldSymbol IFieldDeclarationNode.Symbol => Symbol;
    LexicalScope IDefinitionNode.LexicalScope
        => ContainingLexicalScope;

    public static IFieldDefinitionNode Create(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, bool isLentBinding, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IFieldDefinitionSyntax syntax, bool isMutableBinding, IdentifierName name, ITypeNode typeNode, IMaybeAntetype bindingAntetype, DataType bindingType, IEntryNode entry, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer, IExitNode exit)
        => new FieldDefinitionNode(parent, accessModifier, containingSymbol, isLentBinding, variableBindingsMap, syntax, isMutableBinding, name, typeNode, bindingAntetype, bindingType, entry, initializer, currentInitializer, exit);
}

// [Closed(typeof(AssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionDefinitionNode : IConcreteFunctionInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IAssociatedMemberDefinitionNode, IAssociatedFunctionDeclarationNode
{
    new IAssociatedFunctionDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName IAssociatedMemberDefinitionNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    StandardName IAssociatedFunctionDeclarationNode.Name => Name;
    new FunctionType Type { get; }
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
    new FunctionSymbol Symbol { get; }
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;
    InvocableSymbol IInvocableDefinitionNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;

    public static IAssociatedFunctionDefinitionNode Create(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IAssociatedFunctionDefinitionSyntax syntax, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, FunctionType type, IEntryNode entry, IBodyNode body, IExitNode exit)
        => new AssociatedFunctionDefinitionNode(parent, containingSymbol, variableBindingsMap, accessModifier, syntax, name, parameters, @return, type, entry, body, exit);
}

// [Closed(typeof(AttributeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAttributeNode : ICodeNode
{
    new IAttributeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IStandardTypeNameNode TypeName { get; }
    ConstructorSymbol? ReferencedSymbol { get; }

    public static IAttributeNode Create(ISemanticNode parent, IAttributeSyntax syntax, IStandardTypeNameNode typeName)
        => new AttributeNode(parent, syntax, typeName);
}

[Closed(
    typeof(ICapabilitySetNode),
    typeof(ICapabilityNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityConstraintNode : ICodeNode
{
    new ICapabilityConstraintSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraint Constraint { get; }
}

// [Closed(typeof(CapabilitySetNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;

    public static ICapabilitySetNode Create(ISemanticNode parent, ICapabilitySetSyntax syntax, CapabilitySet constraint)
        => new CapabilitySetNode(parent, syntax, constraint);
}

// [Closed(typeof(CapabilityNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    Capability Capability { get; }

    public static ICapabilityNode Create(ISemanticNode parent, ICapabilityConstraint constraint, ICapabilitySyntax syntax, Capability capability)
        => new CapabilityNode(parent, constraint, syntax, capability);
}

[Closed(
    typeof(IConstructorOrInitializerParameterNode),
    typeof(ISelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterNode : ICodeNode
{
    new IParameterSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IdentifierName? Name { get; }
    bool Unused { get; }
    IMaybeAntetype BindingAntetype { get; }
    Pseudotype BindingType { get; }
    IFlowState FlowStateAfter { get; }
    IPreviousValueId PreviousValueId();
    ValueId BindingValueId { get; }
    IFlowState FlowStateBefore();
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IFieldParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorOrInitializerParameterNode : IParameterNode
{
    new IConstructorOrInitializerParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new DataType BindingType { get; }
    Pseudotype IParameterNode.BindingType => BindingType;
    ParameterType ParameterType { get; }
}

// [Closed(typeof(NamedParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedParameterNode : IConstructorOrInitializerParameterNode, ILocalBindingNode
{
    new INamedParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    ITypeNode TypeNode { get; }
    new IMaybeAntetype BindingAntetype { get; }
    IMaybeAntetype IParameterNode.BindingAntetype => BindingAntetype;
    IMaybeAntetype IBindingNode.BindingAntetype => BindingAntetype;
    new ValueId BindingValueId { get; }
    ValueId IParameterNode.BindingValueId => BindingValueId;
    ValueId IBindingNode.BindingValueId => BindingValueId;
    new DataType BindingType { get; }
    DataType IConstructorOrInitializerParameterNode.BindingType => BindingType;
    Pseudotype IParameterNode.BindingType => BindingType;
    DataType INamedBindingNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;

    public static INamedParameterNode Create(ISemanticNode parent, bool unused, IFlowState flowStateAfter, ParameterType parameterType, INamedParameterSyntax syntax, bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeNode typeNode, IMaybeAntetype bindingAntetype, DataType bindingType)
        => new NamedParameterNode(parent, unused, flowStateAfter, parameterType, syntax, isMutableBinding, isLentBinding, name, typeNode, bindingAntetype, bindingType);
}

[Closed(
    typeof(IConstructorSelfParameterNode),
    typeof(IInitializerSelfParameterNode),
    typeof(IMethodSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfParameterNode : IParameterNode, IBindingNode
{
    new ISelfParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSelfParameterNode : ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype ISelfParameterNode.BindingType => BindingType;
    Pseudotype IParameterNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new ObjectType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;

    public static IConstructorSelfParameterNode Create(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, IConstructorSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, ObjectType containingDeclaredType)
        => new ConstructorSelfParameterNode(parent, name, unused, flowStateAfter, containingTypeDefinition, parameterType, bindingAntetype, syntax, isLentBinding, capability, bindingType, containingDeclaredType);
}

// [Closed(typeof(InitializerSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSelfParameterNode : ISelfParameterNode
{
    new IInitializerSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    Pseudotype ISelfParameterNode.BindingType => BindingType;
    Pseudotype IParameterNode.BindingType => BindingType;
    Pseudotype IBindingNode.BindingType => BindingType;
    new StructType ContainingDeclaredType { get; }
    IDeclaredUserType ISelfParameterNode.ContainingDeclaredType => ContainingDeclaredType;

    public static IInitializerSelfParameterNode Create(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, IInitializerSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, StructType containingDeclaredType)
        => new InitializerSelfParameterNode(parent, name, unused, flowStateAfter, containingTypeDefinition, parameterType, bindingAntetype, syntax, isLentBinding, capability, bindingType, containingDeclaredType);
}

// [Closed(typeof(MethodSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSelfParameterNode : ISelfParameterNode
{
    new IMethodSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Capability { get; }

    public static IMethodSelfParameterNode Create(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, IDeclaredUserType containingDeclaredType, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, Pseudotype bindingType, IMethodSelfParameterSyntax syntax, bool isLentBinding, ICapabilityConstraintNode capability)
        => new MethodSelfParameterNode(parent, name, unused, flowStateAfter, containingTypeDefinition, containingDeclaredType, parameterType, bindingAntetype, bindingType, syntax, isLentBinding, capability);
}

// [Closed(typeof(FieldParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldParameterNode : IConstructorOrInitializerParameterNode
{
    new IFieldParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IFieldDefinitionNode? ReferencedField { get; }

    public static IFieldParameterNode Create(ISemanticNode parent, bool unused, IMaybeAntetype bindingAntetype, IFlowState flowStateAfter, DataType bindingType, ParameterType parameterType, IFieldParameterSyntax syntax, IdentifierName name, ITypeDefinitionNode containingTypeDefinition)
        => new FieldParameterNode(parent, unused, bindingAntetype, flowStateAfter, bindingType, parameterType, syntax, name, containingTypeDefinition);
}

[Closed(
    typeof(IBlockBodyNode),
    typeof(IExpressionBodyNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyNode : IBodyOrBlockNode
{
    IFlowState FlowStateAfter { get; }
}

// [Closed(typeof(BlockBodyNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockBodyNode : IBodyNode
{
    new IBlockBodySyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IFixedList<IBodyStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;

    public static IBlockBodyNode Create(ISemanticNode parent, IFlowState flowStateAfter, IBlockBodySyntax syntax, IFixedList<IBodyStatementNode> statements)
        => new BlockBodyNode(parent, flowStateAfter, syntax, statements);
}

// [Closed(typeof(ExpressionBodyNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IResultStatementNode ResultStatement { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements
        => FixedList.Create(ResultStatement);

    public static IExpressionBodyNode Create(ISemanticNode parent, IFlowState flowStateAfter, IExpressionBodySyntax syntax, IResultStatementNode resultStatement, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType)
        => new ExpressionBodyNode(parent, flowStateAfter, syntax, resultStatement, expectedAntetype, expectedType);
}

[Closed(
    typeof(ITypeNameNode),
    typeof(IOptionalTypeNode),
    typeof(ICapabilityTypeNode),
    typeof(IFunctionTypeNode),
    typeof(IViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNode : ICodeNode
{
    new ITypeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeAntetype NamedAntetype { get; }
    DataType NamedType { get; }
}

[Closed(
    typeof(IStandardTypeNameNode),
    typeof(ISimpleTypeNameNode),
    typeof(IQualifiedTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNameNode : ITypeNode
{
    new ITypeNameSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    TypeName Name { get; }
    BareType? NamedBareType { get; }
    LexicalScope ContainingLexicalScope { get; }
    TypeSymbol? ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(IGenericTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardTypeNameNode : ITypeNameNode
{
    new IStandardTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool IsAttributeType { get; }
    new StandardName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    ITypeDeclarationNode? ReferencedDeclaration { get; }
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(ISpecialTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISimpleTypeNameNode : ITypeNameNode
{
    new ISimpleTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(IdentifierTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierTypeNameNode : IStandardTypeNameNode, ISimpleTypeNameNode
{
    new IIdentifierTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
    TypeName ITypeNameNode.Name => Name;

    public static IIdentifierTypeNameNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, bool isAttributeType, IIdentifierTypeNameSyntax syntax, IdentifierName name)
        => new IdentifierTypeNameNode(parent, namedAntetype, namedType, namedBareType, isAttributeType, syntax, name);
}

// [Closed(typeof(SpecialTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameNode : ISimpleTypeNameNode
{
    new ISpecialTypeNameSyntax Syntax { get; }
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new SpecialTypeName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    new TypeSymbol ReferencedSymbol { get; }
    TypeSymbol? ITypeNameNode.ReferencedSymbol => ReferencedSymbol;

    public static ISpecialTypeNameNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, ISpecialTypeNameSyntax syntax, SpecialTypeName name)
        => new SpecialTypeNameNode(parent, namedAntetype, namedType, namedBareType, syntax, name);
}

// [Closed(typeof(GenericTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericTypeNameNode : IStandardTypeNameNode
{
    new IGenericTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardTypeNameNode.Name => Name;
    TypeName ITypeNameNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }

    public static IGenericTypeNameNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, bool isAttributeType, IGenericTypeNameSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
        => new GenericTypeNameNode(parent, namedAntetype, namedType, namedBareType, isAttributeType, syntax, name, typeArguments);
}

// [Closed(typeof(QualifiedTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameNode : ITypeNameNode
{
    new IQualifiedTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameNode Context { get; }
    IStandardTypeNameNode QualifiedName { get; }

    public static IQualifiedTypeNameNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, TypeName name, BareType? namedBareType, IQualifiedTypeNameSyntax syntax, ITypeNameNode context, IStandardTypeNameNode qualifiedName)
        => new QualifiedTypeNameNode(parent, namedAntetype, namedType, name, namedBareType, syntax, context, qualifiedName);
}

// [Closed(typeof(OptionalTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalTypeNode : ITypeNode
{
    new IOptionalTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNode Referent { get; }

    public static IOptionalTypeNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, IOptionalTypeSyntax syntax, ITypeNode referent)
        => new OptionalTypeNode(parent, namedAntetype, namedType, syntax, referent);
}

// [Closed(typeof(CapabilityTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityTypeNode : ITypeNode
{
    new ICapabilityTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    ITypeNode Referent { get; }

    public static ICapabilityTypeNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
        => new CapabilityTypeNode(parent, namedAntetype, namedType, syntax, capability, referent);
}

// [Closed(typeof(FunctionTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionTypeNode : ITypeNode
{
    new IFunctionTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<IParameterTypeNode> Parameters { get; }
    ITypeNode Return { get; }

    public static IFunctionTypeNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, IFunctionTypeSyntax syntax, IFixedList<IParameterTypeNode> parameters, ITypeNode @return)
        => new FunctionTypeNode(parent, namedAntetype, namedType, syntax, parameters, @return);
}

// [Closed(typeof(ParameterTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterTypeNode : ICodeNode
{
    new IParameterTypeSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool IsLent { get; }
    ITypeNode Referent { get; }
    ParameterType Parameter { get; }

    public static IParameterTypeNode Create(ISemanticNode parent, IParameterTypeSyntax syntax, bool isLent, ITypeNode referent, ParameterType parameter)
        => new ParameterTypeNode(parent, syntax, isLent, referent, parameter);
}

[Closed(
    typeof(ICapabilityViewpointTypeNode),
    typeof(ISelfViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IViewpointTypeNode : ITypeNode
{
    new IViewpointTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNode Referent { get; }
}

// [Closed(typeof(CapabilityViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityViewpointTypeNode : IViewpointTypeNode
{
    new ICapabilityViewpointTypeSyntax Syntax { get; }
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }

    public static ICapabilityViewpointTypeNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ICapabilityViewpointTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
        => new CapabilityViewpointTypeNode(parent, namedAntetype, namedType, syntax, capability, referent);
}

// [Closed(typeof(SelfViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfViewpointTypeNode : IViewpointTypeNode
{
    new ISelfViewpointTypeSyntax Syntax { get; }
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    Pseudotype? NamedSelfType { get; }

    public static ISelfViewpointTypeNode Create(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ISelfViewpointTypeSyntax syntax, ITypeNode referent, Pseudotype? namedSelfType)
        => new SelfViewpointTypeNode(parent, namedAntetype, namedType, syntax, referent, namedSelfType);
}

[Closed(
    typeof(IElseClauseNode),
    typeof(IDataFlowNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IControlFlowNode : ICodeNode
{
    ControlFlowSet ControlFlowNext { get; }
    ControlFlowSet ControlFlowPrevious { get; }
    IEntryNode ControlFlowEntry();
    ControlFlowSet ControlFlowFollowing();
}

// [Closed(typeof(EntryNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IEntryNode : IDataFlowNode
{
    FixedDictionary<IVariableBindingNode, int> VariableBindingsMap();

    public static IEntryNode Create(ISemanticNode parent, ICodeSyntax? syntax, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
        => new EntryNode(parent, syntax, controlFlowNext, controlFlowPrevious, dataFlowPrevious, definitelyAssigned, definitelyUnassigned);
}

// [Closed(typeof(ExitNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExitNode : IDataFlowNode
{

    public static IExitNode Create(ISemanticNode parent, ICodeSyntax? syntax, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
        => new ExitNode(parent, syntax, controlFlowNext, controlFlowPrevious, dataFlowPrevious, definitelyAssigned, definitelyUnassigned);
}

[Closed(
    typeof(IVariableBindingNode),
    typeof(IEntryNode),
    typeof(IExitNode),
    typeof(IAssignmentExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDataFlowNode : IControlFlowNode
{
    IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBodyStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStatementNode : IControlFlowNode
{
    new IStatementSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeAntetype? ResultAntetype { get; }
    DataType? ResultType { get; }
    IFlowState FlowStateAfter { get; }
    LexicalScope ContainingLexicalScope();
    LexicalScope LexicalScope()
        => ContainingLexicalScope();
    IPreviousValueId PreviousValueId();
    ValueId? ResultValueId { get; }
}

// [Closed(typeof(ResultStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    DataType? ExpectedType { get; }
    new IFlowState FlowStateAfter { get; }
    IFlowState IStatementNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    ValueId? IStatementNode.ResultValueId
        => ValueId;
    ValueId IElseClauseNode.ValueId
        => Expression?.ValueId ?? default;

    public static IResultStatementNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IMaybeAntetype antetype, DataType type, IResultStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IFlowState flowStateAfter)
        => new ResultStatementNode(parent, controlFlowNext, controlFlowPrevious, resultAntetype, resultType, antetype, type, syntax, expression, currentExpression, expectedAntetype, expectedType, flowStateAfter);
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(VariableDeclarationStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, IVariableBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    ICapabilityNode? Capability { get; }
    ITypeNode? Type { get; }
    IAmbiguousExpressionNode? TempInitializer { get; }
    IExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IStatementNode.ContainingLexicalScope() => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
    new LexicalScope LexicalScope { get; }
    LexicalScope IStatementNode.LexicalScope() => LexicalScope;
    IFlowState FlowStateBefore();
    ValueId? IStatementNode.ResultValueId
        => null;

    public static IVariableDeclarationStatementNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IVariableDeclarationStatementSyntax syntax, bool isMutableBinding, IdentifierName name, ICapabilityNode? capability, ITypeNode? type, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer)
        => new VariableDeclarationStatementNode(parent, controlFlowNext, controlFlowPrevious, resultAntetype, resultType, flowStateAfter, isLentBinding, bindingAntetype, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, name, capability, type, initializer, currentInitializer);
}

// [Closed(typeof(ExpressionStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionStatementNode : IBodyStatementNode
{
    new IExpressionStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    ValueId? IStatementNode.ResultValueId
        => null;

    public static IExpressionStatementNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IFlowState flowStateAfter, IExpressionStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression)
        => new ExpressionStatementNode(parent, controlFlowNext, controlFlowPrevious, resultAntetype, resultType, flowStateAfter, syntax, expression, currentExpression);
}

[Closed(
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternNode : IControlFlowNode
{
    new IPatternSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFlowState FlowStateAfter { get; }
    ConditionalLexicalScope FlowLexicalScope();
    IPreviousValueId PreviousValueId();
    ValueId? MatchReferentValueId { get; }
    IMaybeAntetype ContextBindingAntetype();
    DataType ContextBindingType();
}

// [Closed(typeof(BindingContextPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingContextPatternNode : IPatternNode
{
    new IBindingContextPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool IsMutableBinding { get; }
    IPatternNode Pattern { get; }
    ITypeNode? Type { get; }
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => Pattern.FlowLexicalScope();

    public static IBindingContextPatternNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, IBindingContextPatternSyntax syntax, bool isMutableBinding, IPatternNode pattern, ITypeNode? type)
        => new BindingContextPatternNode(parent, controlFlowNext, controlFlowPrevious, flowStateAfter, syntax, isMutableBinding, pattern, type);
}

[Closed(
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalOrBindingPatternNode : IPatternNode
{
    new IOptionalOrBindingPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(BindingPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingPatternNode : IOptionalOrBindingPatternNode, IVariableBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IFlowState FlowStateBefore();
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => LexicalScopingAspect.BindingPattern_FlowLexicalScope(this);

    public static IBindingPatternNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IBindingPatternSyntax syntax, bool isMutableBinding, IdentifierName name)
        => new BindingPatternNode(parent, controlFlowNext, controlFlowPrevious, flowStateAfter, isLentBinding, bindingAntetype, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, name);
}

// [Closed(typeof(OptionalPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalPatternNode : IOptionalOrBindingPatternNode
{
    new IOptionalPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IOptionalOrBindingPatternNode Pattern { get; }
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => Pattern.FlowLexicalScope();

    public static IOptionalPatternNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern)
        => new OptionalPatternNode(parent, controlFlowNext, controlFlowPrevious, flowStateAfter, syntax, pattern);
}

[Closed(
    typeof(IAmbiguousAssignableExpressionNode),
    typeof(IExpressionNode),
    typeof(IUnresolvedInvocationExpressionNode),
    typeof(IAmbiguousNameExpressionNode),
    typeof(IAmbiguousMoveExpressionNode),
    typeof(IAmbiguousFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousExpressionNode : ICodeNode
{
    new IExpressionSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ConditionalLexicalScope FlowLexicalScope()
        => LexicalScopingAspect.AmbiguousExpression_FlowLexicalScope(this);
    LexicalScope ContainingLexicalScope();
    IPreviousValueId PreviousValueId();
    ValueId ValueId { get; }
}

[Closed(
    typeof(IAssignableExpressionNode),
    typeof(IIdentifierNameExpressionNode),
    typeof(IMemberAccessExpressionNode),
    typeof(IPropertyNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousAssignableExpressionNode : IAmbiguousExpressionNode
{
    new IAssignableExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionNode : IAmbiguousExpressionNode, IControlFlowNode
{
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    IMaybeExpressionAntetype Antetype { get; }
    DataType? ExpectedType { get; }
    DataType Type { get; }
    IFlowState FlowStateAfter { get; }
    bool ImplicitRecoveryAllowed();
    bool ShouldPrepareToReturn();
}

[Closed(
    typeof(IFieldAccessExpressionNode),
    typeof(IVariableNameExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownIdentifierNameExpressionNode),
    typeof(IUnknownMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignableExpressionNode : IExpressionNode, IAmbiguousAssignableExpressionNode
{
}

// [Closed(typeof(BlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    new IFixedList<IStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
    new IMaybeAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;
    IMaybeAntetype IBlockOrResultNode.Antetype => Antetype;
    IFlowState FlowStateBefore();
    new LexicalScope ContainingLexicalScope();
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope();
    LexicalScope IBodyOrBlockNode.ContainingLexicalScope() => ContainingLexicalScope();
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;
    new DataType Type { get; }
    DataType IExpressionNode.Type => Type;
    DataType IBlockOrResultNode.Type => Type;
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;

    public static IBlockExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IBlockExpressionSyntax syntax, IFixedList<IStatementNode> statements, IMaybeAntetype antetype, DataType type, IFlowState flowStateAfter)
        => new BlockExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, expectedType, syntax, statements, antetype, type, flowStateAfter);
}

// [Closed(typeof(NewObjectExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INewObjectExpressionNode : IInvocationExpressionNode
{
    new INewObjectExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameNode ConstructingType { get; }
    IdentifierName? ConstructorName { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IMaybeAntetype ConstructingAntetype { get; }
    IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { get; }
    IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { get; }
    IConstructorDeclarationNode? ReferencedConstructor { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
    PackageNameScope PackageNameScope();
    IFlowState FlowStateBefore();

    public static INewObjectExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, INewObjectExpressionSyntax syntax, ITypeNameNode constructingType, IdentifierName? constructorName, IFixedList<IAmbiguousExpressionNode> arguments, IMaybeAntetype constructingAntetype, IFixedSet<IConstructorDeclarationNode> referencedConstructors, IFixedSet<IConstructorDeclarationNode> compatibleConstructors, IConstructorDeclarationNode? referencedConstructor, ContextualizedOverload? contextualizedOverload)
        => new NewObjectExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, constructingType, constructorName, arguments, constructingAntetype, referencedConstructors, compatibleConstructors, referencedConstructor, contextualizedOverload);
}

// [Closed(typeof(UnsafeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnsafeExpressionNode : IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }

    public static IUnsafeExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IUnsafeExpressionSyntax syntax, IAmbiguousExpressionNode expression)
        => new UnsafeExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, expression);
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILiteralExpressionNode : IExpressionNode
{
    new ILiteralExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFlowState FlowStateBefore();
}

// [Closed(typeof(BoolLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBoolLiteralExpressionNode : ILiteralExpressionNode
{
    new IBoolLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool Value { get; }
    new BoolConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;

    public static IBoolLiteralExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IBoolLiteralExpressionSyntax syntax, bool value, BoolConstValueType type)
        => new BoolLiteralExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, value, type);
}

// [Closed(typeof(IntegerLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    new IIntegerLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    BigInteger Value { get; }
    new IntegerConstValueType Type { get; }
    DataType IExpressionNode.Type => Type;

    public static IIntegerLiteralExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IIntegerLiteralExpressionSyntax syntax, BigInteger value, IntegerConstValueType type)
        => new IntegerLiteralExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, value, type);
}

// [Closed(typeof(NoneLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    new INoneLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new OptionalType Type { get; }
    DataType IExpressionNode.Type => Type;

    public static INoneLiteralExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, INoneLiteralExpressionSyntax syntax, OptionalType type)
        => new NoneLiteralExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, type);
}

// [Closed(typeof(StringLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    new IStringLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    string Value { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;

    public static IStringLiteralExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IStringLiteralExpressionSyntax syntax, string value, DataType type)
        => new StringLiteralExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, value, type);
}

// [Closed(typeof(AssignmentExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignmentExpressionNode : IExpressionNode, IDataFlowNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousAssignableExpressionNode TempLeftOperand { get; }
    IAssignableExpressionNode? LeftOperand { get; }
    IAmbiguousAssignableExpressionNode CurrentLeftOperand { get; }
    AssignmentOperator Operator { get; }
    IAmbiguousExpressionNode TempRightOperand { get; }
    IExpressionNode? RightOperand { get; }
    IAmbiguousExpressionNode CurrentRightOperand { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.AssignmentExpression_FlowLexicalScope(this);

    public static IAssignmentExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IAssignmentExpressionSyntax syntax, IAmbiguousAssignableExpressionNode leftOperand, IAmbiguousAssignableExpressionNode currentLeftOperand, AssignmentOperator @operator, IAmbiguousExpressionNode rightOperand, IAmbiguousExpressionNode currentRightOperand)
        => new AssignmentExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, leftOperand, currentLeftOperand, @operator, rightOperand, currentRightOperand);
}

// [Closed(typeof(BinaryOperatorExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBinaryOperatorExpressionNode : IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempLeftOperand { get; }
    IExpressionNode? LeftOperand { get; }
    BinaryOperator Operator { get; }
    IAmbiguousExpressionNode TempRightOperand { get; }
    IExpressionNode? RightOperand { get; }
    IAntetype? NumericOperatorCommonAntetype { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.BinaryOperatorExpression_FlowLexicalScope(this);

    public static IBinaryOperatorExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IBinaryOperatorExpressionSyntax syntax, IAmbiguousExpressionNode leftOperand, BinaryOperator @operator, IAmbiguousExpressionNode rightOperand, IAntetype? numericOperatorCommonAntetype)
        => new BinaryOperatorExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, leftOperand, @operator, rightOperand, numericOperatorCommonAntetype);
}

// [Closed(typeof(UnaryOperatorExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnaryOperatorExpressionNode : IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IAmbiguousExpressionNode TempOperand { get; }
    IExpressionNode? Operand { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.UnaryOperatorExpression_FlowLexicalScope(this);

    public static IUnaryOperatorExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IUnaryOperatorExpressionSyntax syntax, UnaryOperatorFixity fixity, UnaryOperator @operator, IAmbiguousExpressionNode operand)
        => new UnaryOperatorExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, fixity, @operator, operand);
}

// [Closed(typeof(IdExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdExpressionNode : IExpressionNode
{
    new IIdExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempReferent { get; }
    IExpressionNode? Referent { get; }

    public static IIdExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent)
        => new IdExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, referent);
}

// [Closed(typeof(ConversionExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConversionExpressionNode : IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempReferent { get; }
    IExpressionNode? Referent { get; }
    ConversionOperator Operator { get; }
    ITypeNode ConvertToType { get; }

    public static IConversionExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IConversionExpressionSyntax syntax, IAmbiguousExpressionNode referent, ConversionOperator @operator, ITypeNode convertToType)
        => new ConversionExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, referent, @operator, convertToType);
}

// [Closed(typeof(ImplicitConversionExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitConversionExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    new SimpleAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;

    public static IImplicitConversionExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode referent, IExpressionNode currentReferent, SimpleAntetype antetype)
        => new ImplicitConversionExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, expectedType, type, flowStateAfter, referent, currentReferent, antetype);
}

// [Closed(typeof(PatternMatchExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternMatchExpressionNode : IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempReferent { get; }
    IExpressionNode? Referent { get; }
    IPatternNode Pattern { get; }

    public static IPatternMatchExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IPatternMatchExpressionSyntax syntax, IAmbiguousExpressionNode referent, IPatternNode pattern)
        => new PatternMatchExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, referent, pattern);
}

// [Closed(typeof(IfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempCondition { get; }
    IExpressionNode? Condition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;

    public static IIfExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IIfExpressionSyntax syntax, IAmbiguousExpressionNode condition, IBlockOrResultNode thenBlock, IElseClauseNode? elseClause, IFlowState flowStateAfter)
        => new IfExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, syntax, condition, thenBlock, elseClause, flowStateAfter);
}

// [Closed(typeof(LoopExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILoopExpressionNode : IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }

    public static ILoopExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ILoopExpressionSyntax syntax, IBlockExpressionNode block)
        => new LoopExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, block);
}

// [Closed(typeof(WhileExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IWhileExpressionNode : IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempCondition { get; }
    IExpressionNode? Condition { get; }
    IBlockExpressionNode Block { get; }

    public static IWhileExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IWhileExpressionSyntax syntax, IAmbiguousExpressionNode condition, IBlockExpressionNode block)
        => new WhileExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, condition, block);
}

// [Closed(typeof(ForeachExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IForeachExpressionNode : IExpressionNode, IVariableBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IdentifierName VariableName { get; }
    IAmbiguousExpressionNode TempInExpression { get; }
    IExpressionNode? InExpression { get; }
    ITypeNode? DeclaredType { get; }
    IBlockExpressionNode Block { get; }
    ITypeDeclarationNode? ReferencedIterableDeclaration { get; }
    IStandardMethodDeclarationNode? ReferencedIterateMethod { get; }
    IMaybeExpressionAntetype IteratorAntetype { get; }
    DataType IteratorType { get; }
    ITypeDeclarationNode? ReferencedIteratorDeclaration { get; }
    IStandardMethodDeclarationNode? ReferencedNextMethod { get; }
    IMaybeAntetype IteratedAntetype { get; }
    DataType IteratedType { get; }
    IFlowState FlowStateBeforeBlock { get; }
    PackageNameScope PackageNameScope();
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
    LexicalScope LexicalScope { get; }

    public static IForeachExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, IdentifierName name, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IForeachExpressionSyntax syntax, bool isMutableBinding, IdentifierName variableName, IAmbiguousExpressionNode inExpression, ITypeNode? declaredType, IBlockExpressionNode block, ITypeDeclarationNode? referencedIterableDeclaration, IStandardMethodDeclarationNode? referencedIterateMethod, IMaybeExpressionAntetype iteratorAntetype, DataType iteratorType, ITypeDeclarationNode? referencedIteratorDeclaration, IStandardMethodDeclarationNode? referencedNextMethod, IMaybeAntetype iteratedAntetype, DataType iteratedType, IFlowState flowStateBeforeBlock)
        => new ForeachExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, isLentBinding, bindingAntetype, name, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, variableName, inExpression, declaredType, block, referencedIterableDeclaration, referencedIterateMethod, iteratorAntetype, iteratorType, referencedIteratorDeclaration, referencedNextMethod, iteratedAntetype, iteratedType, flowStateBeforeBlock);
}

// [Closed(typeof(BreakExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode? TempValue { get; }
    IExpressionNode? Value { get; }

    public static IBreakExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, IBreakExpressionSyntax syntax, IAmbiguousExpressionNode? value)
        => new BreakExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, syntax, value);
}

// [Closed(typeof(NextExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INextExpressionNode : INeverTypedExpressionNode
{
    new INextExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;

    public static INextExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, INextExpressionSyntax syntax)
        => new NextExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, syntax);
}

// [Closed(typeof(ReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnExpressionNode : INeverTypedExpressionNode
{
    new IReturnExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode? TempValue { get; }
    IExpressionNode? Value { get; }
    IAmbiguousExpressionNode? CurrentValue { get; }
    IExitNode ControlFlowExit();
    DataType? ExpectedReturnType { get; }

    public static IReturnExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, IReturnExpressionSyntax syntax, IAmbiguousExpressionNode? value, IAmbiguousExpressionNode? currentValue)
        => new ReturnExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, syntax, value, currentValue);
}

// [Closed(typeof(UnresolvedInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnresolvedInvocationExpressionNode : IAmbiguousExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }

    public static IUnresolvedInvocationExpressionNode Create(ISemanticNode parent, IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments)
        => new UnresolvedInvocationExpressionNode(parent, syntax, expression, currentExpression, arguments, currentArguments);
}

[Closed(
    typeof(INewObjectExpressionNode),
    typeof(IFunctionInvocationExpressionNode),
    typeof(IMethodInvocationExpressionNode),
    typeof(IGetterInvocationExpressionNode),
    typeof(ISetterInvocationExpressionNode),
    typeof(IFunctionReferenceInvocationExpressionNode),
    typeof(IInitializerInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocationExpressionNode : IExpressionNode
{
    IEnumerable<IAmbiguousExpressionNode> TempAllArguments { get; }
    IEnumerable<IExpressionNode?> AllArguments { get; }
}

// [Closed(typeof(FunctionInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFunctionGroupNameNode FunctionGroup { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
    IFlowState FlowStateBefore();

    public static IFunctionInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IFunctionGroupNameNode functionGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedSet<IFunctionLikeDeclarationNode> compatibleDeclarations, IFunctionLikeDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
        => new FunctionInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, functionGroup, arguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
}

// [Closed(typeof(MethodInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodGroupNameNode MethodGroup { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { get; }
    IStandardMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }

    public static IMethodInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IMethodGroupNameNode methodGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments, IFixedSet<IStandardMethodDeclarationNode> compatibleDeclarations, IStandardMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
        => new MethodInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, methodGroup, arguments, currentArguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
}

// [Closed(typeof(GetterInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterInvocationExpressionNode : IInvocationExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }

    public static IGetterInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, IGetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
        => new GetterInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, context, propertyName, referencedPropertyAccessors, referencedDeclaration, contextualizedOverload);
}

// [Closed(typeof(SetterInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterInvocationExpressionNode : IInvocationExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IAmbiguousExpressionNode TempValue { get; }
    IExpressionNode? Value { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }

    public static ISetterInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IAssignmentExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IAmbiguousExpressionNode value, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, ISetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
        => new SetterInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, context, propertyName, value, referencedPropertyAccessors, referencedDeclaration, contextualizedOverload);
}

// [Closed(typeof(FunctionReferenceInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionReferenceInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionNode Expression { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    FunctionAntetype FunctionAntetype { get; }
    FunctionType FunctionType { get; }

    public static IFunctionReferenceInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments, FunctionAntetype functionAntetype, FunctionType functionType)
        => new FunctionReferenceInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, expression, arguments, functionAntetype, functionType);
}

// [Closed(typeof(InitializerInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IInitializerGroupNameNode InitializerGroup { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; }
    IInitializerDeclarationNode? ReferencedDeclaration { get; }
    ContextualizedOverload? ContextualizedOverload { get; }
    IFlowState FlowStateBefore();

    public static IInitializerInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IInitializerGroupNameNode initializerGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedSet<IInitializerDeclarationNode> compatibleDeclarations, IInitializerDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
        => new InitializerInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, tempAllArguments, allArguments, syntax, initializerGroup, arguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
}

// [Closed(typeof(UnknownInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownInvocationExpressionNode : IExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }

    public static IUnknownInvocationExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments)
        => new UnknownInvocationExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, expression, arguments);
}

[Closed(
    typeof(IAmbiguousNameNode),
    typeof(INameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousNameExpressionNode : IAmbiguousExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

[Closed(
    typeof(IUnresolvedSimpleNameNode),
    typeof(IStandardNameExpressionNode),
    typeof(IMemberAccessExpressionNode),
    typeof(IPropertyNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousNameNode : IAmbiguousNameExpressionNode
{
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(ISimpleNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnresolvedSimpleNameNode : IAmbiguousNameNode
{
    new ISimpleNameSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

[Closed(
    typeof(IIdentifierNameExpressionNode),
    typeof(IGenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardNameExpressionNode : IAmbiguousNameNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    StandardName Name { get; }
    IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
}

// [Closed(typeof(IdentifierNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierNameExpressionNode : IStandardNameExpressionNode, IUnresolvedSimpleNameNode, IAmbiguousAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;

    public static IIdentifierNameExpressionNode Create(ISemanticNode parent, IFixedList<IDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
        => new IdentifierNameExpressionNode(parent, referencedDeclarations, syntax, name);
}

// [Closed(typeof(GenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericNameExpressionNode : IStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }

    public static IGenericNameExpressionNode Create(ISemanticNode parent, IFixedList<IDeclarationNode> referencedDeclarations, IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
        => new GenericNameExpressionNode(parent, referencedDeclarations, syntax, name, typeArguments);
}

// [Closed(typeof(MemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMemberAccessExpressionNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempContext { get; }
    IExpressionNode? Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    PackageNameScope PackageNameScope();

    public static IMemberAccessExpressionNode Create(ISemanticNode parent, IMemberAccessExpressionSyntax syntax, IAmbiguousExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments)
        => new MemberAccessExpressionNode(parent, syntax, context, memberName, typeArguments);
}

// [Closed(typeof(PropertyNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPropertyNameNode : IAmbiguousNameNode, IAmbiguousAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }

    public static IPropertyNameNode Create(ISemanticNode parent, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
        => new PropertyNameNode(parent, syntax, context, propertyName, referencedPropertyAccessors);
}

[Closed(
    typeof(ILocalBindingNameExpressionNode),
    typeof(ISimpleNameExpressionNode),
    typeof(INamespaceNameNode),
    typeof(IFunctionGroupNameNode),
    typeof(IFunctionNameNode),
    typeof(IMethodGroupNameNode),
    typeof(IFieldAccessExpressionNode),
    typeof(ITypeNameExpressionNode),
    typeof(IInitializerGroupNameNode),
    typeof(ISpecialTypeNameExpressionNode),
    typeof(IUnknownNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INameExpressionNode : IExpressionNode, IAmbiguousNameExpressionNode
{
}

[Closed(
    typeof(IVariableNameExpressionNode),
    typeof(ISelfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILocalBindingNameExpressionNode : INameExpressionNode
{
    IBindingNode? ReferencedDefinition { get; }
}

[Closed(
    typeof(IVariableNameExpressionNode),
    typeof(IInstanceExpressionNode),
    typeof(IMissingNameExpressionNode),
    typeof(IUnknownIdentifierNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISimpleNameExpressionNode : INameExpressionNode, IUnresolvedSimpleNameNode
{
}

[Closed(
    typeof(IUnqualifiedNamespaceNameNode),
    typeof(IQualifiedNamespaceNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceNameNode : INameExpressionNode
{
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
    IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(UnqualifiedNamespaceNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnqualifiedNamespaceNameNode : INamespaceNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IdentifierName Name { get; }

    public static IUnqualifiedNamespaceNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
        => new UnqualifiedNamespaceNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, referencedDeclarations, syntax, name);
}

// [Closed(typeof(QualifiedNamespaceNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedNamespaceNameNode : INamespaceNameNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
    IdentifierName Name { get; }

    public static IQualifiedNamespaceNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IdentifierName name)
        => new QualifiedNamespaceNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, referencedDeclarations, syntax, context, name);
}

// [Closed(typeof(FunctionGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionGroupNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }

    public static IFunctionGroupNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, INameExpressionNode? context, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFixedSet<IFunctionLikeDeclarationNode> referencedDeclarations)
        => new FunctionGroupNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, context, functionName, typeArguments, referencedDeclarations);
}

// [Closed(typeof(FunctionNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionNameNode : INameExpressionNode
{
    IFunctionGroupNameNode FunctionGroup { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFunctionLikeDeclarationNode? ReferencedDeclaration { get; }
    IFlowState FlowStateBefore();

    public static IFunctionNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, IFunctionGroupNameNode functionGroup, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFunctionLikeDeclarationNode? referencedDeclaration)
        => new FunctionNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, functionGroup, functionName, typeArguments, referencedDeclaration);
}

// [Closed(typeof(MethodGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    StandardName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }

    public static IMethodGroupNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IMemberAccessExpressionSyntax syntax, IExpressionNode context, IExpressionNode currentContext, StandardName methodName, IFixedList<ITypeNode> typeArguments, IFixedSet<IStandardMethodDeclarationNode> referencedDeclarations)
        => new MethodGroupNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, context, currentContext, methodName, typeArguments, referencedDeclarations);
}

// [Closed(typeof(FieldAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldAccessExpressionNode : INameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IdentifierName FieldName { get; }
    IFieldDeclarationNode ReferencedDeclaration { get; }

    public static IFieldAccessExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IMemberAccessExpressionSyntax syntax, IExpressionNode context, IdentifierName fieldName, IFieldDeclarationNode referencedDeclaration)
        => new FieldAccessExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, context, fieldName, referencedDeclaration);
}

// [Closed(typeof(VariableNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableNameExpressionNode : ILocalBindingNameExpressionNode, IAssignableExpressionNode, ISimpleNameExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IdentifierName Name { get; }
    new ILocalBindingNode ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    IFlowState FlowStateBefore();

    public static IVariableNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IIdentifierNameExpressionSyntax syntax, IdentifierName name, ILocalBindingNode referencedDefinition, IFixedSet<IDataFlowNode> dataFlowPrevious)
        => new VariableNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, name, referencedDefinition, dataFlowPrevious);
}

[Closed(
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNameExpressionNode : INameExpressionNode
{
    StandardName Name { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    ITypeDeclarationNode ReferencedDeclaration { get; }
    IMaybeAntetype NamedAntetype { get; }
    BareType? NamedBareType { get; }
}

// [Closed(typeof(StandardTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;

    public static IStandardTypeNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IStandardNameExpressionSyntax syntax, IFixedList<ITypeNode> typeArguments)
        => new StandardTypeNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, name, referencedDeclaration, namedAntetype, namedBareType, syntax, typeArguments);
}

// [Closed(typeof(QualifiedTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }

    public static IQualifiedTypeNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IFixedList<ITypeNode> typeArguments)
        => new QualifiedTypeNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, name, referencedDeclaration, namedAntetype, namedBareType, syntax, context, typeArguments);
}

// [Closed(typeof(InitializerGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerGroupNameNode : INameExpressionNode
{
    ITypeNameExpressionNode Context { get; }
    StandardName? InitializerName { get; }
    IMaybeAntetype InitializingAntetype { get; }
    IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }

    public static IInitializerGroupNameNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, ITypeNameExpressionNode context, StandardName? initializerName, IMaybeAntetype initializingAntetype, IFixedSet<IInitializerDeclarationNode> referencedDeclarations)
        => new InitializerGroupNameNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, context, initializerName, initializingAntetype, referencedDeclarations);
}

// [Closed(typeof(SpecialTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameExpressionNode : INameExpressionNode
{
    new ISpecialTypeNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    SpecialTypeName Name { get; }
    TypeSymbol ReferencedSymbol { get; }
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;

    public static ISpecialTypeNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ISpecialTypeNameExpressionSyntax syntax, SpecialTypeName name, TypeSymbol referencedSymbol, UnknownType type)
        => new SpecialTypeNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, name, referencedSymbol, type);
}

[Closed(
    typeof(ISelfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInstanceExpressionNode : ISimpleNameExpressionNode
{
    new IInstanceExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
}

// [Closed(typeof(SelfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfExpressionNode : IInstanceExpressionNode, ILocalBindingNameExpressionNode
{
    new ISelfExpressionSyntax Syntax { get; }
    IInstanceExpressionSyntax IInstanceExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    bool IsImplicit { get; }
    Pseudotype Pseudotype { get; }
    new ISelfParameterNode? ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    IFlowState FlowStateBefore();
    IExecutableDefinitionNode ContainingDeclaration { get; }

    public static ISelfExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ISelfExpressionSyntax syntax, bool isImplicit, Pseudotype pseudotype, ISelfParameterNode? referencedDefinition)
        => new SelfExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, isImplicit, pseudotype, referencedDefinition);
}

// [Closed(typeof(MissingNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMissingNameExpressionNode : ISimpleNameExpressionNode, IAssignableExpressionNode
{
    new IMissingNameSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;

    public static IMissingNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IMissingNameSyntax syntax, UnknownType type)
        => new MissingNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, syntax, type);
}

[Closed(
    typeof(IUnknownStandardNameExpressionNode),
    typeof(IUnknownMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownNameExpressionNode : INameExpressionNode
{
    new UnknownType Type { get; }
    DataType IExpressionNode.Type => Type;
}

[Closed(
    typeof(IUnknownIdentifierNameExpressionNode),
    typeof(IUnknownGenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownStandardNameExpressionNode : IUnknownNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    StandardName Name { get; }
    IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(UnknownIdentifierNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownIdentifierNameExpressionNode : IUnknownStandardNameExpressionNode, ISimpleNameExpressionNode, IAssignableExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;

    public static IUnknownIdentifierNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedSet<IDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
        => new UnknownIdentifierNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, referencedDeclarations, syntax, name);
}

// [Closed(typeof(UnknownGenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownGenericNameExpressionNode : IUnknownStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    StandardName IUnknownStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }

    public static IUnknownGenericNameExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedSet<IDeclarationNode> referencedDeclarations, IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
        => new UnknownGenericNameExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, referencedDeclarations, syntax, name, typeArguments);
}

// [Closed(typeof(UnknownMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownMemberAccessExpressionNode : IUnknownNameExpressionNode, IAssignableExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAssignableExpressionSyntax IAmbiguousAssignableExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IDeclarationNode> ReferencedMembers { get; }

    public static IUnknownMemberAccessExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments, IFixedSet<IDeclarationNode> referencedMembers)
        => new UnknownMemberAccessExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, flowStateAfter, type, syntax, context, memberName, typeArguments, referencedMembers);
}

// [Closed(typeof(AmbiguousMoveExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousMoveExpressionNode : IAmbiguousExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IUnresolvedSimpleNameNode TempReferent { get; }
    ISimpleNameExpressionNode? Referent { get; }

    public static IAmbiguousMoveExpressionNode Create(ISemanticNode parent, IMoveExpressionSyntax syntax, IUnresolvedSimpleNameNode referent)
        => new AmbiguousMoveExpressionNode(parent, syntax, referent);
}

[Closed(
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IRecoveryExpressionNode : IExpressionNode
{
    bool IsImplicit { get; }
}

[Closed(
    typeof(IMoveVariableExpressionNode),
    typeof(IMoveValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveExpressionNode : IRecoveryExpressionNode
{
    IExpressionNode Referent { get; }
}

// [Closed(typeof(MoveVariableExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveVariableExpressionNode : IMoveExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IMoveExpressionNode.Referent => Referent;

    public static IMoveVariableExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, ILocalBindingNameExpressionNode referent)
        => new MoveVariableExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, isImplicit, referent);
}

// [Closed(typeof(MoveValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveValueExpressionNode : IMoveExpressionNode
{

    public static IMoveValueExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, IExpressionNode referent)
        => new MoveValueExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, isImplicit, referent);
}

// [Closed(typeof(ImplicitTempMoveExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitTempMoveExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }

    public static IImplicitTempMoveExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode referent)
        => new ImplicitTempMoveExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, referent);
}

// [Closed(typeof(AmbiguousFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousFreezeExpressionNode : IAmbiguousExpressionNode
{
    new IFreezeExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IUnresolvedSimpleNameNode TempReferent { get; }
    ISimpleNameExpressionNode? Referent { get; }

    public static IAmbiguousFreezeExpressionNode Create(ISemanticNode parent, IFreezeExpressionSyntax syntax, IUnresolvedSimpleNameNode referent)
        => new AmbiguousFreezeExpressionNode(parent, syntax, referent);
}

[Closed(
    typeof(IFreezeVariableExpressionNode),
    typeof(IFreezeValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeExpressionNode : IRecoveryExpressionNode
{
    IExpressionNode Referent { get; }
    bool IsTemporary { get; }
}

// [Closed(typeof(FreezeVariableExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeVariableExpressionNode : IFreezeExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    IExpressionNode IFreezeExpressionNode.Referent => Referent;

    public static IFreezeVariableExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, bool isTemporary, ILocalBindingNameExpressionNode referent)
        => new FreezeVariableExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, isImplicit, isTemporary, referent);
}

// [Closed(typeof(FreezeValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeValueExpressionNode : IFreezeExpressionNode
{

    public static IFreezeValueExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, bool isTemporary, IExpressionNode referent)
        => new FreezeValueExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, isImplicit, isTemporary, referent);
}

// [Closed(typeof(PrepareToReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrepareToReturnExpressionNode : IExpressionNode
{
    IExpressionNode Value { get; }
    IExpressionNode CurrentValue { get; }

    public static IPrepareToReturnExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode value, IExpressionNode currentValue)
        => new PrepareToReturnExpressionNode(parent, controlFlowNext, controlFlowPrevious, syntax, expectedAntetype, antetype, expectedType, type, flowStateAfter, value, currentValue);
}

// [Closed(typeof(AsyncBlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncBlockExpressionNode : IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }

    public static IAsyncBlockExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAsyncBlockExpressionSyntax syntax, IBlockExpressionNode block)
        => new AsyncBlockExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, block);
}

// [Closed(typeof(AsyncStartExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncStartExpressionNode : IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool Scheduled { get; }
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }

    public static IAsyncStartExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAsyncStartExpressionSyntax syntax, bool scheduled, IAmbiguousExpressionNode expression)
        => new AsyncStartExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, scheduled, expression);
}

// [Closed(typeof(AwaitExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAwaitExpressionNode : IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }

    public static IAwaitExpressionNode Create(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAwaitExpressionSyntax syntax, IAmbiguousExpressionNode expression)
        => new AwaitExpressionNode(parent, controlFlowNext, controlFlowPrevious, expectedAntetype, antetype, expectedType, type, flowStateAfter, syntax, expression);
}

[Closed(
    typeof(IChildDeclarationNode),
    typeof(ISymbolDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDeclarationNode : ISemanticNode
{
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IInvocableDeclarationNode),
    typeof(IBindingDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISymbolDeclarationNode : IDeclarationNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(IFunctionLikeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDeclarationNode : ISymbolDeclarationNode, IChildDeclarationNode
{
    new InvocableSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IBindingNode),
    typeof(INamedBindingDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingDeclarationNode : IChildDeclarationNode
{
}

[Closed(
    typeof(INamedBindingNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedBindingDeclarationNode : IBindingDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IPackageNode),
    typeof(IPackageSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageMemberDeclarationNode : INamespaceMemberDeclarationNode
{
}

[Closed(
    typeof(IPackageFacetNode),
    typeof(IPackageFacetSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetChildDeclarationNode : IChildDeclarationNode
{
    StandardName? Name { get; }
    IPackageFacetDeclarationNode Facet { get; }
}

[Closed(
    typeof(INamespaceDefinitionNode),
    typeof(INamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceDeclarationNode : INamespaceMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
    FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName { get; }
    FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName { get; }
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named)
        => MembersByName.GetValueOrDefault(named) ?? [];
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembersByName.GetValueOrDefault(named) ?? [];
}

[Closed(
    typeof(INamespaceMemberDefinitionNode),
    typeof(IPackageMemberDeclarationNode),
    typeof(INamespaceDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberDeclarationNode : IPackageFacetChildDeclarationNode, INamedDeclarationNode, ISymbolDeclarationNode
{
    new StandardName Name { get; }
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionLikeDeclarationNode : INamedDeclarationNode, IInvocableDeclarationNode
{
    new FunctionSymbol Symbol { get; }
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionType Type { get; }
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode, IFunctionLikeDeclarationNode
{
}

[Closed(
    typeof(IPrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrimitiveTypeDeclarationNode : ITypeDeclarationNode
{
    new SpecialTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName { get; }
    FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName { get; }
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(StandardName named)
        => InclusiveInstanceMembersByName.GetValueOrDefault(named) ?? [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(StandardName named)
        => AssociatedMembersByName.GetValueOrDefault(named) ?? [];
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IUserTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUserTypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ITypeDeclarationNode
{
    IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName { get; }
    FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName { get; }
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(StandardName named)
        => InclusiveInstanceMembersByName.GetValueOrDefault(named) ?? [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(StandardName named)
        => AssociatedMembersByName.GetValueOrDefault(named) ?? [];
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IClassSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<IClassMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers => InclusiveMembers;
}

[Closed(
    typeof(IStructDefinitionNode),
    typeof(IStructSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<IStructMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers => InclusiveMembers;
}

[Closed(
    typeof(ITraitDefinitionNode),
    typeof(ITraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitDeclarationNode : IUserTypeDeclarationNode
{
    new IFixedSet<ITraitMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers => InclusiveMembers;
}

[Closed(
    typeof(IGenericParameterNode),
    typeof(IGenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterDeclarationNode : ITypeDeclarationNode, IAssociatedMemberDeclarationNode
{
    new IdentifierName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    new GenericParameterTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(StandardName named)
        => [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(StandardName named)
        => [];
}

[Closed(
    typeof(ITypeMemberDefinitionNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IInstanceMemberDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberDeclarationNode : IPackageFacetChildDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(ITraitMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IStructMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IGenericParameterDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedMemberDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInstanceMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IMethodDefinitionNode),
    typeof(IStandardMethodDeclarationNode),
    typeof(IPropertyAccessorDeclarationNode),
    typeof(IMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardMethodDeclarationNode : IMethodDeclarationNode
{
    int Arity { get; }
    FunctionType MethodGroupType { get; }
}

[Closed(
    typeof(IGetterMethodDeclarationNode),
    typeof(ISetterMethodDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPropertyAccessorDeclarationNode : IMethodDeclarationNode
{
}

[Closed(
    typeof(IGetterMethodDefinitionNode),
    typeof(IGetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDeclarationNode : IPropertyAccessorDeclarationNode
{
}

[Closed(
    typeof(ISetterMethodDefinitionNode),
    typeof(ISetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDeclarationNode : IPropertyAccessorDeclarationNode
{
}

[Closed(
    typeof(IConstructorDefinitionNode),
    typeof(IConstructorSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDeclarationNode : INamedDeclarationNode, ISymbolDeclarationNode
{
    new TypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IFixedSet<BareReferenceType> Supertypes { get; }
    IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
    IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named);
    IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named);
}

// [Closed(typeof(PackageSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageSymbolNode : IPackageDeclarationNode
{

    public static IPackageSymbolNode Create(ISyntax? syntax, IdentifierName? aliasOrName, IdentifierName name, PackageSymbol symbol, IPackageFacetDeclarationNode mainFacet, IPackageFacetDeclarationNode testingFacet)
        => new PackageSymbolNode(syntax, aliasOrName, name, symbol, mainFacet, testingFacet);
}

// [Closed(typeof(PackageFacetSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetSymbolNode : IPackageFacetDeclarationNode
{

    public static IPackageFacetSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName? packageAliasOrName, IdentifierName packageName, PackageSymbol symbol, INamespaceDeclarationNode globalNamespace)
        => new PackageFacetSymbolNode(syntax, parent, packageAliasOrName, packageName, symbol, globalNamespace);
}

// [Closed(typeof(NamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceSymbolNode : INamespaceDeclarationNode
{
    new IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;

    public static INamespaceSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName name, NamespaceSymbol symbol, IFixedList<INamespaceMemberDeclarationNode> nestedMembers, IFixedList<INamespaceMemberDeclarationNode> members)
        => new NamespaceSymbolNode(syntax, parent, name, symbol, nestedMembers, members);
}

// [Closed(typeof(FunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionSymbolNode : IFunctionDeclarationNode
{

    public static IFunctionSymbolNode Create(ISyntax? syntax, ISemanticNode parent, FunctionSymbol symbol, StandardName name, FunctionType type)
        => new FunctionSymbolNode(syntax, parent, symbol, name, type);
}

// [Closed(typeof(PrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrimitiveTypeSymbolNode : IPrimitiveTypeDeclarationNode
{

    public static IPrimitiveTypeSymbolNode Create(ISyntax? syntax, ISemanticNode parent, TypeSymbol symbol, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, SpecialTypeName name, IFixedSet<ITypeMemberDeclarationNode> members)
        => new PrimitiveTypeSymbolNode(syntax, parent, symbol, supertypes, inclusiveMembers, name, members);
}

// [Closed(typeof(UserTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUserTypeSymbolNode : IUserTypeDeclarationNode
{

    public static IUserTypeSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITypeMemberDeclarationNode> members)
        => new UserTypeSymbolNode(syntax, parent, supertypes, name, symbol, inclusiveMembers, genericParameters, members);
}

// [Closed(typeof(ClassSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassSymbolNode : IClassDeclarationNode
{

    public static IClassSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IClassMemberDeclarationNode> members)
        => new ClassSymbolNode(syntax, parent, supertypes, name, symbol, inclusiveMembers, genericParameters, members);
}

// [Closed(typeof(StructSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructSymbolNode : IStructDeclarationNode
{

    public static IStructSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IStructMemberDeclarationNode> members)
        => new StructSymbolNode(syntax, parent, supertypes, name, symbol, inclusiveMembers, genericParameters, members);
}

// [Closed(typeof(TraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitSymbolNode : ITraitDeclarationNode
{

    public static ITraitSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITraitMemberDeclarationNode> members)
        => new TraitSymbolNode(syntax, parent, supertypes, name, symbol, inclusiveMembers, genericParameters, members);
}

// [Closed(typeof(GenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterSymbolNode : IGenericParameterDeclarationNode
{

    public static IGenericParameterSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IdentifierName name, GenericParameterTypeSymbol symbol, IFixedSet<ITypeMemberDeclarationNode> members)
        => new GenericParameterSymbolNode(syntax, parent, supertypes, inclusiveMembers, name, symbol, members);
}

[Closed(
    typeof(IStandardMethodSymbolNode),
    typeof(IGetterMethodSymbolNode),
    typeof(ISetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSymbolNode : IMethodDeclarationNode
{
}

// [Closed(typeof(StandardMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardMethodSymbolNode : IMethodSymbolNode, IStandardMethodDeclarationNode
{

    public static IStandardMethodSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol, int arity, FunctionType methodGroupType)
        => new StandardMethodSymbolNode(syntax, parent, name, symbol, arity, methodGroupType);
}

// [Closed(typeof(GetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodSymbolNode : IMethodSymbolNode, IGetterMethodDeclarationNode
{

    public static IGetterMethodSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol)
        => new GetterMethodSymbolNode(syntax, parent, name, symbol);
}

// [Closed(typeof(SetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodSymbolNode : IMethodSymbolNode, ISetterMethodDeclarationNode
{

    public static ISetterMethodSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol)
        => new SetterMethodSymbolNode(syntax, parent, name, symbol);
}

// [Closed(typeof(ConstructorSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSymbolNode : IConstructorDeclarationNode
{

    public static IConstructorSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName? name, ConstructorSymbol symbol)
        => new ConstructorSymbolNode(syntax, parent, name, symbol);
}

// [Closed(typeof(InitializerSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSymbolNode : IInitializerDeclarationNode
{

    public static IInitializerSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName? name, InitializerSymbol symbol)
        => new InitializerSymbolNode(syntax, parent, name, symbol);
}

// [Closed(typeof(FieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldSymbolNode : IFieldDeclarationNode
{

    public static IFieldSymbolNode Create(ISyntax? syntax, ISemanticNode parent, IdentifierName name, DataType bindingType, FieldSymbol symbol)
        => new FieldSymbolNode(syntax, parent, name, bindingType, symbol);
}

// [Closed(typeof(AssociatedFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode
{

    public static IAssociatedFunctionSymbolNode Create(ISyntax? syntax, ISemanticNode parent, FunctionSymbol symbol, FunctionType type, StandardName name)
        => new AssociatedFunctionSymbolNode(syntax, parent, symbol, type, name);
}

// TODO switch back to `file` and not `partial` once fully transitioned
internal abstract partial class SemanticNode : TreeNode, IChildTreeNode<ISemanticNode>
{
    private SemanticNode? parent;

    protected SemanticNode() { }
    protected SemanticNode(bool inFinalTree) : base(inFinalTree) { }

    [DebuggerStepThrough]
    protected sealed override ITreeNode PeekParent()
        // Use volatile read to ensure order of operations as seen by other threads
        => Volatile.Read(in parent) ?? throw Child.ParentMissing(this);

    protected SemanticNode GetParent(IInheritanceContext ctx)
    {
        // Use volatile read to ensure order of operations as seen by other threads
        var node = Volatile.Read(in parent) ?? throw Child.ParentMissing(this);
        ctx.AccessParentOf(this);
        return node;
    }

    void IChildTreeNode<ISemanticNode>.SetParent(ISemanticNode newParent)
    {
        if (newParent is not SemanticNode newParentNode)
            throw new ArgumentException($"Parent must be a {nameof(SemanticNode)}.", nameof(newParent));

        // Use volatile write to ensure order of operations as seen by other threads
        Volatile.Write(ref parent, newParentNode);
    }

    /// <summary>
    /// The previous node to this one in a preorder traversal of the tree.
    /// </summary>
    protected virtual SemanticNode Previous(IInheritanceContext ctx)
    {
        SemanticNode? previous = null;
        var parent = GetParent(ctx);
        foreach (var child in parent.Children().Cast<SemanticNode>())
        {
            if (child == this)
                // If this is the first child, return the parent without descending
                return previous?.LastDescendant() ?? parent;
            previous = child;
        }

        throw new UnreachableException("Node is not a child of its parent.");
    }

    // TODO can this be more efficient?
    internal SemanticNode LastDescendant()
        => ((SemanticNode?)Children().LastOrDefault())?.LastDescendant() ?? this;

    protected int? IndexOfNode<T>(IEnumerable<T> nodes, SemanticNode node)
        where T : ISemanticNode
    {
        if (node is not T value)
            return null;
        var index = 0;
        foreach (var item in nodes)
        {
            if (ReferenceEquals(item, value))
                return index;
            index++;
        }

        return null;
    }

    protected bool ContainsNode<T>(IEnumerable<T> nodes, SemanticNode node)
        where T : ISemanticNode
        => node is T value ? nodes.Contains(value) : false;

    protected bool ContainsNode<T>(IFixedSet<T> nodes, SemanticNode node)
        where T : ISemanticNode
        => node is T value ? nodes.Contains(value) : false;

    protected bool ContainsNode<T>(IReadOnlySet<T> nodes, SemanticNode node)
        where T : ISemanticNode
        => node is T value ? nodes.Contains(value) : false;

    internal virtual LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ContainingLexicalScope(this, descendant, ctx);
    protected LexicalScope Inherited_ContainingLexicalScope(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingLexicalScope(this, this, ctx);

    internal virtual ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ContainingDeclaration(this, descendant, ctx);
    protected ISymbolDeclarationNode Inherited_ContainingDeclaration(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContainingDeclaration(this, this, ctx);

    internal virtual bool MayContribute_Diagnostics => false;
    internal virtual bool SubtreeMayContribute_Diagnostics => false;

    internal virtual IPackageDeclarationNode Inherited_Package(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_Package(this, descendant, ctx);
    protected IPackageDeclarationNode Inherited_Package(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_Package(this, this, ctx);

    internal virtual PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_PackageNameScope(this, descendant, ctx);
    protected PackageNameScope Inherited_PackageNameScope(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_PackageNameScope(this, this, ctx);

    internal virtual CodeFile Inherited_File(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_File(this, descendant, ctx);
    protected CodeFile Inherited_File(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_File(this, this, ctx);

    internal virtual IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_Facet(this, descendant, ctx);
    protected IPackageFacetDeclarationNode Inherited_Facet(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_Facet(this, this, ctx);

    internal virtual IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_FlowStateBefore(this, descendant, ctx);
    protected IFlowState Inherited_FlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_FlowStateBefore(this, this, ctx);

    internal virtual IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ControlFlowEntry(this, descendant, ctx);
    protected IEntryNode Inherited_ControlFlowEntry(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowEntry(this, this, ctx);

    internal virtual ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ControlFlowFollowing(this, descendant, ctx);
    protected ControlFlowSet Inherited_ControlFlowFollowing(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowFollowing(this, this, ctx);

    internal virtual FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_VariableBindingsMap(this, descendant, ctx);
    protected FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_VariableBindingsMap(this, this, ctx);

    internal virtual ValueId? Inherited_MatchReferentValueId(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_MatchReferentValueId(this, descendant, ctx);
    protected ValueId? Inherited_MatchReferentValueId(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_MatchReferentValueId(this, this, ctx);

    internal virtual IMaybeAntetype Inherited_ContextBindingAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ContextBindingAntetype(this, descendant, ctx);
    protected IMaybeAntetype Inherited_ContextBindingAntetype(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContextBindingAntetype(this, this, ctx);

    internal virtual DataType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ContextBindingType(this, descendant, ctx);
    protected DataType Inherited_ContextBindingType(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ContextBindingType(this, this, ctx);

    internal virtual bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ImplicitRecoveryAllowed(this, descendant, ctx);
    protected bool Inherited_ImplicitRecoveryAllowed(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ImplicitRecoveryAllowed(this, this, ctx);

    internal virtual bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ShouldPrepareToReturn(this, descendant, ctx);
    protected bool Inherited_ShouldPrepareToReturn(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ShouldPrepareToReturn(this, this, ctx);

    internal virtual IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ControlFlowExit(this, descendant, ctx);
    protected IExitNode Inherited_ControlFlowExit(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ControlFlowExit(this, this, ctx);

    internal virtual DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => GetParent(ctx).Inherited_ExpectedReturnType(this, descendant, ctx);
    protected DataType? Inherited_ExpectedReturnType(IInheritanceContext ctx)
        => GetParent(ctx).Inherited_ExpectedReturnType(this, this, ctx);

    internal virtual IPreviousValueId Previous_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        // TODO does this need to throw an exception for the root of the tree?
        => Previous(ctx).Previous_PreviousValueId(this, ctx);
    protected IPreviousValueId Previous_PreviousValueId(IInheritanceContext ctx)
        => Previous(ctx).Previous_PreviousValueId(this, ctx);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageNode : SemanticNode, IPackageNode
{
    private IPackageNode Self { [Inline] get => this; }

    public IPackageSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedSet<IPackageReferenceNode> References { [DebuggerStepThrough] get; }
    public IPackageFacetNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetNode TestingFacet { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public PackageSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.Package_Symbol);
    private PackageSymbol? symbol;
    private bool symbolCached;
    public FixedDictionary<IdentifierName, IPackageDeclarationNode> PackageDeclarations
        => GrammarAttribute.IsCached(in packageDeclarationsCached) ? packageDeclarations!
            : this.Synthetic(ref packageDeclarationsCached, ref packageDeclarations,
                SymbolNodeAspect.Package_PackageDeclarations);
    private FixedDictionary<IdentifierName, IPackageDeclarationNode>? packageDeclarations;
    private bool packageDeclarationsCached;
    public IFunctionDefinitionNode? EntryPoint
        => GrammarAttribute.IsCached(in entryPointCached) ? entryPoint
            : this.Synthetic(ref entryPointCached, ref entryPoint,
                DefinitionsAspect.Package_EntryPoint);
    private IFunctionDefinitionNode? entryPoint;
    private bool entryPointCached;
    public IPackageSymbols PackageSymbols
        => GrammarAttribute.IsCached(in packageSymbolsCached) ? packageSymbols!
            : this.Synthetic(ref packageSymbolsCached, ref packageSymbols,
                SymbolsAspect.Package_PackageSymbols);
    private IPackageSymbols? packageSymbols;
    private bool packageSymbolsCached;
    public IPackageReferenceNode IntrinsicsReference
        => GrammarAttribute.IsCached(in intrinsicsReferenceCached) ? intrinsicsReference!
            : this.Synthetic(ref intrinsicsReferenceCached, ref intrinsicsReference,
                n => Child.Attach(this, BuiltInsAspect.Package_IntrinsicsReference(n)));
    private IPackageReferenceNode? intrinsicsReference;
    private bool intrinsicsReferenceCached;
    public IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations
        => GrammarAttribute.IsCached(in primitivesDeclarationsCached) ? primitivesDeclarations!
            : this.Synthetic(ref primitivesDeclarationsCached, ref primitivesDeclarations,
                n => ChildSet.Attach(this, BuiltInsAspect.Package_PrimitivesDeclarations(n)));
    private IFixedSet<ITypeDeclarationNode>? primitivesDeclarations;
    private bool primitivesDeclarationsCached;

    public PackageNode(IPackageSyntax syntax, IFixedSet<IPackageReferenceNode> references, IPackageFacetNode mainFacet, IPackageFacetNode testingFacet, DiagnosticCollection diagnostics)
    {
        Syntax = syntax;
        References = references;
        MainFacet = mainFacet;
        TestingFacet = testingFacet;
        Diagnostics = diagnostics;
    }

    internal override IPackageDeclarationNode Inherited_Package(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.MainFacet))
            return LexicalScopingAspect.Package_MainFacet_PackageNameScope(this);
        if (ReferenceEquals(descendant, Self.TestingFacet))
            return LexicalScopingAspect.Package_TestingFacet_PackageNameScope(this);
        return base.Inherited_PackageNameScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageReferenceNode : SemanticNode, IPackageReferenceNode
{
    private IPackageReferenceNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IPackageReferenceSyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName AliasOrName { [DebuggerStepThrough] get; }
    public IPackageSymbols PackageSymbols { [DebuggerStepThrough] get; }
    public bool IsTrusted { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageSymbolNode SymbolNode
        => GrammarAttribute.IsCached(in symbolNodeCached) ? symbolNode!
            : this.Synthetic(ref symbolNodeCached, ref symbolNode,
                SymbolNodeAspect.PackageReference_SymbolNode);
    private IPackageSymbolNode? symbolNode;
    private bool symbolNodeCached;

    public PackageReferenceNode(ISemanticNode parent, IPackageReferenceSyntax? syntax, IdentifierName aliasOrName, IPackageSymbols packageSymbols, bool isTrusted)
    {
        Parent = parent;
        Syntax = syntax;
        AliasOrName = aliasOrName;
        PackageSymbols = packageSymbols;
        IsTrusted = isTrusted;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageFacetNode : SemanticNode, IPackageFacetNode
{
    private IPackageFacetNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? PackageAliasOrName { [DebuggerStepThrough] get; }
    public PackageSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName PackageName { [DebuggerStepThrough] get; }
    public PackageSymbol PackageSymbol { [DebuggerStepThrough] get; }
    public IFixedSet<ICompilationUnitNode> CompilationUnits { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope
        => GrammarAttribute.IsCached(in packageNameScopeCached) ? packageNameScope!
            : this.Inherited(ref packageNameScopeCached, ref packageNameScope,
                Inherited_PackageNameScope);
    private PackageNameScope? packageNameScope;
    private bool packageNameScopeCached;
    public INamespaceDefinitionNode GlobalNamespace
        => GrammarAttribute.IsCached(in globalNamespaceCached) ? globalNamespace!
            : this.Synthetic(ref globalNamespaceCached, ref globalNamespace,
                SymbolNodeAspect.PackageFacet_GlobalNamespace);
    private INamespaceDefinitionNode? globalNamespace;
    private bool globalNamespaceCached;
    public IFixedSet<IPackageMemberDefinitionNode> Definitions
        => GrammarAttribute.IsCached(in definitionsCached) ? definitions!
            : this.Synthetic(ref definitionsCached, ref definitions,
                DefinitionsAspect.PackageFacet_Definitions);
    private IFixedSet<IPackageMemberDefinitionNode>? definitions;
    private bool definitionsCached;

    public PackageFacetNode(ISemanticNode parent, IdentifierName? packageAliasOrName, PackageSymbol symbol, IPackageSyntax syntax, IdentifierName packageName, PackageSymbol packageSymbol, IFixedSet<ICompilationUnitNode> compilationUnits)
    {
        Parent = parent;
        PackageAliasOrName = packageAliasOrName;
        Symbol = symbol;
        Syntax = syntax;
        PackageName = packageName;
        PackageSymbol = packageSymbol;
        CompilationUnits = compilationUnits;
    }

    internal override IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CompilationUnitNode : SemanticNode, ICompilationUnitNode
{
    private ICompilationUnitNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ICompilationUnitSyntax Syntax { [DebuggerStepThrough] get; }
    public PackageSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public NamespaceName ImplicitNamespaceName { [DebuggerStepThrough] get; }
    public NamespaceSymbol ImplicitNamespaceSymbol { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public NamespaceScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                (ctx) => (NamespaceScope)Inherited_ContainingLexicalScope(ctx));
    private NamespaceScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode ContainingDeclaration
        => (IPackageFacetNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.CompilationUnit_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public INamespaceDefinitionNode ImplicitNamespace
        => GrammarAttribute.IsCached(in implicitNamespaceCached) ? implicitNamespace!
            : this.Synthetic(ref implicitNamespaceCached, ref implicitNamespace,
                SymbolNodeAspect.CompilationUnit_ImplicitNamespace);
    private INamespaceDefinitionNode? implicitNamespace;
    private bool implicitNamespaceCached;

    public CompilationUnitNode(ISemanticNode parent, ICompilationUnitSyntax syntax, PackageSymbol containingSymbol, NamespaceName implicitNamespaceName, NamespaceSymbol implicitNamespaceSymbol, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> definitions, DiagnosticCollection diagnostics)
    {
        Parent = parent;
        Syntax = syntax;
        ContainingSymbol = containingSymbol;
        ImplicitNamespaceName = implicitNamespaceName;
        ImplicitNamespaceSymbol = implicitNamespaceSymbol;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
        Diagnostics = diagnostics;
    }

    internal override CodeFile Inherited_File(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ContextAspect.CompilationUnit_Children_Broadcast_File(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return SymbolNodeAspect.CompilationUnit_Children_ContainingDeclaration(this);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UsingDirectiveNode : SemanticNode, IUsingDirectiveNode
{
    private IUsingDirectiveNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IUsingDirectiveSyntax Syntax { [DebuggerStepThrough] get; }
    public NamespaceName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public UsingDirectiveNode(ISemanticNode parent, IUsingDirectiveSyntax syntax, NamespaceName name)
    {
        Parent = parent;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceBlockDefinitionNode : SemanticNode, INamespaceBlockDefinitionNode
{
    private INamespaceBlockDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public StandardName? Name { [DebuggerStepThrough] get; }
    public INamespaceDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsGlobalQualified { [DebuggerStepThrough] get; }
    public NamespaceName DeclaredNames { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public NamespaceSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public NamespaceSearchScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                (ctx) => (NamespaceSearchScope)Inherited_ContainingLexicalScope(ctx));
    private NamespaceSearchScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public INamespaceDefinitionNode ContainingDeclaration
        => (INamespaceDefinitionNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.NamespaceBlockDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public INamespaceDefinitionNode ContainingNamespace
        => GrammarAttribute.IsCached(in containingNamespaceCached) ? containingNamespace!
            : this.Synthetic(ref containingNamespaceCached, ref containingNamespace,
                SymbolNodeAspect.NamespaceBlockDefinition_ContainingNamespace);
    private INamespaceDefinitionNode? containingNamespace;
    private bool containingNamespaceCached;
    public INamespaceDefinitionNode Definition
        => GrammarAttribute.IsCached(in definitionCached) ? definition!
            : this.Synthetic(ref definitionCached, ref definition,
                SymbolNodeAspect.NamespaceBlockDefinition_Definition);
    private INamespaceDefinitionNode? definition;
    private bool definitionCached;

    public NamespaceBlockDefinitionNode(ISemanticNode parent, StandardName? name, INamespaceDefinitionSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveNode> usingDirectives, IFixedList<INamespaceBlockMemberDefinitionNode> members, NamespaceSymbol containingSymbol, NamespaceSymbol symbol)
    {
        Parent = parent;
        Name = name;
        Syntax = syntax;
        IsGlobalQualified = isGlobalQualified;
        DeclaredNames = declaredNames;
        UsingDirectives = usingDirectives;
        Members = members;
        ContainingSymbol = containingSymbol;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceDefinitionNode : SemanticNode, INamespaceDefinitionNode
{
    private INamespaceDefinitionNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { [DebuggerStepThrough] get; }
    public IFixedList<IPackageMemberDefinitionNode> PackageMembers { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;

    public NamespaceDefinitionNode(ISyntax? syntax, ISemanticNode parent, NamespaceSymbol symbol, IdentifierName name, IFixedList<INamespaceMemberDeclarationNode> nestedMembers, IFixedList<INamespaceDefinitionNode> memberNamespaces, IFixedList<IPackageMemberDefinitionNode> packageMembers, IFixedList<INamespaceMemberDefinitionNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Symbol = symbol;
        Name = name;
        NestedMembers = nestedMembers;
        MemberNamespaces = memberNamespaces;
        PackageMembers = packageMembers;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionDefinitionNode : SemanticNode, IFunctionDefinitionNode
{
    private IFunctionDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public IFunctionDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public NamespaceSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public INamespaceDeclarationNode ContainingDeclaration
        => (INamespaceDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.FunctionDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public FunctionSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.FunctionDefinition_Symbol);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public FunctionDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IFunctionDefinitionSyntax syntax, NamespaceSymbol containingSymbol, IFixedList<IAttributeNode> attributes, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit, FunctionType type)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        VariableBindingsMap = variableBindingsMap;
        Syntax = syntax;
        ContainingSymbol = containingSymbol;
        Attributes = attributes;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassDefinitionNode : SemanticNode, IClassDefinitionNode
{
    private IClassDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public Symbol ContainingSymbol { [DebuggerStepThrough] get; }
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public bool IsConst { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IClassDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public bool IsAbstract { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IStandardTypeNameNode? BaseTypeName { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
    public ObjectType DeclaredType { [DebuggerStepThrough] get; }
    public IFixedList<IClassMemberDefinitionNode> SourceMembers { [DebuggerStepThrough] get; }
    public IFixedSet<IClassMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IDefaultConstructorDefinitionNode? DefaultConstructor { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public UserTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private UserTypeSymbol? symbol;
    private bool symbolCached;
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public ClassDefinitionNode(ISemanticNode parent, Symbol containingSymbol, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IClassDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, bool isAbstract, IFixedList<IGenericParameterNode> genericParameters, IStandardTypeNameNode? baseTypeName, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedList<IClassMemberDefinitionNode> sourceMembers, IFixedSet<IClassMemberDefinitionNode> members, IDefaultConstructorDefinitionNode? defaultConstructor)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        InclusiveMembers = inclusiveMembers;
        IsConst = isConst;
        Name = name;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
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
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructDefinitionNode : SemanticNode, IStructDefinitionNode
{
    private IStructDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public Symbol ContainingSymbol { [DebuggerStepThrough] get; }
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public bool IsConst { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IStructDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
    public StructType DeclaredType { [DebuggerStepThrough] get; }
    public IFixedList<IStructMemberDefinitionNode> SourceMembers { [DebuggerStepThrough] get; }
    public IFixedSet<IStructMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IDefaultInitializerDefinitionNode? DefaultInitializer { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public UserTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private UserTypeSymbol? symbol;
    private bool symbolCached;
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public StructDefinitionNode(ISemanticNode parent, Symbol containingSymbol, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, IStructDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, StructType declaredType, IFixedList<IStructMemberDefinitionNode> sourceMembers, IFixedSet<IStructMemberDefinitionNode> members, IDefaultInitializerDefinitionNode? defaultInitializer)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        InclusiveMembers = inclusiveMembers;
        IsConst = isConst;
        Name = name;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
        Syntax = syntax;
        Attributes = attributes;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        DeclaredType = declaredType;
        SourceMembers = sourceMembers;
        Members = members;
        DefaultInitializer = defaultInitializer;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitDefinitionNode : SemanticNode, ITraitDefinitionNode
{
    private ITraitDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public Symbol ContainingSymbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public bool IsConst { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public ITraitDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
    public ObjectType DeclaredType { [DebuggerStepThrough] get; }
    public IFixedSet<ITraitMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public UserTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private UserTypeSymbol? symbol;
    private bool symbolCached;
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public TraitDefinitionNode(ISemanticNode parent, Symbol containingSymbol, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers, bool isConst, StandardName name, IFixedSet<BareReferenceType> supertypes, AccessModifier accessModifier, ITraitDefinitionSyntax syntax, IFixedList<IAttributeNode> attributes, IFixedList<IGenericParameterNode> genericParameters, IFixedList<IStandardTypeNameNode> supertypeNames, ObjectType declaredType, IFixedSet<ITraitMemberDefinitionNode> members)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        InclusiveMembers = inclusiveMembers;
        IsConst = isConst;
        Name = name;
        Supertypes = supertypes;
        AccessModifier = accessModifier;
        Syntax = syntax;
        Attributes = attributes;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        DeclaredType = declaredType;
        Members = members;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterNode : SemanticNode, IGenericParameterNode
{
    private IGenericParameterNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IGenericParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityConstraintNode Constraint { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TypeParameterIndependence Independence { [DebuggerStepThrough] get; }
    public TypeParameterVariance Variance { [DebuggerStepThrough] get; }
    public GenericParameter Parameter { [DebuggerStepThrough] get; }
    public IDeclaredUserType ContainingDeclaredType { [DebuggerStepThrough] get; }
    public GenericParameterType DeclaredType { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public GenericParameterTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.GenericParameter_Symbol);
    private GenericParameterTypeSymbol? symbol;
    private bool symbolCached;

    public GenericParameterNode(ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IGenericParameterSyntax syntax, ICapabilityConstraintNode constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, GenericParameter parameter, IDeclaredUserType containingDeclaredType, GenericParameterType declaredType, UserTypeSymbol containingSymbol, IFixedSet<ITypeMemberDefinitionNode> members)
    {
        Parent = parent;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Syntax = syntax;
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
        Parameter = parameter;
        ContainingDeclaredType = containingDeclaredType;
        DeclaredType = declaredType;
        ContainingSymbol = containingSymbol;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AbstractMethodDefinitionNode : SemanticNode, IAbstractMethodDefinitionNode
{
    private IAbstractMethodDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public MethodKind Kind { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public int Arity { [DebuggerStepThrough] get; }
    public FunctionType MethodGroupType { [DebuggerStepThrough] get; }
    public IAbstractMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public ObjectType ContainingDeclaredType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public MethodSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public AbstractMethodDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, int arity, FunctionType methodGroupType, IAbstractMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, ObjectType containingDeclaredType)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        ContainingSymbol = containingSymbol;
        Kind = kind;
        Name = name;
        Arity = arity;
        MethodGroupType = methodGroupType;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        ContainingDeclaredType = containingDeclaredType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardMethodDefinitionNode : SemanticNode, IStandardMethodDefinitionNode
{
    private IStandardMethodDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public MethodKind Kind { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public int Arity { [DebuggerStepThrough] get; }
    public FunctionType MethodGroupType { [DebuggerStepThrough] get; }
    public IStandardMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public MethodSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public StandardMethodDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, int arity, FunctionType methodGroupType, IStandardMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        ContainingSymbol = containingSymbol;
        Kind = kind;
        Name = name;
        VariableBindingsMap = variableBindingsMap;
        Arity = arity;
        MethodGroupType = methodGroupType;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodDefinitionNode : SemanticNode, IGetterMethodDefinitionNode
{
    private IGetterMethodDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public MethodKind Kind { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public IGetterMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode Return { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public MethodSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public GetterMethodDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IGetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode @return, IEntryNode entry, IBodyNode body, IExitNode exit)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        ContainingSymbol = containingSymbol;
        Kind = kind;
        Name = name;
        VariableBindingsMap = variableBindingsMap;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodDefinitionNode : SemanticNode, ISetterMethodDefinitionNode
{
    private ISetterMethodDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public MethodKind Kind { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public ISetterMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public MethodSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public SetterMethodDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, MethodKind kind, IdentifierName name, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, ISetterMethodDefinitionSyntax syntax, IMethodSelfParameterNode selfParameter, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, IEntryNode entry, IBodyNode body, IExitNode exit)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        ContainingSymbol = containingSymbol;
        Kind = kind;
        Name = name;
        VariableBindingsMap = variableBindingsMap;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class DefaultConstructorDefinitionNode : SemanticNode, IDefaultConstructorDefinitionNode
{
    private IDefaultConstructorDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IConstructorDefinitionSyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode? Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ConstructorSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.DefaultConstructorDefinition_Symbol);
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public DefaultConstructorDefinitionNode(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IConstructorDefinitionSyntax? syntax, IdentifierName? name, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
        Syntax = syntax;
        Name = name;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SourceConstructorDefinitionNode : SemanticNode, ISourceConstructorDefinitionNode
{
    private ISourceConstructorDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IConstructorDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ConstructorSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.SourceConstructorDefinition_Symbol);
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public SourceConstructorDefinitionNode(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IdentifierName? name, IConstructorDefinitionSyntax syntax, IConstructorSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
        Name = name;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class DefaultInitializerDefinitionNode : SemanticNode, IDefaultInitializerDefinitionNode
{
    private IDefaultInitializerDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IInitializerDefinitionSyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode? Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public InitializerSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.DefaultInitializerDefinition_Symbol);
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.InitializerDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public DefaultInitializerDefinitionNode(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IInitializerDefinitionSyntax? syntax, IdentifierName? name, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBodyNode? body, IExitNode exit)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
        Syntax = syntax;
        Name = name;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SourceInitializerDefinitionNode : SemanticNode, ISourceInitializerDefinitionNode
{
    private ISourceInitializerDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IInitializerDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public InitializerSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.SourceInitializerDefinition_Symbol);
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.InitializerDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public SourceInitializerDefinitionNode(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IdentifierName? name, IInitializerDefinitionSyntax syntax, IInitializerSelfParameterNode selfParameter, IFixedList<IConstructorOrInitializerParameterNode> parameters, IEntryNode entry, IBlockBodyNode body, IExitNode exit)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
        Name = name;
        Syntax = syntax;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldDefinitionNode : SemanticNode, IFieldDefinitionNode
{
    private IFieldDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public IFieldDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeNode TypeNode { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode? TempInitializer { [DebuggerStepThrough] get; }
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentInitializer { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.FieldDefinition_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.FieldDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public FieldSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.FieldDefinition_Symbol);
    private FieldSymbol? symbol;
    private bool symbolCached;

    public FieldDefinitionNode(ISemanticNode parent, AccessModifier accessModifier, UserTypeSymbol containingSymbol, bool isLentBinding, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, IFieldDefinitionSyntax syntax, bool isMutableBinding, IdentifierName name, ITypeNode typeNode, IMaybeAntetype bindingAntetype, DataType bindingType, IEntryNode entry, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer, IExitNode exit)
    {
        Parent = parent;
        AccessModifier = accessModifier;
        ContainingSymbol = containingSymbol;
        IsLentBinding = isLentBinding;
        VariableBindingsMap = variableBindingsMap;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        TypeNode = typeNode;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        Entry = entry;
        TempInitializer = initializer;
        CurrentInitializer = currentInitializer;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssociatedFunctionDefinitionNode : SemanticNode, IAssociatedFunctionDefinitionNode
{
    private IAssociatedFunctionDefinitionNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public UserTypeSymbol ContainingSymbol { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode,int> VariableBindingsMap { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier { [DebuggerStepThrough] get; }
    public IAssociatedFunctionDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode Facet
        => GrammarAttribute.IsCached(in facetCached) ? facet!
            : this.Inherited(ref facetCached, ref facet,
                (ctx) => (IPackageFacetNode)Inherited_Facet(ctx));
    private IPackageFacetNode? facet;
    private bool facetCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.AssociatedFunctionDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public FunctionSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.AssociatedFunctionDefinition_Symbol);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public AssociatedFunctionDefinitionNode(ISemanticNode parent, UserTypeSymbol containingSymbol, FixedDictionary<IVariableBindingNode,int> variableBindingsMap, AccessModifier accessModifier, IAssociatedFunctionDefinitionSyntax syntax, IdentifierName name, IFixedList<INamedParameterNode> parameters, ITypeNode? @return, FunctionType type, IEntryNode entry, IBodyNode body, IExitNode exit)
    {
        Parent = parent;
        ContainingSymbol = containingSymbol;
        VariableBindingsMap = variableBindingsMap;
        AccessModifier = accessModifier;
        Syntax = syntax;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Type = type;
        Entry = entry;
        Body = body;
        Exit = exit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AttributeNode : SemanticNode, IAttributeNode
{
    private IAttributeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IAttributeSyntax Syntax { [DebuggerStepThrough] get; }
    public IStandardTypeNameNode TypeName { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ConstructorSymbol? ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.Attribute_ReferencedSymbol);
    private ConstructorSymbol? referencedSymbol;
    private bool referencedSymbolCached;

    public AttributeNode(ISemanticNode parent, IAttributeSyntax syntax, IStandardTypeNameNode typeName)
    {
        Parent = parent;
        Syntax = syntax;
        TypeName = typeName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilitySetNode : SemanticNode, ICapabilitySetNode
{
    private ICapabilitySetNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ICapabilitySetSyntax Syntax { [DebuggerStepThrough] get; }
    public CapabilitySet Constraint { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilitySetNode(ISemanticNode parent, ICapabilitySetSyntax syntax, CapabilitySet constraint)
    {
        Parent = parent;
        Syntax = syntax;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityNode : SemanticNode, ICapabilityNode
{
    private ICapabilityNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ICapabilityConstraint Constraint { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Syntax { [DebuggerStepThrough] get; }
    public Capability Capability { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityNode(ISemanticNode parent, ICapabilityConstraint constraint, ICapabilitySyntax syntax, Capability capability)
    {
        Parent = parent;
        Constraint = constraint;
        Syntax = syntax;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamedParameterNode : SemanticNode, INamedParameterNode
{
    private INamedParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ParameterType ParameterType { [DebuggerStepThrough] get; }
    public INamedParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeNode TypeNode { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public NamedParameterNode(ISemanticNode parent, bool unused, IFlowState flowStateAfter, ParameterType parameterType, INamedParameterSyntax syntax, bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeNode typeNode, IMaybeAntetype bindingAntetype, DataType bindingType)
    {
        Parent = parent;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ParameterType = parameterType;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        IsLentBinding = isLentBinding;
        Name = name;
        TypeNode = typeNode;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSelfParameterNode : SemanticNode, IConstructorSelfParameterNode
{
    private IConstructorSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ITypeDefinitionNode ContainingTypeDefinition { [DebuggerStepThrough] get; }
    public SelfParameterType ParameterType { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public CapabilityType BindingType { [DebuggerStepThrough] get; }
    public ObjectType ContainingDeclaredType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public ConstructorSelfParameterNode(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, IConstructorSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, ObjectType containingDeclaredType)
    {
        Parent = parent;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ContainingTypeDefinition = containingTypeDefinition;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
        BindingType = bindingType;
        ContainingDeclaredType = containingDeclaredType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSelfParameterNode : SemanticNode, IInitializerSelfParameterNode
{
    private IInitializerSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ITypeDefinitionNode ContainingTypeDefinition { [DebuggerStepThrough] get; }
    public SelfParameterType ParameterType { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public CapabilityType BindingType { [DebuggerStepThrough] get; }
    public StructType ContainingDeclaredType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public InitializerSelfParameterNode(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, IInitializerSelfParameterSyntax syntax, bool isLentBinding, ICapabilityNode capability, CapabilityType bindingType, StructType containingDeclaredType)
    {
        Parent = parent;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ContainingTypeDefinition = containingTypeDefinition;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
        BindingType = bindingType;
        ContainingDeclaredType = containingDeclaredType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodSelfParameterNode : SemanticNode, IMethodSelfParameterNode
{
    private IMethodSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ITypeDefinitionNode ContainingTypeDefinition { [DebuggerStepThrough] get; }
    public IDeclaredUserType ContainingDeclaredType { [DebuggerStepThrough] get; }
    public SelfParameterType ParameterType { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public Pseudotype BindingType { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityConstraintNode Capability { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public MethodSelfParameterNode(ISemanticNode parent, IdentifierName? name, bool unused, IFlowState flowStateAfter, ITypeDefinitionNode containingTypeDefinition, IDeclaredUserType containingDeclaredType, SelfParameterType parameterType, IMaybeAntetype bindingAntetype, Pseudotype bindingType, IMethodSelfParameterSyntax syntax, bool isLentBinding, ICapabilityConstraintNode capability)
    {
        Parent = parent;
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ContainingTypeDefinition = containingTypeDefinition;
        ContainingDeclaredType = containingDeclaredType;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldParameterNode : SemanticNode, IFieldParameterNode
{
    private IFieldParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public ParameterType ParameterType { [DebuggerStepThrough] get; }
    public IFieldParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeDefinitionNode ContainingTypeDefinition { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IFieldDefinitionNode? ReferencedField
        => GrammarAttribute.IsCached(in referencedFieldCached) ? referencedField
            : this.Synthetic(ref referencedFieldCached, ref referencedField,
                SymbolNodeAspect.FieldParameter_ReferencedField);
    private IFieldDefinitionNode? referencedField;
    private bool referencedFieldCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public FieldParameterNode(ISemanticNode parent, bool unused, IMaybeAntetype bindingAntetype, IFlowState flowStateAfter, DataType bindingType, ParameterType parameterType, IFieldParameterSyntax syntax, IdentifierName name, ITypeDefinitionNode containingTypeDefinition)
    {
        Parent = parent;
        Unused = unused;
        BindingAntetype = bindingAntetype;
        FlowStateAfter = flowStateAfter;
        BindingType = bindingType;
        ParameterType = parameterType;
        Syntax = syntax;
        Name = name;
        ContainingTypeDefinition = containingTypeDefinition;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockBodyNode : SemanticNode, IBlockBodyNode
{
    private IBlockBodyNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IBodyStatementNode> Statements { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public BlockBodyNode(ISemanticNode parent, IFlowState flowStateAfter, IBlockBodySyntax syntax, IFixedList<IBodyStatementNode> statements)
    {
        Parent = parent;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Statements = statements;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is {} statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionBodyNode : SemanticNode, IExpressionBodyNode
{
    private IExpressionBodyNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IResultStatementNode ResultStatement { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public ExpressionBodyNode(ISemanticNode parent, IFlowState flowStateAfter, IExpressionBodySyntax syntax, IResultStatementNode resultStatement, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType)
    {
        Parent = parent;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        ResultStatement = resultStatement;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is {} statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierTypeNameNode : SemanticNode, IIdentifierTypeNameNode
{
    private IIdentifierTypeNameNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public bool IsAttributeType { [DebuggerStepThrough] get; }
    public IIdentifierTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                SymbolNodeAspect.StandardTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public TypeSymbol? ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.StandardTypeName_ReferencedSymbol);
    private TypeSymbol? referencedSymbol;
    private bool referencedSymbolCached;

    public IdentifierTypeNameNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, bool isAttributeType, IIdentifierTypeNameSyntax syntax, IdentifierName name)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        NamedBareType = namedBareType;
        IsAttributeType = isAttributeType;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SpecialTypeNameNode : SemanticNode, ISpecialTypeNameNode
{
    private ISpecialTypeNameNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public ISpecialTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public TypeSymbol ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol!
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.SpecialTypeName_ReferencedSymbol);
    private TypeSymbol? referencedSymbol;
    private bool referencedSymbolCached;

    public SpecialTypeNameNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, ISpecialTypeNameSyntax syntax, SpecialTypeName name)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericTypeNameNode : SemanticNode, IGenericTypeNameNode
{
    private IGenericTypeNameNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public bool IsAttributeType { [DebuggerStepThrough] get; }
    public IGenericTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                SymbolNodeAspect.StandardTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public TypeSymbol? ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.StandardTypeName_ReferencedSymbol);
    private TypeSymbol? referencedSymbol;
    private bool referencedSymbolCached;

    public GenericTypeNameNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, BareType? namedBareType, bool isAttributeType, IGenericTypeNameSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        NamedBareType = namedBareType;
        IsAttributeType = isAttributeType;
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameNode : SemanticNode, IQualifiedTypeNameNode
{
    private IQualifiedTypeNameNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public TypeName Name { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public IQualifiedTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNameNode Context { [DebuggerStepThrough] get; }
    public IStandardTypeNameNode QualifiedName { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public TypeSymbol? ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.QualifiedTypeName_ReferencedSymbol);
    private TypeSymbol? referencedSymbol;
    private bool referencedSymbolCached;

    public QualifiedTypeNameNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, TypeName name, BareType? namedBareType, IQualifiedTypeNameSyntax syntax, ITypeNameNode context, IStandardTypeNameNode qualifiedName)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Name = name;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Context = context;
        QualifiedName = qualifiedName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalTypeNode : SemanticNode, IOptionalTypeNode
{
    private IOptionalTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public IOptionalTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public OptionalTypeNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, IOptionalTypeSyntax syntax, ITypeNode referent)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityTypeNode : SemanticNode, ICapabilityTypeNode
{
    private ICapabilityTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ICapabilityTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityTypeNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ICapabilityTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionTypeNode : SemanticNode, IFunctionTypeNode
{
    private IFunctionTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public IFunctionTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IParameterTypeNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode Return { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public FunctionTypeNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, IFunctionTypeSyntax syntax, IFixedList<IParameterTypeNode> parameters, ITypeNode @return)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Parameters = parameters;
        Return = @return;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ParameterTypeNode : SemanticNode, IParameterTypeNode
{
    private IParameterTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IParameterTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLent { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public ParameterType Parameter { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public ParameterTypeNode(ISemanticNode parent, IParameterTypeSyntax syntax, bool isLent, ITypeNode referent, ParameterType parameter)
    {
        Parent = parent;
        Syntax = syntax;
        IsLent = isLent;
        Referent = referent;
        Parameter = parameter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityViewpointTypeNode : SemanticNode, ICapabilityViewpointTypeNode
{
    private ICapabilityViewpointTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ICapabilityViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityViewpointTypeNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ICapabilityViewpointTypeSyntax syntax, ICapabilityNode capability, ITypeNode referent)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfViewpointTypeNode : SemanticNode, ISelfViewpointTypeNode
{
    private ISelfViewpointTypeNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ISelfViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public Pseudotype? NamedSelfType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public SelfViewpointTypeNode(ISemanticNode parent, IMaybeAntetype namedAntetype, DataType namedType, ISelfViewpointTypeSyntax syntax, ITypeNode referent, Pseudotype? namedSelfType)
    {
        Parent = parent;
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Referent = referent;
        NamedSelfType = namedSelfType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class EntryNode : SemanticNode, IEntryNode
{
    private IEntryNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ICodeSyntax? Syntax { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap()
        => Inherited_VariableBindingsMap(GrammarAttribute.CurrentInheritanceContext());

    public EntryNode(ISemanticNode parent, ICodeSyntax? syntax, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Parent = parent;
        Syntax = syntax;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExitNode : SemanticNode, IExitNode
{
    private IExitNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ICodeSyntax? Syntax { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    public ExitNode(ISemanticNode parent, ICodeSyntax? syntax, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
        Parent = parent;
        Syntax = syntax;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ResultStatementNode : SemanticNode, IResultStatementNode
{
    private IResultStatementNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeAntetype? ResultAntetype { [DebuggerStepThrough] get; }
    public DataType? ResultType { [DebuggerStepThrough] get; }
    public IMaybeAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IResultStatementSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public ResultStatementNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IMaybeAntetype antetype, DataType type, IResultStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IFlowState flowStateAfter)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        Antetype = antetype;
        Type = type;
        Syntax = syntax;
        TempExpression = expression;
        CurrentExpression = currentExpression;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableDeclarationStatementNode : SemanticNode, IVariableDeclarationStatementNode
{
    private IVariableDeclarationStatementNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeAntetype? ResultAntetype { [DebuggerStepThrough] get; }
    public DataType? ResultType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IVariableDeclarationStatementSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ICapabilityNode? Capability { [DebuggerStepThrough] get; }
    public ITypeNode? Type { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode? TempInitializer { [DebuggerStepThrough] get; }
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentInitializer { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.VariableDeclarationStatement_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.VariableDeclarationStatement_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;

    public VariableDeclarationStatementNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IVariableDeclarationStatementSyntax syntax, bool isMutableBinding, IdentifierName name, ICapabilityNode? capability, ITypeNode? type, IAmbiguousExpressionNode? initializer, IAmbiguousExpressionNode? currentInitializer)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        FlowStateAfter = flowStateAfter;
        IsLentBinding = isLentBinding;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        Capability = capability;
        Type = type;
        TempInitializer = initializer;
        CurrentInitializer = currentInitializer;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionStatementNode : SemanticNode, IExpressionStatementNode
{
    private IExpressionStatementNode Self { [Inline] get => this; }

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeAntetype? ResultAntetype { [DebuggerStepThrough] get; }
    public DataType? ResultType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionStatementSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    public ExpressionStatementNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeAntetype? resultAntetype, DataType? resultType, IFlowState flowStateAfter, IExpressionStatementSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = expression;
        CurrentExpression = currentExpression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingContextPatternNode : SemanticNode, IBindingContextPatternNode
{
    private IBindingContextPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IBindingContextPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IPatternNode Pattern { [DebuggerStepThrough] get; }
    public ITypeNode? Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeAntetype ContextBindingAntetype()
        => Inherited_ContextBindingAntetype(GrammarAttribute.CurrentInheritanceContext());
    public DataType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());

    public BindingContextPatternNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, IBindingContextPatternSyntax syntax, bool isMutableBinding, IPatternNode pattern, ITypeNode? type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingPatternNode : SemanticNode, IBindingPatternNode
{
    private IBindingPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IBindingPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeAntetype ContextBindingAntetype()
        => Inherited_ContextBindingAntetype(GrammarAttribute.CurrentInheritanceContext());
    public DataType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.BindingPattern_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public BindingPatternNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IBindingPatternSyntax syntax, bool isMutableBinding, IdentifierName name)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        FlowStateAfter = flowStateAfter;
        IsLentBinding = isLentBinding;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalPatternNode : SemanticNode, IOptionalPatternNode
{
    private IOptionalPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IOptionalPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public IOptionalOrBindingPatternNode Pattern { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeAntetype ContextBindingAntetype()
        => Inherited_ContextBindingAntetype(GrammarAttribute.CurrentInheritanceContext());
    public DataType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());

    public OptionalPatternNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IFlowState flowStateAfter, IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockExpressionNode : SemanticNode, IBlockExpressionNode
{
    private IBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IStatementNode> Statements { [DebuggerStepThrough] get; }
    public IMaybeAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BlockExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, IBlockExpressionSyntax syntax, IFixedList<IStatementNode> statements, IMaybeAntetype antetype, DataType type, IFlowState flowStateAfter)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        Syntax = syntax;
        Statements = statements;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is {} statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NewObjectExpressionNode : SemanticNode, INewObjectExpressionNode
{
    private INewObjectExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public INewObjectExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNameNode ConstructingType { [DebuggerStepThrough] get; }
    public IdentifierName? ConstructorName { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IMaybeAntetype ConstructingAntetype { [DebuggerStepThrough] get; }
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { [DebuggerStepThrough] get; }
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { [DebuggerStepThrough] get; }
    public IConstructorDeclarationNode? ReferencedConstructor { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NewObjectExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, INewObjectExpressionSyntax syntax, ITypeNameNode constructingType, IdentifierName? constructorName, IFixedList<IAmbiguousExpressionNode> arguments, IMaybeAntetype constructingAntetype, IFixedSet<IConstructorDeclarationNode> referencedConstructors, IFixedSet<IConstructorDeclarationNode> compatibleConstructors, IConstructorDeclarationNode? referencedConstructor, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        ConstructingType = constructingType;
        ConstructorName = constructorName;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        ConstructingAntetype = constructingAntetype;
        ReferencedConstructors = referencedConstructors;
        CompatibleConstructors = compatibleConstructors;
        ReferencedConstructor = referencedConstructor;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnsafeExpressionNode : SemanticNode, IUnsafeExpressionNode
{
    private IUnsafeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IUnsafeExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnsafeExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IUnsafeExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BoolLiteralExpressionNode : SemanticNode, IBoolLiteralExpressionNode
{
    private IBoolLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IBoolLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool Value { [DebuggerStepThrough] get; }
    public BoolConstValueType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BoolLiteralExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IBoolLiteralExpressionSyntax syntax, bool value, BoolConstValueType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Value = value;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IntegerLiteralExpressionNode : SemanticNode, IIntegerLiteralExpressionNode
{
    private IIntegerLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IIntegerLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public BigInteger Value { [DebuggerStepThrough] get; }
    public IntegerConstValueType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IntegerLiteralExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IIntegerLiteralExpressionSyntax syntax, BigInteger value, IntegerConstValueType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Value = value;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NoneLiteralExpressionNode : SemanticNode, INoneLiteralExpressionNode
{
    private INoneLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public INoneLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public OptionalType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NoneLiteralExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, INoneLiteralExpressionSyntax syntax, OptionalType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StringLiteralExpressionNode : SemanticNode, IStringLiteralExpressionNode
{
    private IStringLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IStringLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public string Value { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public StringLiteralExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IStringLiteralExpressionSyntax syntax, string value, DataType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Value = value;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssignmentExpressionNode : SemanticNode, IAssignmentExpressionNode
{
    private IAssignmentExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IAssignmentExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousAssignableExpressionNode TempLeftOperand { [DebuggerStepThrough] get; }
    public IAssignableExpressionNode? LeftOperand => TempLeftOperand as IAssignableExpressionNode;
    public IAmbiguousAssignableExpressionNode CurrentLeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempRightOperand { [DebuggerStepThrough] get; }
    public IExpressionNode? RightOperand => TempRightOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentRightOperand { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AssignmentExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IAssignmentExpressionSyntax syntax, IAmbiguousAssignableExpressionNode leftOperand, IAmbiguousAssignableExpressionNode currentLeftOperand, AssignmentOperator @operator, IAmbiguousExpressionNode rightOperand, IAmbiguousExpressionNode currentRightOperand)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        Syntax = syntax;
        TempLeftOperand = leftOperand;
        CurrentLeftOperand = currentLeftOperand;
        Operator = @operator;
        TempRightOperand = rightOperand;
        CurrentRightOperand = currentRightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BinaryOperatorExpressionNode : SemanticNode, IBinaryOperatorExpressionNode
{
    private IBinaryOperatorExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IBinaryOperatorExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempLeftOperand { [DebuggerStepThrough] get; }
    public IExpressionNode? LeftOperand => TempLeftOperand as IExpressionNode;
    public BinaryOperator Operator { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempRightOperand { [DebuggerStepThrough] get; }
    public IExpressionNode? RightOperand => TempRightOperand as IExpressionNode;
    public IAntetype? NumericOperatorCommonAntetype { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BinaryOperatorExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IBinaryOperatorExpressionSyntax syntax, IAmbiguousExpressionNode leftOperand, BinaryOperator @operator, IAmbiguousExpressionNode rightOperand, IAntetype? numericOperatorCommonAntetype)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempLeftOperand = leftOperand;
        Operator = @operator;
        TempRightOperand = rightOperand;
        NumericOperatorCommonAntetype = numericOperatorCommonAntetype;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.RightOperand))
            return LexicalScopingAspect.BinaryOperatorExpression_RightOperand_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnaryOperatorExpressionNode : SemanticNode, IUnaryOperatorExpressionNode
{
    private IUnaryOperatorExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IUnaryOperatorExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public UnaryOperatorFixity Fixity { [DebuggerStepThrough] get; }
    public UnaryOperator Operator { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempOperand { [DebuggerStepThrough] get; }
    public IExpressionNode? Operand => TempOperand as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnaryOperatorExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IUnaryOperatorExpressionSyntax syntax, UnaryOperatorFixity fixity, UnaryOperator @operator, IAmbiguousExpressionNode operand)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Fixity = fixity;
        Operator = @operator;
        TempOperand = operand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdExpressionNode : SemanticNode, IIdExpressionNode
{
    private IIdExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IIdExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempReferent { [DebuggerStepThrough] get; }
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IdExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConversionExpressionNode : SemanticNode, IConversionExpressionNode
{
    private IConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IConversionExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempReferent { [DebuggerStepThrough] get; }
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public ConversionOperator Operator { [DebuggerStepThrough] get; }
    public ITypeNode ConvertToType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ConversionExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IConversionExpressionSyntax syntax, IAmbiguousExpressionNode referent, ConversionOperator @operator, ITypeNode convertToType)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = referent;
        Operator = @operator;
        ConvertToType = convertToType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitConversionExpressionNode : SemanticNode, IImplicitConversionExpressionNode
{
    private IImplicitConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionNode Referent { [DebuggerStepThrough] get; }
    public IExpressionNode CurrentReferent { [DebuggerStepThrough] get; }
    public SimpleAntetype Antetype { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitConversionExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode referent, IExpressionNode currentReferent, SimpleAntetype antetype)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Referent = referent;
        CurrentReferent = currentReferent;
        Antetype = antetype;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PatternMatchExpressionNode : SemanticNode, IPatternMatchExpressionNode
{
    private IPatternMatchExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IPatternMatchExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempReferent { [DebuggerStepThrough] get; }
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public IPatternNode Pattern { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PatternMatchExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IPatternMatchExpressionSyntax syntax, IAmbiguousExpressionNode referent, IPatternNode pattern)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = referent;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IfExpressionNode : SemanticNode, IIfExpressionNode
{
    private IIfExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IIfExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempCondition { [DebuggerStepThrough] get; }
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    public IBlockOrResultNode ThenBlock { [DebuggerStepThrough] get; }
    public IElseClauseNode? ElseClause { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IfExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IIfExpressionSyntax syntax, IAmbiguousExpressionNode condition, IBlockOrResultNode thenBlock, IElseClauseNode? elseClause, IFlowState flowStateAfter)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        Syntax = syntax;
        TempCondition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
        FlowStateAfter = flowStateAfter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class LoopExpressionNode : SemanticNode, ILoopExpressionNode
{
    private ILoopExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ILoopExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IBlockExpressionNode Block { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public LoopExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ILoopExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class WhileExpressionNode : SemanticNode, IWhileExpressionNode
{
    private IWhileExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IWhileExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempCondition { [DebuggerStepThrough] get; }
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    public IBlockExpressionNode Block { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public WhileExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IWhileExpressionSyntax syntax, IAmbiguousExpressionNode condition, IBlockExpressionNode block)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempCondition = condition;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ForeachExpressionNode : SemanticNode, IForeachExpressionNode
{
    private IForeachExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned { [DebuggerStepThrough] get; }
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned { [DebuggerStepThrough] get; }
    public IForeachExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempInExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? InExpression => TempInExpression as IExpressionNode;
    public ITypeNode? DeclaredType { [DebuggerStepThrough] get; }
    public IBlockExpressionNode Block { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode? ReferencedIterableDeclaration { [DebuggerStepThrough] get; }
    public IStandardMethodDeclarationNode? ReferencedIterateMethod { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype IteratorAntetype { [DebuggerStepThrough] get; }
    public DataType IteratorType { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode? ReferencedIteratorDeclaration { [DebuggerStepThrough] get; }
    public IStandardMethodDeclarationNode? ReferencedNextMethod { [DebuggerStepThrough] get; }
    public IMaybeAntetype IteratedAntetype { [DebuggerStepThrough] get; }
    public DataType IteratedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateBeforeBlock { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.ForeachExpression_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ForeachExpression_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ForeachExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isLentBinding, IMaybeAntetype bindingAntetype, IdentifierName name, DataType bindingType, IFixedSet<IDataFlowNode> dataFlowPrevious, BindingFlags<IVariableBindingNode> definitelyAssigned, BindingFlags<IVariableBindingNode> definitelyUnassigned, IForeachExpressionSyntax syntax, bool isMutableBinding, IdentifierName variableName, IAmbiguousExpressionNode inExpression, ITypeNode? declaredType, IBlockExpressionNode block, ITypeDeclarationNode? referencedIterableDeclaration, IStandardMethodDeclarationNode? referencedIterateMethod, IMaybeExpressionAntetype iteratorAntetype, DataType iteratorType, ITypeDeclarationNode? referencedIteratorDeclaration, IStandardMethodDeclarationNode? referencedNextMethod, IMaybeAntetype iteratedAntetype, DataType iteratedType, IFlowState flowStateBeforeBlock)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsLentBinding = isLentBinding;
        BindingAntetype = bindingAntetype;
        Name = name;
        BindingType = bindingType;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        VariableName = variableName;
        TempInExpression = inExpression;
        DeclaredType = declaredType;
        Block = block;
        ReferencedIterableDeclaration = referencedIterableDeclaration;
        ReferencedIterateMethod = referencedIterateMethod;
        IteratorAntetype = iteratorAntetype;
        IteratorType = iteratorType;
        ReferencedIteratorDeclaration = referencedIteratorDeclaration;
        ReferencedNextMethod = referencedNextMethod;
        IteratedAntetype = iteratedAntetype;
        IteratedType = iteratedType;
        FlowStateBeforeBlock = flowStateBeforeBlock;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BreakExpressionNode : SemanticNode, IBreakExpressionNode
{
    private IBreakExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public NeverType Type { [DebuggerStepThrough] get; }
    public IBreakExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode? TempValue { [DebuggerStepThrough] get; }
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BreakExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, IBreakExpressionSyntax syntax, IAmbiguousExpressionNode? value)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        TempValue = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NextExpressionNode : SemanticNode, INextExpressionNode
{
    private INextExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public NeverType Type { [DebuggerStepThrough] get; }
    public INextExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NextExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, INextExpressionSyntax syntax)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ReturnExpressionNode : SemanticNode, IReturnExpressionNode
{
    private IReturnExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public NeverType Type { [DebuggerStepThrough] get; }
    public IReturnExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode? TempValue { [DebuggerStepThrough] get; }
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentValue { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IExitNode ControlFlowExit()
        => Inherited_ControlFlowExit(GrammarAttribute.CurrentInheritanceContext());
    public DataType? ExpectedReturnType
        => GrammarAttribute.IsCached(in expectedReturnTypeCached) ? expectedReturnType
            : this.Inherited(ref expectedReturnTypeCached, ref expectedReturnType,
                Inherited_ExpectedReturnType);
    private DataType? expectedReturnType;
    private bool expectedReturnTypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ReturnExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, NeverType type, IReturnExpressionSyntax syntax, IAmbiguousExpressionNode? value, IAmbiguousExpressionNode? currentValue)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        TempValue = value;
        CurrentValue = currentValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnresolvedInvocationExpressionNode : SemanticNode, IUnresolvedInvocationExpressionNode
{
    private IUnresolvedInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnresolvedInvocationExpressionNode(ISemanticNode parent, IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IAmbiguousExpressionNode currentExpression, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments)
    {
        Parent = parent;
        Syntax = syntax;
        TempExpression = expression;
        CurrentExpression = currentExpression;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CurrentArguments = currentArguments;
    }

    protected override IChildTreeNode Rewrite()
        => OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_TypeNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_ToUnknown(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionInvocationExpressionNode : SemanticNode, IFunctionInvocationExpressionNode
{
    private IFunctionInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFunctionGroupNameNode FunctionGroup { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations { [DebuggerStepThrough] get; }
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IFunctionGroupNameNode functionGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedSet<IFunctionLikeDeclarationNode> compatibleDeclarations, IFunctionLikeDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        FunctionGroup = functionGroup;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodInvocationExpressionNode : SemanticNode, IMethodInvocationExpressionNode
{
    private IMethodInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodGroupNameNode MethodGroup { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations { [DebuggerStepThrough] get; }
    public IStandardMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IMethodGroupNameNode methodGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedList<IAmbiguousExpressionNode> currentArguments, IFixedSet<IStandardMethodDeclarationNode> compatibleDeclarations, IStandardMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        MethodGroup = methodGroup;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CurrentArguments = currentArguments;
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterInvocationExpressionNode : SemanticNode, IGetterInvocationExpressionNode
{
    private IGetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public StandardName PropertyName { [DebuggerStepThrough] get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { [DebuggerStepThrough] get; }
    public IGetterMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public GetterInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, IGetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterInvocationExpressionNode : SemanticNode, ISetterInvocationExpressionNode
{
    private ISetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IAssignmentExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public StandardName PropertyName { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempValue { [DebuggerStepThrough] get; }
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { [DebuggerStepThrough] get; }
    public ISetterMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SetterInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IAssignmentExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IAmbiguousExpressionNode value, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors, ISetterMethodDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        TempValue = value;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionReferenceInvocationExpressionNode : SemanticNode, IFunctionReferenceInvocationExpressionNode
{
    private IFunctionReferenceInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Expression { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public FunctionAntetype FunctionAntetype { [DebuggerStepThrough] get; }
    public FunctionType FunctionType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionReferenceInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments, FunctionAntetype functionAntetype, FunctionType functionType)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Expression = expression;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        FunctionAntetype = functionAntetype;
        FunctionType = functionType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerInvocationExpressionNode : SemanticNode, IInitializerInvocationExpressionNode
{
    private IInitializerInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments { [DebuggerStepThrough] get; }
    public IEnumerable<IExpressionNode?> AllArguments { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IInitializerGroupNameNode InitializerGroup { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { [DebuggerStepThrough] get; }
    public IInitializerDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public ContextualizedOverload? ContextualizedOverload { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IEnumerable<IAmbiguousExpressionNode> tempAllArguments, IEnumerable<IExpressionNode?> allArguments, IInvocationExpressionSyntax syntax, IInitializerGroupNameNode initializerGroup, IFixedList<IAmbiguousExpressionNode> arguments, IFixedSet<IInitializerDeclarationNode> compatibleDeclarations, IInitializerDeclarationNode? referencedDeclaration, ContextualizedOverload? contextualizedOverload)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        InitializerGroup = initializerGroup;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CompatibleDeclarations = compatibleDeclarations;
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownInvocationExpressionNode : SemanticNode, IUnknownInvocationExpressionNode
{
    private IUnknownInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownInvocationExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IInvocationExpressionSyntax syntax, IAmbiguousExpressionNode expression, IFixedList<IAmbiguousExpressionNode> arguments)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = expression;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierNameExpressionNode : SemanticNode, IIdentifierNameExpressionNode
{
    private IIdentifierNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedList<IDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IdentifierNameExpressionNode(ISemanticNode parent, IFixedList<IDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
    {
        Parent = parent;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericNameExpressionNode : SemanticNode, IGenericNameExpressionNode
{
    private IGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedList<IDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IGenericNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public GenericNameExpressionNode(ISemanticNode parent, IFixedList<IDeclarationNode> referencedDeclarations, IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MemberAccessExpressionNode : SemanticNode, IMemberAccessExpressionNode
{
    private IMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempContext { [DebuggerStepThrough] get; }
    public IExpressionNode? Context => TempContext as IExpressionNode;
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MemberAccessExpressionNode(ISemanticNode parent, IMemberAccessExpressionSyntax syntax, IAmbiguousExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        Syntax = syntax;
        TempContext = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PropertyNameNode : SemanticNode, IPropertyNameNode
{
    private IPropertyNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public StandardName PropertyName { [DebuggerStepThrough] get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PropertyNameNode(ISemanticNode parent, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName propertyName, IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Parent = parent;
        Syntax = syntax;
        Context = context;
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnqualifiedNamespaceNameNode : SemanticNode, IUnqualifiedNamespaceNameNode
{
    private IUnqualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnqualifiedNamespaceNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedNamespaceNameNode : SemanticNode, IQualifiedNamespaceNameNode
{
    private IQualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public INamespaceNameNode Context { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedNamespaceNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedList<INamespaceDeclarationNode> referencedDeclarations, IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IdentifierName name)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Context = context;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionGroupNameNode : SemanticNode, IFunctionGroupNameNode
{
    private IFunctionGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public INameExpressionNode? Context { [DebuggerStepThrough] get; }
    public StandardName FunctionName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionGroupNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, INameExpressionNode? context, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFixedSet<IFunctionLikeDeclarationNode> referencedDeclarations)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = context;
        FunctionName = functionName;
        TypeArguments = typeArguments;
        ReferencedDeclarations = referencedDeclarations;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionNameNode : SemanticNode, IFunctionNameNode
{
    private IFunctionNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFunctionGroupNameNode FunctionGroup { [DebuggerStepThrough] get; }
    public StandardName FunctionName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFunctionLikeDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, IFunctionGroupNameNode functionGroup, StandardName functionName, IFixedList<ITypeNode> typeArguments, IFunctionLikeDeclarationNode? referencedDeclaration)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        FunctionGroup = functionGroup;
        FunctionName = functionName;
        TypeArguments = typeArguments;
        ReferencedDeclaration = referencedDeclaration;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodGroupNameNode : SemanticNode, IMethodGroupNameNode
{
    private IMethodGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public IExpressionNode CurrentContext { [DebuggerStepThrough] get; }
    public StandardName MethodName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodGroupNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IMemberAccessExpressionSyntax syntax, IExpressionNode context, IExpressionNode currentContext, StandardName methodName, IFixedList<ITypeNode> typeArguments, IFixedSet<IStandardMethodDeclarationNode> referencedDeclarations)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = context;
        CurrentContext = currentContext;
        MethodName = methodName;
        TypeArguments = typeArguments;
        ReferencedDeclarations = referencedDeclarations;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldAccessExpressionNode : SemanticNode, IFieldAccessExpressionNode
{
    private IFieldAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public IdentifierName FieldName { [DebuggerStepThrough] get; }
    public IFieldDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FieldAccessExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IMemberAccessExpressionSyntax syntax, IExpressionNode context, IdentifierName fieldName, IFieldDeclarationNode referencedDeclaration)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = context;
        FieldName = fieldName;
        ReferencedDeclaration = referencedDeclaration;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableNameExpressionNode : SemanticNode, IVariableNameExpressionNode
{
    private IVariableNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ILocalBindingNode ReferencedDefinition { [DebuggerStepThrough] get; }
    public IFixedSet<IDataFlowNode> DataFlowPrevious { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public VariableNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IIdentifierNameExpressionSyntax syntax, IdentifierName name, ILocalBindingNode referencedDefinition, IFixedSet<IDataFlowNode> dataFlowPrevious)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Name = name;
        ReferencedDefinition = referencedDefinition;
        DataFlowPrevious = dataFlowPrevious;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardTypeNameExpressionNode : SemanticNode, IStandardTypeNameExpressionNode
{
    private IStandardTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public IStandardNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public StandardTypeNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IStandardNameExpressionSyntax syntax, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        Syntax = syntax;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameExpressionNode : SemanticNode, IQualifiedTypeNameExpressionNode
{
    private IQualifiedTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public INamespaceNameNode Context { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedTypeNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, StandardName name, ITypeDeclarationNode referencedDeclaration, IMaybeAntetype namedAntetype, BareType? namedBareType, IMemberAccessExpressionSyntax syntax, INamespaceNameNode context, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Context = context;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerGroupNameNode : SemanticNode, IInitializerGroupNameNode
{
    private IInitializerGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNameExpressionNode Context { [DebuggerStepThrough] get; }
    public StandardName? InitializerName { [DebuggerStepThrough] get; }
    public IMaybeAntetype InitializingAntetype { [DebuggerStepThrough] get; }
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerGroupNameNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, INameExpressionSyntax syntax, ITypeNameExpressionNode context, StandardName? initializerName, IMaybeAntetype initializingAntetype, IFixedSet<IInitializerDeclarationNode> referencedDeclarations)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = context;
        InitializerName = initializerName;
        InitializingAntetype = initializingAntetype;
        ReferencedDeclarations = referencedDeclarations;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SpecialTypeNameExpressionNode : SemanticNode, ISpecialTypeNameExpressionNode
{
    private ISpecialTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ISpecialTypeNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public TypeSymbol ReferencedSymbol { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SpecialTypeNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, ISpecialTypeNameExpressionSyntax syntax, SpecialTypeName name, TypeSymbol referencedSymbol, UnknownType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Name = name;
        ReferencedSymbol = referencedSymbol;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfExpressionNode : SemanticNode, ISelfExpressionNode
{
    private ISelfExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public ISelfExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public Pseudotype Pseudotype { [DebuggerStepThrough] get; }
    public ISelfParameterNode? ReferencedDefinition { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IExecutableDefinitionNode ContainingDeclaration
        => GrammarAttribute.IsCached(in containingDeclarationCached) ? containingDeclaration!
            : this.Inherited(ref containingDeclarationCached, ref containingDeclaration,
                (ctx) => (IExecutableDefinitionNode)Inherited_ContainingDeclaration(ctx));
    private IExecutableDefinitionNode? containingDeclaration;
    private bool containingDeclarationCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SelfExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, ISelfExpressionSyntax syntax, bool isImplicit, Pseudotype pseudotype, ISelfParameterNode? referencedDefinition)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        IsImplicit = isImplicit;
        Pseudotype = pseudotype;
        ReferencedDefinition = referencedDefinition;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MissingNameExpressionNode : SemanticNode, IMissingNameExpressionNode
{
    private IMissingNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IMissingNameSyntax Syntax { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MissingNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, IMissingNameSyntax syntax, UnknownType type)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownIdentifierNameExpressionNode : SemanticNode, IUnknownIdentifierNameExpressionNode
{
    private IUnknownIdentifierNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownIdentifierNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedSet<IDeclarationNode> referencedDeclarations, IIdentifierNameExpressionSyntax syntax, IdentifierName name)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownGenericNameExpressionNode : SemanticNode, IUnknownGenericNameExpressionNode
{
    private IUnknownGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IGenericNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownGenericNameExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IFixedSet<IDeclarationNode> referencedDeclarations, IGenericNameExpressionSyntax syntax, GenericName name, IFixedList<ITypeNode> typeArguments)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations;
        Syntax = syntax;
        Name = name;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownMemberAccessExpressionNode : SemanticNode, IUnknownMemberAccessExpressionNode
{
    private IUnknownMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public UnknownType Type { [DebuggerStepThrough] get; }
    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IExpressionNode Context { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IDeclarationNode> ReferencedMembers { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownMemberAccessExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, IFlowState flowStateAfter, UnknownType type, IMemberAccessExpressionSyntax syntax, IExpressionNode context, StandardName memberName, IFixedList<ITypeNode> typeArguments, IFixedSet<IDeclarationNode> referencedMembers)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        ReferencedMembers = referencedMembers;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousMoveExpressionNode : SemanticNode, IAmbiguousMoveExpressionNode
{
    private IAmbiguousMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IMoveExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IUnresolvedSimpleNameNode TempReferent { [DebuggerStepThrough] get; }
    public ISimpleNameExpressionNode? Referent => TempReferent as ISimpleNameExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AmbiguousMoveExpressionNode(ISemanticNode parent, IMoveExpressionSyntax syntax, IUnresolvedSimpleNameNode referent)
    {
        Parent = parent;
        Syntax = syntax;
        TempReferent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveVariableExpressionNode : SemanticNode, IMoveVariableExpressionNode
{
    private IMoveVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public ILocalBindingNameExpressionNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveVariableExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, ILocalBindingNameExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveValueExpressionNode : SemanticNode, IMoveValueExpressionNode
{
    private IMoveValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public IExpressionNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveValueExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, IExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitTempMoveExpressionNode : SemanticNode, IImplicitTempMoveExpressionNode
{
    private IImplicitTempMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitTempMoveExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousFreezeExpressionNode : SemanticNode, IAmbiguousFreezeExpressionNode
{
    private IAmbiguousFreezeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFreezeExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IUnresolvedSimpleNameNode TempReferent { [DebuggerStepThrough] get; }
    public ISimpleNameExpressionNode? Referent => TempReferent as ISimpleNameExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AmbiguousFreezeExpressionNode(ISemanticNode parent, IFreezeExpressionSyntax syntax, IUnresolvedSimpleNameNode referent)
    {
        Parent = parent;
        Syntax = syntax;
        TempReferent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeVariableExpressionNode : SemanticNode, IFreezeVariableExpressionNode
{
    private IFreezeVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public bool IsTemporary { [DebuggerStepThrough] get; }
    public ILocalBindingNameExpressionNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeVariableExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, bool isTemporary, ILocalBindingNameExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        IsTemporary = isTemporary;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeValueExpressionNode : SemanticNode, IFreezeValueExpressionNode
{
    private IFreezeValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public bool IsTemporary { [DebuggerStepThrough] get; }
    public IExpressionNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeValueExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, bool isImplicit, bool isTemporary, IExpressionNode referent)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        IsTemporary = isTemporary;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PrepareToReturnExpressionNode : SemanticNode, IPrepareToReturnExpressionNode
{
    private IPrepareToReturnExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionNode Value { [DebuggerStepThrough] get; }
    public IExpressionNode CurrentValue { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PrepareToReturnExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IExpressionSyntax syntax, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IExpressionNode value, IExpressionNode currentValue)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Value = value;
        CurrentValue = currentValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncBlockExpressionNode : SemanticNode, IAsyncBlockExpressionNode
{
    private IAsyncBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IAsyncBlockExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IBlockExpressionNode Block { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncBlockExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAsyncBlockExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncStartExpressionNode : SemanticNode, IAsyncStartExpressionNode
{
    private IAsyncStartExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IAsyncStartExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool Scheduled { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncStartExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAsyncStartExpressionSyntax syntax, bool scheduled, IAmbiguousExpressionNode expression)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Scheduled = scheduled;
        TempExpression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AwaitExpressionNode : SemanticNode, IAwaitExpressionNode
{
    private IAwaitExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype? ExpectedAntetype { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
    public DataType? ExpectedType { [DebuggerStepThrough] get; }
    public DataType Type { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IAwaitExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode TempExpression { [DebuggerStepThrough] get; }
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AwaitExpressionNode(ISemanticNode parent, ControlFlowSet controlFlowNext, ControlFlowSet controlFlowPrevious, IMaybeExpressionAntetype? expectedAntetype, IMaybeExpressionAntetype antetype, DataType? expectedType, DataType type, IFlowState flowStateAfter, IAwaitExpressionSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Parent = parent;
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ExpectedAntetype = expectedAntetype;
        Antetype = antetype;
        ExpectedType = expectedType;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageSymbolNode : SemanticNode, IPackageSymbolNode
{
    private IPackageSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? AliasOrName { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public PackageSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageFacetDeclarationNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetDeclarationNode TestingFacet { [DebuggerStepThrough] get; }

    public PackageSymbolNode(ISyntax? syntax, IdentifierName? aliasOrName, IdentifierName name, PackageSymbol symbol, IPackageFacetDeclarationNode mainFacet, IPackageFacetDeclarationNode testingFacet)
    {
        Syntax = syntax;
        AliasOrName = aliasOrName;
        Name = name;
        Symbol = symbol;
        MainFacet = mainFacet;
        TestingFacet = testingFacet;
    }

    internal override IPackageDeclarationNode Inherited_Package(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageFacetSymbolNode : SemanticNode, IPackageFacetSymbolNode
{
    private IPackageFacetSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? PackageAliasOrName { [DebuggerStepThrough] get; }
    public IdentifierName PackageName { [DebuggerStepThrough] get; }
    public PackageSymbol Symbol { [DebuggerStepThrough] get; }
    public INamespaceDeclarationNode GlobalNamespace { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());

    public PackageFacetSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName? packageAliasOrName, IdentifierName packageName, PackageSymbol symbol, INamespaceDeclarationNode globalNamespace)
    {
        Syntax = syntax;
        Parent = parent;
        PackageAliasOrName = packageAliasOrName;
        PackageName = packageName;
        Symbol = symbol;
        GlobalNamespace = globalNamespace;
    }

    internal override IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceSymbolNode : SemanticNode, INamespaceSymbolNode
{
    private INamespaceSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;

    public NamespaceSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName name, NamespaceSymbol symbol, IFixedList<INamespaceMemberDeclarationNode> nestedMembers, IFixedList<INamespaceMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
        NestedMembers = nestedMembers;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionSymbolNode : SemanticNode, IFunctionSymbolNode
{
    private IFunctionSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public FunctionSymbolNode(ISyntax? syntax, ISemanticNode parent, FunctionSymbol symbol, StandardName name, FunctionType type)
    {
        Syntax = syntax;
        Parent = parent;
        Symbol = symbol;
        Name = name;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PrimitiveTypeSymbolNode : SemanticNode, IPrimitiveTypeSymbolNode
{
    private IPrimitiveTypeSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public TypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.PrimitiveTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.PrimitiveTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public PrimitiveTypeSymbolNode(ISyntax? syntax, ISemanticNode parent, TypeSymbol symbol, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, SpecialTypeName name, IFixedSet<ITypeMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Symbol = symbol;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Name = name;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UserTypeSymbolNode : SemanticNode, IUserTypeSymbolNode
{
    private IUserTypeSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public UserTypeSymbolNode(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITypeMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Supertypes = supertypes;
        Name = name;
        Symbol = symbol;
        InclusiveMembers = inclusiveMembers;
        GenericParameters = genericParameters;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassSymbolNode : SemanticNode, IClassSymbolNode
{
    private IClassSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<IClassMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public ClassSymbolNode(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<IClassMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IClassMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Supertypes = supertypes;
        Name = name;
        Symbol = symbol;
        InclusiveMembers = inclusiveMembers;
        GenericParameters = genericParameters;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructSymbolNode : SemanticNode, IStructSymbolNode
{
    private IStructSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<IStructMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public StructSymbolNode(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<IStructMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<IStructMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Supertypes = supertypes;
        Name = name;
        Symbol = symbol;
        InclusiveMembers = inclusiveMembers;
        GenericParameters = genericParameters;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitSymbolNode : SemanticNode, ITraitSymbolNode
{
    private ITraitSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<ITraitMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;

    public TraitSymbolNode(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, StandardName name, UserTypeSymbol symbol, IFixedSet<ITraitMemberDeclarationNode> inclusiveMembers, IFixedList<IGenericParameterDeclarationNode> genericParameters, IFixedSet<ITraitMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Supertypes = supertypes;
        Name = name;
        Symbol = symbol;
        InclusiveMembers = inclusiveMembers;
        GenericParameters = genericParameters;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterSymbolNode : SemanticNode, IGenericParameterSymbolNode
{
    private IGenericParameterSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IFixedSet<BareReferenceType> Supertypes { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public GenericParameterTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberDeclarationNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public GenericParameterSymbolNode(ISyntax? syntax, ISemanticNode parent, IFixedSet<BareReferenceType> supertypes, IFixedSet<ITypeMemberDeclarationNode> inclusiveMembers, IdentifierName name, GenericParameterTypeSymbol symbol, IFixedSet<ITypeMemberDeclarationNode> members)
    {
        Syntax = syntax;
        Parent = parent;
        Supertypes = supertypes;
        InclusiveMembers = inclusiveMembers;
        Name = name;
        Symbol = symbol;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardMethodSymbolNode : SemanticNode, IStandardMethodSymbolNode
{
    private IStandardMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public int Arity { [DebuggerStepThrough] get; }
    public FunctionType MethodGroupType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public StandardMethodSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol, int arity, FunctionType methodGroupType)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
        Arity = arity;
        MethodGroupType = methodGroupType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodSymbolNode : SemanticNode, IGetterMethodSymbolNode
{
    private IGetterMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public GetterMethodSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodSymbolNode : SemanticNode, ISetterMethodSymbolNode
{
    private ISetterMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public SetterMethodSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName name, MethodSymbol symbol)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSymbolNode : SemanticNode, IConstructorSymbolNode
{
    private IConstructorSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public ConstructorSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public ConstructorSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName? name, ConstructorSymbol symbol)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSymbolNode : SemanticNode, IInitializerSymbolNode
{
    private IInitializerSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public InitializerSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public InitializerSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName? name, InitializerSymbol symbol)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldSymbolNode : SemanticNode, IFieldSymbolNode
{
    private IFieldSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public FieldSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public FieldSymbolNode(ISyntax? syntax, ISemanticNode parent, IdentifierName name, DataType bindingType, FieldSymbol symbol)
    {
        Syntax = syntax;
        Parent = parent;
        Name = name;
        BindingType = bindingType;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssociatedFunctionSymbolNode : SemanticNode, IAssociatedFunctionSymbolNode
{
    private IAssociatedFunctionSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public ISemanticNode Parent { [DebuggerStepThrough] get; }
    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public AssociatedFunctionSymbolNode(ISyntax? syntax, ISemanticNode parent, FunctionSymbol symbol, FunctionType type, StandardName name)
    {
        Syntax = syntax;
        Parent = parent;
        Symbol = symbol;
        Type = type;
        Name = name;
    }
}

