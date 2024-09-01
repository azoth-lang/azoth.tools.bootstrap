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
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
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

#nullable enable
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
    IPackageDeclarationNode Package { get; }
    ISemanticNode Parent => (ISemanticNode)PeekParent()!;
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
    FixedDictionary<IdentifierName, IPackageDeclarationNode> PackageDeclarations { get; }
    new IdentifierName? AliasOrName
        => null;
    IdentifierName? IPackageDeclarationNode.AliasOrName => AliasOrName;
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IPackageDeclarationNode.Name => Name;
    IFunctionDefinitionNode? EntryPoint { get; }
    DiagnosticCollection Diagnostics { get; }
    new PackageSymbol Symbol { get; }
    PackageSymbol IPackageDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IPackageSymbols PackageSymbols { get; }
    IPackageReferenceNode IntrinsicsReference { get; }
    IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations { get; }

    public static IPackageNode Create(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IPackageFacetNode mainFacet,
        IPackageFacetNode testingFacet)
        => new PackageNode(syntax, references, mainFacet, testingFacet);
}

[Closed(
    typeof(IStandardPackageReferenceNode),
    typeof(IIntrinsicsPackageReferenceNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPackageSymbolNode SymbolNode { get; }
    IPackageSymbols PackageSymbols { get; }
    IdentifierName AliasOrName { get; }
    bool IsTrusted { get; }
}

// [Closed(typeof(StandardPackageReferenceNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardPackageReferenceNode : IPackageReferenceNode
{
    new IPackageReferenceSyntax Syntax { get; }
    IPackageReferenceSyntax? IPackageReferenceNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPackageSymbols IPackageReferenceNode.PackageSymbols
        => Syntax.Package;
    IdentifierName IPackageReferenceNode.AliasOrName
        => Syntax.AliasOrName;
    bool IPackageReferenceNode.IsTrusted
        => Syntax.IsTrusted;

    public static IStandardPackageReferenceNode Create(
        IPackageReferenceSyntax syntax)
        => new StandardPackageReferenceNode(syntax);
}

// [Closed(typeof(IntrinsicsPackageReferenceNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntrinsicsPackageReferenceNode : IPackageReferenceNode
{
    new IPackageReferenceSyntax? Syntax
        => null;
    IPackageReferenceSyntax? IPackageReferenceNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPackageSymbols IPackageReferenceNode.PackageSymbols
        => IntrinsicPackageSymbol.Instance;
    IdentifierName IPackageReferenceNode.AliasOrName
        => PackageSymbols.PackageSymbol.Name;
    bool IPackageReferenceNode.IsTrusted
        => true;

    public static IIntrinsicsPackageReferenceNode Create(
        )
        => new IntrinsicsPackageReferenceNode();
}

// [Closed(typeof(PackageFacetNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetNode : IPackageFacetDeclarationNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    PackageNameScope PackageNameScope { get; }
    new INamespaceDefinitionNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;
    IFixedSet<IFacetMemberDefinitionNode> Definitions { get; }

    public static IPackageFacetNode Create(
        IPackageSyntax syntax,
        IEnumerable<ICompilationUnitNode> compilationUnits)
        => new PackageFacetNode(syntax, compilationUnits);
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(ITypeDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFacetMemberDefinitionNode : INamespaceBlockMemberDefinitionNode, INamespaceMemberDefinitionNode
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
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    NamespaceScope ContainingLexicalScope { get; }
    NamespaceSearchScope LexicalScope { get; }
    new CodeFile File
        => Syntax.File;
    CodeFile ICodeNode.File => File;
    IPackageFacetNode ContainingDeclaration { get; }
    INamespaceDefinitionNode ImplicitNamespace { get; }
    DiagnosticCollection Diagnostics { get; }
    NamespaceName ImplicitNamespaceName
        => Syntax.ImplicitNamespaceName;
    NamespaceSymbol ImplicitNamespaceSymbol
        => ImplicitNamespace.Symbol;
    PackageSymbol ContainingSymbol
        => ContainingDeclaration.PackageSymbol;

    public static ICompilationUnitNode Create(
        ICompilationUnitSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> definitions)
        => new CompilationUnitNode(syntax, usingDirectives, definitions);
}

// [Closed(typeof(UsingDirectiveNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUsingDirectiveNode : ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    NamespaceName Name
        => Syntax.Name;

    public static IUsingDirectiveNode Create(
        IUsingDirectiveSyntax syntax)
        => new UsingDirectiveNode(syntax);
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
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    new IPackageFacetNode Facet { get; }
    IPackageFacetDeclarationNode IPackageFacetChildDeclarationNode.Facet => Facet;
    ISymbolDeclarationNode ContainingDeclaration { get; }
    Symbol ContainingSymbol
        => ContainingDeclaration.Symbol;
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDefinitionNode : IDefinitionNode, IInvocableDeclarationNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    ValueIdScope ValueIdScope { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExecutableDefinitionNode : IDefinitionNode, ISymbolDeclarationNode
{
    ValueIdScope ValueIdScope { get; }
    IEntryNode Entry { get; }
    IExitNode Exit { get; }
    FixedDictionary<IVariableBindingNode, int> VariableBindingsMap { get; }
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
    IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.ConcreteInvocableDefinition_FlowStateBefore(this);
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
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(NamespaceBlockDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockDefinitionNode : INamespaceBlockMemberDefinitionNode
{
    new INamespaceDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    new NamespaceSearchScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    new NamespaceSearchScope LexicalScope { get; }
    LexicalScope IDefinitionNode.LexicalScope => LexicalScope;
    new INamespaceDefinitionNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    INamespaceDefinitionNode ContainingNamespace { get; }
    INamespaceDefinitionNode Definition { get; }
    NamespaceName DeclaredNames
        => Syntax.DeclaredNames;
    bool IsGlobalQualified
        => Syntax.IsGlobalQualified;
    new NamespaceSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    NamespaceSymbol Symbol
        => Definition.Symbol;
    StandardName? IPackageFacetChildDeclarationNode.Name
        => DeclaredNames.Segments.LastOrDefault();

    public static INamespaceBlockDefinitionNode Create(
        INamespaceDefinitionSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> members)
        => new NamespaceBlockDefinitionNode(syntax, usingDirectives, members);
}

[Closed(
    typeof(IFacetMemberDefinitionNode),
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
    IFixedList<IFacetMemberDefinitionNode> PackageMembers { get; }
    new ISyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;

    public static INamespaceDefinitionNode Create(
        NamespaceSymbol symbol,
        IEnumerable<INamespaceDefinitionNode> memberNamespaces,
        IEnumerable<IFacetMemberDefinitionNode> packageMembers)
        => new NamespaceDefinitionNode(symbol, memberNamespaces, packageMembers);
}

[Closed(
    typeof(IFacetMemberDefinitionNode),
    typeof(INamespaceDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberDefinitionNode : INamespaceMemberDeclarationNode
{
}

// [Closed(typeof(FunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDefinitionNode : IFacetMemberDefinitionNode, IFunctionDeclarationNode, IConcreteFunctionInvocableDefinitionNode
{
    new IFunctionDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new FunctionType Type { get; }
    FunctionType IFunctionLikeDeclarationNode.Type => Type;
    FunctionType IConcreteFunctionInvocableDefinitionNode.Type => Type;
    new INamespaceDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    new IdentifierName Name
        => Syntax.Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    new NamespaceSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    Symbol IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    FunctionSymbol IConcreteFunctionInvocableDefinitionNode.Symbol => Symbol;

    public static IFunctionDefinitionNode Create(
        IFunctionDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        => new FunctionDefinitionNode(syntax, attributes, parameters, @return, body);
}

[Closed(
    typeof(IClassDefinitionNode),
    typeof(IStructDefinitionNode),
    typeof(ITraitDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDefinitionNode : IFacetMemberDefinitionNode, IAssociatedMemberDefinitionNode, IUserTypeDeclarationNode
{
    new ITypeDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    LexicalScope SupertypesLexicalScope { get; }
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames
        => SupertypeNames;
    bool IsConst
        => Syntax.ConstModifier is not null;
    new StandardName Name
        => Syntax.Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    StandardName IAssociatedMemberDefinitionNode.Name => Name;
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    UserTypeSymbol IUserTypeDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    new IFixedSet<BareReferenceType> Supertypes { get; }
    IFixedSet<BareReferenceType> ITypeDeclarationNode.Supertypes => Supertypes;
    IDeclaredUserType DeclaredType { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new AccessModifier AccessModifier { get; }
    AccessModifier IFacetMemberDefinitionNode.AccessModifier => AccessModifier;
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
    IStandardTypeNameNode? BaseTypeName { get; }
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    bool IsAbstract
        => Syntax.AbstractModifier is not null;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedSet<IClassMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IDefaultConstructorDefinitionNode? DefaultConstructor { get; }
    IEnumerable<IStandardTypeNameNode> ITypeDefinitionNode.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);

    public static IClassDefinitionNode Create(
        IClassDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IStandardTypeNameNode? baseTypeName,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDefinitionNode> sourceMembers)
        => new ClassDefinitionNode(syntax, attributes, genericParameters, baseTypeName, supertypeNames, sourceMembers);
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
    IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    new StructType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;
    new IFixedSet<IStructMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IDefaultInitializerDefinitionNode? DefaultInitializer { get; }

    public static IStructDefinitionNode Create(
        IStructDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDefinitionNode> sourceMembers)
        => new StructDefinitionNode(syntax, attributes, genericParameters, supertypeNames, sourceMembers);
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
    new IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
    new ObjectType DeclaredType { get; }
    IDeclaredUserType ITypeDefinitionNode.DeclaredType => DeclaredType;

    public static ITraitDefinitionNode Create(
        ITraitDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<ITraitMemberDefinitionNode> members)
        => new TraitDefinitionNode(syntax, attributes, genericParameters, supertypeNames, members);
}

// [Closed(typeof(GenericParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterNode : ICodeNode, IGenericParameterDeclarationNode
{
    new IGenericParameterSyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    IUserTypeDeclarationNode ContainingDeclaration { get; }
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IGenericParameterDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeParameterIndependence Independence
        => Syntax.Independence;
    TypeParameterVariance Variance
        => Syntax.Variance;
    UserTypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    new GenericParameterTypeSymbol Symbol { get; }
    GenericParameterTypeSymbol IGenericParameterDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    IDeclaredUserType ContainingDeclaredType { get; }
    GenericParameter Parameter { get; }
    GenericParameterType DeclaredType { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> IGenericParameterDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];
    IFixedSet<BareReferenceType> ITypeDeclarationNode.Supertypes
        => [];

    public static IGenericParameterNode Create(
        IGenericParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
        => new GenericParameterNode(syntax, constraint);
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
    new IUserTypeDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDefinitionNode.ContainingDeclaration => ContainingDeclaration;
    new UserTypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
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
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IdentifierName Name
        => Syntax.Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IMethodDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    MethodKind Kind { get; }
    new MethodSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    MethodSymbol IMethodDeclarationNode.Symbol => Symbol;
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
    IDeclaredUserType ContainingDeclaredType { get; }
    FunctionType IStandardMethodDeclarationNode.MethodGroupType
        => Symbol.MethodGroupType;
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Standard;
    int IStandardMethodDeclarationNode.Arity
        => Parameters.Count;

    public static IAbstractMethodDefinitionNode Create(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        => new AbstractMethodDefinitionNode(syntax, selfParameter, parameters, @return);
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
    FunctionType IStandardMethodDeclarationNode.MethodGroupType
        => Symbol.MethodGroupType;
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Standard;
    int IStandardMethodDeclarationNode.Arity
        => Parameters.Count;

    public static IStandardMethodDefinitionNode Create(
        IStandardMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        => new StandardMethodDefinitionNode(syntax, selfParameter, parameters, @return, body);
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
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Getter;

    public static IGetterMethodDefinitionNode Create(
        IGetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode @return,
        IBodyNode body)
        => new GetterMethodDefinitionNode(syntax, selfParameter, parameters, @return, body);
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
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Setter;

    public static ISetterMethodDefinitionNode Create(
        ISetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        => new SetterMethodDefinitionNode(syntax, selfParameter, parameters, @return, body);
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
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    ConstructorSymbol IConstructorDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{

    public static IDefaultConstructorDefinitionNode Create(
        IConstructorDefinitionSyntax? syntax,
        IdentifierName? name,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBodyNode? body)
        => new DefaultConstructorDefinitionNode(syntax, name, parameters, body);
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

    public static ISourceConstructorDefinitionNode Create(
        IdentifierName? name,
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        => new SourceConstructorDefinitionNode(name, syntax, selfParameter, parameters, body);
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
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    InitializerSymbol IInitializerDeclarationNode.Symbol => Symbol;
}

// [Closed(typeof(DefaultInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultInitializerDefinitionNode : IInitializerDefinitionNode
{

    public static IDefaultInitializerDefinitionNode Create(
        IInitializerDefinitionSyntax? syntax,
        IdentifierName? name,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBodyNode? body)
        => new DefaultInitializerDefinitionNode(syntax, name, parameters, body);
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

    public static ISourceInitializerDefinitionNode Create(
        IdentifierName? name,
        IInitializerDefinitionSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        => new SourceInitializerDefinitionNode(name, syntax, selfParameter, parameters, body);
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

    public static IFieldDefinitionNode Create(
        bool isLentBinding,
        IFieldDefinitionSyntax syntax,
        bool isMutableBinding,
        IdentifierName name,
        ITypeNode typeNode,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IAmbiguousExpressionNode? initializer,
        IAmbiguousExpressionNode? currentInitializer)
        => new FieldDefinitionNode(isLentBinding, syntax, isMutableBinding, name, typeNode, bindingAntetype, bindingType, initializer, currentInitializer);
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
    InvocableSymbol IInvocableDeclarationNode.Symbol => Symbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol IFunctionLikeDeclarationNode.Symbol => Symbol;

    public static IAssociatedFunctionDefinitionNode Create(
        IAssociatedFunctionDefinitionSyntax syntax,
        IdentifierName name,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        FunctionType type,
        IBodyNode body)
        => new AssociatedFunctionDefinitionNode(syntax, name, parameters, @return, type, body);
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

    public static IAttributeNode Create(
        IAttributeSyntax syntax,
        IStandardTypeNameNode typeName)
        => new AttributeNode(syntax, typeName);
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

    public static ICapabilitySetNode Create(
        ICapabilitySetSyntax syntax,
        CapabilitySet constraint)
        => new CapabilitySetNode(syntax, constraint);
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

    public static ICapabilityNode Create(
        ICapabilityConstraint constraint,
        ICapabilitySyntax syntax,
        Capability capability)
        => new CapabilityNode(constraint, syntax, capability);
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

    public static INamedParameterNode Create(
        bool unused,
        IFlowState flowStateAfter,
        ParameterType parameterType,
        INamedParameterSyntax syntax,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        ITypeNode typeNode,
        IMaybeAntetype bindingAntetype,
        DataType bindingType)
        => new NamedParameterNode(unused, flowStateAfter, parameterType, syntax, isMutableBinding, isLentBinding, name, typeNode, bindingAntetype, bindingType);
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
    SelfParameterType ParameterType { get; }
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IDeclaredUserType ContainingDeclaredType { get; }
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

    public static IConstructorSelfParameterNode Create(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        IConstructorSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityNode capability,
        CapabilityType bindingType)
        => new ConstructorSelfParameterNode(name, unused, flowStateAfter, parameterType, bindingAntetype, syntax, isLentBinding, capability, bindingType);
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

    public static IInitializerSelfParameterNode Create(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        IInitializerSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityNode capability,
        CapabilityType bindingType)
        => new InitializerSelfParameterNode(name, unused, flowStateAfter, parameterType, bindingAntetype, syntax, isLentBinding, capability, bindingType);
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

    public static IMethodSelfParameterNode Create(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        Pseudotype bindingType,
        IMethodSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityConstraintNode capability)
        => new MethodSelfParameterNode(name, unused, flowStateAfter, parameterType, bindingAntetype, bindingType, syntax, isLentBinding, capability);
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

    public static IFieldParameterNode Create(
        bool unused,
        IMaybeAntetype bindingAntetype,
        IFlowState flowStateAfter,
        DataType bindingType,
        ParameterType parameterType,
        IFieldParameterSyntax syntax,
        IdentifierName name)
        => new FieldParameterNode(unused, bindingAntetype, flowStateAfter, bindingType, parameterType, syntax, name);
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

    public static IBlockBodyNode Create(
        IFlowState flowStateAfter,
        IBlockBodySyntax syntax,
        IEnumerable<IBodyStatementNode> statements)
        => new BlockBodyNode(flowStateAfter, syntax, statements);
}

// [Closed(typeof(ExpressionBodyNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IResultStatementNode ResultStatement { get; }
    DataType? ExpectedType { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements
        => FixedList.Create(ResultStatement);

    public static IExpressionBodyNode Create(
        IFlowState flowStateAfter,
        IExpressionBodySyntax syntax,
        IResultStatementNode resultStatement)
        => new ExpressionBodyNode(flowStateAfter, syntax, resultStatement);
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
    new StandardName Name { get; }
    TypeName ITypeNameNode.Name => Name;
    ITypeDeclarationNode? ReferencedDeclaration { get; }
    bool IsAttributeType { get; }
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

    public static IIdentifierTypeNameNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        IIdentifierTypeNameSyntax syntax,
        IdentifierName name)
        => new IdentifierTypeNameNode(namedAntetype, namedType, namedBareType, syntax, name);
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

    public static ISpecialTypeNameNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        ISpecialTypeNameSyntax syntax,
        SpecialTypeName name)
        => new SpecialTypeNameNode(namedAntetype, namedType, namedBareType, syntax, name);
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

    public static IGenericTypeNameNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        IGenericTypeNameSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
        => new GenericTypeNameNode(namedAntetype, namedType, namedBareType, syntax, name, typeArguments);
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

    public static IQualifiedTypeNameNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        TypeName name,
        BareType? namedBareType,
        IQualifiedTypeNameSyntax syntax,
        ITypeNameNode context,
        IStandardTypeNameNode qualifiedName)
        => new QualifiedTypeNameNode(namedAntetype, namedType, name, namedBareType, syntax, context, qualifiedName);
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

    public static IOptionalTypeNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        IOptionalTypeSyntax syntax,
        ITypeNode referent)
        => new OptionalTypeNode(namedAntetype, namedType, syntax, referent);
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

    public static ICapabilityTypeNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ICapabilityTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
        => new CapabilityTypeNode(namedAntetype, namedType, syntax, capability, referent);
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

    public static IFunctionTypeNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        IFunctionTypeSyntax syntax,
        IEnumerable<IParameterTypeNode> parameters,
        ITypeNode @return)
        => new FunctionTypeNode(namedAntetype, namedType, syntax, parameters, @return);
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

    public static IParameterTypeNode Create(
        IParameterTypeSyntax syntax,
        bool isLent,
        ITypeNode referent,
        ParameterType parameter)
        => new ParameterTypeNode(syntax, isLent, referent, parameter);
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

    public static ICapabilityViewpointTypeNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
        => new CapabilityViewpointTypeNode(namedAntetype, namedType, syntax, capability, referent);
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
    Pseudotype? MethodSelfType { get; }

    public static ISelfViewpointTypeNode Create(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ISelfViewpointTypeSyntax syntax,
        ITypeNode referent)
        => new SelfViewpointTypeNode(namedAntetype, namedType, syntax, referent);
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

    public static IEntryNode Create(
        ICodeSyntax? syntax,
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned)
        => new EntryNode(syntax, controlFlowNext, controlFlowPrevious, dataFlowPrevious, definitelyAssigned, definitelyUnassigned);
}

// [Closed(typeof(ExitNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExitNode : IDataFlowNode
{

    public static IExitNode Create(
        ICodeSyntax? syntax,
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned)
        => new ExitNode(syntax, controlFlowNext, controlFlowPrevious, dataFlowPrevious, definitelyAssigned, definitelyUnassigned);
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
    DataType? ExpectedType { get; }
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
    new IFlowState FlowStateAfter { get; }
    IFlowState IStatementNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    ValueId? IStatementNode.ResultValueId
        => ValueId;
    ValueId IElseClauseNode.ValueId
        => Expression?.ValueId ?? default;

    public static IResultStatementNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IMaybeAntetype antetype,
        DataType type,
        IResultStatementSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression,
        IFlowState flowStateAfter)
        => new ResultStatementNode(controlFlowNext, controlFlowPrevious, resultAntetype, resultType, antetype, type, syntax, expression, currentExpression, flowStateAfter);
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

    public static IVariableDeclarationStatementNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IVariableDeclarationStatementSyntax syntax,
        bool isMutableBinding,
        IdentifierName name,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer,
        IAmbiguousExpressionNode? currentInitializer)
        => new VariableDeclarationStatementNode(controlFlowNext, controlFlowPrevious, resultAntetype, resultType, flowStateAfter, isLentBinding, bindingAntetype, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, name, capability, type, initializer, currentInitializer);
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

    public static IExpressionStatementNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IFlowState flowStateAfter,
        IExpressionStatementSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression)
        => new ExpressionStatementNode(controlFlowNext, controlFlowPrevious, resultAntetype, resultType, flowStateAfter, syntax, expression, currentExpression);
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

    public static IBindingContextPatternNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        IBindingContextPatternSyntax syntax,
        bool isMutableBinding,
        IPatternNode pattern,
        ITypeNode? type)
        => new BindingContextPatternNode(controlFlowNext, controlFlowPrevious, flowStateAfter, syntax, isMutableBinding, pattern, type);
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

    public static IBindingPatternNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IBindingPatternSyntax syntax,
        bool isMutableBinding,
        IdentifierName name)
        => new BindingPatternNode(controlFlowNext, controlFlowPrevious, flowStateAfter, isLentBinding, bindingAntetype, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, name);
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

    public static IOptionalPatternNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        IOptionalPatternSyntax syntax,
        IOptionalOrBindingPatternNode pattern)
        => new OptionalPatternNode(controlFlowNext, controlFlowPrevious, flowStateAfter, syntax, pattern);
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
    IMaybeExpressionAntetype Antetype { get; }
    DataType Type { get; }
    IFlowState FlowStateAfter { get; }
    DataType? ExpectedType { get; }
    bool ImplicitRecoveryAllowed();
    bool ShouldPrepareToReturn();
    IMaybeExpressionAntetype? ExpectedAntetype { get; }
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

    public static IBlockExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IBlockExpressionSyntax syntax,
        IEnumerable<IStatementNode> statements,
        IMaybeAntetype antetype,
        DataType type,
        IFlowState flowStateAfter)
        => new BlockExpressionNode(controlFlowNext, controlFlowPrevious, syntax, statements, antetype, type, flowStateAfter);
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

    public static INewObjectExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        INewObjectExpressionSyntax syntax,
        ITypeNameNode constructingType,
        IdentifierName? constructorName,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IMaybeAntetype constructingAntetype,
        IEnumerable<IConstructorDeclarationNode> referencedConstructors,
        IEnumerable<IConstructorDeclarationNode> compatibleConstructors,
        IConstructorDeclarationNode? referencedConstructor,
        ContextualizedOverload? contextualizedOverload)
        => new NewObjectExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, constructingType, constructorName, arguments, constructingAntetype, referencedConstructors, compatibleConstructors, referencedConstructor, contextualizedOverload);
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

    public static IUnsafeExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IUnsafeExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new UnsafeExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, expression);
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

    public static IBoolLiteralExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IBoolLiteralExpressionSyntax syntax,
        bool value,
        BoolConstValueType type)
        => new BoolLiteralExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, value, type);
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

    public static IIntegerLiteralExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IIntegerLiteralExpressionSyntax syntax,
        BigInteger value,
        IntegerConstValueType type)
        => new IntegerLiteralExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, value, type);
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

    public static INoneLiteralExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        INoneLiteralExpressionSyntax syntax,
        OptionalType type)
        => new NoneLiteralExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, type);
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

    public static IStringLiteralExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IStringLiteralExpressionSyntax syntax,
        string value,
        DataType type)
        => new StringLiteralExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, value, type);
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

    public static IAssignmentExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IAssignmentExpressionSyntax syntax,
        IAmbiguousAssignableExpressionNode leftOperand,
        IAmbiguousAssignableExpressionNode currentLeftOperand,
        AssignmentOperator @operator,
        IAmbiguousExpressionNode rightOperand,
        IAmbiguousExpressionNode currentRightOperand)
        => new AssignmentExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, leftOperand, currentLeftOperand, @operator, rightOperand, currentRightOperand);
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

    public static IBinaryOperatorExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        BinaryOperator @operator,
        IAmbiguousExpressionNode rightOperand,
        IAntetype? numericOperatorCommonAntetype)
        => new BinaryOperatorExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, leftOperand, @operator, rightOperand, numericOperatorCommonAntetype);
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

    public static IUnaryOperatorExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IUnaryOperatorExpressionSyntax syntax,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IAmbiguousExpressionNode operand)
        => new UnaryOperatorExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, fixity, @operator, operand);
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

    public static IIdExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IIdExpressionSyntax syntax,
        IAmbiguousExpressionNode referent)
        => new IdExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, referent);
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

    public static IConversionExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IConversionExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        ConversionOperator @operator,
        ITypeNode convertToType)
        => new ConversionExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, referent, @operator, convertToType);
}

// [Closed(typeof(ImplicitConversionExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitConversionExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    new SimpleAntetype Antetype { get; }
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;

    public static IImplicitConversionExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode referent,
        IExpressionNode currentReferent,
        SimpleAntetype antetype)
        => new ImplicitConversionExpressionNode(controlFlowNext, controlFlowPrevious, syntax, type, flowStateAfter, referent, currentReferent, antetype);
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

    public static IPatternMatchExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
        => new PatternMatchExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, referent, pattern);
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

    public static IIfExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IIfExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause,
        IFlowState flowStateAfter)
        => new IfExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, syntax, condition, thenBlock, elseClause, flowStateAfter);
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

    public static ILoopExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        ILoopExpressionSyntax syntax,
        IBlockExpressionNode block)
        => new LoopExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, block);
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

    public static IWhileExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
        => new WhileExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, condition, block);
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

    public static IForeachExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        IdentifierName name,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IForeachExpressionSyntax syntax,
        bool isMutableBinding,
        IdentifierName variableName,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? declaredType,
        IBlockExpressionNode block,
        ITypeDeclarationNode? referencedIterableDeclaration,
        IStandardMethodDeclarationNode? referencedIterateMethod,
        IMaybeExpressionAntetype iteratorAntetype,
        DataType iteratorType,
        ITypeDeclarationNode? referencedIteratorDeclaration,
        IStandardMethodDeclarationNode? referencedNextMethod,
        IMaybeAntetype iteratedAntetype,
        DataType iteratedType,
        IFlowState flowStateBeforeBlock)
        => new ForeachExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, isLentBinding, bindingAntetype, name, bindingType, dataFlowPrevious, definitelyAssigned, definitelyUnassigned, syntax, isMutableBinding, variableName, inExpression, declaredType, block, referencedIterableDeclaration, referencedIterateMethod, iteratorAntetype, iteratorType, referencedIteratorDeclaration, referencedNextMethod, iteratedAntetype, iteratedType, flowStateBeforeBlock);
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

    public static IBreakExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        IBreakExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
        => new BreakExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, syntax, value);
}

// [Closed(typeof(NextExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INextExpressionNode : INeverTypedExpressionNode
{
    new INextExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax? ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;

    public static INextExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        INextExpressionSyntax syntax)
        => new NextExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, syntax);
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

    public static IReturnExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        IReturnExpressionSyntax syntax,
        IAmbiguousExpressionNode? value,
        IAmbiguousExpressionNode? currentValue)
        => new ReturnExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, syntax, value, currentValue);
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

    public static IUnresolvedInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IAmbiguousExpressionNode> currentArguments)
        => new UnresolvedInvocationExpressionNode(syntax, expression, currentExpression, arguments, currentArguments);
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

    public static IFunctionInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IFunctionLikeDeclarationNode> compatibleDeclarations,
        IFunctionLikeDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
        => new FunctionInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, functionGroup, arguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
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

    public static IMethodInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IAmbiguousExpressionNode> currentArguments,
        IEnumerable<IStandardMethodDeclarationNode> compatibleDeclarations,
        IStandardMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
        => new MethodInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, methodGroup, arguments, currentArguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
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

    public static IGetterInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        IGetterMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
        => new GetterInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, context, propertyName, referencedPropertyAccessors, referencedDeclaration, contextualizedOverload);
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

    public static ISetterInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        ISetterMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
        => new SetterInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, context, propertyName, value, referencedPropertyAccessors, referencedDeclaration, contextualizedOverload);
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

    public static IFunctionReferenceInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        FunctionAntetype functionAntetype,
        FunctionType functionType)
        => new FunctionReferenceInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, expression, arguments, functionAntetype, functionType);
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

    public static IInitializerInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IInitializerDeclarationNode> compatibleDeclarations,
        IInitializerDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
        => new InitializerInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, tempAllArguments, allArguments, syntax, initializerGroup, arguments, compatibleDeclarations, referencedDeclaration, contextualizedOverload);
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

    public static IUnknownInvocationExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new UnknownInvocationExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, expression, arguments);
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

    public static IIdentifierNameExpressionNode Create(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
        => new IdentifierNameExpressionNode(referencedDeclarations, syntax, name);
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

    public static IGenericNameExpressionNode Create(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
        => new GenericNameExpressionNode(referencedDeclarations, syntax, name, typeArguments);
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

    public static IMemberAccessExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        IAmbiguousExpressionNode context,
        StandardName memberName,
        IEnumerable<ITypeNode> typeArguments)
        => new MemberAccessExpressionNode(syntax, context, memberName, typeArguments);
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

    public static IPropertyNameNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
        => new PropertyNameNode(syntax, context, propertyName, referencedPropertyAccessors);
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

    public static IUnqualifiedNamespaceNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
        => new UnqualifiedNamespaceNameNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, referencedDeclarations, syntax, name);
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

    public static IQualifiedNamespaceNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations,
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IdentifierName name)
        => new QualifiedNamespaceNameNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, referencedDeclarations, syntax, context, name);
}

// [Closed(typeof(FunctionGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionGroupNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    StandardName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionLikeDeclarationNode> ReferencedDeclarations { get; }

    public static IFunctionGroupNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionLikeDeclarationNode> referencedDeclarations)
        => new FunctionGroupNameNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, context, functionName, typeArguments, referencedDeclarations);
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

    public static IFunctionNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IFunctionLikeDeclarationNode? referencedDeclaration)
        => new FunctionNameNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, functionGroup, functionName, typeArguments, referencedDeclaration);
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

    public static IMethodGroupNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IExpressionNode currentContext,
        StandardName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IStandardMethodDeclarationNode> referencedDeclarations)
        => new MethodGroupNameNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, context, currentContext, methodName, typeArguments, referencedDeclarations);
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

    public static IFieldAccessExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IdentifierName fieldName,
        IFieldDeclarationNode referencedDeclaration)
        => new FieldAccessExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, context, fieldName, referencedDeclaration);
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

    public static IVariableNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name,
        ILocalBindingNode referencedDefinition,
        IFixedSet<IDataFlowNode> dataFlowPrevious)
        => new VariableNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, name, referencedDefinition, dataFlowPrevious);
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

    public static IStandardTypeNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        StandardName name,
        ITypeDeclarationNode referencedDeclaration,
        IMaybeAntetype namedAntetype,
        BareType? namedBareType,
        IStandardNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
        => new StandardTypeNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, name, referencedDeclaration, namedAntetype, namedBareType, syntax, typeArguments);
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

    public static IQualifiedTypeNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        StandardName name,
        ITypeDeclarationNode referencedDeclaration,
        IMaybeAntetype namedAntetype,
        BareType? namedBareType,
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<ITypeNode> typeArguments)
        => new QualifiedTypeNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, name, referencedDeclaration, namedAntetype, namedBareType, syntax, context, typeArguments);
}

// [Closed(typeof(InitializerGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerGroupNameNode : INameExpressionNode
{
    ITypeNameExpressionNode Context { get; }
    StandardName? InitializerName { get; }
    IMaybeAntetype InitializingAntetype { get; }
    IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }

    public static IInitializerGroupNameNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        ITypeNameExpressionNode context,
        StandardName? initializerName,
        IMaybeAntetype initializingAntetype,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
        => new InitializerGroupNameNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, context, initializerName, initializingAntetype, referencedDeclarations);
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

    public static ISpecialTypeNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        ISpecialTypeNameExpressionSyntax syntax,
        SpecialTypeName name,
        TypeSymbol referencedSymbol,
        UnknownType type)
        => new SpecialTypeNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, name, referencedSymbol, type);
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

    public static ISelfExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        ISelfExpressionSyntax syntax,
        bool isImplicit,
        Pseudotype pseudotype,
        ISelfParameterNode? referencedDefinition)
        => new SelfExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, isImplicit, pseudotype, referencedDefinition);
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

    public static IMissingNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IMissingNameSyntax syntax,
        UnknownType type)
        => new MissingNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, syntax, type);
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

    public static IUnknownIdentifierNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
        => new UnknownIdentifierNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, referencedDeclarations, syntax, name);
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

    public static IUnknownGenericNameExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
        => new UnknownGenericNameExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, referencedDeclarations, syntax, name, typeArguments);
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

    public static IUnknownMemberAccessExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName memberName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IDeclarationNode> referencedMembers)
        => new UnknownMemberAccessExpressionNode(controlFlowNext, controlFlowPrevious, antetype, flowStateAfter, type, syntax, context, memberName, typeArguments, referencedMembers);
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

    public static IAmbiguousMoveExpressionNode Create(
        IMoveExpressionSyntax syntax,
        IUnresolvedSimpleNameNode referent)
        => new AmbiguousMoveExpressionNode(syntax, referent);
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

    public static IMoveVariableExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        ILocalBindingNameExpressionNode referent)
        => new MoveVariableExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, isImplicit, referent);
}

// [Closed(typeof(MoveValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveValueExpressionNode : IMoveExpressionNode
{

    public static IMoveValueExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        IExpressionNode referent)
        => new MoveValueExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, isImplicit, referent);
}

// [Closed(typeof(ImplicitTempMoveExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitTempMoveExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }

    public static IImplicitTempMoveExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode referent)
        => new ImplicitTempMoveExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, referent);
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

    public static IAmbiguousFreezeExpressionNode Create(
        IFreezeExpressionSyntax syntax,
        IUnresolvedSimpleNameNode referent)
        => new AmbiguousFreezeExpressionNode(syntax, referent);
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

    public static IFreezeVariableExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        bool isTemporary,
        ILocalBindingNameExpressionNode referent)
        => new FreezeVariableExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, isImplicit, isTemporary, referent);
}

// [Closed(typeof(FreezeValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeValueExpressionNode : IFreezeExpressionNode
{

    public static IFreezeValueExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        bool isTemporary,
        IExpressionNode referent)
        => new FreezeValueExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, isImplicit, isTemporary, referent);
}

// [Closed(typeof(PrepareToReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrepareToReturnExpressionNode : IExpressionNode
{
    IExpressionNode Value { get; }
    IExpressionNode CurrentValue { get; }

    public static IPrepareToReturnExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode value,
        IExpressionNode currentValue)
        => new PrepareToReturnExpressionNode(controlFlowNext, controlFlowPrevious, syntax, antetype, type, flowStateAfter, value, currentValue);
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

    public static IAsyncBlockExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAsyncBlockExpressionSyntax syntax,
        IBlockExpressionNode block)
        => new AsyncBlockExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, block);
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

    public static IAsyncStartExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAsyncStartExpressionSyntax syntax,
        bool scheduled,
        IAmbiguousExpressionNode expression)
        => new AsyncStartExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, scheduled, expression);
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

    public static IAwaitExpressionNode Create(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAwaitExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new AwaitExpressionNode(controlFlowNext, controlFlowPrevious, antetype, type, flowStateAfter, syntax, expression);
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
    typeof(IPackageFacetChildDeclarationNode),
    typeof(IChildSymbolNode))]
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
    typeof(IExecutableDefinitionNode),
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
    typeof(IInvocableDefinitionNode),
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
    typeof(IPackageFacetNode),
    typeof(IPackageFacetSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetDeclarationNode : IChildDeclarationNode, ISymbolDeclarationNode
{
    IdentifierName PackageName
        => Package.Name;
    PackageSymbol PackageSymbol
        => Package.Symbol;
    new PackageSymbol Symbol
        => PackageSymbol;
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    INamespaceDeclarationNode GlobalNamespace { get; }
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IUserTypeDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFacetMemberDeclarationNode : INamespaceMemberDeclarationNode
{
}

[Closed(
    typeof(IDefinitionNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetChildDeclarationNode : IChildDeclarationNode
{
    IPackageFacetDeclarationNode Facet { get; }
    StandardName? Name { get; }
}

[Closed(
    typeof(INamespaceDefinitionNode),
    typeof(INamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceDeclarationNode : INamespaceMemberDeclarationNode
{
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName { get; }
    FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName { get; }
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named)
        => MembersByName.GetValueOrDefault(named) ?? [];
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembersByName.GetValueOrDefault(named) ?? [];
    new IdentifierName Name
        => Symbol.Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;
    StandardName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
}

[Closed(
    typeof(INamespaceMemberDefinitionNode),
    typeof(IFacetMemberDeclarationNode),
    typeof(INamespaceDeclarationNode),
    typeof(INamespaceMemberSymbolNode))]
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
public partial interface IFunctionDeclarationNode : IFacetMemberDeclarationNode, IFunctionLikeDeclarationNode
{
}

[Closed(
    typeof(IPrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrimitiveTypeDeclarationNode : ITypeDeclarationNode
{
    new SpecialTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
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
public partial interface IUserTypeDeclarationNode : IFacetMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ITypeDeclarationNode
{
    IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
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
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
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
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
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
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
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
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
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
    typeof(IInstanceMemberDeclarationNode),
    typeof(ITypeMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberDeclarationNode : IPackageFacetChildDeclarationNode, ISymbolDeclarationNode
{
}

[Closed(
    typeof(IClassMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IClassMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(ITraitMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(ITraitMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IStructMemberDefinitionNode),
    typeof(IUserTypeDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IStructMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IGenericParameterDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode),
    typeof(IAssociatedMemberSymbolNode))]
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
    IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named);
    IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named);
    IFixedSet<BareReferenceType> Supertypes { get; }
    IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IPackageSymbolNode),
    typeof(IPackageFacetSymbolNode),
    typeof(INamespaceMemberSymbolNode),
    typeof(IGenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IChildSymbolNode : IChildDeclarationNode
{
    new ISyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(PackageSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageSymbolNode : IPackageDeclarationNode, IChildSymbolNode
{
    new IPackageFacetSymbolNode MainFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.MainFacet => MainFacet;
    new IPackageFacetSymbolNode TestingFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.TestingFacet => TestingFacet;

    public static IPackageSymbolNode Create(
        IdentifierName? aliasOrName,
        IdentifierName name,
        PackageSymbol symbol,
        IPackageFacetSymbolNode mainFacet,
        IPackageFacetSymbolNode testingFacet)
        => new PackageSymbolNode(aliasOrName, name, symbol, mainFacet, testingFacet);
}

// [Closed(typeof(PackageFacetSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetSymbolNode : IPackageFacetDeclarationNode, IChildSymbolNode
{
    FixedSymbolTree SymbolTree { get; }
    new INamespaceSymbolNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;

    public static IPackageFacetSymbolNode Create(
        FixedSymbolTree symbolTree)
        => new PackageFacetSymbolNode(symbolTree);
}

// [Closed(typeof(NamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceSymbolNode : INamespaceDeclarationNode, INamespaceMemberSymbolNode
{
    new IFixedList<INamespaceMemberSymbolNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;

    public static INamespaceSymbolNode Create(
        NamespaceSymbol symbol,
        IEnumerable<INamespaceMemberSymbolNode> members)
        => new NamespaceSymbolNode(symbol, members);
}

[Closed(
    typeof(INamespaceSymbolNode),
    typeof(IFunctionSymbolNode),
    typeof(IUserTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberSymbolNode : INamespaceMemberDeclarationNode, IChildSymbolNode
{
}

// [Closed(typeof(FunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionSymbolNode : IFunctionDeclarationNode, INamespaceMemberSymbolNode
{

    public static IFunctionSymbolNode Create(
        FunctionSymbol symbol,
        StandardName name,
        FunctionType type)
        => new FunctionSymbolNode(symbol, name, type);
}

// [Closed(typeof(PrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrimitiveTypeSymbolNode : IPrimitiveTypeDeclarationNode
{
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IPrimitiveTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<BareReferenceType> ITypeDeclarationNode.Supertypes
        => Symbol.GetDeclaredType()?.Supertypes ?? [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => Members;

    public static IPrimitiveTypeSymbolNode Create(
        ISyntax? syntax,
        TypeSymbol symbol,
        SpecialTypeName name,
        IEnumerable<ITypeMemberSymbolNode> members)
        => new PrimitiveTypeSymbolNode(syntax, symbol, name, members);
}

[Closed(
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUserTypeSymbolNode : IUserTypeDeclarationNode, INamespaceMemberSymbolNode
{
    new IFixedList<IGenericParameterSymbolNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    ISymbolTree SymbolTree();
    IFixedSet<BareReferenceType> ITypeDeclarationNode.Supertypes
        => Symbol.GetDeclaredType().Supertypes;
}

// [Closed(typeof(ClassSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassSymbolNode : IClassDeclarationNode, IUserTypeSymbolNode
{
    new IFixedSet<IClassMemberSymbolNode> Members { get; }
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.InclusiveMembers
        => Members;

    public static IClassSymbolNode Create(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<IClassMemberSymbolNode> members)
        => new ClassSymbolNode(name, symbol, genericParameters, members);
}

// [Closed(typeof(StructSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructSymbolNode : IStructDeclarationNode, IUserTypeSymbolNode
{
    new IFixedSet<IStructMemberSymbolNode> Members { get; }
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.InclusiveMembers
        => Members;

    public static IStructSymbolNode Create(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<IStructMemberSymbolNode> members)
        => new StructSymbolNode(name, symbol, genericParameters, members);
}

// [Closed(typeof(TraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitSymbolNode : ITraitDeclarationNode, IUserTypeSymbolNode
{
    new IFixedSet<ITraitMemberSymbolNode> Members { get; }
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.InclusiveMembers
        => Members;

    public static ITraitSymbolNode Create(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<ITraitMemberSymbolNode> members)
        => new TraitSymbolNode(name, symbol, genericParameters, members);
}

// [Closed(typeof(GenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterSymbolNode : IGenericParameterDeclarationNode, IChildSymbolNode
{
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IGenericParameterDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<BareReferenceType> ITypeDeclarationNode.Supertypes
        => Symbol.GetDeclaredType()?.Supertypes ?? [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];

    public static IGenericParameterSymbolNode Create(
        IdentifierName name,
        GenericParameterTypeSymbol symbol,
        IEnumerable<ITypeMemberSymbolNode> members)
        => new GenericParameterSymbolNode(name, symbol, members);
}

[Closed(
    typeof(IClassMemberSymbolNode),
    typeof(ITraitMemberSymbolNode),
    typeof(IStructMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberSymbolNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IAssociatedMemberSymbolNode),
    typeof(IMethodSymbolNode),
    typeof(IConstructorSymbolNode),
    typeof(IFieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassMemberSymbolNode : IClassMemberDeclarationNode, ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IAssociatedMemberSymbolNode),
    typeof(IMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitMemberSymbolNode : ITraitMemberDeclarationNode, ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IAssociatedMemberSymbolNode),
    typeof(IMethodSymbolNode),
    typeof(IInitializerSymbolNode),
    typeof(IFieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberSymbolNode : IStructMemberDeclarationNode, ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IAssociatedFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedMemberSymbolNode : IAssociatedMemberDeclarationNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode
{
}

[Closed(
    typeof(IStandardMethodSymbolNode),
    typeof(IGetterMethodSymbolNode),
    typeof(ISetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSymbolNode : IMethodDeclarationNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode
{
}

// [Closed(typeof(StandardMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardMethodSymbolNode : IStandardMethodDeclarationNode, IMethodSymbolNode
{
    int IStandardMethodDeclarationNode.Arity
        => Symbol.Arity;
    FunctionType IStandardMethodDeclarationNode.MethodGroupType
        => Symbol.MethodGroupType;

    public static IStandardMethodSymbolNode Create(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
        => new StandardMethodSymbolNode(syntax, name, symbol);
}

// [Closed(typeof(GetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodSymbolNode : IGetterMethodDeclarationNode, IMethodSymbolNode
{

    public static IGetterMethodSymbolNode Create(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
        => new GetterMethodSymbolNode(syntax, name, symbol);
}

// [Closed(typeof(SetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodSymbolNode : ISetterMethodDeclarationNode, IMethodSymbolNode
{

    public static ISetterMethodSymbolNode Create(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
        => new SetterMethodSymbolNode(syntax, name, symbol);
}

// [Closed(typeof(ConstructorSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSymbolNode : IConstructorDeclarationNode, IClassMemberSymbolNode
{

    public static IConstructorSymbolNode Create(
        ISyntax? syntax,
        IdentifierName? name,
        ConstructorSymbol symbol)
        => new ConstructorSymbolNode(syntax, name, symbol);
}

// [Closed(typeof(InitializerSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSymbolNode : IInitializerDeclarationNode, IStructMemberSymbolNode
{

    public static IInitializerSymbolNode Create(
        ISyntax? syntax,
        IdentifierName? name,
        InitializerSymbol symbol)
        => new InitializerSymbolNode(syntax, name, symbol);
}

// [Closed(typeof(FieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldSymbolNode : IFieldDeclarationNode, IClassMemberSymbolNode, IStructMemberSymbolNode
{

    public static IFieldSymbolNode Create(
        ISyntax? syntax,
        IdentifierName name,
        DataType bindingType,
        FieldSymbol symbol)
        => new FieldSymbolNode(syntax, name, bindingType, symbol);
}

// [Closed(typeof(AssociatedFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode, IAssociatedMemberSymbolNode
{

    public static IAssociatedFunctionSymbolNode Create(
        ISyntax? syntax,
        FunctionSymbol symbol,
        FunctionType type,
        StandardName name)
        => new AssociatedFunctionSymbolNode(syntax, symbol, type, name);
}

// TODO switch back to `file` and not `partial` once fully transitioned
internal abstract partial class SemanticNode : TreeNode, IChildTreeNode<ISemanticNode>
{
    private SemanticNode? parent;

    protected SemanticNode() { }
    protected SemanticNode(bool inFinalTree) : base(inFinalTree) { }

    [DebuggerStepThrough]
    protected sealed override SemanticNode? PeekParent()
        // Use volatile read to ensure order of operations as seen by other threads. If parent is
        // null, report an error if not in final tree. Root nodes are always in the final tree.
        => Volatile.Read(in parent) ?? (InFinalTree ? null : throw Child.ParentMissing(this));

    private SemanticNode? GetParent(IInheritanceContext ctx)
    {
        // Use volatile read to ensure order of operations as seen by other threads
        var node = Volatile.Read(in parent) ?? (InFinalTree ? null : throw Child.ParentMissing(this));
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
    protected virtual SemanticNode? Previous(IInheritanceContext ctx)
    {
        SemanticNode? previous = null;
        var parent = GetParent(ctx);
        if (parent is null)
            return null;
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
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingLexicalScope", child, descendant)).Inherited_ContainingLexicalScope(this, descendant, ctx);
    protected LexicalScope Inherited_ContainingLexicalScope(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingLexicalScope(this, this, ctx);

    internal virtual IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => (Previous(ctx) ?? throw Child.PreviousFailed("PreviousValueId", before)).Next_PreviousValueId(this, ctx);
    protected IPreviousValueId Previous_PreviousValueId(IInheritanceContext ctx)
        => Previous(ctx)!.Next_PreviousValueId(this, ctx);

    internal virtual ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingDeclaration", child, descendant)).Inherited_ContainingDeclaration(this, descendant, ctx);
    protected ISymbolDeclarationNode Inherited_ContainingDeclaration(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingDeclaration(this, this, ctx);

    protected IFixedSet<SemanticNode> CollectContributors_Diagnostics()
    {
        var contributors = new List<SemanticNode>();
        foreach (var child in Children().Cast<SemanticNode>())
            child.CollectContributors_Diagnostics(contributors);
        return contributors.ToFixedSet();
    }
    internal virtual void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        foreach (var child in Children().Cast<SemanticNode>())
            child.CollectContributors_Diagnostics(contributors);
    }
    protected DiagnosticCollection Collect_Diagnostics(IFixedSet<SemanticNode> contributors)
    {
        var builder = new DiagnosticCollectionBuilder();
        Contribute_This_Diagnostics(builder);
        foreach (var contributor in contributors)
            contributor.Contribute_Diagnostics(builder);
        return builder.Build();
    }
    internal virtual void Contribute_This_Diagnostics(DiagnosticCollectionBuilder builder) { }
    internal virtual void Contribute_Diagnostics(DiagnosticCollectionBuilder builder) { }

    internal virtual IPackageDeclarationNode Inherited_Package(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("Package", child, descendant)).Inherited_Package(this, descendant, ctx);
    protected IPackageDeclarationNode Inherited_Package(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_Package(this, this, ctx);

    internal virtual PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("PackageNameScope", child, descendant)).Inherited_PackageNameScope(this, descendant, ctx);
    protected PackageNameScope Inherited_PackageNameScope(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_PackageNameScope(this, this, ctx);

    internal virtual CodeFile Inherited_File(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("File", child, descendant)).Inherited_File(this, descendant, ctx);
    protected CodeFile Inherited_File(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_File(this, this, ctx);

    internal virtual IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("Facet", child, descendant)).Inherited_Facet(this, descendant, ctx);
    protected IPackageFacetDeclarationNode Inherited_Facet(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_Facet(this, this, ctx);

    internal virtual IDeclaredUserType Inherited_ContainingDeclaredType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingDeclaredType", child, descendant)).Inherited_ContainingDeclaredType(this, descendant, ctx);
    protected IDeclaredUserType Inherited_ContainingDeclaredType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingDeclaredType(this, this, ctx);

    internal virtual IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("FlowStateBefore", child, descendant)).Inherited_FlowStateBefore(this, descendant, ctx);
    protected IFlowState Inherited_FlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_FlowStateBefore(this, this, ctx);

    internal virtual ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingTypeDefinition", child, descendant)).Inherited_ContainingTypeDefinition(this, descendant, ctx);
    protected ITypeDefinitionNode Inherited_ContainingTypeDefinition(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingTypeDefinition(this, this, ctx);

    internal virtual DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedType", child, descendant)).Inherited_ExpectedType(this, descendant, ctx);
    protected DataType? Inherited_ExpectedType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedType(this, this, ctx);

    internal virtual IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedAntetype", child, descendant)).Inherited_ExpectedAntetype(this, descendant, ctx);
    protected IMaybeExpressionAntetype? Inherited_ExpectedAntetype(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedAntetype(this, this, ctx);

    internal virtual bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("IsAttributeType", child, descendant)).Inherited_IsAttributeType(this, descendant, ctx);
    protected bool Inherited_IsAttributeType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_IsAttributeType(this, this, ctx);

    internal virtual Pseudotype? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("MethodSelfType", child, descendant)).Inherited_MethodSelfType(this, descendant, ctx);
    protected Pseudotype? Inherited_MethodSelfType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_MethodSelfType(this, this, ctx);

    internal virtual IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ControlFlowEntry", child, descendant)).Inherited_ControlFlowEntry(this, descendant, ctx);
    protected IEntryNode Inherited_ControlFlowEntry(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ControlFlowEntry(this, this, ctx);

    internal virtual ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ControlFlowFollowing", child, descendant)).Inherited_ControlFlowFollowing(this, descendant, ctx);
    protected ControlFlowSet Inherited_ControlFlowFollowing(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ControlFlowFollowing(this, this, ctx);

    internal virtual FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("VariableBindingsMap", child, descendant)).Inherited_VariableBindingsMap(this, descendant, ctx);
    protected FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_VariableBindingsMap(this, this, ctx);

    internal virtual ValueId? Inherited_MatchReferentValueId(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("MatchReferentValueId", child, descendant)).Inherited_MatchReferentValueId(this, descendant, ctx);
    protected ValueId? Inherited_MatchReferentValueId(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_MatchReferentValueId(this, this, ctx);

    internal virtual IMaybeAntetype Inherited_ContextBindingAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContextBindingAntetype", child, descendant)).Inherited_ContextBindingAntetype(this, descendant, ctx);
    protected IMaybeAntetype Inherited_ContextBindingAntetype(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContextBindingAntetype(this, this, ctx);

    internal virtual DataType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContextBindingType", child, descendant)).Inherited_ContextBindingType(this, descendant, ctx);
    protected DataType Inherited_ContextBindingType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContextBindingType(this, this, ctx);

    internal virtual bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ImplicitRecoveryAllowed", child, descendant)).Inherited_ImplicitRecoveryAllowed(this, descendant, ctx);
    protected bool Inherited_ImplicitRecoveryAllowed(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ImplicitRecoveryAllowed(this, this, ctx);

    internal virtual bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ShouldPrepareToReturn", child, descendant)).Inherited_ShouldPrepareToReturn(this, descendant, ctx);
    protected bool Inherited_ShouldPrepareToReturn(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ShouldPrepareToReturn(this, this, ctx);

    internal virtual IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ControlFlowExit", child, descendant)).Inherited_ControlFlowExit(this, descendant, ctx);
    protected IExitNode Inherited_ControlFlowExit(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ControlFlowExit(this, this, ctx);

    internal virtual DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedReturnType", child, descendant)).Inherited_ExpectedReturnType(this, descendant, ctx);
    protected DataType? Inherited_ExpectedReturnType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedReturnType(this, this, ctx);

    internal virtual ISymbolTree Inherited_SymbolTree(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("SymbolTree", child, descendant)).Inherited_SymbolTree(this, descendant, ctx);
    protected ISymbolTree Inherited_SymbolTree(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_SymbolTree(this, this, ctx);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageNode : SemanticNode, IPackageNode
{
    private IPackageNode Self { [Inline] get => this; }

    public IPackageSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedSet<IPackageReferenceNode> References { [DebuggerStepThrough] get; }
    public IPackageFacetNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetNode TestingFacet { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics
        => GrammarAttribute.IsCached(in diagnosticsCached) ? diagnostics!
            : this.Aggregate(ref diagnosticsContributors, ref diagnosticsCached, ref diagnostics,
                CollectContributors_Diagnostics, Collect_Diagnostics);
    private DiagnosticCollection? diagnostics;
    private bool diagnosticsCached;
    private IFixedSet<SemanticNode>? diagnosticsContributors;
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
    public PackageSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.Package_Symbol);
    private PackageSymbol? symbol;
    private bool symbolCached;
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

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IPackageFacetNode mainFacet,
        IPackageFacetNode testingFacet)
        : base(true)
    {
        Syntax = syntax;
        References = ChildSet.Attach(this, references);
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
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

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
        => builder.Add(Diagnostics);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
        => contributors.Add(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardPackageReferenceNode : SemanticNode, IStandardPackageReferenceNode
{
    private IStandardPackageReferenceNode Self { [Inline] get => this; }

    public IPackageReferenceSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageSymbolNode SymbolNode { [DebuggerStepThrough] get; }

    public StandardPackageReferenceNode(
        IPackageReferenceSyntax syntax)
    {
        Syntax = syntax;
        SymbolNode = Child.Attach(this, SymbolNodeAspect.PackageReference_SymbolNode(this));
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IntrinsicsPackageReferenceNode : SemanticNode, IIntrinsicsPackageReferenceNode
{
    private IIntrinsicsPackageReferenceNode Self { [Inline] get => this; }

    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageSymbolNode SymbolNode { [DebuggerStepThrough] get; }

    public IntrinsicsPackageReferenceNode(
        )
    {
        SymbolNode = Child.Attach(this, SymbolNodeAspect.PackageReference_SymbolNode(this));
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageFacetNode : SemanticNode, IPackageFacetNode
{
    private IPackageFacetNode Self { [Inline] get => this; }

    public IPackageSyntax Syntax { [DebuggerStepThrough] get; }
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
    public IFixedSet<IFacetMemberDefinitionNode> Definitions
        => GrammarAttribute.IsCached(in definitionsCached) ? definitions!
            : this.Synthetic(ref definitionsCached, ref definitions,
                DefinitionsAspect.PackageFacet_Definitions);
    private IFixedSet<IFacetMemberDefinitionNode>? definitions;
    private bool definitionsCached;

    public PackageFacetNode(
        IPackageSyntax syntax,
        IEnumerable<ICompilationUnitNode> compilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = ChildSet.Attach(this, compilationUnits);
    }

    internal override PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return PackageNameScope;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return Is.OfType<NamespaceScope>(PackageNameScope.PackageGlobalScope);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IPackageFacetNode>(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return Is.OfType<IPackageFacetNode>(this);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CompilationUnitNode : SemanticNode, ICompilationUnitNode
{
    private ICompilationUnitNode Self { [Inline] get => this; }

    public ICompilationUnitSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public NamespaceScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                (ctx) => (NamespaceScope)Inherited_ContainingLexicalScope(ctx));
    private NamespaceScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IPackageFacetNode ContainingDeclaration
        => (IPackageFacetNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public DiagnosticCollection Diagnostics
        => GrammarAttribute.IsCached(in diagnosticsCached) ? diagnostics!
            : this.Aggregate(ref diagnosticsContributors, ref diagnosticsCached, ref diagnostics,
                CollectContributors_Diagnostics, Collect_Diagnostics);
    private DiagnosticCollection? diagnostics;
    private bool diagnosticsCached;
    private IFixedSet<SemanticNode>? diagnosticsContributors;
    public NamespaceSearchScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.CompilationUnit_LexicalScope);
    private NamespaceSearchScope? lexicalScope;
    private bool lexicalScopeCached;
    public INamespaceDefinitionNode ImplicitNamespace
        => GrammarAttribute.IsCached(in implicitNamespaceCached) ? implicitNamespace!
            : this.Synthetic(ref implicitNamespaceCached, ref implicitNamespace,
                SymbolNodeAspect.CompilationUnit_ImplicitNamespace);
    private INamespaceDefinitionNode? implicitNamespace;
    private bool implicitNamespaceCached;

    public CompilationUnitNode(
        ICompilationUnitSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> definitions)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.Attach(this, usingDirectives);
        Definitions = ChildList.Attach(this, definitions);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<NamespaceSearchScope>(LexicalScope);
    }

    internal override CodeFile Inherited_File(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ContextAspect.CompilationUnit_Children_Broadcast_File(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return Is.OfType<INamespaceDefinitionNode>(ImplicitNamespace);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
        => builder.Add(Diagnostics);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
        => contributors.Add(this);

    internal override void Contribute_This_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        DefinitionsAspect.CompilationUnit_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UsingDirectiveNode : SemanticNode, IUsingDirectiveNode
{
    private IUsingDirectiveNode Self { [Inline] get => this; }

    public IUsingDirectiveSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public UsingDirectiveNode(
        IUsingDirectiveSyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceBlockDefinitionNode : SemanticNode, INamespaceBlockDefinitionNode
{
    private INamespaceBlockDefinitionNode Self { [Inline] get => this; }

    public INamespaceDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveNode> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
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
    public NamespaceSearchScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.NamespaceBlockDefinition_LexicalScope);
    private NamespaceSearchScope? lexicalScope;
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

    public NamespaceBlockDefinitionNode(
        INamespaceDefinitionSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> members)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.Attach(this, usingDirectives);
        Members = ChildList.Attach(this, members);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<NamespaceSearchScope>(LexicalScope);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return ContextAspect.NamespaceBlockDefinition_Children_ContainingDeclaration(this);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceDefinitionNode : SemanticNode, INamespaceDefinitionNode
{
    private INamespaceDefinitionNode Self { [Inline] get => this; }

    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { [DebuggerStepThrough] get; }
    public IFixedList<IFacetMemberDefinitionNode> PackageMembers { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public IFixedList<INamespaceMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
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
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => GrammarAttribute.IsCached(in nestedMembersCached) ? nestedMembers!
            : this.Synthetic(ref nestedMembersCached, ref nestedMembers,
                DeclarationsAspect.NamespaceDeclaration_NestedMembers);
    private IFixedList<INamespaceMemberDeclarationNode>? nestedMembers;
    private bool nestedMembersCached;

    public NamespaceDefinitionNode(
        NamespaceSymbol symbol,
        IEnumerable<INamespaceDefinitionNode> memberNamespaces,
        IEnumerable<IFacetMemberDefinitionNode> packageMembers)
    {
        Symbol = symbol;
        MemberNamespaces = ChildList.Attach(this, memberNamespaces);
        PackageMembers = packageMembers.ToFixedList();
        Members = DefinitionsAspect.NamespaceDefinition_Members(this);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionDefinitionNode : SemanticNode, IFunctionDefinitionNode
{
    private IFunctionDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IFunctionDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.FunctionDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.FunctionDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public FunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                TypeMemberDeclarationsAspect.FunctionDefinition_Type);
    private FunctionType? type;
    private bool typeCached;
    public FunctionSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.FunctionDefinition_Symbol);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }

    public FunctionDefinitionNode(
        IFunctionDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
    {
        Syntax = syntax;
        Attributes = ChildList.Attach(this, attributes);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.Parameters.Count && ReferenceEquals(child, Self.Parameters[0]))
            return Self.FlowStateBefore();
        if (IndexOfNode(Self.Parameters, child) is { } index)
            return Parameters[index - 1].FlowStateAfter;
        if (ReferenceEquals(child, Self.Body))
            return Parameters.LastOrDefault()?.FlowStateAfter ?? Self.FlowStateBefore();
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Type.Return.Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Type.Return.Type;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Type.Return.Type.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassDefinitionNode : SemanticNode, IClassDefinitionNode
{
    private IClassDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IClassDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IStandardTypeNameNode? BaseTypeName { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IClassMemberDefinitionNode> SourceMembers { [DebuggerStepThrough] get; }
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
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.ClassDefinition_InclusiveMembers);
    private IFixedSet<IClassMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public ObjectType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType,
                TypeDefinitionsAspect.ClassDefinition_DeclaredType);
    private ObjectType? declaredType;
    private bool declaredTypeCached;
    public IFixedSet<IClassMemberDefinitionNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                DefaultMembersAspect.ClassDefinition_Members);
    private IFixedSet<IClassMemberDefinitionNode>? members;
    private bool membersCached;
    public IDefaultConstructorDefinitionNode? DefaultConstructor
        => GrammarAttribute.IsCached(in defaultConstructorCached) ? defaultConstructor
            : this.Synthetic(ref defaultConstructorCached, ref defaultConstructor,
                n => Child.Attach(this, DefaultMembersAspect.ClassDefinition_DefaultConstructor(n)));
    private IDefaultConstructorDefinitionNode? defaultConstructor;
    private bool defaultConstructorCached;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IFixedSet<BareReferenceType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareReferenceType>> supertypes = new([]);
    private bool supertypesCached;
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

    public ClassDefinitionNode(
        IClassDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IStandardTypeNameNode? baseTypeName,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDefinitionNode> sourceMembers)
    {
        Syntax = syntax;
        Attributes = ChildList.Attach(this, attributes);
        GenericParameters = ChildList.Attach(this, genericParameters);
        BaseTypeName = Child.Attach(this, baseTypeName);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
        SourceMembers = ChildList.Attach(this, sourceMembers);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.GenericParameters, child))
            return ContainingLexicalScope;
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IUserTypeDeclarationNode>(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
    }

    internal override IDeclaredUserType Inherited_ContainingDeclaredType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingDeclaredType(this);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeDefinitionsAspect.TypeDefinition_Contribute_Diagnostics(this, builder);
        TypeDefinitionsAspect.ClassDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructDefinitionNode : SemanticNode, IStructDefinitionNode
{
    private IStructDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IStructDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IStructMemberDefinitionNode> SourceMembers { [DebuggerStepThrough] get; }
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
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.StructDefinition_InclusiveMembers);
    private IFixedSet<IStructMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public StructType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType,
                TypeDefinitionsAspect.StructDefinition_DeclaredType);
    private StructType? declaredType;
    private bool declaredTypeCached;
    public IFixedSet<IStructMemberDefinitionNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                DefaultMembersAspect.StructDefinition_Members);
    private IFixedSet<IStructMemberDefinitionNode>? members;
    private bool membersCached;
    public IDefaultInitializerDefinitionNode? DefaultInitializer
        => GrammarAttribute.IsCached(in defaultInitializerCached) ? defaultInitializer
            : this.Synthetic(ref defaultInitializerCached, ref defaultInitializer,
                n => Child.Attach(this, DefaultMembersAspect.StructDefinition_DefaultInitializer(n)));
    private IDefaultInitializerDefinitionNode? defaultInitializer;
    private bool defaultInitializerCached;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IFixedSet<BareReferenceType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareReferenceType>> supertypes = new([]);
    private bool supertypesCached;
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

    public StructDefinitionNode(
        IStructDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDefinitionNode> sourceMembers)
    {
        Syntax = syntax;
        Attributes = ChildList.Attach(this, attributes);
        GenericParameters = ChildList.Attach(this, genericParameters);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
        SourceMembers = ChildList.Attach(this, sourceMembers);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.GenericParameters, child))
            return ContainingLexicalScope;
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IUserTypeDeclarationNode>(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
    }

    internal override IDeclaredUserType Inherited_ContainingDeclaredType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingDeclaredType(this);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeDefinitionsAspect.TypeDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitDefinitionNode : SemanticNode, ITraitDefinitionNode
{
    private ITraitDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ITraitDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeNode> Attributes { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameNode> SupertypeNames { [DebuggerStepThrough] get; }
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
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.TraitDefinition_InclusiveMembers);
    private IFixedSet<ITraitMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public ObjectType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType,
                TypeDefinitionsAspect.TraitDefinition_DeclaredType);
    private ObjectType? declaredType;
    private bool declaredTypeCached;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IFixedSet<BareReferenceType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareReferenceType>> supertypes = new([]);
    private bool supertypesCached;
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

    public TraitDefinitionNode(
        ITraitDefinitionSyntax syntax,
        IEnumerable<IAttributeNode> attributes,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<ITraitMemberDefinitionNode> members)
    {
        Syntax = syntax;
        Attributes = ChildList.Attach(this, attributes);
        GenericParameters = ChildList.Attach(this, genericParameters);
        SupertypeNames = ChildList.Attach(this, supertypeNames);
        Members = ChildSet.Attach(this, members);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ContainsNode(Self.GenericParameters, child))
            return ContainingLexicalScope;
        if (ContainsNode(Self.AllSupertypeNames, child))
            return LexicalScopingAspect.TypeDefinition_AllSupertypeNames_Broadcast_ContainingLexicalScope(this);
        if (ContainsNode(Self.Members, child))
            return LexicalScopingAspect.TypeDefinition_Members_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IUserTypeDeclarationNode>(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
    }

    internal override IDeclaredUserType Inherited_ContainingDeclaredType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingDeclaredType(this);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeDefinitionsAspect.TypeDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterNode : SemanticNode, IGenericParameterNode
{
    private IGenericParameterNode Self { [Inline] get => this; }

    public IGenericParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityConstraintNode Constraint { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public GenericParameterTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.GenericParameter_Symbol);
    private GenericParameterTypeSymbol? symbol;
    private bool symbolCached;
    public GenericParameter Parameter
        => GrammarAttribute.IsCached(in parameterCached) ? parameter!
            : this.Synthetic(ref parameterCached, ref parameter,
                TypeDefinitionsAspect.GenericParameter_Parameter);
    private GenericParameter? parameter;
    private bool parameterCached;
    public GenericParameterType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType,
                TypeDefinitionsAspect.GenericParameter_DeclaredType);
    private GenericParameterType? declaredType;
    private bool declaredTypeCached;

    public GenericParameterNode(
        IGenericParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = Child.Attach(this, constraint);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AbstractMethodDefinitionNode : SemanticNode, IAbstractMethodDefinitionNode
{
    private IAbstractMethodDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IAbstractMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;

    public AbstractMethodDefinitionNode(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeMemberDeclarationsAspect.MethodDefinition_Contribute_Diagnostics(this, builder);
        TypeModifiersAspect.AbstractMethodDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardMethodDefinitionNode : SemanticNode, IStandardMethodDefinitionNode
{
    private IStandardMethodDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IStandardMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }

    public StandardMethodDefinitionNode(
        IStandardMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.SelfParameter))
            return Self.FlowStateBefore();
        if (0 < Self.Parameters.Count && ReferenceEquals(child, Self.Parameters[0]))
            return SelfParameter.FlowStateAfter;
        if (IndexOfNode(Self.Parameters, child) is { } index)
            return Parameters[index - 1].FlowStateAfter;
        if (ReferenceEquals(child, Self.Body))
            return Parameters.LastOrDefault()?.FlowStateAfter ?? SelfParameter.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Return?.NamedType ?? DataType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IExecutableDefinitionNode>(this);
    }

    internal override Pseudotype? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.MethodGroupType.Return.Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.MethodGroupType.Return.Type.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeMemberDeclarationsAspect.MethodDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodDefinitionNode : SemanticNode, IGetterMethodDefinitionNode
{
    private IGetterMethodDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IGetterMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode Return { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }

    public GetterMethodDefinitionNode(
        IGetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode @return,
        IBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.SelfParameter))
            return Self.FlowStateBefore();
        if (0 < Self.Parameters.Count && ReferenceEquals(child, Self.Parameters[0]))
            return SelfParameter.FlowStateAfter;
        if (IndexOfNode(Self.Parameters, child) is { } index)
            return Parameters[index - 1].FlowStateAfter;
        if (ReferenceEquals(child, Self.Body))
            return Parameters.LastOrDefault()?.FlowStateAfter ?? SelfParameter.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Return?.NamedType ?? DataType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IExecutableDefinitionNode>(this);
    }

    internal override Pseudotype? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeMemberDeclarationsAspect.MethodDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodDefinitionNode : SemanticNode, ISetterMethodDefinitionNode
{
    private ISetterMethodDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ISetterMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMethodSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }

    public SetterMethodDefinitionNode(
        ISetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.SelfParameter))
            return Self.FlowStateBefore();
        if (0 < Self.Parameters.Count && ReferenceEquals(child, Self.Parameters[0]))
            return SelfParameter.FlowStateAfter;
        if (IndexOfNode(Self.Parameters, child) is { } index)
            return Parameters[index - 1].FlowStateAfter;
        if (ReferenceEquals(child, Self.Body))
            return Parameters.LastOrDefault()?.FlowStateAfter ?? SelfParameter.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Return?.NamedType ?? DataType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<IExecutableDefinitionNode>(this);
    }

    internal override Pseudotype? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeMemberDeclarationsAspect.MethodDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class DefaultConstructorDefinitionNode : SemanticNode, IDefaultConstructorDefinitionNode
{
    private IDefaultConstructorDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IConstructorDefinitionSyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBodyNode? Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;

    public DefaultConstructorDefinitionNode(
        IConstructorDefinitionSyntax? syntax,
        IdentifierName? name,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBodyNode? body)
    {
        Syntax = syntax;
        Name = name;
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SourceConstructorDefinitionNode : SemanticNode, ISourceConstructorDefinitionNode
{
    private ISourceConstructorDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IConstructorDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;

    public SourceConstructorDefinitionNode(
        IdentifierName? name,
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
    {
        Name = name;
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class DefaultInitializerDefinitionNode : SemanticNode, IDefaultInitializerDefinitionNode
{
    private IDefaultInitializerDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IInitializerDefinitionSyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBodyNode? Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;

    public DefaultInitializerDefinitionNode(
        IInitializerDefinitionSyntax? syntax,
        IdentifierName? name,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBodyNode? body)
    {
        Syntax = syntax;
        Name = name;
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SourceInitializerDefinitionNode : SemanticNode, ISourceInitializerDefinitionNode
{
    private ISourceInitializerDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IInitializerDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;

    public SourceInitializerDefinitionNode(
        IdentifierName? name,
        IInitializerDefinitionSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
    {
        Name = name;
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldDefinitionNode : SemanticNode, IFieldDefinitionNode
{
    private IFieldDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IFieldDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeNode TypeNode { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public IAmbiguousExpressionNode? TempInitializer { [DebuggerStepThrough] get; }
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentInitializer { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.FieldDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }

    public FieldDefinitionNode(
        bool isLentBinding,
        IFieldDefinitionSyntax syntax,
        bool isMutableBinding,
        IdentifierName name,
        ITypeNode typeNode,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IAmbiguousExpressionNode? initializer,
        IAmbiguousExpressionNode? currentInitializer)
    {
        IsLentBinding = isLentBinding;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Name = name;
        TypeNode = Child.Attach(this, typeNode);
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        TempInitializer = Child.Attach(this, initializer);
        CurrentInitializer = currentInitializer;
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssociatedFunctionDefinitionNode : SemanticNode, IAssociatedFunctionDefinitionNode
{
    private IAssociatedFunctionDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IAssociatedFunctionDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
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
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.ConcreteInvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;
    public ValueIdScope ValueIdScope
        => GrammarAttribute.IsCached(in valueIdScopeCached) ? valueIdScope!
            : this.Synthetic(ref valueIdScopeCached, ref valueIdScope,
                ValueIdsAspect.InvocableDefinition_ValueIdScope);
    private ValueIdScope? valueIdScope;
    private bool valueIdScopeCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;

    public AssociatedFunctionDefinitionNode(
        IAssociatedFunctionDefinitionSyntax syntax,
        IdentifierName name,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        FunctionType type,
        IBodyNode body)
    {
        Syntax = syntax;
        Name = name;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Type = type;
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.ConcreteInvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueIdsAspect.InvocableDefinition_Next_PreviousValueId(this);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AttributeNode : SemanticNode, IAttributeNode
{
    private IAttributeNode Self { [Inline] get => this; }

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

    public AttributeNode(
        IAttributeSyntax syntax,
        IStandardTypeNameNode typeName)
    {
        Syntax = syntax;
        TypeName = Child.Attach(this, typeName);
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.TypeName))
            return true;
        return base.Inherited_IsAttributeType(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilitySetNode : SemanticNode, ICapabilitySetNode
{
    private ICapabilitySetNode Self { [Inline] get => this; }

    public ICapabilitySetSyntax Syntax { [DebuggerStepThrough] get; }
    public CapabilitySet Constraint { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilitySetNode(
        ICapabilitySetSyntax syntax,
        CapabilitySet constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityNode : SemanticNode, ICapabilityNode
{
    private ICapabilityNode Self { [Inline] get => this; }

    public ICapabilityConstraint Constraint { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Syntax { [DebuggerStepThrough] get; }
    public Capability Capability { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityNode(
        ICapabilityConstraint constraint,
        ICapabilitySyntax syntax,
        Capability capability)
    {
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

    public NamedParameterNode(
        bool unused,
        IFlowState flowStateAfter,
        ParameterType parameterType,
        INamedParameterSyntax syntax,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        ITypeNode typeNode,
        IMaybeAntetype bindingAntetype,
        DataType bindingType)
    {
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ParameterType = parameterType;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        IsLentBinding = isLentBinding;
        Name = name;
        TypeNode = Child.Attach(this, typeNode);
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSelfParameterNode : SemanticNode, IConstructorSelfParameterNode
{
    private IConstructorSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public SelfParameterType ParameterType { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public CapabilityType BindingType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public ConstructorSelfParameterNode(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        IConstructorSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityNode capability,
        CapabilityType bindingType)
    {
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = Child.Attach(this, capability);
        BindingType = bindingType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSelfParameterNode : SemanticNode, IInitializerSelfParameterNode
{
    private IInitializerSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public SelfParameterType ParameterType { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public CapabilityType BindingType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public InitializerSelfParameterNode(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        IInitializerSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityNode capability,
        CapabilityType bindingType)
    {
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = Child.Attach(this, capability);
        BindingType = bindingType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodSelfParameterNode : SemanticNode, IMethodSelfParameterNode
{
    private IMethodSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool Unused { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
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
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public IDeclaredUserType ContainingDeclaredType
        => GrammarAttribute.IsCached(in containingDeclaredTypeCached) ? containingDeclaredType!
            : this.Inherited(ref containingDeclaredTypeCached, ref containingDeclaredType,
                Inherited_ContainingDeclaredType);
    private IDeclaredUserType? containingDeclaredType;
    private bool containingDeclaredTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;

    public MethodSelfParameterNode(
        IdentifierName? name,
        bool unused,
        IFlowState flowStateAfter,
        SelfParameterType parameterType,
        IMaybeAntetype bindingAntetype,
        Pseudotype bindingType,
        IMethodSelfParameterSyntax syntax,
        bool isLentBinding,
        ICapabilityConstraintNode capability)
    {
        Name = name;
        Unused = unused;
        FlowStateAfter = flowStateAfter;
        ParameterType = parameterType;
        BindingAntetype = bindingAntetype;
        BindingType = bindingType;
        Syntax = syntax;
        IsLentBinding = isLentBinding;
        Capability = Child.Attach(this, capability);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldParameterNode : SemanticNode, IFieldParameterNode
{
    private IFieldParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public bool Unused { [DebuggerStepThrough] get; }
    public IMaybeAntetype BindingAntetype { [DebuggerStepThrough] get; }
    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public ParameterType ParameterType { [DebuggerStepThrough] get; }
    public IFieldParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
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

    public FieldParameterNode(
        bool unused,
        IMaybeAntetype bindingAntetype,
        IFlowState flowStateAfter,
        DataType bindingType,
        ParameterType parameterType,
        IFieldParameterSyntax syntax,
        IdentifierName name)
    {
        Unused = unused;
        BindingAntetype = bindingAntetype;
        FlowStateAfter = flowStateAfter;
        BindingType = bindingType;
        ParameterType = parameterType;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockBodyNode : SemanticNode, IBlockBodyNode
{
    private IBlockBodyNode Self { [Inline] get => this; }

    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IBodyStatementNode> Statements { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public BlockBodyNode(
        IFlowState flowStateAfter,
        IBlockBodySyntax syntax,
        IEnumerable<IBodyStatementNode> statements)
    {
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionBodyNode : SemanticNode, IExpressionBodyNode
{
    private IExpressionBodyNode Self { [Inline] get => this; }

    public IFlowState FlowStateAfter { [DebuggerStepThrough] get; }
    public IExpressionBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IResultStatementNode ResultStatement { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;

    public ExpressionBodyNode(
        IFlowState flowStateAfter,
        IExpressionBodySyntax syntax,
        IResultStatementNode resultStatement)
    {
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierTypeNameNode : SemanticNode, IIdentifierTypeNameNode
{
    private IIdentifierTypeNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
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
    public bool IsAttributeType
        => GrammarAttribute.IsCached(in isAttributeTypeCached) ? isAttributeType
            : this.Inherited(ref isAttributeTypeCached, ref isAttributeType, ref syncLock,
                Inherited_IsAttributeType);
    private bool isAttributeType;
    private bool isAttributeTypeCached;
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

    public IdentifierTypeNameNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        IIdentifierTypeNameSyntax syntax,
        IdentifierName name)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SpecialTypeNameNode : SemanticNode, ISpecialTypeNameNode
{
    private ISpecialTypeNameNode Self { [Inline] get => this; }

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

    public SpecialTypeNameNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        ISpecialTypeNameSyntax syntax,
        SpecialTypeName name)
    {
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
    private AttributeLock syncLock;

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public BareType? NamedBareType { [DebuggerStepThrough] get; }
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
    public bool IsAttributeType
        => GrammarAttribute.IsCached(in isAttributeTypeCached) ? isAttributeType
            : this.Inherited(ref isAttributeTypeCached, ref isAttributeType, ref syncLock,
                Inherited_IsAttributeType);
    private bool isAttributeType;
    private bool isAttributeTypeCached;
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

    public GenericTypeNameNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        BareType? namedBareType,
        IGenericTypeNameSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Name = name;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameNode : SemanticNode, IQualifiedTypeNameNode
{
    private IQualifiedTypeNameNode Self { [Inline] get => this; }

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

    public QualifiedTypeNameNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        TypeName name,
        BareType? namedBareType,
        IQualifiedTypeNameSyntax syntax,
        ITypeNameNode context,
        IStandardTypeNameNode qualifiedName)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Name = name;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        QualifiedName = Child.Attach(this, qualifiedName);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalTypeNode : SemanticNode, IOptionalTypeNode
{
    private IOptionalTypeNode Self { [Inline] get => this; }

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public IOptionalTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public OptionalTypeNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        IOptionalTypeSyntax syntax,
        ITypeNode referent)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityTypeNode : SemanticNode, ICapabilityTypeNode
{
    private ICapabilityTypeNode Self { [Inline] get => this; }

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ICapabilityTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityTypeNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ICapabilityTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionTypeNode : SemanticNode, IFunctionTypeNode
{
    private IFunctionTypeNode Self { [Inline] get => this; }

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public IFunctionTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IParameterTypeNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode Return { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public FunctionTypeNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        IFunctionTypeSyntax syntax,
        IEnumerable<IParameterTypeNode> parameters,
        ITypeNode @return)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ParameterTypeNode : SemanticNode, IParameterTypeNode
{
    private IParameterTypeNode Self { [Inline] get => this; }

    public IParameterTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public bool IsLent { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public ParameterType Parameter { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public ParameterTypeNode(
        IParameterTypeSyntax syntax,
        bool isLent,
        ITypeNode referent,
        ParameterType parameter)
    {
        Syntax = syntax;
        IsLent = isLent;
        Referent = Child.Attach(this, referent);
        Parameter = parameter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityViewpointTypeNode : SemanticNode, ICapabilityViewpointTypeNode
{
    private ICapabilityViewpointTypeNode Self { [Inline] get => this; }

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ICapabilityViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityViewpointTypeNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfViewpointTypeNode : SemanticNode, ISelfViewpointTypeNode
{
    private ISelfViewpointTypeNode Self { [Inline] get => this; }

    public IMaybeAntetype NamedAntetype { [DebuggerStepThrough] get; }
    public DataType NamedType { [DebuggerStepThrough] get; }
    public ISelfViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public Pseudotype? MethodSelfType
        => GrammarAttribute.IsCached(in methodSelfTypeCached) ? methodSelfType
            : this.Inherited(ref methodSelfTypeCached, ref methodSelfType,
                Inherited_MethodSelfType);
    private Pseudotype? methodSelfType;
    private bool methodSelfTypeCached;

    public SelfViewpointTypeNode(
        IMaybeAntetype namedAntetype,
        DataType namedType,
        ISelfViewpointTypeSyntax syntax,
        ITypeNode referent)
    {
        NamedAntetype = namedAntetype;
        NamedType = namedType;
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class EntryNode : SemanticNode, IEntryNode
{
    private IEntryNode Self { [Inline] get => this; }

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

    public EntryNode(
        ICodeSyntax? syntax,
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
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

    public ExitNode(
        ICodeSyntax? syntax,
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned)
    {
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;

    public ResultStatementNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IMaybeAntetype antetype,
        DataType type,
        IResultStatementSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression,
        IFlowState flowStateAfter)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        Antetype = antetype;
        Type = type;
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
        CurrentExpression = currentExpression;
        FlowStateAfter = flowStateAfter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableDeclarationStatementNode : SemanticNode, IVariableDeclarationStatementNode
{
    private IVariableDeclarationStatementNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public VariableDeclarationStatementNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IVariableDeclarationStatementSyntax syntax,
        bool isMutableBinding,
        IdentifierName name,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer,
        IAmbiguousExpressionNode? currentInitializer)
    {
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
        Capability = Child.Attach(this, capability);
        Type = Child.Attach(this, type);
        TempInitializer = Child.Attach(this, initializer);
        CurrentInitializer = currentInitializer;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionStatementNode : SemanticNode, IExpressionStatementNode
{
    private IExpressionStatementNode Self { [Inline] get => this; }

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

    public ExpressionStatementNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeAntetype? resultAntetype,
        DataType? resultType,
        IFlowState flowStateAfter,
        IExpressionStatementSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        ResultAntetype = resultAntetype;
        ResultType = resultType;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
        CurrentExpression = currentExpression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingContextPatternNode : SemanticNode, IBindingContextPatternNode
{
    private IBindingContextPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public BindingContextPatternNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        IBindingContextPatternSyntax syntax,
        bool isMutableBinding,
        IPatternNode pattern,
        ITypeNode? type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        IsMutableBinding = isMutableBinding;
        Pattern = Child.Attach(this, pattern);
        Type = Child.Attach(this, type);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingPatternNode : SemanticNode, IBindingPatternNode
{
    private IBindingPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public BindingPatternNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IBindingPatternSyntax syntax,
        bool isMutableBinding,
        IdentifierName name)
    {
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

    public OptionalPatternNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IFlowState flowStateAfter,
        IOptionalPatternSyntax syntax,
        IOptionalOrBindingPatternNode pattern)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockExpressionNode : SemanticNode, IBlockExpressionNode
{
    private IBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public BlockExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IBlockExpressionSyntax syntax,
        IEnumerable<IStatementNode> statements,
        IMaybeAntetype antetype,
        DataType type,
        IFlowState flowStateAfter)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NewObjectExpressionNode : SemanticNode, INewObjectExpressionNode
{
    private INewObjectExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public NewObjectExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        INewObjectExpressionSyntax syntax,
        ITypeNameNode constructingType,
        IdentifierName? constructorName,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IMaybeAntetype constructingAntetype,
        IEnumerable<IConstructorDeclarationNode> referencedConstructors,
        IEnumerable<IConstructorDeclarationNode> compatibleConstructors,
        IConstructorDeclarationNode? referencedConstructor,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        ConstructingType = Child.Attach(this, constructingType);
        ConstructorName = constructorName;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        ConstructingAntetype = constructingAntetype;
        ReferencedConstructors = referencedConstructors.ToFixedSet();
        CompatibleConstructors = compatibleConstructors.ToFixedSet();
        ReferencedConstructor = referencedConstructor;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnsafeExpressionNode : SemanticNode, IUnsafeExpressionNode
{
    private IUnsafeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnsafeExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IUnsafeExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BoolLiteralExpressionNode : SemanticNode, IBoolLiteralExpressionNode
{
    private IBoolLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BoolLiteralExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IBoolLiteralExpressionSyntax syntax,
        bool value,
        BoolConstValueType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IntegerLiteralExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IIntegerLiteralExpressionSyntax syntax,
        BigInteger value,
        IntegerConstValueType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NoneLiteralExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        INoneLiteralExpressionSyntax syntax,
        OptionalType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public StringLiteralExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IStringLiteralExpressionSyntax syntax,
        string value,
        DataType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AssignmentExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IAssignmentExpressionSyntax syntax,
        IAmbiguousAssignableExpressionNode leftOperand,
        IAmbiguousAssignableExpressionNode currentLeftOperand,
        AssignmentOperator @operator,
        IAmbiguousExpressionNode rightOperand,
        IAmbiguousExpressionNode currentRightOperand)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        DataFlowPrevious = dataFlowPrevious;
        DefinitelyAssigned = definitelyAssigned;
        DefinitelyUnassigned = definitelyUnassigned;
        Syntax = syntax;
        TempLeftOperand = Child.Attach(this, leftOperand);
        CurrentLeftOperand = currentLeftOperand;
        Operator = @operator;
        TempRightOperand = Child.Attach(this, rightOperand);
        CurrentRightOperand = currentRightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BinaryOperatorExpressionNode : SemanticNode, IBinaryOperatorExpressionNode
{
    private IBinaryOperatorExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public BinaryOperatorExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        BinaryOperator @operator,
        IAmbiguousExpressionNode rightOperand,
        IAntetype? numericOperatorCommonAntetype)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempLeftOperand = Child.Attach(this, leftOperand);
        Operator = @operator;
        TempRightOperand = Child.Attach(this, rightOperand);
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnaryOperatorExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IUnaryOperatorExpressionSyntax syntax,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IAmbiguousExpressionNode operand)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Fixity = fixity;
        Operator = @operator;
        TempOperand = Child.Attach(this, operand);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdExpressionNode : SemanticNode, IIdExpressionNode
{
    private IIdExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IdExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IIdExpressionSyntax syntax,
        IAmbiguousExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConversionExpressionNode : SemanticNode, IConversionExpressionNode
{
    private IConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ConversionExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IConversionExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        ConversionOperator @operator,
        ITypeNode convertToType)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = Child.Attach(this, referent);
        Operator = @operator;
        ConvertToType = Child.Attach(this, convertToType);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitConversionExpressionNode : SemanticNode, IImplicitConversionExpressionNode
{
    private IImplicitConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitConversionExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode referent,
        IExpressionNode currentReferent,
        SimpleAntetype antetype)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Referent = Child.Attach(this, referent);
        CurrentReferent = currentReferent;
        Antetype = antetype;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PatternMatchExpressionNode : SemanticNode, IPatternMatchExpressionNode
{
    private IPatternMatchExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PatternMatchExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempReferent = Child.Attach(this, referent);
        Pattern = Child.Attach(this, pattern);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IfExpressionNode : SemanticNode, IIfExpressionNode
{
    private IIfExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IfExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IIfExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause,
        IFlowState flowStateAfter)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        Syntax = syntax;
        TempCondition = Child.Attach(this, condition);
        ThenBlock = Child.Attach(this, thenBlock);
        ElseClause = Child.Attach(this, elseClause);
        FlowStateAfter = flowStateAfter;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class LoopExpressionNode : SemanticNode, ILoopExpressionNode
{
    private ILoopExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public LoopExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        ILoopExpressionSyntax syntax,
        IBlockExpressionNode block)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Block = Child.Attach(this, block);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class WhileExpressionNode : SemanticNode, IWhileExpressionNode
{
    private IWhileExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public WhileExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempCondition = Child.Attach(this, condition);
        Block = Child.Attach(this, block);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ForeachExpressionNode : SemanticNode, IForeachExpressionNode
{
    private IForeachExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public ForeachExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isLentBinding,
        IMaybeAntetype bindingAntetype,
        IdentifierName name,
        DataType bindingType,
        IFixedSet<IDataFlowNode> dataFlowPrevious,
        BindingFlags<IVariableBindingNode> definitelyAssigned,
        BindingFlags<IVariableBindingNode> definitelyUnassigned,
        IForeachExpressionSyntax syntax,
        bool isMutableBinding,
        IdentifierName variableName,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? declaredType,
        IBlockExpressionNode block,
        ITypeDeclarationNode? referencedIterableDeclaration,
        IStandardMethodDeclarationNode? referencedIterateMethod,
        IMaybeExpressionAntetype iteratorAntetype,
        DataType iteratorType,
        ITypeDeclarationNode? referencedIteratorDeclaration,
        IStandardMethodDeclarationNode? referencedNextMethod,
        IMaybeAntetype iteratedAntetype,
        DataType iteratedType,
        IFlowState flowStateBeforeBlock)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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
        TempInExpression = Child.Attach(this, inExpression);
        DeclaredType = Child.Attach(this, declaredType);
        Block = Child.Attach(this, block);
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BreakExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        IBreakExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        TempValue = Child.Attach(this, value);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NextExpressionNode : SemanticNode, INextExpressionNode
{
    private INextExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NextExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        INextExpressionSyntax syntax)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public ReturnExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        NeverType type,
        IReturnExpressionSyntax syntax,
        IAmbiguousExpressionNode? value,
        IAmbiguousExpressionNode? currentValue)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        TempValue = Child.Attach(this, value);
        CurrentValue = currentValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnresolvedInvocationExpressionNode : SemanticNode, IUnresolvedInvocationExpressionNode
{
    private IUnresolvedInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

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

    public UnresolvedInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IAmbiguousExpressionNode currentExpression,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IAmbiguousExpressionNode> currentArguments)
    {
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
        CurrentExpression = currentExpression;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CurrentArguments = currentArguments.ToFixedList();
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IFunctionLikeDeclarationNode> compatibleDeclarations,
        IFunctionLikeDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        FunctionGroup = Child.Attach(this, functionGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CompatibleDeclarations = compatibleDeclarations.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodInvocationExpressionNode : SemanticNode, IMethodInvocationExpressionNode
{
    private IMethodInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IAmbiguousExpressionNode> currentArguments,
        IEnumerable<IStandardMethodDeclarationNode> compatibleDeclarations,
        IStandardMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        MethodGroup = Child.Attach(this, methodGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CurrentArguments = currentArguments.ToFixedList();
        CompatibleDeclarations = compatibleDeclarations.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterInvocationExpressionNode : SemanticNode, IGetterInvocationExpressionNode
{
    private IGetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public GetterInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        IGetterMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterInvocationExpressionNode : SemanticNode, ISetterInvocationExpressionNode
{
    private ISetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SetterInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        ISetterMethodDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        TempValue = Child.Attach(this, value);
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionReferenceInvocationExpressionNode : SemanticNode, IFunctionReferenceInvocationExpressionNode
{
    private IFunctionReferenceInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionReferenceInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        FunctionAntetype functionAntetype,
        FunctionType functionType)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        Expression = Child.Attach(this, expression);
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IEnumerable<IAmbiguousExpressionNode> tempAllArguments,
        IEnumerable<IExpressionNode?> allArguments,
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments,
        IEnumerable<IInitializerDeclarationNode> compatibleDeclarations,
        IInitializerDeclarationNode? referencedDeclaration,
        ContextualizedOverload? contextualizedOverload)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        TempAllArguments = tempAllArguments;
        AllArguments = allArguments;
        Syntax = syntax;
        InitializerGroup = Child.Attach(this, initializerGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
        CompatibleDeclarations = compatibleDeclarations.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
        ContextualizedOverload = contextualizedOverload;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownInvocationExpressionNode : SemanticNode, IUnknownInvocationExpressionNode
{
    private IUnknownInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownInvocationExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierNameExpressionNode : SemanticNode, IIdentifierNameExpressionNode
{
    private IIdentifierNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public IdentifierNameExpressionNode(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
    {
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericNameExpressionNode : SemanticNode, IGenericNameExpressionNode
{
    private IGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public GenericNameExpressionNode(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
    {
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
        Syntax = syntax;
        Name = name;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MemberAccessExpressionNode : SemanticNode, IMemberAccessExpressionNode
{
    private IMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public MemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IAmbiguousExpressionNode context,
        StandardName memberName,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TempContext = Child.Attach(this, context);
        MemberName = memberName;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PropertyNameNode : SemanticNode, IPropertyNameNode
{
    private IPropertyNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public PropertyNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnqualifiedNamespaceNameNode : SemanticNode, IUnqualifiedNamespaceNameNode
{
    private IUnqualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnqualifiedNamespaceNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedNamespaceNameNode : SemanticNode, IQualifiedNamespaceNameNode
{
    private IQualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedNamespaceNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations,
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IdentifierName name)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
        Syntax = syntax;
        Context = Child.Attach(this, context);
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionGroupNameNode : SemanticNode, IFunctionGroupNameNode
{
    private IFunctionGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionGroupNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionLikeDeclarationNode> referencedDeclarations)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionNameNode : SemanticNode, IFunctionNameNode
{
    private IFunctionNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        StandardName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IFunctionLikeDeclarationNode? referencedDeclaration)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        FunctionGroup = Child.Attach(this, functionGroup);
        FunctionName = functionName;
        TypeArguments = typeArguments.ToFixedList();
        ReferencedDeclaration = referencedDeclaration;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodGroupNameNode : SemanticNode, IMethodGroupNameNode
{
    private IMethodGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodGroupNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IExpressionNode currentContext,
        StandardName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IStandardMethodDeclarationNode> referencedDeclarations)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        CurrentContext = currentContext;
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldAccessExpressionNode : SemanticNode, IFieldAccessExpressionNode
{
    private IFieldAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FieldAccessExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IdentifierName fieldName,
        IFieldDeclarationNode referencedDeclaration)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        FieldName = fieldName;
        ReferencedDeclaration = referencedDeclaration;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableNameExpressionNode : SemanticNode, IVariableNameExpressionNode
{
    private IVariableNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public VariableNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name,
        ILocalBindingNode referencedDefinition,
        IFixedSet<IDataFlowNode> dataFlowPrevious)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public StandardTypeNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        StandardName name,
        ITypeDeclarationNode referencedDeclaration,
        IMaybeAntetype namedAntetype,
        BareType? namedBareType,
        IStandardNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameExpressionNode : SemanticNode, IQualifiedTypeNameExpressionNode
{
    private IQualifiedTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedTypeNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        StandardName name,
        ITypeDeclarationNode referencedDeclaration,
        IMaybeAntetype namedAntetype,
        BareType? namedBareType,
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<ITypeNode> typeArguments)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Name = name;
        ReferencedDeclaration = referencedDeclaration;
        NamedAntetype = namedAntetype;
        NamedBareType = namedBareType;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerGroupNameNode : SemanticNode, IInitializerGroupNameNode
{
    private IInitializerGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerGroupNameNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        INameExpressionSyntax syntax,
        ITypeNameExpressionNode context,
        StandardName? initializerName,
        IMaybeAntetype initializingAntetype,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        InitializerName = initializerName;
        InitializingAntetype = initializingAntetype;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SpecialTypeNameExpressionNode : SemanticNode, ISpecialTypeNameExpressionNode
{
    private ISpecialTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SpecialTypeNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        ISpecialTypeNameExpressionSyntax syntax,
        SpecialTypeName name,
        TypeSymbol referencedSymbol,
        UnknownType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
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

    public SelfExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        ISelfExpressionSyntax syntax,
        bool isImplicit,
        Pseudotype pseudotype,
        ISelfParameterNode? referencedDefinition)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MissingNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        IMissingNameSyntax syntax,
        UnknownType type)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
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

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownIdentifierNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IIdentifierNameExpressionSyntax syntax,
        IdentifierName name)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        Syntax = syntax;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownGenericNameExpressionNode : SemanticNode, IUnknownGenericNameExpressionNode
{
    private IUnknownGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownGenericNameExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        Syntax = syntax;
        Name = name;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownMemberAccessExpressionNode : SemanticNode, IUnknownMemberAccessExpressionNode
{
    private IUnknownMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownMemberAccessExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        IFlowState flowStateAfter,
        UnknownType type,
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName memberName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IDeclarationNode> referencedMembers)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        FlowStateAfter = flowStateAfter;
        Type = type;
        Syntax = syntax;
        Context = Child.Attach(this, context);
        MemberName = memberName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedMembers = referencedMembers.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousMoveExpressionNode : SemanticNode, IAmbiguousMoveExpressionNode
{
    private IAmbiguousMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public AmbiguousMoveExpressionNode(
        IMoveExpressionSyntax syntax,
        IUnresolvedSimpleNameNode referent)
    {
        Syntax = syntax;
        TempReferent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveVariableExpressionNode : SemanticNode, IMoveVariableExpressionNode
{
    private IMoveVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveVariableExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        ILocalBindingNameExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveValueExpressionNode : SemanticNode, IMoveValueExpressionNode
{
    private IMoveValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveValueExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        IExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitTempMoveExpressionNode : SemanticNode, IImplicitTempMoveExpressionNode
{
    private IImplicitTempMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitTempMoveExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousFreezeExpressionNode : SemanticNode, IAmbiguousFreezeExpressionNode
{
    private IAmbiguousFreezeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

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

    public AmbiguousFreezeExpressionNode(
        IFreezeExpressionSyntax syntax,
        IUnresolvedSimpleNameNode referent)
    {
        Syntax = syntax;
        TempReferent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeVariableExpressionNode : SemanticNode, IFreezeVariableExpressionNode
{
    private IFreezeVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeVariableExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        bool isTemporary,
        ILocalBindingNameExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        IsTemporary = isTemporary;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeValueExpressionNode : SemanticNode, IFreezeValueExpressionNode
{
    private IFreezeValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeValueExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        bool isImplicit,
        bool isTemporary,
        IExpressionNode referent)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        IsImplicit = isImplicit;
        IsTemporary = isTemporary;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PrepareToReturnExpressionNode : SemanticNode, IPrepareToReturnExpressionNode
{
    private IPrepareToReturnExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PrepareToReturnExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IExpressionSyntax syntax,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IExpressionNode value,
        IExpressionNode currentValue)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Syntax = syntax;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Value = Child.Attach(this, value);
        CurrentValue = currentValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncBlockExpressionNode : SemanticNode, IAsyncBlockExpressionNode
{
    private IAsyncBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncBlockExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAsyncBlockExpressionSyntax syntax,
        IBlockExpressionNode block)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Block = Child.Attach(this, block);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncStartExpressionNode : SemanticNode, IAsyncStartExpressionNode
{
    private IAsyncStartExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncStartExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAsyncStartExpressionSyntax syntax,
        bool scheduled,
        IAmbiguousExpressionNode expression)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        Scheduled = scheduled;
        TempExpression = Child.Attach(this, expression);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AwaitExpressionNode : SemanticNode, IAwaitExpressionNode
{
    private IAwaitExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public ControlFlowSet ControlFlowNext { [DebuggerStepThrough] get; }
    public ControlFlowSet ControlFlowPrevious { [DebuggerStepThrough] get; }
    public IMaybeExpressionAntetype Antetype { [DebuggerStepThrough] get; }
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
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private DataType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype,
                Inherited_ExpectedAntetype);
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AwaitExpressionNode(
        ControlFlowSet controlFlowNext,
        ControlFlowSet controlFlowPrevious,
        IMaybeExpressionAntetype antetype,
        DataType type,
        IFlowState flowStateAfter,
        IAwaitExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        ControlFlowNext = controlFlowNext;
        ControlFlowPrevious = controlFlowPrevious;
        Antetype = antetype;
        Type = type;
        FlowStateAfter = flowStateAfter;
        Syntax = syntax;
        TempExpression = Child.Attach(this, expression);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageSymbolNode : SemanticNode, IPackageSymbolNode
{
    private IPackageSymbolNode Self { [Inline] get => this; }

    public IdentifierName? AliasOrName { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public PackageSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageFacetSymbolNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetSymbolNode TestingFacet { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());

    public PackageSymbolNode(
        IdentifierName? aliasOrName,
        IdentifierName name,
        PackageSymbol symbol,
        IPackageFacetSymbolNode mainFacet,
        IPackageFacetSymbolNode testingFacet)
    {
        AliasOrName = aliasOrName;
        Name = name;
        Symbol = symbol;
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
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

    public FixedSymbolTree SymbolTree { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public INamespaceSymbolNode GlobalNamespace { [DebuggerStepThrough] get; }

    public PackageFacetSymbolNode(
        FixedSymbolTree symbolTree)
    {
        SymbolTree = symbolTree;
        GlobalNamespace = Child.Attach(this, SymbolNodeAspect.PackageFacetSymbol_GlobalNamespace(this));
    }

    internal override IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override ISymbolTree Inherited_SymbolTree(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return SymbolTree;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceSymbolNode : SemanticNode, INamespaceSymbolNode
{
    private INamespaceSymbolNode Self { [Inline] get => this; }

    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceMemberSymbolNode> Members { [DebuggerStepThrough] get; }
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
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => GrammarAttribute.IsCached(in nestedMembersCached) ? nestedMembers!
            : this.Synthetic(ref nestedMembersCached, ref nestedMembers,
                DeclarationsAspect.NamespaceDeclaration_NestedMembers);
    private IFixedList<INamespaceMemberDeclarationNode>? nestedMembers;
    private bool nestedMembersCached;

    public NamespaceSymbolNode(
        NamespaceSymbol symbol,
        IEnumerable<INamespaceMemberSymbolNode> members)
    {
        Symbol = symbol;
        Members = ChildList.Attach(this, members);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionSymbolNode : SemanticNode, IFunctionSymbolNode
{
    private IFunctionSymbolNode Self { [Inline] get => this; }

    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public FunctionSymbolNode(
        FunctionSymbol symbol,
        StandardName name,
        FunctionType type)
    {
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
    public TypeSymbol Symbol { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberSymbolNode> Members { [DebuggerStepThrough] get; }
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

    public PrimitiveTypeSymbolNode(
        ISyntax? syntax,
        TypeSymbol symbol,
        SpecialTypeName name,
        IEnumerable<ITypeMemberSymbolNode> members)
    {
        Syntax = syntax;
        Symbol = symbol;
        Name = name;
        Members = ChildSet.Attach(this, members);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassSymbolNode : SemanticNode, IClassSymbolNode
{
    private IClassSymbolNode Self { [Inline] get => this; }

    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSymbolNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<IClassMemberSymbolNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
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

    public ClassSymbolNode(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<IClassMemberSymbolNode> members)
    {
        Name = name;
        Symbol = symbol;
        GenericParameters = ChildList.Attach(this, genericParameters);
        Members = ChildSet.Attach(this, members);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructSymbolNode : SemanticNode, IStructSymbolNode
{
    private IStructSymbolNode Self { [Inline] get => this; }

    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSymbolNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<IStructMemberSymbolNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
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

    public StructSymbolNode(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<IStructMemberSymbolNode> members)
    {
        Name = name;
        Symbol = symbol;
        GenericParameters = ChildList.Attach(this, genericParameters);
        Members = ChildSet.Attach(this, members);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitSymbolNode : SemanticNode, ITraitSymbolNode
{
    private ITraitSymbolNode Self { [Inline] get => this; }

    public StandardName Name { [DebuggerStepThrough] get; }
    public UserTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSymbolNode> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedSet<ITraitMemberSymbolNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
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

    public TraitSymbolNode(
        StandardName name,
        UserTypeSymbol symbol,
        IEnumerable<IGenericParameterSymbolNode> genericParameters,
        IEnumerable<ITraitMemberSymbolNode> members)
    {
        Name = name;
        Symbol = symbol;
        GenericParameters = ChildList.Attach(this, genericParameters);
        Members = ChildSet.Attach(this, members);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterSymbolNode : SemanticNode, IGenericParameterSymbolNode
{
    private IGenericParameterSymbolNode Self { [Inline] get => this; }

    public IdentifierName Name { [DebuggerStepThrough] get; }
    public GenericParameterTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedSet<ITypeMemberSymbolNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public GenericParameterSymbolNode(
        IdentifierName name,
        GenericParameterTypeSymbol symbol,
        IEnumerable<ITypeMemberSymbolNode> members)
    {
        Name = name;
        Symbol = symbol;
        Members = members.ToFixedSet();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardMethodSymbolNode : SemanticNode, IStandardMethodSymbolNode
{
    private IStandardMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public StandardMethodSymbolNode(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodSymbolNode : SemanticNode, IGetterMethodSymbolNode
{
    private IGetterMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public GetterMethodSymbolNode(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodSymbolNode : SemanticNode, ISetterMethodSymbolNode
{
    private ISetterMethodSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public SetterMethodSymbolNode(
        ISyntax? syntax,
        IdentifierName name,
        MethodSymbol symbol)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSymbolNode : SemanticNode, IConstructorSymbolNode
{
    private IConstructorSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public ConstructorSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public ConstructorSymbolNode(
        ISyntax? syntax,
        IdentifierName? name,
        ConstructorSymbol symbol)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSymbolNode : SemanticNode, IInitializerSymbolNode
{
    private IInitializerSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public InitializerSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public InitializerSymbolNode(
        ISyntax? syntax,
        IdentifierName? name,
        InitializerSymbol symbol)
    {
        Syntax = syntax;
        Name = name;
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldSymbolNode : SemanticNode, IFieldSymbolNode
{
    private IFieldSymbolNode Self { [Inline] get => this; }

    public ISyntax? Syntax { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public DataType BindingType { [DebuggerStepThrough] get; }
    public FieldSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public FieldSymbolNode(
        ISyntax? syntax,
        IdentifierName name,
        DataType bindingType,
        FieldSymbol symbol)
    {
        Syntax = syntax;
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
    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public FunctionType Type { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());

    public AssociatedFunctionSymbolNode(
        ISyntax? syntax,
        FunctionSymbol symbol,
        FunctionType type,
        StandardName name)
    {
        Syntax = syntax;
        Symbol = symbol;
        Type = type;
        Name = name;
    }
}

