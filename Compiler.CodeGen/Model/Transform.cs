using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Transform
{
    public Pass Pass { get; }
    public TransformNode? Syntax { get; }
    public bool IsDeclared => Syntax is not null;
    public Parameter? From { get; }
    public IFixedList<Parameter> AdditionalParameters { get; }
    public IFixedList<Parameter> AllParameters { get; }
    public Parameter? To { get; }
    public IFixedList<Parameter> AdditionalReturnValues { get; }
    public IFixedList<Parameter> AllReturnValues { get; }
    public bool AutoGenerate { get; }

    public Transform(Pass pass, TransformNode syntax)
    {
        Pass = pass;
        Syntax = syntax;
        var grammar = pass.FromLanguage?.Grammar;
        From = Parameter.CreateFromSyntax(grammar, syntax.From);
        AdditionalParameters = syntax.AdditionalParameters.Select(p => Parameter.CreateFromSyntax(grammar, p)).ToFixedList();
        AllParameters = From.YieldValue().Concat(AdditionalParameters).ToFixedList();
        To = Parameter.CreateFromSyntax(pass.ToLanguage?.Grammar, syntax.To);
        AdditionalReturnValues = syntax.AdditionalReturnValues.Select(p => Parameter.CreateFromSyntax(pass.ToLanguage?.Grammar, p)).ToFixedList();
        AllReturnValues = To.YieldValue().Concat(AdditionalReturnValues).ToFixedList();
        AutoGenerate = Syntax.AutoGenerate;
    }

    public Transform(Pass pass, Parameter? from, IEnumerable<Parameter> additionalParameters,
        Parameter? to, IEnumerable<Parameter> additionalReturnValues, bool autoGenerate)
    {
        Pass = pass;
        From = from;
        AdditionalParameters = additionalParameters.ToFixedList();
        AllParameters = From.YieldValue().Concat(AdditionalParameters).ToFixedList();
        To = to;
        AdditionalReturnValues = additionalReturnValues.ToFixedList();
        AllReturnValues = To.YieldValue().Concat(AdditionalReturnValues).ToFixedList();
        AutoGenerate = autoGenerate;
    }
}
