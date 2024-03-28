using System.CodeDom.Compiler;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IST.Classes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed class Concrete
{
    public interface Package : IImplementationRestricted
    {
        PackageSymbol Symbol { get; }
        IFixedList<CompilationUnit> CompilationUnits { get; }

        public static Package Create(PackageSymbol symbol, IFixedList<CompilationUnit> compilationUnits)
           => new Package_Concrete(symbol, (IFixedList<CompilationUnit_Concrete>)compilationUnits);
    }

    public interface CompilationUnit : IImplementationRestricted
    {
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }

        public static CompilationUnit Create(CodeFile file, NamespaceName implicitNamespaceName)
           => new CompilationUnit_Concrete(file, implicitNamespaceName);
    }

    [Closed(
        typeof(Expression))]
    public interface Syntax : IImplementationRestricted
    {
    }

    [Closed(
        typeof(BoolLiteral))]
    public interface Expression : Syntax
    {
    }

    public interface BoolLiteral : Expression
    {
        bool Value { get; }

        public static BoolLiteral Create(bool value)
           => new BoolLiteral_Concrete(value);
    }

}
