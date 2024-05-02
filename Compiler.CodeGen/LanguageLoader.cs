using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public sealed class LanguageLoader(string? langPath)
{
    private readonly Dictionary<string, Language> languages = new();
    private readonly IFixedList<string> languageSearchPaths = langPath.YieldValue()
                                                                      .Append(Environment.CurrentDirectory)
                                                                      .ToFixedList();

    public Language GetOrLoadLanguageNamed(string name)
    {
        if (languages.TryGetValue(name, out var language))
            return language;

        var languagePath = languageSearchPaths.Select(path => Path.Combine(path, $"{name}.lang"))
                                              .FirstOrDefault(File.Exists);

        if (languagePath is null)
            throw new InvalidOperationException($"Could not find file for language '{name}'");

        return LoadLanguage(name, languagePath);
    }

    public Language GetOrLoadLanguageFromPath(string relativePath)
    {
        var name = Path.GetFileNameWithoutExtension(relativePath);
        if (languages.TryGetValue(name, out var language))
            return language;

        var languagePath = languageSearchPaths.Select(path => Path.Combine(path, relativePath))
                                              .FirstOrDefault(File.Exists);

        if (languagePath is null)
            throw new InvalidOperationException($"Could language file '{relativePath}'");

        return LoadLanguage(name, languagePath);
    }

    private Language LoadLanguage(string name, string languagePath)
    {
        var inputFile = File.ReadAllText(languagePath)
                        ?? throw new InvalidOperationException("null from reading input file");

        var language = new Language(LanguageParser.ParseLanguage(inputFile, languagePath, this), this);
        languages.Add(name, language);
        return language;
    }
}
