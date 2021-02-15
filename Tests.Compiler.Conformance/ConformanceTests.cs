using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IR;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.IL.Assembly;
using Azoth.Tools.Bootstrap.IL.IO;
using Azoth.Tools.Bootstrap.Interpreter;
using Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Azoth.Tools.Bootstrap.Tests.Conformance
{
    [Trait("Category", "Conformance")]
    public class ConformanceTests
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
            var references = new Dictionary<Name, PackageIR>();

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
                var (packageIL, stdLibIL) = EmitIL(package, supportPackage);

                // Disassemble
                var ilAssembler = new Disassembler();
                testOutput.WriteLine(ilAssembler.Disassemble(packageIL));

                // Execute and check results
                var process = Execute(packageIL, stdLibIL);

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

        private PackageIR CompileSupportPackage(AzothCompiler compiler)
        {
            try
            {
                var sourceDir = TestsSupportPackage.GetDirectory();
                var sourcePaths = CodeFiles.GetIn(sourceDir);
                var rootNamespace = FixedList<string>.Empty;
                var codeFiles = sourcePaths.Select(p => LoadCode(p, sourceDir, rootNamespace)).ToList();
                var package = compiler.CompilePackage(TestsSupportPackage.Name, codeFiles,
                    FixedDictionary<Name, PackageIR>.Empty);
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

        private static (MemoryStream PackageIL, MemoryStream StdLibIL) EmitIL(PackageIR package, PackageIR stdLibPackage)
        {
            var writer = new ILWriter();
            var packageIL = new MemoryStream();
            //writer.Write(package, packageIL);
            packageIL.Position = 0;
            var stdLibIL = new MemoryStream();
            //writer.Write(stdLibPackage, stdLibIL);
            stdLibIL.Position = 0;
            //return (packageIL, stdLibIL);
            throw new NotImplementedException();
        }

        private static InterpreterProcess Execute(MemoryStream packageIL, MemoryStream stdLibIL)
        {
            var interpreter = new AzothInterpreter();
            interpreter.LoadPackage(stdLibIL);
            interpreter.LoadPackage(packageIL);
            return interpreter.Execute();
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
            var testsDirectory = TestsDirectory.Get();
            foreach (var fullPath in CodeFiles.GetIn(testsDirectory))
            {
                var relativePath = Path.GetRelativePath(testsDirectory, fullPath);
                testCases.Add(new TestCase(fullPath, relativePath));
            }
            return testCases;
        }
    }
}
