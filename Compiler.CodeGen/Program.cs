using System;
using System.IO;
using System.Text;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public static class Program
{
    public const string DirectiveMarker = "◊";

    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Expected exactly one argument: CompilerCodeGen <grammar file>");
            return -1;
        }

        try
        {
            Console.WriteLine("~~~~~~ Compiler Code Generator");
            var inputPath = args[0];
            var treeOutputPath = Path.ChangeExtension(inputPath, ".tree.cs");
            var childrenOutputPath = Path.ChangeExtension(inputPath, ".children.cs");
            Console.WriteLine($"Input:  {inputPath}");
            Console.WriteLine($"Tree Output: {treeOutputPath}");
            Console.WriteLine($"Children Output: {childrenOutputPath}");

            var inputFile = File.ReadAllText(inputPath)
                            ?? throw new InvalidOperationException("null from reading input file");
            var grammar = Parser.ReadGrammarConfig(inputFile);

            grammar.Validate();

            var treeCode = CodeBuilder.GenerateTree(grammar);
            WriteIfChanged(treeOutputPath, treeCode);

            var walkerCode = CodeBuilder.GenerateChildren(grammar);
            WriteIfChanged(childrenOutputPath, walkerCode);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return -1;
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
