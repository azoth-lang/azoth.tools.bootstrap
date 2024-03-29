using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// Base type for any declaration that declares a callable thing.
/// </summary>
public partial interface IInvocableDeclarationSyntax
{
    IFixedList<IParameterSyntax> AllParameters { get; }
}
