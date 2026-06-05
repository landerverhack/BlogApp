---
description: "Use when writing C# code, creating classes, models, DTOs, domain types, value objects, or designing data structures. Enforces records, immutability, value object patterns, and modern C# conventions."
applyTo: "**/*.cs"
---

# C# Best Practices

## Prefer Records Over Classes

- **Always use `record` or `record class`** for data-carrying types: DTOs, models, query results, API responses.
- Use `record struct` for small, stack-allocated value types (coordinates, ranges, ids).
- Only use a regular `class` when the type has meaningful mutable state, lifecycle methods (e.g., services, repositories, controllers), or requires inheritance that records don't support.

```csharp
// Prefer this
public record Post(int Id, string Title, string Body, DateTime PublishedAt);

// Over this
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime PublishedAt { get; set; }
}
```

## Value Objects

- Wrap primitives with domain meaning in a **Value Object** — a `record` with validation logic.
- A Value Object is immutable and equality is based on its value, not identity.
- Validate in the constructor or use a static factory method that returns `Result<T>` or throws on invalid input.
- Common candidates: `Email`, `Title`, `Slug`, `Money`, `DateRange`, `PostId`.

```csharp
public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("Invalid email address.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
}
```

## Immutability

- Prefer `init`-only properties over `set` on records and classes.
- Use `with` expressions to derive modified copies of records instead of mutating state.

```csharp
var updated = original with { Title = "New Title" };
```

## Nullability

- Enable nullable reference types (`<Nullable>enable</Nullable>`) in all projects.
- Use `string?` only when `null` is a valid domain state. Prefer `string.Empty` over `null` for missing text.
- Annotate all parameters and return types explicitly.

## Naming & Conventions

- `record` names: PascalCase nouns (`Post`, `CreatePostCommand`, `PostSummary`).
- Value Objects: named after the domain concept, not the primitive (`Email`, not `EmailString`).
- Avoid suffix noise: `PostDto` is acceptable for transport types; avoid `PostData`, `PostInfo`, `PostModel` inconsistencies — pick one convention per layer and stick to it.

## General C# Best Practices

- Use `required` modifier on record properties when they must always be set.
- Prefer `IReadOnlyList<T>` / `IReadOnlyCollection<T>` over `List<T>` in public APIs.
- Use `sealed` on records and classes that are not designed for inheritance.
- Use `file`-scoped namespaces (`namespace MyApp.Models;` instead of block-scoped).
- Prefer primary constructors for simple dependency injection in services.
- Use pattern matching (`is`, `switch` expressions) over casting and chains of `if`/`else if`.
- Avoid `static` mutable state; prefer dependency injection.
