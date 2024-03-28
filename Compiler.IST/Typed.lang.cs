using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

public sealed class Typed
{
    [Closed(
        typeof(IntLiteral))]
    public interface Expression : Syntax
    {
        DataType Type { get; }
    }

    public interface IntLiteral : Expression
    {
        BigInteger Value { get; }
    }

    public interface Package
    {
        PackageSymbol Symbol { get; }
        IFixedList<CompilationUnit> CompilationUnits { get; }
    }

    public interface CompilationUnit
    {
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
    }

    [Closed(
        typeof(Expression))]
    public interface Syntax
    {
    }

}
