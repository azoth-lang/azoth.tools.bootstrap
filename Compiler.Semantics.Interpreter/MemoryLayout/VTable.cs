using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class VTable : TypeLayout
{
    public IClassDefinitionNode Class { [DebuggerStepThrough] get; }
    private readonly MethodSignatureCache methodSignatures;
    private readonly FrozenDictionary<OrdinaryTypeSymbol, ITypeDefinitionNode> types;
    private readonly ConcurrentDictionary<MethodSignature, IMethodDefinitionNode> methods = new();

    public VTable(
        IClassDefinitionNode @class,
        MethodSignatureCache methodSignatures,
        FrozenDictionary<OrdinaryTypeSymbol, ITypeDefinitionNode> types)
        : base(@class, 2)
    {
        Class = @class;
        this.methodSignatures = methodSignatures;
        this.types = types;
    }

    public IMethodDefinitionNode this[MethodSignature signature]
    {
        [Inline(InlineBehavior.Remove)]
        get => methods.GetOrAdd(signature, LookupMethod);
    }

    public AzothValue[] CreateInstanceFields(BareType bareType)
    {
        var fields = base.CreateInstanceFields();
        fields[1] = AzothValue.BareType(bareType);
        return fields;
    }

    private IMethodDefinitionNode LookupMethod(MethodSignature signature)
    {
        var method = LookupMethod(Class, signature);
        if (method is null)
            throw new InvalidOperationException($"No method found for {signature} on {Class}");
#if DEBUG
        if (method.Body is null)
            throw new InvalidOperationException("Method lookup should not resolve to abstract method.");
#endif
        return method;
    }

    private IMethodDefinitionNode? LookupMethod(ITypeDefinitionNode type, MethodSignature signature)
    {
        foreach (var method in type.Members.OfType<IMethodDefinitionNode>())
            if (methodSignatures[method.Symbol.Assigned()].EqualsOrOverrides(signature))
                return method;

        foreach (var supertypeName in type.AllSupertypeNames)
        {
            var supertype = types[(OrdinaryTypeSymbol)supertypeName.ReferencedDeclaration!.Symbol];
            var method = LookupMethod(supertype, signature);
            if (method is not null) return method;
        }

        return null;
    }
}
