namespace BlogApp.Models;

public record class Comment
{
    public int Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public int PostId { get; init; }
    public Post Post { get; init; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
