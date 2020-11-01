namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Config
{
    public class GrammarProperty
    {
        public string Name { get; }
        public GrammarType Type { get; }

        public GrammarProperty(string name, GrammarType type)
        {
            Name = name;
            Type = type;
        }
    }
}
