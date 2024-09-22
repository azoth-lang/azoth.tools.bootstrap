using System;
using System.Collections.Concurrent;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class VTable
{
    public IClassDefinitionNode Class { get; }
    private readonly MethodSignatureCache methodSignatures;
    private readonly FixedDictionary<UserTypeSymbol, ITypeDefinitionNode> types;
    private readonly ConcurrentDictionary<MethodSignature, IMethodDefinitionNode> methods = new();

    public VTable(
        IClassDefinitionNode @class,
        MethodSignatureCache methodSignatures,
        FixedDictionary<UserTypeSymbol, ITypeDefinitionNode> types)
    {
        Class = @class;
        this.methodSignatures = methodSignatures;
        this.types = types;
    }

    public IMethodDefinitionNode this[MethodSignature signature]
        => methods.GetOrAdd(signature, LookupMethod);

    private IMethodDefinitionNode LookupMethod(MethodSignature signature)
    {
        var method = LookupMethod(Class, signature);
        if (method is not null) return method;

        throw new InvalidOperationException($"No method found for {signature} on {Class}");
    }

    private IMethodDefinitionNode? LookupMethod(ITypeDefinitionNode type, MethodSignature signature)
    {
        foreach (var method in type.Members.OfType<IMethodDefinitionNode>())
            if (methodSignatures[method.Symbol.Assigned()].EqualsOrOverrides(signature))
                return method;

        foreach (var supertypeName in type.AllSupertypeNames)
        {
            var supertype = types[(UserTypeSymbol)supertypeName.ReferencedDeclaration!.Symbol];
            var method = LookupMethod(supertype, signature);
            if (method is not null) return method;
        }

        return null;
    }
}
