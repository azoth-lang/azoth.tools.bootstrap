using System;
using System.IO;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;
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

        var inputFileArg = app.Argument("input file", "The input file to generate code from").IsRequired();

        app.OnExecute(() =>
        {
            Console.WriteLine("Azoth Compiler Code Generator");
            var inputFile = inputFileArg.Value!;
            var extension = Path.GetExtension(inputFile);
            switch (extension)
            {
                case ".tree":
                    return GenerateTree(inputFile);
                case ".lang":
                    return GenerateLang(inputFile);
                default:
                    Console.WriteLine($"Unknown file extension: {extension}");
                    return 1;
            }
        });

        return app.Execute(args);
    }

    private static int GenerateTree(string inputPath)
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
            var grammar = TreeParser.ParseGrammar(inputFile);

            grammar.ValidateTreeOrdering();

            var treeCode = TreeCodeBuilder.GenerateTree(grammar);
            WriteIfChanged(treeOutputPath, treeCode);

            var walkerCode = TreeCodeBuilder.GenerateChildren(grammar);
            WriteIfChanged(childrenOutputPath, walkerCode);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static int GenerateLang(string inputPath)
    {
        try
        {
            var langOutputPath = Path.ChangeExtension(inputPath, ".lang.cs");
            Console.WriteLine($"Input:  {inputPath}");
            Console.WriteLine($"Lang Output: {langOutputPath}");

            var inputFile = File.ReadAllText(inputPath)
                            ?? throw new InvalidOperationException("null from reading input file");
            var language = LanguageParser.ParseLanguage(inputFile);

            var languageCode = LanguageCodeBuilder.GenerateLanguage(language);
            WriteIfChanged(langOutputPath, languageCode);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
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
