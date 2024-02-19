using System.IO;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// A CodeReference to a file on disk referenced by its path.
public sealed class CodePath : CodeReference, ICodeFileSource
{
    public string Path { get; }

    public CodePath(string path, bool isTest)
        : this(path, FixedList.Empty<string>(), isTest)
    {
    }

    public CodePath(string path, IFixedList<string> @namespace, bool isTesting)
        : base(@namespace, isTesting)
    {
        Requires.That(nameof(path), System.IO.Path.IsPathFullyQualified(path), "must be fully qualified");
        Path = path;
    }

    public override string ToString() => Path;

    public CodeFile Load()
    {
        var text = File.ReadAllText(Path, CodeFile.Encoding);
        return new CodeFile(this, new CodeText(text));
    }

    public async Task<CodeFile> LoadAsync()
    {
        var text = await File.ReadAllTextAsync(Path, CodeFile.Encoding).ConfigureAwait(false);
        return new CodeFile(this, new CodeText(text));
    }
}
