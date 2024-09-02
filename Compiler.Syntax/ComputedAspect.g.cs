using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ComputedAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DiagnosticCollection Package_Diagnostics(IPackageSyntax node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IStatementSyntax> ExpressionBody_Statements(IExpressionBodySyntax node);
}
