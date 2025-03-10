using System;
using System.IO;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Helpers;

public static class SolutionDirectory
{
    public static string Get()
    {
        var directory = Directory.GetCurrentDirectory() ?? throw new InvalidOperationException("Could not get current directory");
        while (!Directory.GetFiles(directory, "*.sln", SearchOption.TopDirectoryOnly).Any())
        {
            directory = Path.GetDirectoryName(directory) ?? throw new InvalidOperationException("Null directory name");
        }
        return directory ?? throw new InvalidOperationException("Compiler is confused, this can't be null");
    }
}
