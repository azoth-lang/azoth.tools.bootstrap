using System.CodeDom.Compiler;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IST.Classes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed class Typed
{
    [Closed(
        typeof(IntLiteral),
        typeof(StringLiteral))]
    public interface Expression : Syntax
    {
        DataType Type { get; }
    }

    public interface IntLiteral : Expression
    {
        BigInteger Value { get; }

        public static IntLiteral Create(BigInteger value, DataType type)
           => new IntLiteral_Typed(value, type);
    }

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

    public interface StringLiteral : Expression
    {
        string Value { get; }

        public static StringLiteral Create(string value, DataType type)
           => new StringLiteral_Typed(value, type);

        public static StringLiteral Create(Concrete.StringLiteral stringLiteral, DataType type)
           => new StringLiteral_Typed(stringLiteral.Value, type);
    }

}
