using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// <summary>
/// A source that can provide a <see cref="CodeFile" />
/// </summary>
public interface ICodeFileSource
{
    /// <summary>
    /// Start loading the <see cref="CodeFile"/>.
    /// </summary>
    /// <remarks>If called repeatedly or concurrently, the CodeFile should be loaded only once and
    /// returned from then on.</remarks>
    ValueTask<CodeFile> LoadAsync();
}
