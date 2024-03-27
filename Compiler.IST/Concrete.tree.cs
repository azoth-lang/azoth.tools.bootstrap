using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable PartialTypeWithSinglePart

public partial interface Package
{
    `PackageSymbol` Symbol { get; }
    IFixedList<CompilationUnit> CompilationUnits { get; }
}

public partial interface CompilationUnit
{
    `CodeFile` File { get; }
    `NamespaceName` ImplicitNamespaceName { get; }
}

public partial interface Syntax
{
}

