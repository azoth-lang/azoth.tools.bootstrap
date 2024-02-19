using System.IO;
using System.Text;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// A CodeFile represents the combination of CodeText and CodePath
public class CodeFile
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

    public static CodeFile Load(string path, bool isTest)
        => Load(path, FixedList.Empty<string>(), isTest);

    public static CodeFile Load(string path, IFixedList<string> @namespace, bool isTest)
    {
        var fullPath = Path.GetFullPath(path);
        return new CodeFile(new CodePath(fullPath, @namespace, isTest), new CodeText(File.ReadAllText(fullPath, Encoding)));
    }
}
