using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types
{
    /// <summary>
    /// Analyzes an <see cref="ITypeSyntax" /> to evaluate which type it refers to.
    /// </summary>
    public class TypeResolver
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public TypeResolver(CodeFile file, Diagnostics diagnostics)
        {
            this.file = file;
            this.diagnostics = diagnostics;
        }

        [return: NotNullIfNotNull(nameof(typeSyntax))]
        public DataType? Evaluate(ITypeSyntax? typeSyntax, bool implicitRead)
        {
            switch (typeSyntax)
            {
                default:
                    throw ExhaustiveMatch.Failed(typeSyntax);
                case null:
                    return null;
                case ITypeNameSyntax typeName:
                {
                    var symbolPromises = typeName.LookupInContainingScope().ToFixedList();
                    switch (symbolPromises.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, typeName.Span));
                            typeName.ReferencedSymbol.Fulfill(null);
                            typeName.NamedType = DataType.Unknown;
                            break;
                        case 1:
                            var symbol = symbolPromises.Single().Result;
                            typeName.ReferencedSymbol.Fulfill(symbol);
                            var type = symbol.DeclaresDataType;
                            if (implicitRead) type = type.ToReadOnly();
                            typeName.NamedType = type;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, typeName.Span));
                            typeName.ReferencedSymbol.Fulfill(null);
                            typeName.NamedType = DataType.Unknown;
                            break;
                    }
                    break;
                }
                case ICapabilityTypeSyntax referenceCapability:
                {
                    var type = Evaluate(referenceCapability.ReferentType, implicitRead: false);
                    if (type == DataType.Unknown)
                        return DataType.Unknown;
                    if (type is ReferenceType referenceType)
                        referenceCapability.NamedType = Evaluate(referenceType, referenceCapability.Capability);
                    else
                        referenceCapability.NamedType = DataType.Unknown;
                    break;
                }
                case IOptionalTypeSyntax optionalType:
                {
                    var referent = Evaluate(optionalType.Referent, implicitRead);
                    return optionalType.NamedType = new OptionalType(referent);
                }
            }

            return typeSyntax.NamedType ?? throw new InvalidOperationException();
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public ObjectType Evaluate(ObjectType referenceType, IReferenceCapabilitySyntax capability)
            => referenceType.To(Evaluate(capability));

        private static ReferenceType Evaluate(ReferenceType referenceType, IReferenceCapabilitySyntax capability)
            => referenceType.To(Evaluate(capability));

        private static ReferenceCapability Evaluate(IReferenceCapabilitySyntax capability)
        {
            return capability.Declared switch
            {
                DeclaredReferenceCapability.Isolated => ReferenceCapability.Isolated,

                DeclaredReferenceCapability.Constant => ReferenceCapability.Constant,
                DeclaredReferenceCapability.Identity => ReferenceCapability.Identity,

                DeclaredReferenceCapability.Mutable => ReferenceCapability.Mutable,
                DeclaredReferenceCapability.ReadOnly => ReferenceCapability.ReadOnly,
                _ => throw ExhaustiveMatch.Failed(capability.Declared),
            };
        }
    }
}
