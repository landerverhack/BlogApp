namespace BlogApp.Models;

/// <summary>
/// Request model for executing a system command.
/// </summary>
public record CommandRequest
{
    /// <summary>
    /// The command to execute (e.g., "dir" or "/bin/ls")
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// Optional command arguments
    /// </summary>
    public string? Arguments { get; init; }

    /// <summary>
    /// Optional working directory for the process
    /// </summary>
    public string? WorkingDirectory { get; init; }

    /// <summary>
    /// Timeout in milliseconds for process execution (default: 30000ms)
    /// </summary>
    public int TimeoutMs { get; init; } = 30000;
}
