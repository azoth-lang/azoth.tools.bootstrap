using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public interface IFieldSymbol : IBindingSymbol
{
    public new IdentifierName Name { get; }

    public new DataType Type { get; }
    public new UserTypeSymbol ContainingSymbol { get; }
}
