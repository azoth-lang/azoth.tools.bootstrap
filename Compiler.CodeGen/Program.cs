using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;
using Azoth.Tools.Bootstrap.Framework;
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

            bool success = Generate(inputFileArgs.Values!);

            return success ? 0 : 1;
        });

        return app.Execute(args);
    }

    private static bool Generate(IReadOnlyList<string> inputPath)
    {
        var pathLookup = inputPath.ToLookup(Path.GetExtension);
        foreach (var group in pathLookup)
            switch (group.Key)
            {
                case ".tree":
                case ".aspect":
                    continue;
                default:
                    Console.WriteLine($"Unknown file extension: {group.Key}");
                    return false;
            }

        var treePaths = pathLookup[".tree"].ToFixedList();
        if (treePaths.IsEmpty)
        {
            Console.WriteLine("No tree files specified");
            return false;
        }

        if (treePaths.Count > 1)
        {
            Console.WriteLine("Multiple tree files specified");
            return false;
        }

        var treePath = treePaths[0];
        var aspectPaths = pathLookup[".aspect"].ToFixedList();

        return GenerateTree(treePath, aspectPaths);
    }

    private static bool GenerateTree(string treePath, IFixedList<string> aspectPaths)
    {
        try
        {
            var treeOutputPath = Path.ChangeExtension(treePath, ".tree.cs");
            var childrenOutputPath = Path.ChangeExtension(treePath, ".children.cs");
            Console.WriteLine($"Tree Input:  {treePath}");
            Console.WriteLine($"Tree Output: {treeOutputPath}");
            Console.WriteLine($"Children Output: {childrenOutputPath}");

            var treeSyntax = TreeParser.Parse(File.ReadAllText(treePath));
            var aspectSyntax = new List<AspectSyntax>();
            foreach (var aspectPath in aspectPaths)
            {
                Console.WriteLine($"Aspect Input: {aspectPath}");
                var aspect = AspectParser.Parse(File.ReadAllText(aspectPath));
                aspectSyntax.Add(aspect);
            }

            var tree = new TreeModel(treeSyntax, aspectSyntax);
            tree.Validate();

            {
                var treeCode = TreeCodeBuilder.GenerateTree(tree);
                WriteIfChanged(treeOutputPath, treeCode);

                var walkerCode = TreeCodeBuilder.GenerateChildren(tree);
                WriteIfChanged(childrenOutputPath, walkerCode);
            }

            foreach (var (aspect, path) in tree.Aspects.EquiZip(aspectPaths))
            {
                var directory = Path.GetDirectoryName(path)!;
                var aspectOutputPath = Path.Combine(directory, aspect.Name + ".g.cs");
                Console.WriteLine($"Aspect Output: {aspectOutputPath}");
                var aspectCode = AspectCodeBuilder.GenerateAspect(aspect);
                WriteIfChanged(aspectOutputPath, aspectCode);
            }

            return true;
        }
        catch (ValidationFailedException)
        {
            Console.WriteLine("Validation failed");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine(ex);
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
