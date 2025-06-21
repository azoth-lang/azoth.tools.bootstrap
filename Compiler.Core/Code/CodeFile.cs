using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Code;

/// A CodeFile represents the combination of CodeText and CodeReference
public class CodeFile : IComparable<CodeFile>, ICodeFileSource
{
    /// Source code files are encoded with UTF-8 without a BOM. C# UTF-8 include
    /// the BOM by default. So we make our own Encoding object.

    public static readonly Encoding Encoding = new UTF8Encoding(false);

    public CodeReference Reference { get; }

    public CodeText Code { get; }

    public bool IsTesting => Reference.IsTesting;

    public CodeFile(CodeReference reference, CodeText text)
    {
        Code = text;
        Reference = reference;
    }

    public static ValueTask<CodeFile> LoadAsync(string path, bool isTest)
        => LoadAsync(path, FixedList.Empty<string>(), isTest);

    public static ValueTask<CodeFile> LoadAsync(string path, IFixedList<string> @namespace, bool isTest)
    {
        var fullPath = Path.GetFullPath(path);
        var codePath = new CodePath(fullPath, @namespace, isTest, preload: true);
        return codePath.LoadAsync();
    }

    public int CompareTo(CodeFile? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Reference.CompareTo(other.Reference);
    }

    ValueTask<CodeFile> ICodeFileSource.LoadAsync() => ValueTask.FromResult(this);
}
