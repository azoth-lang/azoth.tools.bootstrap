using System;
using System.Collections.Concurrent;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal class VTable
{
    private readonly MethodSignatureCache methodSignatures;
    public IClassDeclaration Class { get; }
    private readonly ConcurrentDictionary<MethodSignature, IMethodDeclaration> methods = new();

    public VTable(IClassDeclaration @class, MethodSignatureCache methodSignatures)
    {
        this.methodSignatures = methodSignatures;
        Class = @class;
    }

    public IMethodDeclaration this[MethodSignature signature]
        => methods.GetOrAdd(signature, LookupMethod);

    private IMethodDeclaration LookupMethod(MethodSignature signature)
    {
        var method = LookupMethod(Class, signature);
        if (method is not null) return method;

        throw new InvalidOperationException($"No method found for {signature} on {Class}");
    }

    private IMethodDeclaration? LookupMethod(ITypeDeclaration type, MethodSignature signature)
    {
        foreach (var method in type.Members.OfType<IMethodDeclaration>())
            if (methodSignatures[method.Symbol].EqualsOrOverrides(signature))
                return method;

        foreach (var supertype in type.Supertypes)
        {
            var method = LookupMethod(supertype, signature);
            if (method is not null) return method;
        }

        if (type is IClassDeclaration { BaseClass: not null and var baseClass })
        {
            var method = LookupMethod(baseClass, signature);
            if (method is not null) return method;
        }

        return null;
    }
}
