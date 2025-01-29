# Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter


## Performance

Because performance is very important in the interpreter a number of odd or verbose things have been
done.

* When checking types against simple types for operations, this is done using reference equality on
  the plain type. This avoids repeated calls to `.Equals()` for each of the different types.
* More frequent use of `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
