# Add Entity Checklist

Use this checklist to verify a new entity is correctly implemented before committing.

## Entity Class (`Models/<EntityName>.cs`)

- [ ] File created in `BlogApp/Models/`
- [ ] Uses `record class` (not `class`)
- [ ] All properties use `{ get; init; }` (immutable)
- [ ] Has `public int Id { get; init; }` primary key
- [ ] Required string properties initialized to `string.Empty`
- [ ] Navigation properties initialized to `null!`
- [ ] Timestamp properties default to `DateTime.UtcNow`
- [ ] Namespace is `BlogApp.Models`

## Configuration Class (`Data/Configurations/<EntityName>Configuration.cs`)

- [ ] File created in `BlogApp/Data/Configurations/`
- [ ] Implements `IEntityTypeConfiguration<EntityName>`
- [ ] Class name ends with `Configuration` (e.g., `CommentConfiguration`)
- [ ] Has `Configure(EntityTypeBuilder<EntityName> builder)` method
- [ ] Calls `builder.HasKey(x => x.Id)`
- [ ] All required strings have `.IsRequired()` and `.HasMaxLength(...)`
- [ ] Relationships use `.HasOne()`, `.WithMany()`, `.HasForeignKey()`
- [ ] Foreign key relationships specify `.OnDelete(DeleteBehavior.Cascade)` or appropriate behavior
- [ ] Namespace is `BlogApp.Data.Configurations`

## AppDbContext Registration

- [ ] DbSet property added: `public DbSet<EntityName> EntityNames => Set<EntityName>();`
- [ ] Property uses expression-body syntax (`=>` not `{ get; }`)
- [ ] `OnModelCreating` includes `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);`
- [ ] No direct configuration on entity class—all in Fluent API

## Documentation (`Readme.md`)

- [ ] "## Entities" section exists (created if needed)
- [ ] Entity added to table with columns: Entity | Properties | Description
- [ ] One-line description is clear and concise
- [ ] Properties listed include: Id, all main properties, navigation properties

## Type Safety & SQLite Compatibility

- [ ] All properties use SQLite-compatible types: `int`, `string`, `bool`, `DateTime`, `decimal`
- [ ] No arrays, lists, or complex types in entity
- [ ] Primary key is `int` (auto-increment compatible with SQLite)
- [ ] String lengths specified with `.HasMaxLength()` for all required strings

## Code Review

- [ ] Entity follows [C# best practices](./../instructions/csharp-best-practices.instructions.md)
- [ ] No data attributes (no `[Required]`, `[MaxLength]`, etc.) — all constraints in Fluent API
- [ ] Code compiles without errors
- [ ] Related entities are properly configured (no missing `.HasOne()` or `.WithMany()`)
