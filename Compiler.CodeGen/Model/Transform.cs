using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Transform
{
    public TransformNode? Syntax { get; }
    public IFixedList<Parameter> From { get; }
    public IFixedList<Parameter> To { get; }

    public Transform(Pass pass, TransformNode syntax)
    {
        Syntax = syntax;
        From = Syntax.From.Select(p => new Parameter(pass.FromLanguage?.Grammar, p)).ToFixedList();
        To = Syntax.To.Select(p => new Parameter(pass.ToLanguage?.Grammar, p)).ToFixedList();
    }

    public Transform(IEnumerable<Parameter> from, IEnumerable<Parameter> to)
    {
        From = from.ToFixedList();
        To = to.ToFixedList();
    }
}
