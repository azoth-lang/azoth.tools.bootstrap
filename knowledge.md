# Azoth Bootstrap Compiler Knowledge

## Project Overview
- Bootstrap compiler for the Azoth language
- Currently runs as a tree interpreter
- Project Status: Alpha Active
- Goal: Implement basic object-oriented subset similar to Java 1.0 feature set

## Key Principles
- Make Azoth source code as correct as possible given current language design
- Ensure language features are documented in language reference
- Implement standard library features as compiler intrinsics initially
- Replace intrinsics with correct standard library code when possible

## Development Guidelines
- Keep namespace hierarchies flat to avoid excessive `using` statements
- Don't prefix interfaces used as traits with "I" in Types project
- Use "Named" instead of "Nominal" for clarity
- Avoid reference cycles in design
- Strong typing throughout the codebase

## Project Structure
- Core: Basic utilities and shared code
- Types: Type system implementation
- Symbols: Symbol management (name + type pairing)
- Semantics: Semantic analysis
- API: Public interface to the compiler

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
