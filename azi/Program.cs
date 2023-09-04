using McMaster.Extensions.CommandLineUtils;

namespace Azoth.Tools.Bootstrap.Interpreter.CLI;

public static class Program
{
    public static int Main(string[] args)
    {
        using var app = new CommandLineApplication()
        {
            Name = "azi",
            Description = "The Azoth Interpreter"
        };

        app.HelpOption();

        return app.Execute(args);
    }
}
