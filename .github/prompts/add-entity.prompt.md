---
name: "Add Entity"
description: "Add a new EF Core entity to the BlogApp project: entity class in Models/, separate Fluent API mapping configuration, DbSet registration in AppDbContext, and Readme update."
argument-hint: "Entity name and its properties, e.g. 'Comment with AuthorName (string), Body (string), PostId (int), CreatedAt (DateTime)'"
agent: "agent"
---

Add a new EF Core entity to this Blazor/SQLite project. Follow all steps below exactly.

## Inputs

The entity to add: **${}** (replace with the argument provided above).

## Rules

- Follow [C# best practices](./../instructions/csharp-best-practices.instructions.md): use `record class` for the entity; `init`-only properties.
- Entity class goes in `BlogApp/Models/`.
- Mapping configuration goes in `BlogApp/Data/Configurations/` as a separate class implementing `IEntityTypeConfiguration<T>` — **no data attributes on the entity**.
- Use **Fluent API only** inside the `Configure` method.
- Register the `DbSet<T>` in `BlogApp/Data/AppDbContext.cs` using the expression-body pattern already present (`public DbSet<T> Xs => Set<T>();`).
- Apply configurations in `OnModelCreating` inside `AppDbContext` using `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);` — add this call if not already present.
- All string properties that are required must be configured as `.IsRequired()` and include a reasonable `.HasMaxLength(...)`.
- SQLite-compatible types only (TEXT, INTEGER, REAL, BLOB). Use `int` PKs with auto-increment.
- Update [Readme.md](./../../Readme.md): add the new entity to an "## Entities" section (create it if absent). List each entity with its properties and a one-line description.

## Steps

1. Create `BlogApp/Models/<EntityName>.cs` — the entity record.
2. Create `BlogApp/Data/Configurations/<EntityName>Configuration.cs` — the Fluent API configuration.
3. Edit `BlogApp/Data/AppDbContext.cs` — add `DbSet` and ensure `ApplyConfigurationsFromAssembly` is called.
4. Edit `Readme.md` — update or create the "## Entities" section.

## Example Output

### Entity (`BlogApp/Models/Comment.cs`)
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

### Configuration (`BlogApp/Data/Configurations/CommentConfiguration.cs`)
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

### AppDbContext addition
```csharp
public DbSet<Comment> Comments => Set<Comment>();
```

### Readme "## Entities" entry
```markdown
## Entities

| Entity | Properties | Description |
|--------|-----------|-------------|
| Post | Id, Title, Content, CreatedAt | A blog post with title and body content. |
| Comment | Id, AuthorName, Body, PostId, CreatedAt | A reader comment attached to a Post. |
```
