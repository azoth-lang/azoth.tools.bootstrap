using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Emit.C;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;
using Azoth.Tools.Bootstrap.Tests.Unit.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Azoth.Tools.Bootstrap.Tests.Conformance
{
    [Trait("Category", "Conformance")]
    public class ConformanceTests : IClassFixture<RuntimeLibraryFixture>
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

        private static readonly Regex ExitCodePattern = new Regex(@"//[ \t]*exit code: (?<exitCode>\d+)", RegexOptions.Compiled);
        private const string ExpectedOutputFileFormat = @"//[ \t]*{0} file: (?<file>[a-zA-Z0-9_.]+)";
        private const string ExpectedOutputFormat = @"\/\*[ \t]*{0}:\r?\n(?<output>(\*+[^/]|[^*])*)\*\/";
        private static readonly Regex ExpectCompileErrorsPattern = new Regex(@"//[ \t]*compile: errors", RegexOptions.Compiled);
        private static readonly Regex ErrorPattern = new Regex(@"//[ \t]*ERROR([ \t].*)?", RegexOptions.Compiled | RegexOptions.Multiline);

        [Theory]
        [MemberData(nameof(GetConformanceTestCases))]
        public void Test_cases(TestCase testCase)
        {
            // Setup
            var codeFile = CodeFile.Load(testCase.FullCodePath);
            var code = codeFile.Code.Text;
            var compiler = new AzothCompiler()
            {
                SaveLivenessAnalysis = true,
                SaveReachabilityGraphs = true,
            };
            var references = new Dictionary<Name, PackageIL>();

            // Reference Standard Library
            var stdLibPackage = CompileStdLib(compiler);
            references.Add("azoth.stdlib", stdLibPackage);

            try
            {
                // Analyze
                var package = compiler.CompilePackage("testPackage", codeFile.Yield(),
                    references.ToFixedDictionary());

                // Check for compiler errors
                Assert.NotNull(package.Diagnostics);
                var diagnostics = package.Diagnostics;
                var errorDiagnostics = CheckErrorsExpected(testCase, codeFile, code, diagnostics);

                // Disassemble
                var ilAssembler = new ILAssembler();
                testOutput.WriteLine(ilAssembler.Disassemble(package));

                // We got only expected errors, but need to not go on to emit code
                if (errorDiagnostics.Any())
                    return;

                // Emit Code
                var codePath = Path.ChangeExtension(testCase.FullCodePath, "c");
                EmitCode(package, stdLibPackage, codePath);

                // Compile Code to Executable
                var exePath = CompileToExecutable(codePath);

                // Execute and check results
                var process = Execute(exePath);

                process.WaitForExit();
                var stdout = process.StandardOutput.ReadToEnd();
                testOutput.WriteLine("stdout:");
                testOutput.WriteLine(stdout);
                Assert.Equal(ExpectedOutput(code, "stdout", testCase.FullCodePath), stdout);
                var stderr = process.StandardError.ReadToEnd();
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

        private PackageIL CompileStdLib(AzothCompiler compiler)
        {
            try
            {
                var sourceDir = Path.Combine(SolutionDirectory.Get(), @"stdlib\src");
                var sourcePaths =
                    Directory.EnumerateFiles(sourceDir, "*.ad", SearchOption.AllDirectories);
                var rootNamespace = FixedList<string>.Empty;
                var codeFiles = sourcePaths.Select(p => LoadCode(p, sourceDir, rootNamespace))
                    .ToList();
                return compiler.CompilePackage("azoth.stdlib", codeFiles,
                    FixedDictionary<Name, PackageIL>.Empty);
            }
            catch (FatalCompilationErrorException ex)
            {
                testOutput.WriteLine("Std Lib Compiler Errors:");
                foreach (var diagnostic in ex.Diagnostics)
                {
                    testOutput.WriteLine(
                        $"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                    testOutput.WriteLine(diagnostic.Message);
                }
                Assert.True(false, "Compilation errors in standard library");
                throw new UnreachableCodeException();
            }
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
            var expectedCompileErrorLines = ExpectedCompileErrorLines(codeFile, code);

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

            if (expectedCompileErrorLines.Any())
            {
                foreach (var expectedCompileErrorLine in expectedCompileErrorLines)
                {
                    // Assert a single error on the given line
                    var errorsOnLine = errorDiagnostics.Count(e =>
                        e.StartPosition.Line == expectedCompileErrorLine);
                    Assert.True(errorsOnLine == 1,
                        $"Expected one error on line {expectedCompileErrorLine}, found {errorsOnLine}");
                }
            }

            if (expectCompileErrors)
                Assert.True(errorDiagnostics.Any(), "Expected compilation errors and there were none");
            else
                foreach (var error in errorDiagnostics)
                {
                    var errorLine = error.StartPosition.Line;
                    if (expectedCompileErrorLines.All(line => line != errorLine))
                        Assert.True(false, $"Unexpected error on line {error.StartPosition.Line}");
                }

            return errorDiagnostics;
        }

        private static bool ExpectCompileErrors(string code)
        {
            return ExpectCompileErrorsPattern.IsMatch(code);
        }

        private static List<int> ExpectedCompileErrorLines(CodeFile codeFile, string code)
        {
            return ErrorPattern.Matches(code)
                .Select(match => codeFile.Code.Lines.LineIndexContainingOffset(match.Index) + 1)
                .ToList();
        }

        private static void EmitCode(PackageIL package, PackageIL stdLibPackage, string path)
        {
            var codeEmitter = new CodeEmitter();
            codeEmitter.Emit(package);
            codeEmitter.Emit(stdLibPackage);
            File.WriteAllText(path, codeEmitter.GetEmittedCode(), Encoding.UTF8);
        }

        private string CompileToExecutable(string codePath)
        {
            var compiler = new CLangCompiler();
            var sourceFiles = new[] { codePath, RuntimeLibraryFixture.GetRuntimeLibraryPath() };
            var headerSearchPaths = new[] { RuntimeLibraryFixture.GetRuntimeDirectory() };
            var outputPath = Path.ChangeExtension(codePath, "exe");
            var exitCode = compiler.Compile(new CompilerOutputAdapter(testOutput), sourceFiles, headerSearchPaths, outputPath);
            Assert.True(exitCode == 0, $"clang exited with {exitCode}");
            return outputPath;
        }

        private static Process Execute(string executable)
        {
            var startInfo = new ProcessStartInfo(executable)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            return Process.Start(startInfo);
        }

        private static int ExpectedExitCode(string code)
        {
            var exitCode = ExitCodePattern.Match(code).Groups["exitCode"]?.Captures.SingleOrDefault()?.Value ?? "0";
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
            if (path != null)
            {
                var testCaseDirectory = Path.GetDirectoryName(testCasePath) ?? throw new InvalidOperationException();
                path = Path.Combine(testCaseDirectory, path);
                return File.ReadAllText(path);
            }

            // Then look for inline expected output
            match = Regex.Match(code, string.Format(CultureInfo.InvariantCulture, ExpectedOutputFormat, channel));
            return match.Groups["output"]?.Captures.SingleOrDefault()?.Value ?? "";
        }

        public static TheoryData<TestCase> GetConformanceTestCases()
        {
            var testCases = new TheoryData<TestCase>();
            var testsDirectory = Path.Combine(SolutionDirectory.Get(), "tests");
            var adFiles = Directory.EnumerateFiles(testsDirectory, "*.ad", SearchOption.AllDirectories);
            var azFiles = Directory.EnumerateFiles(testsDirectory, "*.az", SearchOption.AllDirectories);
            foreach (var fullPath in adFiles.Concat(azFiles))
            {
                var relativePath = Path.GetRelativePath(testsDirectory, fullPath);
                testCases.Add(new TestCase(fullPath, relativePath));
            }
            return testCases;
        }
    }
}
