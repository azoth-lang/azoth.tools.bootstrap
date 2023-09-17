using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

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
        foreach (var method in Class.Members.OfType<IMethodDeclaration>())
            if (methodSignatures[method.Symbol].EqualsOrOverrides(signature))
                return method;
        throw new InvalidOperationException($"No method found for {signature} on {Class}");
    }
}
