---
name: add-entity
description: "Add a new EF Core entity to the BlogApp project. Use when you need to: add a new model, create an entity, add a database table, extend the domain model, or register a new entity in DbContext. Creates the entity class, Fluent API configuration, DbSet registration, and documentation automatically."
argument-hint: "Entity name and properties, e.g. 'Comment with AuthorName (string), Body (string), PostId (int), CreatedAt (DateTime)'"
user-invocable: true
---

# Add Entity

## When to Use

- Add a new domain model to the BlogApp project
- Create a new database entity (table)
- Extend the data model with additional entities
- Register a new type with EF Core
- Add database persistence for a new concept

## Workflow Overview

The skill creates four artifacts:
1. **Entity record** in `BlogApp/Models/` — immutable domain class
2. **Configuration** in `BlogApp/Data/Configurations/` — Fluent API constraints
3. **DbSet registration** in `BlogApp/Data/AppDbContext.cs` — query access
4. **Documentation** in `Readme.md` — entity catalog

## Prerequisites

- BlogApp is open in VS Code
- Follow [C# best practices](./../instructions/csharp-best-practices.instructions.md)

## Step-by-Step Procedure

### 1. Define the Entity

Create `BlogApp/Models/<EntityName>.cs` as a `record class` with:
- `Id` property (int, auto-increment primary key)
- Required properties with `init` accessors (read-only after construction)
- String defaults set to `string.Empty`
- Navigation properties for relationships (e.g., `Post Post { get; init; }`)
- Timestamp properties default to `DateTime.UtcNow`

**Template:**
```csharp
namespace BlogApp.Models;

public record class EntityName
{
    public int Id { get; init; }
    public string PropertyName { get; init; } = string.Empty;
    public int RelatedEntityId { get; init; }
    public RelatedEntity RelatedEntity { get; init; } = null!;
}
```

### 2. Create Fluent API Configuration

Create `BlogApp/Data/Configurations/<EntityName>Configuration.cs` implementing `IEntityTypeConfiguration<T>`.

Configure in the `Configure` method:
- `.HasKey()` — primary key
- `.Property()` — constraints on each property
  - `.IsRequired()` — non-nullable strings and foreign keys
  - `.HasMaxLength(...)` — reasonable string limits
- `.HasOne()` / `.WithMany()` — relationships
- `.HasForeignKey()` — explicit FK references
- `.OnDelete(DeleteBehavior.Cascade)` — cascading deletes

**Template:**
```csharp
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Data.Configurations;

public class EntityNameConfiguration : IEntityTypeConfiguration<EntityName>
{
    public void Configure(EntityTypeBuilder<EntityName> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PropertyName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(e => e.RelatedEntity)
            .WithMany()
            .HasForeignKey(e => e.RelatedEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 3. Register in AppDbContext

In `BlogApp/Data/AppDbContext.cs`:

**Add a DbSet property** using the expression-body pattern:
```csharp
public DbSet<EntityName> EntityNames => Set<EntityName>();
```

**Ensure configuration auto-loading** in `OnModelCreating`:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
```

(Add the `ApplyConfigurationsFromAssembly` call if not already present.)

### 4. Update Documentation

Edit `Readme.md` and add or update the "## Entities" section with a table row:

```markdown
## Entities

| Entity | Properties | Description |
|--------|-----------|-------------|
| EntityName | Id, PropertyName, RelatedEntityId | Brief one-line description. |
```

## Constraints & Rules

- **SQLite-compatible types only**: Use `int`, `string`, `DateTime`, `bool`, `decimal` — no arrays or custom types
- **Primary keys**: Always `int` with auto-increment pattern
- **String properties**: Required strings must have `.IsRequired()` and `.HasMaxLength(...)`
- **No data attributes**: Keep entities clean — all constraints go in Fluent API configuration
- **Immutable properties**: Use `init` accessors, not `set`; initialize required strings to `string.Empty`
- **Foreign keys**: Explicit `HasForeignKey()` calls in configuration, with corresponding navigation property in entity

## Quality Checklist

Before finishing, verify all items in the [Add Entity Checklist](./references/CHECKLIST.md).

## Example: Adding a Comment Entity

**Input:** `Comment with AuthorName (string), Body (string), PostId (int), CreatedAt (DateTime)`

**Output:**

`BlogApp/Models/Comment.cs`:
```csharp
namespace BlogApp.Models;

public record class Comment
{
    public int Id { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public int PostId { get; init; }
    public Post Post { get; init; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
```

`BlogApp/Data/Configurations/CommentConfiguration.cs`:
```csharp
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.AuthorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Body)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(c => c.Post)
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

`Readme.md` — Entities section updated:
```markdown
| Comment | Id, AuthorName, Body, PostId, Post, CreatedAt | A reader comment attached to a Post. |
```

## Related Skills & Customizations

- **Lab Instructions**: Document this entity-adding workflow as a student exercise
- **Add Endpoint Skill**: Create API endpoints (GET, POST, DELETE) for the entity
