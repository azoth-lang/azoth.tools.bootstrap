using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;
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
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using InlineMethod;
using AzothType = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;
using AzothPlainType = Azoth.Tools.Bootstrap.Compiler.Types.Plain.PlainType;

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
    typeof(IControlFlowNode),
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
public partial interface IElseClauseNode : ICodeNode, IControlFlowNode
{
    ValueId ValueId { get; }
    IFlowState FlowStateAfter { get; }
}

[Closed(
    typeof(IResultStatementNode),
    typeof(IBlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockOrResultNode : IElseClauseNode
{
    IMaybeType Type { get; }
    IMaybePlainType PlainType { get; }
}

[Closed(
    typeof(INamedBindingNode),
    typeof(ISelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingNode : ICodeNode, IBindingDeclarationNode
{
    ValueId BindingValueId { get; }
    IMaybeNonVoidPlainType BindingPlainType { get; }
    IMaybeNonVoidType BindingType { get; }
    bool IsLentBinding { get; }
}

[Closed(
    typeof(ILocalBindingNode),
    typeof(IFieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedBindingNode : IBindingNode, INamedBindingDeclarationNode
{
    LexicalScope ContainingLexicalScope { get; }
    bool IsMutableBinding { get; }
}

[Closed(
    typeof(IVariableBindingNode),
    typeof(INamedParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILocalBindingNode : INamedBindingNode
{
    new ILocalBindingSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    new IdentifierName? AliasOrName
        => null;
    IdentifierName? IPackageDeclarationNode.AliasOrName => AliasOrName;
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IPackageDeclarationNode.Name => Name;
    IFunctionDefinitionNode? EntryPoint { get; }
    DiagnosticCollection Diagnostics { get; }
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
    IdentifierName AliasOrName { get; }
    bool IsTrusted { get; }
    IPackageSymbols PackageSymbols { get; }
}

// [Closed(typeof(StandardPackageReferenceNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardPackageReferenceNode : IPackageReferenceNode
{
    new IPackageReferenceSyntax Syntax { get; }
    IPackageReferenceSyntax? IPackageReferenceNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IdentifierName IPackageReferenceNode.AliasOrName
        => Syntax.AliasOrName;
    bool IPackageReferenceNode.IsTrusted
        => Syntax.IsTrusted;
    IPackageSymbols IPackageReferenceNode.PackageSymbols
        => Syntax.Package;

    public static IStandardPackageReferenceNode Create(IPackageReferenceSyntax syntax)
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
    IdentifierName IPackageReferenceNode.AliasOrName
        => PackageSymbols.PackageSymbol.Name;
    bool IPackageReferenceNode.IsTrusted
        => true;
    IPackageSymbols IPackageReferenceNode.PackageSymbols
        => IntrinsicPackageSymbol.Instance;

    public static IIntrinsicsPackageReferenceNode Create()
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
    IFixedSet<IFacetMemberDefinitionNode> Definitions { get; }
    new INamespaceDefinitionNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;

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
    typeof(IElseClauseNode),
    typeof(IBindingNode),
    typeof(ICompilationUnitNode),
    typeof(IImportDirectiveNode),
    typeof(IConcreteFunctionInvocableDefinitionNode),
    typeof(ITypeDefinitionNode),
    typeof(IGenericParameterNode),
    typeof(IMethodDefinitionNode),
    typeof(IOrdinaryConstructorDefinitionNode),
    typeof(IOrdinaryInitializerDefinitionNode),
    typeof(IFieldDefinitionNode),
    typeof(IAttributeNode),
    typeof(ICapabilityConstraintNode),
    typeof(IParameterNode),
    typeof(ITypeNode),
    typeof(IParameterTypeNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IAmbiguousExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICodeNode : IChildNode
{
    new ICodeSyntax Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    CodeFile File { get; }
}

// [Closed(typeof(CompilationUnitNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICompilationUnitNode : ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<IImportDirectiveNode> ImportDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Definitions { get; }
    NamespaceScope ContainingLexicalScope { get; }
    NamespaceSearchScope LexicalScope { get; }
    new CodeFile File
        => Syntax.File;
    CodeFile ICodeNode.File => File;
    IPackageFacetNode ContainingDeclaration { get; }
    DiagnosticCollection Diagnostics { get; }
    NamespaceName ImplicitNamespaceName
        => Syntax.ImplicitNamespaceName;
    NamespaceSymbol ImplicitNamespaceSymbol
        => ImplicitNamespace.Symbol;
    PackageSymbol ContainingSymbol
        => ContainingDeclaration.PackageSymbol;
    INamespaceDefinitionNode ImplicitNamespace { get; }

    public static ICompilationUnitNode Create(
        ICompilationUnitSyntax syntax,
        IEnumerable<IImportDirectiveNode> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> definitions)
        => new CompilationUnitNode(syntax, importDirectives, definitions);
}

// [Closed(typeof(ImportDirectiveNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImportDirectiveNode : ICodeNode
{
    new IImportDirectiveSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    NamespaceName Name
        => Syntax.Name;

    public static IImportDirectiveNode Create(IImportDirectiveSyntax syntax)
        => new ImportDirectiveNode(syntax);
}

[Closed(
    typeof(IExecutableDefinitionNode),
    typeof(INamespaceBlockMemberDefinitionNode),
    typeof(ITypeMemberDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefinitionNode : IPackageFacetChildDeclarationNode
{
    new IDefinitionSyntax? Syntax { get; }
    ISyntax? ISemanticNode.Syntax => Syntax;
    LexicalScope ContainingLexicalScope { get; }
    LexicalScope LexicalScope { get; }
    new IPackageFacetNode Facet { get; }
    IPackageFacetDeclarationNode IPackageFacetChildDeclarationNode.Facet => Facet;
    Symbol? ContainingSymbol
        => ContainingDeclaration.Symbol;
}

[Closed(
    typeof(IInvocableDefinitionNode),
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
    typeof(IMethodDefinitionNode),
    typeof(IConstructorDefinitionNode),
    typeof(IInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDefinitionNode : IExecutableDefinitionNode, IInvocableDeclarationNode
{
    IFixedList<IConstructorOrInitializerParameterNode> Parameters { get; }
    IBodyNode? Body { get; }
    IFlowState FlowStateBefore()
        => TypeMemberDeclarationsAspect.InvocableDefinition_FlowStateBefore(this);
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConcreteFunctionInvocableDefinitionNode : ICodeNode, IInvocableDefinitionNode, IFunctionInvocableDeclarationNode
{
    new IInvocableDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new IdentifierName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IBodyNode Body { get; }
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Return?.NamedType ?? AzothType.Void;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Return?.NamedPlainType ?? AzothPlainType.Void;
}

// [Closed(typeof(NamespaceBlockDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockDefinitionNode : INamespaceBlockMemberDefinitionNode
{
    new INamespaceBlockDefinitionSyntax Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<IImportDirectiveNode> ImportDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    new NamespaceSearchScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    new NamespaceSearchScope LexicalScope { get; }
    LexicalScope IDefinitionNode.LexicalScope => LexicalScope;
    new INamespaceDefinitionNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
    NamespaceName DeclaredNames
        => Syntax.DeclaredNames;
    bool IsGlobalQualified
        => Syntax.IsGlobalQualified;
    new NamespaceSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    Symbol? IDefinitionNode.ContainingSymbol => ContainingSymbol;
    NamespaceSymbol Symbol
        => Definition.Symbol;
    INamespaceDefinitionNode ContainingNamespace { get; }
    INamespaceDefinitionNode Definition { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name
        => DeclaredNames.Segments.LastOrDefault();

    public static INamespaceBlockDefinitionNode Create(
        INamespaceBlockDefinitionSyntax syntax,
        IEnumerable<IImportDirectiveNode> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> members)
        => new NamespaceBlockDefinitionNode(syntax, importDirectives, members);
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
    new NamespaceSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    NamespaceSymbol INamespaceDeclarationNode.Symbol => Symbol;
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
    ISyntax? ISemanticNode.Syntax => Syntax;
    IInvocableDefinitionSyntax IConcreteFunctionInvocableDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    OrdinaryName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    new NamespaceSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    Symbol? IDefinitionNode.ContainingSymbol => ContainingSymbol;

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
public partial interface ITypeDefinitionNode : ICodeNode, IFacetMemberDefinitionNode, IAssociatedMemberDefinitionNode, IUserTypeDeclarationNode
{
    new ITypeDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    IFixedList<IStandardTypeNameNode> SupertypeNames { get; }
    LexicalScope SupertypesLexicalScope { get; }
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames
        => SupertypeNames;
    bool IsConst
        => Syntax.ConstModifier is not null;
    new OrdinaryName Name
        => Syntax.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    OrdinaryName INamespaceMemberDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    OrdinaryName IAssociatedMemberDefinitionNode.Name => Name;
    new Symbol ContainingSymbol
        => ContainingDeclaration.Symbol!;
    Symbol? IDefinitionNode.ContainingSymbol => ContainingSymbol;
    new OrdinaryTypeConstructor TypeFactory { get; }
    BareTypeConstructor INonVariableTypeDeclarationNode.TypeFactory => TypeFactory;
    ITypeFactory ITypeDeclarationNode.TypeFactory => TypeFactory;
    new IFixedSet<BareType> Supertypes { get; }
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes => Supertypes;
    new IFixedSet<ITypeMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new IImplicitSelfDefinitionNode ImplicitSelf { get; }
    IImplicitSelfDeclarationNode INonVariableTypeDeclarationNode.ImplicitSelf => ImplicitSelf;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IStandardTypeNameNode? BaseTypeName { get; }
    IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    bool IsAbstract
        => Syntax.AbstractModifier is not null;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    IFixedSet<ITypeMemberDefinitionNode> ITypeDefinitionNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;

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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IGenericParameterDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeParameterIndependence Independence
        => Syntax.Independence;
    TypeParameterVariance Variance
        => Syntax.Variance;
    OrdinaryTypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    OrdinaryTypeConstructor ContainingTypeConstructor { get; }
    TypeConstructorParameter Parameter { get; }
    GenericParameterType DeclaredType { get; }
    new IFixedSet<ITypeMemberDefinitionNode> Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> IGenericParameterDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => [];

    public static IGenericParameterNode Create(
        IGenericParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
        => new GenericParameterNode(syntax, constraint);
}

// [Closed(typeof(ImplicitSelfDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitSelfDefinitionNode : IImplicitSelfDeclarationNode
{
    TypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol!;
    new SelfTypeConstructor TypeFactory { get; }
    AssociatedTypeConstructor IImplicitSelfDeclarationNode.TypeFactory => TypeFactory;
    ITypeFactory ITypeDeclarationNode.TypeFactory => TypeFactory;

    public static IImplicitSelfDefinitionNode Create()
        => new ImplicitSelfDefinitionNode();
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
    typeof(IMethodDefinitionNode),
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
public partial interface IAlwaysTypeMemberDefinitionNode : ITypeMemberDefinitionNode, IAlwaysTypeMemberDeclarationNode
{
    new IUserTypeDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
    ITypeDeclarationNode IAlwaysTypeMemberDeclarationNode.ContainingDeclaration => ContainingDeclaration;
    new OrdinaryTypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    Symbol? IDefinitionNode.ContainingSymbol => ContainingSymbol;
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IAssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedMemberDefinitionNode : IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, INamedDeclarationNode
{
    new OrdinaryName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IOrdinaryMethodDefinitionNode),
    typeof(IGetterMethodDefinitionNode),
    typeof(ISetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodDefinitionNode : ICodeNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, ITraitMemberDefinitionNode, IStructMemberDefinitionNode, IInvocableDefinitionNode, IMethodDeclarationNode
{
    new IMethodDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    IMethodSelfParameterNode SelfParameter { get; }
    new IFixedList<INamedParameterNode> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    ITypeNode? Return { get; }
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName IMethodDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    MethodKind Kind { get; }
    IMaybeNonVoidType IMethodDeclarationNode.SelfParameterType
        => SelfParameter.ParameterType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Return?.NamedType ?? AzothType.Void;
    IMaybeNonVoidPlainType IMethodDeclarationNode.SelfParameterPlainType
        => SelfParameter.BindingPlainType;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Return?.NamedPlainType ?? AzothPlainType.Void;
}

// [Closed(typeof(AbstractMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAbstractMethodDefinitionNode : IMethodDefinitionNode, IOrdinaryMethodDeclarationNode
{
    new IAbstractMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new IBodyNode? Body
        => null;
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    OrdinaryTypeConstructor ContainingTypeConstructor { get; }
    IMaybeFunctionType IOrdinaryMethodDeclarationNode.MethodGroupType
        => Symbol?.MethodGroupType ?? IMaybeFunctionType.Unknown;
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Standard;
    int IOrdinaryMethodDeclarationNode.Arity
        => Parameters.Count;
    IMaybeFunctionPlainType IOrdinaryMethodDeclarationNode.MethodGroupPlainType
        => MethodGroupType.PlainType;

    public static IAbstractMethodDefinitionNode Create(
        IAbstractMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
        => new AbstractMethodDefinitionNode(syntax, selfParameter, parameters, @return);
}

// [Closed(typeof(OrdinaryMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryMethodDefinitionNode : IMethodDefinitionNode, IOrdinaryMethodDeclarationNode
{
    new IOrdinaryMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new IBodyNode Body { get; }
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeFunctionType IOrdinaryMethodDeclarationNode.MethodGroupType
        => Symbol?.MethodGroupType ?? IMaybeFunctionType.Unknown;
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Standard;
    int IOrdinaryMethodDeclarationNode.Arity
        => Parameters.Count;
    IMaybeFunctionPlainType IOrdinaryMethodDeclarationNode.MethodGroupPlainType
        => MethodGroupType.PlainType;

    public static IOrdinaryMethodDefinitionNode Create(
        IOrdinaryMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        => new OrdinaryMethodDefinitionNode(syntax, selfParameter, parameters, @return, body);
}

// [Closed(typeof(GetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDefinitionNode : IMethodDefinitionNode, IGetterMethodDeclarationNode
{
    new IGetterMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new ITypeNode Return { get; }
    ITypeNode? IMethodDefinitionNode.Return => Return;
    MethodKind IMethodDefinitionNode.Kind
        => MethodKind.Getter;

    public static IGetterMethodDefinitionNode Create(
        IGetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode @return,
        IBodyNode? body)
        => new GetterMethodDefinitionNode(syntax, selfParameter, parameters, @return, body);
}

// [Closed(typeof(SetterMethodDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDefinitionNode : IMethodDefinitionNode, ISetterMethodDeclarationNode
{
    new ISetterMethodDefinitionSyntax Syntax { get; }
    IMethodDefinitionSyntax IMethodDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    new IBodyNode Body { get; }
    IBodyNode? IInvocableDefinitionNode.Body => Body;
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
    typeof(IOrdinaryConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorDefinitionNode : IInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IConstructorDeclarationNode
{
    new IConstructorDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    new IdentifierName? Name
        => Syntax?.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName? IConstructorDeclarationNode.Name => Name;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol?.ReturnType ?? IMaybeType.Unknown;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => ContainingTypeDefinition.TypeFactory.ConstructWithParameterPlainTypes();
}

// [Closed(typeof(DefaultConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultConstructorDefinitionNode : IConstructorDefinitionNode
{
    new IConstructorDefinitionSyntax? Syntax
        => null;
    IConstructorDefinitionSyntax? IConstructorDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IFixedList<IConstructorOrInitializerParameterNode> Parameters
        => FixedList.Empty<IConstructorOrInitializerParameterNode>();
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    new IBodyNode? Body
        => null;
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeNonVoidType IConstructorDeclarationNode.SelfParameterType
        => Symbol!.SelfParameterType;
    IMaybeNonVoidPlainType IConstructorDeclarationNode.SelfParameterPlainType
        => ContainingTypeDefinition.TypeFactory.ConstructWithParameterPlainTypes();

    public static IDefaultConstructorDefinitionNode Create()
        => new DefaultConstructorDefinitionNode();
}

// [Closed(typeof(OrdinaryConstructorDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryConstructorDefinitionNode : ICodeNode, IConstructorDefinitionNode
{
    new IConstructorDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IConstructorDefinitionSyntax? IConstructorDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IConstructorSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeNonVoidType IConstructorDeclarationNode.SelfParameterType
        => SelfParameter.ParameterType;
    IMaybeNonVoidPlainType IConstructorDeclarationNode.SelfParameterPlainType
        => SelfParameter.BindingPlainType;

    public static IOrdinaryConstructorDefinitionNode Create(
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        => new OrdinaryConstructorDefinitionNode(syntax, selfParameter, parameters, body);
}

[Closed(
    typeof(IDefaultInitializerDefinitionNode),
    typeof(IOrdinaryInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerDefinitionNode : IInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IStructMemberDefinitionNode, IInitializerDeclarationNode
{
    new IInitializerDefinitionSyntax? Syntax { get; }
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    new IdentifierName? Name
        => Syntax?.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName? IInitializerDeclarationNode.Name => Name;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol?.ReturnType ?? IMaybeType.Unknown;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => ContainingTypeDefinition.TypeFactory.ConstructWithParameterPlainTypes();
}

// [Closed(typeof(DefaultInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefaultInitializerDefinitionNode : IInitializerDefinitionNode
{
    new IInitializerDefinitionSyntax? Syntax
        => null;
    IInitializerDefinitionSyntax? IInitializerDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IFixedList<IConstructorOrInitializerParameterNode> Parameters
        => FixedList.Empty<IConstructorOrInitializerParameterNode>();
    IFixedList<IConstructorOrInitializerParameterNode> IInvocableDefinitionNode.Parameters => Parameters;
    new IBodyNode? Body
        => null;
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeNonVoidType IInitializerDeclarationNode.SelfParameterType
        => Symbol!.SelfParameterType;
    IMaybeNonVoidPlainType IInitializerDeclarationNode.SelfParameterPlainType
        => ContainingTypeDefinition.TypeFactory.ConstructWithParameterPlainTypes();

    public static IDefaultInitializerDefinitionNode Create()
        => new DefaultInitializerDefinitionNode();
}

// [Closed(typeof(OrdinaryInitializerDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryInitializerDefinitionNode : ICodeNode, IInitializerDefinitionNode
{
    new IInitializerDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IInitializerDefinitionSyntax? IInitializerDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IInitializerSelfParameterNode SelfParameter { get; }
    new IBlockBodyNode Body { get; }
    IBodyNode? IInvocableDefinitionNode.Body => Body;
    IMaybeNonVoidType IInitializerDeclarationNode.SelfParameterType
        => SelfParameter.ParameterType;
    IMaybeNonVoidPlainType IInitializerDeclarationNode.SelfParameterPlainType
        => SelfParameter.BindingPlainType;

    public static IOrdinaryInitializerDefinitionNode Create(
        IInitializerDefinitionSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
        => new OrdinaryInitializerDefinitionNode(syntax, selfParameter, parameters, body);
}

// [Closed(typeof(FieldDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldDefinitionNode : ICodeNode, IAlwaysTypeMemberDefinitionNode, IClassMemberDefinitionNode, IStructMemberDefinitionNode, INamedBindingNode, IFieldDeclarationNode, IExecutableDefinitionNode
{
    new IFieldDefinitionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeNode TypeNode { get; }
    IAmbiguousExpressionNode? TempInitializer { get; }
    IExpressionNode? Initializer { get; }
    IAmbiguousExpressionNode? CurrentInitializer { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IDefinitionNode.ContainingLexicalScope => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
    new IMaybeNonVoidType BindingType { get; }
    IMaybeNonVoidType IBindingNode.BindingType => BindingType;
    IMaybeNonVoidType IFieldDeclarationNode.BindingType => BindingType;
    new bool IsMutableBinding
        => Syntax.IsMutableBinding;
    bool INamedBindingNode.IsMutableBinding => IsMutableBinding;
    bool IFieldDeclarationNode.IsMutableBinding => IsMutableBinding;
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    LexicalScope IDefinitionNode.LexicalScope
        => ContainingLexicalScope;
    bool IBindingNode.IsLentBinding
        => false;

    public static IFieldDefinitionNode Create(
        IFieldDefinitionSyntax syntax,
        ITypeNode typeNode,
        IAmbiguousExpressionNode? initializer)
        => new FieldDefinitionNode(syntax, typeNode, initializer);
}

// [Closed(typeof(AssociatedFunctionDefinitionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionDefinitionNode : IConcreteFunctionInvocableDefinitionNode, IAlwaysTypeMemberDefinitionNode, IAssociatedMemberDefinitionNode, IAssociatedFunctionDeclarationNode
{
    new IAssociatedFunctionDefinitionSyntax Syntax { get; }
    IInvocableDefinitionSyntax IConcreteFunctionInvocableDefinitionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IDefinitionSyntax? IDefinitionNode.Syntax => Syntax;
    ITypeMemberDefinitionSyntax? ITypeMemberDefinitionNode.Syntax => Syntax;
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IConcreteFunctionInvocableDefinitionNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    OrdinaryName IAssociatedMemberDefinitionNode.Name => Name;
    OrdinaryName IAssociatedFunctionDeclarationNode.Name => Name;

    public static IAssociatedFunctionDefinitionNode Create(
        IAssociatedFunctionDefinitionSyntax syntax,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
        => new AssociatedFunctionDefinitionNode(syntax, parameters, @return, body);
}

// [Closed(typeof(AttributeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAttributeNode : ICodeNode
{
    new IAttributeSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IStandardTypeNameNode TypeName { get; }
    InvocableSymbol? ReferencedSymbol { get; }

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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(CapabilitySetNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    CapabilitySet CapabilitySet
        => Syntax.CapabilitySet;

    public static ICapabilitySetNode Create(ICapabilitySetSyntax syntax)
        => new CapabilitySetNode(syntax);
}

// [Closed(typeof(CapabilityNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    DeclaredCapability DeclaredCapability
        => Syntax.Capability;

    public static ICapabilityNode Create(ICapabilitySyntax syntax)
        => new CapabilityNode(syntax);
}

[Closed(
    typeof(IConstructorOrInitializerParameterNode),
    typeof(ISelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterNode : ICodeNode
{
    new IParameterSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ValueIdScope ValueIdScope { get; }
    ValueId BindingValueId { get; }
    IFlowState FlowStateBefore();
    IFlowState FlowStateAfter { get; }
    IMaybeNonVoidPlainType BindingPlainType { get; }
    IMaybeNonVoidType BindingType { get; }
    IdentifierName? Name { get; }
    bool Unused
        => Name?.Text.StartsWith('_') ?? false;
}

[Closed(
    typeof(INamedParameterNode),
    typeof(IFieldParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorOrInitializerParameterNode : IParameterNode
{
    new IConstructorOrInitializerParameterSyntax Syntax { get; }
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeParameterType ParameterType { get; }
    new IdentifierName Name { get; }
    IdentifierName? IParameterNode.Name => Name;
}

// [Closed(typeof(NamedParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedParameterNode : IConstructorOrInitializerParameterNode, ILocalBindingNode
{
    new INamedParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    ITypeNode TypeNode { get; }
    new IdentifierName Name
        => Syntax.Name;
    IdentifierName IConstructorOrInitializerParameterNode.Name => Name;
    IdentifierName? IParameterNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new ValueId BindingValueId { get; }
    ValueId IParameterNode.BindingValueId => BindingValueId;
    ValueId IBindingNode.BindingValueId => BindingValueId;
    new IMaybeNonVoidPlainType BindingPlainType { get; }
    IMaybeNonVoidPlainType IParameterNode.BindingPlainType => BindingPlainType;
    IMaybeNonVoidPlainType IBindingNode.BindingPlainType => BindingPlainType;
    new IMaybeNonVoidType BindingType { get; }
    IMaybeNonVoidType IParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IBindingNode.BindingType => BindingType;
    bool IBindingNode.IsLentBinding
        => Syntax.IsLentBinding;
    bool INamedBindingNode.IsMutableBinding
        => Syntax.IsMutableBinding;

    public static INamedParameterNode Create(
        INamedParameterSyntax syntax,
        ITypeNode typeNode)
        => new NamedParameterNode(syntax, typeNode);
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new ConstructedPlainType BindingPlainType { get; }
    IMaybeNonVoidPlainType IParameterNode.BindingPlainType => BindingPlainType;
    IMaybeNonVoidPlainType IBindingNode.BindingPlainType => BindingPlainType;
    IMaybeNonVoidType ParameterType { get; }
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    OrdinaryTypeConstructor ContainingTypeConstructor { get; }
    SelfTypeConstructor ContainingSelfTypeConstructor { get; }
    new ValueId BindingValueId { get; }
    ValueId IParameterNode.BindingValueId => BindingValueId;
    ValueId IBindingNode.BindingValueId => BindingValueId;
    new IMaybeNonVoidType BindingType { get; }
    IMaybeNonVoidType IParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IBindingNode.BindingType => BindingType;
    IdentifierName? IParameterNode.Name
        => null;
    bool IBindingNode.IsLentBinding
        => Syntax.IsLentBinding;
}

// [Closed(typeof(ConstructorSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSelfParameterNode : ISelfParameterNode
{
    new IConstructorSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    IMaybeNonVoidType ISelfParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IBindingNode.BindingType => BindingType;

    public static IConstructorSelfParameterNode Create(
        IConstructorSelfParameterSyntax syntax,
        ICapabilityNode capability)
        => new ConstructorSelfParameterNode(syntax, capability);
}

// [Closed(typeof(InitializerSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSelfParameterNode : ISelfParameterNode
{
    new IInitializerSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    new CapabilityType BindingType { get; }
    IMaybeNonVoidType ISelfParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IParameterNode.BindingType => BindingType;
    IMaybeNonVoidType IBindingNode.BindingType => BindingType;

    public static IInitializerSelfParameterNode Create(
        IInitializerSelfParameterSyntax syntax,
        ICapabilityNode capability)
        => new InitializerSelfParameterNode(syntax, capability);
}

// [Closed(typeof(MethodSelfParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSelfParameterNode : ISelfParameterNode
{
    new IMethodSelfParameterSyntax Syntax { get; }
    ISelfParameterSyntax ISelfParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }

    public static IMethodSelfParameterNode Create(
        IMethodSelfParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
        => new MethodSelfParameterNode(syntax, constraint);
}

// [Closed(typeof(FieldParameterNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldParameterNode : IConstructorOrInitializerParameterNode
{
    new IFieldParameterSyntax Syntax { get; }
    IConstructorOrInitializerParameterSyntax IConstructorOrInitializerParameterNode.Syntax => Syntax;
    IParameterSyntax IParameterNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IFlowState FlowStateAfter
        => FlowStateBefore();
    IFlowState IParameterNode.FlowStateAfter => FlowStateAfter;
    ITypeDefinitionNode ContainingTypeDefinition { get; }
    IFieldDefinitionNode? ReferencedField { get; }
    IdentifierName IConstructorOrInitializerParameterNode.Name
        => Syntax.Name;

    public static IFieldParameterNode Create(IFieldParameterSyntax syntax)
        => new FieldParameterNode(syntax);
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IFixedList<IBodyStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
    new IFlowState FlowStateAfter
        => throw new NotImplementedException();
    IFlowState IBodyNode.FlowStateAfter => FlowStateAfter;

    public static IBlockBodyNode Create(
        IBlockBodySyntax syntax,
        IEnumerable<IBodyStatementNode> statements)
        => new BlockBodyNode(syntax, statements);
}

// [Closed(typeof(ExpressionBodyNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodyNode : IBodyNode
{
    new IExpressionBodySyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IResultStatementNode ResultStatement { get; }
    IMaybeType? ExpectedType { get; }
    new IFlowState FlowStateAfter
        => throw new NotImplementedException();
    IFlowState IBodyNode.FlowStateAfter => FlowStateAfter;
    IMaybePlainType? ExpectedPlainType { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements
        => FixedList.Create(ResultStatement);

    public static IExpressionBodyNode Create(
        IExpressionBodySyntax syntax,
        IResultStatementNode resultStatement)
        => new ExpressionBodyNode(syntax, resultStatement);
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeType NamedType { get; }
    IMaybePlainType NamedPlainType { get; }
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    LexicalScope ContainingLexicalScope { get; }
    TypeName Name
        => Syntax.Name;
    ITypeDeclarationNode? ReferencedDeclaration { get; }
    BareType? NamedBareType { get; }
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    bool IsAttributeType { get; }
    new OrdinaryName Name
        => Syntax.Name;
    TypeName ITypeNameNode.Name => Name;
}

[Closed(
    typeof(IIdentifierTypeNameNode),
    typeof(IBuiltInTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISimpleTypeNameNode : ITypeNameNode
{
    new ISimpleTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName IStandardTypeNameNode.Name => Name;
    TypeName ITypeNameNode.Name => Name;

    public static IIdentifierTypeNameNode Create(IIdentifierTypeNameSyntax syntax)
        => new IdentifierTypeNameNode(syntax);
}

// [Closed(typeof(BuiltInTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeNameNode : ISimpleTypeNameNode
{
    new IBuiltInTypeNameSyntax Syntax { get; }
    ISimpleTypeNameSyntax ISimpleTypeNameNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new BuiltInTypeName Name
        => Syntax.Name;
    TypeName ITypeNameNode.Name => Name;

    public static IBuiltInTypeNameNode Create(IBuiltInTypeNameSyntax syntax)
        => new BuiltInTypeNameNode(syntax);
}

// [Closed(typeof(GenericTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericTypeNameNode : IStandardTypeNameNode
{
    new IGenericTypeNameSyntax Syntax { get; }
    IStandardTypeNameSyntax IStandardTypeNameNode.Syntax => Syntax;
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<ITypeNode> TypeArguments { get; }
    new GenericName Name
        => Syntax.Name;
    OrdinaryName IStandardTypeNameNode.Name => Name;
    TypeName ITypeNameNode.Name => Name;

    public static IGenericTypeNameNode Create(
        IGenericTypeNameSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
        => new GenericTypeNameNode(syntax, typeArguments);
}

// [Closed(typeof(QualifiedTypeNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameNode : ITypeNameNode
{
    new IQualifiedTypeNameSyntax Syntax { get; }
    ITypeNameSyntax ITypeNameNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameNode Context { get; }
    IStandardTypeNameNode QualifiedName { get; }
    IMaybePlainType ITypeNode.NamedPlainType
        => throw new NotImplementedException();
    BareType? ITypeNameNode.NamedBareType
        => throw new NotImplementedException();

    public static IQualifiedTypeNameNode Create(
        IQualifiedTypeNameSyntax syntax,
        ITypeNameNode context,
        IStandardTypeNameNode qualifiedName)
        => new QualifiedTypeNameNode(syntax, context, qualifiedName);
}

// [Closed(typeof(OptionalTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalTypeNode : ITypeNode
{
    new IOptionalTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNode Referent { get; }

    public static IOptionalTypeNode Create(
        IOptionalTypeSyntax syntax,
        ITypeNode referent)
        => new OptionalTypeNode(syntax, referent);
}

// [Closed(typeof(CapabilityTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityTypeNode : ITypeNode
{
    new ICapabilityTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }
    ITypeNode Referent { get; }

    public static ICapabilityTypeNode Create(
        ICapabilityTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
        => new CapabilityTypeNode(syntax, capability, referent);
}

// [Closed(typeof(FunctionTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionTypeNode : ITypeNode
{
    new IFunctionTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<IParameterTypeNode> Parameters { get; }
    ITypeNode Return { get; }

    public static IFunctionTypeNode Create(
        IFunctionTypeSyntax syntax,
        IEnumerable<IParameterTypeNode> parameters,
        ITypeNode @return)
        => new FunctionTypeNode(syntax, parameters, @return);
}

// [Closed(typeof(ParameterTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterTypeNode : ICodeNode
{
    new IParameterTypeSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNode Referent { get; }
    bool IsLent
        => Syntax.IsLent;
    IMaybeParameterType Parameter { get; }

    public static IParameterTypeNode Create(
        IParameterTypeSyntax syntax,
        ITypeNode referent)
        => new ParameterTypeNode(syntax, referent);
}

[Closed(
    typeof(ICapabilityViewpointTypeNode),
    typeof(ISelfViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IViewpointTypeNode : ITypeNode
{
    new IViewpointTypeSyntax Syntax { get; }
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ICapabilityNode Capability { get; }

    public static ICapabilityViewpointTypeNode Create(
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
        => new CapabilityViewpointTypeNode(syntax, capability, referent);
}

// [Closed(typeof(SelfViewpointTypeNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfViewpointTypeNode : IViewpointTypeNode
{
    new ISelfViewpointTypeSyntax Syntax { get; }
    IViewpointTypeSyntax IViewpointTypeNode.Syntax => Syntax;
    ITypeSyntax ITypeNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeType? MethodSelfType { get; }

    public static ISelfViewpointTypeNode Create(
        ISelfViewpointTypeSyntax syntax,
        ITypeNode referent)
        => new SelfViewpointTypeNode(syntax, referent);
}

[Closed(
    typeof(IElseClauseNode),
    typeof(IDataFlowNode),
    typeof(IStatementNode),
    typeof(IPatternNode),
    typeof(IExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IControlFlowNode : IChildNode
{
    IEntryNode ControlFlowEntry();
    ControlFlowSet ControlFlowNext { get; }
    ControlFlowSet ControlFlowPrevious { get; }
    ControlFlowSet ControlFlowFollowing();
}

// [Closed(typeof(EntryNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IEntryNode : IDataFlowNode
{
    new IEntryNode ControlFlowEntry()
        => this;
    IEntryNode IControlFlowNode.ControlFlowEntry() => ControlFlowEntry();
    new ICodeSyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
    FixedDictionary<IVariableBindingNode, int> VariableBindingsMap();
    new BindingFlags<IVariableBindingNode> DefinitelyAssigned { get; }
    BindingFlags<IVariableBindingNode> IDataFlowNode.DefinitelyAssigned => DefinitelyAssigned;
    new BindingFlags<IVariableBindingNode> DefinitelyUnassigned { get; }
    BindingFlags<IVariableBindingNode> IDataFlowNode.DefinitelyUnassigned => DefinitelyUnassigned;
    IFixedSet<IDataFlowNode> IDataFlowNode.DataFlowPrevious
        => [];

    public static IEntryNode Create()
        => new EntryNode();
}

// [Closed(typeof(ExitNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExitNode : IDataFlowNode
{
    new ICodeSyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ControlFlowSet IControlFlowNode.ControlFlowNext
        => ControlFlowSet.Empty;

    public static IExitNode Create()
        => new ExitNode();
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
public partial interface IStatementNode : ICodeNode, IControlFlowNode
{
    new IStatementSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    LexicalScope ContainingLexicalScope();
    LexicalScope LexicalScope()
        => ContainingLexicalScope();
    ValueId? ResultValueId { get; }
    IMaybeType? ResultType
        => null;
    IFlowState FlowStateAfter { get; }
    IMaybePlainType? ResultPlainType
        => null;
}

// [Closed(typeof(ResultStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IResultStatementNode : IStatementNode, IBlockOrResultNode
{
    new IResultStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    bool ImplicitRecoveryAllowed();
    bool ShouldPrepareToReturn();
    IMaybeType? ExpectedType { get; }
    new IFlowState FlowStateAfter
        => Expression?.FlowStateAfter ?? IFlowState.Empty;
    IFlowState IStatementNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    IMaybePlainType? ExpectedPlainType { get; }
    ValueId? IStatementNode.ResultValueId
        => ValueId;
    ValueId IElseClauseNode.ValueId
        => Expression?.ValueId ?? default;
    IMaybeType? IStatementNode.ResultType
        => Type;
    IMaybePlainType? IStatementNode.ResultPlainType
        => PlainType;

    public static IResultStatementNode Create(
        IResultStatementSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new ResultStatementNode(syntax, expression);
}

[Closed(
    typeof(IVariableDeclarationStatementNode),
    typeof(IExpressionStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyStatementNode : IStatementNode
{
    new IBodyStatementSyntax Syntax { get; }
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(VariableDeclarationStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableDeclarationStatementNode : IBodyStatementNode, IVariableBindingNode
{
    new IVariableDeclarationStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ValueIdScope ValueIdScope { get; }
    IFlowState FlowStateBefore();
    ValueId? IStatementNode.ResultValueId
        => null;
    bool IBindingNode.IsLentBinding
        => false;
    bool INamedBindingNode.IsMutableBinding
        => Syntax.IsMutableBinding;
    IdentifierName INamedBindingDeclarationNode.Name
        => Syntax.Name;

    public static IVariableDeclarationStatementNode Create(
        IVariableDeclarationStatementSyntax syntax,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer)
        => new VariableDeclarationStatementNode(syntax, capability, type, initializer);
}

// [Closed(typeof(ExpressionStatementNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionStatementNode : IBodyStatementNode
{
    new IExpressionStatementSyntax Syntax { get; }
    IBodyStatementSyntax IBodyStatementNode.Syntax => Syntax;
    IStatementSyntax IStatementNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    ValueId? IStatementNode.ResultValueId
        => null;

    public static IExpressionStatementNode Create(
        IExpressionStatementSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new ExpressionStatementNode(syntax, expression);
}

[Closed(
    typeof(IBindingContextPatternNode),
    typeof(IOptionalOrBindingPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternNode : ICodeNode, IControlFlowNode
{
    new IPatternSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ConditionalLexicalScope FlowLexicalScope();
    ValueId? MatchReferentValueId { get; }
    IFlowState FlowStateAfter { get; }
    IMaybeNonVoidPlainType ContextBindingPlainType();
    IMaybeNonVoidType ContextBindingType();
}

// [Closed(typeof(BindingContextPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingContextPatternNode : IPatternNode
{
    new IBindingContextPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IPatternNode Pattern { get; }
    ITypeNode? Type { get; }
    new IFlowState FlowStateAfter
        => Pattern.FlowStateAfter;
    IFlowState IPatternNode.FlowStateAfter => FlowStateAfter;
    bool IsMutableBinding
        => Syntax.IsMutableBinding;
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => Pattern.FlowLexicalScope();

    public static IBindingContextPatternNode Create(
        IBindingContextPatternSyntax syntax,
        IPatternNode pattern,
        ITypeNode? type)
        => new BindingContextPatternNode(syntax, pattern, type);
}

[Closed(
    typeof(IBindingPatternNode),
    typeof(IOptionalPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalOrBindingPatternNode : IPatternNode
{
    new IOptionalOrBindingPatternSyntax Syntax { get; }
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(BindingPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingPatternNode : IOptionalOrBindingPatternNode, IVariableBindingNode
{
    new IBindingPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    ValueIdScope ValueIdScope { get; }
    IFlowState FlowStateBefore();
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => LexicalScopingAspect.BindingPattern_FlowLexicalScope(this);
    bool IBindingNode.IsLentBinding
        => false;
    bool INamedBindingNode.IsMutableBinding
        => Syntax.IsMutableBinding;
    IdentifierName INamedBindingDeclarationNode.Name
        => Syntax.Name;

    public static IBindingPatternNode Create(IBindingPatternSyntax syntax)
        => new BindingPatternNode(syntax);
}

// [Closed(typeof(OptionalPatternNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalPatternNode : IOptionalOrBindingPatternNode
{
    new IOptionalPatternSyntax Syntax { get; }
    IOptionalOrBindingPatternSyntax IOptionalOrBindingPatternNode.Syntax => Syntax;
    IPatternSyntax IPatternNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IOptionalOrBindingPatternNode Pattern { get; }
    new IFlowState FlowStateAfter
        => Pattern.FlowStateAfter;
    IFlowState IPatternNode.FlowStateAfter => FlowStateAfter;
    ConditionalLexicalScope IPatternNode.FlowLexicalScope()
        => Pattern.FlowLexicalScope();

    public static IOptionalPatternNode Create(
        IOptionalPatternSyntax syntax,
        IOptionalOrBindingPatternNode pattern)
        => new OptionalPatternNode(syntax, pattern);
}

[Closed(
    typeof(IExpressionNode),
    typeof(IAmbiguousNameExpressionNode),
    typeof(IAmbiguousMoveExpressionNode),
    typeof(IAmbiguousFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousExpressionNode : ICodeNode
{
    new IExpressionSyntax Syntax { get; }
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ConditionalLexicalScope FlowLexicalScope()
        => LexicalScopingAspect.AmbiguousExpression_FlowLexicalScope(this);
    LexicalScope ContainingLexicalScope();
    ValueIdScope ValueIdScope { get; }
    ValueId ValueId { get; }
}

[Closed(
    typeof(IBlockExpressionNode),
    typeof(IUnsafeExpressionNode),
    typeof(INeverTypedExpressionNode),
    typeof(ILiteralExpressionNode),
    typeof(IAssignmentExpressionNode),
    typeof(IBinaryOperatorExpressionNode),
    typeof(IUnaryOperatorExpressionNode),
    typeof(IConversionExpressionNode),
    typeof(IImplicitConversionExpressionNode),
    typeof(IPatternMatchExpressionNode),
    typeof(IIfExpressionNode),
    typeof(ILoopExpressionNode),
    typeof(IWhileExpressionNode),
    typeof(IForeachExpressionNode),
    typeof(IInvocationExpressionNode),
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
    IMaybeType? ExpectedType { get; }
    bool ImplicitRecoveryAllowed();
    bool ShouldPrepareToReturn();
    IMaybeType Type { get; }
    IFlowState FlowStateAfter { get; }
    IMaybePlainType? ExpectedPlainType { get; }
    IMaybePlainType PlainType { get; }
}

// [Closed(typeof(BlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockExpressionNode : IExpressionNode, IBlockOrResultNode, IBodyOrBlockNode
{
    new IBlockExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new IFixedList<IStatementNode> Statements { get; }
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => Statements;
    IFlowState FlowStateBefore();
    new IMaybeType Type { get; }
    IMaybeType IExpressionNode.Type => Type;
    IMaybeType IBlockOrResultNode.Type => Type;
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    new IMaybePlainType PlainType { get; }
    IMaybePlainType IExpressionNode.PlainType => PlainType;
    IMaybePlainType IBlockOrResultNode.PlainType => PlainType;
    new LexicalScope ContainingLexicalScope();
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope();
    LexicalScope IBodyOrBlockNode.ContainingLexicalScope() => ContainingLexicalScope();
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;

    public static IBlockExpressionNode Create(
        IBlockExpressionSyntax syntax,
        IEnumerable<IStatementNode> statements)
        => new BlockExpressionNode(syntax, statements);
}

// [Closed(typeof(NewObjectExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INewObjectExpressionNode : IInvocationExpressionNode
{
    new INewObjectExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ITypeNameNode ConstructingType { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    PackageNameScope PackageNameScope();
    IFlowState FlowStateBefore();
    ContextualizedCall? ContextualizedCall { get; }
    IMaybeNonVoidPlainType ConstructingPlainType { get; }
    IFixedSet<IConstructorDeclarationNode> CompatibleConstructors { get; }
    IConstructorDeclarationNode? ReferencedConstructor { get; }
    IdentifierName? ConstructorName
        => Syntax.ConstructorName;
    IFixedSet<IConstructorDeclarationNode> ReferencedConstructors { get; }
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments;
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments;

    public static INewObjectExpressionNode Create(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode constructingType,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new NewObjectExpressionNode(syntax, constructingType, arguments);
}

// [Closed(typeof(UnsafeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnsafeExpressionNode : IExpressionNode
{
    new IUnsafeExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => TempExpression.FlowLexicalScope();

    public static IUnsafeExpressionNode Create(
        IUnsafeExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new UnsafeExpressionNode(syntax, expression);
}

[Closed(
    typeof(IBreakExpressionNode),
    typeof(INextExpressionNode),
    typeof(IReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INeverTypedExpressionNode : IExpressionNode
{
    new NeverType Type
        => AzothType.Never;
    IMaybeType IExpressionNode.Type => Type;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Never;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new CapabilityType Type { get; }
    IMaybeType IExpressionNode.Type => Type;
    bool Value
        => Syntax.Value;

    public static IBoolLiteralExpressionNode Create(IBoolLiteralExpressionSyntax syntax)
        => new BoolLiteralExpressionNode(syntax);
}

// [Closed(typeof(IntegerLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntegerLiteralExpressionNode : ILiteralExpressionNode
{
    new IIntegerLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new CapabilityType Type { get; }
    IMaybeType IExpressionNode.Type => Type;
    BigInteger Value
        => Syntax.Value;

    public static IIntegerLiteralExpressionNode Create(IIntegerLiteralExpressionSyntax syntax)
        => new IntegerLiteralExpressionNode(syntax);
}

// [Closed(typeof(NoneLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INoneLiteralExpressionNode : ILiteralExpressionNode
{
    new INoneLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new OptionalType Type { get; }
    IMaybeType IExpressionNode.Type => Type;

    public static INoneLiteralExpressionNode Create(INoneLiteralExpressionSyntax syntax)
        => new NoneLiteralExpressionNode(syntax);
}

// [Closed(typeof(StringLiteralExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStringLiteralExpressionNode : ILiteralExpressionNode
{
    new IStringLiteralExpressionSyntax Syntax { get; }
    ILiteralExpressionSyntax ILiteralExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    string Value
        => Syntax.Value;

    public static IStringLiteralExpressionNode Create(IStringLiteralExpressionSyntax syntax)
        => new StringLiteralExpressionNode(syntax);
}

// [Closed(typeof(AssignmentExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignmentExpressionNode : IExpressionNode, IDataFlowNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempLeftOperand { get; }
    IExpressionNode? LeftOperand { get; }
    IAmbiguousExpressionNode CurrentLeftOperand { get; }
    IAmbiguousExpressionNode TempRightOperand { get; }
    IExpressionNode? RightOperand { get; }
    IAmbiguousExpressionNode CurrentRightOperand { get; }
    AssignmentOperator Operator
        => Syntax.Operator;
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.AssignmentExpression_FlowLexicalScope(this);

    public static IAssignmentExpressionNode Create(
        IAssignmentExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
        => new AssignmentExpressionNode(syntax, leftOperand, rightOperand);
}

// [Closed(typeof(BinaryOperatorExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBinaryOperatorExpressionNode : IExpressionNode
{
    new IBinaryOperatorExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempLeftOperand { get; }
    IExpressionNode? LeftOperand { get; }
    IAmbiguousExpressionNode CurrentLeftOperand { get; }
    IAmbiguousExpressionNode TempRightOperand { get; }
    IExpressionNode? RightOperand { get; }
    IAmbiguousExpressionNode CurrentRightOperand { get; }
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    BinaryOperator Operator
        => Syntax.Operator;
    PlainType? NumericOperatorCommonPlainType { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.BinaryOperatorExpression_FlowLexicalScope(this);

    public static IBinaryOperatorExpressionNode Create(
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
        => new BinaryOperatorExpressionNode(syntax, leftOperand, rightOperand);
}

// [Closed(typeof(UnaryOperatorExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnaryOperatorExpressionNode : IExpressionNode
{
    new IUnaryOperatorExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempOperand { get; }
    IExpressionNode? Operand { get; }
    IAmbiguousExpressionNode CurrentOperand { get; }
    UnaryOperator Operator
        => Syntax.Operator;
    UnaryOperatorFixity Fixity
        => Syntax.Fixity;
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => LexicalScopingAspect.UnaryOperatorExpression_FlowLexicalScope(this);

    public static IUnaryOperatorExpressionNode Create(
        IUnaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode operand)
        => new UnaryOperatorExpressionNode(syntax, operand);
}

// [Closed(typeof(ConversionExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConversionExpressionNode : IExpressionNode
{
    new IConversionExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempReferent { get; }
    IExpressionNode? Referent { get; }
    IAmbiguousExpressionNode CurrentReferent { get; }
    ITypeNode ConvertToType { get; }
    ConversionOperator Operator
        => Syntax.Operator;
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => TempReferent.FlowLexicalScope();

    public static IConversionExpressionNode Create(
        IConversionExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        ITypeNode convertToType)
        => new ConversionExpressionNode(syntax, referent, convertToType);
}

// [Closed(typeof(ImplicitConversionExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitConversionExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    new ConstructedPlainType PlainType { get; }
    IMaybePlainType IExpressionNode.PlainType => PlainType;
    new IExpressionSyntax Syntax
        => Referent.Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => Referent.FlowLexicalScope();

    public static IImplicitConversionExpressionNode Create(
        IExpressionNode referent,
        ConstructedPlainType plainType)
        => new ImplicitConversionExpressionNode(referent, plainType);
}

// [Closed(typeof(PatternMatchExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternMatchExpressionNode : IExpressionNode
{
    new IPatternMatchExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempReferent { get; }
    IExpressionNode? Referent { get; }
    IAmbiguousExpressionNode CurrentReferent { get; }
    IPatternNode Pattern { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => Pattern.FlowLexicalScope();
    IMaybeType IExpressionNode.Type
        => AzothType.Bool;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Bool;

    public static IPatternMatchExpressionNode Create(
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
        => new PatternMatchExpressionNode(syntax, referent, pattern);
}

// [Closed(typeof(IfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIfExpressionNode : IExpressionNode, IElseClauseNode
{
    new IIfExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempCondition { get; }
    IExpressionNode? Condition { get; }
    IAmbiguousExpressionNode CurrentCondition { get; }
    IBlockOrResultNode ThenBlock { get; }
    IBlockOrResultNode CurrentThenBlock { get; }
    IElseClauseNode? ElseClause { get; }
    IElseClauseNode? CurrentElseClause { get; }
    new IFlowState FlowStateAfter { get; }
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IFlowState IElseClauseNode.FlowStateAfter => FlowStateAfter;
    new ValueId ValueId { get; }
    ValueId IAmbiguousExpressionNode.ValueId => ValueId;
    ValueId IElseClauseNode.ValueId => ValueId;

    public static IIfExpressionNode Create(
        IIfExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause)
        => new IfExpressionNode(syntax, condition, thenBlock, elseClause);
}

// [Closed(typeof(LoopExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILoopExpressionNode : IExpressionNode
{
    new ILoopExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
    IBlockExpressionNode CurrentBlock { get; }

    public static ILoopExpressionNode Create(
        ILoopExpressionSyntax syntax,
        IBlockExpressionNode block)
        => new LoopExpressionNode(syntax, block);
}

// [Closed(typeof(WhileExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IWhileExpressionNode : IExpressionNode
{
    new IWhileExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempCondition { get; }
    IExpressionNode? Condition { get; }
    IAmbiguousExpressionNode CurrentCondition { get; }
    IBlockExpressionNode Block { get; }
    IBlockExpressionNode CurrentBlock { get; }

    public static IWhileExpressionNode Create(
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
        => new WhileExpressionNode(syntax, condition, block);
}

// [Closed(typeof(ForeachExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IForeachExpressionNode : IExpressionNode, IVariableBindingNode
{
    new IForeachExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ILocalBindingSyntax ILocalBindingNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempInExpression { get; }
    IExpressionNode? InExpression { get; }
    IAmbiguousExpressionNode CurrentInExpression { get; }
    ITypeNode? DeclaredType { get; }
    IBlockExpressionNode Block { get; }
    IBlockExpressionNode CurrentBlock { get; }
    PackageNameScope PackageNameScope();
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    LexicalScope INamedBindingNode.ContainingLexicalScope => ContainingLexicalScope;
    LexicalScope LexicalScope { get; }
    IdentifierName VariableName
        => Syntax.VariableName;
    IMaybeNonVoidType IteratorType { get; }
    IMaybeNonVoidType IteratedType { get; }
    IFlowState FlowStateBeforeBlock { get; }
    ITypeDeclarationNode? ReferencedIterableDeclaration { get; }
    IOrdinaryMethodDeclarationNode? ReferencedIterateMethod { get; }
    IMaybeNonVoidPlainType IteratorPlainType { get; }
    ITypeDeclarationNode? ReferencedIteratorDeclaration { get; }
    IOrdinaryMethodDeclarationNode? ReferencedNextMethod { get; }
    IMaybeNonVoidPlainType IteratedPlainType { get; }
    bool IBindingNode.IsLentBinding
        => false;
    bool INamedBindingNode.IsMutableBinding
        => Syntax.IsMutableBinding;
    IdentifierName INamedBindingDeclarationNode.Name
        => VariableName;

    public static IForeachExpressionNode Create(
        IForeachExpressionSyntax syntax,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? declaredType,
        IBlockExpressionNode block)
        => new ForeachExpressionNode(syntax, inExpression, declaredType, block);
}

// [Closed(typeof(BreakExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBreakExpressionNode : INeverTypedExpressionNode
{
    new IBreakExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode? TempValue { get; }
    IExpressionNode? Value { get; }
    IAmbiguousExpressionNode? CurrentValue { get; }

    public static IBreakExpressionNode Create(
        IBreakExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
        => new BreakExpressionNode(syntax, value);
}

// [Closed(typeof(NextExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INextExpressionNode : INeverTypedExpressionNode
{
    new INextExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;

    public static INextExpressionNode Create(INextExpressionSyntax syntax)
        => new NextExpressionNode(syntax);
}

// [Closed(typeof(ReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnExpressionNode : INeverTypedExpressionNode
{
    new IReturnExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode? TempValue { get; }
    IExpressionNode? Value { get; }
    IAmbiguousExpressionNode? CurrentValue { get; }
    IMaybeType? ExpectedReturnType { get; }
    IExitNode ControlFlowExit();
    IMaybePlainType? ExpectedReturnPlainType { get; }

    public static IReturnExpressionNode Create(
        IReturnExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
        => new ReturnExpressionNode(syntax, value);
}

[Closed(
    typeof(INewObjectExpressionNode),
    typeof(IUnknownInvocationExpressionNode),
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

// [Closed(typeof(UnknownInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFlowState FlowStateBefore();
    IMaybeType IExpressionNode.Type
        => AzothType.Unknown;
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments;
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IUnknownInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new UnknownInvocationExpressionNode(syntax, expression, arguments);
}

// [Closed(typeof(FunctionInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFunctionNameNode Function { get; }
    IFunctionNameNode CurrentFunction { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFlowState FlowStateBefore();
    ContextualizedCall? ContextualizedCall { get; }
    CallCandidate<IFunctionInvocableDeclarationNode>? SelectedCallCandidate
        => Function.SelectedCallCandidate;
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments;
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments;

    public static IFunctionInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IFunctionNameNode function,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new FunctionInvocationExpressionNode(syntax, function, arguments);
}

// [Closed(typeof(MethodInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMethodNameNode Method { get; }
    IMethodNameNode CurrentMethod { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    ContextualizedCall? ContextualizedCall { get; }
    CallCandidate<IOrdinaryMethodDeclarationNode>? SelectedCallCandidate
        => Method.SelectedCallCandidate;
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments.Prepend(Method.Context);
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments.Prepend(Method.Context);

    public static IMethodInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IMethodNameNode method,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new MethodInvocationExpressionNode(syntax, method, arguments);
}

// [Closed(typeof(GetterInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterInvocationExpressionNode : IInvocationExpressionNode, INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    OrdinaryName PropertyName { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    ContextualizedCall? ContextualizedCall { get; }
    IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.GetterInvocationExpression_FlowStateAfter(this);
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => [Context];
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => [Context];

    public static IGetterInvocationExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
        => new GetterInvocationExpressionNode(syntax, context, propertyName, referencedPropertyAccessors);
}

// [Closed(typeof(SetterInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterInvocationExpressionNode : IInvocationExpressionNode
{
    new IAssignmentExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    OrdinaryName PropertyName { get; }
    IAmbiguousExpressionNode TempValue { get; }
    IExpressionNode? Value { get; }
    IAmbiguousExpressionNode CurrentValue { get; }
    IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    ContextualizedCall? ContextualizedCall { get; }
    ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => [Context, TempValue];
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => [Context, Value];

    public static ISetterInvocationExpressionNode Create(
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
        => new SetterInvocationExpressionNode(syntax, context, propertyName, value, referencedPropertyAccessors);
}

// [Closed(typeof(FunctionReferenceInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionReferenceInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IExpressionNode Expression { get; }
    IExpressionNode CurrentExpression { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    FunctionType FunctionType { get; }
    FunctionPlainType FunctionPlainType { get; }
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments;
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments;

    public static IFunctionReferenceInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new FunctionReferenceInvocationExpressionNode(syntax, expression, arguments);
}

// [Closed(typeof(InitializerInvocationExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerInvocationExpressionNode : IInvocationExpressionNode
{
    new IInvocationExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IInitializerGroupNameNode InitializerGroup { get; }
    IInitializerGroupNameNode CurrentInitializerGroup { get; }
    IFixedList<IAmbiguousExpressionNode> TempArguments { get; }
    IFixedList<IExpressionNode?> Arguments { get; }
    IFixedList<IAmbiguousExpressionNode> CurrentArguments { get; }
    IFlowState FlowStateBefore();
    ContextualizedCall? ContextualizedCall { get; }
    IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations { get; }
    IInitializerDeclarationNode? ReferencedDeclaration { get; }
    IEnumerable<IAmbiguousExpressionNode> IInvocationExpressionNode.TempAllArguments
        => TempArguments;
    IEnumerable<IExpressionNode?> IInvocationExpressionNode.AllArguments
        => Arguments;

    public static IInitializerInvocationExpressionNode Create(
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
        => new InitializerInvocationExpressionNode(syntax, initializerGroup, arguments);
}

[Closed(
    typeof(IAmbiguousNameNode),
    typeof(INameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousNameExpressionNode : IAmbiguousExpressionNode
{
    new INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

[Closed(
    typeof(IUnresolvedSimpleNameNode),
    typeof(IStandardNameExpressionNode))]
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    OrdinaryName Name
        => Syntax.Name;
    IFixedList<IDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(IdentifierNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierNameExpressionNode : IStandardNameExpressionNode, IUnresolvedSimpleNameNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName IStandardNameExpressionNode.Name => Name;

    public static IIdentifierNameExpressionNode Create(IIdentifierNameExpressionSyntax syntax)
        => new IdentifierNameExpressionNode(syntax);
}

// [Closed(typeof(GenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericNameExpressionNode : IStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IStandardNameExpressionNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IFixedList<ITypeNode> TypeArguments { get; }
    new GenericName Name
        => Syntax.Name;
    OrdinaryName IStandardNameExpressionNode.Name => Name;

    public static IGenericNameExpressionNode Create(
        IGenericNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
        => new GenericNameExpressionNode(syntax, typeArguments);
}

[Closed(
    typeof(IGetterInvocationExpressionNode),
    typeof(IUnknownNameExpressionNode),
    typeof(ILocalBindingNameExpressionNode),
    typeof(ISimpleNameExpressionNode),
    typeof(INamespaceNameNode),
    typeof(IFunctionGroupNameNode),
    typeof(IFunctionNameNode),
    typeof(IMethodGroupNameNode),
    typeof(IMethodNameNode),
    typeof(IFieldAccessExpressionNode),
    typeof(ITypeNameExpressionNode),
    typeof(IInitializerGroupNameNode),
    typeof(IBuiltInTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INameExpressionNode : IExpressionNode, IAmbiguousNameExpressionNode
{
    IFlowState FlowStateBefore();
    new IFlowState FlowStateAfter
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(FlowStateAfter)} not implemented.");
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
}

[Closed(
    typeof(IUnresolvedMemberAccessExpressionNode),
    typeof(IUnknownStandardNameExpressionNode),
    typeof(IAmbiguousMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownNameExpressionNode : INameExpressionNode
{
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;
}

// [Closed(typeof(UnresolvedMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnresolvedMemberAccessExpressionNode : IUnknownNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempContext { get; }
    IExpressionNode? Context { get; }
    IAmbiguousExpressionNode CurrentContext { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    PackageNameScope PackageNameScope();
    OrdinaryName MemberName
        => Syntax.MemberName;
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.UnresolvedMemberAccessExpression_FlowStateAfter(this);

    public static IUnresolvedMemberAccessExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        IAmbiguousExpressionNode context,
        IEnumerable<ITypeNode> typeArguments)
        => new UnresolvedMemberAccessExpressionNode(syntax, context, typeArguments);
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
    typeof(IUnqualifiedNamespaceNameNode),
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
    IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;
}

// [Closed(typeof(UnqualifiedNamespaceNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnqualifiedNamespaceNameNode : INamespaceNameNode, ISimpleNameExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IdentifierName Name
        => Syntax.Name;

    public static IUnqualifiedNamespaceNameNode Create(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
        => new UnqualifiedNamespaceNameNode(syntax, referencedDeclarations);
}

// [Closed(typeof(QualifiedNamespaceNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedNamespaceNameNode : INamespaceNameNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
    INamespaceNameNode CurrentContext { get; }
    IdentifierName Name
        => (IdentifierName)Syntax.MemberName;

    public static IQualifiedNamespaceNameNode Create(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
        => new QualifiedNamespaceNameNode(syntax, context, referencedDeclarations);
}

// [Closed(typeof(FunctionGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionGroupNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    INameExpressionNode? CurrentContext { get; }
    OrdinaryName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionInvocableDeclarationNode> ReferencedDeclarations { get; }
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CallCandidates { get; }
    IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CompatibleCallCandidates { get; }
    CallCandidate<IFunctionInvocableDeclarationNode>? SelectedCallCandidate { get; }
    IFunctionInvocableDeclarationNode? ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => FlowStateBefore();
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IFunctionGroupNameNode Create(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations)
        => new FunctionGroupNameNode(syntax, context, functionName, typeArguments, referencedDeclarations);
}

// [Closed(typeof(FunctionNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionNameNode : INameExpressionNode
{
    INameExpressionNode? Context { get; }
    INameExpressionNode? CurrentContext { get; }
    OrdinaryName FunctionName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IFunctionInvocableDeclarationNode> ReferencedDeclarations { get; }
    IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CallCandidates { get; }
    IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CompatibleCallCandidates { get; }
    CallCandidate<IFunctionInvocableDeclarationNode>? SelectedCallCandidate { get; }
    IFunctionInvocableDeclarationNode? ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.FunctionName_FlowStateAfter(this);

    public static IFunctionNameNode Create(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations,
        IEnumerable<CallCandidate<IFunctionInvocableDeclarationNode>> callCandidates,
        IEnumerable<CallCandidate<IFunctionInvocableDeclarationNode>> compatibleCallCandidates,
        CallCandidate<IFunctionInvocableDeclarationNode>? selectedCallCandidate,
        IFunctionInvocableDeclarationNode? referencedDeclaration)
        => new FunctionNameNode(syntax, context, functionName, typeArguments, referencedDeclarations, callCandidates, compatibleCallCandidates, selectedCallCandidate, referencedDeclaration);
}

// [Closed(typeof(MethodGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodGroupNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    OrdinaryName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IOrdinaryMethodDeclarationNode> ReferencedDeclarations { get; }
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CallCandidates { get; }
    IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CompatibleCallCandidates { get; }
    CallCandidate<IOrdinaryMethodDeclarationNode>? SelectedCallCandidate { get; }
    IOrdinaryMethodDeclarationNode? ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => Context.FlowStateAfter;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IMethodGroupNameNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IOrdinaryMethodDeclarationNode> referencedDeclarations)
        => new MethodGroupNameNode(syntax, context, methodName, typeArguments, referencedDeclarations);
}

// [Closed(typeof(MethodNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodNameNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    OrdinaryName MethodName { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IOrdinaryMethodDeclarationNode> ReferencedDeclarations { get; }
    IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CallCandidates { get; }
    IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CompatibleCallCandidates { get; }
    CallCandidate<IOrdinaryMethodDeclarationNode>? SelectedCallCandidate { get; }
    IOrdinaryMethodDeclarationNode? ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => Context.FlowStateAfter;

    public static IMethodNameNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IOrdinaryMethodDeclarationNode> referencedDeclarations,
        IEnumerable<CallCandidate<IOrdinaryMethodDeclarationNode>> callCandidates,
        IEnumerable<CallCandidate<IOrdinaryMethodDeclarationNode>> compatibleCallCandidates,
        CallCandidate<IOrdinaryMethodDeclarationNode>? selectedCallCandidate,
        IOrdinaryMethodDeclarationNode? referencedDeclaration)
        => new MethodNameNode(syntax, context, methodName, typeArguments, referencedDeclarations, callCandidates, compatibleCallCandidates, selectedCallCandidate, referencedDeclaration);
}

// [Closed(typeof(FieldAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldAccessExpressionNode : INameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    IdentifierName FieldName { get; }
    IFieldDeclarationNode ReferencedDeclaration { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.FieldAccessExpression_FlowStateAfter(this);

    public static IFieldAccessExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IdentifierName fieldName,
        IFieldDeclarationNode referencedDeclaration)
        => new FieldAccessExpressionNode(syntax, context, fieldName, referencedDeclaration);
}

// [Closed(typeof(VariableNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableNameExpressionNode : ILocalBindingNameExpressionNode, ISimpleNameExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    new ILocalBindingNode ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    IdentifierName Name
        => Syntax.Name;
    IFixedSet<IDataFlowNode> DataFlowPrevious { get; }
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.VariableNameExpression_FlowStateAfter(this);

    public static IVariableNameExpressionNode Create(
        IIdentifierNameExpressionSyntax syntax,
        ILocalBindingNode referencedDefinition)
        => new VariableNameExpressionNode(syntax, referencedDefinition);
}

[Closed(
    typeof(IStandardTypeNameExpressionNode),
    typeof(IQualifiedTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNameExpressionNode : INameExpressionNode
{
    IFixedList<ITypeNode> TypeArguments { get; }
    ITypeDeclarationNode ReferencedDeclaration { get; }
    OrdinaryName Name { get; }
    IMaybePlainType NamedPlainType { get; }
    BareType? NamedBareType { get; }
    IMaybeType IExpressionNode.Type
        => AzothType.Unknown;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;
}

// [Closed(typeof(StandardTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    OrdinaryName ITypeNameExpressionNode.Name
        => Syntax.Name;

    public static IStandardTypeNameExpressionNode Create(
        IStandardNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
        => new StandardTypeNameExpressionNode(syntax, typeArguments, referencedDeclaration);
}

// [Closed(typeof(QualifiedTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameExpressionNode : ITypeNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    INamespaceNameNode Context { get; }
    INamespaceNameNode CurrentContext { get; }
    OrdinaryName ITypeNameExpressionNode.Name
        => Syntax.MemberName;

    public static IQualifiedTypeNameExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
        => new QualifiedTypeNameExpressionNode(syntax, context, typeArguments, referencedDeclaration);
}

// [Closed(typeof(InitializerGroupNameNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerGroupNameNode : INameExpressionNode
{
    ITypeNameExpressionNode Context { get; }
    ITypeNameExpressionNode CurrentContext { get; }
    OrdinaryName? InitializerName { get; }
    IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }
    IMaybePlainType InitializingPlainType
        => Context.NamedPlainType;
    IMaybeType IExpressionNode.Type
        => throw new NotImplementedException();
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IInitializerGroupNameNode Create(
        INameExpressionSyntax syntax,
        ITypeNameExpressionNode context,
        OrdinaryName? initializerName,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
        => new InitializerGroupNameNode(syntax, context, initializerName, referencedDeclarations);
}

// [Closed(typeof(BuiltInTypeNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeNameExpressionNode : INameExpressionNode
{
    new IBuiltInTypeNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new LexicalScope ContainingLexicalScope { get; }
    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    BuiltInTypeName Name
        => Syntax.Name;
    ITypeDeclarationNode? ReferencedDeclaration { get; }
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IBuiltInTypeNameExpressionNode Create(IBuiltInTypeNameExpressionSyntax syntax)
        => new BuiltInTypeNameExpressionNode(syntax);
}

[Closed(
    typeof(ISelfExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInstanceExpressionNode : ISimpleNameExpressionNode
{
    new IInstanceExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
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
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    IExecutableDefinitionNode ContainingDeclaration { get; }
    bool IsImplicit
        => Syntax.IsImplicit;
    new ISelfParameterNode? ReferencedDefinition { get; }
    IBindingNode? ILocalBindingNameExpressionNode.ReferencedDefinition => ReferencedDefinition;
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.SelfExpression_FlowStateAfter(this);

    public static ISelfExpressionNode Create(ISelfExpressionSyntax syntax)
        => new SelfExpressionNode(syntax);
}

// [Closed(typeof(MissingNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMissingNameExpressionNode : ISimpleNameExpressionNode
{
    new IMissingNameSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    new UnknownType Type
        => AzothType.Unknown;
    IMaybeType IExpressionNode.Type => Type;
    IMaybePlainType IExpressionNode.PlainType
        => AzothPlainType.Unknown;

    public static IMissingNameExpressionNode Create(IMissingNameSyntax syntax)
        => new MissingNameExpressionNode(syntax);
}

[Closed(
    typeof(IUnknownIdentifierNameExpressionNode),
    typeof(IUnknownGenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownStandardNameExpressionNode : IUnknownNameExpressionNode
{
    new IStandardNameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    OrdinaryName Name { get; }
    IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }
}

// [Closed(typeof(UnknownIdentifierNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownIdentifierNameExpressionNode : IUnknownStandardNameExpressionNode, ISimpleNameExpressionNode
{
    new IIdentifierNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    ISimpleNameSyntax IUnresolvedSimpleNameNode.Syntax => Syntax;
    new IdentifierName Name
        => Syntax.Name;
    OrdinaryName IUnknownStandardNameExpressionNode.Name => Name;

    public static IUnknownIdentifierNameExpressionNode Create(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<IDeclarationNode> referencedDeclarations)
        => new UnknownIdentifierNameExpressionNode(syntax, referencedDeclarations);
}

// [Closed(typeof(UnknownGenericNameExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnknownGenericNameExpressionNode : IUnknownStandardNameExpressionNode
{
    new IGenericNameExpressionSyntax Syntax { get; }
    IStandardNameExpressionSyntax IUnknownStandardNameExpressionNode.Syntax => Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    new GenericName Name { get; }
    OrdinaryName IUnknownStandardNameExpressionNode.Name => Name;
    IFixedList<ITypeNode> TypeArguments { get; }

    public static IUnknownGenericNameExpressionNode Create(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
        => new UnknownGenericNameExpressionNode(referencedDeclarations, syntax, name, typeArguments);
}

// [Closed(typeof(AmbiguousMemberAccessExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousMemberAccessExpressionNode : IUnknownNameExpressionNode
{
    new IMemberAccessExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    INameExpressionSyntax IAmbiguousNameExpressionNode.Syntax => Syntax;
    IExpressionNode Context { get; }
    IExpressionNode CurrentContext { get; }
    IFixedList<ITypeNode> TypeArguments { get; }
    IFixedSet<IDeclarationNode> ReferencedMembers { get; }
    OrdinaryName MemberName
        => Syntax.MemberName;
    IFlowState INameExpressionNode.FlowStateAfter
        => ExpressionTypesAspect.AmbiguousMemberAccessExpression_FlowStateAfter(this);

    public static IAmbiguousMemberAccessExpressionNode Create(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IDeclarationNode> referencedMembers)
        => new AmbiguousMemberAccessExpressionNode(syntax, context, typeArguments, referencedMembers);
}

// [Closed(typeof(AmbiguousMoveExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousMoveExpressionNode : IAmbiguousExpressionNode
{
    new IMoveExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousNameExpressionNode TempReferent { get; }
    INameExpressionNode? Referent { get; }
    IAmbiguousNameExpressionNode CurrentReferent { get; }

    public static IAmbiguousMoveExpressionNode Create(
        IMoveExpressionSyntax syntax,
        IAmbiguousNameExpressionNode referent)
        => new AmbiguousMoveExpressionNode(syntax, referent);
}

[Closed(
    typeof(IMoveExpressionNode),
    typeof(IFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IRecoveryExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    bool IsImplicit { get; }
}

[Closed(
    typeof(IMoveVariableExpressionNode),
    typeof(IMoveValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveExpressionNode : IRecoveryExpressionNode
{
    IMaybePlainType IExpressionNode.PlainType
        => Referent.PlainType;
}

// [Closed(typeof(MoveVariableExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveVariableExpressionNode : IMoveExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    new ILocalBindingNameExpressionNode CurrentReferent { get; }
    IExpressionNode IRecoveryExpressionNode.Referent => Referent;
    IExpressionNode IRecoveryExpressionNode.CurrentReferent => CurrentReferent;

    public static IMoveVariableExpressionNode Create(
        IExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isImplicit)
        => new MoveVariableExpressionNode(syntax, referent, isImplicit);
}

// [Closed(typeof(MoveValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveValueExpressionNode : IMoveExpressionNode
{

    public static IMoveValueExpressionNode Create(
        IExpressionSyntax syntax,
        IExpressionNode referent,
        bool isImplicit)
        => new MoveValueExpressionNode(syntax, referent, isImplicit);
}

// [Closed(typeof(ImplicitTempMoveExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitTempMoveExpressionNode : IExpressionNode
{
    IExpressionNode Referent { get; }
    IExpressionNode CurrentReferent { get; }
    IMaybePlainType IExpressionNode.PlainType
        => Referent.PlainType;

    public static IImplicitTempMoveExpressionNode Create(
        IExpressionSyntax syntax,
        IExpressionNode referent)
        => new ImplicitTempMoveExpressionNode(syntax, referent);
}

// [Closed(typeof(AmbiguousFreezeExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAmbiguousFreezeExpressionNode : IAmbiguousExpressionNode
{
    new IFreezeExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousNameExpressionNode TempReferent { get; }
    INameExpressionNode? Referent { get; }
    IAmbiguousNameExpressionNode CurrentReferent { get; }

    public static IAmbiguousFreezeExpressionNode Create(
        IFreezeExpressionSyntax syntax,
        IAmbiguousNameExpressionNode referent)
        => new AmbiguousFreezeExpressionNode(syntax, referent);
}

[Closed(
    typeof(IFreezeVariableExpressionNode),
    typeof(IFreezeValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeExpressionNode : IRecoveryExpressionNode
{
    bool IsTemporary { get; }
    IMaybePlainType IExpressionNode.PlainType
        => Referent.PlainType;
}

// [Closed(typeof(FreezeVariableExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeVariableExpressionNode : IFreezeExpressionNode
{
    new ILocalBindingNameExpressionNode Referent { get; }
    new ILocalBindingNameExpressionNode CurrentReferent { get; }
    IExpressionNode IRecoveryExpressionNode.Referent => Referent;
    IExpressionNode IRecoveryExpressionNode.CurrentReferent => CurrentReferent;

    public static IFreezeVariableExpressionNode Create(
        IExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
        => new FreezeVariableExpressionNode(syntax, referent, isTemporary, isImplicit);
}

// [Closed(typeof(FreezeValueExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeValueExpressionNode : IFreezeExpressionNode
{

    public static IFreezeValueExpressionNode Create(
        IExpressionSyntax syntax,
        IExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
        => new FreezeValueExpressionNode(syntax, referent, isTemporary, isImplicit);
}

// [Closed(typeof(PrepareToReturnExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrepareToReturnExpressionNode : IExpressionNode
{
    IExpressionNode Value { get; }
    IExpressionNode CurrentValue { get; }
    new IExpressionSyntax Syntax
        => Value.Syntax;
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IMaybeType IExpressionNode.Type
        => Value.Type;
    IMaybePlainType IExpressionNode.PlainType
        => Value.PlainType;

    public static IPrepareToReturnExpressionNode Create(IExpressionNode value)
        => new PrepareToReturnExpressionNode(value);
}

// [Closed(typeof(AsyncBlockExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncBlockExpressionNode : IExpressionNode
{
    new IAsyncBlockExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IBlockExpressionNode Block { get; }
    IBlockExpressionNode CurrentBlock { get; }
    new IFlowState FlowStateAfter
        => throw new NotImplementedException();
    IFlowState IExpressionNode.FlowStateAfter => FlowStateAfter;
    IMaybeType IExpressionNode.Type
        => Block.Type;
    IMaybePlainType IExpressionNode.PlainType
        => Block.PlainType;

    public static IAsyncBlockExpressionNode Create(
        IAsyncBlockExpressionSyntax syntax,
        IBlockExpressionNode block)
        => new AsyncBlockExpressionNode(syntax, block);
}

// [Closed(typeof(AsyncStartExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncStartExpressionNode : IExpressionNode
{
    new IAsyncStartExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    bool Scheduled
        => Syntax.Scheduled;

    public static IAsyncStartExpressionNode Create(
        IAsyncStartExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new AsyncStartExpressionNode(syntax, expression);
}

// [Closed(typeof(AwaitExpressionNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAwaitExpressionNode : IExpressionNode
{
    new IAwaitExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    ICodeSyntax ICodeNode.Syntax => Syntax;
    ISyntax? ISemanticNode.Syntax => Syntax;
    IAmbiguousExpressionNode TempExpression { get; }
    IExpressionNode? Expression { get; }
    IAmbiguousExpressionNode CurrentExpression { get; }
    ConditionalLexicalScope IAmbiguousExpressionNode.FlowLexicalScope()
        => TempExpression.FlowLexicalScope();

    public static IAwaitExpressionNode Create(
        IAwaitExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
        => new AwaitExpressionNode(syntax, expression);
}

[Closed(
    typeof(IChildDeclarationNode),
    typeof(ISymbolDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDeclarationNode : ISemanticNode
{
    ISymbolDeclarationNode ContainingDeclaration { get; }
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IBindingDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IPackageFacetChildDeclarationNode),
    typeof(IInvocableDeclarationNode),
    typeof(IChildSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IChildDeclarationNode : IDeclarationNode, IChildNode
{
}

[Closed(
    typeof(IAssociatedMemberDefinitionNode),
    typeof(INamedBindingDeclarationNode),
    typeof(IFunctionInvocableDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeDeclarationNode),
    typeof(IMethodDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedDeclarationNode : IChildDeclarationNode
{
    TypeName Name { get; }
}

[Closed(
    typeof(IExecutableDefinitionNode),
    typeof(IPackageDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IInvocableDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeDeclarationNode),
    typeof(ITypeMemberDeclarationNode),
    typeof(IAssociatedMemberDeclarationNode),
    typeof(IChildSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISymbolDeclarationNode : IDeclarationNode
{
    Symbol? Symbol { get; }
}

[Closed(
    typeof(IBindingNode),
    typeof(INamedBindingDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingDeclarationNode : IChildDeclarationNode
{
}

[Closed(
    typeof(INamedBindingNode),
    typeof(IFieldDeclarationNode))]
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
    IPackageFacetDeclarationNode MainFacet { get; }
    IPackageFacetDeclarationNode TestingFacet { get; }
    new PackageSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
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
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    INamespaceDeclarationNode GlobalNamespace { get; }
}

[Closed(
    typeof(IDefinitionNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetChildDeclarationNode : IChildDeclarationNode
{
    IPackageFacetDeclarationNode Facet { get; }
    OrdinaryName? Name { get; }
}

[Closed(
    typeof(IInvocableDefinitionNode),
    typeof(IFunctionInvocableDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDeclarationNode : ISymbolDeclarationNode, IChildDeclarationNode
{
    IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes { get; }
    IFixedList<IMaybeParameterType> ParameterTypes { get; }
    IMaybePlainType ReturnPlainType { get; }
    IMaybeType ReturnType { get; }
    new InvocableSymbol? Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IConcreteFunctionInvocableDefinitionNode),
    typeof(IFunctionDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionInvocableDeclarationNode : INamedDeclarationNode, IInvocableDeclarationNode
{
    new FunctionSymbol? Symbol { get; }
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    IMaybeFunctionPlainType PlainType { get; }
    IMaybeFunctionType Type { get; }
}

[Closed(
    typeof(INamespaceDefinitionNode),
    typeof(INamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceDeclarationNode : INamespaceMemberDeclarationNode
{
    FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName { get; }
    FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName { get; }
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(OrdinaryName name)
        => MembersByName.GetValueOrDefault(name) ?? [];
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(OrdinaryName name)
        => NestedMembersByName.GetValueOrDefault(name) ?? [];
    new IdentifierName Name
        => Symbol.Name;
    OrdinaryName INamespaceMemberDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
}

[Closed(
    typeof(INamespaceMemberDefinitionNode),
    typeof(INamespaceDeclarationNode),
    typeof(IFunctionDeclarationNode),
    typeof(IUserTypeDeclarationNode),
    typeof(INamespaceMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberDeclarationNode : IPackageFacetChildDeclarationNode, INamedDeclarationNode, ISymbolDeclarationNode
{
    new OrdinaryName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IFunctionDefinitionNode),
    typeof(IFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDeclarationNode : INamespaceMemberDeclarationNode, IFunctionInvocableDeclarationNode
{
    new INamespaceDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
}

[Closed(
    typeof(INonVariableTypeDeclarationNode),
    typeof(IGenericParameterDeclarationNode),
    typeof(IImplicitSelfDeclarationNode),
    typeof(ITypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDeclarationNode : INamedDeclarationNode, ISymbolDeclarationNode
{
    IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(OrdinaryName name);
    IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(OrdinaryName name);
    IFixedSet<BareType> Supertypes { get; }
    ITypeFactory TypeFactory { get; }
    new TypeSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }
}

[Closed(
    typeof(IBuiltInTypeDeclarationNode),
    typeof(IUserTypeDeclarationNode),
    typeof(INonVariableTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INonVariableTypeDeclarationNode : ITypeDeclarationNode
{
    new BareTypeConstructor TypeFactory { get; }
    ITypeFactory ITypeDeclarationNode.TypeFactory => TypeFactory;
    IImplicitSelfDeclarationNode ImplicitSelf { get; }
}

[Closed(
    typeof(IBuiltInTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeDeclarationNode : INonVariableTypeDeclarationNode
{
    new BuiltInTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName { get; }
    FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName { get; }
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName name)
        => InclusiveInstanceMembersByName.GetValueOrDefault(name) ?? [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName name)
        => AssociatedMembersByName.GetValueOrDefault(name) ?? [];
}

[Closed(
    typeof(ITypeDefinitionNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IOrdinaryTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUserTypeDeclarationNode : INamespaceMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INonVariableTypeDeclarationNode
{
    IFixedList<IGenericParameterDeclarationNode> GenericParameters { get; }
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName { get; }
    FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName { get; }
    new OrdinaryTypeSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName name)
        => InclusiveInstanceMembersByName.GetValueOrDefault(name) ?? [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName name)
        => AssociatedMembersByName.GetValueOrDefault(name) ?? [];
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
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    new IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    new IUserTypeDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
    new GenericParameterTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName name)
        => [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName name)
        => [];
}

[Closed(
    typeof(IImplicitSelfDefinitionNode),
    typeof(ISelfSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImplicitSelfDeclarationNode : ITypeDeclarationNode
{
    new INonVariableTypeDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
    new ISyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
    new BuiltInTypeName Name
        => BuiltInTypeName.Self;
    TypeName INamedDeclarationNode.Name => Name;
    new AssociatedTypeConstructor TypeFactory { get; }
    ITypeFactory ITypeDeclarationNode.TypeFactory => TypeFactory;
    new AssociatedTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName name)
        => ContainingDeclaration.InclusiveInstanceMembersNamed(name);
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName name)
        => ContainingDeclaration.AssociatedMembersNamed(name);
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];
}

[Closed(
    typeof(ITypeMemberDefinitionNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(IAlwaysTypeMemberDeclarationNode),
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
    typeof(IAlwaysTypeMemberDefinitionNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAlwaysTypeMemberDeclarationNode : ITypeMemberDeclarationNode
{
    new ITypeDeclarationNode ContainingDeclaration { get; }
    ISymbolDeclarationNode IDeclarationNode.ContainingDeclaration => ContainingDeclaration;
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
    typeof(IOrdinaryMethodDeclarationNode),
    typeof(IPropertyAccessorDeclarationNode),
    typeof(IMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodDeclarationNode : IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode, IInstanceMemberDeclarationNode, IInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode
{
    new IdentifierName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType SelfParameterPlainType { get; }
    IMaybeNonVoidType SelfParameterType { get; }
    new MethodSymbol? Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAbstractMethodDefinitionNode),
    typeof(IOrdinaryMethodDefinitionNode),
    typeof(IOrdinaryMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryMethodDeclarationNode : IMethodDeclarationNode
{
    int Arity { get; }
    IMaybeFunctionPlainType MethodGroupPlainType { get; }
    IMaybeFunctionType MethodGroupType { get; }
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
public partial interface IConstructorDeclarationNode : IAssociatedMemberDeclarationNode, IInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType SelfParameterPlainType { get; }
    IMaybeNonVoidType SelfParameterType { get; }
    new ConstructorSymbol? Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IInitializerDefinitionNode),
    typeof(IInitializerSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerDeclarationNode : IAssociatedMemberDeclarationNode, IInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType SelfParameterPlainType { get; }
    IMaybeNonVoidType SelfParameterType { get; }
    new InitializerSymbol? Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IFieldDefinitionNode),
    typeof(IFieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldDeclarationNode : IClassMemberDeclarationNode, IStructMemberDeclarationNode, IInstanceMemberDeclarationNode, INamedBindingDeclarationNode, IAlwaysTypeMemberDeclarationNode
{
    new IdentifierName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    bool IsMutableBinding { get; }
    IMaybeNonVoidType BindingType { get; }
    new FieldSymbol? Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IAssociatedFunctionDefinitionNode),
    typeof(IAssociatedFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionDeclarationNode : IAssociatedMemberDeclarationNode, IFunctionInvocableDeclarationNode, IAlwaysTypeMemberDeclarationNode
{
    new OrdinaryName Name { get; }
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IPackageSymbolNode),
    typeof(IPackageFacetSymbolNode),
    typeof(INamespaceMemberSymbolNode),
    typeof(ITypeSymbolNode),
    typeof(ITypeMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IChildSymbolNode : ISymbolDeclarationNode, IChildDeclarationNode
{
    ISymbolTree SymbolTree();
    new Symbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    new ISyntax? Syntax
        => null;
    ISyntax? ISemanticNode.Syntax => Syntax;
}

// [Closed(typeof(PackageSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageSymbolNode : IPackageDeclarationNode, IChildSymbolNode
{
    IPackageReferenceNode PackageReference { get; }
    new PackageSymbol Symbol
        => PackageReference.PackageSymbols.PackageSymbol;
    PackageSymbol IPackageDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName Name
        => Symbol.Name;
    IdentifierName IPackageDeclarationNode.Name => Name;
    new IdentifierName AliasOrName
        => PackageReference.AliasOrName;
    IdentifierName? IPackageDeclarationNode.AliasOrName => AliasOrName;
    new IPackageFacetSymbolNode MainFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.MainFacet => MainFacet;
    new IPackageFacetSymbolNode TestingFacet { get; }
    IPackageFacetDeclarationNode IPackageDeclarationNode.TestingFacet => TestingFacet;

    public static IPackageSymbolNode Create(IPackageReferenceNode packageReference)
        => new PackageSymbolNode(packageReference);
}

// [Closed(typeof(PackageFacetSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageFacetSymbolNode : IPackageFacetDeclarationNode, IChildSymbolNode
{
    new FixedSymbolTree SymbolTree { get; }
    ISymbolTree IChildSymbolNode.SymbolTree() => SymbolTree;
    new PackageSymbol Symbol
        => PackageSymbol;
    PackageSymbol IPackageFacetDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new INamespaceSymbolNode GlobalNamespace { get; }
    INamespaceDeclarationNode IPackageFacetDeclarationNode.GlobalNamespace => GlobalNamespace;

    public static IPackageFacetSymbolNode Create(FixedSymbolTree symbolTree)
        => new PackageFacetSymbolNode(symbolTree);
}

// [Closed(typeof(NamespaceSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceSymbolNode : INamespaceDeclarationNode, INamespaceMemberSymbolNode
{
    new NamespaceSymbol Symbol { get; }
    NamespaceSymbol INamespaceDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IFixedList<INamespaceMemberSymbolNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> INamespaceDeclarationNode.Members => Members;

    public static INamespaceSymbolNode Create(NamespaceSymbol symbol)
        => new NamespaceSymbolNode(symbol);
}

[Closed(
    typeof(INamespaceSymbolNode),
    typeof(IFunctionSymbolNode),
    typeof(IOrdinaryTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceMemberSymbolNode : INamespaceMemberDeclarationNode, IChildSymbolNode
{
}

// [Closed(typeof(FunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionSymbolNode : IFunctionDeclarationNode, INamespaceMemberSymbolNode
{
    new FunctionSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol? IFunctionInvocableDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName Name
        => Symbol.Name;
    OrdinaryName INamespaceMemberDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IFixedList<IMaybeNonVoidPlainType> IInvocableDeclarationNode.ParameterPlainTypes
        => Symbol.Type.Parameters.ToPlainTypes();
    IFixedList<IMaybeParameterType> IInvocableDeclarationNode.ParameterTypes
        => Symbol.Type.Parameters;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Symbol.Type.Return.PlainType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol.Type.Return;
    IMaybeFunctionPlainType IFunctionInvocableDeclarationNode.PlainType
        => Symbol.Type.PlainType;
    IMaybeFunctionType IFunctionInvocableDeclarationNode.Type
        => Symbol.Type;

    public static IFunctionSymbolNode Create(FunctionSymbol symbol)
        => new FunctionSymbolNode(symbol);
}

[Closed(
    typeof(INonVariableTypeSymbolNode),
    typeof(IVoidTypeSymbolNode),
    typeof(INeverTypeSymbolNode),
    typeof(IGenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeSymbolNode : ITypeDeclarationNode, IChildSymbolNode
{
    new TypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
}

[Closed(
    typeof(IBuiltInTypeSymbolNode),
    typeof(IOrdinaryTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INonVariableTypeSymbolNode : INonVariableTypeDeclarationNode, ITypeSymbolNode
{
    new ISelfSymbolNode ImplicitSelf { get; }
    IImplicitSelfDeclarationNode INonVariableTypeDeclarationNode.ImplicitSelf => ImplicitSelf;
}

[Closed(
    typeof(IPrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeSymbolNode : IBuiltInTypeDeclarationNode, INonVariableTypeSymbolNode
{
    new PrimitiveSymbolTree SymbolTree()
        => Primitive.SymbolTree;
    ISymbolTree IChildSymbolNode.SymbolTree() => SymbolTree();
    new BuiltInTypeName Name { get; }
    BuiltInTypeName IBuiltInTypeDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IBuiltInTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => Symbol.TryGetTypeConstructor()?.Supertypes ?? [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => Members;
}

// [Closed(typeof(VoidTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVoidTypeSymbolNode : ITypeSymbolNode
{
    new VoidTypeSymbol Symbol { get; }
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    TypeName INamedDeclarationNode.Name
        => Symbol.Name;
    ITypeFactory ITypeDeclarationNode.TypeFactory
        => ITypeFactory.Void;
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => [];
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => Members;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName named)
        => [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName named)
        => [];

    public static IVoidTypeSymbolNode Create(VoidTypeSymbol symbol)
        => new VoidTypeSymbolNode(symbol);
}

// [Closed(typeof(NeverTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INeverTypeSymbolNode : ITypeSymbolNode
{
    new NeverTypeSymbol Symbol { get; }
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    TypeName INamedDeclarationNode.Name
        => Symbol.Name;
    ITypeFactory ITypeDeclarationNode.TypeFactory
        => ITypeFactory.Never;
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => [];
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => Members;
    IEnumerable<IInstanceMemberDeclarationNode> ITypeDeclarationNode.InclusiveInstanceMembersNamed(OrdinaryName named)
        => [];
    IEnumerable<IAssociatedMemberDeclarationNode> ITypeDeclarationNode.AssociatedMembersNamed(OrdinaryName named)
        => [];

    public static INeverTypeSymbolNode Create(NeverTypeSymbol symbol)
        => new NeverTypeSymbolNode(symbol);
}

// [Closed(typeof(PrimitiveTypeSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPrimitiveTypeSymbolNode : IBuiltInTypeSymbolNode
{
    new BuiltInTypeSymbol Symbol { get; }
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    BuiltInTypeName IBuiltInTypeSymbolNode.Name
        => Symbol.Name;
    BareTypeConstructor INonVariableTypeDeclarationNode.TypeFactory
        => Symbol.TypeConstructor;

    public static IPrimitiveTypeSymbolNode Create(BuiltInTypeSymbol symbol)
        => new PrimitiveTypeSymbolNode(symbol);
}

[Closed(
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryTypeSymbolNode : IUserTypeDeclarationNode, INonVariableTypeSymbolNode, INamespaceMemberSymbolNode
{
    new OrdinaryTypeSymbol Symbol { get; }
    OrdinaryTypeSymbol IUserTypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new OrdinaryName Name
        => Symbol.Name;
    OrdinaryName INamespaceMemberDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    new IFixedList<IGenericParameterSymbolNode> GenericParameters { get; }
    IFixedList<IGenericParameterDeclarationNode> IUserTypeDeclarationNode.GenericParameters => GenericParameters;
    new IFixedSet<ITypeMemberSymbolNode> Members { get; }
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => Symbol.TryGetTypeConstructor().Supertypes;
    BareTypeConstructor INonVariableTypeDeclarationNode.TypeFactory
        => Symbol.TypeConstructor;
}

// [Closed(typeof(ClassSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassSymbolNode : IClassDeclarationNode, IOrdinaryTypeSymbolNode
{
    new IFixedSet<IClassMemberSymbolNode> Members { get; }
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IOrdinaryTypeSymbolNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<IClassMemberDeclarationNode> IClassDeclarationNode.InclusiveMembers
        => Members;

    public static IClassSymbolNode Create(OrdinaryTypeSymbol symbol)
        => new ClassSymbolNode(symbol);
}

// [Closed(typeof(StructSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructSymbolNode : IStructDeclarationNode, IOrdinaryTypeSymbolNode
{
    new IFixedSet<IStructMemberSymbolNode> Members { get; }
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IOrdinaryTypeSymbolNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<IStructMemberDeclarationNode> IStructDeclarationNode.InclusiveMembers
        => Members;

    public static IStructSymbolNode Create(OrdinaryTypeSymbol symbol)
        => new StructSymbolNode(symbol);
}

// [Closed(typeof(TraitSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitSymbolNode : ITraitDeclarationNode, IOrdinaryTypeSymbolNode
{
    new IFixedSet<ITraitMemberSymbolNode> Members { get; }
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> IOrdinaryTypeSymbolNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<ITraitMemberDeclarationNode> ITraitDeclarationNode.InclusiveMembers
        => Members;

    public static ITraitSymbolNode Create(OrdinaryTypeSymbol symbol)
        => new TraitSymbolNode(symbol);
}

// [Closed(typeof(GenericParameterSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterSymbolNode : IGenericParameterDeclarationNode, ITypeSymbolNode
{
    new GenericParameterTypeSymbol Symbol { get; }
    GenericParameterTypeSymbol IGenericParameterDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName Name
        => Symbol.Name;
    IdentifierName IGenericParameterDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    new IFixedSet<ITypeMemberSymbolNode> Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> IGenericParameterDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    IFixedSet<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
    IFixedSet<BareType> ITypeDeclarationNode.Supertypes
        => Symbol.TryGetTypeConstructor()?.Supertypes ?? [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];

    public static IGenericParameterSymbolNode Create(GenericParameterTypeSymbol symbol)
        => new GenericParameterSymbolNode(symbol);
}

// [Closed(typeof(SelfSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfSymbolNode : IImplicitSelfDeclarationNode
{
    new AssociatedTypeSymbol Symbol { get; }
    AssociatedTypeSymbol IImplicitSelfDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    new IFixedSet<ITypeMemberSymbolNode> Members
        => [];
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
    TypeSymbol ContainingSymbol
        => ContainingDeclaration.Symbol;
    AssociatedTypeConstructor IImplicitSelfDeclarationNode.TypeFactory
        => Symbol.TypeConstructor;
    IFixedSet<ITypeMemberDeclarationNode> ITypeDeclarationNode.InclusiveMembers
        => [];

    public static ISelfSymbolNode Create(AssociatedTypeSymbol symbol)
        => new SelfSymbolNode(symbol);
}

[Closed(
    typeof(IClassMemberSymbolNode),
    typeof(ITraitMemberSymbolNode),
    typeof(IStructMemberSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberSymbolNode : ITypeMemberDeclarationNode, IChildSymbolNode
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
    typeof(IOrdinaryMethodSymbolNode),
    typeof(IGetterMethodSymbolNode),
    typeof(ISetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSymbolNode : IMethodDeclarationNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode
{
    new MethodSymbol Symbol { get; }
    MethodSymbol? IMethodDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName Name
        => Symbol.Name;
    IdentifierName IMethodDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType IMethodDeclarationNode.SelfParameterPlainType
        => Symbol.SelfParameterType.PlainType;
    IMaybeNonVoidType IMethodDeclarationNode.SelfParameterType
        => Symbol.SelfParameterType;
    IFixedList<IMaybeNonVoidPlainType> IInvocableDeclarationNode.ParameterPlainTypes
        => Symbol.MethodGroupType.Parameters.ToPlainTypes();
    IFixedList<IMaybeParameterType> IInvocableDeclarationNode.ParameterTypes
        => Symbol.MethodGroupType.Parameters;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Symbol.MethodGroupType.Return.PlainType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol.MethodGroupType.Return;
}

// [Closed(typeof(OrdinaryMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryMethodSymbolNode : IOrdinaryMethodDeclarationNode, IMethodSymbolNode
{
    int IOrdinaryMethodDeclarationNode.Arity
        => Symbol.Arity;
    IMaybeFunctionPlainType IOrdinaryMethodDeclarationNode.MethodGroupPlainType
        => Symbol.MethodGroupType.PlainType;
    IMaybeFunctionType IOrdinaryMethodDeclarationNode.MethodGroupType
        => Symbol.MethodGroupType;

    public static IOrdinaryMethodSymbolNode Create(MethodSymbol symbol)
        => new OrdinaryMethodSymbolNode(symbol);
}

// [Closed(typeof(GetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodSymbolNode : IGetterMethodDeclarationNode, IMethodSymbolNode
{

    public static IGetterMethodSymbolNode Create(MethodSymbol symbol)
        => new GetterMethodSymbolNode(symbol);
}

// [Closed(typeof(SetterMethodSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodSymbolNode : ISetterMethodDeclarationNode, IMethodSymbolNode
{

    public static ISetterMethodSymbolNode Create(MethodSymbol symbol)
        => new SetterMethodSymbolNode(symbol);
}

// [Closed(typeof(ConstructorSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSymbolNode : IConstructorDeclarationNode, IClassMemberSymbolNode
{
    new ConstructorSymbol Symbol { get; }
    ConstructorSymbol? IConstructorDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName? Name
        => Symbol.Name;
    IdentifierName? IConstructorDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType IConstructorDeclarationNode.SelfParameterPlainType
        => Symbol.SelfParameterType.PlainType;
    IMaybeNonVoidType IConstructorDeclarationNode.SelfParameterType
        => Symbol.SelfParameterType;
    IFixedList<IMaybeNonVoidPlainType> IInvocableDeclarationNode.ParameterPlainTypes
        => Symbol.ParameterTypes.ToPlainTypes();
    IFixedList<IMaybeParameterType> IInvocableDeclarationNode.ParameterTypes
        => Symbol.ParameterTypes;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Symbol.ReturnType.PlainType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol.ReturnType;

    public static IConstructorSymbolNode Create(ConstructorSymbol symbol)
        => new ConstructorSymbolNode(symbol);
}

// [Closed(typeof(InitializerSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSymbolNode : IInitializerDeclarationNode, IStructMemberSymbolNode
{
    new InitializerSymbol Symbol { get; }
    InitializerSymbol? IInitializerDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName? Name
        => Symbol.Name;
    IdentifierName? IInitializerDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IMaybeNonVoidPlainType IInitializerDeclarationNode.SelfParameterPlainType
        => Symbol.SelfParameterType.PlainType;
    IMaybeNonVoidType IInitializerDeclarationNode.SelfParameterType
        => Symbol.SelfParameterType;
    IFixedList<IMaybeNonVoidPlainType> IInvocableDeclarationNode.ParameterPlainTypes
        => Symbol.ParameterTypes.ToPlainTypes();
    IFixedList<IMaybeParameterType> IInvocableDeclarationNode.ParameterTypes
        => Symbol.ParameterTypes;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Symbol.ReturnType.PlainType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol.ReturnType;

    public static IInitializerSymbolNode Create(InitializerSymbol symbol)
        => new InitializerSymbolNode(symbol);
}

// [Closed(typeof(FieldSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldSymbolNode : IFieldDeclarationNode, IClassMemberSymbolNode, IStructMemberSymbolNode
{
    new FieldSymbol Symbol { get; }
    FieldSymbol? IFieldDeclarationNode.Symbol => Symbol;
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    new IdentifierName Name
        => Symbol.Name;
    IdentifierName IFieldDeclarationNode.Name => Name;
    OrdinaryName? IPackageFacetChildDeclarationNode.Name => Name;
    IdentifierName INamedBindingDeclarationNode.Name => Name;
    TypeName INamedDeclarationNode.Name => Name;
    bool IFieldDeclarationNode.IsMutableBinding
        => Symbol.IsMutableBinding;
    IMaybeNonVoidType IFieldDeclarationNode.BindingType
        => Symbol.Type;

    public static IFieldSymbolNode Create(FieldSymbol symbol)
        => new FieldSymbolNode(symbol);
}

// [Closed(typeof(AssociatedFunctionSymbolNode))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionSymbolNode : IAssociatedFunctionDeclarationNode, IAssociatedMemberSymbolNode
{
    new FunctionSymbol Symbol { get; }
    Symbol? ISymbolDeclarationNode.Symbol => Symbol;
    FunctionSymbol? IFunctionInvocableDeclarationNode.Symbol => Symbol;
    InvocableSymbol? IInvocableDeclarationNode.Symbol => Symbol;
    Symbol IChildSymbolNode.Symbol => Symbol;
    IMaybeFunctionPlainType IFunctionInvocableDeclarationNode.PlainType
        => Symbol.Type.PlainType;
    IMaybeFunctionType IFunctionInvocableDeclarationNode.Type
        => Symbol.Type;
    IFixedList<IMaybeNonVoidPlainType> IInvocableDeclarationNode.ParameterPlainTypes
        => Symbol.Type.Parameters.ToPlainTypes();
    IFixedList<IMaybeParameterType> IInvocableDeclarationNode.ParameterTypes
        => Symbol.Type.Parameters;
    IMaybePlainType IInvocableDeclarationNode.ReturnPlainType
        => Symbol.Type.Return.PlainType;
    IMaybeType IInvocableDeclarationNode.ReturnType
        => Symbol.Type.Return;

    public static IAssociatedFunctionSymbolNode Create(
        OrdinaryName name,
        FunctionSymbol symbol)
        => new AssociatedFunctionSymbolNode(name, symbol);
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

    internal virtual ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (PeekParent() ?? throw Child.InheritFailed("ValueIdScope", child, descendant)).Inherited_ValueIdScope(this, descendant, ctx);
    protected ValueIdScope Inherited_ValueIdScope(IInheritanceContext ctx)
        => PeekParent()!.Inherited_ValueIdScope(this, this, ctx);

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

    internal virtual OrdinaryTypeConstructor Inherited_ContainingTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingTypeConstructor", child, descendant)).Inherited_ContainingTypeConstructor(this, descendant, ctx);
    protected OrdinaryTypeConstructor Inherited_ContainingTypeConstructor(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingTypeConstructor(this, this, ctx);

    internal virtual ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingTypeDefinition", child, descendant)).Inherited_ContainingTypeDefinition(this, descendant, ctx);
    protected ITypeDefinitionNode Inherited_ContainingTypeDefinition(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingTypeDefinition(this, this, ctx);

    internal virtual IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("FlowStateBefore", child, descendant)).Inherited_FlowStateBefore(this, descendant, ctx);
    protected IFlowState Inherited_FlowStateBefore(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_FlowStateBefore(this, this, ctx);

    internal virtual SelfTypeConstructor Inherited_ContainingSelfTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContainingSelfTypeConstructor", child, descendant)).Inherited_ContainingSelfTypeConstructor(this, descendant, ctx);
    protected SelfTypeConstructor Inherited_ContainingSelfTypeConstructor(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContainingSelfTypeConstructor(this, this, ctx);

    internal virtual IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedType", child, descendant)).Inherited_ExpectedType(this, descendant, ctx);
    protected IMaybeType? Inherited_ExpectedType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedType(this, this, ctx);

    internal virtual IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedPlainType", child, descendant)).Inherited_ExpectedPlainType(this, descendant, ctx);
    protected IMaybePlainType? Inherited_ExpectedPlainType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedPlainType(this, this, ctx);

    internal virtual bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("IsAttributeType", child, descendant)).Inherited_IsAttributeType(this, descendant, ctx);
    protected bool Inherited_IsAttributeType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_IsAttributeType(this, this, ctx);

    internal virtual IMaybeType? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("MethodSelfType", child, descendant)).Inherited_MethodSelfType(this, descendant, ctx);
    protected IMaybeType? Inherited_MethodSelfType(IInheritanceContext ctx)
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

    internal virtual bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ImplicitRecoveryAllowed", child, descendant)).Inherited_ImplicitRecoveryAllowed(this, descendant, ctx);
    protected bool Inherited_ImplicitRecoveryAllowed(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ImplicitRecoveryAllowed(this, this, ctx);

    internal virtual bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ShouldPrepareToReturn", child, descendant)).Inherited_ShouldPrepareToReturn(this, descendant, ctx);
    protected bool Inherited_ShouldPrepareToReturn(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ShouldPrepareToReturn(this, this, ctx);

    internal virtual ValueId? Inherited_MatchReferentValueId(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("MatchReferentValueId", child, descendant)).Inherited_MatchReferentValueId(this, descendant, ctx);
    protected ValueId? Inherited_MatchReferentValueId(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_MatchReferentValueId(this, this, ctx);

    internal virtual IMaybeNonVoidPlainType Inherited_ContextBindingPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContextBindingPlainType", child, descendant)).Inherited_ContextBindingPlainType(this, descendant, ctx);
    protected IMaybeNonVoidPlainType Inherited_ContextBindingPlainType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContextBindingPlainType(this, this, ctx);

    internal virtual IMaybeNonVoidType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ContextBindingType", child, descendant)).Inherited_ContextBindingType(this, descendant, ctx);
    protected IMaybeNonVoidType Inherited_ContextBindingType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ContextBindingType(this, this, ctx);

    internal virtual IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedReturnType", child, descendant)).Inherited_ExpectedReturnType(this, descendant, ctx);
    protected IMaybeType? Inherited_ExpectedReturnType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedReturnType(this, this, ctx);

    internal virtual IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ControlFlowExit", child, descendant)).Inherited_ControlFlowExit(this, descendant, ctx);
    protected IExitNode Inherited_ControlFlowExit(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ControlFlowExit(this, this, ctx);

    internal virtual IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("ExpectedReturnPlainType", child, descendant)).Inherited_ExpectedReturnPlainType(this, descendant, ctx);
    protected IMaybePlainType? Inherited_ExpectedReturnPlainType(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_ExpectedReturnPlainType(this, this, ctx);

    internal virtual ISymbolTree Inherited_SymbolTree(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => (GetParent(ctx) ?? throw Child.InheritFailed("SymbolTree", child, descendant)).Inherited_SymbolTree(this, descendant, ctx);
    protected ISymbolTree Inherited_SymbolTree(IInheritanceContext ctx)
        => GetParent(ctx)!.Inherited_SymbolTree(this, this, ctx);

    // ControlFlowPrevious is a collection attribute family
    protected IFixedSet<SemanticNode> CollectContributors_ControlFlowPrevious<TRoot>(SemanticNode target, IInheritanceContext ctx)
    {
        if (this is TRoot) return CollectContributors_ControlFlowPrevious(target);
        return GetParent(ctx)?.CollectContributors_ControlFlowPrevious<TRoot>(target, ctx)
            ?? (InFinalTree ? CollectContributors_ControlFlowPrevious(target)
                : throw Child.ParentMissing(this));
    }
    // TODO avoid adding this to every node
    private ContributorCollection<SemanticNode>? contributors_ControlFlowPrevious;
    private IFixedSet<SemanticNode> CollectContributors_ControlFlowPrevious(SemanticNode target)
    {
        var contributors = Lazy.Initialize(ref contributors_ControlFlowPrevious,
            static () => new ContributorCollection<SemanticNode>());
        contributors.EnsurePopulated(CollectContributors_ControlFlowPrevious);
        return contributors.Remove(target).ToFixedSet();
    }
    internal virtual void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        foreach (var child in Children().Cast<SemanticNode>())
            child.CollectContributors_ControlFlowPrevious(contributors);
    }
    protected ControlFlowSet Collect_ControlFlowPrevious(SemanticNode target, IFixedSet<SemanticNode> contributors)
    {
        var builder = new Dictionary<IControlFlowNode, ControlFlowKind>();
        foreach (var contributor in contributors)
            contributor.Contribute_ControlFlowPrevious(target, builder);
        return builder.ToControlFlowSet();
    }
    internal virtual void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder) { }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageNode : SemanticNode, IPackageNode
{
    private IPackageNode Self { [Inline] get => this; }

    public IPackageSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedSet<IPackageReferenceNode> References { [DebuggerStepThrough] get; }
    public IPackageFacetNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetNode TestingFacet { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public DiagnosticCollection Diagnostics
        => GrammarAttribute.IsCached(in diagnosticsCached) ? diagnostics!
            : this.Aggregate(ref diagnosticsContributors, ref diagnosticsCached, ref diagnostics,
                CollectContributors_Diagnostics, Collect_Diagnostics);
    private DiagnosticCollection? diagnostics;
    private bool diagnosticsCached;
    private IFixedSet<SemanticNode>? diagnosticsContributors;
    public IFunctionDefinitionNode? EntryPoint
        => GrammarAttribute.IsCached(in entryPointCached) ? entryPoint
            : this.Synthetic(ref entryPointCached, ref entryPoint,
                DefinitionsAspect.Package_EntryPoint);
    private IFunctionDefinitionNode? entryPoint;
    private bool entryPointCached;
    public IPackageReferenceNode IntrinsicsReference
        => GrammarAttribute.IsCached(in intrinsicsReferenceCached) ? intrinsicsReference!
            : this.Synthetic(ref intrinsicsReferenceCached, ref intrinsicsReference,
                n => Child.Attach(this, BuiltInsAspect.Package_IntrinsicsReference(n)));
    private IPackageReferenceNode? intrinsicsReference;
    private bool intrinsicsReferenceCached;
    public IPackageSymbols PackageSymbols
        => GrammarAttribute.IsCached(in packageSymbolsCached) ? packageSymbols!
            : this.Synthetic(ref packageSymbolsCached, ref packageSymbols,
                SymbolsAspect.Package_PackageSymbols);
    private IPackageSymbols? packageSymbols;
    private bool packageSymbolsCached;
    public IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations
        => GrammarAttribute.IsCached(in primitivesDeclarationsCached) ? primitivesDeclarations!
            : this.Synthetic(ref primitivesDeclarationsCached, ref primitivesDeclarations,
                n => ChildSet.Attach(this, BuiltInsAspect.Package_PrimitivesDeclarations(n)));
    private IFixedSet<ITypeDeclarationNode>? primitivesDeclarations;
    private bool primitivesDeclarationsCached;
    public PackageSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.Package_Symbol);
    private PackageSymbol? symbol;
    private bool symbolCached;

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

    public StandardPackageReferenceNode(IPackageReferenceSyntax syntax)
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

    public IntrinsicsPackageReferenceNode()
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope
        => GrammarAttribute.IsCached(in packageNameScopeCached) ? packageNameScope!
            : this.Inherited(ref packageNameScopeCached, ref packageNameScope,
                Inherited_PackageNameScope);
    private PackageNameScope? packageNameScope;
    private bool packageNameScopeCached;
    public IFixedSet<IFacetMemberDefinitionNode> Definitions
        => GrammarAttribute.IsCached(in definitionsCached) ? definitions!
            : this.Synthetic(ref definitionsCached, ref definitions,
                DefinitionsAspect.PackageFacet_Definitions);
    private IFixedSet<IFacetMemberDefinitionNode>? definitions;
    private bool definitionsCached;
    public INamespaceDefinitionNode GlobalNamespace
        => GrammarAttribute.IsCached(in globalNamespaceCached) ? globalNamespace!
            : this.Synthetic(ref globalNamespaceCached, ref globalNamespace,
                NamespaceDefinitions.PackageFacet_GlobalNamespace);
    private INamespaceDefinitionNode? globalNamespace;
    private bool globalNamespaceCached;

    public PackageFacetNode(
        IPackageSyntax syntax,
        IEnumerable<ICompilationUnitNode> compilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = ChildSet.Attach(this, compilationUnits);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return this;
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
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

    internal override PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return PackageNameScope;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CompilationUnitNode : SemanticNode, ICompilationUnitNode
{
    private ICompilationUnitNode Self { [Inline] get => this; }

    public ICompilationUnitSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IImportDirectiveNode> ImportDirectives { [DebuggerStepThrough] get; }
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
    public INamespaceDefinitionNode ImplicitNamespace
        => GrammarAttribute.IsCached(in implicitNamespaceCached) ? implicitNamespace!
            : this.Synthetic(ref implicitNamespaceCached, ref implicitNamespace,
                NamespaceDefinitions.CompilationUnit_ImplicitNamespace);
    private INamespaceDefinitionNode? implicitNamespace;
    private bool implicitNamespaceCached;
    public NamespaceSearchScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.CompilationUnit_LexicalScope);
    private NamespaceSearchScope? lexicalScope;
    private bool lexicalScopeCached;

    public CompilationUnitNode(
        ICompilationUnitSyntax syntax,
        IEnumerable<IImportDirectiveNode> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> definitions)
    {
        Syntax = syntax;
        ImportDirectives = ChildList.Attach(this, importDirectives);
        Definitions = ChildList.Attach(this, definitions);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return Is.OfType<INamespaceDefinitionNode>(ImplicitNamespace);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<NamespaceSearchScope>(LexicalScope);
    }

    internal override CodeFile Inherited_File(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ContextAspect.CompilationUnit_Children_Broadcast_File(this);
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
file class ImportDirectiveNode : SemanticNode, IImportDirectiveNode
{
    private IImportDirectiveNode Self { [Inline] get => this; }

    public IImportDirectiveSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public ImportDirectiveNode(IImportDirectiveSyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceBlockDefinitionNode : SemanticNode, INamespaceBlockDefinitionNode
{
    private INamespaceBlockDefinitionNode Self { [Inline] get => this; }

    public INamespaceBlockDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IImportDirectiveNode> ImportDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
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
    public INamespaceDefinitionNode ContainingNamespace
        => GrammarAttribute.IsCached(in containingNamespaceCached) ? containingNamespace!
            : this.Synthetic(ref containingNamespaceCached, ref containingNamespace,
                NamespaceDefinitions.NamespaceBlockDefinition_ContainingNamespace);
    private INamespaceDefinitionNode? containingNamespace;
    private bool containingNamespaceCached;
    public INamespaceDefinitionNode Definition
        => GrammarAttribute.IsCached(in definitionCached) ? definition!
            : this.Synthetic(ref definitionCached, ref definition,
                NamespaceDefinitions.NamespaceBlockDefinition_Definition);
    private INamespaceDefinitionNode? definition;
    private bool definitionCached;
    public NamespaceSearchScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.NamespaceBlockDefinition_LexicalScope);
    private NamespaceSearchScope? lexicalScope;
    private bool lexicalScopeCached;

    public NamespaceBlockDefinitionNode(
        INamespaceBlockDefinitionSyntax syntax,
        IEnumerable<IImportDirectiveNode> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> members)
    {
        Syntax = syntax;
        ImportDirectives = ChildList.Attach(this, importDirectives);
        Members = ChildList.Attach(this, members);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return ContextAspect.NamespaceBlockDefinition_Children_ContainingDeclaration(this);
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Is.OfType<NamespaceSearchScope>(LexicalScope);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceDefinitionNode : SemanticNode, INamespaceDefinitionNode
{
    private INamespaceDefinitionNode Self { [Inline] get => this; }

    public NamespaceSymbol Symbol { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { [DebuggerStepThrough] get; }
    public IFixedList<IFacetMemberDefinitionNode> PackageMembers { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public IFixedList<INamespaceMemberDefinitionNode> Members { [DebuggerStepThrough] get; }
    public FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => GrammarAttribute.IsCached(in nestedMembersCached) ? nestedMembers!
            : this.Synthetic(ref nestedMembersCached, ref nestedMembers,
                DeclarationsAspect.NamespaceDeclaration_NestedMembers);
    private IFixedList<INamespaceMemberDeclarationNode>? nestedMembers;
    private bool nestedMembersCached;
    public FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;

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
    public INamespaceDeclarationNode ContainingDeclaration
        => (INamespaceDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
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
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.FunctionDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.FunctionDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public IMaybeFunctionPlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                DefinitionPlainTypesAspect.ConcreteFunctionInvocableDefinition_PlainType);
    private IMaybeFunctionPlainType? plainType;
    private bool plainTypeCached;
    public FunctionSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.FunctionDefinition_Symbol);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public IMaybeFunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                TypeMemberDeclarationsAspect.ConcreteFunctionInvocableDefinition_Type);
    private IMaybeFunctionType? type;
    private bool typeCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

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
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IDefaultConstructorDefinitionNode? DefaultConstructor
        => GrammarAttribute.IsCached(in defaultConstructorCached) ? defaultConstructor
            : this.Synthetic(ref defaultConstructorCached, ref defaultConstructor,
                n => Child.Attach(this, DefaultMembersAspect.ClassDefinition_DefaultConstructor(n)));
    private IDefaultConstructorDefinitionNode? defaultConstructor;
    private bool defaultConstructorCached;
    public IImplicitSelfDefinitionNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, DefaultMembersAspect.TypeDefinition_ImplicitSelf(n)));
    private IImplicitSelfDefinitionNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<IClassMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.ClassDefinition_InclusiveMembers);
    private IFixedSet<IClassMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedSet<IClassMemberDefinitionNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                DefaultMembersAspect.ClassDefinition_Members);
    private IFixedSet<IClassMemberDefinitionNode>? members;
    private bool membersCached;
    public IFixedSet<BareType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareType>> supertypes = new(BareType.AnySet);
    private bool supertypesCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public OrdinaryTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private OrdinaryTypeSymbol? symbol;
    private bool symbolCached;
    public OrdinaryTypeConstructor TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DefinitionPlainTypesAspect.ClassDefinition_TypeFactory);
    private OrdinaryTypeConstructor? typeFactory;
    private bool typeFactoryCached;

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

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
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

    internal override SelfTypeConstructor Inherited_ContainingSelfTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingSelfTypeConstructor(this);
    }

    internal override OrdinaryTypeConstructor Inherited_ContainingTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingTypeConstructor(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IDefaultInitializerDefinitionNode? DefaultInitializer
        => GrammarAttribute.IsCached(in defaultInitializerCached) ? defaultInitializer
            : this.Synthetic(ref defaultInitializerCached, ref defaultInitializer,
                n => Child.Attach(this, DefaultMembersAspect.StructDefinition_DefaultInitializer(n)));
    private IDefaultInitializerDefinitionNode? defaultInitializer;
    private bool defaultInitializerCached;
    public IImplicitSelfDefinitionNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, DefaultMembersAspect.TypeDefinition_ImplicitSelf(n)));
    private IImplicitSelfDefinitionNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<IStructMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.StructDefinition_InclusiveMembers);
    private IFixedSet<IStructMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedSet<IStructMemberDefinitionNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                DefaultMembersAspect.StructDefinition_Members);
    private IFixedSet<IStructMemberDefinitionNode>? members;
    private bool membersCached;
    public IFixedSet<BareType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareType>> supertypes = new(BareType.AnySet);
    private bool supertypesCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public OrdinaryTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private OrdinaryTypeSymbol? symbol;
    private bool symbolCached;
    public OrdinaryTypeConstructor TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DefinitionPlainTypesAspect.StructDefinition_TypeFactory);
    private OrdinaryTypeConstructor? typeFactory;
    private bool typeFactoryCached;

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

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
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

    internal override SelfTypeConstructor Inherited_ContainingSelfTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingSelfTypeConstructor(this);
    }

    internal override OrdinaryTypeConstructor Inherited_ContainingTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingTypeConstructor(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IImplicitSelfDefinitionNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, DefaultMembersAspect.TypeDefinition_ImplicitSelf(n)));
    private IImplicitSelfDefinitionNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers
        => GrammarAttribute.IsCached(in inclusiveMembersCached) ? inclusiveMembers!
            : this.Synthetic(ref inclusiveMembersCached, ref inclusiveMembers,
                InheritanceAspect.TraitDefinition_InclusiveMembers);
    private IFixedSet<ITraitMemberDeclarationNode>? inclusiveMembers;
    private bool inclusiveMembersCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.TypeDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedSet<BareType> Supertypes
        => GrammarAttribute.IsCached(in supertypesCached) ? supertypes.UnsafeValue
            : this.Circular(ref supertypesCached, ref supertypes,
                TypeDefinitionsAspect.TypeDefinition_Supertypes);
    private Circular<IFixedSet<BareType>> supertypes = new(BareType.AnySet);
    private bool supertypesCached;
    public LexicalScope SupertypesLexicalScope
        => GrammarAttribute.IsCached(in supertypesLexicalScopeCached) ? supertypesLexicalScope!
            : this.Synthetic(ref supertypesLexicalScopeCached, ref supertypesLexicalScope,
                LexicalScopingAspect.TypeDefinition_SupertypesLexicalScope);
    private LexicalScope? supertypesLexicalScope;
    private bool supertypesLexicalScopeCached;
    public OrdinaryTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.TypeDefinition_Symbol);
    private OrdinaryTypeSymbol? symbol;
    private bool symbolCached;
    public OrdinaryTypeConstructor TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DefinitionPlainTypesAspect.TraitDefinition_TypeFactory);
    private OrdinaryTypeConstructor? typeFactory;
    private bool typeFactoryCached;

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

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
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

    internal override SelfTypeConstructor Inherited_ContainingSelfTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingSelfTypeConstructor(this);
    }

    internal override OrdinaryTypeConstructor Inherited_ContainingTypeConstructor(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return DefinitionTypesAspect.TypeDefinition_Children_Broadcast_ContainingTypeConstructor(this);
    }

    internal override ITypeDefinitionNode Inherited_ContainingTypeDefinition(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override bool Inherited_IsAttributeType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return false;
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
    public OrdinaryTypeConstructor ContainingTypeConstructor
        => GrammarAttribute.IsCached(in containingTypeConstructorCached) ? containingTypeConstructor!
            : this.Inherited(ref containingTypeConstructorCached, ref containingTypeConstructor,
                Inherited_ContainingTypeConstructor);
    private OrdinaryTypeConstructor? containingTypeConstructor;
    private bool containingTypeConstructorCached;
    public GenericParameterType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType,
                TypeDefinitionsAspect.GenericParameter_DeclaredType);
    private GenericParameterType? declaredType;
    private bool declaredTypeCached;
    public TypeConstructorParameter Parameter
        => GrammarAttribute.IsCached(in parameterCached) ? parameter!
            : this.Synthetic(ref parameterCached, ref parameter,
                TypeDefinitionsAspect.GenericParameter_Parameter);
    private TypeConstructorParameter? parameter;
    private bool parameterCached;
    public GenericParameterTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.GenericParameter_Symbol);
    private GenericParameterTypeSymbol? symbol;
    private bool symbolCached;
    public ITypeFactory TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DeclarationsAspect.GenericParameterDeclaration_TypeFactory);
    private ITypeFactory? typeFactory;
    private bool typeFactoryCached;

    public GenericParameterNode(
        IGenericParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = Child.Attach(this, constraint);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitSelfDefinitionNode : SemanticNode, IImplicitSelfDefinitionNode
{
    private IImplicitSelfDefinitionNode Self { [Inline] get => this; }

    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public INonVariableTypeDeclarationNode ContainingDeclaration
        => (INonVariableTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public AssociatedTypeSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.ImplicitSelfDefinition_Symbol);
    private AssociatedTypeSymbol? symbol;
    private bool symbolCached;
    public SelfTypeConstructor TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DefinitionPlainTypesAspect.ImplicitSelfDefinition_TypeFactory);
    private SelfTypeConstructor? typeFactory;
    private bool typeFactoryCached;

    public ImplicitSelfDefinitionNode()
    {
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
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
    public OrdinaryTypeConstructor ContainingTypeConstructor
        => GrammarAttribute.IsCached(in containingTypeConstructorCached) ? containingTypeConstructor!
            : this.Inherited(ref containingTypeConstructorCached, ref containingTypeConstructor,
                Inherited_ContainingTypeConstructor);
    private OrdinaryTypeConstructor? containingTypeConstructor;
    private bool containingTypeConstructorCached;
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public MethodSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

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
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override IMaybeType? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.MethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

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
file class OrdinaryMethodDefinitionNode : SemanticNode, IOrdinaryMethodDefinitionNode
{
    private IOrdinaryMethodDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IOrdinaryMethodDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public MethodSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public OrdinaryMethodDefinitionNode(
        IOrdinaryMethodDefinitionSyntax syntax,
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
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override IMaybeType? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.MethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public MethodSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public GetterMethodDefinitionNode(
        IGetterMethodDefinitionSyntax syntax,
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode @return,
        IBodyNode? body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override IMaybeType? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.MethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.MethodDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public MethodSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.MethodDefinition_Symbol);
    private MethodSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

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
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override IMaybeType? Inherited_MethodSelfType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return TypeExpressionsAspect.MethodDefinition_Children_Broadcast_MethodSelfType(this);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

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

    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
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
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public ConstructorSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.DefaultConstructorDefinition_Symbol);
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public DefaultConstructorDefinitionNode()
    {
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OrdinaryConstructorDefinitionNode : SemanticNode, IOrdinaryConstructorDefinitionNode
{
    private IOrdinaryConstructorDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IConstructorDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ConstructorDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public ConstructorSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.OrdinaryConstructorDefinition_Symbol);
    private ConstructorSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public OrdinaryConstructorDefinitionNode(
        IConstructorDefinitionSyntax syntax,
        IConstructorSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return AzothPlainType.Void;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return AzothType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
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

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class DefaultInitializerDefinitionNode : SemanticNode, IDefaultInitializerDefinitionNode
{
    private IDefaultInitializerDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
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
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.InitializerDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public InitializerSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.DefaultInitializerDefinition_Symbol);
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public DefaultInitializerDefinitionNode()
    {
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OrdinaryInitializerDefinitionNode : SemanticNode, IOrdinaryInitializerDefinitionNode
{
    private IOrdinaryInitializerDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IInitializerDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterNode SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterNode> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodyNode Body { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.InitializerDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public InitializerSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.OrdinaryInitializerDefinition_Symbol);
    private InitializerSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public OrdinaryInitializerDefinitionNode(
        IInitializerDefinitionSyntax syntax,
        IInitializerSelfParameterNode selfParameter,
        IEnumerable<IConstructorOrInitializerParameterNode> parameters,
        IBlockBodyNode body)
    {
        Syntax = syntax;
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return AzothPlainType.Void;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return AzothType.Void;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
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

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldDefinitionNode : SemanticNode, IFieldDefinitionNode
{
    private IFieldDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IFieldDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode TypeNode { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode?> initializer;
    private bool initializerCached;
    public IAmbiguousExpressionNode? TempInitializer
        => GrammarAttribute.IsCached(in initializerCached) ? initializer.UnsafeValue
            : this.RewritableChild(ref initializerCached, ref initializer);
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentInitializer => initializer.UnsafeValue;
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                DefinitionPlainTypesAspect.FieldDefinition_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.FieldDefinition_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.FieldDefinition_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public FieldSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.FieldDefinition_Symbol);
    private FieldSymbol? symbol;
    private bool symbolCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.FieldDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public FieldDefinitionNode(
        IFieldDefinitionSyntax syntax,
        ITypeNode typeNode,
        IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, typeNode);
        this.initializer = Child.Create(this, initializer);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowSet.CreateNormal(Initializer ?? (IControlFlowNode)Exit);
        if (ReferenceEquals(descendant, Self.CurrentInitializer))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentInitializer))
            return null;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentInitializer))
            return null;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeMemberDeclarationsAspect.FieldDefinition_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssociatedFunctionDefinitionNode : SemanticNode, IAssociatedFunctionDefinitionNode
{
    private IAssociatedFunctionDefinitionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IAssociatedFunctionDefinitionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode? Return { [DebuggerStepThrough] get; }
    public IBodyNode Body { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
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
    public AccessModifier AccessModifier
        => GrammarAttribute.IsCached(in accessModifierCached) ? accessModifier
            : this.Synthetic(ref accessModifierCached, ref accessModifier, ref syncLock,
                TypeModifiersAspect.TypeMemberDefinition_AccessModifier);
    private AccessModifier accessModifier;
    private bool accessModifierCached;
    public IEntryNode Entry { [DebuggerStepThrough] get; }
    public IExitNode Exit { [DebuggerStepThrough] get; }
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.AssociatedFunctionDefinition_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes
        => GrammarAttribute.IsCached(in parameterPlainTypesCached) ? parameterPlainTypes!
            : this.Synthetic(ref parameterPlainTypesCached, ref parameterPlainTypes,
                DefinitionPlainTypesAspect.InvocableDefinition_ParameterPlainTypes);
    private IFixedList<IMaybeNonVoidPlainType>? parameterPlainTypes;
    private bool parameterPlainTypesCached;
    public IFixedList<IMaybeParameterType> ParameterTypes
        => GrammarAttribute.IsCached(in parameterTypesCached) ? parameterTypes!
            : this.Synthetic(ref parameterTypesCached, ref parameterTypes,
                DefinitionTypesAspect.InvocableDefinition_ParameterTypes);
    private IFixedList<IMaybeParameterType>? parameterTypes;
    private bool parameterTypesCached;
    public IMaybeFunctionPlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                DefinitionPlainTypesAspect.ConcreteFunctionInvocableDefinition_PlainType);
    private IMaybeFunctionPlainType? plainType;
    private bool plainTypeCached;
    public FunctionSymbol? Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol
            : this.Synthetic(ref symbolCached, ref symbol,
                SymbolsAspect.AssociatedFunctionDefinition_Symbol);
    private FunctionSymbol? symbol;
    private bool symbolCached;
    public IMaybeFunctionType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                TypeMemberDeclarationsAspect.ConcreteFunctionInvocableDefinition_Type);
    private IMaybeFunctionType? type;
    private bool typeCached;
    public ValueIdScope ValueIdScope { [DebuggerStepThrough] get; }
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap
        => GrammarAttribute.IsCached(in variableBindingsMapCached) ? variableBindingsMap!
            : this.Synthetic(ref variableBindingsMapCached, ref variableBindingsMap,
                VariablesAspect.InvocableDefinition_VariableBindingsMap);
    private FixedDictionary<IVariableBindingNode, int>? variableBindingsMap;
    private bool variableBindingsMapCached;

    public AssociatedFunctionDefinitionNode(
        IAssociatedFunctionDefinitionSyntax syntax,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return,
        IBodyNode body)
    {
        Syntax = syntax;
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
        Body = Child.Attach(this, body);
        Entry = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Entry(this));
        Exit = Child.Attach(this, ControlFlowAspect.ExecutableDefinition_Exit(this));
        ValueIdScope = ValueIdsAspect.ExecutableDefinition_ValueIdScope(this);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IEntryNode Inherited_ControlFlowEntry(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Entry;
    }

    internal override IExitNode Inherited_ControlFlowExit(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return Exit;
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return ControlFlowAspect.InvocableDefinition_Entry_ControlFlowFollowing(this);
        if (ReferenceEquals(child, Self.Body))
            return ControlFlowSet.CreateNormal(Exit);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedReturnPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnPlainType;
        return base.Inherited_ExpectedReturnPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedReturnType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedReturnType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Body))
            return Self.ReturnType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
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

    internal override ValueIdScope Inherited_ValueIdScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return ValueIdScope;
    }

    internal override FixedDictionary<IVariableBindingNode, int> Inherited_VariableBindingsMap(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Entry))
            return VariableBindingsMap;
        return base.Inherited_VariableBindingsMap(child, descendant, ctx);
    }
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
    public InvocableSymbol? ReferencedSymbol
        => GrammarAttribute.IsCached(in referencedSymbolCached) ? referencedSymbol
            : this.Synthetic(ref referencedSymbolCached, ref referencedSymbol,
                SymbolsAspect.Attribute_ReferencedSymbol);
    private InvocableSymbol? referencedSymbol;
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

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        BindingNamesAspect.Attribute_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilitySetNode : SemanticNode, ICapabilitySetNode
{
    private ICapabilitySetNode Self { [Inline] get => this; }

    public ICapabilitySetSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilitySetNode(ICapabilitySetSyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityNode : SemanticNode, ICapabilityNode
{
    private ICapabilityNode Self { [Inline] get => this; }

    public ICapabilitySyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    public CapabilityNode(ICapabilitySyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamedParameterNode : SemanticNode, INamedParameterNode
{
    private INamedParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public INamedParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode TypeNode { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.NamedParameter_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.NamedParameter_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.NamedParameter_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeParameterType ParameterType
        => GrammarAttribute.IsCached(in parameterTypeCached) ? parameterType!
            : this.Synthetic(ref parameterTypeCached, ref parameterType,
                NameBindingTypesAspect.NamedParameter_ParameterType);
    private IMaybeParameterType? parameterType;
    private bool parameterTypeCached;

    public NamedParameterNode(
        INamedParameterSyntax syntax,
        ITypeNode typeNode)
    {
        Syntax = syntax;
        TypeNode = Child.Attach(this, typeNode);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        NameBindingTypesAspect.NamedParameter_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSelfParameterNode : SemanticNode, IConstructorSelfParameterNode
{
    private IConstructorSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IConstructorSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public OrdinaryTypeConstructor ContainingTypeConstructor
        => GrammarAttribute.IsCached(in containingTypeConstructorCached) ? containingTypeConstructor!
            : this.Inherited(ref containingTypeConstructorCached, ref containingTypeConstructor,
                Inherited_ContainingTypeConstructor);
    private OrdinaryTypeConstructor? containingTypeConstructor;
    private bool containingTypeConstructorCached;
    public SelfTypeConstructor ContainingSelfTypeConstructor
        => GrammarAttribute.IsCached(in containingSelfTypeConstructorCached) ? containingSelfTypeConstructor!
            : this.Inherited(ref containingSelfTypeConstructorCached, ref containingSelfTypeConstructor,
                Inherited_ContainingSelfTypeConstructor);
    private SelfTypeConstructor? containingSelfTypeConstructor;
    private bool containingSelfTypeConstructorCached;
    public ConstructedPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.SelfParameter_BindingPlainType);
    private ConstructedPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public CapabilityType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.ConstructorSelfParameter_BindingType);
    private CapabilityType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SelfParameter_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeNonVoidType ParameterType
        => GrammarAttribute.IsCached(in parameterTypeCached) ? parameterType!
            : this.Synthetic(ref parameterTypeCached, ref parameterType,
                NameBindingTypesAspect.SelfParameter_ParameterType);
    private IMaybeNonVoidType? parameterType;
    private bool parameterTypeCached;

    public ConstructorSelfParameterNode(
        IConstructorSelfParameterSyntax syntax,
        ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        NameBindingTypesAspect.ConstructorSelfParameter_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSelfParameterNode : SemanticNode, IInitializerSelfParameterNode
{
    private IInitializerSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IInitializerSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public OrdinaryTypeConstructor ContainingTypeConstructor
        => GrammarAttribute.IsCached(in containingTypeConstructorCached) ? containingTypeConstructor!
            : this.Inherited(ref containingTypeConstructorCached, ref containingTypeConstructor,
                Inherited_ContainingTypeConstructor);
    private OrdinaryTypeConstructor? containingTypeConstructor;
    private bool containingTypeConstructorCached;
    public SelfTypeConstructor ContainingSelfTypeConstructor
        => GrammarAttribute.IsCached(in containingSelfTypeConstructorCached) ? containingSelfTypeConstructor!
            : this.Inherited(ref containingSelfTypeConstructorCached, ref containingSelfTypeConstructor,
                Inherited_ContainingSelfTypeConstructor);
    private SelfTypeConstructor? containingSelfTypeConstructor;
    private bool containingSelfTypeConstructorCached;
    public ConstructedPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.SelfParameter_BindingPlainType);
    private ConstructedPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public CapabilityType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.InitializerSelfParameter_BindingType);
    private CapabilityType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SelfParameter_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeNonVoidType ParameterType
        => GrammarAttribute.IsCached(in parameterTypeCached) ? parameterType!
            : this.Synthetic(ref parameterTypeCached, ref parameterType,
                NameBindingTypesAspect.SelfParameter_ParameterType);
    private IMaybeNonVoidType? parameterType;
    private bool parameterTypeCached;

    public InitializerSelfParameterNode(
        IInitializerSelfParameterSyntax syntax,
        ICapabilityNode capability)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        NameBindingTypesAspect.InitializerSelfParameter_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodSelfParameterNode : SemanticNode, IMethodSelfParameterNode
{
    private IMethodSelfParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IMethodSelfParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityConstraintNode Constraint { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public OrdinaryTypeConstructor ContainingTypeConstructor
        => GrammarAttribute.IsCached(in containingTypeConstructorCached) ? containingTypeConstructor!
            : this.Inherited(ref containingTypeConstructorCached, ref containingTypeConstructor,
                Inherited_ContainingTypeConstructor);
    private OrdinaryTypeConstructor? containingTypeConstructor;
    private bool containingTypeConstructorCached;
    public SelfTypeConstructor ContainingSelfTypeConstructor
        => GrammarAttribute.IsCached(in containingSelfTypeConstructorCached) ? containingSelfTypeConstructor!
            : this.Inherited(ref containingSelfTypeConstructorCached, ref containingSelfTypeConstructor,
                Inherited_ContainingSelfTypeConstructor);
    private SelfTypeConstructor? containingSelfTypeConstructor;
    private bool containingSelfTypeConstructorCached;
    public ConstructedPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.SelfParameter_BindingPlainType);
    private ConstructedPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.MethodSelfParameter_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SelfParameter_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeNonVoidType ParameterType
        => GrammarAttribute.IsCached(in parameterTypeCached) ? parameterType!
            : this.Synthetic(ref parameterTypeCached, ref parameterType,
                NameBindingTypesAspect.SelfParameter_ParameterType);
    private IMaybeNonVoidType? parameterType;
    private bool parameterTypeCached;

    public MethodSelfParameterNode(
        IMethodSelfParameterSyntax syntax,
        ICapabilityConstraintNode constraint)
    {
        Syntax = syntax;
        Constraint = Child.Attach(this, constraint);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        NameBindingTypesAspect.MethodSelfParameter_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldParameterNode : SemanticNode, IFieldParameterNode
{
    private IFieldParameterNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IFieldParameterSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDefinitionNode ContainingTypeDefinition
        => GrammarAttribute.IsCached(in containingTypeDefinitionCached) ? containingTypeDefinition!
            : this.Inherited(ref containingTypeDefinitionCached, ref containingTypeDefinition,
                Inherited_ContainingTypeDefinition);
    private ITypeDefinitionNode? containingTypeDefinition;
    private bool containingTypeDefinitionCached;
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.FieldParameter_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.FieldParameter_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public IMaybeParameterType ParameterType
        => GrammarAttribute.IsCached(in parameterTypeCached) ? parameterType!
            : this.Synthetic(ref parameterTypeCached, ref parameterType,
                NameBindingTypesAspect.FieldParameter_ParameterType);
    private IMaybeParameterType? parameterType;
    private bool parameterTypeCached;
    public IFieldDefinitionNode? ReferencedField
        => GrammarAttribute.IsCached(in referencedFieldCached) ? referencedField
            : this.Synthetic(ref referencedFieldCached, ref referencedField,
                BindingNamesAspect.FieldParameter_ReferencedField);
    private IFieldDefinitionNode? referencedField;
    private bool referencedFieldCached;

    public FieldParameterNode(IFieldParameterSyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockBodyNode : SemanticNode, IBlockBodyNode
{
    private IBlockBodyNode Self { [Inline] get => this; }

    public IBlockBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IBodyStatementNode> Statements { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public BlockBodyNode(
        IBlockBodySyntax syntax,
        IEnumerable<IBodyStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } index)
            return index < Statements.Count - 1 ? ControlFlowSet.CreateNormal(Statements[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.Statements.Count && ReferenceEquals(child, Self.Statements[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.Statements, child) is { } index)
            return Statements[index - 1].FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionBodyNode : SemanticNode, IExpressionBodyNode
{
    private IExpressionBodyNode Self { [Inline] get => this; }

    public IExpressionBodySyntax Syntax { [DebuggerStepThrough] get; }
    public IResultStatementNode ResultStatement { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;

    public ExpressionBodyNode(
        IExpressionBodySyntax syntax,
        IResultStatementNode resultStatement)
    {
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.ResultStatement))
            return ExpectedPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.ResultStatement))
            return ExpectedType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.ResultStatement))
            return true;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.ResultStatement))
            return true;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierTypeNameNode : SemanticNode, IIdentifierTypeNameNode
{
    private IIdentifierTypeNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IIdentifierTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
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
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType,
                BareTypeAspect.IdentifierTypeName_NamedBareType);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.IdentifierTypeName_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.TypeName_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                BindingNamesAspect.StandardTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;

    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        BindingNamesAspect.StandardTypeName_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BuiltInTypeNameNode : SemanticNode, IBuiltInTypeNameNode
{
    private IBuiltInTypeNameNode Self { [Inline] get => this; }

    public IBuiltInTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
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
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType,
                BareTypeAspect.BuiltInTypeName_NamedBareType);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.BuiltInTypeName_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.BuiltInTypeName_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                BindingNamesAspect.BuiltInTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;

    public BuiltInTypeNameNode(IBuiltInTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericTypeNameNode : SemanticNode, IGenericTypeNameNode
{
    private IGenericTypeNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IGenericTypeNameSyntax Syntax { [DebuggerStepThrough] get; }
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
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType,
                BareTypeAspect.GenericTypeName_NamedBareType);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.GenericTypeName_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.TypeName_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                BindingNamesAspect.StandardTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;

    public GenericTypeNameNode(
        IGenericTypeNameSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        BindingNamesAspect.StandardTypeName_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameNode : SemanticNode, IQualifiedTypeNameNode
{
    private IQualifiedTypeNameNode Self { [Inline] get => this; }

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
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.TypeName_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                BindingNamesAspect.QualifiedTypeName_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;

    public QualifiedTypeNameNode(
        IQualifiedTypeNameSyntax syntax,
        ITypeNameNode context,
        IStandardTypeNameNode qualifiedName)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        QualifiedName = Child.Attach(this, qualifiedName);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalTypeNode : SemanticNode, IOptionalTypeNode
{
    private IOptionalTypeNode Self { [Inline] get => this; }

    public IOptionalTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.OptionalType_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.OptionalType_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;

    public OptionalTypeNode(
        IOptionalTypeSyntax syntax,
        ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityTypeNode : SemanticNode, ICapabilityTypeNode
{
    private ICapabilityTypeNode Self { [Inline] get => this; }

    public ICapabilityTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.CapabilityType_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.CapabilityType_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;

    public CapabilityTypeNode(
        ICapabilityTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeExpressionsAspect.CapabilityType_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionTypeNode : SemanticNode, IFunctionTypeNode
{
    private IFunctionTypeNode Self { [Inline] get => this; }

    public IFunctionTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IParameterTypeNode> Parameters { [DebuggerStepThrough] get; }
    public ITypeNode Return { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.FunctionType_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.FunctionType_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;

    public FunctionTypeNode(
        IFunctionTypeSyntax syntax,
        IEnumerable<IParameterTypeNode> parameters,
        ITypeNode @return)
    {
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
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeParameterType Parameter
        => GrammarAttribute.IsCached(in parameterCached) ? parameter!
            : this.Synthetic(ref parameterCached, ref parameter,
                TypeExpressionsAspect.ParameterType_Parameter);
    private IMaybeParameterType? parameter;
    private bool parameterCached;

    public ParameterTypeNode(
        IParameterTypeSyntax syntax,
        ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityViewpointTypeNode : SemanticNode, ICapabilityViewpointTypeNode
{
    private ICapabilityViewpointTypeNode Self { [Inline] get => this; }

    public ICapabilityViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode Capability { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.ViewpointType_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.CapabilityViewpointType_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;

    public CapabilityViewpointTypeNode(
        ICapabilityViewpointTypeSyntax syntax,
        ICapabilityNode capability,
        ITypeNode referent)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Referent = Child.Attach(this, referent);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeExpressionsAspect.CapabilityViewpointType_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfViewpointTypeNode : SemanticNode, ISelfViewpointTypeNode
{
    private ISelfViewpointTypeNode Self { [Inline] get => this; }

    public ISelfViewpointTypeSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNode Referent { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? MethodSelfType
        => GrammarAttribute.IsCached(in methodSelfTypeCached) ? methodSelfType
            : this.Inherited(ref methodSelfTypeCached, ref methodSelfType,
                Inherited_MethodSelfType);
    private IMaybeType? methodSelfType;
    private bool methodSelfTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.ViewpointType_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public IMaybeType NamedType
        => GrammarAttribute.IsCached(in namedTypeCached) ? namedType!
            : this.Synthetic(ref namedTypeCached, ref namedType,
                TypeExpressionsAspect.SelfViewpointType_NamedType);
    private IMaybeType? namedType;
    private bool namedTypeCached;

    public SelfViewpointTypeNode(
        ISelfViewpointTypeSyntax syntax,
        ITypeNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        TypeExpressionsAspect.SelfViewpointType_Contribute_Diagnostics(this, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class EntryNode : SemanticNode, IEntryNode
{
    private IEntryNode Self { [Inline] get => this; }

    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap()
        => Inherited_VariableBindingsMap(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Entry_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned!
            : this.Synthetic(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.Entry_DefinitelyAssigned);
    private BindingFlags<IVariableBindingNode>? definitelyAssigned;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned!
            : this.Synthetic(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.Entry_DefinitelyUnassigned);
    private BindingFlags<IVariableBindingNode>? definitelyUnassigned;
    private bool definitelyUnassignedCached;

    public EntryNode()
    {
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExitNode : SemanticNode, IExitNode
{
    private IExitNode Self { [Inline] get => this; }

    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.Exit_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.Exit_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;

    public ExitNode()
    {
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ResultStatementNode : SemanticNode, IResultStatementNode
{
    private IResultStatementNode Self { [Inline] get => this; }

    public IResultStatementSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ResultStatement_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.ResultStatement_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ResultStatement_Type);
    private IMaybeType? type;
    private bool typeCached;

    public ResultStatementNode(
        IResultStatementSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return ExpectedPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return ExpectedType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return ImplicitRecoveryAllowed();
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return ShouldPrepareToReturn();
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableDeclarationStatementNode : SemanticNode, IVariableDeclarationStatementNode
{
    private IVariableDeclarationStatementNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IVariableDeclarationStatementSyntax Syntax { [DebuggerStepThrough] get; }
    public ICapabilityNode? Capability { [DebuggerStepThrough] get; }
    public ITypeNode? Type { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode?> initializer;
    private bool initializerCached;
    public IAmbiguousExpressionNode? TempInitializer
        => GrammarAttribute.IsCached(in initializerCached) ? initializer.UnsafeValue
            : this.RewritableChild(ref initializerCached, ref initializer);
    public IExpressionNode? Initializer => TempInitializer as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentInitializer => initializer.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.VariableDeclarationStatement_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.VariableDeclarationStatement_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.VariableDeclarationStatement_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.VariableDeclarationStatement_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.VariableDeclarationStatement_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.VariableDeclarationStatement_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                NameBindingTypesAspect.VariableDeclarationStatement_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.VariableDeclarationStatement_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;

    public VariableDeclarationStatementNode(
        IVariableDeclarationStatementSyntax syntax,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Type = Child.Attach(this, type);
        this.initializer = Child.Create(this, initializer);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentInitializer))
            return BindingPlainType;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentInitializer))
            return BindingType;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentInitializer))
            return true;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentInitializer))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ShadowingAspect.VariableBinding_Contribute_Diagnostics(this, builder);
        NameBindingPlainTypesAspect.VariableDeclarationStatement_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionStatementNode : SemanticNode, IExpressionStatementNode
{
    private IExpressionStatementNode Self { [Inline] get => this; }

    public IExpressionStatementSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ExpressionStatement_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ExpressionStatement_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;

    public ExpressionStatementNode(
        IExpressionStatementSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingContextPatternNode : SemanticNode, IBindingContextPatternNode
{
    private IBindingContextPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IBindingContextPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public IPatternNode Pattern { [DebuggerStepThrough] get; }
    public ITypeNode? Type { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeNonVoidPlainType ContextBindingPlainType()
        => Inherited_ContextBindingPlainType(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeNonVoidType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.BindingContextPattern_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;

    public BindingContextPatternNode(
        IBindingContextPatternSyntax syntax,
        IPatternNode pattern,
        ITypeNode? type)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
        Type = Child.Attach(this, type);
    }

    internal override IMaybeNonVoidPlainType Inherited_ContextBindingPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingPlainTypesAspect.BindingContextPattern_Pattern_ContextBindingPlainType(this);
        return base.Inherited_ContextBindingPlainType(child, descendant, ctx);
    }

    internal override IMaybeNonVoidType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingTypesAspect.BindingContextPattern_Pattern_ContextBindingType(this);
        return base.Inherited_ContextBindingType(child, descendant, ctx);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingPatternNode : SemanticNode, IBindingPatternNode
{
    private IBindingPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IBindingPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeNonVoidPlainType ContextBindingPlainType()
        => Inherited_ContextBindingPlainType(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeNonVoidType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.BindingPattern_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.BindingPattern_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.BindingPattern_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.BindingPattern_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.BindingPattern_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.BindingPattern_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                NameBindingTypesAspect.BindingPattern_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;

    public BindingPatternNode(IBindingPatternSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ShadowingAspect.VariableBinding_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalPatternNode : SemanticNode, IOptionalPatternNode
{
    private IOptionalPatternNode Self { [Inline] get => this; }
    private AttributeLock syncLock;

    public IOptionalPatternSyntax Syntax { [DebuggerStepThrough] get; }
    public IOptionalOrBindingPatternNode Pattern { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public ValueId? MatchReferentValueId
        => GrammarAttribute.IsCached(in matchReferentValueIdCached) ? matchReferentValueId
            : this.Inherited(ref matchReferentValueIdCached, ref matchReferentValueId, ref syncLock,
                Inherited_MatchReferentValueId);
    private ValueId? matchReferentValueId;
    private bool matchReferentValueIdCached;
    public IMaybeNonVoidPlainType ContextBindingPlainType()
        => Inherited_ContextBindingPlainType(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeNonVoidType ContextBindingType()
        => Inherited_ContextBindingType(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.OptionalPattern_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;

    public OptionalPatternNode(
        IOptionalPatternSyntax syntax,
        IOptionalOrBindingPatternNode pattern)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
    }

    internal override IMaybeNonVoidPlainType Inherited_ContextBindingPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingPlainTypesAspect.OptionalPattern_Pattern_ContextBindingPlainType(this);
        return base.Inherited_ContextBindingPlainType(child, descendant, ctx);
    }

    internal override IMaybeNonVoidType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingTypesAspect.OptionalPattern_Pattern_ContextBindingType(this);
        return base.Inherited_ContextBindingType(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionPlainTypesAspect.OptionalPattern_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockExpressionNode : SemanticNode, IBlockExpressionNode
{
    private IBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IBlockExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<IStatementNode> Statements { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.BlockExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BlockExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.BlockExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.BlockExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BlockExpressionNode(
        IBlockExpressionSyntax syntax,
        IEnumerable<IStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } statementIndex)
            return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.Statements, child) is { } index)
            return index < Statements.Count - 1 ? ControlFlowSet.CreateNormal(Statements[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.Statements.Count && ReferenceEquals(child, Self.Statements[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.Statements, child) is { } index)
            return Statements[index - 1].FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        InvalidStructureAspect.BlockExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NewObjectExpressionNode : SemanticNode, INewObjectExpressionNode
{
    private INewObjectExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INewObjectExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public ITypeNameNode ConstructingType { [DebuggerStepThrough] get; }
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors
        => GrammarAttribute.IsCached(in compatibleConstructorsCached) ? compatibleConstructors!
            : this.Synthetic(ref compatibleConstructorsCached, ref compatibleConstructors,
                OverloadResolutionAspect.NewObjectExpression_CompatibleConstructors);
    private IFixedSet<IConstructorDeclarationNode>? compatibleConstructors;
    private bool compatibleConstructorsCached;
    public IMaybeNonVoidPlainType ConstructingPlainType
        => GrammarAttribute.IsCached(in constructingPlainTypeCached) ? constructingPlainType!
            : this.Synthetic(ref constructingPlainTypeCached, ref constructingPlainType,
                NameBindingPlainTypesAspect.NewObjectExpression_ConstructingPlainType);
    private IMaybeNonVoidPlainType? constructingPlainType;
    private bool constructingPlainTypeCached;
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.NewObjectExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.NewObjectExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.NewObjectExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.NewObjectExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IConstructorDeclarationNode? ReferencedConstructor
        => GrammarAttribute.IsCached(in referencedConstructorCached) ? referencedConstructor
            : this.Synthetic(ref referencedConstructorCached, ref referencedConstructor,
                OverloadResolutionAspect.NewObjectExpression_ReferencedConstructor);
    private IConstructorDeclarationNode? referencedConstructor;
    private bool referencedConstructorCached;
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors
        => GrammarAttribute.IsCached(in referencedConstructorsCached) ? referencedConstructors!
            : this.Synthetic(ref referencedConstructorsCached, ref referencedConstructors,
                BindingNamesAspect.NewObjectExpression_ReferencedConstructors);
    private IFixedSet<IConstructorDeclarationNode>? referencedConstructors;
    private bool referencedConstructorsCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.NewObjectExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NewObjectExpressionNode(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode constructingType,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        ConstructingType = Child.Attach(this, constructingType);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.ConstructingType))
            return ContainingLexicalScope();
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return ContainingLexicalScope();
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return TempArguments[index - 1].FlowLexicalScope().True;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return index < Arguments.Count - 1 ? ControlFlowSet.CreateNormal(Arguments[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return ContextualizedCall?.ParameterTypes[index].Type.PlainType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return ContextualizedCall?.ParameterTypes[index].Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.NewObjectExpression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.NewObjectExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnsafeExpressionNode : SemanticNode, IUnsafeExpressionNode
{
    private IUnsafeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IUnsafeExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.UnsafeExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.UnsafeExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.UnsafeExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.UnsafeExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnsafeExpressionNode(
        IUnsafeExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BoolLiteralExpressionNode : SemanticNode, IBoolLiteralExpressionNode
{
    private IBoolLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IBoolLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.LiteralExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.BoolLiteralExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public CapabilityType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.BoolLiteralExpression_Type);
    private CapabilityType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BoolLiteralExpressionNode(IBoolLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IntegerLiteralExpressionNode : SemanticNode, IIntegerLiteralExpressionNode
{
    private IIntegerLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIntegerLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.LiteralExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.IntegerLiteralExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public CapabilityType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.IntegerLiteralExpression_Type);
    private CapabilityType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IntegerLiteralExpressionNode(IIntegerLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NoneLiteralExpressionNode : SemanticNode, INoneLiteralExpressionNode
{
    private INoneLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INoneLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.LiteralExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.NoneLiteralExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public OptionalType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.NoneLiteralExpression_Type);
    private OptionalType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NoneLiteralExpressionNode(INoneLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StringLiteralExpressionNode : SemanticNode, IStringLiteralExpressionNode
{
    private IStringLiteralExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IStringLiteralExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.LiteralExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.StringLiteralExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.StringLiteralExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public StringLiteralExpressionNode(IStringLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.StringLiteralExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssignmentExpressionNode : SemanticNode, IAssignmentExpressionNode
{
    private IAssignmentExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IAssignmentExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> leftOperand;
    private bool leftOperandCached;
    public IAmbiguousExpressionNode TempLeftOperand
        => GrammarAttribute.IsCached(in leftOperandCached) ? leftOperand.UnsafeValue
            : this.RewritableChild(ref leftOperandCached, ref leftOperand);
    public IExpressionNode? LeftOperand => TempLeftOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentLeftOperand => leftOperand.UnsafeValue;
    private RewritableChild<IAmbiguousExpressionNode> rightOperand;
    private bool rightOperandCached;
    public IAmbiguousExpressionNode TempRightOperand
        => GrammarAttribute.IsCached(in rightOperandCached) ? rightOperand.UnsafeValue
            : this.RewritableChild(ref rightOperandCached, ref rightOperand);
    public IExpressionNode? RightOperand => TempRightOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentRightOperand => rightOperand.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.AssignmentExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.AssignmentExpression_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.AssignmentExpression_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.AssignmentExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.AssignmentExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.AssignmentExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AssignmentExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentLeftOperand))
            return ControlFlowSet.CreateNormal(RightOperand);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentRightOperand))
            return LeftOperand?.PlainType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentRightOperand))
            return LeftOperand?.Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentRightOperand))
            return LeftOperand?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.AssignmentExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.AssignmentExpression_Rewrite_PropertyNameLeftOperand(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BinaryOperatorExpressionNode : SemanticNode, IBinaryOperatorExpressionNode
{
    private IBinaryOperatorExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IBinaryOperatorExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> leftOperand;
    private bool leftOperandCached;
    public IAmbiguousExpressionNode TempLeftOperand
        => GrammarAttribute.IsCached(in leftOperandCached) ? leftOperand.UnsafeValue
            : this.RewritableChild(ref leftOperandCached, ref leftOperand);
    public IExpressionNode? LeftOperand => TempLeftOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentLeftOperand => leftOperand.UnsafeValue;
    private RewritableChild<IAmbiguousExpressionNode> rightOperand;
    private bool rightOperandCached;
    public IAmbiguousExpressionNode TempRightOperand
        => GrammarAttribute.IsCached(in rightOperandCached) ? rightOperand.UnsafeValue
            : this.RewritableChild(ref rightOperandCached, ref rightOperand);
    public IExpressionNode? RightOperand => TempRightOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentRightOperand => rightOperand.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.BinaryOperatorExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BinaryOperatorExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public PlainType? NumericOperatorCommonPlainType
        => GrammarAttribute.IsCached(in numericOperatorCommonPlainTypeCached) ? numericOperatorCommonPlainType
            : this.Synthetic(ref numericOperatorCommonPlainTypeCached, ref numericOperatorCommonPlainType,
                ExpressionPlainTypesAspect.BinaryOperatorExpression_NumericOperatorCommonPlainType);
    private PlainType? numericOperatorCommonPlainType;
    private bool numericOperatorCommonPlainTypeCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.BinaryOperatorExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.BinaryOperatorExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BinaryOperatorExpressionNode(
        IBinaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode leftOperand,
        IAmbiguousExpressionNode rightOperand)
    {
        Syntax = syntax;
        this.leftOperand = Child.Create(this, leftOperand);
        this.rightOperand = Child.Create(this, rightOperand);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentLeftOperand))
            return ContainingLexicalScope;
        if (ReferenceEquals(child, Self.CurrentRightOperand))
            return LexicalScopingAspect.BinaryOperatorExpression_RightOperand_Broadcast_ContainingLexicalScope(this);
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentLeftOperand))
            return ControlFlowSet.CreateNormal(RightOperand);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentLeftOperand))
            return NumericOperatorCommonPlainType;
        if (ReferenceEquals(descendant, Self.CurrentRightOperand))
            return NumericOperatorCommonPlainType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentRightOperand))
            return LeftOperand?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.BinaryOperatorExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnaryOperatorExpressionNode : SemanticNode, IUnaryOperatorExpressionNode
{
    private IUnaryOperatorExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IUnaryOperatorExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> operand;
    private bool operandCached;
    public IAmbiguousExpressionNode TempOperand
        => GrammarAttribute.IsCached(in operandCached) ? operand.UnsafeValue
            : this.RewritableChild(ref operandCached, ref operand);
    public IExpressionNode? Operand => TempOperand as IExpressionNode;
    public IAmbiguousExpressionNode CurrentOperand => operand.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.UnaryOperatorExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.UnaryOperatorExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.UnaryOperatorExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.UnaryOperatorExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnaryOperatorExpressionNode(
        IUnaryOperatorExpressionSyntax syntax,
        IAmbiguousExpressionNode operand)
    {
        Syntax = syntax;
        this.operand = Child.Create(this, operand);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionPlainTypesAspect.UnaryOperatorExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConversionExpressionNode : SemanticNode, IConversionExpressionNode
{
    private IConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IConversionExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public IAmbiguousExpressionNode CurrentReferent => referent.UnsafeValue;
    public ITypeNode ConvertToType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ConversionExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ConversionExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.ConversionExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ConversionExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ConversionExpressionNode(
        IConversionExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        ITypeNode convertToType)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        ConvertToType = Child.Attach(this, convertToType);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.ConversionExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitConversionExpressionNode : SemanticNode, IImplicitConversionExpressionNode
{
    private IImplicitConversionExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public ConstructedPlainType PlainType { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ImplicitConversionExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ImplicitConversionExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ImplicitConversionExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitConversionExpressionNode(
        IExpressionNode referent,
        ConstructedPlainType plainType)
    {
        this.referent = Child.Create(this, referent);
        PlainType = plainType;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentReferent))
            return null;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentReferent))
            return null;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PatternMatchExpressionNode : SemanticNode, IPatternMatchExpressionNode
{
    private IPatternMatchExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IPatternMatchExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public IAmbiguousExpressionNode CurrentReferent => referent.UnsafeValue;
    public IPatternNode Pattern { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.PatternMatchExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.PatternMatchExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PatternMatchExpressionNode(
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        Pattern = Child.Attach(this, pattern);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentReferent))
            return ContainingLexicalScope();
        if (ReferenceEquals(child, Self.Pattern))
            return TempReferent.FlowLexicalScope().True;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IMaybeNonVoidPlainType Inherited_ContextBindingPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingPlainTypesAspect.PatternMatchExpression_Pattern_ContextBindingPlainType(this);
        return base.Inherited_ContextBindingPlainType(child, descendant, ctx);
    }

    internal override IMaybeNonVoidType Inherited_ContextBindingType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.Pattern))
            return NameBindingTypesAspect.PatternMatchExpression_Pattern_ContextBindingType(this);
        return base.Inherited_ContextBindingType(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentReferent))
            return ControlFlowSet.CreateNormal(Pattern);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Pattern))
            return Referent?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override ValueId? Inherited_MatchReferentValueId(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.Pattern))
            return Referent?.ValueId;
        return base.Inherited_MatchReferentValueId(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IfExpressionNode : SemanticNode, IIfExpressionNode
{
    private IIfExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIfExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> condition;
    private bool conditionCached;
    public IAmbiguousExpressionNode TempCondition
        => GrammarAttribute.IsCached(in conditionCached) ? condition.UnsafeValue
            : this.RewritableChild(ref conditionCached, ref condition);
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    public IAmbiguousExpressionNode CurrentCondition => condition.UnsafeValue;
    private RewritableChild<IBlockOrResultNode> thenBlock;
    private bool thenBlockCached;
    public IBlockOrResultNode ThenBlock
        => GrammarAttribute.IsCached(in thenBlockCached) ? thenBlock.UnsafeValue
            : this.RewritableChild(ref thenBlockCached, ref thenBlock);
    public IBlockOrResultNode CurrentThenBlock => thenBlock.UnsafeValue;
    private RewritableChild<IElseClauseNode?> elseClause;
    private bool elseClauseCached;
    public IElseClauseNode? ElseClause
        => GrammarAttribute.IsCached(in elseClauseCached) ? elseClause.UnsafeValue
            : this.RewritableChild(ref elseClauseCached, ref elseClause);
    public IElseClauseNode? CurrentElseClause => elseClause.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.IfExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.IfExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.IfExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.IfExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IfExpressionNode(
        IIfExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        this.thenBlock = Child.Create(this, thenBlock);
        this.elseClause = Child.Create(this, elseClause);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentThenBlock))
            return TempCondition.FlowLexicalScope().True;
        if (ReferenceEquals(child, Self.CurrentElseClause))
            return TempCondition.FlowLexicalScope().False;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentCondition))
            return CurrentElseClause is not null ? ControlFlowSet.CreateNormal(ThenBlock, ElseClause!) : ControlFlowSet.CreateNormal(ThenBlock).Union(ControlFlowFollowing());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentCondition))
            return AzothPlainType.OptionalBool;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentCondition))
            return AzothType.OptionalBool;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentThenBlock))
            return Condition?.FlowStateAfter ?? IFlowState.Empty;
        if (ReferenceEquals(child, Self.CurrentElseClause))
            return Condition?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.IfExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class LoopExpressionNode : SemanticNode, ILoopExpressionNode
{
    private ILoopExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public ILoopExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.LoopExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.LoopExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.LoopExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.LoopExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public LoopExpressionNode(
        ILoopExpressionSyntax syntax,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.block = Child.Create(this, block);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentBlock))
            return ControlFlowSet.CreateLoop(CurrentBlock).Union(ControlFlowFollowing());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class WhileExpressionNode : SemanticNode, IWhileExpressionNode
{
    private IWhileExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IWhileExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> condition;
    private bool conditionCached;
    public IAmbiguousExpressionNode TempCondition
        => GrammarAttribute.IsCached(in conditionCached) ? condition.UnsafeValue
            : this.RewritableChild(ref conditionCached, ref condition);
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    public IAmbiguousExpressionNode CurrentCondition => condition.UnsafeValue;
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.WhileExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.WhileExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.WhileExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.WhileExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public WhileExpressionNode(
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        this.block = Child.Create(this, block);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentBlock))
            return TempCondition.FlowLexicalScope().True;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentCondition))
            return ControlFlowSet.CreateNormal(Block).Union(ControlFlowFollowing());
        if (ReferenceEquals(child, Self.CurrentBlock))
            return ControlFlowSet.CreateLoop(Condition).Union(ControlFlowFollowing());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentCondition))
            return AzothPlainType.OptionalBool;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentCondition))
            return AzothType.OptionalBool;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentBlock))
            return Condition?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ForeachExpressionNode : SemanticNode, IForeachExpressionNode
{
    private IForeachExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IForeachExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> inExpression;
    private bool inExpressionCached;
    public IAmbiguousExpressionNode TempInExpression
        => GrammarAttribute.IsCached(in inExpressionCached) ? inExpression.UnsafeValue
            : this.RewritableChild(ref inExpressionCached, ref inExpression);
    public IExpressionNode? InExpression => TempInExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentInExpression => inExpression.UnsafeValue;
    public ITypeNode? DeclaredType { [DebuggerStepThrough] get; }
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IMaybeNonVoidPlainType BindingPlainType
        => GrammarAttribute.IsCached(in bindingPlainTypeCached) ? bindingPlainType!
            : this.Synthetic(ref bindingPlainTypeCached, ref bindingPlainType,
                NameBindingPlainTypesAspect.ForeachExpression_BindingPlainType);
    private IMaybeNonVoidPlainType? bindingPlainType;
    private bool bindingPlainTypeCached;
    public IMaybeNonVoidType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType,
                NameBindingTypesAspect.ForeachExpression_BindingType);
    private IMaybeNonVoidType? bindingType;
    private bool bindingTypeCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ValueIdsAspect.ForeachExpression_BindingValueId);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ForeachExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.ForeachExpression_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.ForeachExpression_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ForeachExpressionTypesAspect.ForeachExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IFlowState FlowStateBeforeBlock
        => GrammarAttribute.IsCached(in flowStateBeforeBlockCached) ? flowStateBeforeBlock!
            : this.Synthetic(ref flowStateBeforeBlockCached, ref flowStateBeforeBlock,
                ForeachExpressionTypesAspect.ForeachExpression_FlowStateBeforeBlock);
    private IFlowState? flowStateBeforeBlock;
    private bool flowStateBeforeBlockCached;
    public IMaybeNonVoidPlainType IteratedPlainType
        => GrammarAttribute.IsCached(in iteratedPlainTypeCached) ? iteratedPlainType!
            : this.Synthetic(ref iteratedPlainTypeCached, ref iteratedPlainType,
                ForeachExpressionPlainTypesAspect.ForeachExpression_IteratedPlainType);
    private IMaybeNonVoidPlainType? iteratedPlainType;
    private bool iteratedPlainTypeCached;
    public IMaybeNonVoidType IteratedType
        => GrammarAttribute.IsCached(in iteratedTypeCached) ? iteratedType!
            : this.Synthetic(ref iteratedTypeCached, ref iteratedType,
                ForeachExpressionTypesAspect.ForeachExpression_IteratedType);
    private IMaybeNonVoidType? iteratedType;
    private bool iteratedTypeCached;
    public IMaybeNonVoidPlainType IteratorPlainType
        => GrammarAttribute.IsCached(in iteratorPlainTypeCached) ? iteratorPlainType!
            : this.Synthetic(ref iteratorPlainTypeCached, ref iteratorPlainType,
                ForeachExpressionPlainTypesAspect.ForeachExpression_IteratorPlainType);
    private IMaybeNonVoidPlainType? iteratorPlainType;
    private bool iteratorPlainTypeCached;
    public IMaybeNonVoidType IteratorType
        => GrammarAttribute.IsCached(in iteratorTypeCached) ? iteratorType!
            : this.Synthetic(ref iteratorTypeCached, ref iteratorType,
                ForeachExpressionTypesAspect.ForeachExpression_IteratorType);
    private IMaybeNonVoidType? iteratorType;
    private bool iteratorTypeCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ForeachExpression_LexicalScope);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.ForeachExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public ITypeDeclarationNode? ReferencedIterableDeclaration
        => GrammarAttribute.IsCached(in referencedIterableDeclarationCached) ? referencedIterableDeclaration
            : this.Synthetic(ref referencedIterableDeclarationCached, ref referencedIterableDeclaration,
                ForeachExpressionPlainTypesAspect.ForeachExpression_ReferencedIterableDeclaration);
    private ITypeDeclarationNode? referencedIterableDeclaration;
    private bool referencedIterableDeclarationCached;
    public IOrdinaryMethodDeclarationNode? ReferencedIterateMethod
        => GrammarAttribute.IsCached(in referencedIterateMethodCached) ? referencedIterateMethod
            : this.Synthetic(ref referencedIterateMethodCached, ref referencedIterateMethod,
                ForeachExpressionPlainTypesAspect.ForeachExpression_ReferencedIterateMethod);
    private IOrdinaryMethodDeclarationNode? referencedIterateMethod;
    private bool referencedIterateMethodCached;
    public ITypeDeclarationNode? ReferencedIteratorDeclaration
        => GrammarAttribute.IsCached(in referencedIteratorDeclarationCached) ? referencedIteratorDeclaration
            : this.Synthetic(ref referencedIteratorDeclarationCached, ref referencedIteratorDeclaration,
                ForeachExpressionPlainTypesAspect.ForeachExpression_ReferencedIteratorDeclaration);
    private ITypeDeclarationNode? referencedIteratorDeclaration;
    private bool referencedIteratorDeclarationCached;
    public IOrdinaryMethodDeclarationNode? ReferencedNextMethod
        => GrammarAttribute.IsCached(in referencedNextMethodCached) ? referencedNextMethod
            : this.Synthetic(ref referencedNextMethodCached, ref referencedNextMethod,
                ForeachExpressionPlainTypesAspect.ForeachExpression_ReferencedNextMethod);
    private IOrdinaryMethodDeclarationNode? referencedNextMethod;
    private bool referencedNextMethodCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ForeachExpressionTypesAspect.ForeachExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ForeachExpressionNode(
        IForeachExpressionSyntax syntax,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? declaredType,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.inExpression = Child.Create(this, inExpression);
        DeclaredType = Child.Attach(this, declaredType);
        this.block = Child.Create(this, block);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentInExpression))
            return ContainingLexicalScope;
        if (ReferenceEquals(child, Self.CurrentBlock))
            return LexicalScope;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentInExpression))
            return ControlFlowSet.CreateNormal(Block);
        if (ReferenceEquals(child, Self.CurrentBlock))
            return ControlFlowSet.CreateLoop(Block).Union(ControlFlowFollowing());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentBlock))
            return FlowStateBeforeBlock;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ShadowingAspect.VariableBinding_Contribute_Diagnostics(this, builder);
        ForeachExpressionTypesAspect.ForeachExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BreakExpressionNode : SemanticNode, IBreakExpressionNode
{
    private IBreakExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IBreakExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode?> value;
    private bool valueCached;
    public IAmbiguousExpressionNode? TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentValue => value.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BreakExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BreakExpressionNode(
        IBreakExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
    {
        Syntax = syntax;
        this.value = Child.Create(this, value);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NextExpressionNode : SemanticNode, INextExpressionNode
{
    private INextExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INextExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.NextExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public NextExpressionNode(INextExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ReturnExpressionNode : SemanticNode, IReturnExpressionNode
{
    private IReturnExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IReturnExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode?> value;
    private bool valueCached;
    public IAmbiguousExpressionNode? TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IAmbiguousExpressionNode? CurrentValue => value.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IMaybeType? ExpectedReturnType
        => GrammarAttribute.IsCached(in expectedReturnTypeCached) ? expectedReturnType
            : this.Inherited(ref expectedReturnTypeCached, ref expectedReturnType,
                Inherited_ExpectedReturnType);
    private IMaybeType? expectedReturnType;
    private bool expectedReturnTypeCached;
    public IExitNode ControlFlowExit()
        => Inherited_ControlFlowExit(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedReturnPlainType
        => GrammarAttribute.IsCached(in expectedReturnPlainTypeCached) ? expectedReturnPlainType
            : this.Inherited(ref expectedReturnPlainTypeCached, ref expectedReturnPlainType,
                Inherited_ExpectedReturnPlainType);
    private IMaybePlainType? expectedReturnPlainType;
    private bool expectedReturnPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ReturnExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ReturnExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ReturnExpressionNode(
        IReturnExpressionSyntax syntax,
        IAmbiguousExpressionNode? value)
    {
        Syntax = syntax;
        this.value = Child.Create(this, value);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentValue))
            return ControlFlowSet.CreateNormal(ControlFlowExit());
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return ExpectedReturnPlainType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return ExpectedReturnType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return true;
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return true;
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.ReturnExpression_Contribute_Diagnostics(this, builder);
        InvalidStructureAspect.ReturnExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownInvocationExpressionNode : SemanticNode, IUnknownInvocationExpressionNode
{
    private IUnknownInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.UnknownInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentExpression))
            return ContainingLexicalScope();
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return TempExpression.FlowLexicalScope().True;
        if (IndexOfNode(Self.CurrentArguments, child) is { } argumentIndex)
            return TempArguments[argumentIndex - 1].FlowLexicalScope().True;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return OverloadResolutionAspect.UnknownInvocationExpression_Expression_ExpectedPlainType(this);
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentExpression))
            return null;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return Expression?.FlowStateAfter ?? base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.UnknownInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => OverloadResolutionAspect.UnknownInvocationExpression_Rewrite_FunctionNameExpression(this)
        ?? OverloadResolutionAspect.UnknownInvocationExpression_Rewrite_MethodNameExpression(this)
        ?? OverloadResolutionAspect.UnknownInvocationExpression_Rewrite_TypeNameExpression(this)
        ?? OverloadResolutionAspect.UnknownInvocationExpression_Rewrite_InitializerGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnknownInvocationExpression_Rewrite_FunctionReferenceExpression(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionInvocationExpressionNode : SemanticNode, IFunctionInvocationExpressionNode
{
    private IFunctionInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IFunctionNameNode> function;
    private bool functionCached;
    public IFunctionNameNode Function
        => GrammarAttribute.IsCached(in functionCached) ? function.UnsafeValue
            : this.RewritableChild(ref functionCached, ref function);
    public IFunctionNameNode CurrentFunction => function.UnsafeValue;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.FunctionInvocationExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.FunctionInvocationExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FunctionInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.FunctionInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FunctionInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IFunctionNameNode function,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.function = Child.Create(this, function);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } argumentIndex)
            return TempArguments[argumentIndex - 1].FlowLexicalScope().True;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentFunction))
            return !TempArguments.IsEmpty ? ControlFlowSet.CreateNormal(Arguments[0]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return index < Arguments.Count - 1 ? ControlFlowSet.CreateNormal(Arguments[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return Self.SelectedCallCandidate?.ParameterPlainTypes[index];
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return ContextualizedCall?.ParameterTypes[index].Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.FunctionInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodInvocationExpressionNode : SemanticNode, IMethodInvocationExpressionNode
{
    private IMethodInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IMethodNameNode> method;
    private bool methodCached;
    public IMethodNameNode Method
        => GrammarAttribute.IsCached(in methodCached) ? method.UnsafeValue
            : this.RewritableChild(ref methodCached, ref method);
    public IMethodNameNode CurrentMethod => method.UnsafeValue;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.MethodInvocationExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.MethodInvocationExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MethodInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.MethodInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.MethodInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IMethodNameNode method,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.method = Child.Create(this, method);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentMethod))
            return !TempArguments.IsEmpty ? ControlFlowSet.CreateNormal(Arguments[0]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return index < Arguments.Count - 1 ? ControlFlowSet.CreateNormal(Arguments[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return Self.SelectedCallCandidate?.ParameterPlainTypes[index];
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return Method.FlowStateAfter;
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.MethodInvocationExpression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.MethodInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterInvocationExpressionNode : SemanticNode, IGetterInvocationExpressionNode
{
    private IGetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public OrdinaryName PropertyName { [DebuggerStepThrough] get; }
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.GetterInvocationExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.GetterInvocationExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.GetterInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IGetterMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.GetterInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public GetterInvocationExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName propertyName,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = BindingNamesAspect.GetterInvocationExpression_ReferencedDeclaration(this);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return Self.ReferencedDeclaration?.SelfParameterPlainType;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return ContextualizedCall?.SelfParameterType?.ToUpperBound();
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.GetterInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterInvocationExpressionNode : SemanticNode, ISetterInvocationExpressionNode
{
    private ISetterInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IAssignmentExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public OrdinaryName PropertyName { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> value;
    private bool valueCached;
    public IAmbiguousExpressionNode TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IAmbiguousExpressionNode CurrentValue => value.UnsafeValue;
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.SetterInvocationExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.SetterInvocationExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SetterInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.SetterInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public ISetterMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.SetterInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SetterInvocationExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        PropertyName = propertyName;
        this.value = Child.Create(this, value);
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = BindingNamesAspect.SetterInvocationExpression_ReferencedDeclaration(this);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentContext))
            return ControlFlowSet.CreateNormal(Value);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return Self.ReferencedDeclaration?.SelfParameterPlainType;
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return Self.ReferencedDeclaration?.ParameterPlainTypes[0];
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return ContextualizedCall?.SelfParameterType?.ToUpperBound();
        if (ReferenceEquals(descendant, Self.CurrentValue))
            return ContextualizedCall?.ParameterTypes[0].Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentValue))
            return Context.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.SetterInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionReferenceInvocationExpressionNode : SemanticNode, IFunctionReferenceInvocationExpressionNode
{
    private IFunctionReferenceInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> expression;
    private bool expressionCached;
    public IExpressionNode Expression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode CurrentExpression => expression.UnsafeValue;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.FunctionReferenceInvocationExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FunctionReferenceInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public FunctionPlainType FunctionPlainType
        => GrammarAttribute.IsCached(in functionPlainTypeCached) ? functionPlainType!
            : this.Synthetic(ref functionPlainTypeCached, ref functionPlainType,
                ExpressionPlainTypesAspect.FunctionReferenceInvocationExpression_FunctionPlainType);
    private FunctionPlainType? functionPlainType;
    private bool functionPlainTypeCached;
    public FunctionType FunctionType
        => GrammarAttribute.IsCached(in functionTypeCached) ? functionType!
            : this.Synthetic(ref functionTypeCached, ref functionType,
                ExpressionTypesAspect.FunctionReferenceInvocationExpression_FunctionType);
    private FunctionType? functionType;
    private bool functionTypeCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.FunctionReferenceInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FunctionReferenceInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionReferenceInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, Self.CurrentExpression))
            return !TempArguments.IsEmpty ? ControlFlowSet.CreateNormal(Arguments[0]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return index < Arguments.Count - 1 ? ControlFlowSet.CreateNormal(Arguments[index + 1]) : base.Inherited_ControlFlowFollowing(child, descendant, ctx);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return FunctionType.Parameters[index].Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? Expression.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.FunctionReferenceInvocationExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerInvocationExpressionNode : SemanticNode, IInitializerInvocationExpressionNode
{
    private IInitializerInvocationExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IInvocationExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IInitializerGroupNameNode> initializerGroup;
    private bool initializerGroupCached;
    public IInitializerGroupNameNode InitializerGroup
        => GrammarAttribute.IsCached(in initializerGroupCached) ? initializerGroup.UnsafeValue
            : this.RewritableChild(ref initializerGroupCached, ref initializerGroup);
    public IInitializerGroupNameNode CurrentInitializerGroup => initializerGroup.UnsafeValue;
    private IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations
        => GrammarAttribute.IsCached(in compatibleDeclarationsCached) ? compatibleDeclarations!
            : this.Synthetic(ref compatibleDeclarationsCached, ref compatibleDeclarations,
                OverloadResolutionAspect.InitializerInvocationExpression_CompatibleDeclarations);
    private IFixedSet<IInitializerDeclarationNode>? compatibleDeclarations;
    private bool compatibleDeclarationsCached;
    public ContextualizedCall? ContextualizedCall
        => GrammarAttribute.IsCached(in contextualizedCallCached) ? contextualizedCall
            : this.Synthetic(ref contextualizedCallCached, ref contextualizedCall,
                ExpressionTypesAspect.InitializerInvocationExpression_ContextualizedCall);
    private ContextualizedCall? contextualizedCall;
    private bool contextualizedCallCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.InitializerInvocationExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.InitializerInvocationExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IInitializerDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.InitializerInvocationExpression_ReferencedDeclaration);
    private IInitializerDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.InitializerInvocationExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.initializerGroup = Child.Create(this, initializerGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return Self.ReferencedDeclaration?.ParameterPlainTypes[index];
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (IndexOfNode(Self.CurrentArguments, descendant) is { } index)
            return ContextualizedCall?.ParameterTypes[index].Type;
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (0 < Self.CurrentArguments.Count && ReferenceEquals(child, Self.CurrentArguments[0]))
            return base.Inherited_FlowStateBefore(child, descendant, ctx);
        if (IndexOfNode(Self.CurrentArguments, child) is { } index)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierNameExpressionNode : SemanticNode, IIdentifierNameExpressionNode
{
    private IIdentifierNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IFixedList<IDeclarationNode> ReferencedDeclarations
        => GrammarAttribute.IsCached(in referencedDeclarationsCached) ? referencedDeclarations!
            : this.Synthetic(ref referencedDeclarationsCached, ref referencedDeclarations,
                BindingAmbiguousNamesAspect.StandardNameExpression_ReferencedDeclarations);
    private IFixedList<IDeclarationNode>? referencedDeclarations;
    private bool referencedDeclarationsCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public IdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.IdentifierNameExpression_Rewrite(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericNameExpressionNode : SemanticNode, IGenericNameExpressionNode
{
    private IGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IGenericNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public IFixedList<IDeclarationNode> ReferencedDeclarations
        => GrammarAttribute.IsCached(in referencedDeclarationsCached) ? referencedDeclarations!
            : this.Synthetic(ref referencedDeclarationsCached, ref referencedDeclarations,
                BindingAmbiguousNamesAspect.StandardNameExpression_ReferencedDeclarations);
    private IFixedList<IDeclarationNode>? referencedDeclarations;
    private bool referencedDeclarationsCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public GenericNameExpressionNode(
        IGenericNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnresolvedMemberAccessExpressionNode : SemanticNode, IUnresolvedMemberAccessExpressionNode
{
    private IUnresolvedMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> context;
    private bool contextCached;
    public IAmbiguousExpressionNode TempContext
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode? Context => TempContext as IExpressionNode;
    public IAmbiguousExpressionNode CurrentContext => context.UnsafeValue;
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnresolvedMemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IAmbiguousExpressionNode context,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.UnresolvedMemberAccessExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.UnresolvedMemberAccessExpression_Rewrite_NamespaceNameContext(this)
        ?? BindingAmbiguousNamesAspect.UnresolvedMemberAccessExpression_Rewrite_TypeNameExpressionContext(this)
        ?? BindingAmbiguousNamesAspect.UnresolvedMemberAccessExpression_Rewrite_ExpressionContext(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnqualifiedNamespaceNameNode : SemanticNode, IUnqualifiedNamespaceNameNode
{
    private IUnqualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnqualifiedNamespaceNameNode(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedNamespaceNameNode : SemanticNode, IQualifiedNamespaceNameNode
{
    private IQualifiedNamespaceNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<INamespaceNameNode> context;
    private bool contextCached;
    public INamespaceNameNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public INamespaceNameNode CurrentContext => context.UnsafeValue;
    public IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedNamespaceNameNode(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionGroupNameNode : SemanticNode, IFunctionGroupNameNode
{
    private IFunctionGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<INameExpressionNode?> context;
    private bool contextCached;
    public INameExpressionNode? Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public INameExpressionNode? CurrentContext => context.UnsafeValue;
    public OrdinaryName FunctionName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IFunctionInvocableDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CallCandidates
        => GrammarAttribute.IsCached(in callCandidatesCached) ? callCandidates!
            : this.Synthetic(ref callCandidatesCached, ref callCandidates,
                OverloadResolutionAspect.FunctionGroupName_CallCandidates);
    private IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>>? callCandidates;
    private bool callCandidatesCached;
    public IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CompatibleCallCandidates
        => GrammarAttribute.IsCached(in compatibleCallCandidatesCached) ? compatibleCallCandidates!
            : this.Synthetic(ref compatibleCallCandidatesCached, ref compatibleCallCandidates,
                OverloadResolutionAspect.FunctionGroupName_CompatibleCallCandidates);
    private IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>>? compatibleCallCandidates;
    private bool compatibleCallCandidatesCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFunctionInvocableDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.FunctionGroupName_ReferencedDeclaration);
    private IFunctionInvocableDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public CallCandidate<IFunctionInvocableDeclarationNode>? SelectedCallCandidate
        => GrammarAttribute.IsCached(in selectedCallCandidateCached) ? selectedCallCandidate
            : this.Synthetic(ref selectedCallCandidateCached, ref selectedCallCandidate,
                OverloadResolutionAspect.FunctionGroupName_SelectedCallCandidate);
    private CallCandidate<IFunctionInvocableDeclarationNode>? selectedCallCandidate;
    private bool selectedCallCandidateCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionGroupNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations)
    {
        BindingAmbiguousNamesAspect.Validate_FunctionGroupNameNode(syntax, context, functionName, typeArguments, referencedDeclarations);
        Syntax = syntax;
        this.context = Child.Create(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.FunctionGroupName_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.FunctionGroupName_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.FunctionGroupName_Rewrite_ToFunctionName(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionNameNode : SemanticNode, IFunctionNameNode
{
    private IFunctionNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<INameExpressionNode?> context;
    private bool contextCached;
    public INameExpressionNode? Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public INameExpressionNode? CurrentContext => context.UnsafeValue;
    public OrdinaryName FunctionName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IFunctionInvocableDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CallCandidates { [DebuggerStepThrough] get; }
    public IFixedSet<CallCandidate<IFunctionInvocableDeclarationNode>> CompatibleCallCandidates { [DebuggerStepThrough] get; }
    public CallCandidate<IFunctionInvocableDeclarationNode>? SelectedCallCandidate { [DebuggerStepThrough] get; }
    public IFunctionInvocableDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.FunctionName_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FunctionName_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FunctionNameNode(
        INameExpressionSyntax syntax,
        INameExpressionNode? context,
        OrdinaryName functionName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IFunctionInvocableDeclarationNode> referencedDeclarations,
        IEnumerable<CallCandidate<IFunctionInvocableDeclarationNode>> callCandidates,
        IEnumerable<CallCandidate<IFunctionInvocableDeclarationNode>> compatibleCallCandidates,
        CallCandidate<IFunctionInvocableDeclarationNode>? selectedCallCandidate,
        IFunctionInvocableDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        FunctionName = functionName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        CallCandidates = callCandidates.ToFixedSet();
        CompatibleCallCandidates = compatibleCallCandidates.ToFixedSet();
        SelectedCallCandidate = selectedCallCandidate;
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.FunctionName_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodGroupNameNode : SemanticNode, IMethodGroupNameNode
{
    private IMethodGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public OrdinaryName MethodName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IOrdinaryMethodDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CallCandidates
        => GrammarAttribute.IsCached(in callCandidatesCached) ? callCandidates!
            : this.Synthetic(ref callCandidatesCached, ref callCandidates,
                OverloadResolutionAspect.MethodGroupName_CallCandidates);
    private IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>>? callCandidates;
    private bool callCandidatesCached;
    public IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CompatibleCallCandidates
        => GrammarAttribute.IsCached(in compatibleCallCandidatesCached) ? compatibleCallCandidates!
            : this.Synthetic(ref compatibleCallCandidatesCached, ref compatibleCallCandidates,
                OverloadResolutionAspect.MethodGroupName_CompatibleCallCandidates);
    private IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>>? compatibleCallCandidates;
    private bool compatibleCallCandidatesCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.MethodGroupName_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IOrdinaryMethodDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.MethodGroupName_ReferencedDeclaration);
    private IOrdinaryMethodDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public CallCandidate<IOrdinaryMethodDeclarationNode>? SelectedCallCandidate
        => GrammarAttribute.IsCached(in selectedCallCandidateCached) ? selectedCallCandidate
            : this.Synthetic(ref selectedCallCandidateCached, ref selectedCallCandidate,
                OverloadResolutionAspect.MethodGroupName_SelectedCallCandidate);
    private CallCandidate<IOrdinaryMethodDeclarationNode>? selectedCallCandidate;
    private bool selectedCallCandidateCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodGroupNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IOrdinaryMethodDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.MethodGroupName_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.MethodGroupName_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.MethodGroupName_Rewrite_ToMethodName(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodNameNode : SemanticNode, IMethodNameNode
{
    private IMethodNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public OrdinaryName MethodName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IOrdinaryMethodDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CallCandidates { [DebuggerStepThrough] get; }
    public IFixedSet<CallCandidate<IOrdinaryMethodDeclarationNode>> CompatibleCallCandidates { [DebuggerStepThrough] get; }
    public CallCandidate<IOrdinaryMethodDeclarationNode>? SelectedCallCandidate { [DebuggerStepThrough] get; }
    public IOrdinaryMethodDeclarationNode? ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.MethodName_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.MethodName_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.MethodName_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MethodNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        OrdinaryName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IOrdinaryMethodDeclarationNode> referencedDeclarations,
        IEnumerable<CallCandidate<IOrdinaryMethodDeclarationNode>> callCandidates,
        IEnumerable<CallCandidate<IOrdinaryMethodDeclarationNode>> compatibleCallCandidates,
        CallCandidate<IOrdinaryMethodDeclarationNode>? selectedCallCandidate,
        IOrdinaryMethodDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        CallCandidates = callCandidates.ToFixedSet();
        CompatibleCallCandidates = compatibleCallCandidates.ToFixedSet();
        SelectedCallCandidate = selectedCallCandidate;
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return ExpressionPlainTypesAspect.MethodName_Context_ExpectedPlainType(this);
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return ExpressionTypesAspect.MethodName_Context_ExpectedType(this);
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentContext))
            return true;
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldAccessExpressionNode : SemanticNode, IFieldAccessExpressionNode
{
    private IFieldAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public IdentifierName FieldName { [DebuggerStepThrough] get; }
    public IFieldDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.FieldAccessExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.FieldAccessExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FieldAccessExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FieldAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IdentifierName fieldName,
        IFieldDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        FieldName = fieldName;
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.FieldAccessExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableNameExpressionNode : SemanticNode, IVariableNameExpressionNode
{
    private IVariableNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public ILocalBindingNode ReferencedDefinition { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.VariableNameExpression_DataFlowPrevious);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.VariableNameExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.VariableNameExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public VariableNameExpressionNode(
        IIdentifierNameExpressionSyntax syntax,
        ILocalBindingNode referencedDefinition)
    {
        Syntax = syntax;
        ReferencedDefinition = referencedDefinition;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        DefiniteAssignmentAspect.VariableNameExpression_Contribute_Diagnostics(this, builder);
        ShadowingAspect.VariableNameExpression_Contribute_Diagnostics(this, builder);
        SingleAssignmentAspect.VariableNameExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StandardTypeNameExpressionNode : SemanticNode, IStandardTypeNameExpressionNode
{
    private IStandardTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IStandardNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType,
                BareTypeAspect.TypeNameExpression_NamedBareType);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.TypeNameExpression_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public StandardTypeNameExpressionNode(
        IStandardNameExpressionSyntax syntax,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameExpressionNode : SemanticNode, IQualifiedTypeNameExpressionNode
{
    private IQualifiedTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<INamespaceNameNode> context;
    private bool contextCached;
    public INamespaceNameNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public INamespaceNameNode CurrentContext => context.UnsafeValue;
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ReferencedDeclaration { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public BareType? NamedBareType
        => GrammarAttribute.IsCached(in namedBareTypeCached) ? namedBareType
            : this.Synthetic(ref namedBareTypeCached, ref namedBareType,
                BareTypeAspect.TypeNameExpression_NamedBareType);
    private BareType? namedBareType;
    private bool namedBareTypeCached;
    public IMaybePlainType NamedPlainType
        => GrammarAttribute.IsCached(in namedPlainTypeCached) ? namedPlainType!
            : this.Synthetic(ref namedPlainTypeCached, ref namedPlainType,
                TypeExpressionsPlainTypesAspect.TypeNameExpression_NamedPlainType);
    private IMaybePlainType? namedPlainType;
    private bool namedPlainTypeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public QualifiedTypeNameExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<ITypeNode> typeArguments,
        ITypeDeclarationNode referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerGroupNameNode : SemanticNode, IInitializerGroupNameNode
{
    private IInitializerGroupNameNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public INameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<ITypeNameExpressionNode> context;
    private bool contextCached;
    public ITypeNameExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public ITypeNameExpressionNode CurrentContext => context.UnsafeValue;
    public OrdinaryName? InitializerName { [DebuggerStepThrough] get; }
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public InitializerGroupNameNode(
        INameExpressionSyntax syntax,
        ITypeNameExpressionNode context,
        OrdinaryName? initializerName,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        InitializerName = initializerName;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BuiltInTypeNameExpressionNode : SemanticNode, IBuiltInTypeNameExpressionNode
{
    private IBuiltInTypeNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IBuiltInTypeNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                Inherited_ContainingLexicalScope);
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ITypeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                BindingNamesAspect.BuiltInTypeNameExpression_ReferencedDeclaration);
    private ITypeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public BuiltInTypeNameExpressionNode(IBuiltInTypeNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfExpressionNode : SemanticNode, ISelfExpressionNode
{
    private ISelfExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public ISelfExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public IExecutableDefinitionNode ContainingDeclaration
        => GrammarAttribute.IsCached(in containingDeclarationCached) ? containingDeclaration!
            : this.Inherited(ref containingDeclarationCached, ref containingDeclaration,
                (ctx) => (IExecutableDefinitionNode)Inherited_ContainingDeclaration(ctx));
    private IExecutableDefinitionNode? containingDeclaration;
    private bool containingDeclarationCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.SelfExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public ISelfParameterNode? ReferencedDefinition
        => GrammarAttribute.IsCached(in referencedDefinitionCached) ? referencedDefinition
            : this.Synthetic(ref referencedDefinitionCached, ref referencedDefinition,
                BindingNamesAspect.SelfExpression_ReferencedDefinition);
    private ISelfParameterNode? referencedDefinition;
    private bool referencedDefinitionCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.SelfExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public SelfExpressionNode(ISelfExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        BindingNamesAspect.SelfExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MissingNameExpressionNode : SemanticNode, IMissingNameExpressionNode
{
    private IMissingNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMissingNameSyntax Syntax { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MissingNameExpressionNode(IMissingNameSyntax syntax)
    {
        Syntax = syntax;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownIdentifierNameExpressionNode : SemanticNode, IUnknownIdentifierNameExpressionNode
{
    private IUnknownIdentifierNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IIdentifierNameExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownIdentifierNameExpressionNode(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<IDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.UnknownIdentifierNameExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnknownGenericNameExpressionNode : SemanticNode, IUnknownGenericNameExpressionNode
{
    private IUnknownGenericNameExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

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
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public UnknownGenericNameExpressionNode(
        IEnumerable<IDeclarationNode> referencedDeclarations,
        IGenericNameExpressionSyntax syntax,
        GenericName name,
        IEnumerable<ITypeNode> typeArguments)
    {
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
        Syntax = syntax;
        Name = name;
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousMemberAccessExpressionNode : SemanticNode, IAmbiguousMemberAccessExpressionNode
{
    private IAmbiguousMemberAccessExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMemberAccessExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public IFixedList<ITypeNode> TypeArguments { [DebuggerStepThrough] get; }
    public IFixedSet<IDeclarationNode> ReferencedMembers { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AmbiguousMemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IDeclarationNode> referencedMembers)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedMembers = referencedMembers.ToFixedSet();
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.AmbiguousMemberAccessExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousMoveExpressionNode : SemanticNode, IAmbiguousMoveExpressionNode
{
    private IAmbiguousMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IMoveExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousNameExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousNameExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public INameExpressionNode? Referent => TempReferent as INameExpressionNode;
    public IAmbiguousNameExpressionNode CurrentReferent => referent.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AmbiguousMoveExpressionNode(
        IMoveExpressionSyntax syntax,
        IAmbiguousNameExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    protected override IChildTreeNode Rewrite()
        => CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveVariableExpressionNode : SemanticNode, IMoveVariableExpressionNode
{
    private IMoveVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<ILocalBindingNameExpressionNode> referent;
    private bool referentCached;
    public ILocalBindingNameExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public ILocalBindingNameExpressionNode CurrentReferent => referent.UnsafeValue;
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.RecoveryExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MoveVariableExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.MoveExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveVariableExpressionNode(
        IExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsImplicit = isImplicit;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.MoveVariableExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveValueExpressionNode : SemanticNode, IMoveValueExpressionNode
{
    private IMoveValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.RecoveryExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MoveValueExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.MoveExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public MoveValueExpressionNode(
        IExpressionSyntax syntax,
        IExpressionNode referent,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsImplicit = isImplicit;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentReferent))
            return IsImplicit ? ShouldPrepareToReturn() : false;
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.MoveValueExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImplicitTempMoveExpressionNode : SemanticNode, IImplicitTempMoveExpressionNode
{
    private IImplicitTempMoveExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ImplicitTempMoveExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ImplicitTempMoveExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.ImplicitTempMoveExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public ImplicitTempMoveExpressionNode(
        IExpressionSyntax syntax,
        IExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AmbiguousFreezeExpressionNode : SemanticNode, IAmbiguousFreezeExpressionNode
{
    private IAmbiguousFreezeExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IFreezeExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousNameExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousNameExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public INameExpressionNode? Referent => TempReferent as INameExpressionNode;
    public IAmbiguousNameExpressionNode CurrentReferent => referent.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AmbiguousFreezeExpressionNode(
        IFreezeExpressionSyntax syntax,
        IAmbiguousNameExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    protected override IChildTreeNode Rewrite()
        => CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeVariableExpressionNode : SemanticNode, IFreezeVariableExpressionNode
{
    private IFreezeVariableExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<ILocalBindingNameExpressionNode> referent;
    private bool referentCached;
    public ILocalBindingNameExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public ILocalBindingNameExpressionNode CurrentReferent => referent.UnsafeValue;
    public bool IsTemporary { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.RecoveryExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FreezeVariableExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FreezeExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeVariableExpressionNode(
        IExpressionSyntax syntax,
        ILocalBindingNameExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.FreezeVariableExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeValueExpressionNode : SemanticNode, IFreezeValueExpressionNode
{
    private IFreezeValueExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public bool IsTemporary { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.RecoveryExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FreezeValueExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.FreezeExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public FreezeValueExpressionNode(
        IExpressionSyntax syntax,
        IExpressionNode referent,
        bool isTemporary,
        bool isImplicit)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        IsTemporary = isTemporary;
        IsImplicit = isImplicit;
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(descendant, Self.CurrentReferent))
            return IsImplicit ? ShouldPrepareToReturn() : false;
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionTypesAspect.FreezeValueExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PrepareToReturnExpressionNode : SemanticNode, IPrepareToReturnExpressionNode
{
    private IPrepareToReturnExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    private RewritableChild<IExpressionNode> value;
    private bool valueCached;
    public IExpressionNode Value
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IExpressionNode CurrentValue => value.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.PrepareToReturnExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.PrepareToReturnExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public PrepareToReturnExpressionNode(IExpressionNode value)
    {
        this.value = Child.Create(this, value);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncBlockExpressionNode : SemanticNode, IAsyncBlockExpressionNode
{
    private IAsyncBlockExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IAsyncBlockExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncBlockExpressionNode(
        IAsyncBlockExpressionSyntax syntax,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.block = Child.Create(this, block);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncStartExpressionNode : SemanticNode, IAsyncStartExpressionNode
{
    private IAsyncStartExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IAsyncStartExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.AsyncStartExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.AsyncStartExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.AsyncStartExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AsyncStartExpressionNode(
        IAsyncStartExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AwaitExpressionNode : SemanticNode, IAwaitExpressionNode
{
    private IAwaitExpressionNode Self { [Inline] get => this; }
    private AttributeLock syncLock;
    protected override bool MayHaveRewrite => true;

    public IAwaitExpressionSyntax Syntax { [DebuggerStepThrough] get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public CodeFile File
        => Inherited_File(GrammarAttribute.CurrentInheritanceContext());
    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());
    public ValueIdScope ValueIdScope
        => Inherited_ValueIdScope(GrammarAttribute.CurrentInheritanceContext());
    public IEntryNode ControlFlowEntry()
        => Inherited_ControlFlowEntry(GrammarAttribute.CurrentInheritanceContext());
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Collection(ref controlFlowPreviousContributors, ref controlFlowPreviousCached, ref controlFlowPrevious,
                CollectContributors_ControlFlowPrevious<IExecutableDefinitionNode>, Collect_ControlFlowPrevious);
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    private IFixedSet<SemanticNode>? controlFlowPreviousContributors;
    public ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
    public IMaybeType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType,
                Inherited_ExpectedType);
    private IMaybeType? expectedType;
    private bool expectedTypeCached;
    public bool ImplicitRecoveryAllowed()
        => Inherited_ImplicitRecoveryAllowed(GrammarAttribute.CurrentInheritanceContext());
    public bool ShouldPrepareToReturn()
        => Inherited_ShouldPrepareToReturn(GrammarAttribute.CurrentInheritanceContext());
    public IMaybePlainType? ExpectedPlainType
        => GrammarAttribute.IsCached(in expectedPlainTypeCached) ? expectedPlainType
            : this.Inherited(ref expectedPlainTypeCached, ref expectedPlainType,
                Inherited_ExpectedPlainType);
    private IMaybePlainType? expectedPlainType;
    private bool expectedPlainTypeCached;
    public ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.AwaitExpression_ControlFlowNext);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.AwaitExpression_FlowStateAfter);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public IMaybePlainType PlainType
        => GrammarAttribute.IsCached(in plainTypeCached) ? plainType!
            : this.Synthetic(ref plainTypeCached, ref plainType,
                ExpressionPlainTypesAspect.AwaitExpression_PlainType);
    private IMaybePlainType? plainType;
    private bool plainTypeCached;
    public IMaybeType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.AwaitExpression_Type);
    private IMaybeType? type;
    private bool typeCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref syncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    private ValueId valueId;
    private bool valueIdCached;

    public AwaitExpressionNode(
        IAwaitExpressionSyntax syntax,
        IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybePlainType? Inherited_ExpectedPlainType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedPlainType(child, descendant, ctx);
    }

    internal override IMaybeType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return false;
        return base.Inherited_ShouldPrepareToReturn(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        ExpressionPlainTypesAspect.AwaitExpression_Contribute_Diagnostics(this, builder);
    }

    internal override void CollectContributors_ControlFlowPrevious(ContributorCollection<SemanticNode> contributors)
    {
        contributors.AddToRange(Self.ControlFlowNext.Keys.Cast<SemanticNode>(), this);
        base.CollectContributors_ControlFlowPrevious(contributors);
    }

    internal override void Contribute_ControlFlowPrevious(SemanticNode target, Dictionary<IControlFlowNode, ControlFlowKind> builder)
    {
        if (target is IControlFlowNode t)
            ControlFlowAspect.ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(this, t, builder);
    }

    protected override IChildTreeNode Rewrite()
        => ExpressionTypesAspect.Expression_Rewrite_ImplicitMove(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_ImplicitFreeze(this)
        ?? ExpressionTypesAspect.Expression_Rewrite_PrepareToReturn(this)
        ?? ExpressionPlainTypesAspect.Expression_Rewrite_ImplicitConversion(this)
        ?? base.Rewrite();
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageSymbolNode : SemanticNode, IPackageSymbolNode
{
    private IPackageSymbolNode Self { [Inline] get => this; }

    public IPackageReferenceNode PackageReference { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetSymbolNode MainFacet { [DebuggerStepThrough] get; }
    public IPackageFacetSymbolNode TestingFacet { [DebuggerStepThrough] get; }

    public PackageSymbolNode(IPackageReferenceNode packageReference)
    {
        PackageReference = packageReference;
        MainFacet = Child.Attach(this, SymbolNodeAspect.PackageSymbol_MainFacet(this));
        TestingFacet = Child.Attach(this, SymbolNodeAspect.PackageSymbol_TestingFacet(this));
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public INamespaceSymbolNode GlobalNamespace { [DebuggerStepThrough] get; }

    public PackageFacetSymbolNode(FixedSymbolTree symbolTree)
    {
        SymbolTree = symbolTree;
        GlobalNamespace = Child.Attach(this, SymbolNodeAspect.PackageFacetSymbol_GlobalNamespace(this));
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (ReferenceEquals(child, descendant))
            return this;
        return base.Inherited_ContainingDeclaration(child, descendant, ctx);
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
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public IFixedList<INamespaceMemberSymbolNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                n => ChildList.Attach(this, SymbolNodeAspect.NamespaceSymbol_Members(n)));
    private IFixedList<INamespaceMemberSymbolNode>? members;
    private bool membersCached;
    public FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => GrammarAttribute.IsCached(in nestedMembersCached) ? nestedMembers!
            : this.Synthetic(ref nestedMembersCached, ref nestedMembers,
                DeclarationsAspect.NamespaceDeclaration_NestedMembers);
    private IFixedList<INamespaceMemberDeclarationNode>? nestedMembers;
    private bool nestedMembersCached;
    public FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;

    public NamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionSymbolNode : SemanticNode, IFunctionSymbolNode
{
    private IFunctionSymbolNode Self { [Inline] get => this; }

    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public INamespaceDeclarationNode ContainingDeclaration
        => (INamespaceDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public FunctionSymbolNode(FunctionSymbol symbol)
    {
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VoidTypeSymbolNode : SemanticNode, IVoidTypeSymbolNode
{
    private IVoidTypeSymbolNode Self { [Inline] get => this; }

    public VoidTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public VoidTypeSymbolNode(VoidTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NeverTypeSymbolNode : SemanticNode, INeverTypeSymbolNode
{
    private INeverTypeSymbolNode Self { [Inline] get => this; }

    public NeverTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public NeverTypeSymbolNode(NeverTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PrimitiveTypeSymbolNode : SemanticNode, IPrimitiveTypeSymbolNode
{
    private IPrimitiveTypeSymbolNode Self { [Inline] get => this; }

    public BuiltInTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.BuiltInTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public ISelfSymbolNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, SymbolNodeAspect.NonVariableTypeSymbol_ImplicitSelf(n)));
    private ISelfSymbolNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.BuiltInTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<ITypeMemberSymbolNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                n => ChildSet.Attach(this, SymbolNodeAspect.BuiltInTypeSymbol_Members(n)));
    private IFixedSet<ITypeMemberSymbolNode>? members;
    private bool membersCached;

    public PrimitiveTypeSymbolNode(BuiltInTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassSymbolNode : SemanticNode, IClassSymbolNode
{
    private IClassSymbolNode Self { [Inline] get => this; }

    public OrdinaryTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IFixedList<IGenericParameterSymbolNode> GenericParameters
        => GrammarAttribute.IsCached(in genericParametersCached) ? genericParameters!
            : this.Synthetic(ref genericParametersCached, ref genericParameters,
                n => ChildList.Attach(this, SymbolNodeAspect.OrdinaryTypeSymbol_GenericParameters(n)));
    private IFixedList<IGenericParameterSymbolNode>? genericParameters;
    private bool genericParametersCached;
    public ISelfSymbolNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, SymbolNodeAspect.NonVariableTypeSymbol_ImplicitSelf(n)));
    private ISelfSymbolNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<IClassMemberSymbolNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                n => ChildSet.Attach(this, SymbolNodeAspect.ClassSymbol_Members(n)));
    private IFixedSet<IClassMemberSymbolNode>? members;
    private bool membersCached;

    public ClassSymbolNode(OrdinaryTypeSymbol symbol)
    {
        SymbolNodeAspect.Validate_ClassSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructSymbolNode : SemanticNode, IStructSymbolNode
{
    private IStructSymbolNode Self { [Inline] get => this; }

    public OrdinaryTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IFixedList<IGenericParameterSymbolNode> GenericParameters
        => GrammarAttribute.IsCached(in genericParametersCached) ? genericParameters!
            : this.Synthetic(ref genericParametersCached, ref genericParameters,
                n => ChildList.Attach(this, SymbolNodeAspect.OrdinaryTypeSymbol_GenericParameters(n)));
    private IFixedList<IGenericParameterSymbolNode>? genericParameters;
    private bool genericParametersCached;
    public ISelfSymbolNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, SymbolNodeAspect.NonVariableTypeSymbol_ImplicitSelf(n)));
    private ISelfSymbolNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<IStructMemberSymbolNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                n => ChildSet.Attach(this, SymbolNodeAspect.StructSymbol_Members(n)));
    private IFixedSet<IStructMemberSymbolNode>? members;
    private bool membersCached;

    public StructSymbolNode(OrdinaryTypeSymbol symbol)
    {
        SymbolNodeAspect.Validate_StructSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitSymbolNode : SemanticNode, ITraitSymbolNode
{
    private ITraitSymbolNode Self { [Inline] get => this; }

    public OrdinaryTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public ISymbolDeclarationNode ContainingDeclaration
        => Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> AssociatedMembersByName
        => GrammarAttribute.IsCached(in associatedMembersByNameCached) ? associatedMembersByName!
            : this.Synthetic(ref associatedMembersByNameCached, ref associatedMembersByName,
                NameLookupAspect.UserTypeDeclaration_AssociatedMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>? associatedMembersByName;
    private bool associatedMembersByNameCached;
    public IFixedList<IGenericParameterSymbolNode> GenericParameters
        => GrammarAttribute.IsCached(in genericParametersCached) ? genericParameters!
            : this.Synthetic(ref genericParametersCached, ref genericParameters,
                n => ChildList.Attach(this, SymbolNodeAspect.OrdinaryTypeSymbol_GenericParameters(n)));
    private IFixedList<IGenericParameterSymbolNode>? genericParameters;
    private bool genericParametersCached;
    public ISelfSymbolNode ImplicitSelf
        => GrammarAttribute.IsCached(in implicitSelfCached) ? implicitSelf!
            : this.Synthetic(ref implicitSelfCached, ref implicitSelf,
                n => Child.Attach(this, SymbolNodeAspect.NonVariableTypeSymbol_ImplicitSelf(n)));
    private ISelfSymbolNode? implicitSelf;
    private bool implicitSelfCached;
    public FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> InclusiveInstanceMembersByName
        => GrammarAttribute.IsCached(in inclusiveInstanceMembersByNameCached) ? inclusiveInstanceMembersByName!
            : this.Synthetic(ref inclusiveInstanceMembersByNameCached, ref inclusiveInstanceMembersByName,
                NameLookupAspect.UserTypeDeclaration_InclusiveInstanceMembersByName);
    private FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>? inclusiveInstanceMembersByName;
    private bool inclusiveInstanceMembersByNameCached;
    public IFixedSet<ITraitMemberSymbolNode> Members
        => GrammarAttribute.IsCached(in membersCached) ? members!
            : this.Synthetic(ref membersCached, ref members,
                n => ChildSet.Attach(this, SymbolNodeAspect.TraitSymbol_Members(n)));
    private IFixedSet<ITraitMemberSymbolNode>? members;
    private bool membersCached;

    public TraitSymbolNode(OrdinaryTypeSymbol symbol)
    {
        SymbolNodeAspect.Validate_TraitSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterSymbolNode : SemanticNode, IGenericParameterSymbolNode
{
    private IGenericParameterSymbolNode Self { [Inline] get => this; }

    public GenericParameterTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public IUserTypeDeclarationNode ContainingDeclaration
        => (IUserTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());
    public ITypeFactory TypeFactory
        => GrammarAttribute.IsCached(in typeFactoryCached) ? typeFactory!
            : this.Synthetic(ref typeFactoryCached, ref typeFactory,
                DeclarationsAspect.GenericParameterDeclaration_TypeFactory);
    private ITypeFactory? typeFactory;
    private bool typeFactoryCached;

    public GenericParameterSymbolNode(GenericParameterTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfSymbolNode : SemanticNode, ISelfSymbolNode
{
    private ISelfSymbolNode Self { [Inline] get => this; }

    public AssociatedTypeSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public INonVariableTypeDeclarationNode ContainingDeclaration
        => (INonVariableTypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());

    public SelfSymbolNode(AssociatedTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OrdinaryMethodSymbolNode : SemanticNode, IOrdinaryMethodSymbolNode
{
    private IOrdinaryMethodSymbolNode Self { [Inline] get => this; }

    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public OrdinaryMethodSymbolNode(MethodSymbol symbol)
    {
        SymbolNodeAspect.Validate_OrdinaryMethodSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodSymbolNode : SemanticNode, IGetterMethodSymbolNode
{
    private IGetterMethodSymbolNode Self { [Inline] get => this; }

    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public GetterMethodSymbolNode(MethodSymbol symbol)
    {
        SymbolNodeAspect.Validate_GetterMethodSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodSymbolNode : SemanticNode, ISetterMethodSymbolNode
{
    private ISetterMethodSymbolNode Self { [Inline] get => this; }

    public MethodSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public SetterMethodSymbolNode(MethodSymbol symbol)
    {
        SymbolNodeAspect.Validate_SetterMethodSymbolNode(symbol);
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorSymbolNode : SemanticNode, IConstructorSymbolNode
{
    private IConstructorSymbolNode Self { [Inline] get => this; }

    public ConstructorSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public ConstructorSymbolNode(ConstructorSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSymbolNode : SemanticNode, IInitializerSymbolNode
{
    private IInitializerSymbolNode Self { [Inline] get => this; }

    public InitializerSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public InitializerSymbolNode(InitializerSymbol symbol)
    {
        Symbol = symbol;
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        return this;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldSymbolNode : SemanticNode, IFieldSymbolNode
{
    private IFieldSymbolNode Self { [Inline] get => this; }

    public FieldSymbol Symbol { [DebuggerStepThrough] get; }
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public FieldSymbolNode(FieldSymbol symbol)
    {
        Symbol = symbol;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssociatedFunctionSymbolNode : SemanticNode, IAssociatedFunctionSymbolNode
{
    private IAssociatedFunctionSymbolNode Self { [Inline] get => this; }

    public OrdinaryName Name { [DebuggerStepThrough] get; }
    public FunctionSymbol Symbol { [DebuggerStepThrough] get; }
    public ITypeDeclarationNode ContainingDeclaration
        => (ITypeDeclarationNode)Inherited_ContainingDeclaration(GrammarAttribute.CurrentInheritanceContext());
    public IPackageDeclarationNode Package
        => Inherited_Package(GrammarAttribute.CurrentInheritanceContext());
    public IPackageFacetDeclarationNode Facet
        => Inherited_Facet(GrammarAttribute.CurrentInheritanceContext());
    public ISymbolTree SymbolTree()
        => Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext());

    public AssociatedFunctionSymbolNode(
        OrdinaryName name,
        FunctionSymbol symbol)
    {
        Name = name;
        Symbol = symbol;
    }
}

