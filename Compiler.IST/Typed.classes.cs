using System.CodeDom.Compiler;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST;


namespace Azoth.Tools.Bootstrap.Compiler.IST.Classes
{
    using static Azoth.Tools.Bootstrap.Compiler.IST.Typed;

    internal sealed partial class PackageNode : Package
    {
        IFixedList<PackageReference> Package.References => References;
        IFixedList<CompilationUnit> Package.CompilationUnits => CompilationUnits;
        CompilationUnit Package.TestingCompilationUnits => TestingCompilationUnits;
    }
    internal sealed partial class PackageReferenceNode : PackageReference
    {
    }
    internal sealed partial class CompilationUnitNode : CompilationUnit
    {
        IFixedList<UsingDirective> CompilationUnit.UsingDirectives => UsingDirectives;
        IFixedList<NamespaceMemberDeclaration> CompilationUnit.Declarations => Declarations;
    }
    internal sealed partial class UsingDirectiveNode : UsingDirective
    {
    }
    internal sealed partial class NamespaceDeclarationNode : NamespaceDeclaration
    {
        IFixedList<UsingDirective> NamespaceDeclaration.UsingDirectives => UsingDirectives;
        IFixedList<NamespaceMemberDeclaration> NamespaceDeclaration.Declarations => Declarations;
    }
    internal sealed partial class ClassDeclarationNode : ClassDeclaration
    {
        IFixedList<ClassMemberDeclaration> ClassDeclaration.Members => Members;
    }
    internal sealed partial class StructDeclarationNode : StructDeclaration
    {
        IFixedList<StructMemberDeclaration> StructDeclaration.Members => Members;
    }
    internal sealed partial class TraitDeclarationNode : TraitDeclaration
    {
        IFixedList<TraitMemberDeclaration> TraitDeclaration.Members => Members;
    }
    internal sealed partial class ClassMemberDeclarationNode : ClassMemberDeclaration
    {
    }
    internal sealed partial class TraitMemberDeclarationNode : TraitMemberDeclaration
    {
    }
    internal sealed partial class StructMemberDeclarationNode : StructMemberDeclaration
    {
    }
    internal sealed partial class FunctionDeclarationNode : FunctionDeclaration
    {
    }
}
