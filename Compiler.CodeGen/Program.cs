using System;
using System.IO;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Passes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;
using McMaster.Extensions.CommandLineUtils;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public static class Program
{
    public static int Main(string[] args)
    {
        using var app = new CommandLineApplication()
        {
            Name = "CompilerCodeGen",
            Description = "Code generator for the Azoth compiler"
        };

        app.HelpOption();

        var langDirPathOption = app.Option("-p|--path <path>", "A directory path to search for language files", CommandOptionType.SingleValue);
        var inputFileArgs = app.Argument("input file(s)", "The input file(s) to generate code from", multipleValues: true).IsRequired();

        app.OnExecute(() =>
        {
            Console.WriteLine("Azoth Compiler Code Generator");
            var langDirPath = langDirPathOption.Value();
            if (langDirPath is not null)
                Console.WriteLine($"Language Path: {langDirPath}");

            var languageLoader = new LanguageLoader(langDirPath);
            bool success = true;
            foreach (var inputPath in inputFileArgs.Values)
                success &= Generate(inputPath!, languageLoader);

            return success ? 0 : 1;
        });

        return app.Execute(args);
    }

    private static bool Generate(string inputPath, LanguageLoader languageLoader)
    {
        var extension = Path.GetExtension(inputPath);
        switch (extension)
        {
            case ".tree":
                return GenerateTree(inputPath);
            case ".lang":
                return GenerateLang(inputPath, languageLoader);
            case ".pass":
                return GeneratePass(inputPath, languageLoader);
            default:
                Console.WriteLine($"Unknown file extension: {extension}");
                return false;
        }
    }

    private static bool GenerateTree(string inputPath)
    {
        try
        {
            var treeOutputPath = Path.ChangeExtension(inputPath, ".tree.cs");
            var childrenOutputPath = Path.ChangeExtension(inputPath, ".children.cs");
            Console.WriteLine($"Input:  {inputPath}");
            Console.WriteLine($"Tree Output: {treeOutputPath}");
            Console.WriteLine($"Children Output: {childrenOutputPath}");

            var inputFile = File.ReadAllText(inputPath)
                            ?? throw new InvalidOperationException("null from reading input file");
            var grammarSyntax = TreeParser.ParseGrammar(inputFile);

            var languageSyntax = new LanguageNode(null, inputPath, grammarSyntax, null);
            var language = new Language(languageSyntax);

            language.Grammar.ValidateTreeOrdering();

            var treeCode = TreeCodeBuilder.GenerateTree(language);
            WriteIfChanged(treeOutputPath, treeCode);

            var walkerCode = TreeCodeBuilder.GenerateChildren(language);
            WriteIfChanged(childrenOutputPath, walkerCode);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }

    private static bool GenerateLang(string inputPath, LanguageLoader languageLoader)
    {
        try
        {
            Console.WriteLine($"Input: {inputPath}");
            var language = languageLoader.GetOrLoadLanguageFromPath(inputPath);
            var fullPath = language.Syntax.DefinitionFilePath;
            var langOutputPath = Path.ChangeExtension(fullPath, ".lang.cs");
            var classesOutputPath = Path.ChangeExtension(fullPath, ".nodes.cs");
            Console.WriteLine($"Lang Output: {langOutputPath}");
            Console.WriteLine($"Nodes Output: {classesOutputPath}");

            var languageCode = LanguageCodeBuilder.GenerateLanguage(language);
            WriteIfChanged(langOutputPath, languageCode);

            var nodesCode = LanguageCodeBuilder.GenerateNodes(language);
            WriteIfChanged(classesOutputPath, nodesCode);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }

    private static bool GeneratePass(string inputPath, LanguageLoader languageLoader)
    {
        try
        {
            var passOutputPath = Path.ChangeExtension(inputPath, ".pass.cs");
            Console.WriteLine($"Input: {inputPath}");
            Console.WriteLine($"Pass Output: {passOutputPath}");

            var inputFile = File.ReadAllText(inputPath)
                            ?? throw new InvalidOperationException("null from reading input file");
            var syntax = PassParser.ParsePass(inputFile);

            var pass = new Pass(syntax, languageLoader);

            var passCode = PassCodeBuilder.GeneratePass(pass);
            WriteIfChanged(passOutputPath, passCode);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }

    /// <summary>
    /// Only write code if it has changed so VS doesn't think the file is
    /// constantly changing and needs to be recompiled.
    /// </summary>
    private static void WriteIfChanged(string filePath, string code)
    {
        var previousCode = File.Exists(filePath)
            ? File.ReadAllText(filePath, new UTF8Encoding(false, true))
            : null;
        if (code != previousCode) File.WriteAllText(filePath, code);
    }
}
