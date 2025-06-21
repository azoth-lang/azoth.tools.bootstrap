using System;
using System.IO;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// <summary>A <see cref="CodeReference"/> to a file on disk referenced by its path.</summary>
public sealed class CodePath : CodeReference, ICodeFileSource
{
    public string Path { get; }
    private Task<CodeFile>? codeFileTask;

    public CodePath(string path, bool isTest)
        : this(path, FixedList.Empty<string>(), isTest)
    {
    }

    public CodePath(string path, IFixedList<string> @namespace, bool isTesting, bool preload = false)
        : base(@namespace, isTesting)
    {
        Requires.That(System.IO.Path.IsPathFullyQualified(path), nameof(path), "must be fully qualified");
        Path = path;
        if (preload) codeFileTask = LoadCodeFileAsync(this);
    }

    public override int CompareTo(CodeReference? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        if (other is not CodePath otherPath) return -1;
        return string.Compare(Path, otherPath.Path, StringComparison.CurrentCulture);
    }

    public override string ToString() => Path;

    public ValueTask<CodeFile> LoadAsync() => new(Lazy.InitializeOnce(ref codeFileTask, this, LoadCodeFileAsync));

    private static async Task<CodeFile> LoadCodeFileAsync(CodePath codePath)
    {
        var text = await File.ReadAllTextAsync(codePath.Path, CodeFile.Encoding).ConfigureAwait(false);
        return new(codePath, new CodeText(text));
    }
}
