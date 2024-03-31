using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using static Azoth.Tools.Bootstrap.Compiler.IST.Concrete;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

using AST = Compiler.AST;

/// <summary>
/// A nanopass that constructs the IST from the CST and binds it to the CST.
/// </summary>
public sealed class SyntaxBinderPass
{
    public static Package Build(PackageSyntax<AST.Package> packageSyntax)
        => throw new NotImplementedException();
}
