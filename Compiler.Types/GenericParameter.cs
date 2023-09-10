using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A generic parameter to a type.
/// </summary>
/// <remarks>At the moment all generic parameters are types. In the future, they don't have to be.
/// That is why this class exists to ease refactoring later.</remarks>
public sealed class GenericParameter
{
    public GenericParameter(Name name)
    {
        Name = name;
    }

    public Name Name { get; }
    // TODO  public DataType DataType { get; }

    public override string ToString() => Name.ToString();

    public static implicit operator GenericParameter(string name)
        => new(name);
}
