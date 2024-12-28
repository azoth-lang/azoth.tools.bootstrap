# Azoth Compiler Knowledge

## Project Overview

- Compiler for the Azoth language
- Currently runs as a tree interpreter
- Goal: Implement basic object-oriented subset similar to C# 2.0 feature set (i.e. including generics)
- The design follows a Rewritable Reference Attribute Grammar (ReRAG) approach
  - Tree nodes have attributes which are computed on demand and cached when appropriate
  - Attributes can be marked cyclic and will then evaluate cycles to a fixed point
  - Code generation is used to make this approach feasible
    - *.tree files generate tree node
    - *.aspect files add attributes to the tree node and generate partial classes for the methods for those attributes

## Key Principles

- Make Azoth source code as correct as possible given current language design
- Ensure language features are documented in language reference
- Implement standard library features as compiler intrinsics initially
- Replace intrinsics with correct standard library code when possible

## Development Guidelines

- Keep namespace hierarchies flat to avoid excessive `using` statements
- Use "Named" instead of "Nominal" for clarity
- Avoid reference cycles in design
- Strong typing throughout the codebase

## Project Structure

- Framework: utility code and code that is not specific to the project
- Core: Core compiler functionality including the attribute grammar framework, diagnostics, enums
- Types: Type system implementation
- Symbols: Symbol management (name + type pairing)
- Parsing: The parser which generates a syntax tree
- Syntax: The nodes for the syntax tree
- Semantics: Semantic analysis, including semantic tree nodes
- API: Public interface to the compiler
- CodeGen: Code generation used to generate tree nodes and aspects

## Current Focus

Implementing basic object-oriented subset including:

- Stand-alone Functions (no generics)
- Simple Types (int, uint, size, offset)
- Simple Class Declarations
- Fields, Constructors, Basic Methods
- Optional Types
- Basic Control Flow
- Namespaces
- Basic List[T]

## Excluded Features

- Loop Labels
- Document Comment Validation
- Full Type Inference

## Testing

Tests are organized in projects named `Tests.Unit.*`
