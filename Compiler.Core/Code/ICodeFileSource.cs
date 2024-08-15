using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// <summary>
/// A source for
/// </summary>
public interface ICodeFileSource
{
    CodeFile Load();
    Task<CodeFile> LoadAsync();
}
