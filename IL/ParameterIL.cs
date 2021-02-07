using System.ComponentModel.DataAnnotations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.IL
{
    [Closed(
        typeof(SelfParameterIL),
        typeof(NamedParameterIL),
        typeof(FieldParameterIL))]
    public abstract class ParameterIL
    {
        public bool IsMutableBinding { get; }
        public DataType DataType { get; internal set; }
    }
}
