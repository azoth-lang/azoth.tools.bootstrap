using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;
using MoreLinq;
using Xunit;
using Xunit.Abstractions;

namespace Azoth.Tools.Bootstrap.Tests.Conformance;

[Trait("Category", "Conformance")]
public partial class ConformanceTests
{
    private readonly ITestOutputHelper testOutput;

    public ConformanceTests(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }

    [Fact]
    public void Can_load_all_conformance_test_cases()
    {
        Assert.NotEmpty(GetConformanceTestCases());
    }

    [GeneratedRegex(@"//[ \t]*exit code: (?<exitCode>\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex ExitCodePattern();
    private const string ExpectedOutputFileFormat = @"//[ \t]*{0} file: (?<file>[a-zA-Z0-9_.]+)";
    private const string ExpectedOutputFormat = @"\/\*[ \t]*{0}:\r?\n(?<output>(\*+[^/]|[^*])*)\*\/";
    [GeneratedRegex("//[ \\t]*compile: errors", RegexOptions.ExplicitCapture)]
    private static partial Regex ExpectCompileErrorsPattern();
    [GeneratedRegex(@"(?<!^.*//.*)//[ \t]*ERROR( x(?<count>\d+))?([ \t].*)?",
        RegexOptions.Multiline | RegexOptions.ExplicitCapture)]
    private static partial Regex ErrorPattern();

    [Theory]
    [MemberData(nameof(GetConformanceTestCases))]
    public async Task Test_cases(TestCase testCase)
    {
        // Setup
        var codeFile = CodeFile.Load(testCase.FullCodePath);
        var code = codeFile.Code.Text;
        var compiler = new AzothCompiler()
        {
            SaveLivenessAnalysis = true,
            SaveReachabilityGraphs = true,
        };
        var references = new Dictionary<Name, Package>();

        // Reference Standard Library
        var supportPackage = CompileSupportPackage(compiler);
        references.Add(TestsSupportPackage.Name, supportPackage);

        try
        {
            // Analyze
            var package = compiler.CompilePackage("testPackage", codeFile.Yield(),
                references.ToFixedDictionary());

            // Check for compiler errors
            Assert.NotNull(package.Diagnostics);
            var diagnostics = package.Diagnostics;
            var errorDiagnostics = CheckErrorsExpected(testCase, codeFile, code, diagnostics);

            // We got only expected errors, but need to not go on to emit code
            if (errorDiagnostics.Any())
                return;

            // Emit Code
            //var (packageIL, stdLibIL) = EmitIL(package, supportPackage);

            // Disassemble
            //var ilAssembler = new Disassembler();
            //testOutput.WriteLine(ilAssembler.Disassemble(packageIL));
            //packageIL.Position = 0;

            // Execute and check results
            var process = Execute(package);
            //var process = Execute(packageIL, stdLibIL);

            await process.WaitForExitAsync();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            testOutput.WriteLine("stdout:");
            testOutput.WriteLine(stdout);
            Assert.Equal(ExpectedOutput(code, "stdout", testCase.FullCodePath), stdout);
            var stderr = await process.StandardError.ReadToEndAsync();
            testOutput.WriteLine("stderr:");
            testOutput.WriteLine(stderr);
            Assert.Equal(ExpectedOutput(code, "stderr", testCase.FullCodePath), stderr);
            Assert.Equal(ExpectedExitCode(code), process.ExitCode);
        }
        catch (FatalCompilationErrorException ex)
        {
            var diagnostics = ex.Diagnostics;
            CheckErrorsExpected(testCase, codeFile, code, diagnostics);
        }
    }

    private Package CompileSupportPackage(AzothCompiler compiler)
    {
        try
        {
            var sourceDir = TestsSupportPackage.GetDirectory();
            var sourcePaths = CodeFiles.GetIn(sourceDir);
            var rootNamespace = FixedList<string>.Empty;
            var codeFiles = sourcePaths.Select(p => LoadCode(p, sourceDir, rootNamespace)).ToList();
            var package = compiler.CompilePackage(TestsSupportPackage.Name, codeFiles,
                FixedDictionary<Name, Package>.Empty);
            if (package.Diagnostics.Any(d => d.Level >= DiagnosticLevel.CompilationError))
                ReportSupportCompilationErrors(package.Diagnostics);
            return package;
        }
        catch (FatalCompilationErrorException ex)
        {
            ReportSupportCompilationErrors(ex.Diagnostics);
            throw new UnreachableCodeException();
        }
    }

    private void ReportSupportCompilationErrors(FixedList<Diagnostic> diagnostics)
    {
        testOutput.WriteLine("Test Support Package Compiler Errors:");
        foreach (var diagnostic in diagnostics)
        {
            testOutput.WriteLine(
                $"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
            testOutput.WriteLine(diagnostic.Message);
        }

        Assert.True(false, "Compilation Errors in Test Support Package");
    }

    private static CodeFile LoadCode(
        string path,
        string sourceDir,
        FixedList<string> rootNamespace)
    {
        var relativeDirectory = Path.GetDirectoryName(Path.GetRelativePath(sourceDir, path)) ?? throw new InvalidOperationException();
        var ns = rootNamespace.Concat(relativeDirectory.SplitOrEmpty(Path.DirectorySeparatorChar)).ToFixedList();
        return CodeFile.Load(path, ns);
    }

    private List<Diagnostic> CheckErrorsExpected(
        TestCase testCase,
        CodeFile codeFile,
        string code,
        FixedList<Diagnostic> diagnostics)
    {
        // Check for compiler errors
        var expectCompileErrors = ExpectCompileErrors(code);
        var expectedCompileErrors = ExpectedCompileErrors(codeFile, code);

        if (diagnostics.Any())
        {
            testOutput.WriteLine("Compiler Errors:");
            foreach (var diagnostic in diagnostics)
            {
                testOutput.WriteLine(
                    $"{testCase.RelativeCodePath}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                testOutput.WriteLine(diagnostic.Message);
            }

            testOutput.WriteLine("");
        }

        var errorDiagnostics = diagnostics
                               .Where(d => d.Level >= DiagnosticLevel.CompilationError).ToList();

        var errors = new StringBuilder();

        var matchedErrors = errorDiagnostics
            .FullGroupJoin(expectedCompileErrors, d => d.StartPosition.Line, l => l.Line,
                (line, errors, expected) => (line, errors.Count(), expected.Sum(e => e.Count)));

        foreach (var matchedError in matchedErrors)
        {
            switch (matchedError)
            {
                case (var line, 1, 0) when !expectCompileErrors:
                    errors.AppendLine($"Unexpected error on line {line}.");
                    break;
                case (var line, int errorCount and >= 1, 0) when !expectCompileErrors:
                    errors.AppendLine($"{errorCount} unexpected errors on line {line}.");
                    break;
                case (var line, 0, 1):
                    errors.AppendLine($"Expected an error on line {line}.");
                    break;
                case (var line, int errorCount and not 1, 1):
                    errors.AppendLine(
                        $"Expected one error on line {line}, found {errorCount}.");
                    break;
            }
        }

        if (expectCompileErrors && !errorDiagnostics.Any())
            errors.AppendLine("Expected compilation errors and there were none");

        Assert.True(errors.Length == 0, errors.ToString().TrimEnd());

        return errorDiagnostics;
    }

    private static bool ExpectCompileErrors(string code)
        => ExpectCompileErrorsPattern().IsMatch(code);

    private static List<ExpectedError> ExpectedCompileErrors(CodeFile codeFile, string code)
    {

        return ErrorPattern().Matches(code)
                             .Select(Selector)
                             .OrderBy(e => e.Line)
                             .ToList();

        ExpectedError Selector(Match match)
        {
            var line = codeFile.Code.Lines.LineIndexContainingOffset(match.Index) + 1;
            if (!int.TryParse(match.Groups["count"].Value, out var count))
                count = 1;
            return new(line, count);
        }
    }

    private static InterpreterProcess Execute(Package package)
    {
        var interpreter = new AzothTreeInterpreter();
        return interpreter.Execute(package);
    }

    private static int ExpectedExitCode(string code)
    {
        var exitCode = ExitCodePattern().Match(code).Groups["exitCode"]?.Captures.SingleOrDefault()?.Value ?? "0";
        return int.Parse(exitCode, CultureInfo.InvariantCulture);
    }

    private static string ExpectedOutput(
        string code,
        string channel,
        string testCasePath)
    {
        // First check if there is a file for the expected output
        var match = Regex.Match(code, string.Format(CultureInfo.InvariantCulture, ExpectedOutputFileFormat, channel));
        var path = match.Groups["file"]?.Captures.SingleOrDefault()?.Value;
        if (path is not null)
        {
            var testCaseDirectory = Path.GetDirectoryName(testCasePath) ?? throw new InvalidOperationException();
            path = Path.Combine(testCaseDirectory, path);
            return File.ReadAllText(path);
        }

        // Then look for inline expected output
        match = Regex.Match(code, string.Format(CultureInfo.InvariantCulture, ExpectedOutputFormat, channel));
        return match.Groups["output"].Captures.SingleOrDefault()?.Value ?? "";
    }

    public static TheoryData<TestCase> GetConformanceTestCases()
    {
        var testCases = new TheoryData<TestCase>();
        var testsDirectory = TestsDirectory.Get();
        foreach (var fullPath in CodeFiles.GetIn(testsDirectory))
        {
            var relativePath = Path.GetRelativePath(testsDirectory, fullPath);
            testCases.Add(new TestCase(fullPath, relativePath));
        }
        return testCases;
    }
}
