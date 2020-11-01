using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class CapabilityTypeSyntax : TypeSyntax, ICapabilityTypeSyntax
    {
        public ITypeSyntax ReferentType { get; }
        public ReferenceCapability Capability { get; }

        public CapabilityTypeSyntax(
            ReferenceCapability referenceCapability,
            ITypeSyntax referentType,
            TextSpan span)
            : base(span)
        {
            ReferentType = referentType;
            Capability = referenceCapability;
        }

        public override string ToString()
        {
            return $"{Capability} {ReferentType}";
        }
    }
}
