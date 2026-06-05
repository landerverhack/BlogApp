namespace BlogApp.Models;

public record class Comment
{
    public int Id { get; init; }
    public required string Content { get; init; }
    public int PostId { get; init; }
    public required Post Post { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
